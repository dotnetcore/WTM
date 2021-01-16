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
        public IActionResult Upload([FromServices] WtmFileProvider fp, string sm = null, string groupName = null, string subdir = null, string extra = null, string csName = null)
        {
            var FileData = Request.Form.Files[0];
            var file = fp.Upload(FileData.FileName, FileData.Length, FileData.OpenReadStream(), groupName, subdir, extra, sm, Wtm.CreateDC(cskey: csName));
            return Ok(new { Id = file.GetID(), Name = file.FileName });
        }

        [HttpPost("[action]")]
        [ActionDescription("UploadPic")]
        public IActionResult UploadImage([FromServices] WtmFileProvider fp, int? width = null, int? height = null, string sm = null, string groupName = null, string subdir = null, string extra = null, string csName = null)
        {
            if (width == null && height == null)
            {
                return Upload(fp, sm, groupName, csName);
            }
            var FileData = Request.Form.Files[0];

            Image oimage = Image.FromStream(FileData.OpenReadStream());
            if (oimage == null)
            {
                return BadRequest(Localizer["Sys.UploadFailed"]);
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
            var file = fp.Upload(FileData.FileName, FileData.Length, ms, groupName, subdir, extra, sm, Wtm.CreateDC(cskey: csName));
            oimage.Dispose();
            ms.Dispose();

            if (file != null)
            {
                return Ok(new { Id = file.GetID(), Name = file.FileName });
            }
            return BadRequest(Localizer["Sys.UploadFailed"]);

        }

        [HttpGet("[action]/{id}")]
        [ActionDescription("GetFileName")]
        public IActionResult GetFileName([FromServices] WtmFileProvider fp, string id, string csName = null)
        {
            return Ok(fp.GetFileName(id, ConfigInfo.CreateDC(csName)));
        }

        [HttpGet("[action]/{id}")]
        [ActionDescription("GetFile")]
        public IActionResult GetFile([FromServices] WtmFileProvider fp, string id, string csName = null)
        {
            var file = fp.GetFile(id, true, ConfigInfo.CreateDC(csName));


            if (file == null)
            {
                return BadRequest(Localizer["Sys.FileNotFound"]);
            }
            file.DataStream?.CopyToAsync(Response.Body);
            file.DataStream.Dispose();
            return new EmptyResult();
        }

        [HttpGet("[action]/{id}")]
        [ActionDescription("DownloadFile")]
        public IActionResult DownloadFile([FromServices] WtmFileProvider fp, string id, string csName = null)
        {
            var file = fp.GetFile(id, true, ConfigInfo.CreateDC(csName));
            if (file == null)
            {
                return BadRequest(Localizer["Sys.FileNotFound"]);
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
        public IActionResult DeletedFile([FromServices] WtmFileProvider fp, string id, string csName = null)
        {
            fp.DeleteFile(id, ConfigInfo.CreateDC(csName));
            return Ok();
        }
    }
}
