using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace WtmBlazorUtils
{
    public class WTComponent : ComponentBase
    {
        /// <summary>
        /// 获得/设置 用户自定义属性
        /// </summary>
        [Parameter(CaptureUnmatchedValues = true)]
        public IDictionary<string, object> AdditionalAttributes { get; set; }

    }
}
