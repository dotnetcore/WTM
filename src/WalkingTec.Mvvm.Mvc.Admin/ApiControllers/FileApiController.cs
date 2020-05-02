using System;
using System.Drawing;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Auth.Attribute;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Admin.Api
{
    [AuthorizeJwtWithCookie]
    [ApiController]
    [Route("api/_file")]
    [AllRights]
    [ActionDescription("File")]
    public class FileApiController : BaseApiController
    {
        [HttpPost("[action]")]
        [ActionDescription("UploadFile")]
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

            if (string.IsNullOrEmpty(vm.Entity.Path) && vm.Entity.SaveFileMode == SaveFileModeEnum.Local)
            {
                return BadRequest(Core.Program._localizer["UploadFailed"]);
            }
            if (string.IsNullOrEmpty(vm.Entity.Path) && vm.Entity.SaveFileMode == SaveFileModeEnum.DFS)
            {
                return BadRequest(Core.Program._localizer["UploadFailed"]);
            }
            if (vm.Entity.FileData == null && vm.Entity.SaveFileMode == SaveFileModeEnum.Database)
            {
                return BadRequest(Core.Program._localizer["UploadFailed"]);
            }
            vm.DoAdd();
            return Ok(new { Id = vm.Entity.ID.ToString(), Name = vm.Entity.FileName });
        }

        [HttpPost("[action]")]
        [ActionDescription("UploadPic")]
        public IActionResult UploadImage(int? width = null, int? height = null)
        {
            if (width == null && height == null)
            {
                return Upload();
            }
            var FileData = Request.Form.Files[0];
            var sm = ConfigInfo.FileUploadOptions.SaveFileMode;
            var vm = CreateVM<FileAttachmentVM>();

            Image oimage = Image.FromStream(FileData.OpenReadStream());
            if (oimage == null)
            {
                return BadRequest(Core.Program._localizer["UploadFailed"]);
            }
            if (width == null)
            {
                width = height * oimage.Width / oimage.Height;
            }
            if (height == null)
            {
                height = width * oimage.Height / oimage.Width;
            }
            MemoryStream ms = new MemoryStream();
            oimage.GetThumbnailImage(width.Value, height.Value, null, IntPtr.Zero).Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            vm.Entity.FileName = FileData.FileName.Replace(".", "") + ".jpg";
            vm.Entity.UploadTime = DateTime.Now;
            vm.Entity.SaveFileMode = sm;
            vm = FileHelper.GetFileByteForUpload(vm, ms, ConfigInfo, FileData.FileName.Replace(".", "") + ".jpg", sm);
            vm.Entity.Length = ms.Length;
            vm.Entity.IsTemprory = true;
            oimage.Dispose();

            if ((!string.IsNullOrEmpty(vm.Entity.Path) && (vm.Entity.SaveFileMode == SaveFileModeEnum.Local || vm.Entity.SaveFileMode == SaveFileModeEnum.DFS)) || (vm.Entity.FileData != null && vm.Entity.SaveFileMode == SaveFileModeEnum.Database))
            {
                vm.DoAdd();
                return Ok(new { Id = vm.Entity.ID.ToString(), Name = vm.Entity.FileName });
            }
            return BadRequest(Core.Program._localizer["UploadFailed"]);

        }

        [HttpGet("[action]/{id}")]
        [ActionDescription("GetFileName")]
        public IActionResult GetFileName(Guid id)
        {
            FileAttachmentVM vm = CreateVM<FileAttachmentVM>(id);
            return Ok(vm.Entity.FileName);
        }

        [HttpGet("[action]/{id}")]
        [ActionDescription("GetFile")]
        public IActionResult GetFile(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(Core.Program._localizer["FileNotFound"]);
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

        [HttpGet("[action]/{id}")]
        [ActionDescription("DownloadFile")]
        public IActionResult DownloadFile(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(Core.Program._localizer["FileNotFound"]);
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

        [HttpGet("[action]/{id}")]
        [ActionDescription("DeleteFile")]
        public IActionResult DeletedFile(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(Core.Program._localizer["FileNotFound"]);
            }
            var vm = CreateVM<FileAttachmentVM>(id);
            vm.DoDelete();
            return Ok();
        }
    }
}
