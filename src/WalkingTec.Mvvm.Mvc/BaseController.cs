using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;

using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc
{    
    /// <summary>
    /// MVC 控制器基类，所有使用 WTM 框架的 MVC 控制器均应继承此类；
    /// 内置 WTMContext 请求上下文及一系列代理属性，并提供模型重新初始化、重新验证、重新绑定等辅助方法
    /// </summary>
    public abstract class BaseController : Controller, IBaseController
    {
        /// <summary>
        /// WTM 请求级上下文，包含数据上下文、登录用户、配置、缓存等信息，由框架在请求时自动注入；
        /// 下方的 ConfigInfo、DC、Cache 等代理属性均由它转发
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public WTMContext Wtm { get; set; }


        /// <summary>
        /// 框架配置信息（转发自 Wtm.ConfigInfo）
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public Configs ConfigInfo { get => Wtm?.ConfigInfo; }

        /// <summary>
        /// 全局数据（转发自 Wtm.GlobaInfo），包含用户、角色、菜单等框架缓存数据
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public GlobalData GlobaInfo { get => Wtm?.GlobaInfo; }

        /// <summary>
        /// UI 服务（转发自 Wtm.UIService），提供各 UI 框架下的 HTML 控件生成等辅助方法
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public IUIService UIService { get => Wtm?.UIService; }

        /// <summary>
        /// 分布式缓存（转发自 Wtm.Cache）
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public IDistributedCache Cache { get => Wtm?.Cache; }

        /// <summary>
        /// 当前使用的数据库连接字符串（转发自 Wtm.CurrentCS）
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public string CurrentCS { get => Wtm?.CurrentCS; }

        /// <summary>
        /// 当前连接的数据库类型（转发自 Wtm.CurrentDbType），未指定时为 null
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public DBTypeEnum? CurrentDbType { get => Wtm?.CurrentDbType; }

        /// <summary>
        /// 父窗口 ID（转发自 Wtm.ParentWindowId），弹窗场景下指向打开当前窗口的页面
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public string ParentWindowId { get => Wtm?.ParentWindowId; }

        /// <summary>
        /// 当前窗口 ID（转发自 Wtm.CurrentWindowId），标识当前页面所在的框架窗口
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public string CurrentWindowId { get => Wtm?.CurrentWindowId; }

        /// <summary>
        /// 窗口 ID 链（转发自 Wtm.WindowIds），记录从最外层到当前窗口的层级路径
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public string WindowIds { get => Wtm?.WindowIds; }

        #region DataContext

        /// <summary>
        /// 数据上下文（转发自 Wtm.DC），即当前可用的 EF Core DbContext 实例
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public IDataContext DC { get => Wtm?.DC; }

        #endregion

        #region URL
        /// <summary>
        /// 站点根地址（转发自 Wtm.BaseUrl）
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public string BaseUrl { get => Wtm?.BaseUrl; }
        #endregion

        /// <summary>
        /// 多语言本地化资源（转发自 Wtm.Localizer）
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public IStringLocalizer Localizer { get => Wtm?.Localizer; }


        //-------------------------------------------方法------------------------------------//

        #region ReInit model
        /// <summary>
        /// 根据 VM 上 ReInitAttribute 配置的模式，在模型验证成功或失败后决定是否调用 DoReInit 重新初始化 VM
        /// </summary>
        /// <param name="msd">模型状态字典</param>
        /// <param name="model">要重新初始化的 VM</param>
        private void SetReInit(ModelStateDictionary msd, BaseVM model)
        {
            var reinit = model.GetType().GetTypeInfo().GetCustomAttributes(typeof(ReInitAttribute), false).Cast<ReInitAttribute>().SingleOrDefault();

            //验证成功时，配置为 SUCCESSONLY 或 ALWAYS 模式才重新初始化
            if (ModelState.IsValid)
            {
                if (reinit != null && (reinit.ReInitMode == ReInitModes.SUCCESSONLY || reinit.ReInitMode == ReInitModes.ALWAYS))
                {
                    model.DoReInit();
                }
            }
            else
            {
                //验证失败时，未配置特性或配置为 FAILEDONLY、ALWAYS 模式则重新初始化
                if (reinit == null || (reinit.ReInitMode == ReInitModes.FAILEDONLY || reinit.ReInitMode == ReInitModes.ALWAYS))
                {
                    model.DoReInit();
                }
            }
        }
        #endregion

        #region Validate model
        /// <summary>
        /// 对指定对象重新执行模型验证，并收集所有验证失败的错误信息
        /// </summary>
        /// <param name="item">要验证的对象</param>
        /// <returns>错误字典，键为字段名，值为拼接后的错误消息</returns>
        [NonAction]
        public Dictionary<string, string> RedoValidation(object item)
        {
            Dictionary<string, string> rv = new Dictionary<string, string>();
            TryValidateModel(item);

            foreach (var e in ControllerContext.ModelState)
            {
                if (e.Value.ValidationState == ModelValidationState.Invalid)
                {
                    rv.Add(e.Key, e.Value.Errors.Select(x => x.ErrorMessage).ToSepratedString());
                }
            }

            return rv;
        }
        #endregion

        #region update viewmodel
        /// <summary>
        /// Set viewmodel's properties to the matching items posted by user
        /// </summary>
        /// <param name="vm">ViewModel</param>
        /// <param name="prefix">prefix</param>
        /// <returns>true if success</returns>
        [NonAction]
        public bool RedoUpdateModel(object vm, string prefix = null)
        {
            try
            {
                BaseVM bvm = vm as BaseVM;
                foreach (var item in bvm.FC.Keys)
                {
                    PropertyHelper.SetPropertyValue(vm, item, bvm.FC[item], prefix, true);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 创建返回前端脚本的结果对象，并在响应头中标记 IsScript，以便前端按脚本方式处理返回内容
        /// </summary>
        /// <returns>前端脚本执行结果对象</returns>
        [NonAction]
        public FResult FFResult()
        {
            var rv = new FResult
            {
                Controller = this
            };
            try
            {
                rv.Controller.Response?.Headers?.Append("IsScript", "true");
            }
            catch { }
            return rv;
        }

        /// <summary>
        /// 返回统一格式的 JSON 数据，包含 Msg、Code、Data 三个字段
        /// </summary>
        /// <param name="data">要返回的数据</param>
        /// <param name="statusCode">状态码，默认 200</param>
        /// <param name="msg">提示消息，默认 success</param>
        /// <returns>Ok 响应结果</returns>
        protected ActionResult JsonMore(object data, int statusCode = StatusCodes.Status200OK, string msg = "success")
        {
            return Ok(new { Msg = msg, Code = statusCode, Data = data });
        }

    }

}
