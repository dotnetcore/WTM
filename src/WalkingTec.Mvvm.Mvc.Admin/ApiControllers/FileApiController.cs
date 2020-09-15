using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Support.FileHandlers;
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
        public IActionResult Upload([FromServices] WtmFileProvider fp)
        {
            var fh = fp.CreateFileHandler();
            var FileData = Request.Form.Files[0];
            var file = fh.Upload(FileData.FileName, FileData.Length, FileData.OpenReadStream());
            return Ok(new { Id = file.GetID(), Name = file.FileName });
        }

        [HttpPost("[action]")]
        [ActionDescription("UploadPic")]
        public IActionResult UploadImage([FromServices] WtmFileProvider fp,int? width = null, int? height = null)
        {
            if (width == null && height == null)
            {
                return Upload(fp);
            }
            var fh = fp.CreateFileHandler();
            var FileData = Request.Form.Files[0];

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
            var file = fh.Upload(FileData.FileName, FileData.Length, ms);
            oimage.Dispose();
            ms.Dispose();

            if (file != null)
            {
                return Ok(new { Id = file.GetID(), Name = file.FileName });
            }
            return BadRequest(Core.Program._localizer["UploadFailed"]);

        }

        [HttpGet("[action]/{id}")]
        [ActionDescription("GetFileName")]
        public IActionResult GetFileName([FromServices] WtmFileProvider fp, string id)
        {
            var fh = fp.CreateFileHandler();
            return Ok(fh.GetFileName(id));
        }

        [HttpGet("[action]/{id}")]
        [ActionDescription("GetFile")]
        public IActionResult GetFile([FromServices] WtmFileProvider fp, string id)
        {
            var fh = fp.CreateFileHandler();
            var file = fh.GetFile(id);


            if (file == null)
            {
                return BadRequest(Core.Program._localizer["FileNotFound"]);
            }
            file.DataStream?.CopyToAsync(Response.Body);
            return new EmptyResult();
        }

        [HttpGet("[action]/{id}")]
        [ActionDescription("DownloadFile")]
        public IActionResult DownloadFile([FromServices] WtmFileProvider fp, string id)
        {
            var fh = fp.CreateFileHandler();
            var file = fh.GetFile(id);
            if (file == null)
            {
                return BadRequest(Core.Program._localizer["FileNotFound"]);
            }
            var ext = file.FileExt.ToLower();
            var contenttype = "application/octet-stream";
            if (ext == "pdf")
            {
                contenttype = "application/pdf";
            }
            if (ext == "png" || ext == "bmp" || ext == "gif" || ext == "tif" || ext == "jpg" || ext == "jpeg")
            {
                contenttype = $"image/{ext}";
            }
            return File(file.DataStream, contenttype, file.FileName ?? (Guid.NewGuid().ToString() + ext));
        }

        [HttpGet("[action]/{id}")]
        [ActionDescription("DeleteFile")]
        public IActionResult DeletedFile([FromServices] WtmFileProvider fp, string id)
        {
            var fh = fp.CreateFileHandler();
            fh.DeleteFile(id);
            return Ok();
        }
    }
}
