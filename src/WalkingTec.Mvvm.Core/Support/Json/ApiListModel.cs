using System.Collections.Generic;

namespace WalkingTec.Mvvm.Core.Support.Json
{
    /// <summary>
    /// API 分页列表返回模型，封装分页查询的数据列表、总记录数、当前页码和总页数，
    /// 用于 API 接口向前端一次性返回分页信息
    /// </summary>
    public class ApiListModel<T>
    {
        /// <summary>
        /// 当前页的数据列表
        /// </summary>
        public List<T> Data { get; set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 当前页码
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount { get; set; }
    }
}
