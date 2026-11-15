using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 知识文档处理服务 — 分块 + 嵌入 + 存储管线
    /// </summary>
    public class KnowledgeProcessService
    {
        private readonly IEmbeddingService _embeddingService;

        /// <summary>
        /// 构造函数，注入嵌入服务；未提供时使用内置嵌入服务
        /// </summary>
        /// <param name="embeddingService">嵌入服务实例，可为空</param>
        public KnowledgeProcessService(IEmbeddingService embeddingService = null)
        {
            _embeddingService = embeddingService ?? new BuiltInEmbeddingService();
        }

        /// <summary>
        /// 对文档执行完整处理管线：分块 → 嵌入 → 存储
        /// </summary>
        public async Task ProcessDocumentAsync(WTMContext wtm, Guid docId)
        {
            var dc = wtm.CreateDC();
            var chunkDelay = TimeSpan.FromMilliseconds(100);

            try
            {
                var doc = dc.Set<KnowledgeDoc>().Include(d => d.Chunks).FirstOrDefault(d => d.ID == docId);
                if (doc == null) return;
                if (string.IsNullOrWhiteSpace(doc.Content))
                {
                    doc.Status = DocStatusEnum.Failed;
                    dc.SaveChanges();
                    wtm.DoLog("文档内容为空", ActionLogTypesEnum.Exception, "知识文档处理失败");
                    return;
                }

                doc.Status = DocStatusEnum.Processing;
                dc.SaveChanges();

                var chunkingService = new ChunkingService();
                var chunks = await chunkingService.ChunkDocumentAsync(doc);

                var oldChunks = dc.Set<KnowledgeChunk>().Where(c => c.DocId == docId);
                dc.Set<KnowledgeChunk>().RemoveRange(oldChunks);
                await dc.SaveChangesAsync();

                // 逐块调用嵌入服务计算向量并限速写入，向量计算失败的分块保留空向量；
                // 每batchSize块提交一次，避免变更跟踪器累积大量实体占用内存
                const int batchSize = 200;
                int added = 0;
                foreach (var chunk in chunks)
                {
                    try
                    {
                        var embedding = await _embeddingService.GetEmbeddingAsync(chunk.Content);
                        chunk.Embedding = SerializeEmbedding(embedding);
                        chunk.EmbeddingDim = embedding.Length;
                    }
                    catch { chunk.Embedding = null; }

                    dc.Set<KnowledgeChunk>().Add(chunk);
                    if (++added >= batchSize)
                    {
                        await dc.SaveChangesAsync();
                        added = 0;
                    }
                    await Task.Delay(chunkDelay);
                }

                doc.ChunkCount = chunks.Length;
                doc.Status = DocStatusEnum.Completed;
                await dc.SaveChangesAsync();

                wtm.DoLog(doc.Title, ActionLogTypesEnum.Normal, "知识文档处理完成", $"分块数: {chunks.Length}");
            }
            catch (Exception ex)
            {
                var doc = dc.Set<KnowledgeDoc>().Find(docId);
                if (doc != null) { doc.Status = DocStatusEnum.Failed; dc.SaveChanges(); }
                wtm.DoLog(ex.ToString(), ActionLogTypesEnum.Exception, "知识文档处理失败");
            }
        }

        // 将嵌入向量按字节复制为二进制数组，便于数据库存储
        private static byte[] SerializeEmbedding(float[] embedding)
        {
            var bytes = new byte[embedding.Length * sizeof(float)];
            Buffer.BlockCopy(embedding, 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
