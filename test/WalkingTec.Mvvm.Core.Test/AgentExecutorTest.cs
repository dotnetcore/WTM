using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WalkingTec.Mvvm.Core.Support.Agent;

namespace WalkingTec.Mvvm.Core.Test
{
    /// <summary>
    /// Agent编排引擎（AgentExecutor）的单元测试：
    /// 覆盖模板渲染、LLM/知识库/API/条件/回复各节点、分支选择、
    /// 成环保护以及流程图JSON的序列化往返
    /// </summary>
    [TestClass]
    public class AgentExecutorTest
    {
        /// <summary>
        /// 构造一个节点
        /// </summary>
        private AgentGraphNode Node(string id, string type, string config = null, double x = 0, double y = 0)
        {
            return new AgentGraphNode { Id = id, Type = type, Name = id, X = x, Y = y, ConfigJson = config };
        }

        /// <summary>
        /// 构造一条连线
        /// </summary>
        private AgentGraphEdge Edge(string source, string target, string handle = null)
        {
            return new AgentGraphEdge { Id = $"{source}-{handle ?? ""}-{target}", Source = source, Target = target, SourceHandle = handle };
        }

        private string Config(object cfg)
        {
            return JsonSerializer.Serialize(cfg, AgentGraph.JsonOptions);
        }

        /// <summary>
        /// 只包含模板能力的空服务（纯模板流程测试用）
        /// </summary>
        private AgentRuntimeServices EmptyServices()
        {
            return new AgentRuntimeServices();
        }

        [TestMethod]
        public void GraphJsonRoundTripTest()
        {
            var graph = new AgentGraph();
            graph.Nodes.Add(Node("n1", AgentNodeTypes.Start, Config(new StartConfig { Variables = new List<string> { "query" } })));
            graph.Nodes.Add(Node("n2", AgentNodeTypes.LLM, Config(new LlmConfig { UserPrompt = "{{n1.query}}" })));
            graph.Edges.Add(Edge("n1", "n2"));

            var json = graph.ToJson();
            var parsed = AgentGraph.Parse(json);

            Assert.AreEqual(2, parsed.Nodes.Count);
            Assert.AreEqual(1, parsed.Edges.Count);
            Assert.AreEqual(AgentNodeTypes.LLM, parsed.Nodes[1].Type);
            var llm = AgentNodeConfigResolver.Resolve<LlmConfig>(parsed.Nodes[1]);
            Assert.AreEqual("{{n1.query}}", llm.UserPrompt);
        }

        [TestMethod]
        public async Task StartToAnswerTemplateTest()
        {
            var graph = new AgentGraph();
            graph.Nodes.Add(Node("start1", AgentNodeTypes.Start, Config(new StartConfig { Variables = new List<string> { "name", "city" } })));
            graph.Nodes.Add(Node("end1", AgentNodeTypes.Answer, Config(new AnswerConfig { Text = "你好，{{start1.name}}，欢迎来到{{start1.city}}！" })));
            graph.Edges.Add(Edge("start1", "end1"));

            var result = await AgentExecutor.RunAsync(graph, new Dictionary<string, string>
            {
                ["name"] = "张三",
                ["city"] = "北京"
            }, EmptyServices());

            Assert.AreEqual(AgentRunStatusEnum.Success, result.Status);
            Assert.AreEqual("你好，张三，欢迎来到北京！", result.Output);
            Assert.AreEqual(2, result.NodeLogs.Count);
            Assert.IsTrue(result.NodeLogs.All(x => x.Success));
        }

        [TestMethod]
        public async Task UnresolvedVariableReplacedWithEmptyTest()
        {
            var graph = new AgentGraph();
            graph.Nodes.Add(Node("start1", AgentNodeTypes.Start));
            graph.Nodes.Add(Node("end1", AgentNodeTypes.Answer, Config(new AnswerConfig { Text = "a[{{start1.nothere}}]b[{{othernode.x}}]" })));
            graph.Edges.Add(Edge("start1", "end1"));

            var result = await AgentExecutor.RunAsync(graph, new Dictionary<string, string>(), EmptyServices());

            Assert.AreEqual(AgentRunStatusEnum.Success, result.Status);
            Assert.AreEqual("a[]b[]", result.Output);
        }

