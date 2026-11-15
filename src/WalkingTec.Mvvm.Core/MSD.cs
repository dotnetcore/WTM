using System;
using System.Collections.Generic;
using System.Linq;

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
        /// <summary>
        /// 移除指定字段的模型错误
        /// </summary>
        /// <param name="key">字段名称</param>
        void RemoveModelError(string key);
        /// <summary>
        /// 错误数量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 所有产生错误的字段名称
        /// </summary>
        IEnumerable<string> Keys { get; }

        /// <summary>
        /// 清空所有模型错误
        /// </summary>
        void Clear();

        /// <summary>
        /// 获取第一条错误信息
        /// </summary>
        /// <returns>第一条错误信息，没有错误时返回空字符串</returns>
        string GetFirstError();

        /// <summary>
        /// 模型是否验证通过，true 表示没有错误
        /// </summary>
        bool IsValid { get; }
    }



    /// <summary>
    /// 记录错误的简单类
    /// </summary>
    public class MsdError
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// 产生错误时关联的异常对象
        /// </summary>
        public Exception Exception { get; set; }
    }


    /// <summary>
    /// 模型状态接口的基础实现，使用字典保存字段错误信息
    /// </summary>
    public class BasicMSD : IModelStateService
    {
        // 保存字段名和错误信息的字典
        private Dictionary<string, string> _states;

        /// <summary>
        /// 初始化 BasicMSD 类的新实例
        /// </summary>
        public BasicMSD()
        {
            this._states = new Dictionary<string, string>();
        }

        /// <summary>
        /// 获取指定字段的所有错误
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <returns>该字段的错误列表</returns>
        public List<MsdError> this[string name]
        {
            get
            {
                return _states.Where(x => x.Key == name).Select(x => new MsdError { ErrorMessage = x.Value }).ToList();
            }
        }

        /// <summary>
        /// 添加错误信息
        /// </summary>
        /// <param name="key">错误的字段名</param>
        /// <param name="errorMessage">错误信息</param>
        public void AddModelError(string key, string errorMessage)
        {
            _states.Add(key, errorMessage);
        }

        /// <summary>
        /// 移除指定字段的模型错误
        /// </summary>
        /// <param name="key">字段名称</param>
        public void RemoveModelError(string key)
        {
            _states.Remove(key);
        }

        /// <summary>
        /// 清空所有模型错误
        /// </summary>
        public void Clear()
        {
            _states.Clear();
        }

        /// <summary>
        /// 获取第一条错误信息
        /// </summary>
        /// <returns>第一条错误信息，没有错误时返回空字符串</returns>
        public string GetFirstError()
        {
            string rv = "";
            foreach (var key in Keys)
            {
                if (this[key].Count > 0)
                {
                    rv = this[key].First().ErrorMessage;
                }
            }
            return rv;
        }

        /// <summary>
        /// 错误数量
        /// </summary>
        public int Count => _states.Count;

        /// <summary>
        /// 所有产生错误的字段名称
        /// </summary>
        public IEnumerable<string> Keys => _states.Keys;

        /// <summary>
        /// 模型是否验证通过，true 表示没有错误
        /// </summary>
        bool IModelStateService.IsValid => _states.Count > 0 ? false : true;
    }
}
