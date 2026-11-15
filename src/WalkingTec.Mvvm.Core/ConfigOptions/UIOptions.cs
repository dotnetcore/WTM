namespace WalkingTec.Mvvm.Core.ConfigOptions
{
    /// <summary>
    /// 界面（UI）相关配置，对应 appsettings.json 中的 UIOptions 节，包含表格、下拉框、日期控件及搜索面板的默认行为
    /// </summary>
    public class UIOptions
    {
        /// <summary>
        /// 表格（数据列表）相关默认配置
        /// </summary>
        public DataTableOptions DataTable { get; set; }
        /// <summary>
        /// 下拉框相关默认配置
        /// </summary>
        public ComboBoxOptions ComboBox { get; set; }
        /// <summary>
        /// 日期控件相关默认配置
        /// </summary>
        public DateTimeOptions DateTime { get; set; }
        /// <summary>
        /// 搜索面板相关默认配置
        /// </summary>
        public SearchPanelOptions SearchPanel { get; set; }

        /// <summary>
        /// 表格（数据列表）配置项
        /// </summary>
        public class DataTableOptions
        {
            /// <summary>
            /// 默认列表行数
            /// </summary>
            public int RPP { get; set; }

            /// <summary>
            /// 是否显示表格打印按钮，默认 false
            /// </summary>
            public bool ShowPrint { get; set; }

            /// <summary>
            /// 是否显示表格列筛选按钮，默认 false
            /// </summary>
            public bool ShowFilter { get; set; }
        }

        /// <summary>
        /// 下拉框配置项
        /// </summary>
        public class ComboBoxOptions
        {

            /// <summary>
            /// 默认允许ComboBox搜索
            /// </summary>
            public bool DefaultEnableSearch { get; set; }
        }

        /// <summary>
        /// 日期控件配置项
        /// </summary>
        public class DateTimeOptions
        {

            /// <summary>
            /// 默认开启DateTime只读
            /// </summary>
            public bool DefaultReadonly { get; set; }
        }

        /// <summary>
        /// 搜索面板配置项
        /// </summary>
        public class SearchPanelOptions
        {

            /// <summary>
            /// 默认展开SearchPanel内容
            /// </summary>
            public bool DefaultExpand { get; set; }
        }
    }
}
