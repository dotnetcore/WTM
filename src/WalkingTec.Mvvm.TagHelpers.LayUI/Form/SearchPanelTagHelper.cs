using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Schema;
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

        private BaseSearcher _searcherVM;
        private BaseSearcher SearcherVM
        {
            get
            {
                if(_searcherVM == null)
                {
                    if (ListVM == null)
                    {
                        _searcherVM = Vm?.Model as BaseSearcher;
                    }
                    else
                    {
                        _searcherVM = ListVM.Searcher;
                    }
                }
                return _searcherVM;
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
                    if (_gridIdUserSet==null)
                    {
                        if (ListVM != null)
                        {
                            _gridId = $"{DataTableTagHelper.TABLE_ID_PREFIX}{ListVM?.UniqueId}";
                        }
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

        /// <summary>
        /// 关联的 Chart 组件的 Id
        /// </summary>
        public string ChartId { get; set; }
        public string ChartPrefix { get; set; }
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
                    _searchBtnId = $"{SEARCH_BTN_ID_PREFIX}{Id}";
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
        private string ResetBtnId => $"{RESET_BTN_ID_PREFIX}{SearcherVM?.UniqueId}";

        /// <summary>
        /// 重置按钮
        /// </summary>
        public bool ResetBtn { get; set; }

        /// <summary>
        /// Is expanded
        /// </summary>
        public bool? Expanded { get; set; }

        private Configs _configs;
        public SearchPanelTagHelper(IOptionsMonitor<Configs> configs)
        {
            _configs = configs.CurrentValue;
        }

        private bool IsInSelector = false;

        protected string fieldPre
        {
            get
            {
                string rv = "";
                if(string.IsNullOrEmpty(Vm?.Name) == false) {
                    rv = Vm?.Name;
                    if (ListVM != null)
                    {
                        rv += ".Searcher";
                    }
                }
                else
                {
                    if(ListVM != null)
                    {
                        rv = "Searcher";
                    }
                }
                if(IsInSelector == true)
                {
                    rv = rv.Replace(".Searcher", "").Replace("Searcher", "");
                }
                return rv;
            }
        }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context.Items.ContainsKey("inselector") == true)
            {
                IsInSelector = true;
            }
            if(OldPost == true)
            {
                output.Attributes.Add("oldpost", true);
            }

            var tempSearchTitleId = Guid.NewGuid().ToNoSplitString();
            bool show = false;
            if(SearcherVM?.IsExpanded != null)
            {
                Expanded = SearcherVM?.IsExpanded;
            }
            if(Expanded != null)
            {
                show = Expanded.Value;
            }
            else
            {
                show =_configs.UIOptions.SearchPanel.DefaultExpand;
            }

            string showpage = "";
            if(ListVM?.NeedPage == true)
            {
                showpage = $@",page:{{
        rpptext:'{THProgram._localizer["Sys.RecordsPerPage"]}',
        totaltext:'{THProgram._localizer["Sys.Total"]}',
        recordtext:'{THProgram._localizer["Sys.Record"]}',
        gototext:'{THProgram._localizer["Sys.Goto"]}',
        pagetext:'{THProgram._localizer["Sys.Page"]}',
        oktext:'{THProgram._localizer["Sys.GotoButtonText"]}',
    }}";
            }
            var layuiShow = show ? " layui-show" : string.Empty;
            output.PreContent.AppendHtml($@"
<div class=""layui-collapse"" style=""margin-bottom:5px;"" lay-filter=""{tempSearchTitleId}x"">
  <div class=""layui-colla-item"">
    <h2 class=""layui-colla-title"">{THProgram._localizer["Sys.SearchCondition"]}
      <div style=""text-align:right;margin-top:-43px;"" id=""{tempSearchTitleId}"">
        <a href=""javascript:void(0)"" class=""layui-btn layui-btn-sm"" id=""{SearchBtnId}"" IsSearchButton><i class=""layui-icon"">&#xe615;</i>{THProgram._localizer["Sys.Search"]}</a>
        {(!ResetBtn ? string.Empty : $@"<button type=""button"" class=""layui-btn layui-btn-sm"" id=""{ResetBtnId}"">{THProgram._localizer["Sys.Reset"]}</button>")}
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

            var refreshgridjs = "";
            if (string.IsNullOrEmpty(GridId) == false)
            {
                foreach (var item in GridId.Split(','))
                {
                    refreshgridjs += $@"
    var tempwhere{item} = {{}};
    $.extend(tempwhere{item},{item}defaultfilter.where);
    var page{item} = {item}filterback.page;
    if(keeppage ==null){{ page{item}.curr = 1}}
    table.reload('{item}',{{page: page{item},url:{item}url,where: $.extend(tempwhere{item},ff.GetSearchFormData('{Id}','{fieldPre}'))}});
";
                }
            }
            var refreshchartjs = "";
            if (string.IsNullOrEmpty(ChartId) == false)
            {
                output.Attributes.SetAttribute("chartlink",  ChartId);
                foreach (var item in ChartId.Split(','))
                {
                    refreshchartjs += $@"
    ff.RefreshChart('{item}',{(string.IsNullOrEmpty(ChartPrefix)==true? "undefined":$"'{ChartPrefix}'")});
";
                }
            }
                output.PostElement.AppendHtml($@"
<script>
  layui.use(['table','element'], function () {{
    const table = layui.table;
    layui.element.init();
    $('#{tempSearchTitleId} .layui-btn').on('click',function(e){{e.stopPropagation();}})
    $('#{ResetBtnId}').on('click', function (btn) {{ff.resetForm(this.form.id);}});
    $('#{tempSearchTitleId}').parents('form').append(""<input type='hidden' name='IsExpanded' value='{show.ToString().ToLower()}' />"");
layui.element.on('collapse({tempSearchTitleId}x)', function(data){{
    $('#{tempSearchTitleId}').parents('form').find(""input[name='IsExpanded']"").val(data.show+'');
    ff.triggerResize();
}});

{(OldPost == true ? $"" : $@"
$('#{SearchBtnId}').on('click', function () {{
   var keeppage = null;
    {refreshgridjs}
    {refreshchartjs}
}});
$('#{SearchBtnId}').bind('myclick', function () {{
   var keeppage = true;
    {refreshgridjs}
    {refreshchartjs}
}});
    ")}
layui.element.on('collapse({tempSearchTitleId})', function(data){{ff.triggerResize()}});
}})
</script>");
            return base.ProcessAsync(context, output);
        }
    }
}
