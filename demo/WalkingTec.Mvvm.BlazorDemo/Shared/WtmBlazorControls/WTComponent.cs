using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace WtmBlazorControls
{
    public class WTComponent : ComponentBase
    {
        /// <summary>
        /// 获得/设置 用户自定义属性
        /// </summary>
        [Parameter(CaptureUnmatchedValues = true)]
        public IDictionary<string, object> AdditionalAttributes { get; set; }

        [CascadingParameter]
        public WTRow ParentRow { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if(ParentRow != null)
            {
                ParentRow.Items.Add((b) => this.BuildRenderTree(b));
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if(ParentRow != null && firstRender == true)
            {
                return;
            }
            else
            {
                base.OnAfterRender(firstRender);
            }
        }
    }
}
