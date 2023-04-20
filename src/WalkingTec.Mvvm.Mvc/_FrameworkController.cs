using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Models;
using WalkingTec.Mvvm.Core.Support.FileHandlers;

namespace WalkingTec.Mvvm.Mvc
{
    [AllRights]
    [ActionDescription("Framework")]
    public class _FrameworkController : BaseController
    {

        [HttpPost]
        [Public]
        public async Task<IActionResult> Selector(string _DONOT_USE_VMNAME
            , string _DONOT_USE_KFIELD
            , string _DONOT_USE_VFIELD
            , string _DONOT_USE_FIELD
            , bool _DONOT_USE_MULTI_SEL
            , string _DONOT_USE_SEL_ID
            , string _DONOT_USE_SUBMIT
            , string _DONOT_USE_LINK_FIELD
            , string _DONOT_USE_TRIGGER_URL
            , string _DONOT_USE_CURRENTCS
        )
        {
            string cs =_DONOT_USE_CURRENTCS;
            Wtm.CurrentCS = cs;
            var listVM = Wtm.CreateVM(_DONOT_USE_VMNAME, null, null, true) as IBasePagedListVM<TopBasePoco, ISearcher>;

            if (listVM is IBasePagedListVM<TopBasePoco, ISearcher>)
            {
                RedoUpdateModel(listVM);
            }

            listVM.SearcherMode = ListVMSearchModeEnum.Selector;
            listVM.RemoveActionColumn();
            listVM.RemoveAction();
            ViewBag.TextName = _DONOT_USE_KFIELD;
            ViewBag.ValName = _DONOT_USE_VFIELD;
            ViewBag.FieldName = _DONOT_USE_FIELD;
            ViewBag.MultiSel = _DONOT_USE_MULTI_SEL;
            ViewBag.SelId = _DONOT_USE_SEL_ID;
            ViewBag.SubmitFunc = _DONOT_USE_SUBMIT;
            ViewBag.LinkField = _DONOT_USE_LINK_FIELD;
            ViewBag.TriggerUrl = _DONOT_USE_TRIGGER_URL;
            ViewBag.CurrentCS = cs;
            #region 获取选中的数据
            ViewBag.SelectData = "[]";
            ViewBag.SelectorValueField = _DONOT_USE_VFIELD;
            if (listVM.Ids?.Count > 0)
            {
                var tst = DC.Set<FrameworkRole>().Where(x => listVM.Ids.Contains(x.RoleName)).ToList();
                listVM.DC = Wtm.CreateDC();
                var originNeedPage = listVM.NeedPage;
                listVM.NeedPage = false;
                listVM.SearcherMode = ListVMSearchModeEnum.Batch;
                Type modelType = listVM.ModelType;
                var para = Expression.Parameter(modelType);
                var idproperty = modelType.GetSingleProperty(_DONOT_USE_VFIELD);
                var pro = Expression.Property(para, idproperty);
                listVM.ReplaceWhere = listVM.Ids.GetContainIdExpression(modelType, Expression.Parameter(modelType), pro);
                Regex r = new Regex("<script>.*?</script>");
                string selectData = r.Replace(await (listVM as IBasePagedListVM<TopBasePoco, BaseSearcher>).GetDataJson(), "");
                ViewBag.SelectData = selectData;
                listVM.IsSearched = false;
                listVM.SearcherMode = ListVMSearchModeEnum.Selector;
                listVM.NeedPage = originNeedPage;
            }
            #endregion

            return PartialView(listVM);
        }

