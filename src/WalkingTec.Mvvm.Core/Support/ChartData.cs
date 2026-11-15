namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 图表数据点类，为前端图表组件提供绘图所需的分类、系列、数值等信息
    /// </summary>
    public class ChartData
    {
        //一级分类
        public string Category { get; set; }

        /// <summary>
        /// 数值，对应图表Y轴的值
        /// </summary>
        public double Value { get; set; }
        //散点图 x值
        public double ValueX { get; set; }

        //二级分类 不同数据集
        public string Series { get; set; }

        /// <summary>
        /// 附加数值，用于堆积图等需要基准值的图表
        /// </summary>
        public double Addition { get; set; }

    }
}
