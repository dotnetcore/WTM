using System.Text.Json;
using Microsoft.Extensions.Localization;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 框架核心程序类，保存全局本地化资源和默认的 Json 序列化配置
    /// </summary>
    public class CoreProgram
    {
        /// <summary>
        /// 系统本地化资源，用于读取多语言文本
        /// </summary>
        public static IStringLocalizer _localizer {
            get;
            set;
        }

        /// <summary>
        /// 默认的 System.Text.Json 序列化配置
        /// </summary>
        public static JsonSerializerOptions DefaultJsonOption
        {
            get;set;
        }

        /// <summary>
        /// Post 请求默认的 System.Text.Json 序列化配置
        /// </summary>
        public static JsonSerializerOptions DefaultPostJsonOption
        {
            get; set;
        }


        /// <summary>
        /// 框架内置程序集名称列表
        /// </summary>
        public static string[] Buildindll = new string[]
            {
                    "WalkingTec.Mvvm.Core",
                    "WalkingTec.Mvvm.Mvc",
                    "WalkingTec.Mvvm.Admin",
                    "WalkingTec.Mvvm.Taghelpers"
            };

    }
}
