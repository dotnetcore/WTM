using System;
using System.Collections.Generic;
using System.Text;

namespace WalkingTec.Mvvm.Core
{
    public class ChartData
    {
        //一级分类
        public string Category { get; set; }

        public int Value { get; set; }
        //散点图 x值
        public int ValueX { get; set; }

        //二级分类 不同数据集
        public string Series { get; set; }

        public int Addition { get; set; }

    }
}
