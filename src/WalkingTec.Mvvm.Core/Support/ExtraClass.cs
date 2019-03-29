using System.Collections.Generic;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 简单属性结构类
    /// </summary>
    public class SimpleTreeTextAndValue
    {

        public object Id { get; set; }
        public object Text { get; set; }
        public object ParentId { get; set; }
    }

    /// <summary>
    /// 简单键值对类，用来存著生成下拉菜单的数据
    /// </summary>
    public class SimpleTextAndValue
    {
        public object Text { get; set; }
        public object Value { get; set; }
    }

    /// <summary>
    /// 数据库排序类
    /// </summary>
    public class SortInfo
    {
        public string Property { get; set; }
        public SortDir Direction { get; set; }
    }

    public class ErrorObj
    {
        public Dictionary<string, string> Form { get; set; }
        public List<string> Message { get; set; }
    }

}
