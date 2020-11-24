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
        /// Reset button Id
        /// </summary>
        private string ResetBtnId => $"{RESET_BTN_ID_PREFIX}{ListVM.UniqueId}";

        /// <summary>
        /// 重置按钮
        /// </summary>
        public bool ResetBtn { get; set; }

        /// <summary>
        /// Is expanded
        /// </summary>
        public bool? Expanded { get; set; }
        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var baseVM = Vm?.Model as BaseVM;
            var tempSearchTitleId = Guid.NewGuid().ToNoSplitString();
            bool show = false;
            if(ListVM?.Searcher?.IsExpanded != null)
            {
                Expanded = ListVM?.Searcher?.IsExpanded;
            }
            if(Expanded != null)
            {
                show = Expanded.Value;
            }
            else
            {
                show = GlobalServices.GetRequiredService<Configs>().UiOptions.SearchPanel.DefaultExpand;
            }
            var layuiShow = show ? " layui-show" : string.Empty;
            output.PreContent.AppendHtml($@"
<div class=""layui-collapse"" style=""margin-bottom:5px;"" lay-filter=""{tempSearchTitleId}x"">
  <div class=""layui-colla-item"">
    <h2 class=""layui-colla-title"">{Program._localizer["SearchCondition"]}
      <div style=""text-align:right;margin-top:-43px;"" id=""{tempSearchTitleId}"">
        <a href=""javascript:void(0)"" class=""layui-btn layui-btn-sm"" id=""{SearchBtnId}""><i class=""layui-icon"">&#xe615;</i>{Program._localizer["Search"]}</a>
        {(!ResetBtn ? string.Empty : $@"<button type=""button"" class=""layui-btn layui-btn-sm"" id=""{ResetBtnId}"">{Program._localizer["Reset"]}</button>")}
      </div>
    </h2>
    <div class=""layui-colla-content{layuiShow}"" >
      <input type=""text"" style=""display: none;"">
");
            output.PostContent.AppendHtml($@"
    </div>
  </div>
</div>
");
            output.PostElement.AppendHtml($@"
<script>
  layui.use(['table','element'], function () {{
    const table = layui.table;
    layui.element.init();
    $('#{tempSearchTitleId} .layui-btn').on('click',function(e){{e.stopPropagation();}})
    $('#{ResetBtnId}').on('click', function (btn) {{ff.resetForm(this.form.id);}});
    $('#{tempSearchTitleId}').parents('form').append(""<input type='hidden' name='Searcher.IsExpanded' value='{show.ToString().ToLower()}' />"");
layui.element.on('collapse({tempSearchTitleId}x)', function(data){{
    $('#{tempSearchTitleId}').parents('form').find(""input[name='Searcher.IsExpanded']"").val(data.show+'');
}});

{(OldPost == true ? $"" : $@"
$('#{SearchBtnId}').on('click', function () {{
  var layer = layui.layer;
    var tempwhere = {{}};
    $.extend(tempwhere,{GridId}defaultfilter.where);
      {GridId}filterback.where = tempwhere;
  table.reload('{GridId}',{{url:{GridId}url,where: $.extend(tempwhere,ff.GetSearchFormData('{Id}','{Vm.Name}')),page:{{curr:1}}
    //done: function(res,curr,count){{
    //  if(this.height == undefined){{
    //    var tab = $('#{GridId} + .layui-table-view');tab.css('overflow','hidden').addClass('donotuse_fill donotuse_pdiv');tab.children('.layui-table-box').addClass('donotuse_fill donotuse_pdiv').css('height','100px');tab.find('.layui-table-main').addClass('donotuse_fill');tab.find('.layui-table-header').css('min-height','40px');
    //    ff.triggerResize();
    //  }}
    //}}
  }})
}});
    ")}
layui.element.on('collapse({tempSearchTitleId})', function(data){{ff.triggerResize()}});
}})
</script>");
            return base.ProcessAsync(context, output);
        }
    }
}