        [TestMethod]
        public async Task LlmNodeTest()
        {
            AgentLlmRequest captured = null;
            var services = new AgentRuntimeServices
            {
                Chat = async req =>
                {
                    captured = req;
                    await Task.Yield();
                    return $"回答：{req.User}";
                }
            };
            var graph = new AgentGraph();
            graph.Nodes.Add(Node("start1", AgentNodeTypes.Start));
            graph.Nodes.Add(Node("llm1", AgentNodeTypes.LLM, Config(new LlmConfig
            {
                SystemPrompt = "你是助手",
                UserPrompt = "问题：{{start1.query}}",
                Temperature = 0.7,
                MaxTokens = 100
            })));
            graph.Nodes.Add(Node("end1", AgentNodeTypes.Answer, Config(new AnswerConfig { Text = "LLM说：{{llm1}}" })));
            graph.Edges.Add(Edge("start1", "llm1"));
            graph.Edges.Add(Edge("llm1", "end1"));

            var result = await AgentExecutor.RunAsync(graph, new Dictionary<string, string> { ["query"] = "什么是WTM" }, services);

            Assert.AreEqual(AgentRunStatusEnum.Success, result.Status);
            Assert.AreEqual("你是助手", captured.System);
            Assert.AreEqual("问题：什么是WTM", captured.User);
            Assert.AreEqual(0.7, captured.Temperature);
            Assert.AreEqual(100, captured.MaxTokens);
            Assert.AreEqual("LLM说：回答：问题：什么是WTM", result.Output);
        }

        [TestMethod]
        public async Task LlmNodeWithoutPromptFailsTest()
        {
            var services = new AgentRuntimeServices { Chat = req => Task.FromResult("ok") };
            var graph = new AgentGraph();
            graph.Nodes.Add(Node("start1", AgentNodeTypes.Start));
            graph.Nodes.Add(Node("llm1", AgentNodeTypes.LLM, Config(new LlmConfig { UserPrompt = "" })));
            graph.Edges.Add(Edge("start1", "llm1"));

            var result = await AgentExecutor.RunAsync(graph, new Dictionary<string, string>(), services);

            Assert.AreEqual(AgentRunStatusEnum.Failed, result.Status);
            StringAssert.Contains(result.Error, "用户提示词不能为空");
        }

        [TestMethod]
        public async Task ConditionBranchTrueFalseTest()
        {
            var graph = new AgentGraph();
            graph.Nodes.Add(Node("start1", AgentNodeTypes.Start));
            graph.Nodes.Add(Node("cond1", AgentNodeTypes.Condition, Config(new ConditionConfig
            {
                Left = "{{start1.query}}",
                Operator = ConditionOperator.Contains,
                Right = "重要"
            })));
            graph.Nodes.Add(Node("endYes", AgentNodeTypes.Answer, Config(new AnswerConfig { Text = "走重要分支" })));
            graph.Nodes.Add(Node("endNo", AgentNodeTypes.Answer, Config(new AnswerConfig { Text = "走普通分支" })));
            graph.Edges.Add(Edge("start1", "cond1"));
            graph.Edges.Add(Edge("cond1", "endYes", "true"));
            graph.Edges.Add(Edge("cond1", "endNo", "false"));

            var yes = await AgentExecutor.RunAsync(graph, new Dictionary<string, string> { ["query"] = "这是个重要的事情" }, EmptyServices());
            Assert.AreEqual("走重要分支", yes.Output);

            var no = await AgentExecutor.RunAsync(graph, new Dictionary<string, string> { ["query"] = "随便问问" }, EmptyServices());
            Assert.AreEqual("走普通分支", no.Output);
        }

