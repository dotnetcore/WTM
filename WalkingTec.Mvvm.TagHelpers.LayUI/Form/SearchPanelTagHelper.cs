using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:searchpanel", Attributes = REQUIRED_ATTR_NAME, TagStructure = TagStructure.NormalOrSelfClosing)]
    public class SearchPanelTagHelper : FormTagHelper
    {
        /// <summary>
        /// 搜索按钮前缀
        /// </summary>
        public const string SEARCH_BTN_ID_PREFIX = "wtSearchBtn_";
        /// <summary>
        /// 重置按钮前缀
        /// </summary>
        public const string RESET_BTN_ID_PREFIX = "wtResetBtn_";
        private IBasePagedListVM<TopBasePoco, BaseSearcher> _listVM;
        private IBasePagedListVM<TopBasePoco, BaseSearcher> ListVM
        {
            get
            {
                if (_listVM == null)
                {
                    _listVM = Vm?.Model as IBasePagedListVM<TopBasePoco, BaseSearcher>;
                }
                return _listVM;
            }
        }

        private string _gridIdUserSet;

        private string _gridId;
        /// <summary>
        /// 关联的 Grid 组件的 Id
        /// </summary>
        public string GridId
        {
            get
            {
                if (string.IsNullOrEmpty(_gridId))
                {
                    if (string.IsNullOrEmpty(_gridIdUserSet))
                    {
                        _gridId = $"{DataTableTagHelper.TABLE_ID_PREFIX}{ListVM.UniqueId}";
                    }
                    else
                    {
                        _gridId = _gridIdUserSet;
                    }
                }
                return _gridId;
            }
            set
            {
                _gridId = value;
                _gridIdUserSet = value;
            }
        }

        private string _tableJSVar;
        /// <summary>
        /// datatable 渲染之后返回对象的变量名
        /// </summary>
        public string TableJSVar
        {
            get
            {
                if (string.IsNullOrEmpty(_tableJSVar))
                {
                    _tableJSVar = $"{DataTableTagHelper.TABLE_JSVAR_PREFIX}{(string.IsNullOrEmpty(_gridIdUserSet) ? ListVM.UniqueId : _gridIdUserSet)}";
                }
                return _tableJSVar;
            }
        }

        private string _searchBtnId;
        /// <summary>
        /// 搜索按钮Id
        /// </summary>
        public string SearchBtnId
        {
            get
            {
                if (string.IsNullOrEmpty(_searchBtnId))
                {
                    _searchBtnId = $"{SEARCH_BTN_ID_PREFIX}{ListVM.UniqueId}";
                }
                return _searchBtnId;
            }
            set
            {
                _searchBtnId = value;
            }
        }

        /// <summary>
        /// 重置按钮 Id
        /// </summary>
        private string ResetBtnId => $"{RESET_BTN_ID_PREFIX}{ListVM.UniqueId}";

        /// <summary>
        /// 重置按钮
        /// </summary>
        public bool ResetBtn { get; set; }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var tempSearchTitleId = Guid.NewGuid().ToNoSplitString();
            output.PreContent.AppendHtml($@"
<div class=""layui-collapse"" style=""margin-bottom:5px;"">
  <div class=""layui-colla-item"">
    <h2 class=""layui-colla-title"">搜索条件
      <div style=""text-align:right;margin-top:-43px;"" id=""{tempSearchTitleId}"">
        <a href=""javascript:void(0)"" class=""layui-btn layui-btn-sm"" id=""{SearchBtnId}""><i class=""layui-icon"">&#xe615;</i>搜索</a>
        {(!ResetBtn ? string.Empty : $@"<button type=""reset"" class=""layui-btn layui-btn-sm"" id=""{ResetBtnId}"">重置</button>")}
      </div>
    </h2>
    <div class=""layui-colla-content layui-show"" >
");
            output.PostContent.AppendHtml($@"
    </div>
  </div>
</div>
");
            output.PostElement.AppendHtml($@"
<script type=""text/javascript"">
// 阻止事件冒泡
$('#{tempSearchTitleId} .layui-btn').on('click',function(e){{e.stopPropagation();}})
$('#{SearchBtnId}').on('click', function () {{
    table.reload('{GridId}',{{where: $.extend({TableJSVar}.config.where,ff.GetSearchFormData('{Id}','{Vm.Name}'))}})
}});
</script>");
            return base.ProcessAsync(context, output);
        }
    }
}