        [ActionDescription("GetEmptyData")]
        public async Task<IActionResult> GetEmptyData(string _DONOT_USE_VMNAME)
        {
            var listVM = Wtm.CreateVM(_DONOT_USE_VMNAME, null, null, true) as IBasePagedListVM<TopBasePoco, BaseSearcher>;
            string data = await listVM.GetSingleDataJson(null, false);
            var rv = new ContentResult
            {
                ContentType = "application/json",
                Content = data
            };
            return rv;
        }


        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="_DONOT_USE_VMNAME"></param>
        /// <param name="_DONOT_USE_CS"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionDescription("GetPagingData")]
        public async Task<IActionResult> GetPagingData(string _DONOT_USE_VMNAME, string _DONOT_USE_CS)
        {
            var qs = new Dictionary<string, object>();
            foreach (var item in Request.Form.Keys)
            {
                qs.Add(item, Request.Form[item]);
            }
            //LogDebug.Info($"QueryString:{JsonConvert.SerializeObject(qs)}");
            //var vmType = Type.GetType(_DONOT_USE_VMNAME);
            //var vmCreater = vmType.GetConstructor(Type.EmptyTypes);
            //var listVM = vmCreater.Invoke(null) as BaseVM;
            Wtm.CurrentCS = _DONOT_USE_CS;
            var listVM = Wtm.CreateVM(_DONOT_USE_VMNAME, null, null, true) as IBasePagedListVM<TopBasePoco, BaseSearcher>;
            listVM.FC = qs;
            if (listVM is IBasePagedListVM<TopBasePoco, ISearcher>)
            {
                RedoUpdateModel(listVM);
                string url = "";
                if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
                {
                    Type[] checktypes = new Type[3] { typeof(FrameworkUserBase), typeof(FrameworkGroup), typeof(FrameworkRole) };
                    if (typeof(FrameworkUserBase).IsAssignableFrom(listVM.ModelType))
                    {
                        url = "/api/_frameworkuser/search";
                    }
                    else if (typeof(FrameworkGroup).IsAssignableFrom(listVM.ModelType))
                    {
                        url = "/api/_frameworkgroup/search";
                    }
                    else if (typeof(FrameworkRole).IsAssignableFrom(listVM.ModelType))
                    {
                        url = "/api/_frameworkrole/search";
                    }                    
                }
                if(string.IsNullOrEmpty(url) == false)
                {
                    var result = Wtm.CallAPI<string>("mainhost", url, HttpMethodEnum.POST, listVM.Searcher, 10).Result;
                    var rv = new ContentResult
                    {
                        ContentType = "application/json",
                        Content = result.Data
                };
                    return rv;
                }
                else
                {
                    var rv = new ContentResult
                    {
                        ContentType = "application/json",
                        Content = $@"{{""Data"":{listVM.GetDataJson()},""Count"":{listVM.Searcher.Count},""Msg"":""success"",""Code"":{StatusCodes.Status200OK}}}"
                    };
                    return rv;
                }
            }
            else
            {
                throw new Exception("Invalid Vm Name");
            }
        }


        /// <summary>
        /// 单元格编辑
        /// </summary>
        /// <param name="_DONOT_USE_VMNAME"></param>
        /// <param name="id">实体主键</param>
        /// <param name="field">属性名</param>
        /// <param name="value">属性值</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateModelProperty(string _DONOT_USE_VMNAME, Guid id, string field, string value)
        {
            if (value == null && Microsoft.Extensions.Primitives.StringValues.IsNullOrEmpty(Request.Form[nameof(value)]))
            {
                value = string.Empty;
            }
            var vm = Wtm.CreateVM(_DONOT_USE_VMNAME, id, null, true) as IBaseCRUDVM<TopBasePoco>;
            vm.Entity.SetPropertyValue(field, value);
            await DC.SaveChangesAsync();
            return JsonMore("Success");
        }

        #region Import/Export Excel

