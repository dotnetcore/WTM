using System;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace WtmBlazorUtils
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
            Type componentTyype = null;
            CultureInfo cl = CultureInfo.CurrentUICulture;
            while (cl != null)
            {
                string typename = MainPageType.Namespace + "." + MainPageType.Name + "_" + cl.Name.ToLower();
                componentTyype = Type.GetType(typename);
                if(componentTyype != null)
                {
                    break;
                }
                cl = cl.Parent;
            }
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
