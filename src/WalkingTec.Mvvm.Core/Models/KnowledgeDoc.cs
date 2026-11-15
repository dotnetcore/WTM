using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// AI 知识库文档实体，存储知识库中的原始文档内容及分块、向量化处理配置
    /// </summary>
    public class KnowledgeDoc : BasePoco
    {
        /// <summary>
        /// 文档标题
        /// </summary>
        [Display(Name = "标题")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [StringLength(200)]
        public string Title { get; set; }

        /// <summary>
        /// 文档正文内容
        /// </summary>
        [Display(Name = "内容")]
        public string Content { get; set; }

        /// <summary>
        /// 关联附件 ID；文档内容来自上传文件时使用
        /// </summary>
        [Display(Name = "附件")]
        public Guid? FileAttachmentId { get; set; }

        /// <summary>
        /// 关联附件的导航属性
        /// </summary>
        [Display(Name = "附件")]
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(FileAttachmentId))]
        public FileAttachment FileAttachment { get; set; }

        /// <summary>
        /// 文档来源描述（如网址、上传人等）
        /// </summary>
        [Display(Name = "来源")]
        [StringLength(500)]
        public string Source { get; set; }

        /// <summary>
        /// 文档类型
        /// </summary>
        [Display(Name = "文档类型")]
        public DocTypeEnum DocType { get; set; }

        /// <summary>
        /// 分块策略，决定文档内容的切分方式
        /// </summary>
        [Display(Name = "分块策略")]
        public ChunkStrategyEnum ChunkStrategy { get; set; }

        /// <summary>
        /// 分块大小（每个分块的最大长度），默认 500
        /// </summary>
        [Display(Name = "分块大小")]
        public int ChunkSize { get; set; } = 500;

        /// <summary>
        /// 相邻分块之间的重叠长度，用于保留上下文连贯性，默认 50
        /// </summary>
        [Display(Name = "分块重叠")]
        public int ChunkOverlap { get; set; } = 50;

        /// <summary>
        /// 分块数量，文档分块处理完成后更新
        /// </summary>
        [Display(Name = "分块数")]
        public int ChunkCount { get; set; }

        /// <summary>
        /// 文档处理状态，标识分块与向量化的进度
        /// </summary>
        [Display(Name = "状态")]
        public DocStatusEnum Status { get; set; }

        /// <summary>
        /// 分块列表的导航属性
        /// </summary>
        [Display(Name = "分块列表")]
        public List<KnowledgeChunk> Chunks { get; set; }
    }

    /// <summary>
    /// 文档类型枚举，标识知识库文档的来源格式
    /// </summary>
    public enum DocTypeEnum
    {
        /// <summary>
        /// 普通文本
        /// </summary>
        [Display(Name = "普通文本")]
        Text,
        /// <summary>
        /// Markdown 格式
        /// </summary>
        [Display(Name = "Markdown")]
        Markdown,
        /// <summary>
        /// 网页链接
        /// </summary>
        [Display(Name = "网页链接")]
        Url,
        /// <summary>
        /// 图片
        /// </summary>
        [Display(Name = "图片")]
        Image,
        /// <summary>
        /// Word 文档
        /// </summary>
        [Display(Name = "Word")]
        Word,
        /// <summary>
        /// Excel 表格
        /// </summary>
        [Display(Name = "Excel")]
        Excel,
        /// <summary>
        /// PDF 文档
        /// </summary>
        [Display(Name = "PDF")]
        Pdf
    }

    /// <summary>
    /// 分块策略枚举，决定文档内容切分为分块的方式
    /// </summary>
    public enum ChunkStrategyEnum
    {
        /// <summary>
        /// 按固定长度切分
        /// </summary>
        [Display(Name = "固定长度")]
        FixedLength,
        /// <summary>
        /// 按段落切分
        /// </summary>
        [Display(Name = "按段落")]
        ByParagraph,
        /// <summary>
        /// 按句子切分
        /// </summary>
        [Display(Name = "按句子")]
        BySentence
    }

    /// <summary>
    /// 文档处理状态枚举，标识知识库文档的分块与向量化进度
    /// </summary>
    public enum DocStatusEnum
    {
        /// <summary>
        /// 待处理
        /// </summary>
        [Display(Name = "待处理")]
        Pending,
        /// <summary>
        /// 处理中
        /// </summary>
        [Display(Name = "处理中")]
        Processing,
        /// <summary>
        /// 已完成（分块与向量化均已结束）
        /// </summary>
        [Display(Name = "已完成")]
        Completed,
        /// <summary>
        /// 处理失败
        /// </summary>
        [Display(Name = "处理失败")]
        Failed
    }
}