        /// <summary>
        /// Download Excel
        /// </summary>
        /// <param name="_DONOT_USE_VMNAME"></param>
        /// <param name="_DONOT_USE_CS"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionDescription("Export")]
        public async Task<IActionResult> GetExportExcel(string _DONOT_USE_VMNAME, string _DONOT_USE_CS)
        {
            var qs = new Dictionary<string, object>();
            foreach (var item in Request.Query.Keys)
            {
                qs.Add(item, Request.Query[item]);
            }
            foreach (var item in Request.Form)
            {
                if (qs.ContainsKey(item.Key) == false)
                {
                    qs.Add(item.Key, item.Value);
                }
            }
            var instanceType = Type.GetType(_DONOT_USE_VMNAME);

            Wtm.CurrentCS =  _DONOT_USE_CS;
            var listVM = Wtm.CreateVM(_DONOT_USE_VMNAME) as IBasePagedListVM<TopBasePoco, ISearcher>;

            listVM.FC = qs;
            if (listVM is IBasePagedListVM<TopBasePoco, ISearcher>)
            {
                RedoUpdateModel(listVM);

                listVM.SearcherMode = listVM.Ids != null && listVM.Ids.Count > 0 ? ListVMSearchModeEnum.CheckExport : ListVMSearchModeEnum.Export;

                var data = await listVM.GenerateExcel();
                HttpContext.Response.Cookies.Append("DONOTUSEDOWNLOADING", "0", new Microsoft.AspNetCore.Http.CookieOptions() { Path = "/", Expires = DateTime.Now.AddDays(2) });

                return File(data, "application/vnd.ms-excel", $"Export_{instanceType.Name}_{DateTime.Now.ToString("yyyy-MM-dd")}.xls");
            }
            else
            {
                throw new Exception("Invalid Vm Name");
            }
        }

        /// <summary>
        /// Download Excel Template
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionDescription("DownloadTemplate")]
        public IActionResult GetExcelTemplate(string _DONOT_USE_VMNAME, string _DONOT_USE_CS)
        {
            //Wtm.CurrentCS = _DONOT_USE_CS ?? "default";
            var importVM = Wtm.CreateVM(_DONOT_USE_VMNAME) as IBaseImport<BaseTemplateVM>;
            var qs = new Dictionary<string, string>();
            foreach (var item in Request.Query.Keys)
            {
                qs.Add(item, Request.Query[item]);
            }
            importVM.SetParms(qs);
            var data = importVM.GenerateTemplate(out string fileName);
            HttpContext.Response.Cookies.Append("DONOTUSEDOWNLOADING", "0", new Microsoft.AspNetCore.Http.CookieOptions() { Domain = "/", Expires = DateTime.Now.AddDays(2) });
            return File(data, "application/vnd.ms-excel", fileName);
        }

        #endregion

        [AllowAnonymous]
        [ActionDescription("Sys.ErrorHandle")]
        public async Task<IActionResult> Error()
        {
            var ex = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            ActionLog log = new ActionLog();
            log.LogType = ActionLogTypesEnum.Exception;
            log.ActionTime = DateTime.Now;
            log.ITCode = (await Wtm.GetLoginUserInfo ())?.ITCode ?? string.Empty;

            var controllerDes = ex.Error.TargetSite.DeclaringType.GetCustomAttributes(typeof(ActionDescriptionAttribute), false).Cast<ActionDescriptionAttribute>().FirstOrDefault();
            var actionDes = ex.Error.TargetSite.GetCustomAttributes(typeof(ActionDescriptionAttribute), false).Cast<ActionDescriptionAttribute>().FirstOrDefault();
            var postDes = ex.Error.TargetSite.GetCustomAttributes(typeof(HttpPostAttribute), false).Cast<HttpPostAttribute>().FirstOrDefault();
            //给日志的多语言属性赋值
            log.ModuleName = controllerDes?.GetDescription(ex.Error.TargetSite.DeclaringType) ?? ex.Error.TargetSite.DeclaringType.Name.Replace("Controller", string.Empty);
            log.ActionName = actionDes?.GetDescription(ex.Error.TargetSite.DeclaringType) ?? ex.Error.TargetSite.Name;
            if (postDes != null)
            {
                log.ActionName += "[P]";
            }
            log.ActionUrl = ex.Path;
            log.IP = HttpContext.Connection.RemoteIpAddress.ToString();
            log.Remark = ex.Error.ToString();
            if (string.IsNullOrEmpty(log.Remark) == false && log.Remark.Length > 2000)
            {
                log.Remark = log.Remark.Substring(0, 2000);
            }
            DateTime? starttime = HttpContext.Items["actionstarttime"] as DateTime?;
            if (starttime != null)
            {
                log.Duration = DateTime.Now.Subtract(starttime.Value).TotalSeconds;
            }
            var logger = HttpContext.RequestServices.GetRequiredService<ILogger<ActionLog>>();
            if (logger != null)
            {
                logger.Log<ActionLog>(LogLevel.Error, new EventId(), log, null, (a, b) =>
                {
                    return a.GetLogString();
                });
            }

            var rv = string.Empty;
            if (ConfigInfo.IsQuickDebug == true)
            {
                rv = ex.Error.ToString().Replace(Environment.NewLine, "<br />");
            }
            else
            {
                rv = ex.Error.Message.Replace(Environment.NewLine, "<br />"); ;
            }
            return BadRequest(rv);
        }

