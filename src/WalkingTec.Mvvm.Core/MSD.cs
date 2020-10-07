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
        void RemoveModelError(string key);
        int Count { get; }

        IEnumerable<string> Keys { get; }

        void Clear();

        string GetFirstError();

        bool IsValid { get; }
    }



    /// <summary>
    /// 记录错误的简单类
    /// </summary>
    public class MsdError
    {
        public string ErrorMessage { get; set; }
        public Exception Exception { get; set; }
    }


    public class BasicMSD : IModelStateService
    {
        private Dictionary<string, string> _states;

        public BasicMSD()
        {
            this._states = new Dictionary<string, string>();
        }

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

        public void RemoveModelError(string key)
        {
            _states.Remove(key);
        }

        public void Clear()
        {
            _states.Clear();
        }

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

        public int Count => _states.Count;

        public IEnumerable<string> Keys => _states.Keys;

        bool IModelStateService.IsValid => _states.Count > 0 ? false : true;
    }
}
