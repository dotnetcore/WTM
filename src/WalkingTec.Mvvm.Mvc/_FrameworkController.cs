using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public JsonResult GetDomainMenus(Guid did)
        {
            var data = DC.Set<FrameworkMenu>()
             .Where(x => x.DomainId == did)
             .Select(x => new SimpleMenu { Id = x.ID, ActionId = x.ActionId, IsPublic = x.IsPublic, Url = x.Url, ParentId = x.ParentId })
             .ToList();
            return Json(data);
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
            listVM.OnAfterInitList += (self) =>
            {
                self.RemoveActionColumn();
                self.RemoveAction();
            };
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
            if(listVM.Ids?.Count > 0)
            {
                var originNeedPage = listVM.NeedPage;
                listVM.NeedPage = false;
                listVM.SearcherMode = ListVMSearchModeEnum.Batch;

                ViewBag.SelectData = (listVM as IBasePagedListVM<TopBasePoco, BaseSearcher>).GetDataJson();

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
            string data = listVM.GetSingleDataJson(null);
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
        /// <returns></returns>
        [HttpPost]
        [ActionDescription("获取分页数据")]
        public IActionResult GetPagingData(string _DONOT_USE_VMNAME)
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
        /// <returns></returns>
        [HttpPost]
        [ActionDescription("导出")]
        public IActionResult GetExportExcel(string _DONOT_USE_VMNAME)
        {
            var qs = new Dictionary<string, object>();
            foreach (var item in Request.Query.Keys)
            {
                qs.Add(item, Request.Query[item]);
            }
            foreach (var item in Request.Form)
            {
                if(qs.ContainsKey(item.Key) == false)
                {
                    qs.Add(item.Key, item.Value);
                }
            }
            var instanceType = Type.GetType(_DONOT_USE_VMNAME);

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
        public IActionResult GetExcelTemplate(string _DONOT_USE_VMNAME)
        {
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
        public IActionResult Upload(SaveFileModeEnum? sm = null, string groupName = null, bool IsTemprory = true)
        {
            var FileData = Request.Form.Files[0];
            sm = sm == null ? ConfigInfo.SaveFileMode : sm;
            var vm = CreateVM<FileAttachmentVM>();
            vm.Entity.FileName = FileData.FileName;
            vm.Entity.Length = FileData.Length;
            vm.Entity.UploadTime = DateTime.Now;
            vm.Entity.SaveFileMode = sm;
            vm = FileHelper.GetFileByteForUpload(vm, FileData.OpenReadStream(), ConfigInfo, FileData.FileName, sm, groupName);
            vm.Entity.IsTemprory = IsTemprory;
            if ((!string.IsNullOrEmpty(vm.Entity.Path) && (vm.Entity.SaveFileMode == SaveFileModeEnum.Local|| vm.Entity.SaveFileMode == SaveFileModeEnum.DFS)) || (vm.Entity.FileData != null && vm.Entity.SaveFileMode == SaveFileModeEnum.Database))
            {
                vm.DoAdd();
                return Json(new { Id = vm.Entity.ID.ToString(), Name = vm.Entity.FileName });
            }
            return Json(new { Id = string.Empty, Name = string.Empty }, StatusCodes.Status404NotFound);
        }

        [ActionDescription("获取文件名")]
        public IActionResult GetFileName(Guid id)
        {
            FileAttachmentVM vm = CreateVM<FileAttachmentVM>(id);
            return Ok(vm.Entity.FileName);
        }

        [ActionDescription("获取文件")]
        public IActionResult GetFile(Guid id, bool stream = false)
        {
            if (id == Guid.Empty)
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }
            var vm = CreateVM<FileAttachmentVM>(id);
            var data = FileHelper.GetFileByteForDownLoadByVM(vm, ConfigInfo);
            if(data == null)
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
        public IActionResult ViewFile(Guid id)
        {
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
                html = $@"<img id='FileObject' width='100%' height='100%' border=0 src='/_Framework/GetFile?id={id}&stream=true&DONOTUSECSName={CurrentCS}'/>";
            }
            return Content(html);

        }

        [AllRights]
        public IActionResult OutSide(string url)
        {
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
    }
}
