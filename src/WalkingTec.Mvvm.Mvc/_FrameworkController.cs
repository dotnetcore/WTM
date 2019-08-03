using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc
{
    [AllRights]
    [ActionDescription("框架")]
    public class _FrameworkController : BaseController
    {
        private static JsonSerializerSettings _jsonSerializerSettings;
        public static JsonSerializerSettings JsonSerializerSettings
        {
            get
            {
                if (_jsonSerializerSettings == null)
                {
                    _jsonSerializerSettings = new JsonSerializerSettings();
                    if (_jsonSerializerSettings.Converters == null)
                    {
                        _jsonSerializerSettings.Converters = new List<JsonConverter>();
                    }
                    //_jsonSerializerSettings.Converters.Add(GlobalServices.GetRequiredService<EnumConverterService>());
                    //_jsonSerializerSettings.Converters.Add(GlobalServices.GetRequiredService<BoolConverterService>());
                }
                return _jsonSerializerSettings;
            }
        }

        [HttpPost]
        public IActionResult Selector(string _DONOT_USE_VMNAME
            , string _DONOT_USE_KFIELD
            , string _DONOT_USE_VFIELD
            , string _DONOT_USE_FIELD
            , bool _DONOT_USE_MULTI_SEL
            , string _DONOT_USE_SEL_ID
            , string _DONOT_USE_SUBMIT
        )
        {
            var listVM = CreateVM(_DONOT_USE_VMNAME, null, null, true) as IBasePagedListVM<TopBasePoco, ISearcher>;

            if (listVM is IBasePagedListVM<TopBasePoco, ISearcher>)
            {
                RedoUpdateModel(listVM);
            }

            listVM.SearcherMode = ListVMSearchModeEnum.Selector;
            listVM.RemoveActionColumn();
            listVM.RemoveAction();
            if (listVM.Searcher != null)
            {
                var searcher = listVM.Searcher;
                searcher.CopyContext(listVM);
                searcher.DoInit();
            }
            ViewBag.TextName = _DONOT_USE_KFIELD;
            ViewBag.ValName = _DONOT_USE_VFIELD;
            ViewBag.FieldName = _DONOT_USE_FIELD;
            ViewBag.MultiSel = _DONOT_USE_MULTI_SEL;
            ViewBag.SelId = _DONOT_USE_SEL_ID;
            ViewBag.SubmitFunc = _DONOT_USE_SUBMIT;

            #region 获取选中的数据
            ViewBag.SelectData = "[]";
            if (listVM.Ids?.Count > 0)
            {
                var originNeedPage = listVM.NeedPage;
                listVM.NeedPage = false;
                listVM.SearcherMode = ListVMSearchModeEnum.Batch;
                Regex r = new Regex("<script>.*?</script>");
                ViewBag.SelectData = r.Replace((listVM as IBasePagedListVM<TopBasePoco, BaseSearcher>).GetDataJson(), "");                
                listVM.IsSearched = false;
                listVM.SearcherMode = ListVMSearchModeEnum.Selector;
                listVM.NeedPage = originNeedPage;
            }
            #endregion

            return PartialView(listVM);
        }

        [ActionDescription("获取单行空数据")]
        public IActionResult GetEmptyData(string _DONOT_USE_VMNAME)
        {
            var listVM = CreateVM(_DONOT_USE_VMNAME, null, null, true) as IBasePagedListVM<TopBasePoco, BaseSearcher>;
            string data = listVM.GetSingleDataJson(null, false);
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
        [ActionDescription("获取分页数据")]
        public IActionResult GetPagingData(string _DONOT_USE_VMNAME, string _DONOT_USE_CS)
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
            CurrentCS = _DONOT_USE_CS ?? "default";
            var listVM = CreateVM(_DONOT_USE_VMNAME, null, null, true) as IBasePagedListVM<TopBasePoco, BaseSearcher>;
            listVM.FC = qs;
            if (listVM is IBasePagedListVM<TopBasePoco, ISearcher>)
            {
                RedoUpdateModel(listVM);
                var rv = new ContentResult
                {
                    ContentType = "application/json",
                    Content = $@"{{""Data"":{listVM.GetDataJson()},""Count"":{listVM.Searcher.Count},""Msg"":""success"",""Code"":{StatusCodes.Status200OK}}}"
                };
                return rv;
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
        [ActionDescription("修改属性")]
        public IActionResult UpdateModelProperty(string _DONOT_USE_VMNAME, Guid id, string field, string value)
        {
            if (value == null && Microsoft.Extensions.Primitives.StringValues.IsNullOrEmpty(Request.Form[nameof(value)]))
            {
                value = string.Empty;
            }
            var vm = CreateVM(_DONOT_USE_VMNAME, id, null, true) as IBaseCRUDVM<TopBasePoco>;
            vm.Entity.SetPropertyValue(field, value);
            DC.SaveChanges();
            return Json("Success");
        }

        #region Import/Export Excel

        /// <summary>
        /// Download Excel
        /// </summary>
        /// <param name="_DONOT_USE_VMNAME"></param>
        /// <param name="_DONOT_USE_CS"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionDescription("导出")]
        public IActionResult GetExportExcel(string _DONOT_USE_VMNAME, string _DONOT_USE_CS = "default")
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

            CurrentCS = _DONOT_USE_CS ?? "default";
            var listVM = CreateVM(_DONOT_USE_VMNAME) as IBasePagedListVM<TopBasePoco, ISearcher>;

            listVM.FC = qs;
            if (listVM is IBasePagedListVM<TopBasePoco, ISearcher>)
            {
                RedoUpdateModel(listVM);

                listVM.SearcherMode = listVM.Ids != null && listVM.Ids.Count > 0 ? ListVMSearchModeEnum.CheckExport : ListVMSearchModeEnum.Export;

                var data = listVM.GenerateExcel();
                HttpContext.Response.Cookies.Append("DONOTUSEDOWNLOADING", "0", new CookieOptions() { Path = "/", Expires = DateTime.Now.AddDays(2) });

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
        [ActionDescription("获取模板")]
        public IActionResult GetExcelTemplate(string _DONOT_USE_VMNAME, string _DONOT_USE_CS = "default")
        {
            CurrentCS = _DONOT_USE_CS ?? "default";
            var importVM = CreateVM(_DONOT_USE_VMNAME) as IBaseImport<BaseTemplateVM>;
            var qs = new Dictionary<string, string>();
            foreach (var item in Request.Query.Keys)
            {
                qs.Add(item, Request.Query[item]);
            }
            importVM.SetParms(qs);
            var data = importVM.GenerateTemplate(out string fileName);
            HttpContext.Response.Cookies.Append("DONOTUSEDOWNLOADING", "0", new CookieOptions() { Domain = "/", Expires = DateTime.Now.AddDays(2) });
            return File(data, "application/vnd.ms-excel", fileName);
        }

        #endregion

        [Public]
        [ActionDescription("错误处理")]
        public IActionResult Error()
        {
            var ex = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (ConfigInfo.EnableLog == true)
            {
                ActionLog log = new ActionLog();
                log.LogType = ActionLogTypesEnum.Exception;
                log.ActionTime = DateTime.Now;
                log.ITCode = LoginUserInfo?.ITCode ?? string.Empty;

                var controllerDes = ex.Error.TargetSite.DeclaringType.GetCustomAttributes(typeof(ActionDescriptionAttribute), false).Cast<ActionDescriptionAttribute>().FirstOrDefault();
                var actionDes = ex.Error.TargetSite.GetCustomAttributes(typeof(ActionDescriptionAttribute), false).Cast<ActionDescriptionAttribute>().FirstOrDefault();
                var postDes = ex.Error.TargetSite.GetCustomAttributes(typeof(HttpPostAttribute), false).Cast<HttpPostAttribute>().FirstOrDefault();
                //给日志的多语言属性赋值
                log.ModuleName = controllerDes?.Description ?? ex.Error.TargetSite.DeclaringType.Name.Replace("Controller", string.Empty);
                log.ActionName = actionDes?.Description ?? ex.Error.TargetSite.Name;
                if (postDes != null)
                {
                    log.ActionName += "[P]";
                }
                log.ActionUrl = ex.Path;
                log.IP = HttpContext.Connection.RemoteIpAddress.ToString();
                log.Remark = ex.Error.ToString();
                if (string.IsNullOrEmpty(log.Remark) == false && log.Remark.Length > 1000)
                {
                    log.Remark = log.Remark.Substring(0, 1000);
                }
                DateTime? starttime = HttpContext.Items["actionstarttime"] as DateTime?;
                if (starttime != null)
                {
                    log.Duration = DateTime.Now.Subtract(starttime.Value).TotalSeconds;
                }
                using (var dc = CreateDC(true))
                {
                    dc.Set<ActionLog>().Add(log);
                    dc.SaveChanges();
                }
            }
            var rv = string.Empty;
            if (ConfigInfo.IsQuickDebug == true)
            {
                rv = ex.Error.ToString().Replace(Environment.NewLine, "</br>");
            }
            else
            {
                rv = ex.Error.Message.Replace(Environment.NewLine, "</br>"); ;
            }
            return BadRequest(rv);
        }

        [HttpPost]
        [ActionDescription("UploadFileRoute")]
        public IActionResult Upload(SaveFileModeEnum? sm = null, string groupName = null, bool IsTemprory = true, string _DONOT_USE_CS = "default")
        {
            CurrentCS = _DONOT_USE_CS ?? "default";
            var FileData = Request.Form.Files[0];
            sm = sm == null ? ConfigInfo.FileUploadOptions.SaveFileMode : sm;
            var vm = CreateVM<FileAttachmentVM>();
            vm.Entity.FileName = FileData.FileName;
            vm.Entity.Length = FileData.Length;
            vm.Entity.UploadTime = DateTime.Now;
            vm.Entity.SaveFileMode = sm;
            vm = FileHelper.GetFileByteForUpload(vm, FileData.OpenReadStream(), ConfigInfo, FileData.FileName, sm, groupName);
            vm.Entity.IsTemprory = IsTemprory;
            if ((!string.IsNullOrEmpty(vm.Entity.Path) && (vm.Entity.SaveFileMode == SaveFileModeEnum.Local || vm.Entity.SaveFileMode == SaveFileModeEnum.DFS)) || (vm.Entity.FileData != null && vm.Entity.SaveFileMode == SaveFileModeEnum.Database))
            {
                vm.DoAdd();
                return Json(new { Id = vm.Entity.ID.ToString(), Name = vm.Entity.FileName });
            }
            return Json(new { Id = string.Empty, Name = string.Empty }, StatusCodes.Status404NotFound);
        }

        [HttpPost]
        [ActionDescription("UploadFileRoute")]
        public IActionResult UploadImage(SaveFileModeEnum? sm = null, string groupName = null, bool IsTemprory = true, string _DONOT_USE_CS = "default", int? width = null, int? height = null)
        {
            if (width == null && height == null)
            {
                return Upload(sm, groupName, IsTemprory, _DONOT_USE_CS);
            }
            CurrentCS = _DONOT_USE_CS ?? "default";
            var FileData = Request.Form.Files[0];
            sm = sm == null ? ConfigInfo.FileUploadOptions.SaveFileMode : sm;
            var vm = CreateVM<FileAttachmentVM>();

            Image oimage = Image.FromStream(FileData.OpenReadStream());
            if (oimage == null)
            {
                return Json(new { Id = string.Empty, Name = string.Empty }, StatusCodes.Status404NotFound);
            }
            if (width == null)
            {
                width = height * oimage.Width / oimage.Height;
            }
            if (height == null)
            {
                height = width * oimage.Height /oimage.Width ;
            }
            MemoryStream ms = new MemoryStream();
            oimage.GetThumbnailImage(width.Value, height.Value, null, IntPtr.Zero).Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            vm.Entity.FileName = FileData.FileName.Replace(".", "") + ".jpg";
            vm.Entity.UploadTime = DateTime.Now;
            vm.Entity.SaveFileMode = sm;
            vm = FileHelper.GetFileByteForUpload(vm, ms, ConfigInfo, FileData.FileName.Replace(".", "") + ".jpg", sm, groupName);
            vm.Entity.Length = ms.Length;
            vm.Entity.IsTemprory = IsTemprory;
            oimage.Dispose();

            if ((!string.IsNullOrEmpty(vm.Entity.Path) && (vm.Entity.SaveFileMode == SaveFileModeEnum.Local || vm.Entity.SaveFileMode == SaveFileModeEnum.DFS)) || (vm.Entity.FileData != null && vm.Entity.SaveFileMode == SaveFileModeEnum.Database))
            {
                vm.DoAdd();
                return Json(new { Id = vm.Entity.ID.ToString(), Name = vm.Entity.FileName });
            }
            return Json(new { Id = string.Empty, Name = string.Empty }, StatusCodes.Status404NotFound);
        }


        [HttpPost]
        [ActionDescription("UploadForLayUIRichTextBox")]
        public IActionResult UploadForLayUIRichTextBox(string _DONOT_USE_CS = "default")
        {
            CurrentCS = _DONOT_USE_CS ?? "default";
            var FileData = Request.Form.Files[0];
            var sm = ConfigInfo.FileUploadOptions.SaveFileMode;
            var vm = CreateVM<FileAttachmentVM>();
            vm.Entity.FileName = FileData.FileName;
            vm.Entity.Length = FileData.Length;
            vm.Entity.UploadTime = DateTime.Now;
            vm.Entity.SaveFileMode = sm;
            vm = FileHelper.GetFileByteForUpload(vm, FileData.OpenReadStream(), ConfigInfo, FileData.FileName, sm);
            vm.Entity.IsTemprory = false;
            if ((!string.IsNullOrEmpty(vm.Entity.Path) && (vm.Entity.SaveFileMode == SaveFileModeEnum.Local || vm.Entity.SaveFileMode == SaveFileModeEnum.DFS)) || (vm.Entity.FileData != null && vm.Entity.SaveFileMode == SaveFileModeEnum.Database))
            {
                vm.DoAdd();
                string url = $"/_Framework/GetFile?id={vm.Entity.ID}&stream=true&_DONOT_USE_CS={CurrentCS}";
                return Content($"{{\"code\": 0 , \"msg\": \"\", \"data\": {{\"src\": \"{url}\"}}}}");
            }
            return Content($"{{\"code\": 1 , \"msg\": \"上传失败\", \"data\": {{\"src\": \"\"}}}}");
        }

        [ActionDescription("获取文件名")]
        public IActionResult GetFileName(Guid id, string _DONOT_USE_CS = "default")
        {
            CurrentCS = _DONOT_USE_CS ?? "default";
            FileAttachmentVM vm = CreateVM<FileAttachmentVM>(id);
            return Ok(vm.Entity.FileName);
        }

        [ActionDescription("获取文件")]
        public IActionResult GetFile(Guid id, bool stream = false, string _DONOT_USE_CS = "default",int? width = null, int? height = null)
        {
            CurrentCS = _DONOT_USE_CS ?? "default";
            if (id == Guid.Empty)
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }
            var vm = CreateVM<FileAttachmentVM>(id);
            var data = FileHelper.GetFileByteForDownLoadByVM(vm, ConfigInfo);
            if (data == null)
            {
                data = new byte[0];
            }
            try
            {
                MemoryStream ms = new MemoryStream(data);
                Image oimage = Image.FromStream(ms);
                ms.Close();
                if (oimage != null && (width != null || height != null))
                {
                    if (width == null)
                    {
                        width = height * oimage.Height / oimage.Width;
                    }
                    if (height == null)
                    {
                        height = width * oimage.Width / oimage.Height;
                    }
                    ms = new MemoryStream();
                    oimage.GetThumbnailImage(width.Value, height.Value, null, IntPtr.Zero).Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    data = ms.ToArray();
                    oimage.Dispose();
                }
            }
            catch { }
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
            if (ext == "mp4")
            {
                contenttype = $"video/mpeg4";
            }
            if (stream == false)
            {
                return File(data, contenttype, vm.Entity.FileName ?? (Guid.NewGuid().ToString() + ext));
            }
            else
            {
                Response.Body.Write(data, 0, data.Count());
                return new EmptyResult();
            }
        }

        [ActionDescription("查看文件")]
        public IActionResult ViewFile(Guid id, string _DONOT_USE_CS = "default")
        {
            CurrentCS = _DONOT_USE_CS ?? "default";
            string html = string.Empty;
            FileAttachmentVM vm = CreateVM<FileAttachmentVM>(id);
            if (vm.Entity.FileExt.ToLower() == "pdf")
            {
                html = $@"
            <object  classid=""clsid:CA8A9780-280D-11CF-A24D-444553540000"" width=""100%"" height=""100%"" border=""0""
            id=""FileObject"" name=""pdf"" VIEWASTEXT>
            <param name=""toolbar"" value=""false"">
            <param name=""_Version"" value=""65539"">
            <param name=""_ExtentX"" value=""20108"">
            <param name=""_ExtentY"" value=""10866"">
            <param name=""_StockProps"" value=""0"">
            <param name=""SRC"" value=""/_Framework/GetFile?id={id}&stream=true"">
           </object>
            ";
            }
            else
            {
                html = $@"<img id='FileObject'  border=0 src='/_Framework/GetFile?id={id}&stream=true&_DONOT_USE_CS={_DONOT_USE_CS}'/>";
            }
            return Content(html);

        }

        [AllRights]
        public IActionResult OutSide(string url)
        {
            url = HttpUtility.UrlDecode(url);
            var ctrlActDesc = this.ControllerContext.ActionDescriptor as ControllerActionDescriptor;
            string pagetitle = "";
            var menu = Utils.FindMenu(url);
            if (menu == null)
            {
                var ctrlDes = ctrlActDesc.ControllerTypeInfo.GetCustomAttributes(typeof(ActionDescriptionAttribute), false).Cast<ActionDescriptionAttribute>().FirstOrDefault();
                var actDes = ctrlActDesc.MethodInfo.GetCustomAttributes(typeof(ActionDescriptionAttribute), false).Cast<ActionDescriptionAttribute>().FirstOrDefault();
                if (actDes != null)
                {
                    if (ctrlDes != null)
                    {
                        pagetitle = ctrlDes.Description + " - ";
                    }
                    pagetitle += actDes.Description;
                }
            }
            else
            {
                if (menu.ParentId != null)
                {
                    var pmenu = GlobaInfo.AllMenus.Where(x => x.ID == menu.ParentId).FirstOrDefault();
                    if (pmenu != null)
                    {
                        pagetitle = pmenu.PageName + " - ";
                    }
                }
                pagetitle += menu.PageName;
            }
            HttpContext.Response.Cookies.Append("pagetitle", pagetitle);

            if (LoginUserInfo.IsAccessable(url) == true)
            {
                return Content($"<iframe width='100%' height='100%' frameborder=0 border=0 src='{url}'></iframe>");
            }
            else
            {
                throw new Exception("您没有访问该页面的权限");
            }
        }

        [Public]
        [HttpPost]
        [CrossDomain]
        public IActionResult Login(string userid, string password)
        {
            var user = DC.Set<FrameworkUserBase>()
    .Include(x => x.UserRoles).Include(x => x.UserGroups)
    .Where(x => x.ITCode.ToLower() == userid.ToLower() && x.Password == Utils.GetMD5String(password) && x.IsValid)
    .SingleOrDefault();

            //如果没有找到则输出错误
            if (user == null)
            {
                return BadRequest("登录失败");
            }
            var roleIDs = user.UserRoles.Select(x => x.RoleId).ToList();
            var groupIDs = user.UserGroups.Select(x => x.GroupId).ToList();
            //查找登录用户的数据权限
            var dpris = DC.Set<DataPrivilege>()
                .Where(x => x.UserId == user.ID || (x.GroupId != null && groupIDs.Contains(x.GroupId.Value)))
                .ToList();
            //生成并返回登录用户信息
            LoginUserInfo rv = new LoginUserInfo();
            rv.Id = user.ID;
            rv.ITCode = user.ITCode;
            rv.Name = user.Name;
            rv.Roles = DC.Set<FrameworkRole>().Where(x => user.UserRoles.Select(y => y.RoleId).Contains(x.ID)).ToList();
            rv.Groups = DC.Set<FrameworkGroup>().Where(x => user.UserGroups.Select(y => y.GroupId).Contains(x.ID)).ToList();
            rv.DataPrivileges = dpris;
            //查找登录用户的页面权限
            var pris = DC.Set<FunctionPrivilege>()
                .Where(x => x.UserId == user.ID || (x.RoleId != null && roleIDs.Contains(x.RoleId.Value)))
                .ToList();
            rv.FunctionPrivileges = pris;
            LoginUserInfo = rv;
            return Ok("Success");
        }

        [Public]
        [CrossDomain]
        public IActionResult IsAccessable(string url)
        {
            url = HttpUtility.UrlDecode(url);
            if (LoginUserInfo == null)
            {
                return Unauthorized();
            }
            else
            {
                bool canAccess = LoginUserInfo.IsAccessable(url);
                return Ok(canAccess);
            }
        }

        [Public]
        [ResponseCache(Duration = 3600)]
        public string GetGithubStarts()
        {
            return ReadFromCache<string>("githubstar", () =>
            {
                var s = APIHelper.CallAPI<github>("https://api.github.com/repos/dotnetcore/wtm").Result;
                return s.stargazers_count.ToString();
            }, 1800);
        }

        [Public]
        [ResponseCache(Duration = 3600)]
        public string Redirect()
        {
            return "";
        }

        private class github
        {
            public int stargazers_count { get; set; }
        }

    }
}
