using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;

using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// 控制器基础接口，定义 WTM 框架中控制器可访问的上下文对象，
    /// 包括 Wtm 上下文、系统配置、全局数据、数据上下文、缓存、多语言资源等
    /// </summary>
    public interface IBaseController
    {
        /// <summary>
        /// 框架上下文，提供当前请求的会话信息、数据上下文、缓存、配置等核心对象
        /// </summary>
        WTMContext Wtm { get; set; }

        /// <summary>
        /// 系统配置信息
        /// </summary>
        Configs ConfigInfo { get; }
        /// <summary>
        /// 全局数据，包括菜单、登录用户信息等全局缓存内容
        /// </summary>
        GlobalData GlobaInfo { get; }
        /// <summary>
        /// 当前使用的数据库连接字符串标识
        /// </summary>
        string CurrentCS { get; }

        /// <summary>
        /// 当前连接字符串对应的数据库类型
        /// </summary>
        DBTypeEnum? CurrentDbType { get;  }

        /// <summary>
        /// 当前请求使用的数据上下文
        /// </summary>
        IDataContext DC { get; }

        /// <summary>
        /// 分布式缓存
        /// </summary>
        IDistributedCache Cache { get; }

        /// <summary>
        /// 当前站点的基础地址
        /// </summary>
        string BaseUrl { get;  }

        /// <summary>
        /// 模型状态字典，用于保存模型验证结果
        /// </summary>
        ModelStateDictionary ModelState { get; }

        /// <summary>
        /// 多语言资源访问器
        /// </summary>
        IStringLocalizer Localizer { get; }
    }
}
