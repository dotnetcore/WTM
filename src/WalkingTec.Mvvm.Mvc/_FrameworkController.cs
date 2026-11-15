using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using DUWENINK.Captcha;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Models;
using WalkingTec.Mvvm.Core.Support.FileHandlers;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// 框架内置 API 控制器，提供选择器弹窗、分页数据查询、Excel 导入导出、文件上传下载、
    /// 验证码、菜单、异常处理、多语言切换、租户切换、用户缓存清理等框架级通用功能
    /// </summary>
    [AllRights]
    [ActionDescription("Framework")]
    public class _FrameworkController(ISecurityCodeHelper securityCode) : BaseController
    {
        /// <summary>
        /// 
        /// 验证码生成辅助组件，用于生成图形验证码的文本内容和图片数据
        /// </summary>
        private readonly ISecurityCodeHelper _securityCode = securityCode;





        /// <summary>
        /// 通用选择器弹窗页面：根据传入的 ListVM 名称创建列表 VM，
        /// 以选择器（Selector）模式返回分部视图，供前端在弹窗中选择单条或多条数据
        /// </summary>
        /// <param name="_DONOT_USE_VMNAME">列表 VM 的完整类型名称</param>
        /// <param name="_DONOT_USE_KFIELD">显示文本字段名，即弹窗列表中展示的列</param>
        /// <param name="_DONOT_USE_VFIELD">取值字段名，选中后回填到目标表单的字段值</param>
        /// <param name="_DONOT_USE_FIELD">目标表单字段名</param>
        /// <param name="_DONOT_USE_MULTI_SEL">是否允许多选</param>
        /// <param name="_DONOT_USE_SEL_ID">已选中的值，用于打开弹窗时回显</param>
        /// <param name="_DONOT_USE_SUBMIT">选择完成后提交的回调函数名</param>
        /// <param name="_DONOT_USE_LINK_FIELD">联动字段名，选择后触发其他控件刷新</param>
        /// <param name="_DONOT_USE_TRIGGER_URL">联动触发的 URL</param>
        /// <param name="_DONOT_USE_CURRENTCS">当前使用的数据库连接字符串标识</param>
        /// <returns>选择器弹窗的分部视图</returns>
        [HttpPost]
        [Public]
        public IActionResult Selector(string _DONOT_USE_VMNAME
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
            //切换到前端指定的数据库连接
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
                //构造仅包含已选记录的查询条件，并生成用于回显的 JSON 数据（过滤掉其中的 script 标签以防注入）
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
                string selectData = r.Replace((listVM as IBasePagedListVM<TopBasePoco, BaseSearcher>).GetDataJson(), "");
                ViewBag.SelectData = selectData;
                listVM.IsSearched = false;
                listVM.SearcherMode = ListVMSearchModeEnum.Selector;
                listVM.NeedPage = originNeedPage;
            }
            #endregion

            return PartialView(listVM);
        }

        /// <summary>
        /// 获取指定 ListVM 的空白数据结构 JSON，用于前端初始化表格等信息
        /// </summary>
        /// <param name="_DONOT_USE_VMNAME">列表 VM 的完整类型名称</param>
        /// <returns>包含空数据结构的 JSON 内容结果</returns>
        [ActionDescription("GetEmptyData")]
        public IActionResult GetEmptyData(string _DONOT_USE_VMNAME)
        {
            var listVM = Wtm.CreateVM(_DONOT_USE_VMNAME, null, null, true) as IBasePagedListVM<TopBasePoco, BaseSearcher>;
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
        [ActionDescription("GetPagingData")]
        public IActionResult GetPagingData(string _DONOT_USE_VMNAME, string _DONOT_USE_CS)
        {
            //收集表单提交的搜索条件，供 VM 模型绑定使用
            var qs = new Dictionary<string, object>();
            foreach (var item in Request.Form.Keys)
            {
                qs.Add(item, Request.Form[item]);
            }
            Wtm.CurrentCS = _DONOT_USE_CS;
            var listVM = Wtm.CreateVM(_DONOT_USE_VMNAME, null, null, true) as IBasePagedListVM<TopBasePoco, BaseSearcher>;
            listVM.FC = qs;
            if (listVM is IBasePagedListVM<TopBasePoco, ISearcher>)
            {
                RedoUpdateModel(listVM);
                //配置了主站点且当前非租户用户时，用户/用户组/角色等框架基础数据需转发到主站点统一查询，保证分布式环境数据一致
                string url = "";
                if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
                {
                    Type[] checktypes = new Type[3] { typeof(FrameworkUserBase), typeof(FrameworkGroup), typeof(FrameworkRole) };
                    if (typeof(FrameworkUserBase).IsAssignableFrom(listVM.ModelType))
                    {
                        url = "/api/admin/frameworkuser/search";
                    }
                    else if (typeof(FrameworkGroup).IsAssignableFrom(listVM.ModelType))
                    {
                        url = "/api/admin/frameworkgroup/search";
                    }
                    else if (typeof(FrameworkRole).IsAssignableFrom(listVM.ModelType))
                    {
                        url = "/api/admin/frameworkrole/search";
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
        public IActionResult UpdateModelProperty(string _DONOT_USE_VMNAME, Guid id, string field, string value)
        {
            //表单中未提交该字段时视为清空操作，将属性值置为空字符串
            if (value == null && Microsoft.Extensions.Primitives.StringValues.IsNullOrEmpty(Request.Form[nameof(value)]))
            {
                value = string.Empty;
            }
            var vm = Wtm.CreateVM(_DONOT_USE_VMNAME, id, null, true) as IBaseCRUDVM<TopBasePoco>;
            vm.Entity.SetPropertyValue(field, value);
            DC.SaveChanges();
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
        public IActionResult GetExportExcel(string _DONOT_USE_VMNAME, string _DONOT_USE_CS)
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

                //有勾选行时按勾选导出，否则按当前搜索条件导出
                listVM.SearcherMode = listVM.Ids != null && listVM.Ids.Count > 0 ? ListVMSearchModeEnum.CheckExport : ListVMSearchModeEnum.Export;

                //生成 Excel 文件，并写入下载完成标记 Cookie，前端据此关闭"正在下载"等待提示
                var data = listVM.GenerateExcel();
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
        /// <param name="_DONOT_USE_VMNAME">导入 VM 的完整类型名称</param>
        /// <param name="_DONOT_USE_CS">当前使用的数据库连接字符串标识</param>
        /// <returns></returns>
        [HttpGet]
        [ActionDescription("DownloadTemplate")]
        public IActionResult GetExcelTemplate(string _DONOT_USE_VMNAME, string _DONOT_USE_CS)
        {
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

        /// <summary>
        /// 全局异常处理接口：将未处理异常记录为 Exception 日志，
        /// 并返回错误信息（快速调试模式下返回完整异常堆栈，否则仅返回异常消息）
        /// </summary>
        /// <returns>包含异常信息的 400（BadRequest）响应</returns>
        [AllowAnonymous]
        [ActionDescription("Sys.ErrorHandle")]
        public IActionResult Error()
        {
            var ex = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            ActionLog log = new ActionLog();
            log.LogType = ActionLogTypesEnum.Exception;
            log.ActionTime = DateTime.Now;
            log.ITCode = Wtm.LoginUserInfo?.ITCode ?? string.Empty;

            //读取异常发生位置所在控制器/方法上的描述特性，用于拼装日志中的模块名和动作名
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
            //从 HttpContext.Items 中取出请求开始时间，计算异常请求的执行耗时
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

        /// <summary>
        /// 通用文件上传接口：通过 WtmFileProvider 保存上传的文件，返回文件 ID 和文件名
        /// </summary>
        /// <param name="fp">文件处理提供程序</param>
        /// <param name="sm">文件说明信息</param>
        /// <param name="groupName">附件分组名称</param>
        /// <param name="subdir">附件存放的子目录</param>
        /// <param name="extra">附件附加信息</param>
        /// <param name="IsTemprory">是否为临时文件（当前实现未使用）</param>
        /// <param name="_DONOT_USE_CS">当前使用的数据库连接字符串标识</param>
        /// <returns>包含文件 ID 和文件名的 JSON 结果</returns>
        [HttpPost]
        [ActionDescription("UploadFileRoute")]
        public IActionResult Upload([FromServices] WtmFileProvider fp, string sm = null, string groupName = null, string subdir = null, string extra = null, bool IsTemprory = true, string _DONOT_USE_CS=null)
        {
            var FileData = Request.Form.Files[0];
            var file = fp.Upload(FileData.FileName, FileData.Length, FileData.OpenReadStream(), groupName, subdir, extra, sm, Wtm.CreateDC(cskey: _DONOT_USE_CS));
            return JsonMore(new { Id = file.GetID(), Name = file.FileName });
        }

        /// <summary>
        /// 图片上传接口：可按指定的宽高对图片进行缩放后再上传，未指定宽高时等价于普通文件上传
        /// </summary>
        /// <param name="fp">文件处理提供程序</param>
        /// <param name="sm">文件说明信息</param>
        /// <param name="groupName">附件分组名称</param>
        /// <param name="subdir">附件存放的子目录</param>
        /// <param name="extra">附件附加信息</param>
        /// <param name="IsTemprory">是否为临时文件（当前实现未使用）</param>
        /// <param name="_DONOT_USE_CS">当前使用的数据库连接字符串标识</param>
        /// <param name="width">缩放后的图片宽度，为空时按高度等比计算</param>
        /// <param name="height">缩放后的图片高度，为空时按宽度等比计算</param>
        /// <returns>包含文件 ID 和文件名的 JSON 结果</returns>
        [HttpPost]
        [ActionDescription("UploadFileRoute")]
        public IActionResult UploadImage([FromServices] WtmFileProvider fp, string sm = null, string groupName = null, string subdir = null, string extra = null, bool IsTemprory = true, string _DONOT_USE_CS = null, int? width = null, int? height = null)
        {
            //未指定宽和高时，直接走普通文件上传逻辑
            if (width == null && height == null)
            {
                return Upload(fp, sm, groupName, subdir, extra, IsTemprory, _DONOT_USE_CS);
            }
            var FileData = Request.Form.Files[0];

            Image oimage = Image.Load(FileData.OpenReadStream());
            if (oimage == null)
            {
                return JsonMore(new { Id = string.Empty, Name = string.Empty }, StatusCodes.Status404NotFound);
            }
            //只指定一边时，按原图宽高比等比计算另一边，避免图像变形
            if (width == null)
            {
                width = height * oimage.Width / oimage.Height;
            }
            if (height == null)
            {
                height = width * oimage.Height / oimage.Width;
            }
            //将缩放后的图片以 JPEG 格式写入内存流后再上传
            MemoryStream ms = new MemoryStream();
            oimage.Mutate(x => x.Resize(width.Value, height.Value));
            oimage.SaveAsJpeg(ms);
            ms.Position = 0;

            var file = fp.Upload(FileData.FileName, ms.Length, ms, groupName, subdir, extra, sm, Wtm.CreateDC(cskey: _DONOT_USE_CS));
            oimage.Dispose();
            ms.Dispose();
            return JsonMore(new { Id = file.GetID(), Name = file.FileName });
        }

        /// <summary>
        /// LayUI 富文本编辑器的图片上传接口：上传成功后返回符合 LayUI 要求的 JSON 格式（包含文件访问地址）
        /// </summary>
        /// <param name="fp">文件处理提供程序</param>
        /// <param name="_DONOT_USE_CS">当前使用的数据库连接字符串标识</param>
        /// <param name="groupName">附件分组名称</param>
        /// <param name="subdir">附件存放的子目录</param>
        /// <returns>LayUI 富文本要求的上传结果 JSON</returns>
        [HttpPost]
        [ActionDescription("UploadForLayUIRichTextBox")]
        public IActionResult UploadForLayUIRichTextBox([FromServices] WtmFileProvider fp, string _DONOT_USE_CS = null, string groupName = null, string subdir = null)
        {
            var FileData = Request.Form.Files[0];
            var file = fp.Upload(FileData.FileName, FileData.Length, FileData.OpenReadStream(), groupName, subdir, dc: Wtm.CreateDC(cskey: _DONOT_USE_CS));
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

        /// <summary>
        /// 根据文件 ID 获取文件的原始名称
        /// </summary>
        /// <param name="fp">文件处理提供程序</param>
        /// <param name="id">文件 ID</param>
        /// <param name="_DONOT_USE_CS">当前使用的数据库连接字符串标识</param>
        /// <returns>包含文件名称的响应</returns>
        [ActionDescription("GetFileName")]
        public IActionResult GetFileName([FromServices] WtmFileProvider fp, Guid id, string _DONOT_USE_CS)
        {
            return Ok(fp.GetFileName(id.ToString(), Wtm.CreateDC(cskey: _DONOT_USE_CS)));
        }

        /// <summary>
        /// 读取指定文件内容并输出：可按需生成指定宽高的缩略图；
        /// stream 为 true 时以内联方式输出到响应体，否则作为附件下载
        /// </summary>
        /// <param name="fp">文件处理提供程序</param>
        /// <param name="id">文件 ID</param>
        /// <param name="stream">是否以流式内联方式输出</param>
        /// <param name="_DONOT_USE_CS">当前使用的数据库连接字符串标识</param>
        /// <param name="width">缩略图宽度，为空时按高度等比计算</param>
        /// <param name="height">缩略图高度，为空时按宽度等比计算</param>
        /// <returns>文件内容结果；文件不存在时返回空结果</returns>
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
                //尝试将文件按图片加载；若指定了宽或高则生成缩略图，非图片文件加载失败时忽略异常并使用原始流
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
                    oimage.SaveAsJpeg(ms);
                    rv.Dispose();
                    rv = ms;
                }
                else
                {

                }
            }
            catch { }
            //根据文件扩展名推断响应的 Content-Type
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
            //stream 为 false 时作为附件下载，否则以内联方式直接写入响应体
            rv.Position = 0;
            if (stream == false)
            {
                return File(rv, contenttype, file.FileName ?? (Guid.NewGuid().ToString() + ext));
            }
            else
            {
                //视频文件启用断点续传（Range 处理），支持播放器拖动进度
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

        /// <summary>
        /// 在线预览指定文件：根据文件扩展名生成对应的 HTML 预览标签
        /// （PDF 嵌入对象、视频播放器或图片）
        /// </summary>
        /// <param name="fp">文件处理提供程序</param>
        /// <param name="id">文件 ID</param>
        /// <param name="width">预览宽度，单位像素，为空时使用默认宽度</param>
        /// <param name="_DONOT_USE_CS">当前使用的数据库连接字符串标识</param>
        /// <returns>包含预览 HTML 的内容结果</returns>
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

        /// <summary>
        /// 外部链接包装页：将指定的外部 URL 嵌入 iframe，
        /// 并根据菜单配置生成页面标题，访问前校验该地址是否公开或当前用户是否有权限
        /// </summary>
        /// <param name="url">外部链接地址（URL 编码）</param>
        /// <returns>包含 iframe 的 HTML 内容结果</returns>
        [Public]
        public IActionResult OutSide(string url)
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
            if (Wtm.IsUrlPublic(url) || Wtm.IsAccessable(url))
            {
                return Content($@"<title>{pagetitle}</title>
<iframe src='{url}' frameborder='0' class='layadmin-iframe'></iframe>");
            }
            else
            {
                throw new Exception(MvcProgram._localizer["Sys.NoPrivilege"]);
            }
        }


        /// <summary>
        /// 获取当前用户可见的菜单树，转换为 LayUI 菜单格式后以 JSON 返回
        /// </summary>
        /// <returns>包含菜单数据的 JSON 内容结果</returns>
        [HttpGet]
        public IActionResult Menu()
        {
            var resultMenus = GlobaInfo.AllMenus.ToLayuiMenu(Wtm);
            return Content(JsonSerializer.Serialize(new { Code = 200, Msg = string.Empty, Data = resultMenus }, new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
            }), "application/json");
        }

        /// <summary>
        /// 判断指定 URL 是否可被当前用户访问：未登录时判断是否为公开页面，已登录时判断是否在其权限范围内
        /// </summary>
        /// <param name="url">要检查的页面地址（URL 编码）</param>
        /// <returns>有权限时返回 true，否则返回未授权响应</returns>
        [AllowAnonymous]
        public IActionResult IsAccessable(string url)
        {
            url = HttpUtility.UrlDecode(url);
            if (Wtm.LoginUserInfo == null)
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
                bool canAccess = Wtm.IsAccessable(url);
                return Ok(canAccess);
            }
        }

        /// <summary>
        /// 获取 WTM 项目在 GitHub 上的 Star 数量（结果缓存 30 分钟，响应缓存 1 小时）
        /// </summary>
        /// <returns>Star 数量字符串</returns>
        [AllowAnonymous]
        [ResponseCache(Duration = 3600)]
        public string GetGithubStarts()
        {
            return Wtm.ReadFromCache<string>("githubstar", () =>
            {
                var s = Wtm.CallAPI<Github>("github", "/repos/dotnetcore/wtm").Result.Data;
                return s == null ? "" : s.stargazers_count.ToString();
            }, 1800);
        }

        /// <summary>
        /// 获取 WTM 项目在 GitHub 上的仓库信息（结果缓存 30 分钟，响应缓存 1 小时）
        /// </summary>
        /// <returns>包含仓库信息的 JSON 内容结果</returns>
        [AllowAnonymous]
        [ResponseCache(Duration = 3600)]
        public ActionResult GetGithubInfo()
        {
            var rv = Wtm.ReadFromCache<string>("githubinfo", () =>
            {
                var s = Wtm.CallAPI<Github>("github", "/repos/dotnetcore/wtm").Result;
                return JsonSerializer.Serialize(s);
            }, 1800);
            return Content(rv, "application/json");
        }

        /// <summary>
        /// 重定向辅助接口，当前实现返回空字符串（预留）
        /// </summary>
        /// <returns>空字符串</returns>
        [AllowAnonymous]
        [ResponseCache(Duration = 3600)]
        public string Redirect()
        {
            return "";
        }

        /// <summary>
        /// GitHub 仓库信息实体（内部类，用于反序列化 GitHub API 返回值）
        /// </summary>
        private class Github
        {
            /// <summary>
            /// Star 数量
            /// </summary>
            public int stargazers_count { get; set; }
            /// <summary>
            /// Fork 数量
            /// </summary>
            public int forks_count { get; set; }
            /// <summary>
            /// 订阅者数量
            /// </summary>
            public int subscribers_count { get; set; }
            /// <summary>
            /// 未关闭的 Issue 数量
            /// </summary>
            public int open_issues_count { get; set; }
        }

        /// <summary>
        /// 生成图形验证码：随机生成 4 位验证码文本写入 Session 供校验，并返回验证码图片
        /// </summary>
        /// <returns>验证码图片（PNG）内容结果</returns>
        [AllowAnonymous]
        public ActionResult GetVerifyCode()
        {
            var chkCode = _securityCode.GetRandomEnDigitalText(4);
            //写入Session用于验证码校验，可以对校验码进行加密，提高安全性
            HttpContext.Session.Set<string>("verify_code", chkCode);
            var imgbyte = _securityCode.GetEnDigitalCodeByte(chkCode);
            return File(imgbyte, "image/png");
        
        }

        /// <summary>
        /// 获取前端脚本中使用的多语言提示文本（加载失败、提交失败、请选择等）
        /// </summary>
        /// <returns>脚本多语言文本字典</returns>
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

        /// <summary>
        /// UEditor 富文本编辑器的附件上传接口：支持文件流上传和 Base64（涂鸦图片）上传两种方式
        /// </summary>
        /// <param name="fp">文件处理提供程序</param>
        /// <param name="_DONOT_USE_CS">当前使用的数据库连接字符串标识</param>
        /// <param name="groupName">附件分组名称</param>
        /// <param name="subdir">附件存放的子目录</param>
        /// <returns>UEditor 要求的上传结果 JSON</returns>
        [AllRights]
        [HttpPost]
        [ActionDescription("UploadForLayUIUEditor")]
        public IActionResult UploadForLayUIUEditor([FromServices] WtmFileProvider fp, string _DONOT_USE_CS = "default", string groupName = null, string subdir = null)
        {
            IWtmFile file = null;
            if (Request.Form.Files != null && Request.Form.Files.Count() > 0)
            {
                //通过文件流方式上传附件
                var FileData = Request.Form.Files[0];
                file = fp.Upload(FileData.FileName, FileData.Length, FileData.OpenReadStream(), groupName, subdir, dc: Wtm.CreateDC(cskey: _DONOT_USE_CS));
            }
            else if (Request.Form.Keys != null && Request.Form.ContainsKey("FileID"))
            {
                //通过Base64方式上传附件
                var FileData = Convert.FromBase64String(Request.Form["FileID"]);
                MemoryStream MS = new MemoryStream(FileData);
                file = fp.Upload("SCRAWL_" + DateTime.Now.ToString("yyyyMMddHHmmssttt") + ".jpg", FileData.Length, MS, groupName, subdir, dc: Wtm.CreateDC(cskey: _DONOT_USE_CS));
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
        /// <summary>
        /// 获取 UEditor 富文本编辑器的配置文件内容
        /// </summary>
        /// <returns>UEditor 配置的 JSON 结果</returns>
        [HttpGet]
        public IActionResult UEditorOptions()
        {
            if (ConfigInfo.UEditorOptions == null)
                throw new Exception($"Unregistered service: {nameof(ConfigInfo.UEditorOptions)}");
            return JsonMore(ConfigInfo.UEditorOptions);
        }

        /// <summary>
        /// 切换当前界面语言：写入文化 Cookie（有效期一年）并刷新页面
        /// </summary>
        /// <param name="culture">语言文化代码，如 zh-CN、en-US</param>
        /// <returns>刷新页面的脚本结果</returns>
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

        /// <summary>
        /// 切换当前租户：空字符串表示切回默认（非租户）模式
        /// </summary>
        /// <param name="tenant">租户标识</param>
        /// <returns>刷新页面的脚本结果</returns>
        [Public]
        public IActionResult SetTenant(string tenant)
        {
            Wtm.SetCurrentTenant(tenant == "" ? null : tenant);
            //切换租户后重建登录主体并重新签发 Cookie 身份，使新的租户信息立即生效
            var principal = Wtm.LoginUserInfo.CreatePrincipal();
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, null);
            return FFResult().AddCustomScript("location.reload();");
        }


        /// <summary>
        /// 供 Blazor 使用的语言切换接口：写入文化 Cookie 后以脚本跳转到指定地址
        /// </summary>
        /// <param name="culture">语言文化代码，如 zh-CN、en-US</param>
        /// <param name="redirect">跳转地址（URL 编码）</param>
        /// <returns>包含跳转脚本的 HTML 内容结果</returns>
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


        /// <summary>
        /// 返回 401 未授权响应，供前端在未登录时跳转使用
        /// </summary>
        /// <returns>401 未授权响应</returns>
        [Public]
        [HttpGet]
        public IActionResult Redirect401()
        {
            return this.Unauthorized();
        }

        /// <summary>
        /// 远程进入接口：为已具备登录信息的用户重新签发 Cookie 身份后跳转到指定页面
        /// （用于 Blazor 等远程宿主场景接入）
        /// </summary>
        /// <param name="redirect">跳转地址（URL 编码），为空时跳转到首页</param>
        /// <returns>包含跳转脚本的 HTML 内容结果</returns>
        [Public]
        public async Task<ActionResult> RemoteEntry(string redirect)
        {
            if (string.IsNullOrEmpty(redirect))
            {
                redirect = "/";
            }
            //若当前已存在登录信息，则重新签发 Cookie 身份，供远程宿主（如 Blazor）进入时使用
            if (Wtm?.LoginUserInfo != null)
            {
                var principal = Wtm.LoginUserInfo.CreatePrincipal();
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, null);
            }
            return Content($"<script>window.location.href='{HttpUtility.UrlDecode(redirect)}'</script>", "text/html");
        }

        /// <summary>
        /// 按用户账号清除指定用户的缓存信息
        /// </summary>
        /// <param name="itcode">用户账号（ITCode）数组</param>
        /// <returns>操作结果</returns>
        [AllRights]
        [HttpPost]
        public async Task<ActionResult> RemoveUserCacheByAccount(string[] itcode)
        {
            await Wtm.RemoveUserCache(itcode);
            return Ok();
        }

        /// <summary>
        /// 按角色代码清除拥有这些角色的用户的缓存信息
        /// </summary>
        /// <param name="rolecode">角色代码数组</param>
        /// <returns>操作结果</returns>
        [AllRights]
        [HttpPost]
        public async Task<ActionResult> RemoveUserCacheByRole(string[] rolecode)
        {
            await Wtm.RemoveUserCacheByRole(rolecode);
            return Ok();
        }

        /// <summary>
        /// 按用户组代码清除这些组下用户的缓存信息
        /// </summary>
        /// <param name="groupcode">用户组代码数组</param>
        /// <returns>操作结果</returns>
        [AllRights]
        [HttpPost]
        public async Task<ActionResult> RemoveUserCacheByGroup(string[] groupcode)
        {
            await Wtm.RemoveUserCacheByGroup(groupcode);
            return Ok();
        }
    }
}