        [TestMethod]
        public async Task ConditionNumericCompareTest()
        {
            var graph = new AgentGraph();
            graph.Nodes.Add(Node("start1", AgentNodeTypes.Start));
            graph.Nodes.Add(Node("cond1", AgentNodeTypes.Condition, Config(new ConditionConfig
            {
                Left = "{{start1.score}}",
                Operator = ConditionOperator.GreaterThan,
                Right = "60"
            })));
            graph.Nodes.Add(Node("endYes", AgentNodeTypes.Answer, Config(new AnswerConfig { Text = "及格" })));
            graph.Nodes.Add(Node("endNo", AgentNodeTypes.Answer, Config(new AnswerConfig { Text = "不及格" })));
            graph.Edges.Add(Edge("start1", "cond1"));
            graph.Edges.Add(Edge("cond1", "endYes", "true"));
            graph.Edges.Add(Edge("cond1", "endNo", "false"));

            var pass = await AgentExecutor.RunAsync(graph, new Dictionary<string, string> { ["score"] = "95" }, EmptyServices());
            Assert.AreEqual("及格", pass.Output);

            var fail = await AgentExecutor.RunAsync(graph, new Dictionary<string, string> { ["score"] = "42" }, EmptyServices());
            Assert.AreEqual("不及格", fail.Output);
        }

        [TestMethod]
        public async Task ApiNodeJsonPathTest()
        {
            var services = new AgentRuntimeServices
            {
                HttpSend = (msg, ct) => Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("{\"code\":0,\"data\":{\"city\":\"北京\",\"days\":[\"周一\",\"周二\"]}}")
                })
            };
            var graph = new AgentGraph();
            graph.Nodes.Add(Node("start1", AgentNodeTypes.Start));
            graph.Nodes.Add(Node("api1", AgentNodeTypes.Api, Config(new ApiConfig
            {
                Method = "POST",
                Url = "http://test/api?name={{start1.name}}",
                Body = "{\"q\":\"{{start1.name}}\"}"
            })));
            graph.Nodes.Add(Node("end1", AgentNodeTypes.Answer, Config(new AnswerConfig { Text = "城市={{api1.json.data.city}}，第2天={{api1.json.data.days[1]}}，原文={{api1.body}}" })));
            graph.Edges.Add(Edge("start1", "api1"));
            graph.Edges.Add(Edge("api1", "end1"));

            var result = await AgentExecutor.RunAsync(graph, new Dictionary<string, string> { ["name"] = "wtm" }, services);

