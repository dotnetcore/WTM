using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Npgsql;
using NpgsqlTypes;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Core
{
    public delegate object ColumnFormatCallBack<in T>(T entity, object fieldValue) where T : TopBasePoco;

    /// <summary>
    /// ListVM的搜索模式枚举
    /// </summary>
    public enum ListVMSearchModeEnum
    {
        Search, //搜索
        Export, //导出
        Batch, //批量
        Selector,//选择器
        MasterDetail, //
        CheckExport,
        Custom1, Custom2, Custom3, Custom4, Custom5
    };

    /// <summary>
    /// ListVM的基类，所有ListVM应该继承这个类， 基类提供了搜索，导出等列表常用功能
    /// </summary>
    /// <typeparam name="TModel">ListVM中的Model类</typeparam>
    /// <typeparam name="TSearcher">ListVM使用的Searcher类</typeparam>
    public class BasePagedListVM<TModel, TSearcher> : BaseVM, IBasePagedListVM<TModel, TSearcher>
        where TModel : TopBasePoco
        where TSearcher : BaseSearcher
    {

        public string TotalText { get; set; } = "合计：";

        public virtual DbCommand GetSearchCommand()
        {
            return null;
        }

        private int? _childrenDepth;

        /// <summary>
        /// 多级表头深度  默认 1级
        /// </summary>
        public int ChildrenDepth
        {
            get
            {
                if (_childrenDepth == null)
                {
                    _childrenDepth = _getHeaderDepth();
                }
                return _childrenDepth.Value;
            }
        }

        /// <summary>
        /// GridHeaders
        /// </summary>
        public IEnumerable<IGridColumn<TModel>> GridHeaders { get; set; }

        /// <summary>
        /// GetHeaders
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IGridColumn<TModel>> GetHeaders()
        {
            if (GridHeaders == null)
            {
                GridHeaders = InitGridHeader();
            }
            return GridHeaders;
        }

        /// <summary>
        /// 计算多级表头深度
        /// </summary>
        /// <returns></returns>
        private int _getHeaderDepth()
        {
            IEnumerable<IGridColumn<TModel>> headers = GetHeaders();
            return headers.Max(x => x.MaxDepth);
        }
        private List<GridAction> _gridActions;

        /// <summary>
        /// 页面动作
        /// </summary>
        public List<GridAction> GridActions
        {
            get
            {
                if (_gridActions == null)
                {
                    _gridActions = InitGridAction();
                }
                return _gridActions;
            }
        }

        /// <summary>
        /// 初始化 InitGridHeader，继承的类应该重载这个函数来设定数据的列和动作
        /// </summary>
        protected virtual IEnumerable<IGridColumn<TModel>> InitGridHeader()
        {
            return new List<GridColumn<TModel>>();
        }
        protected virtual List<GridAction> InitGridAction()
        {
            return new List<GridAction>();
        }

        #region GenerateExcel

        /// <summary>
        /// 生成Excel
        /// </summary>
        /// <returns>生成的Excel文件</returns>
        public virtual byte[] GenerateExcel()
        {
            NeedPage = false;
            if (GridHeaders == null)
            {
                GetHeaders();
            }
            if (IsSearched == false)
            {
                DoSearch();
            }
            HSSFWorkbook workbook = new HSSFWorkbook();
            List<HSSFSheet> sheets = new List<NPOI.HSSF.UserModel.HSSFSheet>();
            //准备好sheet，每6万行生成一个sheet
            var sheetno = ((EntityList.Count - 1) / 60000) + 1;
            var headerStyle = workbook.CreateCellStyle();
            //设定单元格边框
            headerStyle.BorderBottom = BorderStyle.Thin;
            headerStyle.BorderLeft = BorderStyle.Thin;
            headerStyle.BorderRight = BorderStyle.Thin;
            headerStyle.BorderTop = BorderStyle.Thin;
            //用灰色填充背景
            var headerbg = HSSFColor.Grey25Percent.Index;
            headerStyle.FillForegroundColor = headerbg;
            headerStyle.FillPattern = FillPattern.SolidForeground;
            headerStyle.FillBackgroundColor = headerbg;
            //去掉 Id 列和动作列
            RemoveActionAndIdColumn();
            //循环生成所有sheet，并为每个sheet添加表头
            var headerrows = 0;
            for (int i = 1; i <= sheetno; i++)
            {
                HSSFSheet sheet = workbook.CreateSheet("Sheet" + i) as HSSFSheet;
                //生成表头
                headerrows = MakeExcelHeader(sheet, GridHeaders, 0, 0, headerStyle);
                sheets.Add(sheet);
            }

            var rowIndex = headerrows;
            var colIndex = 0;

            //Excel中用到的style，每种前景色和背景色的组合为一个style。Nopi值支持4000个style，所以同样的style要重复使用
            Dictionary<string, ICellStyle> styles = new Dictionary<string, ICellStyle>();
            //Excel中用到的font，主要是为了更改字体颜色
            Dictionary<string, IFont> fonts = new Dictionary<string, IFont>();
            //循环数据
            foreach (var row in EntityList)
            {
                var sheetindex = ((rowIndex - headerrows) / 60000);
                colIndex = 0;
                string bgColor = "";
                string fColor = "";
                //获取设定的行背景色
                bgColor = SetFullRowBgColor(row);
                //获取设定的行前景色
                fColor = SetFullRowColor(row);
                var dr = sheets[sheetindex].CreateRow(rowIndex - sheetindex * 60000) as HSSFRow;
                foreach (var baseCol in GridHeaders)
                {
                    //处理枚举变量的多语言
                    bool IsEmunBoolParp = false;
                    var proType = baseCol.FieldType;
                    if (proType.IsEnumOrNullableEnum())
                    {
                        IsEmunBoolParp = true;
                    }

                    foreach (var col in baseCol.BottomChildren)
                    {

                        //获取数据，并过滤特殊字符
                        string text = Regex.Replace(col.GetText(row).ToString(), @"<[^>]*>", String.Empty);

                        //处理枚举变量的多语言

                        if (IsEmunBoolParp)
                        {
                            if (int.TryParse(text, out int enumvalue))
                            {
                                text = PropertyHelper.GetEnumDisplayName(proType, enumvalue);
                            }
                        }


                        //建立excel单元格
                        var cell = dr.CreateCell(colIndex);
                        ICellStyle style = null;
                        IFont font = null;
                        var styleKey = string.Empty;
                        //获取设定的单元格背景色
                        string backColor = col.GetBackGroundColor(row);
                        //获取设定的单元格前景色
                        string foreColor = col.GetForeGroundColor(row);
                        //如果行背景色或单元格背景色有值，则用颜色的ARGB的值作为style的key
                        if (bgColor != "" || backColor != "")
                        {
                            styleKey = backColor == "" ? bgColor : backColor;
                        }
                        //如果行前景色或单元格前景色有值，则用颜色的ARGB加上背景色的ARGB作为style的key
                        if (fColor != "" || foreColor != "")
                        {
                            styleKey += foreColor == "" ? foreColor : fColor;
                        }
                        //如果已经有符合条件的style，则使用
                        if (styles.ContainsKey(styleKey))
                        {
                            style = styles[styleKey];
                        }
                        //如果没有，则新建一个style
                        else
                        {
                            var newKey = "";
                            var newFontKey = "";
                            //新建style
                            style = workbook.CreateCellStyle();
                            //设定单元格边框
                            style.BorderBottom = BorderStyle.Thin;
                            style.BorderLeft = BorderStyle.Thin;
                            style.BorderRight = BorderStyle.Thin;
                            style.BorderTop = BorderStyle.Thin;
                            //如果行前景色或单元格前景色有值，则设定单元格的填充颜色
                            if (bgColor != "" || backColor != "")
                            {
                                newKey = backColor == "" ? bgColor : backColor;
                                var ci = Utils.GetExcelColor(backColor == "" ? bgColor : backColor);
                                style.FillForegroundColor = ci;
                                style.FillPattern = FillPattern.SolidForeground;
                                style.FillBackgroundColor = ci;
                            }
                            //如果行前景色或单元格前景色有值，则设定单元格的字体的颜色
                            if (fColor != "" || foreColor != "")
                            {
                                newFontKey = foreColor == "" ? foreColor : fColor;
                                newKey += foreColor == "" ? foreColor : fColor;
                                //如果已经有符合条件的字体，则使用
                                if (fonts.ContainsKey(newFontKey))
                                {
                                    font = fonts[newFontKey];
                                }
                                //如果没有，则新建
                                else
                                {
                                    //新建字体
                                    font = workbook.CreateFont();
                                    //设定字体颜色
                                    font.Color = Utils.GetExcelColor(foreColor == "" ? fColor : foreColor);
                                    //向集合中添加新字体
                                    fonts.Add(newFontKey, font);
                                }
                                //设定style中的字体
                                style.SetFont(font);
                            }
                            //将新建的style添加到集合中
                            styles.Add(newKey, style);
                        }
                        cell.CellStyle = style;
                        cell.SetCellValue(text);
                        colIndex++;
                    }
                }
                rowIndex++;
            }

            //获取Excel文件的二进制数据
            byte[] rv = new byte[] { };
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                rv = ms.ToArray();
            }
            return rv;
        }

        /// <summary>
        /// 生成Excel的表头
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="cols"></param>
        /// <param name="rowIndex"></param>
        /// <param name="colIndex"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        private int MakeExcelHeader(HSSFSheet sheet, IEnumerable<IGridColumn<TModel>> cols, int rowIndex, int colIndex, ICellStyle style)
        {
            var row = sheet.CreateRow(rowIndex) as HSSFRow;
            int maxLevel = cols.Select(x => x.MaxLevel).Max();
            //循环所有列
            foreach (var col in cols)
            {
                //添加新单元格
                var cell = row.CreateCell(colIndex);
                cell.CellStyle = style;
                cell.SetCellValue(col.Title);
                var bcount = col.BottomChildren.Count();
                var rowspan = 0;
                if (rowIndex == 0)
                {
                    rowspan = maxLevel - col.MaxLevel;
                }
                var cellRangeAddress = new CellRangeAddress(rowIndex, rowIndex + rowspan, colIndex, colIndex + bcount - 1);
                sheet.AddMergedRegion(cellRangeAddress);
                for (int i = cellRangeAddress.FirstRow; i <= cellRangeAddress.LastRow; i++)
                {
                    for (int j = cellRangeAddress.FirstColumn; j <= cellRangeAddress.LastColumn; j++)
                    {
                        var c = HSSFCellUtil.GetCell(HSSFCellUtil.GetRow(i, sheet), j);
                        c.CellStyle = style;
                    }
                }
                if (col.Children != null && col.Children.Count() > 0)
                {
                    MakeExcelHeader(sheet, col.Children, rowIndex + rowspan + 1, colIndex, style);
                }
                colIndex += bcount;
            }
            return maxLevel;
        }

        #endregion

        #region Old
        public SortInfo CreateSortInfo(Expression<Func<TModel, object>> pro, SortDir dir)
        {
            SortInfo rv = new SortInfo
            {
                Property = PropertyHelper.GetPropertyName(pro),
                Direction = dir
            };
            return rv;
        }

        /// <summary>
        /// InitList后触发的事件
        /// </summary>
        public event Action<IBasePagedListVM<TModel, TSearcher>> OnAfterInitList;

        /// <summary>
        ///记录批量操作时列表中选择的Id
        /// </summary>
        public List<Guid> Ids { get; set; }

        /// <summary>
        /// 每页行数
        /// </summary>
        [Obsolete("弃用，改用 DataTableHelper上的Limit")]
        public int RecordsPerPage { get; set; }

        /// <summary>
        /// 是否已经搜索过
        /// </summary>
        public bool IsSearched { get; set; }

        public bool PassSearch { get; set; }
        /// <summary>
        /// 查询模式
        /// </summary>
        public ListVMSearchModeEnum SearcherMode { get; set; }

        /// <summary>
        /// 是否需要分页
        /// </summary>
        public bool NeedPage { get; set; }

        /// <summary>
        /// 数据列表
        /// </summary>
        public List<TModel> EntityList { get; set; }


        /// <summary>
        /// 搜索条件
        /// </summary>
        public TSearcher Searcher { get; set; }

        /// <summary>
        /// 使用 VM 的 Id 来生成 SearcherDiv 的 Id
        /// </summary>
        public string SearcherDivId
        {
            get { return this.UniqueId + "Searcher"; }
        }


        /// <summary>
        /// 替换查询条件，如果被赋值，则列表会使用里面的Lambda来替换原有Query里面的Where条件
        /// </summary>
        public Expression<Func<TopBasePoco, bool>> ReplaceWhere { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BasePagedListVM()
        {
            //默认需要分页
            NeedPage = true;
            //初始化数据列表
            EntityList = new List<TModel>();
            //初始化搜索条件
            Searcher = typeof(TSearcher).GetConstructor(Type.EmptyTypes).Invoke(null) as TSearcher;
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns>数据列表</returns>
        public IEnumerable<TModel> GetEntityList()
        {
            if (IsSearched == false && (EntityList == null || EntityList.Count == 0))
            {
                DoSearch();
            }
            return EntityList?.AsEnumerable();
        }


        /// <summary>
        /// 调用InitListVM并触发OnAfterInitList事件
        /// </summary>
        public void DoInitListVM()
        {
            InitListVM();
            OnAfterInitList?.Invoke(this);
        }


        /// <summary>
        /// 初始化ListVM，继承的类应该重载这个函数来设定数据的列和动作
        /// </summary>
        protected virtual void InitListVM()
        {
        }

        public virtual bool GetIsSelected(object item)
        {
            return false;
        }

        /// <summary>
        /// 设定行前景色，继承的类应重载这个函数来根据每行的数据显示不同的前景色
        /// </summary>
        /// <param name="entity">数据</param>
        /// <returns>前景颜色</returns>
        public virtual string SetFullRowColor(object entity)
        {
            return "";
        }

        /// <summary>
        /// 设定行背景色，继承的类应重载这个函数来根据每行的数据显示不同的背景色
        /// </summary>
        /// <param name="entity">数据</param>
        /// <returns>背景颜色</returns>
        public virtual string SetFullRowBgColor(object entity)
        {
            return "";
        }

        /// <summary>
        /// 设定搜索语句，继承的类应该重载这个函数来指定自己的搜索语句
        /// </summary>
        /// <returns>搜索语句</returns>
        public virtual IOrderedQueryable<TModel> GetSearchQuery()
        {
            return DC.Set<TModel>().OrderByDescending(x => x.ID);
        }

        /// <summary>
        /// 设定导出时搜索语句，继承的类应该重载这个函数来指定自己导出时的搜索语句，如不指定则默认和搜索用的搜索语句相同
        /// </summary>
        /// <returns>搜索语句</returns>
        public virtual IOrderedQueryable<TModel> GetExportQuery()
        {
            return GetSearchQuery();
        }

        /// <summary>
        /// 设定搜索语句，继承的类应该重载这个函数来指定自己导出时的搜索语句，如不指定则默认和搜索用的搜索语句相同
        /// </summary>
        /// <returns>搜索语句</returns>
        public virtual IOrderedQueryable<TModel> GetSelectorQuery()
        {
            return GetSearchQuery();
        }

        /// <summary>
        /// 设定勾选后导出的搜索语句，继承的类应该重载这个函数来指定自己导出时的搜索语句，如不指定则默认和搜索用的搜索语句相同
        /// </summary>
        /// <returns>搜索语句</returns>
        public virtual IOrderedQueryable<TModel> GetCheckedExportQuery()
        {
            var baseQuery = GetExportQuery();
            if (ReplaceWhere == null)
            {
                WhereReplaceModifier mod = new WhereReplaceModifier(x => Ids.Contains(x.ID));
                var newExp = mod.Modify(baseQuery.Expression);
                var newQuery = baseQuery.Provider.CreateQuery<TModel>(newExp) as IOrderedQueryable<TModel>;
                return newQuery;
            }
            else
            {
                return baseQuery;
            }
        }

        /// <summary>
        /// 设定批量模式下的搜索语句，继承的类应重载这个函数来指定自己批量模式的搜索语句，如果不指定则默认使用Ids.Contains(x.Id)来代替搜索语句中的Where条件
        /// </summary>
        /// <returns>搜索语句</returns>
        public virtual IOrderedQueryable<TModel> GetBatchQuery()
        {
            var baseQuery = GetSearchQuery();
            var ids = Ids ?? new List<Guid>();
            if (ReplaceWhere == null)
            {
                WhereReplaceModifier mod = new WhereReplaceModifier(x => ids.Contains(x.ID));
                var newExp = mod.Modify(baseQuery.Expression);
                var newQuery = baseQuery.Provider.CreateQuery<TModel>(newExp) as IOrderedQueryable<TModel>;
                return newQuery;
            }
            else
            {
                return baseQuery;
            }
        }

        /// <summary>
        /// 设定主从模式的搜索语句，继承的类应该重载这个函数来指定自己主从模式的搜索语句，如不指定则默认和搜索用的搜索语句相同
        /// </summary>
        /// <returns>搜索语句</returns>
        public virtual IOrderedQueryable<TModel> GetMasterDetailsQuery()
        {
            return GetSearchQuery();
        }

        /// <summary>
        /// 进行搜索
        /// </summary>
        public virtual void DoSearch()
        {
            var cmd = GetSearchCommand();
            if (cmd == null)
            {
                IOrderedQueryable<TModel> query = null;
                //根据搜索模式调用不同的函数
                switch (SearcherMode)
                {
                    case ListVMSearchModeEnum.Search:
                        query = GetSearchQuery();
                        break;
                    case ListVMSearchModeEnum.Export:
                        query = GetExportQuery();
                        break;
                    case ListVMSearchModeEnum.Batch:
                        query = GetBatchQuery();
                        break;
                    case ListVMSearchModeEnum.MasterDetail:
                        query = GetMasterDetailsQuery();
                        break;
                    case ListVMSearchModeEnum.CheckExport:
                        query = GetCheckedExportQuery();
                        break;
                    case ListVMSearchModeEnum.Selector:
                        query = GetSelectorQuery();
                        break;
                    default:
                        query = GetSearchQuery();
                        break;
                }

                //如果设定了替换条件，则使用替换条件替换Query中的Where语句
                if (ReplaceWhere != null)
                {
                    var mod = new WhereReplaceModifier(ReplaceWhere);
                    var newExp = mod.Modify(query.Expression);
                    query = query.Provider.CreateQuery<TModel>(newExp) as IOrderedQueryable<TModel>;
                }
                if (Searcher.SortInfo != null)
                {
                    var mod = new OrderReplaceModifier(Searcher.SortInfo);
                    var newExp = mod.Modify(query.Expression);
                    query = query.Provider.CreateQuery<TModel>(newExp) as IOrderedQueryable<TModel>;
                }
                if (typeof(TModel).IsSubclassOf(typeof(PersistPoco)))
                {
                    var mod = new IsValidModifier();
                    var newExp = mod.Modify(query.Expression);
                    query = query.Provider.CreateQuery<TModel>(newExp) as IOrderedQueryable<TModel>;
                }
                if (PassSearch == false)
                {
                    //如果需要分页，则添加分页语句
                    if (NeedPage && Searcher.Limit != -1)
                    {
                        //获取返回数据的数量
                        var count = query.Count();
                        if (count < 0)
                        {
                            count = 0;
                        }
                        if (Searcher.Limit == 0)
                        {
                            Searcher.Limit = ConfigInfo?.RPP ?? 20;
                        }
                        //根据返回数据的数量，以及预先设定的每页行数来设定数据量和总页数
                        Searcher.Count = count;
                        Searcher.PageCount = (int)Math.Ceiling((1.0 * Searcher.Count / Searcher.Limit));
                        if (Searcher.Page <= 0)
                        {
                            Searcher.Page = 1;
                        }
                        if (Searcher.PageCount > 0 && Searcher.Page > Searcher.PageCount)
                        {
                            Searcher.Page = Searcher.PageCount;
                        }
                        EntityList = query.Skip((Searcher.Page - 1) * Searcher.Limit).Take(Searcher.Limit).AsNoTracking().ToList();
                    }
                    else //如果不需要分页则直接获取数据
                    {
                        EntityList = query.AsNoTracking().ToList();
                        Searcher.Count = EntityList.Count();
                        Searcher.Limit = EntityList.Count();
                        Searcher.PageCount = 1;
                        Searcher.Page = 1;
                    }
                }
                else
                {
                    EntityList = query.AsNoTracking().ToList();
                }
            }
            else
            {
                ProcessCommand(cmd);
            }
            IsSearched = true;
            //调用AfterDoSearch函数来处理自定义的后续操作
            AfterDoSearcher();
        }


        private void ProcessCommand(DbCommand cmd)
        {
            object total;

            if (DC.Database.IsMySql())
            {
                List<MySqlParameter> parms = new List<MySqlParameter>();
                foreach (MySqlParameter item in cmd.Parameters)
                {
                    parms.Add(new MySqlParameter(string.Format("@{0}", item.ParameterName), item.Value));
                }
                if (cmd.CommandType == CommandType.StoredProcedure)
                {
                    parms.Add(new MySqlParameter("@SearchMode", Enum.GetName(typeof(ListVMSearchModeEnum), SearcherMode)));
                    parms.Add(new MySqlParameter("@NeedPage", (NeedPage && Searcher.Limit != -1)));
                    parms.Add(new MySqlParameter("@CurrentPage", Searcher.Page));
                    parms.Add(new MySqlParameter("@RecordsPerPage", Searcher.Limit));
                    parms.Add(new MySqlParameter("@Sort", Searcher.SortInfo?.Property));
                    parms.Add(new MySqlParameter("@SortDir", Searcher.SortInfo?.Direction));
                    parms.Add(new MySqlParameter("@IDs", Ids == null ? "" : Ids.ToSpratedString()));

                    MySqlParameter outp = new MySqlParameter("@TotalRecords", MySqlDbType.Int64)
                    {
                        Value = 0,
                        Direction = ParameterDirection.Output
                    };
                    parms.Add(outp);
                }
                var pa = parms.ToArray();

                EntityList = DC.Run<TModel>(cmd.CommandText, cmd.CommandType, pa).ToList();
                if (cmd.CommandType == CommandType.StoredProcedure)
                {
                    total = pa.Last().Value;
                }
                else
                {
                    total = EntityList.Count;
                }
            }
            else if (DC.Database.IsNpgsql())
            {
                List<NpgsqlParameter> parms = new List<NpgsqlParameter>();
                foreach (NpgsqlParameter item in cmd.Parameters)
                {
                    parms.Add(new NpgsqlParameter(string.Format("@{0}", item.ParameterName), item.Value));
                }

                if (cmd.CommandType == CommandType.StoredProcedure)
                {
                    parms.Add(new NpgsqlParameter("@SearchMode", Enum.GetName(typeof(ListVMSearchModeEnum), SearcherMode)));
                    parms.Add(new NpgsqlParameter("@NeedPage", (NeedPage && Searcher.Limit != -1)));
                    parms.Add(new NpgsqlParameter("@CurrentPage", Searcher.Page));
                    parms.Add(new NpgsqlParameter("@RecordsPerPage", Searcher.Limit));
                    parms.Add(new NpgsqlParameter("@Sort", Searcher.SortInfo?.Property));
                    parms.Add(new NpgsqlParameter("@SortDir", Searcher.SortInfo?.Direction));
                    parms.Add(new NpgsqlParameter("@IDs", Ids == null ? "" : Ids.ToSpratedString()));

                    NpgsqlParameter outp = new NpgsqlParameter("@TotalRecords", NpgsqlDbType.Bigint)
                    {
                        Value = 0,
                        Direction = ParameterDirection.Output
                    };
                    parms.Add(outp);
                }
                var pa = parms.ToArray();

                EntityList = DC.Run<TModel>(cmd.CommandText, cmd.CommandType, pa).ToList();
                if (cmd.CommandType == CommandType.StoredProcedure)
                {
                    total = pa.Last().Value;
                }
                else
                {
                    total = EntityList.Count;
                }
            }
            else
            {
                List<SqlParameter> parms = new List<SqlParameter>();
                foreach (SqlParameter item in cmd.Parameters)
                {
                    parms.Add(new SqlParameter(string.Format("@{0}", item.ParameterName), item.Value));
                }
                if (cmd.CommandType == CommandType.StoredProcedure)
                {

                    parms.Add(new SqlParameter("@SearchMode", Enum.GetName(typeof(ListVMSearchModeEnum), SearcherMode)));
                    parms.Add(new SqlParameter("@NeedPage", (NeedPage && Searcher.Limit != -1)));
                    parms.Add(new SqlParameter("@CurrentPage", Searcher.Page));
                    parms.Add(new SqlParameter("@RecordsPerPage", Searcher.Limit));
                    parms.Add(new SqlParameter("@Sort", Searcher.SortInfo?.Property));
                    parms.Add(new SqlParameter("@SortDir", Searcher.SortInfo?.Direction));
                    parms.Add(new SqlParameter("@IDs", Ids == null ? "" : Ids.ToSpratedString()));

                    SqlParameter outp = new SqlParameter("@TotalRecords", 0)
                    {
                        Direction = ParameterDirection.Output
                    };
                    parms.Add(outp);
                }
                var pa = parms.ToArray();

                EntityList = DC.Run<TModel>(cmd.CommandText, cmd.CommandType, pa).ToList();
                if (cmd.CommandType == CommandType.StoredProcedure)
                {
                    total = pa.Last().Value;
                }
                else
                {
                    total = EntityList.Count;
                }

            }
            if (NeedPage && Searcher.Limit != -1)
            {
                if (total != null)
                {
                    try
                    {
                        Searcher.Count = long.Parse(total.ToString());
                        Searcher.PageCount = (int)((Searcher.Count - 1) / Searcher.Limit + 1);
                    }
                    catch { }
                }
            }
            else
            {
                Searcher.PageCount = EntityList.Count;
            }

        }

        public DateTime AddTime(DateTime dt, string type, int size)
        {
            switch (type)
            {
                case "year":
                    return dt.AddYears(size);
                case "month":
                    return dt.AddMonths(size);
                case "day":
                    return dt.AddDays(size);
                case "hour":
                    return dt.AddHours(size);
                case "minute":
                    return dt.AddMinutes(size);
                case "second":
                    return dt.AddSeconds(size);
                default:
                    return dt;
            }
        }

        /// <summary>
        /// 搜索后运行的函数，继承的类如果需要在搜索结束后进行其他操作，可重载这个函数
        /// </summary>
        public virtual void AfterDoSearcher()
        {
            if (SearcherMode == ListVMSearchModeEnum.Selector && Ids != null && Ids.Count > 0 && EntityList != null && EntityList.Count > 0)
            {
                foreach (var item in EntityList)
                {
                    if (Ids.Contains(item.ID))
                    {
                        item.Checked = true;
                    }
                }
            }
        }

        /// <summary>
        /// 删除所有ActionGridColumn的列
        /// </summary>
        public void RemoveActionColumn(object root = null)
        {
            if (root == null)
            {
                if (GridHeaders == null)
                {
                    GetHeaders();
                }
                root = GridHeaders;
            }
            if (root != null)
            {
                //IEnumerable<IGridColumn<TModel>>
                var aroot = root as List<GridColumn<TModel>>;
                var toRemove = aroot.Where(x => x.ColumnType == GridColumnTypeEnum.Action).FirstOrDefault();
                aroot.Remove(toRemove);
                foreach (var child in aroot)
                {
                    if (child.Children != null && child.Children.Count() > 0)
                    {
                        RemoveActionColumn(child.Children);
                    }
                }
            }
        }

        public void RemoveAction()
        {
            _gridActions = new List<GridAction>();
        }

        public void RemoveActionAndIdColumn(IEnumerable<IGridColumn<TModel>> root = null)
        {
            if (root == null)
            {
                if (GridHeaders == null)
                {
                    GetHeaders();
                }
                root = GridHeaders;
            }
            if (root != null)
            {
                var aroot = root as List<GridColumn<TModel>>;
                var remove = aroot.Where(x => x.ColumnType == GridColumnTypeEnum.Action || x.FieldName.ToLower() == "id").ToList();
                foreach (var item in remove)
                {
                    aroot.Remove(item);
                }
                foreach (var child in root)
                {
                    if (child.Children != null && child.Children.Count() > 0)
                    {
                        RemoveActionColumn(child.Children);
                    }
                }
            }
        }


        /// <summary>
        /// 添加Error列，主要为批量模式使用
        /// </summary>
        public void AddErrorColumn()
        {
            GetHeaders();
            //寻找所有Header为错误信息的列，如果没有则添加
            if (GridHeaders.Where(x => x.Title == "错误").FirstOrDefault() == null)
            {
                var temp = GridHeaders as List<GridColumn<TModel>>;
                if (temp.Where(x => x.ColumnType == GridColumnTypeEnum.Action).FirstOrDefault() == null)
                {
                    temp.Add(this.MakeGridColumn(x => x.BatchError, Width: 200, Header: "错误").SetForeGroundFunc(x => "ff0000"));
                }
                else
                {
                    temp.Insert(temp.Count - 1, this.MakeGridColumn(x => x.BatchError, Width: 200, Header: "错误").SetForeGroundFunc(x => "ff0000"));
                }
            }
        }

        public void ProcessListError(List<TModel> Entities)
        {
            EntityList = Entities;
            IsSearched = true;
            bool haserror = false;
            List<string> keys = new List<string>();
            if (string.IsNullOrEmpty(DetailGridPrix) == false)
            {
                foreach (var item in MSD.Keys)
                {
                    if (item.StartsWith(DetailGridPrix))
                    {
                        var errors = MSD[item];
                        if (errors.Count > 0)
                        {
                            Regex r = new Regex($"{DetailGridPrix}\\[(.*?)\\]");
                            try
                            {
                                if (int.TryParse(r.Match(item).Groups[1].Value, out int index))
                                {
                                    EntityList[index].BatchError = errors.Select(x => x.ErrorMessage).ToSpratedString();
                                    keys.Add(item);
                                    haserror = true;
                                }
                            }
                            catch { }
                        }
                    }
                }
                foreach (var item in keys)
                {
                    MSD.RemoveModelError(item);
                }
                if (haserror)
                {
                    AddErrorColumn();
                }
            }
        }

        public TModel CreateEmptyEntity()
        {
            return typeof(TModel).GetConstructor(Type.EmptyTypes).Invoke(null) as TModel;
        }

        public void ClearEntityList()
        {
            EntityList?.Clear();
        }

        public string DetailGridPrix { get; set; }

        #endregion
    }
}
