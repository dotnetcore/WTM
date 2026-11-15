using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Core.Test
{
    /// <summary>
    /// ChunkingService 分块逻辑回归测试，重点验证固定长度分块在配置重叠时必然终止
    /// （历史缺陷：结尾窗口起点按重叠回退后永远切分同一个尾巴，死循环耗尽内存）
    /// </summary>
    [TestClass]
    public class ChunkingServiceTest
    {
        /// <summary>
        /// 重叠大于0时固定长度分块必须在限定时间内终止，且首尾分块覆盖全文
        /// </summary>
        [TestMethod]
        public void ChunkFixedLength_TerminatesWithOverlap()
        {
            var doc = new KnowledgeDoc
            {
                Content = new string('字', 714),
                ChunkStrategy = ChunkStrategyEnum.FixedLength,
                ChunkSize = 500,
                ChunkOverlap = 50
            };

            var task = new ChunkingService().ChunkDocumentAsync(doc);
            var finished = Task.WhenAny(task, Task.Delay(10_000)).GetAwaiter().GetResult() == task;
            Assert.IsTrue(finished, "固定长度分块未在限定时间内完成，疑似死循环");

            var chunks = task.Result;
            Assert.IsTrue(chunks.Length >= 1, "至少应产生一个分块");
            Assert.IsTrue(chunks.Length <= 5, $"714字符/块长500/重叠50不应产生{chunks.Length}个分块");
            Assert.IsTrue(doc.Content.StartsWith(chunks[0].Content), "首块应从文本开头开始");
            Assert.IsTrue(doc.Content.EndsWith(chunks[^1].Content), "末块应覆盖到文本结尾");
            Assert.IsTrue(chunks.All(c => c.Content.Length > 0), "分块内容不应为空");
            Assert.IsTrue(chunks.All(c => c.DocId == doc.ID), "分块应回填所属文档ID");
            Assert.IsTrue(chunks.All(c => c.TokenCount > 0), "分块应回填token估算数量");
        }

        /// <summary>
        /// 重叠配置不合法（大于块长或为负数）时应被钳制，分块仍然终止
        /// </summary>
        [DataTestMethod]
        [DataRow(200, 1000)]
        [DataRow(-10, 1000)]
        public void ChunkFixedLength_TerminatesWithInvalidOverlap(int overlap, int _)
        {
            var doc = new KnowledgeDoc
            {
                Content = new string('a', 1000),
                ChunkStrategy = ChunkStrategyEnum.FixedLength,
                ChunkSize = 200,
                ChunkOverlap = overlap
            };

            var task = new ChunkingService().ChunkDocumentAsync(doc);
            var finished = Task.WhenAny(task, Task.Delay(10_000)).GetAwaiter().GetResult() == task;
            Assert.IsTrue(finished, "重叠配置不合法时也不应死循环");

            var chunks = task.Result;
            //重叠被钳制为块长一半(100)，最小前进量100字符，1000字符最多切成11块
            Assert.IsTrue(chunks.Length <= 11, $"不应产生{chunks.Length}个分块");
            Assert.IsTrue(doc.Content.EndsWith(chunks[^1].Content), "末块应覆盖到文本结尾");
        }

        /// <summary>
        /// 按句子分块：多个短句应累积合并到目标长度附近的块中
        /// </summary>
        [TestMethod]
        public void ChunkBySentence_MergesShortSentences()
        {
            var doc = new KnowledgeDoc
            {
                Content = "这是第一句话。这是第二句话！这是第三句话？这是第四句话。",
                ChunkStrategy = ChunkStrategyEnum.BySentence,
                ChunkSize = 500,
                ChunkOverlap = 50
            };

            var chunks = new ChunkingService().ChunkDocumentAsync(doc).GetAwaiter().GetResult();
            Assert.AreEqual(1, chunks.Length, "总长未超过目标长度时应合并为一个块");
            Assert.AreEqual(doc.Content, chunks[0].Content, "合并块应保留全部原文");
        }
    }
}
