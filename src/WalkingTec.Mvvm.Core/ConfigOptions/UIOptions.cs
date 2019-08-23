namespace WalkingTec.Mvvm.Core.ConfigOptions
{
    public class UIOptions
    {
        /// <summary>
        /// 默认列表行数
        /// </summary>
        public int RPP { get; set; }

        /// <summary>
        /// 默认允许ComboBox搜索
        /// </summary>
        public bool ComboBoxEnableSearch { get; set; }

        /// <summary>
        /// 默认开启DateTime只读
        /// </summary>
        public bool DateTimeReadOnly { get; set; }
    }
}
