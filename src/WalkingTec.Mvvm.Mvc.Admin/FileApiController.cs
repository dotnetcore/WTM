using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WalkingTec.Mvvm.Admin.Api
{
    /// <summary>
    /// 文件接口
    /// </summary>
    [ApiController]
    [Route("api/_file")]
    [AllRights]
    public class FileApiController : BaseApiController
    {
        [HttpPost("upload")]
        [ActionDescription("上传文件")]
        public IActionResult Upload()
        {
            var FileData = Request.Form.Files[0];
            var sm = ConfigInfo.FileUploadOptions.SaveFileMode;
            var vm = CreateVM<FileAttachmentVM>();
            vm.Entity.FileName = FileData.FileName;
            vm.Entity.Length = FileData.Length;
            vm.Entity.UploadTime = DateTime.Now;
            vm.Entity.SaveFileMode = sm;
            vm = FileHelper.GetFileByteForUpload(vm, FileData.OpenReadStream(), ConfigInfo, FileData.FileName, sm);
            vm.Entity.IsTemprory = true;

            if(string.IsNullOrEmpty(vm.Entity.Path) && vm.Entity.SaveFileMode == SaveFileModeEnum.Local)
            {
                return BadRequest("服务端没有配置储存文件的地址");
            }
            if (string.IsNullOrEmpty(vm.Entity.Path) && vm.Entity.SaveFileMode == SaveFileModeEnum.DFS)
            {
                return BadRequest("DFS上传失败");
            }
            if (vm.Entity.FileData == null && vm.Entity.SaveFileMode == SaveFileModeEnum.Database)
            {
                return BadRequest("上传失败");
            }
             vm.DoAdd();
            return Ok(new { Id = vm.Entity.ID.ToString(), Name = vm.Entity.FileName });
        }

        [HttpGet("getFileName/{id}")]
        [ActionDescription("获取文件名")]
        public IActionResult GetFileName(Guid id)
        {
            FileAttachmentVM vm = CreateVM<FileAttachmentVM>(id);
            return Ok(vm.Entity.FileName);
        }

        [HttpGet("getFile/{id}")]
        [ActionDescription("获取文件")]
        public IActionResult GetFile(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("没有找到文件");
            }
            var vm = CreateVM<FileAttachmentVM>(id);
            var data = FileHelper.GetFileByteForDownLoadByVM(vm, ConfigInfo);
            if (data == null)
            {
                data = new byte[0];
            }
            Response.Body.Write(data, 0, data.Count());
            return new EmptyResult();
        }

        [HttpGet("downloadFile/{id}")]
        [ActionDescription("下载文件")]
        public IActionResult DownloadFile(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("没有找到文件");
            }
            var vm = CreateVM<FileAttachmentVM>(id);
            var data = FileHelper.GetFileByteForDownLoadByVM(vm, ConfigInfo);
            if (data == null)
            {
                data = new byte[0];
            }
            var ext = vm.Entity.FileExt.ToLower();
            var contenttype = "application/octet-stream";
            if (ext == "pdf")
            {
                contenttype = "application/pdf";
            }
            if (ext == "png" || ext == "bmp" || ext == "gif" || ext == "tif" || ext == "jpg" || ext == "jpeg")
            {
                contenttype = $"image/{ext}";
            }
            return File(data, contenttype, vm.Entity.FileName ?? (Guid.NewGuid().ToString() + ext));
        }

        [HttpGet("deleteFile/{id}")]
        [ActionDescription("下载文件")]
        public IActionResult DeletedFile(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("没有找到文件");
            }
            var vm = CreateVM<FileAttachmentVM>(id);
            vm.DoDelete();
            return Ok();
        }
    }
}
