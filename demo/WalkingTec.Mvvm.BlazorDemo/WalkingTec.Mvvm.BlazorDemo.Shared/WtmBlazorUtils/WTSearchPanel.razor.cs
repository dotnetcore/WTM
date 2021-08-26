using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace WtmBlazorUtils
{
    public partial class WTSearchPanel
    {

        protected virtual string ClassName => CssBuilder.Default("card")
            .AddClass("text-center", IsCenter)
            .AddClass($"border-{Color.ToDescriptionString()}", Color != Color.None)
            .Build();

        /// <summary>
        ///  设置Body Class组件样式
        /// </summary>
        protected virtual string BodyClassName => CssBuilder.Default("card-body")
            .AddClass($"text-{Color.ToDescriptionString()}", Color != Color.None)
            .Build();

        [Inject]
        public WtmBlazorContext WtmBlazor { get; set; }

        /// <summary>
        /// 获得/设置 CardBody
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// 获得/设置Card颜色
        /// </summary>
        [Parameter]
        public Color Color { get; set; }

        /// <summary>
        /// 设置是否居中
        /// </summary>
        [Parameter]
        public bool IsCenter { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnSearch {get;set;}

        [Parameter]
        public bool? Collapsed { get; set; }

        private void Toggle(MouseEventArgs args)
        {
            if(Collapsed == null)
            {
                Collapsed = false;
            }
            Collapsed = !Collapsed;
        }

        protected override void OnInitialized()
        {
            if(Collapsed == null)
            {
                Collapsed = !WtmBlazor.ConfigInfo.UIOptions.SearchPanel.DefaultExpand;
            }
        }
    }
}
