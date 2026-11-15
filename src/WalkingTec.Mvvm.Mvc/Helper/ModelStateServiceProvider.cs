using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// 模拟MVC中的ModelState类
    /// </summary>
    public class ModelStateServiceProvider : IModelStateService
    {
        // MVC 模型状态字典
        private ModelStateDictionary _states;

        /// <summary>
        /// 构造函数，使用指定的模型状态字典初始化服务
        /// </summary>
        /// <param name="s">MVC 模型状态字典</param>
        public ModelStateServiceProvider(ModelStateDictionary s)
        {
            this._states = s;
        }

        /// <summary>
        /// 获取指定字段的所有验证错误
        /// </summary>
        /// <param name="name">字段名</param>
        /// <returns>该字段对应的错误列表</returns>
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

        /// <summary>
        /// 移除指定字段的错误信息
        /// </summary>
        /// <param name="key">字段名</param>
        public void RemoveModelError(string key)
        {
            _states.Remove(key);
        }

        /// <summary>
        /// 清空所有模型验证错误
        /// </summary>
        public void Clear()
        {
            _states.Clear();
        }

        /// <summary>
        /// 获取模型状态条目的数量
        /// </summary>
        public int Count => _states.Count;

        /// <summary>
        /// 获取所有模型状态键的集合
        /// </summary>
        public IEnumerable<string> Keys => _states.Keys;

        /// <summary>
        /// 获取模型状态是否全部通过验证
        /// </summary>
        bool IModelStateService.IsValid => _states.IsValid;

        /// <summary>
        /// 获取第一条错误信息
        /// </summary>
        /// <returns>第一条错误信息，无错误时返回空字符串</returns>
        public string GetFirstError()
        {
            // 遍历所有键，取出包含错误的字段的错误信息
            string rv = "";
            foreach (var key in Keys)
            {
                if(this[key].Count > 0){
                    rv = this[key].First().ErrorMessage;
                }
            }
            return rv;
        }
    }

}
