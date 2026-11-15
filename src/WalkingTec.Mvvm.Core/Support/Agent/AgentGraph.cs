using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WalkingTec.Mvvm.Core.Support.Agent
{
    /// <summary>
    /// 节点类型常量，前后端共用的类型标识
    /// </summary>
    public static class AgentNodeTypes
    {
        /// <summary>
        /// 开始节点：定义流程的输入变量
        /// </summary>
        public const string Start = "start";
        /// <summary>
        /// LLM节点：调用大模型生成文本
        /// </summary>
        public const string LLM = "llm";
        /// <summary>
        /// 知识库节点：按语义相似度检索知识库文档分块
        /// </summary>
        public const string Knowledge = "kb";
        /// <summary>
        /// API调用节点：调用外部HTTP接口
        /// </summary>
        public const string Api = "api";
        /// <summary>
        /// 内部API节点：调用框架自动发现的系统接口（与Agent对话工具同源）
        /// </summary>
        public const string InnerApi = "innerapi";
        /// <summary>
        /// 代码节点：执行一个C#函数（复用AI代码编译执行设施）
        /// </summary>
        public const string Code = "code";
        /// <summary>
        /// 条件分支节点：按条件走真/假两条分支
        /// </summary>
        public const string Condition = "condition";
        /// <summary>
        /// 回复节点：拼接最终输出并结束流程
        /// </summary>
        public const string Answer = "answer";
    }

    /// <summary>
    /// 流程图定义：节点列表 + 连线列表，
    /// 与前端设计器保存的JSON结构一一对应
    /// </summary>
    public class AgentGraph
    {
        /// <summary>
        /// 流程中所有节点
        /// </summary>
        public List<AgentGraphNode> Nodes { get; set; } = new List<AgentGraphNode>();
        /// <summary>
        /// 流程中所有连线
        /// </summary>
        public List<AgentGraphEdge> Edges { get; set; } = new List<AgentGraphEdge>();

        /// <summary>
        /// 供引擎和测试使用的序列化选项（驼峰、枚举字符串、忽略大小写）
        /// </summary>
        public static JsonSerializerOptions JsonOptions { get; } = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// 从JSON解析流程图
        /// </summary>
        /// <param name="json">流程图JSON</param>
        /// <returns>流程图对象</returns>
        public static AgentGraph Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new AgentGraph();
            return JsonSerializer.Deserialize<AgentGraph>(json, JsonOptions) ?? new AgentGraph();
        }

        /// <summary>
        /// 序列化为JSON
        /// </summary>
        /// <returns>流程图JSON字符串</returns>
        public string ToJson()
        {
            return JsonSerializer.Serialize(this, JsonOptions);
        }

        /// <summary>
        /// 按节点Id查找节点
        /// </summary>
        /// <param name="id">节点Id</param>
        /// <returns>节点对象，找不到返回null</returns>
        public AgentGraphNode FindNode(string id)
        {
            return Nodes.Find(n => n.Id == id);
        }

        /// <summary>
        /// 查找开始节点（类型为start，若无则取没有入边的第一个节点）
        /// </summary>
        /// <returns>开始节点，找不到返回null</returns>
        public AgentGraphNode FindStartNode()
        {
            var start = Nodes.Find(n => n.Type == AgentNodeTypes.Start);
            if (start != null)
                return start;
            var targets = new HashSet<string>(Edges.ConvertAll(e => e.Target));
            return Nodes.Find(n => !targets.Contains(n.Id));
        }
    }

    /// <summary>
    /// 流程图节点：位置供前端画布定位，ConfigJson保存该节点类型的详细配置
    /// </summary>
    public class AgentGraphNode
    {
        /// <summary>
        /// 节点唯一Id（前端生成）
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 节点类型，见 AgentNodeTypes
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 节点显示名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 画布横坐标
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// 画布纵坐标
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// 节点配置JSON，按类型解析为 StartConfig / LlmConfig 等
        /// </summary>
        public string ConfigJson { get; set; }
    }

    /// <summary>
    /// 流程图连线；条件分支节点的SourceHandle为true/false，
    /// 其余节点不使用SourceHandle
    /// </summary>
    public class AgentGraphEdge
    {
        /// <summary>
        /// 连线唯一Id（前端生成）
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 起点节点Id
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 起点出口标识，条件节点为true/false，其余为空
        /// </summary>
        public string SourceHandle { get; set; }
        /// <summary>
        /// 终点节点Id
        /// </summary>
        public string Target { get; set; }
    }

    /// <summary>
    /// 开始节点配置：声明流程的输入变量名列表，
    /// 运行时按变量名传入输入值
    /// </summary>
    public class StartConfig
    {
        /// <summary>
        /// 输入变量名列表
        /// </summary>
        public List<string> Variables { get; set; } = new List<string>();
    }

    /// <summary>
    /// LLM节点配置：提示词中可用 {{节点Id.字段}}
    /// 形式的变量引用上游节点的输出
    /// </summary>
    public class LlmConfig
    {
        /// <summary>
        /// 系统提示词
        /// </summary>
        public string SystemPrompt { get; set; }
        /// <summary>
        /// 用户提示词
        /// </summary>
        public string UserPrompt { get; set; }
        /// <summary>
        /// 模型名称，为空时使用系统配置的默认模型
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// 温度，为空时使用系统默认值
        /// </summary>
        public double? Temperature { get; set; }
        /// <summary>
        /// 最大生成Token数，为空时使用系统默认值
        /// </summary>
        public int? MaxTokens { get; set; }
    }

    /// <summary>
    /// 知识库节点配置：对查询文本做向量化后按余弦相似度检索文档分块
    /// </summary>
    public class KnowledgeConfig
    {
        /// <summary>
        /// 限定检索的文档ID列表，为空表示检索全部有权限的文档
        /// </summary>
        public List<Guid> DocIds { get; set; }
        /// <summary>
        /// 返回的最大分块数
        /// </summary>
        public int TopK { get; set; } = 3;
        /// <summary>
        /// 相似度阈值，低于该值的分块被过滤
        /// </summary>
        public double Threshold { get; set; } = 0.1;
        /// <summary>
        /// 查询文本，支持 {{节点Id.字段}} 变量引用
        /// </summary>
        public string Query { get; set; }
    }

    /// <summary>
    /// API调用节点配置：URL、请求头、请求体均支持 {{节点Id.字段}} 变量引用
    /// </summary>
    public class ApiConfig
    {
        /// <summary>
        /// HTTP方法：GET/POST/PUT/DELETE
        /// </summary>
        public string Method { get; set; } = "GET";
        /// <summary>
        /// 请求地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 附加请求头
        /// </summary>
        public List<AgentKeyValue> Headers { get; set; }
        /// <summary>
        /// 请求体（POST/PUT）
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// 超时秒数
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;
    }

    /// <summary>
    /// 内部API节点配置：选择框架自动发现的系统接口（查询/新增等工具），
    /// 参数值支持 {{节点Id.字段}} 变量引用；输出 body（原始JSON文本）与 json（解析后对象）
    /// </summary>
    public class InnerApiConfig
    {
        /// <summary>
        /// 接口工具名，如 query_Student
        /// </summary>
        public string ToolName { get; set; }
        /// <summary>
        /// 参数列表（名称=工具参数名，值支持变量引用）
        /// </summary>
        public List<AgentKeyValue> Args { get; set; } = new List<AgentKeyValue>();
    }

    /// <summary>
    /// 代码节点配置：一个C#静态方法（复用AI代码的编译/黑名单/超时设施）。
    /// 语法检查时从代码提取参数与返回值类型；参数值支持变量引用；
    /// 方法返回值即节点输出，下游用 {{节点Id}} 引用
    /// </summary>
    public class CodeConfig
    {
        /// <summary>
        /// 方法代码，形如 public static double Calc(double x) { ... }
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 生成该代码所用的提示词（AI生成时保存，便于后续再生成）
        /// </summary>
        public string Prompt { get; set; }
        /// <summary>
        /// 方法名（从代码提取，执行时定位方法）
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// 参数定义列表（语法检查提取）
        /// </summary>
        public List<CodeParamDef> Params { get; set; } = new List<CodeParamDef>();
        /// <summary>
        /// 参数取值（参数名→值模板，支持 {{节点Id.字段}} 变量引用）
        /// </summary>
        public List<AgentKeyValue> ParamValues { get; set; } = new List<AgentKeyValue>();
        /// <summary>
        /// 返回值类型名（语法检查提取，展示用）
        /// </summary>
        public string ReturnType { get; set; }
    }

    /// <summary>
    /// 代码节点的参数定义（名称/类型）
    /// </summary>
    public class CodeParamDef
    {
        /// <summary>参数名</summary>
        public string Name { get; set; }
        /// <summary>类型名（C#显示名）</summary>
        public string Type { get; set; }
    }

    /// <summary>
    /// 键值对（API节点的请求头等）
    /// </summary>
    public class AgentKeyValue
    {
        /// <summary>
        /// 键
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// 条件分支节点配置：左右操作数支持 {{节点Id.字段}} 变量引用
    /// </summary>
    public class ConditionConfig
    {
        /// <summary>
        /// 左操作数（支持变量）
        /// </summary>
        public string Left { get; set; }
        /// <summary>
        /// 比较操作符，见 ConditionOperator 常量
        /// </summary>
        public string Operator { get; set; } = "equals";
        /// <summary>
        /// 右操作数（支持变量），isEmpty/isNotEmpty时忽略
        /// </summary>
        public string Right { get; set; }
    }

    /// <summary>
    /// 条件操作符常量
    /// </summary>
    public static class ConditionOperator
    {
        /// <summary>等于</summary>
        public new const string Equals = "equals";
        /// <summary>不等于</summary>
        public const string NotEquals = "notEquals";
        /// <summary>包含</summary>
        public const string Contains = "contains";
        /// <summary>不包含</summary>
        public const string NotContains = "notContains";
        /// <summary>以...开头</summary>
        public const string StartsWith = "startsWith";
        /// <summary>以...结尾</summary>
        public const string EndsWith = "endsWith";
        /// <summary>大于（数值或字符串）</summary>
        public const string GreaterThan = "gt";
        /// <summary>小于（数值或字符串）</summary>
        public const string LessThan = "lt";
        /// <summary>为空</summary>
        public const string IsEmpty = "isEmpty";
        /// <summary>不为空</summary>
        public const string IsNotEmpty = "isNotEmpty";
    }

    /// <summary>
    /// 回复节点配置：模板中支持 {{节点Id.字段}} 变量引用，
    /// 渲染后的文本作为流程最终输出
    /// </summary>
    public class AnswerConfig
    {
        /// <summary>
        /// 回复文本模板
        /// </summary>
        public string Text { get; set; }
    }

    /// <summary>
    /// 节点配置解析辅助：把ConfigJson按节点类型反序列化为对应的配置类
    /// </summary>
    public static class AgentNodeConfigResolver
    {
        /// <summary>
        /// 解析节点配置
        /// </summary>
        /// <typeparam name="T">目标配置类型</typeparam>
        /// <param name="node">节点</param>
        /// <param name="defaultValue">解析失败时返回的默认值</param>
        /// <returns>配置对象</returns>
        public static T Resolve<T>(AgentGraphNode node, T defaultValue = default) where T : class, new()
        {
            if (node == null || string.IsNullOrWhiteSpace(node.ConfigJson))
                return defaultValue ?? new T();
            try
            {
                return JsonSerializer.Deserialize<T>(node.ConfigJson, AgentGraph.JsonOptions) ?? new T();
            }
            catch
            {
                return defaultValue ?? new T();
            }
        }
    }
}
