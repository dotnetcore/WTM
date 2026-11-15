using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 文档分块服务 — 固定长度(含重叠)/按段落/按句子，自动估算 token 数
    /// </summary>
    public class ChunkingService : IChunkingService
    {
        /// <summary>
        /// 按文档设定的分块策略对文档内容进行分块
        /// </summary>
        /// <param name="doc">待分块的知识库文档</param>
        /// <returns>分块后的知识块数组，每个块均已填充序号、token估算数量和所属文档ID</returns>
        public Task<KnowledgeChunk[]> ChunkDocumentAsync(KnowledgeDoc doc)
        {
            var chunks = doc.ChunkStrategy switch
            {
                ChunkStrategyEnum.FixedLength => ChunkFixedLength(doc),
                ChunkStrategyEnum.ByParagraph => ChunkByParagraph(doc),
                ChunkStrategyEnum.BySentence => ChunkBySentence(doc),
                _ => ChunkFixedLength(doc)
            };

            foreach (var c in chunks)
            {
                c.TokenCount = EstimateTokens(c.Content);
                c.DocId = doc.ID;
            }

            return Task.FromResult(chunks.ToArray());
        }

        /// <summary>
        /// 按固定长度分块，相邻块之间保留指定长度的重叠内容
        /// </summary>
        /// <param name="doc">待分块的知识库文档</param>
        /// <returns>分块结果列表</returns>
        private static List<KnowledgeChunk> ChunkFixedLength(KnowledgeDoc doc)
        {
            var result = new List<KnowledgeChunk>();
            var text = doc.Content;
            int size = doc.ChunkSize > 0 ? doc.ChunkSize : 500;
            //重叠必须为非负且不超过分块大小的一半，否则回退量大于前进量会造成死循环
            int overlap = Math.Clamp(doc.ChunkOverlap, 0, size / 2);
            int start = 0, index = 0;

            while (start < text.Length)
            {
                int end = Math.Min(start + size, text.Length);
                //未到文本末尾时，尝试在窗口后半段寻找句子结束符作为切割点，避免切断句子
                if (end < text.Length)
                {
                    int cut = LastCutPosition(text, start, end);
                    if (cut > start + size / 2) end = cut + 1;
                }
                result.Add(new KnowledgeChunk { Index = index, Content = text[start..end] });
                //已切到文本末尾，最后一个分块生成完毕，必须立即结束，否则起点回退重叠后会永远切同一个尾巴
                if (end >= text.Length) break;
                //下一块起点回退overlap个字符，使相邻块之间保留重叠内容；同时保证至少比当前起点前进1个字符，防止死循环
                start = Math.Max(end - overlap, start + 1);
                index++;
            }
            return result;
        }

        /// <summary>
        /// 按段落分块，以连续换行符作为段落分隔标记
        /// </summary>
        /// <param name="doc">待分块的知识库文档</param>
        /// <returns>分块结果列表</returns>
        private static List<KnowledgeChunk> ChunkByParagraph(KnowledgeDoc doc)
        {
            return doc.Content
                .Split(new[] { "\n\n", "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select((text, i) => new KnowledgeChunk { Index = i, Content = text.Trim() })
                .ToList();
        }

        /// <summary>
        /// 按句子分块：将句子逐步累积到缓冲区，超过目标长度时输出为一个块
        /// </summary>
        /// <param name="doc">待分块的知识库文档</param>
        /// <returns>分块结果列表</returns>
        private static List<KnowledgeChunk> ChunkBySentence(KnowledgeDoc doc)
        {
            var result = new List<KnowledgeChunk>();
            var sentences = SplitSentences(doc.Content);
            int targetSize = doc.ChunkSize > 0 ? doc.ChunkSize : 500;
            var buffer = "";
            int index = 0;

            foreach (var s in sentences)
            {
                if (buffer.Length + s.Length > targetSize && buffer.Length > 0)
                {
                    result.Add(new KnowledgeChunk { Index = index++, Content = buffer.Trim() });
                    buffer = s;
                }
                else buffer += s;
            }
            if (!string.IsNullOrWhiteSpace(buffer))
                result.Add(new KnowledgeChunk { Index = index, Content = buffer.Trim() });

            return result;
        }

        /// <summary>
        /// 按中英文句终符号和换行符切分句子
        /// </summary>
        /// <param name="text">待切分的文本</param>
        /// <returns>句子列表</returns>
        private static List<string> SplitSentences(string text)
        {
            var result = new List<string>();
            int start = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '。' || text[i] == '！' || text[i] == '？' ||
                    text[i] == '.' || text[i] == '!' || text[i] == '?' || text[i] == '\n')
                {
                    result.Add(text[start..(i + 1)]);
                    start = i + 1;
                }
            }
            if (start < text.Length) result.Add(text[start..]);
            return result;
        }

        /// <summary>
        /// 在指定范围内从后向前寻找最后一个句子结束符的位置，作为固定长度分块的切割点
        /// </summary>
        /// <param name="text">全文文本</param>
        /// <param name="start">搜索范围起点</param>
        /// <param name="end">搜索范围终点</param>
        /// <returns>切割点下标，找不到返回-1</returns>
        private static int LastCutPosition(string text, int start, int end)
        {
            for (int i = end - 1; i > start; i--)
                if (text[i] == '。' || text[i] == '\n' || text[i] == '.' || text[i] == '！' || text[i] == '？')
                    return i;
            return -1;
        }

        /// <summary>
        /// 估算文本的token数量：中文字符按约1.5字符每token，其他字符按约4字符每token
        /// </summary>
        /// <param name="text">待估算的文本</param>
        /// <returns>估算的token数量</returns>
        private static int EstimateTokens(string text)
        {
            int chinese = text.Count(c => c >= 0x4E00 && c <= 0x9FFF);
            int other = text.Length - chinese;
            return (int)(chinese / 1.5 + other / 4.0);
        }
    }
}
