// WTM默认页面 Wtm buidin page
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Support.FileHandlers;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Admin.Api
{
    [AuthorizeJwtWithCookie]
    [ApiController]
    [Route("api/_file")]
    [AllRights]
    [ActionDescription("_Admin.FileApi")]
    public class FileApiController : BaseApiController
    {
        [HttpPost("[action]")]
        [ActionDescription("UploadFile")]
        public IActionResult Upload([FromServices] WtmFileProvider fp, string sm = null, string groupName = null, string subdir=null,string extra = null,string csName= null)
        {
            var FileData = Request.Form.Files[0];
            var file = fp.Upload(FileData.FileName, FileData.Length, FileData.OpenReadStream(),groupName,subdir,extra,sm,Wtm.CreateDC(cskey:csName));
            return Ok(new { Id = file.GetID(), Name = file.FileName });
        }

        [HttpPost("[action]")]
        [ActionDescription("UploadPic")]
        public IActionResult UploadImage([FromServices] WtmFileProvider fp,int? width = null, int? height = null, string sm = null, string groupName = null, string subdir = null, string extra = null, string csName = null)
        {
            if (width == null && height == null)
            {
                return Upload(fp,sm,groupName,csName);
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
            var file = fp.Upload(FileData.FileName, FileData.Length, ms, groupName,subdir, extra, sm, Wtm.CreateDC(cskey: csName));
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
        public async Task<IActionResult> GetFile([FromServices] WtmFileProvider fp, string id, string csName = null, int? width = null, int? height = null)
        {
            var file = fp.GetFile(id, true, ConfigInfo.CreateDC(csName));


            if (file == null)
            {
                return BadRequest(Localizer["Sys.FileNotFound"]);
            }
            try
            {
                if (width != null || height != null)
                {
                    Image oimage = Image.FromStream(file.DataStream);
                    if (oimage != null)
                    {
                        if (width == null)
                        {
                            width = oimage.Width * height / oimage.Height;
                        }
                        if (height == null)
                        {
                            height = oimage.Height * width / oimage.Width;
                        }
                        var ms = new MemoryStream();
                        oimage.GetThumbnailImage(width.Value, height.Value, null, IntPtr.Zero).Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        ms.Position = 0;
                        await ms?.CopyToAsync(Response.Body);
                        file.DataStream.Dispose();
                        ms.Dispose();
                        oimage.Dispose();
                        return new EmptyResult();
                    }
                }
            }
            catch { }

            var ext = file.FileExt.ToLower();
            if (ext == "mp4")
            {
                return File(file.DataStream, "video/mpeg4", enableRangeProcessing: true);
            }
            else
            {
                await file.DataStream?.CopyToAsync(Response.Body);
                file.DataStream.Dispose();
                return new EmptyResult();
            }
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
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(file.FileName, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return File(file.DataStream, contentType, file.FileName ?? (Guid.NewGuid().ToString() + ext));
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
