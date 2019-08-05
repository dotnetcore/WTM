using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:grid", Attributes = REQUIRED_ATTR_NAME, TagStructure = TagStructure.WithoutEndTag)]
    public class DataTableTagHelper : TagHelper
    {
        #region const
        protected const string REQUIRED_ATTR_NAME = "vm";

        /// <summary>
        /// 用于存储 DataTable render后返回的table变量的前缀
        /// </summary>
        public const string TABLE_JSVAR_PREFIX = "wtVar_";

        /// <summary>
        /// 用于自动生成的 GridId 的前缀
        /// </summary>
        public const string TABLE_ID_PREFIX = "wtTable_";

        /// <summary>
        /// 用于生成操作列
        /// </summary>
        public const string TABLE_TOOLBAR_PREFIX = "wtToolBar_";
        #endregion

        private static JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public ModelExpression Vm { get; set; }

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

        private string _id;
        public string Id
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                {
                    if (string.IsNullOrEmpty(_gridIdUserSet))
                    {
                        _id = $"{TABLE_ID_PREFIX}{ListVM.UniqueId}";
                    }
                    else
                    {
                        _id = _gridIdUserSet;
                    }
                }
                return _id;
            }
            set
            {
                _id = value;
                _gridIdUserSet = value;
            }
        }

        private string _searchPanelId;
        /// <summary>
        /// 搜索面板 Id
        /// </summary>
        public string SearchPanelId
        {
            get
            {
                if (string.IsNullOrEmpty(_searchPanelId))
                {
                    _searchPanelId = $"{FormTagHelper.FORM_ID_PREFIX}{ListVM.UniqueId}";
                }
                return _searchPanelId;
            }
            set
            {
                _searchPanelId = value;
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
                    _tableJSVar = $"{TABLE_JSVAR_PREFIX}{(string.IsNullOrEmpty(_gridIdUserSet) ? ListVM.UniqueId : _gridIdUserSet)}";
                }
                return _tableJSVar;
            }
        }

        private string ToolBarId => $"{TABLE_TOOLBAR_PREFIX}{ListVM.UniqueId}";
        /// <summary>
        /// 设定复选框列 默认false
        /// </summary>
        public bool HiddenCheckbox { get; set; }
        /// <summary>
        /// 设定复选框列 默认false
        /// </summary>
        public bool HiddenGridIndex { get; set; }
        /// <summary>
        /// 全部选中
        /// </summary>
        public bool? CheckedAll { get; set; }
        /// <summary>
        /// 复选框在第几列
        /// </summary>
        public int CheckboxIndex { get; set; }

        /// <summary>
        /// 设定容器高度 默认值：'auto' 若height>=0采用 '固定值' 模式，若height 小于 0 采用 'full-差值' 模式
        /// <para>固定值: 设定一个数字，用于定义容器高度，当容器中的内容超出了该高度时，会自动出现纵向滚动条</para>
        /// <para>full-差值: 高度将始终铺满，无论浏览器尺寸如何。这是一个特定的语法格式，其中 full 是固定的，而 差值 则是一个数值，这需要你来预估，比如：表格容器距离浏览器顶部和底部的距离“和” <para>
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// 设置单元格是否允许自动换行，默认为false
        /// </summary>
        public bool MultiLine { get; set; }

        /// <summary>
        /// 设定容器宽度 默认值：'auto'
        /// table容器的默认宽度是 auto，你可以借助该参数设置一个固定值，当容器中的内容超出了该宽度时，会自动出现横向滚动条。
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// 接口地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Http Request Method 默认 GET
        /// <para>如果无需自定义HTTP类型，可不加该参数 <see cref="HttpMethodEnum" /></para>
        /// </summary>
        public HttpMethodEnum? Method { get; set; }

        /// <summary>
        /// 查询条件
        /// </summary>
        public Dictionary<string, object> Filter { get; set; }

        /// <summary>
        /// 直接赋值数据
        /// <para>你也可以对表格直接赋值，而无需配置异步数据请求接口。他既适用于只展示一页数据，也非常适用于对一段已知数据进行多页展示。</para>
        /// </summary>
        public ModelExpression Data { get; set; }

        /// <summary>
        /// 直接从 ListVM的EntityList获取数据
        /// </summary>
        public bool UseLocalData { get; set; }

        /// <summary>
        /// 数据渲染完的回调
        /// <para>无论是异步请求数据，还是直接赋值数据，都会触发该回调。你可以利用该回调做一些表格以外元素的渲染。</para>
        /// <para>res:    如果是异步请求数据方式，res即为你接口返回的信息。</para>
        /// <para>        如果是直接赋值的方式，res即为：{data: [], count: 99} data为当前页数据、count为数据总长度</para>
        /// <para>curr:   得到当前页码</para>
        /// <para>count:  得到数据总量</para>
        /// </summary>
        public string DoneFunc { get; set; }

        /// <summary>
        /// 复选框选中事件
        /// 点击复选框时触发，回调函数返回一个object参数，携带的成员如下：
        /// obj.checked //当前是否选中状态
        /// obj.data    //选中行的相关数据
        /// obj.type    //如果触发的是全选，则为：all，如果触发的是单选，则为：one
        /// </summary>
        public string CheckedFunc { get; set; }

        /// <summary>
        /// 是否开启分页 默认 true
        /// </summary>
        public bool? Page { get; set; }

        /// <summary>
        /// 每页数据量可选项
        /// <para>默认值：[10,20,30,40,50,70,80,90]</para>
        /// </summary>
        public int[] Limits { get; set; }

        /// <summary>
        /// 默认每页数量 50
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// 是否显示加载条 默认 true
        /// <para>如果设置 false，则在切换分页时，不会出现加载条。该参数只适用于“异步数据请求”的方式（即设置了url的情况下）</para>
        /// </summary>
        public bool? Loading { get; set; }

        /// <summary>
        /// 用于设定表格风格，若使用默认风格不设置该属性即可
        /// </summary>
        public DataTableSkinEnum? Skin { get; set; }

        /// <summary>
        /// 隔行背景，默认true
        /// <para>若不开启隔行背景，设置为false即可</para>
        /// </summary>
        public bool? Even { get; set; }

        /// <summary>
        /// 用于设定表格尺寸，若使用默认尺寸不设置该属性即可
        /// </summary>
        public DataTableSizeEnum? Size { get; set; }

        /// <summary>
        /// 可编辑
        /// </summary>
        public bool Editable { get; set; }

        /// <summary>
        /// VM
        /// </summary>
        public Type VMType { get; set; }

        /// <summary>
        /// 是否显示汇总行
        /// </summary>
        public bool NeedShowTotal { get; set; }
        /// <summary>
        /// 排除的搜索条件
        /// </summary>
        private static readonly string[] _excludeParams = new string[]
        {
            "Page",
            "Limit",
            "Count",
            "PageCount",
            "FC",
            "DC",
            "VMFullName",
            "SortInfo",
            "TreeMode",
            "IsPostBack",
            "DC",
            "LoginUserInfo"
        };

        /// <summary>
        /// 排除的搜索条件类型，搜索条件数据源可能会存储在Searcher对象中
        /// </summary>
        private static readonly Type[] _excludeTypes = new Type[]
        {
            typeof(List<ComboSelectListItem>),
            typeof(ComboSelectListItem[]),
            typeof(IEnumerable<ComboSelectListItem>)
        };

        private void CalcChildCol(List<List<LayuiColumn>> layuiCols, List<IGridColumn<TopBasePoco>> rawCols, int maxDepth, int depth)
        {
            var tempCols = new List<LayuiColumn>();
            layuiCols.Add(tempCols);

            var nextCols = new List<IGridColumn<TopBasePoco>>();// 下一级列头
            foreach (var item in rawCols)
            {
                var tempCol = new LayuiColumn()
                {
                    Title = item.Title,
                    Field = item.Field,
                    Width = item.Width,
                    Sort = item.Sort,
                    Fixed = item.Fixed,
                    Align = item.Align,
                    UnResize = item.UnResize,
                    Hide = item.Hide,
                    ShowTotal = item.ShowTotal
                    //EditType = item.EditType
                };
                if ((item.EditType == EditTypeEnum.Text || item.EditType == null) && string.IsNullOrEmpty(item.Field) == false)
                {
                    tempCol.Templet = new Newtonsoft.Json.Linq.JRaw(getTemplate(item.Field));
                }

                if (item.ShowTotal == true)
                {
                    NeedShowTotal = true;
                }
                if (item.Children != null && item.Children.Count() > 0)
                {
                    tempCol.Colspan = item.ChildrenLength;
                }
                if (maxDepth > 1 && (item.Children == null || item.Children.Count() == 0))
                {
                    tempCol.Rowspan = maxDepth - depth;
                }
                tempCols.Add(tempCol);
                if (item.Children != null && item.Children.Count() > 0)
                    nextCols.AddRange(item.Children);
            }
            if (nextCols.Count > 0)
            {
                CalcChildCol(layuiCols, nextCols, maxDepth, depth + 1);
            }
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Loading == null)
            {
                Loading = true;
            }
            var vmQualifiedName = Vm.Model.GetType().AssemblyQualifiedName;
            vmQualifiedName = vmQualifiedName.Substring(0, vmQualifiedName.LastIndexOf(", Version="));

            var tempGridTitleId = Guid.NewGuid().ToNoSplitString();
            output.TagName = "table";
            output.Attributes.Add("id", Id);
            output.Attributes.Add("lay-filter", Id);
            output.TagMode = TagMode.StartTagAndEndTag;
            //IEnumerable<TopBasePoco> data = null;
            if (Limit == null)
            {
                Limit = GlobalServices.GetRequiredService<Configs>()?.RPP;
            }
            if (Limits == null)
            {
                Limits = new int[] { 10, 20, 50, 80, 100, 150, 200 };
                if (!Limits.Contains(Limit.Value))
                {
                    var list = Limits.ToList();
                    list.Add(Limit.Value);
                    Limits = list.OrderBy(x => x).ToArray();
                }
            }
            // TODO 转换有问题
            Page = ListVM.NeedPage;
            if (UseLocalData)
            {
                // 不需要分页
                ListVM.NeedPage = false;
                //data = ListVM.GetEntityList().ToList();
            }
            else if (string.IsNullOrEmpty(Url))
            {
                Url = "/_Framework/GetPagingData";

                if (Filter == null) Filter = new Dictionary<string, object>();
                Filter.Add("_DONOT_USE_VMNAME", vmQualifiedName);
                Filter.Add("_DONOT_USE_CS", ListVM.CurrentCS);
                Filter.Add("SearcherMode", ListVM.SearcherMode);
                if (ListVM.Ids != null && ListVM.Ids.Count > 0)
                {
                    Filter.Add("Ids", ListVM.Ids);
                }
                // 为首次加载添加Searcher查询参数
                if (ListVM.Searcher != null)
                {
                    var props = ListVM.Searcher.GetType().GetProperties();
                    props = props.Where(x => !_excludeTypes.Contains(x.PropertyType)).ToArray();
                    foreach (var prop in props)
                    {
                        if (!_excludeParams.Contains(prop.Name))
                        {
                            Filter.Add($"Searcher.{prop.Name}", prop.GetValue(ListVM.Searcher));
                        }
                    }
                }
            }

            var request = new Dictionary<string, object>
            {
                {"pageName","Searcher.Page" },   //页码的参数名称，默认：page
                {"limitName","Searcher.Limit" }, //每页数据量的参数名，默认：limit
            };
            var response = new Dictionary<string, object>
            {
                {"statusName","Code" }, //数据状态的字段名称，默认：code
                {"statusCode",200 },    //成功的状态码，默认：0
                {"msgName","Msg" },     //状态信息的字段名称，默认：msg
                {"countName","Count" }, //数据总数的字段名称，默认：count
                {"dataName","Data" }    //数据列表的字段名称，默认：data
            };

            #region 生成 Layui 所需的表头
            var rawCols = ListVM?.GetHeaders();
            var maxDepth = (ListVM?.ChildrenDepth) ?? 1;
            var layuiCols = new List<List<LayuiColumn>>();
            var tempCols = new List<LayuiColumn>();
            layuiCols.Add(tempCols);
            // 添加复选框
            if (!HiddenCheckbox)
            {
                var checkboxHeader = new LayuiColumn()
                {
                    Type = LayuiColumnTypeEnum.Checkbox,
                    LAY_CHECKED = CheckedAll,
                    Rowspan = maxDepth,
                    // Fixed = GridColumnFixedEnum.Left,
                    Width = 45
                };
                tempCols.Add(checkboxHeader);
            }
            // 添加序号列
            if (!HiddenGridIndex)
            {
                var gridIndex = new LayuiColumn()
                {
                    Type = LayuiColumnTypeEnum.Numbers,
                    Rowspan = maxDepth,
                    // Fixed = GridColumnFixedEnum.Left,
                    Width = 45
                };
                tempCols.Add(gridIndex);
            }
            var nextCols = new List<IGridColumn<TopBasePoco>>();// 下一级列头
            foreach (var item in rawCols)
            {
                var tempCol = new LayuiColumn()
                {
                    Title = item.Title,
                    Field = item.Field,
                    Width = item.Width,
                    Sort = item.Sort,
                    Fixed = item.Fixed,
                    Align = item.Align,
                    UnResize = item.UnResize,
                    Hide = item.Hide,
                    ShowTotal = item.ShowTotal
                    //Style = "height:auto !important;white-space:normal !important"
                    //EditType = item.EditType
                };
                //非编辑状态且有字段名的情况下，设置template
                if ((item.EditType == EditTypeEnum.Text || item.EditType == null) && string.IsNullOrEmpty(item.Field) == false)
                {
                    tempCol.Templet = new Newtonsoft.Json.Linq.JRaw(getTemplate(item.Field));
                }

                if (item.ShowTotal == true)
                {
                    NeedShowTotal = true;
                }
                switch (item.ColumnType)
                {
                    case GridColumnTypeEnum.Space:
                        tempCol.Type = LayuiColumnTypeEnum.Space;
                        break;
                    case GridColumnTypeEnum.Action:
                        tempCol.Toolbar = $"#{ToolBarId}";
                        break;
                    default:
                        break;
                }
                if (item.Children != null && item.Children.Count() > 0)
                {
                    tempCol.Colspan = item.ChildrenLength;
                }
                if (maxDepth > 1 && (item.Children == null || item.Children.Count() == 0))
                {
                    tempCol.Rowspan = maxDepth;
                }
                tempCols.Add(tempCol);
                if (item.Children != null && item.Children.Count() > 0)
                    nextCols.AddRange(item.Children);
            }
            if (nextCols.Count > 0)
            {
                CalcChildCol(layuiCols, nextCols, maxDepth, 1);
            }

            if (layuiCols.Count > 0 && layuiCols[0].Count > 0)
            {
                layuiCols[0][0].TotalRowText = ListVM?.TotalText;
            }

            #endregion

            #region 处理 DataTable 操作按钮

            var actionCol = ListVM?.GridActions;

            var rowBtnStrBuilder = new StringBuilder();// Grid 行内按钮
            var toolBarBtnStrBuilder = new StringBuilder();// Grid 工具条按钮
            var gridBtnEventStrBuilder = new StringBuilder();// Grid 按钮事件

            if (actionCol != null && actionCol.Count > 0)
            {
                var vm = Vm.Model as BaseVM;
                foreach (var item in actionCol)
                {
                    AddSubButton(vmQualifiedName, rowBtnStrBuilder, toolBarBtnStrBuilder, gridBtnEventStrBuilder, vm, item);
                }
            }
            #endregion

            #region DataTable

            var vmName = string.Empty;
            if (VMType != null)
            {
                var vmQualifiedName1 = VMType.AssemblyQualifiedName;
                vmName = vmQualifiedName1.Substring(0, vmQualifiedName1.LastIndexOf(", Version="));
            }
            output.PostElement.AppendHtml($@"
<script>
  var table = layui.table;
  /* 暂时解决 layui table首次及table.reload()无loading的bug */
  var layer = layui.layer;
  var msg = layer.msg('数据请求中', {{
    icon: 16,
    time: -1,
    anim: -1,
    fixed: false
  }})
  /* 暂时解决 layui table首次及table.reload()无loading的bug */
 var {Id}option = {{
    elem: '#{Id}'
    ,id: '{Id}'
    ,autoSort: false
    ,totalRow: {NeedShowTotal.ToString().ToLower()}
    {(string.IsNullOrEmpty(Url) ? string.Empty : $",url: '{Url}'")}
    {(Filter == null || Filter.Count == 0 ? string.Empty : $",where: {JsonConvert.SerializeObject(Filter)}")}
    {(Method == null ? ",method:'post'" : $",method: '{Method.Value.ToString().ToLower()}'")}
    {(!Loading.HasValue ? string.Empty : $",loading: {Loading.Value.ToString().ToLower()}")}
    ,request: {JsonConvert.SerializeObject(request)}
    ,response: {JsonConvert.SerializeObject(response)}
    {(Page ?? true ? ",page:true" : ",page:{layout:['count']}")}
    ,limit: {(Page ?? true ? $"{Limit ?? 50}" : $"{(UseLocalData ? ListVM.GetEntityList().Count().ToString() : "0")}")}
    {(Page ?? true ?
        (Limits == null || Limits.Length == 0 ? ",limits:[10,20,50,80,100,150,200]" : $",limits:{JsonConvert.SerializeObject(Limits)}")
        : string.Empty)}
    {(!Width.HasValue ? string.Empty : $",width: {Width.Value}")}
    {(!Height.HasValue ? string.Empty : (Height.Value >= 0 ? $",height: {Height.Value}" : $",height: 'full{Height.Value}'"))}
    ,cols:{JsonConvert.SerializeObject(layuiCols, _jsonSerializerSettings)}
    {(!Skin.HasValue ? string.Empty : $",skin: '{Skin.Value.ToString().ToLower()}'")}
    {(!Even.HasValue ? ",even: true" : $",even: {Even.Value.ToString().ToLower()}")}
    {(!Size.HasValue ? string.Empty : $",size: '{Size.Value.ToString().ToLower()}'")}
,done: function(res,curr,count){{layer.close(msg);
    var tab = $('#{Id} + .layui-table-view');tab.find('table').css('border-collapse','separate');
    {(Height == null ? $"tab.css('overflow','hidden').addClass('donotuse_fill donotuse_pdiv');tab.children('.layui-table-box').addClass('donotuse_fill donotuse_pdiv').css('height','100px');tab.find('.layui-table-main').addClass('donotuse_fill');tab.find('.layui-table-header').css('min-height','40px');ff.triggerResize();" : string.Empty)}
    {(MultiLine == true ? $"tab.find('.layui-table-cell').css('height','auto').css('white-space','normal');" : string.Empty)}
    {(string.IsNullOrEmpty(DoneFunc) ? string.Empty : $"{DoneFunc}(res,curr,count)")}
}}
}}


  {TableJSVar} = table.render({Id}option);

 {(UseLocalData ? $@"ff.LoadLocalData(""{Id}"",{Id}option,{ListVM.GetDataJson().Replace("<script>", "$$script$$").Replace("</script>", "$$#script$$")}); " : string.Empty)}


  // 监听工具条
  function wtToolBarFunc_{Id}(obj){{ //注：tool是工具条事件名，test是table原始容器的属性 lay-filter=""对应的值""
    var data = obj.data, layEvent = obj.event, tr = obj.tr; //获得当前行 tr 的DOM对象
    {(gridBtnEventStrBuilder.Length == 0 ? string.Empty : $@"switch(layEvent){{{gridBtnEventStrBuilder}default:break;}}")}
    return;
  }}
  {(VMType == null || string.IsNullOrEmpty(vmName) ? string.Empty : $@"function wtEditFunc_{Id}(o){{
      var data = {{_DONOT_USE_VMNAME:'{vmName}',id:o.data.ID,field:o.field,value:o.value}};
      $.post(""/_Framework/UpdateModelProperty"",data,function(a,b,c){{
          if(a.code == 200){{ff.Msg('更新成功');}}else{{ff.Msg(a.msg);}}
      }});
  }}")}
  table.on('tool({Id})',wtToolBarFunc_{Id});
  {(VMType == null || string.IsNullOrEmpty(vmName) ? string.Empty : $"table.on('edit({Id})',wtEditFunc_{Id});")}
  {(string.IsNullOrEmpty(CheckedFunc) ? string.Empty : $"table.on('checkbox({Id})',{CheckedFunc});")}

    table.on('sort({Id})', function(obj){{
    var sortfilter = {{}};
    sortfilter['Searcher.SortInfo.Property'] = obj.field;
    sortfilter['Searcher.SortInfo.Direction'] = obj.type.replace(obj.type[0],obj.type[0].toUpperCase());
   var w = $.extend({Id}option.where,sortfilter);

  table.reload('{Id}', {{
    initSort: obj,
    where: w
  }});
  }});
</script>
<!-- Grid 行内按钮 -->
<script type=""text/html"" id=""{ToolBarId}"">{rowBtnStrBuilder}</script>
");
            #endregion

            output.PreElement.AppendHtml($@"<div style=""text-align:right;margin-right:15px"">{toolBarBtnStrBuilder}</div>");
            output.PostElement.AppendHtml($@"
{ (string.IsNullOrEmpty(ListVM.DetailGridPrix) ? "" : $"<input type=\"hidden\" name=\"{Vm.Name}.DetailGridPrix\" value=\"{ListVM.DetailGridPrix}\"/>")}
");
            base.Process(context, output);
        }

        /// <summary>
        /// 添加按钮
        /// </summary>
        /// <param name="vmQualifiedName"></param>
        /// <param name="rowBtnStrBuilder"></param>
        /// <param name="toolBarBtnStrBuilder"></param>
        /// <param name="gridBtnEventStrBuilder"></param>
        /// <param name="vm"></param>
        /// <param name="item"></param>
        /// <param name="isSub"></param>
        private void AddSubButton(
            string vmQualifiedName,
            StringBuilder rowBtnStrBuilder,
            StringBuilder toolBarBtnStrBuilder,
            StringBuilder gridBtnEventStrBuilder,
            BaseVM vm,
            GridAction item,
            bool isSub = false
        )
        {
            if (vm.LoginUserInfo?.IsAccessable(item.Url) == true ||
                item.ParameterType == GridActionParameterTypesEnum.AddRow ||
                item.ParameterType == GridActionParameterTypesEnum.RemoveRow
            )
            {
                // Grid 行内按钮
                if (item.ShowInRow)
                {
                    if (item.ParameterType != GridActionParameterTypesEnum.RemoveRow)
                    {
                        bool condition = false;
                        if (string.IsNullOrEmpty(item.BindVisiableColName) == false)
                        {
                            condition = true;
                        }
                        if (condition == true)
                        {
                            rowBtnStrBuilder.Append("{{#  if(d." + item.BindVisiableColName + " == true || d." + item.BindVisiableColName + " == 'true' ){ }}");
                        }
                        rowBtnStrBuilder.Append($@"<a class=""layui-btn layui-btn-primary layui-btn-xs"" lay-event=""{item.Area + item.ControllerName + item.ActionName + item.QueryString}"">{item.Name}</a>");
                        if (condition == true)
                        {
                            rowBtnStrBuilder.Append("{{#  } else{ }}");
                            //rowBtnStrBuilder.Append($@"<a class=""layui-btn layui-btn-primary layui-btn-xs"" style=""visibility:collapse"">{item.Name}</a>");
                            rowBtnStrBuilder.Append("{{# } }}");
                        }
                    }
                    else
                    {
                        rowBtnStrBuilder.Append($@"<a class=""layui-btn layui-btn-primary layui-btn-xs"" onclick=""ff.RemoveGridRow('{Id}',{Id}option,{{{{d.LAY_INDEX}}}});"">{item.Name}</a>");
                    }
                }

                // Grid 工具条按钮
                if (!item.HideOnToolBar)
                {
                    var icon = string.Empty;
                    if (item.IconCls?.StartsWith("layui-icon-") == true)
                    {
                        icon = $@"<i class=""layui-icon {item.IconCls}""></i>";
                    }
                    else
                    {
                        icon = $@"<i class=""{item.IconCls}""></i>";
                    }
                    //如果是按钮组容器，加载子按钮
                    if (item.ActionName?.Equals("ActionsGroup") == true
                        && item.SubActions != null &&
                        item.SubActions.Count > 0
                    )
                    {
                        var subBarBtnStrList = new StringBuilder();
                        foreach (var subItem in item.SubActions)
                        {
                            StringBuilder subBarBtnStr = new StringBuilder();
                            AddSubButton(vmQualifiedName, rowBtnStrBuilder, subBarBtnStr, gridBtnEventStrBuilder, vm, subItem, true);
                            if (subBarBtnStr.Length > 0)
                            {
                                subBarBtnStrList.AppendFormat("<dd style=\"padding: 0 0px;margin-bottom:1px;line-height: initial;\">{0}</dd>", subBarBtnStr.ToString());
                            }
                        }

                        toolBarBtnStrBuilder.Append($@"<button type=""button"" class=""layui-btn layui-btn-sm layui-unselect layui-form-select downpanel"" style=""z-index:10;"" id=""btn_{item.ButtonId}"">
                                 <div class=""layui-select-title"" style=""padding-right:20px;"">
                                        {item.Name}
                                 <i class=""layui-edge""></i>
                                 </div>
                                 <dl class=""layui-anim layui-anim-upbit"" style=""top: initial;padding:1px 0px 0px 0px;"" >
                                    {subBarBtnStrList.ToString()}
                                 </dl>
                                 </button>");
                        if (!toolBarBtnStrBuilder.ToString().Contains("layui.use(["))
                        {
                            toolBarBtnStrBuilder.Append($@"<script type=""text/javascript"">layui.use([""form""], function () {{
                            var form = layui.form, $ = layui.jquery;
                            $("".downpanel"").on(""click"", "".layui-select-title"", function(e) {{
                                $("".layui-form-select"").not($(this).parents("".layui-form-select"")).removeClass(""layui-form-selected"");
                                $(this).parents("".layui-form-select"").toggleClass(""layui-form-selected"");
                                            e.stopPropagation();
                                        }});
                            $(document).click(function(event) {{
                            var _con2 = $("".downpanel"");
                            if (!_con2.is (event.target) && (_con2.has(event.target).length === 0)) {{
                            _con2.removeClass(""layui -form-selected"");
                            }}
                            }});
                            }});</script>");
                        }

                        //按钮组时直接返回
                        return;
                    }
                    else
                    {
                        string substyle = isSub ? "style=\"width: 100%;\"" : "";
                        toolBarBtnStrBuilder.Append($@"<a href=""javascript:void(0)"" onclick=""wtToolBarFunc_{Id}({{event:'{item.Area + item.ControllerName + item.ActionName + item.QueryString}'}});"" class=""layui-btn layui-btn-sm"" {substyle}>{icon}{item.Name}</a>");
                    }
                }
                var url = item.Url;
                if (item.ControllerName == "_Framework" && item.ActionName == "GetExportExcel") // 导出按钮 接口地址
                {
                    url = $"{url}&_DONOT_USE_VMNAME={vmQualifiedName}";
                }
                var script = new StringBuilder($"var tempUrl = '{url}',whereStr={JsonConvert.SerializeObject(item.whereStr)};");

                switch (item.ParameterType)
                {
                    case GridActionParameterTypesEnum.NoId: break;
                    case GridActionParameterTypesEnum.SingleId:
                        script.Append($@"
if(data==undefined||data==null||data.ID==undefined||data.ID==null){{
    var ids = ff.GetSelections('{Id}');
    if(ids.length == 0){{
        layui.layer.msg('请选择一行');
        return;
    }}else if(ids.length > 1){{
        layui.layer.msg('最多只能选择一行');
        return;
    }}else{{
        tempUrl = tempUrl + '&id=' + ids[0];
        var objs = ff.GetSelectionData('{Id}');
        if(objs!=null && objs.length > 0){{
            tempUrl = ff.concatWhereStr(tempUrl,whereStr,objs[0]);
        }}
    }}
}}else{{
    tempUrl = tempUrl + '&id=' + data.ID;
    tempUrl = ff.concatWhereStr(tempUrl,whereStr,data);
}}
");
                        break;
                    case GridActionParameterTypesEnum.MultiIds:
                        script.Append($@"
isPost = true;
var ids = ff.GetSelections('{Id}');
if(ids.length == 0){{
    layui.layer.msg('请至少选择一行');
    return;
}}
");
                        break;
                    case GridActionParameterTypesEnum.SingleIdWithNull:
                        script.Append($@"
var ids = [];
if(data != null && data.ID != null){{
    ids.push(data.ID);
    tempUrl = ff.concatWhereStr(tempUrl,whereStr,data);
}} else {{
    ids = ff.GetSelections('{Id}');
    var objs = ff.GetSelectionData('{Id}');
    if(objs!=null && objs.length > 0){{
        tempUrl = ff.concatWhereStr(tempUrl,whereStr,objs[0]);
    }}
}}
if(ids.length > 1){{
    layui.layer.msg('最多只能选择一行');
    return;
}}else if(ids.length == 1){{
    tempUrl = tempUrl + '&id=' + ids[0];
}}
");
                        break;
                    case GridActionParameterTypesEnum.MultiIdWithNull:
                        script.Append($@"
var ids = ff.GetSelections('{Id}');
isPost = true;
");
                        break;
                    default: break;
                }

                gridBtnEventStrBuilder.Append($@"
case '{item.Area + item.ControllerName + item.ActionName + item.QueryString}':{{");
                if (item.ParameterType == GridActionParameterTypesEnum.AddRow)
                {
                    Regex r = new Regex("<script>.*?</script>");
                    gridBtnEventStrBuilder.Append($@"ff.AddGridRow(""{Id}"",{Id}option,{r.Replace(ListVM.GetSingleDataJson(null, false), "")});
");
                }
                else if (item.ParameterType == GridActionParameterTypesEnum.RemoveRow) { }
                else
                {
                    string actionScript = "";
                    if (string.IsNullOrEmpty(item.OnClickFunc))
                    {
                        if(item.ShowDialog == true)
                        {
                            string width= "null";
                            string height = "null";
                            if(item.DialogWidth != null)
                            {
                                width = item.DialogWidth.ToString();
                            }
                            if(item.DialogHeight != null)
                            {
                                height = item.DialogHeight.ToString();
                            }
                            if (item.IsRedirect == true)
                            {
                                actionScript = $"ff.LoadPage(tempUrl,{item.IsRedirect.ToString().ToLower()},'{item.DialogTitle ?? ""}',isPost===true&&ids!==null&&ids!==undefined?{{'Ids':ids}}:undefined);";
                            }
                            else
                            {
                                actionScript = $"ff.OpenDialog(tempUrl,'{Guid.NewGuid().ToNoSplitString()}','{item.DialogTitle}',{width},{height},isPost===true&&ids!==null&&ids!==undefined?{{'Ids':ids}}:undefined);";
                            }
                        }
                        else
                        {
                            if(item.Area == string.Empty && item.ControllerName == "_Framework" && item.ActionName == "GetExportExcel")
                            {
                                actionScript = $"ff.DownloadExcelOrPdf(tempUrl,'{SearchPanelId}',{JsonConvert.SerializeObject(Filter)},ids);";
                            }
                            else
                            {
                                if (item.IsRedirect == true)
                                {
                                    actionScript = $"ff.LoadPage(tempUrl,false,'{item.DialogTitle ?? ""}',isPost===true&&ids!==null&&ids!==undefined?{{'Ids':ids}}:undefined);";
                                }
                                else
                                {
                                    actionScript = $"ff.BgRequest(tempUrl, isPost===true&&ids!==null&&ids!==undefined?{{'Ids':ids}}:undefined);";
                                }
                            }
                        }
                    }
                    else
                    {
                        actionScript = $"{item.OnClickFunc}();";
                    }
                    gridBtnEventStrBuilder.Append($@"
var isPost = false;
{script}
{actionScript}");
                }
                gridBtnEventStrBuilder.Append($@"}};break;
");
            }
        }

        private string getTemplate(string field)
        {
            return $@"function(d){{
                var sty = '';var bg = '';var did = '{field}_'+d.LAY_INDEX;if(d.{field}__bgcolor != undefined) bg = ""<script>$('#""+did+""').closest('td').css('background-color','""+d.{field}__bgcolor+""');</s""+""cript>""; if(d.{field}__forecolor != undefined) sty = 'color:'+d.{field}__forecolor+';'; return '<div style=""'+sty+'"" id=""'+did+'"">'+d.{field}.replace(/\""/g,""'"")+bg+'</div>';
            }}
            ";
        }
    }
}