        [HttpPost]
        [ActionDescription("UploadFileRoute")]
        public async Task<IActionResult> Upload([FromServices] WtmFileProvider fp, string sm = null, string groupName = null, string subdir = null, string extra = null, bool IsTemprory = true, string _DONOT_USE_CS=null)
        {
            var FileData = Request.Form.Files[0];
            var file = await fp.Upload(FileData.FileName, FileData.Length, FileData.OpenReadStream(), groupName, subdir, extra, sm, Wtm.CreateDC(cskey: _DONOT_USE_CS));
            return JsonMore(new { Id = file.GetID(), Name = file.FileName });
        }

        [HttpPost]
        [ActionDescription("UploadFileRoute")]
        public async Task<IActionResult> UploadImage([FromServices] WtmFileProvider fp, string sm = null, string groupName = null, string subdir = null, string extra = null, bool IsTemprory = true, string _DONOT_USE_CS = null, int? width = null, int? height = null)
        {
            if (width == null && height == null)
            {
                return await Upload(fp, sm, groupName, subdir, extra, IsTemprory, _DONOT_USE_CS);
            }
            var FileData = Request.Form.Files[0];

            Image oimage = Image.Load(FileData.OpenReadStream());
            if (oimage == null)
            {
                return JsonMore(new { Id = string.Empty, Name = string.Empty }, StatusCodes.Status404NotFound);
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
            oimage.Mutate(x => x.Resize(width.Value, height.Value));
            await oimage.SaveAsJpegAsync (ms);
            ms.Position = 0;

            var file = await fp.Upload(FileData.FileName, ms.Length, ms, groupName, subdir, extra, sm, Wtm.CreateDC(cskey: _DONOT_USE_CS));
            oimage.Dispose();
            ms.Dispose();
            return JsonMore(new { Id = file.GetID(), Name = file.FileName });
        }

        [HttpPost]
        [ActionDescription("UploadForLayUIRichTextBox")]
        public async Task<IActionResult> UploadForLayUIRichTextBox([FromServices] WtmFileProvider fp, string _DONOT_USE_CS = null, string groupName = null, string subdir = null)
        {
            var FileData = Request.Form.Files[0];
            var file = await fp.Upload(FileData.FileName, FileData.Length, FileData.OpenReadStream(), groupName, subdir, dc: Wtm.CreateDC(cskey: _DONOT_USE_CS));
            if (file != null)
            {
                string url = $"/_Framework/GetFile?id={file.GetID()}&stream=true&_DONOT_USE_CS={CurrentCS}";
                return Content($"{{\"code\": 0 , \"msg\": \"\", \"data\": {{\"src\": \"{url}\"}}}}");

            }
            else
            {
                return Content($"{{\"code\": 1 , \"msg\": \"{MvcProgram._localizer["Sys.UploadFailed"]}\", \"data\": {{\"src\": \"\"}}}}");

            }

        }

        [ActionDescription("GetFileName")]
        public IActionResult GetFileName([FromServices] WtmFileProvider fp, Guid id, string _DONOT_USE_CS)
        {
            return Ok(fp.GetFileName(id.ToString(), Wtm.CreateDC(cskey: _DONOT_USE_CS)));
        }

        [ActionDescription("GetFile")]
        public async Task<IActionResult> GetFile([FromServices] WtmFileProvider fp, string id, bool stream = false, string _DONOT_USE_CS = null, int? width = null, int? height = null)
        {
            var file = fp.GetFile(id, true, Wtm.CreateDC(cskey: _DONOT_USE_CS));
            if (file == null)
            {
                return new EmptyResult();
            }
            Stream rv = null;
            try
            {
                rv = file.DataStream;
                Image oimage = Image.Load(rv);
                if (oimage != null && (width != null || height != null))
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
                    oimage.Mutate(x => x.Resize(width.Value, height.Value));
                    await oimage.SaveAsJpegAsync (ms);
                    rv.Dispose();
                    rv = ms;
                }
                else
                {

                }
            }
            catch { }
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
            if (ext == "mp4")
            {
                contenttype = $"video/mpeg4";
            }
            rv.Position = 0;
            if (stream == false)
            {
                return File(rv, contenttype, file.FileName ?? (Guid.NewGuid().ToString() + ext));
            }
            else
            {
                if (ext == "mp4")
                {
                    return File(rv, contenttype, enableRangeProcessing: true);
                }
                else
                {
                    Response.Headers.TryAdd("Content-Disposition", $"inline; filename=\"{HttpUtility.UrlEncode(file.FileName)}\"");
                    await rv.CopyToAsync(Response.Body);
                    rv.Dispose();
                    return new EmptyResult();
                }
            }
        }