            Assert.AreEqual(AgentRunStatusEnum.Success, result.Status);
            StringAssert.Contains(result.Output, "城市=北京");
            StringAssert.Contains(result.Output, "第2天=周二");
            StringAssert.Contains(result.Output, "原文=");
        }

        [TestMethod]
        public async Task ApiNodeHttpErrorFailsTest()
        {
            var services = new AgentRuntimeServices
            {
                HttpSend = (msg, ct) => Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Content = new StringContent("boom")
                })
            };
            var graph = new AgentGraph();
            graph.Nodes.Add(Node("start1", AgentNodeTypes.Start));
            graph.Nodes.Add(Node("api1", AgentNodeTypes.Api, Config(new ApiConfig { Url = "http://test/api" })));
            graph.Edges.Add(Edge("start1", "api1"));

            var result = await AgentExecutor.RunAsync(graph, new Dictionary<string, string>(), services);

            Assert.AreEqual(AgentRunStatusEnum.Failed, result.Status);
            StringAssert.Contains(result.Error, "API调用失败");
        }

        [TestMethod]
        public async Task NoStartNodeFailsTest()
        {
            //无start类型节点且所有节点都有入边（纯环）时找不到起点
            var graph = new AgentGraph();
            graph.Nodes.Add(Node("a1", AgentNodeTypes.LLM, Config(new LlmConfig { UserPrompt = "x" })));
            graph.Nodes.Add(Node("a2", AgentNodeTypes.LLM, Config(new LlmConfig { UserPrompt = "y" })));
            graph.Edges.Add(Edge("a1", "a2"));
            graph.Edges.Add(Edge("a2", "a1"));

            var result = await AgentExecutor.RunAsync(graph, new Dictionary<string, string>(),
                new AgentRuntimeServices { Chat = req => Task.FromResult("ok") });

            Assert.AreEqual(AgentRunStatusEnum.Failed, result.Status);
            StringAssert.Contains(result.Error, "开始节点");
        }

        [TestMethod]
        public async Task SingleNodeWithoutIncomingStartsTest()
        {
            //无start类型节点但存在唯一无入边节点时，该节点作为起点执行
            var graph = new AgentGraph();
            graph.Nodes.Add(Node("end1", AgentNodeTypes.Answer, Config(new AnswerConfig { Text = "直接输出" })));

            var result = await AgentExecutor.RunAsync(graph, new Dictionary<string, string>(), EmptyServices());

            Assert.AreEqual(AgentRunStatusEnum.Success, result.Status);
            Assert.AreEqual("直接输出", result.Output);
        }

        [TestMethod]
        public async Task CycleGuardTest()
        {
            //两个恒为真的条件节点互相指向，触发最大步数保护
            var graph = new AgentGraph();
            graph.Nodes.Add(Node("start1", AgentNodeTypes.Start));
            graph.Nodes.Add(Node("c1", AgentNodeTypes.Condition, Config(new ConditionConfig { Left = "1", Operator = ConditionOperator.Equals, Right = "1" })));
            graph.Nodes.Add(Node("c2", AgentNodeTypes.Condition, Config(new ConditionConfig { Left = "1", Operator = ConditionOperator.Equals, Right = "1" })));
            graph.Edges.Add(Edge("start1", "c1"));
            graph.Edges.Add(Edge("c1", "c2", "true"));
            graph.Edges.Add(Edge("c2", "c1", "true"));

            var result = await AgentExecutor.RunAsync(graph, new Dictionary<string, string>(), EmptyServices());

            Assert.AreEqual(AgentRunStatusEnum.Failed, result.Status);
            StringAssert.Contains(result.Error, "上限");
        }

        [TestMethod]
        public async Task KnowledgeNodeTest()
        {
            //使用内置向量服务准备两篇文档的分块向量
            var embedding = new BuiltInEmbeddingService();
            byte[] ToBytes(string text)
            {
                var vec = embedding.GetEmbeddingAsync(text).Result;
                var bytes = new byte[vec.Length * sizeof(float)];
                System.Buffer.BlockCopy(vec, 0, bytes, 0, bytes.Length);
                return bytes;
            }

            var dc = new FrameworkContext(Guid.NewGuid().ToString(), DBTypeEnum.Memory);
            var doc1 = new KnowledgeDoc { ID = Guid.NewGuid(), Title = "WTM文档", Status = DocStatusEnum.Completed };
            var doc2 = new KnowledgeDoc { ID = Guid.NewGuid(), Title = "天气文档", Status = DocStatusEnum.Completed };
            dc.Set<KnowledgeDoc>().Add(doc1);
            dc.Set<KnowledgeDoc>().Add(doc2);
            dc.Set<KnowledgeChunk>().AddRange(new[]
            {
                new KnowledgeChunk { ID = Guid.NewGuid(), DocId = doc1.ID, Doc = doc1, Index = 0, Content = "wtm 是一个快速开发框架 walkingtec mvvm", Embedding = ToBytes("wtm 是一个快速开发框架 walkingtec mvvm"), EmbeddingDim = embedding.Dimension },
                new KnowledgeChunk { ID = Guid.NewGuid(), DocId = doc2.ID, Doc = doc2, Index = 0, Content = "zzz xxx qqq 罕见词组合", Embedding = ToBytes("zzz xxx qqq 罕见词组合"), EmbeddingDim = embedding.Dimension }
            });
            dc.SaveChanges();

            var services = new AgentRuntimeServices
            {
                Embedding = text => embedding.GetEmbeddingAsync(text),
                Chunks = dc.Set<KnowledgeChunk>().AsNoTracking()
            };
            var graph = new AgentGraph();
            graph.Nodes.Add(Node("start1", AgentNodeTypes.Start));
            graph.Nodes.Add(Node("kb1", AgentNodeTypes.Knowledge, Config(new KnowledgeConfig
            {
                Query = "{{start1.query}}",
                TopK = 1,
                Threshold = 0.05
            })));
            graph.Nodes.Add(Node("end1", AgentNodeTypes.Answer, Config(new AnswerConfig { Text = "检索结果：{{kb1.text}}" })));
            graph.Edges.Add(Edge("start1", "kb1"));
            graph.Edges.Add(Edge("kb1", "end1"));

            var result = await AgentExecutor.RunAsync(graph, new Dictionary<string, string> { ["query"] = "wtm 快速开发框架" }, services);

            Assert.AreEqual(AgentRunStatusEnum.Success, result.Status);
            StringAssert.Contains(result.Output, "检索结果：");
            StringAssert.Contains(result.Output, "walkingtec mvvm");
        }

        [TestMethod]
        public async Task KnowledgeDocFilterTest()
        {
            var embedding = new BuiltInEmbeddingService();
            byte[] ToBytes(string text)
            {
                var vec = embedding.GetEmbeddingAsync(text).Result;
                var bytes = new byte[vec.Length * sizeof(float)];
                System.Buffer.BlockCopy(vec, 0, bytes, 0, bytes.Length);
                return bytes;
            }

            var dc = new FrameworkContext(Guid.NewGuid().ToString(), DBTypeEnum.Memory);
            var doc1 = new KnowledgeDoc { ID = Guid.NewGuid(), Title = "文档1", Status = DocStatusEnum.Completed };
            var doc2 = new KnowledgeDoc { ID = Guid.NewGuid(), Title = "文档2", Status = DocStatusEnum.Completed };
            dc.Set<KnowledgeDoc>().Add(doc1);
            dc.Set<KnowledgeDoc>().Add(doc2);
            dc.Set<KnowledgeChunk>().AddRange(new[]
            {
                new KnowledgeChunk { ID = Guid.NewGuid(), DocId = doc1.ID, Doc = doc1, Index = 0, Content = "alpha alpha content", Embedding = ToBytes("alpha alpha content"), EmbeddingDim = embedding.Dimension },
                new KnowledgeChunk { ID = Guid.NewGuid(), DocId = doc2.ID, Doc = doc2, Index = 0, Content = "beta beta content", Embedding = ToBytes("beta beta content"), EmbeddingDim = embedding.Dimension }
            });
            dc.SaveChanges();

            //只允许检索文档2
            var services = new AgentRuntimeServices
            {
                Embedding = text => embedding.GetEmbeddingAsync(text),
                Chunks = dc.Set<KnowledgeChunk>().AsNoTracking()
            };
            var graph = new AgentGraph();
            graph.Nodes.Add(Node("start1", AgentNodeTypes.Start));
            graph.Nodes.Add(Node("kb1", AgentNodeTypes.Knowledge, Config(new KnowledgeConfig
            {
                Query = "alpha",
                TopK = 5,
                Threshold = 0.01,
                DocIds = new List<Guid> { doc2.ID }
            })));
            graph.Edges.Add(Edge("start1", "kb1"));

            var result = await AgentExecutor.RunAsync(graph, new Dictionary<string, string>(), services);

            Assert.AreEqual(AgentRunStatusEnum.Success, result.Status);
            //文档1被排除后，即使相似度更高也不应出现在输出中
            var log = result.NodeLogs.First(x => x.NodeId == "kb1");
            StringAssert.DoesNotMatch(log.Output, new System.Text.RegularExpressions.Regex("alpha alpha content"));
        }
    }
}
