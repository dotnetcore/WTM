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
        [Display(Name = "FileName")]
        [Required(ErrorMessage = "{0}required")]
        public string FileName { get; set; }
        [Display(Name = "FileExt")]
        [Required(ErrorMessage = "{0}required")]
        [StringLength(10)]
        public string FileExt { get; set; }
        [Display(Name = "Path")]
        public string Path { get; set; }
        [Display(Name = "Length")]
        public long Length { get; set; }
        public DateTime UploadTime { get; set; }
        public bool IsTemprory { get; set; }

        #region 组名，FastDFS服务器用
        [Display(Name = "SaveFileMode")]
        public SaveFileModeEnum? SaveFileMode { get; set; }
        [Display(Name = "GroupName")]
        [StringLength(50,ErrorMessage = "{0}stringmax{1}")]
        public string GroupName { get; set; }
        #endregion
        public byte[] FileData { get; set; }
    }
}
