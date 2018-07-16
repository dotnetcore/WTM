using System;
using System.Collections.Generic;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 模型状态接口
    /// </summary>
    public interface IModelStateService
    {
        /// <summary>
        /// 索引
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        List<MsdError> this[string name] { get; }
        /// <summary>
        /// 添加模型错误
        /// </summary>
        /// <param name="key">字段名称</param>
        /// <param name="errorMessage">错误信息</param>
        void AddModelError(string key, string errorMessage);
        void RemoveModelError(string key);
        int Count { get; }

        IEnumerable<string> Keys { get; }

        void Clear();
    }



    /// <summary>
    /// 记录错误的简单类
    /// </summary>
    public class MsdError
    {
        public string ErrorMessage { get; set; }
        public Exception Exception { get; set; }
    }
}
