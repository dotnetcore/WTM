using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 简单属性结构类
    /// </summary>
    public class SimpleTreeTextAndValue
    {

        /// <summary>
        /// 节点ID
        /// </summary>
        public object Id { get; set; }
        /// <summary>
        /// 节点显示文本
        /// </summary>
        public object Text { get; set; }
        /// <summary>
        /// 父节点ID
        /// </summary>
        public object ParentId { get; set; }
    }

    /// <summary>
    /// 简单键值对类，用来存著生成下拉菜单的数据
    /// </summary>
    public class SimpleTextAndValue
    {
        /// <summary>
        /// 显示文本
        /// </summary>
        public object Text { get; set; }
        /// <summary>
        /// 实际值
        /// </summary>
        public object Value { get; set; }
    }

    /// <summary>
    /// 数据库排序类
    /// </summary>
    public class SortInfo
    {
        /// <summary>
        /// 排序字段名
        /// </summary>
        public string Property { get; set; }
        /// <summary>
        /// 排序方向
        /// </summary>
        public SortDir Direction { get; set; }
    }

    /// <summary>
    /// API 返回结果包装类，包含返回数据、状态码及错误信息
    /// </summary>
    /// <typeparam name="T">返回数据的类型</typeparam>
    public class ApiResult<T>
    {
        /// <summary>
        /// 返回的数据
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// HTTP 状态码
        /// </summary>
        public HttpStatusCode? StatusCode { get; set; }
        /// <summary>
        /// 错误对象，包含表单字段错误与错误消息列表
        /// </summary>
        public ErrorObj Errors { get; set; }
        /// <summary>
        /// 错误消息文本
        /// </summary>
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 构造函数，将 Data 初始化为类型 T 的默认值
        /// </summary>
        public ApiResult()
        {
            Data = default(T);
        }
    }

    /// <summary>
    /// 错误信息类，包含表单字段错误与错误消息列表
    /// </summary>
    public class ErrorObj
    {
        /// <summary>
        /// 表单字段错误集合，键为字段名，值为错误描述
        /// </summary>
        public Dictionary<string, string> Form { get; set; }
        /// <summary>
        /// 错误消息列表
        /// </summary>
        public List<string> Message { get; set; }

        /// <summary>
        /// 获取第一条错误信息，优先取消息列表首条，其次取表单字段错误，均无时返回空字符串
        /// </summary>
        /// <returns>第一条错误信息</returns>
        public string GetFirstError()
        {
            if(Message != null && Message.Any())
            {
                return Message.First();
            }
            if(Form != null && Form.Any())
            {
                return Form.First().Value;
            }
            return "";
        }
    }

}
