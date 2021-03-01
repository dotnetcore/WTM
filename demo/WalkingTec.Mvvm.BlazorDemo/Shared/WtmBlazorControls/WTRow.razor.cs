using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;

namespace WtmBlazorControls
{
    public enum ItemsPerRowEnum
    {
        One = 12, Two = 6, Three = 4, Four = 3, Six = 2, Twelve = 1
    }
    public partial class WTRow : WTComponent
    {
        private string ClassString => CssBuilder.Default("row")
            .AddClassFromAttributes(AdditionalAttributes)
            .Build();

        /// <summary>
        /// 获得/设置 子组件
        /// </summary>
        [Parameter]
        public RenderFragment RowItems { get; set; }

        public List<RenderFragment> Items { get; set; } = new List<RenderFragment>();
        private bool FirstRender { get; set; } = true;

        private string ChildClass
        {
            get
            {
                return $"form-group col-12 col-sm-{(int)ItemsPerRow}";
            }
        }
        /// <summary>
        /// 设置一行显示多少个子组件
        /// </summary>
        [Parameter]
        public ItemsPerRowEnum ItemsPerRow { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                FirstRender = false;
            }
            if (RowItems != null)
            {
                RowItems = null;
                StateHasChanged();
            }
        }
    }
}
