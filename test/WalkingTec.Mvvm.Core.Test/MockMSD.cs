using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WalkingTec.Mvvm.Core.Test
{
    public class MockMSD : IModelStateService
    {
        private Dictionary<string,string> _states;

        public MockMSD()
        {
            this._states = new Dictionary<string, string>();
        }

        public List<MsdError> this[string name]
        {
            get
            {
                return _states.Where(x=>x.Key == name).Select(x => new MsdError { ErrorMessage = x.Value}).ToList();
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

        public int Count => _states.Count;

        public IEnumerable<string> Keys => _states.Keys;
    }

}