        [ActionDescription("ViewFile")]
        public IActionResult ViewFile([FromServices] WtmFileProvider fp, string id, string width, string _DONOT_USE_CS = null)
        {
            var file = fp.GetFile(id, false, Wtm.CreateDC(cskey: _DONOT_USE_CS));
            string html = string.Empty;
            var ext = file.FileExt.ToLower();
            if (ext == "pdf")
            {
                html = $@"
<embed src=""/_Framework/GetFile?id={id}&stream=true"" width=""100%"" height=""100%"" type=""application/pdf"" ></embed>
            ";
            }
            else if (ext == "mp4")
            {
                html = $@"<video id='FileObject' controls='controls' style='{(string.IsNullOrEmpty(width) ? "" : $"width:{width}px")}'  border=0 src='/_Framework/GetFile?id={id}&stream=true&_DONOT_USE_CS={_DONOT_USE_CS}'></video>";
            }
            else
            {
                html = $@"<img id='FileObject' style='flex:auto;{(string.IsNullOrEmpty(width) ? "" : $"width:{width}px")}'  border=0 src='/_Framework/GetFile?id={id}&stream=true&_DONOT_USE_CS={_DONOT_USE_CS}'/>";
            }
            return Content(html);

        }

        [Public]
        public async Task<IActionResult> OutSide(string url)
        {
            url = HttpUtility.UrlDecode(url);
            string pagetitle = string.Empty;
            var menu = Utils.FindMenu(url, Wtm.GlobaInfo.AllMenus);
            if (menu == null)
            {
            }
            else
            {
                if (menu.ParentId != null)
                {
                    var pmenu = GlobaInfo.AllMenus.Where(x => x.ID == menu.ParentId).FirstOrDefault();
                    if (pmenu != null)
                    {
                        pmenu.PageName = Core.CoreProgram._localizer?[pmenu.PageName];

                        pagetitle = pmenu.PageName + " - ";
                    }
                }
                menu.PageName = Core.CoreProgram._localizer?[menu.PageName];

                pagetitle += menu.PageName;
            }
            if (Wtm.IsUrlPublic(url) || await Wtm.IsAccessable(url))
            {
                return Content($@"<title>{pagetitle}</title>
<iframe src='{url}' frameborder='0' class='layadmin-iframe'></iframe>");
            }
            else
            {
                throw new Exception(MvcProgram._localizer["Sys.NoPrivilege"]);
            }
        }


        [HttpGet]
        public IActionResult Menu()
        {
            var resultMenus = GlobaInfo.AllMenus.ToLayuiMenu(Wtm);
            return Content(JsonSerializer.Serialize(new { Code = 200, Msg = string.Empty, Data = resultMenus }, new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
            }), "application/json");
        }

