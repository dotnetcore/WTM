using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Text.Json.Serialization;
using WalkingTec.Mvvm.Core.Models;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// FileAttachment
    /// </summary>
    [Table("FileAttachments")]
    public class FileAttachment : TopBasePoco, IWtmFile, IDisposable
    {
        [Display(Name = "_Admin.FileName")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public string FileName { get; set; }

        [Display(Name = "_Admin.FileExt")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [StringLength(10)]
        public string FileExt { get; set; }

        [Display(Name = "_Admin.Path")]
        public string Path { get; set; }

        [Display(Name = "_Admin.Length")]
        public long Length { get; set; }

        public DateTime UploadTime { get; set; }

        public string SaveMode { get; set; }

        public byte[] FileData { get; set; }

        public string ExtraInfo { get; set; }
        public string HandlerInfo { get; set; }


        [NotMapped]
        [JsonIgnore]
        public Stream DataStream { get; set; }

        public void Dispose()
        {
            if(DataStream != null)
            {
                DataStream.Dispose();
            }
        }

        string IWtmFile.GetID()
        {
            return ID.ToString();
        }
    }
}
