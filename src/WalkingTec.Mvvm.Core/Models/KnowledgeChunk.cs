using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// AI 知识库分块实体，存储文档切分后的文本分块及其嵌入向量，
    /// 用于后续的相似度检索
    /// </summary>
    public class KnowledgeChunk : BasePoco
    {
        /// <summary>
        /// 所属文档 ID
        /// </summary>
        [Display(Name = "文档")]
        [Required]
        public Guid DocId { get; set; }

        /// <summary>
        /// 所属文档的导航属性
        /// </summary>
        [Display(Name = "文档")]
        [ForeignKey(nameof(DocId))]
        public KnowledgeDoc Doc { get; set; }

        /// <summary>
        /// 分块序号，标识该分块在文档中的顺序
        /// </summary>
        [Display(Name = "序号")]
        public int Index { get; set; }

        /// <summary>
        /// 分块的文本内容
        /// </summary>
        [Display(Name = "分块内容")]
        [Required]
        [StringLength(4000)]
        public string Content { get; set; }

        /// <summary>
        /// 分块内容包含的 Token 数量
        /// </summary>
        [Display(Name = "Token 数")]
        public int TokenCount { get; set; }

        /// <summary>
        /// 分块内容对应的嵌入向量（以字节数组形式序列化存储）
        /// </summary>
        [Display(Name = "嵌入向量")]
        public byte[] Embedding { get; set; }

        /// <summary>
        /// 嵌入向量的维度
        /// </summary>
        [Display(Name = "向量维度")]
        public int EmbeddingDim { get; set; }
    }
}