        [AllowAnonymous]
        public async Task<IActionResult> IsAccessable(string url)
        {
            url = HttpUtility.UrlDecode(url);
            if (await Wtm.GetLoginUserInfo () == null)
            {
                if (Wtm.IsUrlPublic(url))
                {
                    return Ok(true);
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                bool canAccess = await Wtm.IsAccessable(url);
                return Ok(canAccess);
            }
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 3600)]
        public string GetGithubStarts()
        {
            return Wtm.ReadFromCache<string>("githubstar", () =>
            {
                var s = Wtm.CallAPI<github>("github", "/repos/dotnetcore/wtm").Result.Data;
                return s == null ? "" : s.stargazers_count.ToString();
            }, 1800);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 3600)]
        public ActionResult GetGithubInfo()
        {
            var rv = Wtm.ReadFromCache<string>("githubinfo", () =>
            {
                var s = Wtm.CallAPI<github>("github", "/repos/dotnetcore/wtm").Result;
                return JsonSerializer.Serialize(s);
            }, 1800);
            return Content(rv, "application/json");
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 3600)]
        public string Redirect()
        {
            return "";
        }

        private class github
        {
            public int stargazers_count { get; set; }
            public int forks_count { get; set; }
            public int subscribers_count { get; set; }
            public int open_issues_count { get; set; }
        }

        [AllowAnonymous]
        public ActionResult GetVerifyCode()
        {
            int codeW = 80;
            int codeH = 30;
            int fontSize = 16;
            string chkCode = string.Empty;
            Color[] color = { Color.Black, Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Brown, Color.DarkBlue, Color.PaleGreen };
            string[] font = { "Times New Roman" };
            char[] character = { '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'd', 'e', 'f', 'h', 'k', 'm', 'n', 'r', 'x', 'y', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X', 'Y' };
            //生成验证码字符串
            Random rnd = new Random();
            for (int i = 0; i < 4; i++)
            {
                chkCode += character[rnd.Next(character.Length)];
            }
            //写入Session用于验证码校验，可以对校验码进行加密，提高安全性
            HttpContext.Session.Set<string>("verify_code", chkCode);

            //创建画布
            Image bmp = new Image<Rgba32>(codeW, codeH);

            //画噪线
            for (int i = 0; i < 3; i++)
            {
                float x1 = rnd.Next(codeW);
                float y1 = rnd.Next(codeH);
                float x2 = rnd.Next(codeW);
                float y2 = rnd.Next(codeH);

                Color clr = color[rnd.Next(color.Length)];
                bmp.Mutate(x => x.DrawLines(clr, 1.0f, new PointF(x1,y1), new PointF(x2,y2)));
            }
            //画验证码
            for (int i = 0; i < chkCode.Length; i++)
            {
                Font ft = new Font(SystemFonts.Get("Arial"), fontSize);
                Color clr = color[rnd.Next(color.Length)];
                bmp.Mutate(x => x.DrawText(chkCode[i].ToString(),ft,clr,new PointF((float)i * 18, (float)0)));
            }
            //将验证码写入图片内存流中，以image/png格式输出
            MemoryStream ms = new MemoryStream();
            try
            {
                bmp.SaveAsPng(ms);
                return File(ms.ToArray(), "image/jpeg");
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                bmp.Dispose();
            }
        }

        [Public]
        public Dictionary<string, string> GetScriptLanguage()
        {
            Dictionary<string, string> rv = new Dictionary<string, string>();
            rv.Add("DONOTUSE_Text_LoadFailed", MvcProgram._localizer["Sys.LoadFailed"]);
            rv.Add("DONOTUSE_Text_SubmitFailed", MvcProgram._localizer["Sys.SubmitFailed"]);
            rv.Add("DONOTUSE_Text_PleaseSelect", MvcProgram._localizer["Sys.PleaseSelect"]);
            rv.Add("DONOTUSE_Text_FailedLoadData", MvcProgram._localizer["Sys.FailedLoadData"]);
            return rv;
        }

        [AllRights]
        [HttpPost]
        [ActionDescription("UploadForLayUIUEditor")]
        public async Task<IActionResult> UploadForLayUIUEditor([FromServices] WtmFileProvider fp, string _DONOT_USE_CS = "default", string groupName = null, string subdir = null)
        {
            IWtmFile file = null;
            if (Request.Form.Files != null && Request.Form.Files.Count() > 0)
            {
                //通过文件流方式上传附件
                var FileData = Request.Form.Files[0];
                file = await fp.Upload(FileData.FileName, FileData.Length, FileData.OpenReadStream(), groupName, subdir, dc: Wtm.CreateDC(cskey: _DONOT_USE_CS));
            }
            else if (Request.Form.Keys != null && Request.Form.ContainsKey("FileID"))
            {
                //通过Base64方式上传附件
                var FileData = Convert.FromBase64String(Request.Form["FileID"]);
                MemoryStream MS = new MemoryStream(FileData);
                file = await fp.Upload("SCRAWL_" + DateTime.Now.ToString("yyyyMMddHHmmssttt") + ".jpg", FileData.Length, MS, groupName, subdir, dc: Wtm.CreateDC(cskey: _DONOT_USE_CS));
                MS.Dispose();
            }



            if (file != null)
            {
                string url = $"/_Framework/GetFile?id={file.GetID()}&stream=true&_DONOT_USE_CS={CurrentCS}";
                return Content($"{{\"Code\": 200 , \"Msg\": \"success\", \"Data\": {{\"src\": \"{url}\",\"FileName\":\"{file.FileName}\"}}}}");

            }
            else
            {
                return Content($"{{\"code\": 1 , \"msg\": \"{MvcProgram._localizer["Sys.UploadFailed"]}\", \"data\": {{\"src\": \"\"}}}}");

            }


        }

        [Public]
        [ActionDescription("加载UEditor配置文件")]
        [ResponseCache(Duration = 3600)]
        [HttpGet]
        public IActionResult UEditorOptions()
        {
            if (ConfigInfo.UEditorOptions == null)
                throw new Exception($"Unregistered service: {nameof(ConfigInfo.UEditorOptions)}");
            return JsonMore(ConfigInfo.UEditorOptions);
        }

        [Public]
        public IActionResult SetLanguage(string culture)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return FFResult().AddCustomScript("location.reload();");
        }

        [Public]
        public async Task<IActionResult> SetTenant(string tenant)
        {
            await Wtm.SetCurrentTenant(tenant == "" ? null : tenant);
            var principal = (await Wtm.GetLoginUserInfo ()).CreatePrincipal();
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, null);
            return FFResult().AddCustomScript("location.reload();");
        }


        [Public]
        public IActionResult SetLanguageForBlazor(string culture, string redirect)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return Content($"<script>window.location.href='{HttpUtility.UrlDecode(redirect)}';</script>", "text/html");
        }


        [Public]
        [HttpGet]
        public IActionResult Redirect401()
        {
            return this.Unauthorized();
        }

        [Public]
        public async Task<ActionResult> RemoteEntry(string redirect)
        {
            if (string.IsNullOrEmpty(redirect))
            {
                redirect = "/";
            }
            if (await Wtm?.GetLoginUserInfo () != null)
            {
                var principal = (await Wtm?.GetLoginUserInfo ()).CreatePrincipal();
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, null);
            }
            return Content($"<script>window.location.href='{HttpUtility.UrlDecode(redirect)}'</script>", "text/html");
        }

        [AllRights]
        [HttpPost]
        public async Task<ActionResult> RemoveUserCacheByAccount(string[] itcode)
        {
            await Wtm.RemoveUserCache(itcode);
            return Ok();
        }

        [AllRights]
        [HttpPost]
        public async Task<ActionResult> RemoveUserCacheByRole(string[] rolecode)
        {
            await Wtm.RemoveUserCacheByRole(rolecode);
            return Ok();
        }

        [AllRights]
        [HttpPost]
        public async Task<ActionResult> RemoveUserCacheByGroup(string[] groupcode)
        {
            await Wtm.RemoveUserCacheByGroup(groupcode);
            return Ok();
        }
    }
}
