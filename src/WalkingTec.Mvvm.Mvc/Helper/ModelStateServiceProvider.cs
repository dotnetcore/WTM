using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// 模拟MVC中的ModelState类
    /// </summary>
    public class ModelStateServiceProvider : IModelStateService
    {
        private ModelStateDictionary _states;

        public ModelStateServiceProvider(ModelStateDictionary s)
        {
            this._states = s;
        }

        public List<MsdError> this[string name]
        {
            get
            {
                return _states?[name].Errors.Select(x => new MsdError { ErrorMessage = x.ErrorMessage, Exception = x.Exception }).ToList();
            }
        }

        /// <summary>
        /// 添加错误信息
        /// </summary>
        /// <param name="key">错误的字段名</param>
        /// <param name="errorMessage">错误信息</param>
        public void AddModelError(string key, string errorMessage)
        {
            if (string.IsNullOrEmpty(key))
            {
                key = Guid.NewGuid().ToString();
            }
            _states.AddModelError(key, errorMessage);
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
