using System;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 子文件接口，定义从属于某条业务数据的附件的公共结构；
    /// 业务模型实现该接口后即可被框架的附件批量上传功能识别和处理
    /// </summary>
    public interface ISubFile
    {
        /// <summary>
        /// 附件 ID，对应 FileAttachment 的主键
        /// </summary>
        Guid FileId { get; set; }
        /// <summary>
        /// 附件实体的导航属性
        /// </summary>
        FileAttachment File { get; set; }
        /// <summary>
        /// 排序号，控制子附件的显示顺序
        /// </summary>
        int Order { get; set; }
    }
}
