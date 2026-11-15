using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 嵌入服务 — 将文本转换为向量
    /// </summary>
    public interface IEmbeddingService
    {
        /// <summary>
        /// 将文本转换为其向量表示
        /// </summary>
        /// <param name="text">要向量化的文本</param>
        /// <returns>文本的嵌入向量</returns>
        Task<float[]> GetEmbeddingAsync(string text);
        /// <summary>
        /// 嵌入向量的维度
        /// </summary>
        int Dimension { get; }
    }

    /// <summary>
    /// 分块服务 — 按策略将文档拆分为分块
    /// </summary>
    public interface IChunkingService
    {
        /// <summary>
        /// 将文档按策略拆分为多个分块
        /// </summary>
        /// <param name="doc">待拆分的知识文档</param>
        /// <returns>拆分得到的分块数组</returns>
        Task<KnowledgeChunk[]> ChunkDocumentAsync(KnowledgeDoc doc);
    }
}
