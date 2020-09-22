using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using WalkingTec.Mvvm.Core.Models;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// FileAttachment
    /// </summary>
    [Table("FileAttachments")]
    public class FileAttachment : TopBasePoco, IWtmFile, IDisposable
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

        public string SaveMode { get; set; }

        public byte[] FileData { get; set; }

        public string ExtraInfo { get; set; }

        [NotMapped]
        public string Url { get; set; }

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
