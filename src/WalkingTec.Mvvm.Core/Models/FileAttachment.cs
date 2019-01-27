using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// FileAttachment
    /// </summary>
    [Table("FileAttachments")]
    public class FileAttachment : BasePoco
    {
        [Display(Name = "文件名" )]
        [Required(ErrorMessage ="{0}是必填项")]
        public string FileName { get; set; }
        [Display(Name = "扩展名" )]
        [Required(ErrorMessage ="{0}是必填项")]
        [StringLength(10)]
        public string FileExt { get; set; }
        [Display(Name = "路径" )]
        public string Path { get; set; }
        [Display(Name = "长度" )]
        public long Length { get; set; }
        public DateTime UploadTime { get; set; }
        public bool IsTemprory { get; set; }

        #region 组名，FastDFS服务器用
        [Display(Name = "保存方式" )]
        public SaveFileModeEnum? SaveFileMode { get; set; }
        [Display(Name = "组名" )]
        [StringLength(50,ErrorMessage ="{0}最多输入{1}个字符")]
        public string GroupName { get; set; }
        #endregion
        public byte[] FileData { get; set; }
    }
}
