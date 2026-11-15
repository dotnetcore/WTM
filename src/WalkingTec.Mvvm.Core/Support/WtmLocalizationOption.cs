using System;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// WTM 本地化配置选项，用于指定多语言资源所在的类型
    /// </summary>
    public class WtmLocalizationOption
    {
        /// <summary>
        /// 提供本地化资源的类型（通常为资源类所在的类型）
        /// </summary>
        public Type LocalizationType { get; set; }
    }
}
