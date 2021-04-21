using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Globalization;

namespace WtmBlazorControls
{
    public class CulturePage<TValue> : ComponentBase
    {
        [Parameter]
        public Type MainPageType { get; set; }

        [Parameter]
        public TValue Model { get; set; }

        /// <summary>
        /// 获得/设置 绑定字段值变化回调委托
        /// </summary>
        [Parameter]
        public EventCallback<TValue> ModelChanged { get; set; }
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            Type componentTyype = Type.GetType(MainPageType.Namespace + "." + MainPageType.Name + "_" + CultureInfo.CurrentUICulture.Name.ToLower());
            if (componentTyype != null)
            {
                builder.OpenComponent(0, componentTyype);
                builder.AddAttribute(1, "model", Model);
                builder.CloseComponent();

            }
            base.BuildRenderTree(builder);
        }
    }
}
