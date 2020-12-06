using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
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

        [JsonIgnore]
        public string TotalText { get; set; } = Program._localizer?["Total"];

        public virtual DbCommand GetSearchCommand()
        {
            return null;
        }

        private int? _childrenDepth;


        /// <summary>
        /// 多级表头深度  默认 1级
        /// </summary>
        public int GetChildrenDepth()
        {
            if (_childrenDepth == null)
            {
                _childrenDepth = _getHeaderDepth();
            }
            return _childrenDepth.Value;
        }

        /// <summary>
        /// GridHeaders
        /// </summary>
        [JsonIgnore]
        private IEnumerable<IGridColumn<TModel>> GridHeaders { get; set; }

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
        public List<GridAction> GetGridActions()
        {
            if (_gridActions == null)
            {
                _gridActions = InitGridAction();
            }
            return _gridActions;
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

            //获取导出的表头
            if (GridHeaders == null)
            {
                GetHeaders();
            }

            //去掉ID列和Action列
            RemoveActionAndIdColumn();

            //如果没有数据源，进行查询
            if (IsSearched == false)
            {
                DoSearch();
            }

            //获取分成Excel的个数
            ExportMaxCount = ExportMaxCount == 0 ? 1000000 : (ExportMaxCount > 1000000 ? 1000000 : ExportMaxCount);
            ExportExcelCount = EntityList.Count < ExportMaxCount ? 1 : ((EntityList.Count % ExportMaxCount) == 0 ? (EntityList.Count / ExportMaxCount) : (EntityList.Count / ExportMaxCount + 1));

            //如果是1，直接下载Excel，如果是多个，下载ZIP包
            if (ExportExcelCount == 1)
            {
                return DownLoadExcel();
            }
            else
            {
                return DownLoadZipPackage(typeof(TModel).Name + "_" + DateTime.Now.ToString("yyyyMMddHHmmssffff"));
            }
        }

        /// <summary>
        /// 根据集合生成单个Excel
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        private IWorkbook GenerateWorkBook(List<TModel> List)
        {
            IWorkbook book = new XSSFWorkbook();
            ISheet sheet = book.CreateSheet();
            IRow row = sheet.CreateRow(0);

            //创建表头样式
            ICellStyle headerStyle = book.CreateCellStyle();
            headerStyle.FillBackgroundColor = ExportTitleBackColor == null ? HSSFColor.LightBlue.Index : ExportTitleBackColor.Value;
            headerStyle.FillPattern = FillPattern.SolidForeground;
            headerStyle.FillForegroundColor = ExportTitleBackColor == null ? HSSFColor.LightBlue.Index : ExportTitleBackColor.Value;
            headerStyle.BorderBottom = BorderStyle.Thin;
            headerStyle.BorderTop = BorderStyle.Thin;
            headerStyle.BorderLeft = BorderStyle.Thin;
            headerStyle.BorderRight = BorderStyle.Thin;
            IFont font = book.CreateFont();
            font.FontName = "Calibri";
            font.FontHeightInPoints = 12;
            font.Color = ExportTitleFontColor == null ? HSSFColor.Black.Index : ExportTitleFontColor.Value;
            headerStyle.SetFont(font);

            ICellStyle cellStyle = book.CreateCellStyle();
            cellStyle.BorderBottom = BorderStyle.Thin;
            cellStyle.BorderTop = BorderStyle.Thin;
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.BorderRight = BorderStyle.Thin;

            //生成表头
            int max = MakeExcelHeader(sheet, GridHeaders, 0, 0, headerStyle);

            //放入数据
            var ColIndex = 0;
            for (int i = 0; i < List.Count; i++)
            {
                ColIndex = 0;
                var DR = sheet.CreateRow(i + max);
                foreach (var baseCol in GridHeaders)
                {

                    foreach (var col in baseCol.BottomChildren)
                    {
                        //处理枚举变量的多语言
                        bool IsEmunBoolParp = false;
                        var proType = col.FieldType;
                        if (proType.IsEnumOrNullableEnum())
                        {
                            IsEmunBoolParp = true;
                        }
                        //获取数据，并过滤特殊字符
                        string text = Regex.Replace(col.GetText(List[i]).ToString(), @"<[^>]*>", String.Empty);

                        //处理枚举变量的多语言
                        if (IsEmunBoolParp)
                        {
                            if (int.TryParse(text, out int enumvalue))
                            {
                                text = PropertyHelper.GetEnumDisplayName(proType, enumvalue);
                            }
                        }

                        //建立excel单元格
                        ICell cell;
                        if (col.FieldType?.IsNumber() == true)
                        {
                            cell = DR.CreateCell(ColIndex, CellType.Numeric);
                            try
                            {
                                cell.SetCellValue(Convert.ToDouble(text));
                            }
                            catch { }
                        }
                        else
                        {
                            cell = DR.CreateCell(ColIndex);
                            cell.SetCellValue(text);
                        }
                        cell.CellStyle = cellStyle;
                        ColIndex++;
                    }
                }
            }            
            return book;
        }

        private byte[] DownLoadExcel()
        {
            var book = GenerateWorkBook(EntityList);
            byte[] rv = new byte[] { };
            using (MemoryStream ms = new MemoryStream())
            {
                book.Write(ms);
                rv = ms.ToArray();
            }
            return rv;
        }

        private byte[] DownLoadZipPackage(string FileName)
        {
            //文件根目录
            string RootPath = $"{GlobalServices.GetRequiredService<IHostingEnvironment>().WebRootPath}\\{FileName}";

            //文件夹目录
            string FilePath = $"{RootPath}//FileFolder";

            //压缩包目录
            string ZipPath = $"{RootPath}//{FileName}.zip";

            //打开文件夹
            DirectoryInfo FileFolder = new DirectoryInfo(FilePath);
            if (!FileFolder.Exists)
            {
                //创建文件夹
                FileFolder.Create();
            }
            else
            {
                //清空文件夹
                FileSystemInfo[] Files = FileFolder.GetFileSystemInfos();
                foreach (var item in Files)
                {
                    if (item is DirectoryInfo)
                    {
                        DirectoryInfo Directory = new DirectoryInfo(item.FullName);
                        Directory.Delete(true);
                    }
                    else
                    {
                        File.Delete(item.FullName);
                    }
                }
            }

            //放入数据
            for (int i = 0; i < ExportExcelCount; i++)
            {
                var List = EntityList.Skip(i * ExportMaxCount).Take(ExportMaxCount).ToList();
                var WorkBook = GenerateWorkBook(List);
                string SavePath = $"{FilePath}/{FileName}_{i + 1}.xlsx";
                using (FileStream FS = new FileStream(SavePath, FileMode.CreateNew))
                {
                    WorkBook.Write(FS);
                }
            }

            //生成压缩包
            ZipFile.CreateFromDirectory(FilePath, ZipPath);

            //读取压缩包
            FileStream ZipFS = new FileStream(ZipPath, FileMode.Open, FileAccess.Read);
            byte[] bt = new byte[ZipFS.Length];
            ZipFS.Read(bt, 0, bt.Length);
            ZipFS.Close();

            //删除根目录文件夹
            DirectoryInfo RootFolder = new DirectoryInfo(RootPath);
            if (RootFolder.Exists)
            {
                RootFolder.Delete(true);
            }

            return bt;
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
        private int MakeExcelHeader(ISheet sheet, IEnumerable<IGridColumn<TModel>> cols, int rowIndex, int colIndex, ICellStyle style)
        {
            var row = sheet.GetRow(rowIndex);
            if (row == null)
            {
                row = sheet.CreateRow(rowIndex);
            }
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
                if (rowIndex >= 0)
                {
                    rowspan = maxLevel - col.MaxLevel;
                }
                var cellRangeAddress = new CellRangeAddress(rowIndex, rowIndex + rowspan, colIndex, colIndex + bcount - 1);
                sheet.AddMergedRegion(cellRangeAddress);
                if (rowspan > 0 || bcount > 1)
                {
                    cell.CellStyle.Alignment = HorizontalAlignment.Center;
                    cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                }
                for (int i = cellRangeAddress.FirstRow; i <= cellRangeAddress.LastRow; i++)
                {
                    IRow r = CellUtil.GetRow(i, sheet);
                    for (int j = cellRangeAddress.FirstColumn; j <= cellRangeAddress.LastColumn; j++)
                    {
                        ICell c = CellUtil.GetCell(r, (short)j);
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
        public List<string> Ids { get; set; }

        /// <summary>
        /// 每页行数
        /// </summary>
        [Obsolete("弃用，改用 DataTableHelper上的Limit")]
        public int RecordsPerPage { get; set; }

        /// <summary>
        /// 是否已经搜索过
        /// </summary>
        [JsonIgnore]
        public bool IsSearched { get; set; }

        [JsonIgnore]
        public bool PassSearch { get; set; }
        /// <summary>
        /// 查询模式
        /// </summary>
        [JsonIgnore]
        public ListVMSearchModeEnum SearcherMode { get; set; }

        /// <summary>
        /// 是否需要分页
        /// </summary>
        [JsonIgnore]
        public bool NeedPage { get; set; }

        /// <summary>
        /// 允许导出Excel的最大行数，超过行数会分成多个文件，最多不能超过100万
        /// </summary>
        [JsonIgnore]
        public int ExportMaxCount { get; set; }

        /// <summary>
        /// 根据允许导出的Excel最大行数，算出最终导出的Excel个数
        /// </summary>
        [JsonIgnore]
        public int ExportExcelCount { get; set; }

        /// <summary>
        /// 导出文件第一行背景颜色，使用HSSFColor，例如：HSSFColor.Red.Index
        /// </summary>
        [JsonIgnore]
        public short? ExportTitleBackColor { get; set; }

        /// <summary>
        /// 导出文件第一行文字颜色，使用HSSFColor，例如：HSSFColor.Red.Index
        /// </summary>
        [JsonIgnore]
        public short? ExportTitleFontColor { get; set; }

        /// <summary>
        /// 数据列表
        /// </summary>
        [JsonIgnore]
        public List<TModel> EntityList { get; set; }


        /// <summary>
        /// 搜索条件
        /// </summary>
        [JsonIgnore]
        public TSearcher Searcher { get; set; }

        /// <summary>
        /// 使用 VM 的 Id 来生成 SearcherDiv 的 Id
        /// </summary>
        [JsonIgnore]
        public string SearcherDivId
        {
            get { return this.UniqueId + "Searcher"; }
        }


        /// <summary>
        /// 替换查询条件，如果被赋值，则列表会使用里面的Lambda来替换原有Query里面的Where条件
        /// </summary>
        [JsonIgnore()]
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

        public override void Validate()
        {
            Searcher?.Validate();
            base.Validate();
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
            var baseQuery = GetBatchQuery();
            return baseQuery;
        }

        /// <summary>
        /// 设定批量模式下的搜索语句，继承的类应重载这个函数来指定自己批量模式的搜索语句，如果不指定则默认使用Ids.Contains(x.Id)来代替搜索语句中的Where条件
        /// </summary>
        /// <returns>搜索语句</returns>
        public virtual IOrderedQueryable<TModel> GetBatchQuery()
        {
            var baseQuery = GetSearchQuery();
            if (ReplaceWhere == null)
            {
                var mod = new WhereReplaceModifier<TModel>(Ids.GetContainIdExpression<TModel>());
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
                    var mod = new WhereReplaceModifier<TopBasePoco>(ReplaceWhere);
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
                            Searcher.Limit = ConfigInfo?.UiOptions.DataTable.RPP ?? 20;
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

            if (Searcher.Page <= 0)
            {
                Searcher.Page = 1;
            }
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
                    var id = item.GetID();
                    if (Ids.Contains(id.ToString()))
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
                List<GridColumn<TModel>> remove = null;
                var idpro = typeof(TModel).GetProperties().Where(x => x.Name.ToLower() == "id").Select(x => x.PropertyType).FirstOrDefault();
                if (idpro == typeof(string))
                {
                    remove = aroot.Where(x => x.ColumnType == GridColumnTypeEnum.Action || x.Hide == true || x.DisableExport).ToList();
                }
                else
                {
                    remove = aroot.Where(x => x.ColumnType == GridColumnTypeEnum.Action || x.Hide == true || x.DisableExport || x.FieldName?.ToLower() == "id").ToList();
                }
                foreach (var item in remove)
                {
                    aroot.Remove(item);
                }
                foreach (var child in root)
                {
                    if (child.Children != null && child.Children.Count() > 0)
                    {
                        RemoveActionAndIdColumn(child.Children);
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
            if (GridHeaders.Where(x => x.Field == "BatchError").FirstOrDefault() == null)
            {
                var temp = GridHeaders as List<GridColumn<TModel>>;
                if (temp.Where(x => x.ColumnType == GridColumnTypeEnum.Action).FirstOrDefault() == null)
                {
                    temp.Add(this.MakeGridColumn(x => x.BatchError, Width: 200, Header: Core.Program._localizer["Error"]).SetForeGroundFunc(x => "ff0000"));
                }
                else
                {
                    temp.Insert(temp.Count - 1, this.MakeGridColumn(x => x.BatchError, Width: 200, Header: Core.Program._localizer["Error"]).SetForeGroundFunc(x => "ff0000"));
                }
            }
        }

        public void ProcessListError(List<TModel> Entities)
        {
            if(Entities == null)
            {
                return;
            }
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

        public virtual void UpdateEntityList(bool updateAllFields = false)
        {
            if (EntityList != null)
            {
                var ftype = EntityList.GetType().GenericTypeArguments.First();
                PropertyInfo[] itemPros = ftype.GetProperties();

                foreach (var newitem in EntityList)
                {
                    var subtype = newitem.GetType();
                    if (subtype.IsSubclassOf(typeof(BasePoco)))
                    {
                        BasePoco ent = newitem as BasePoco;
                        if (ent.UpdateTime == null)
                        {
                            ent.UpdateTime = DateTime.Now;
                        }
                        if (string.IsNullOrEmpty(ent.UpdateBy))
                        {
                            ent.UpdateBy = LoginUserInfo?.ITCode;
                        }
                    }
                    //循环页面传过来的子表数据,将关联到TopBasePoco的字段设为null,并且把外键字段的值设定为主表ID
                    foreach (var itempro in itemPros)
                    {
                        if (itempro.PropertyType.IsSubclassOf(typeof(TopBasePoco)))
                        {
                            itempro.SetValue(newitem, null);
                        }
                    }
                }

                IEnumerable<TopBasePoco> data = null;
                //打开新的数据库联接,获取数据库中的主表和子表数据
                using (var ndc = DC.CreateNew())
                {
                    var ids = EntityList.Select(x => x.GetID().ToString()).ToList();
                    data = ndc.Set<TModel>().AsNoTracking().Where(ids.GetContainIdExpression<TModel>()).ToList();
                }
                //比较子表原数据和新数据的区别
                IEnumerable<TopBasePoco> toadd = null;
                IEnumerable<TopBasePoco> toremove = null;
                Utils.CheckDifference(data, EntityList, out toremove, out toadd);
                //设定子表应该更新的字段
                List<string> setnames = new List<string>();
                foreach (var field in FC.Keys)
                {
                    if (field.StartsWith("EntityList[0]."))
                    {
                        string name = field.Replace("EntityList[0].", "");
                        setnames.Add(name);
                    }
                }

                //前台传过来的数据
                foreach (var newitem in EntityList)
                {
                    //数据库中的数据
                    foreach (var item in data)
                    {
                        //需要更新的数据
                        if (newitem.GetID().ToString() == item.GetID().ToString())
                        {
                            dynamic i = newitem;
                            var newitemType = item.GetType();
                            foreach (var itempro in itemPros)
                            {
                                if (!itempro.PropertyType.IsSubclassOf(typeof(TopBasePoco)) && (updateAllFields == true || setnames.Contains(itempro.Name)))
                                {
                                    var notmapped = itempro.GetCustomAttribute<NotMappedAttribute>();
                                    if (itempro.Name != "ID" && notmapped == null && itempro.PropertyType.IsList() == false)
                                    {
                                        DC.UpdateProperty(i, itempro.Name);
                                    }
                                }
                            }
                            if (item.GetType().IsSubclassOf(typeof(BasePoco)))
                            {
                                DC.UpdateProperty(i, "UpdateTime");
                                DC.UpdateProperty(i, "UpdateBy");
                            }
                        }
                    }
                }
                //需要删除的数据
                foreach (var item in toremove)
                {
                    //如果是PersistPoco，则把IsValid设为false，并不进行物理删除
                    if (ftype.IsSubclassOf(typeof(PersistPoco)))
                    {
                        (item as PersistPoco).IsValid = false;
                        (item as PersistPoco).UpdateTime = DateTime.Now;
                        (item as PersistPoco).UpdateBy = LoginUserInfo?.ITCode;
                        dynamic i = item;
                        DC.UpdateEntity(i);
                    }
                    else
                    {
                        foreach (var itempro in itemPros)
                        {
                            if (itempro.PropertyType.IsSubclassOf(typeof(TopBasePoco)))
                            {
                                itempro.SetValue(item, null);
                            }
                        }
                        dynamic i = item;
                        DC.DeleteEntity(i);
                    }
                }
                //需要添加的数据
                foreach (var item in toadd)
                {
                    if (item.GetType().IsSubclassOf(typeof(BasePoco)))
                    {
                        BasePoco ent = item as BasePoco;
                        if (ent.CreateTime == null)
                        {
                            ent.CreateTime = DateTime.Now;
                        }
                        if (string.IsNullOrEmpty(ent.CreateBy))
                        {
                            ent.CreateBy = LoginUserInfo?.ITCode;
                        }
                    }
                    DC.AddEntity(item);


                }

                DC.SaveChanges();
            }
        }
    }
}
