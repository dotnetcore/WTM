using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// ModelStateDictionary 扩展类，将模型验证错误转换为前端可识别的 ErrorObj 数据
    /// </summary>
    public static class ModelStateExtension
    {
        /// <summary>
        /// 将模型验证错误转换为 ErrorObj 对象：
        /// 字段级错误放入 Form 字典，表单整体错误（键为空或以空格开头）放入 Message 列表
        /// </summary>
        /// <param name="self">模型状态字典</param>
        /// <returns>包含字段级与表单级错误信息的 ErrorObj 对象</returns>
        public static ErrorObj GetErrorJson(this ModelStateDictionary self)
        {
            var mse = new ErrorObj();
            mse.Form = new Dictionary<string, string>();
            mse.Message = new List<string>();
            foreach (var item in self)
            {
                // 只处理验证失败的条目
                if (item.Value.ValidationState == ModelValidationState.Invalid)
                {
                    var name = item.Key;
                    // 键不为空且不以空格开头视为字段级错误，否则视为表单整体错误
                    if (name.ToLower().StartsWith(" ") == false && name != "")
                    {
                        mse.Form.Add(name, item.Value.Errors.FirstOrDefault()?.ErrorMessage);
                    }
                    else
                    {
                        mse.Message.Add(item.Value.Errors.FirstOrDefault()?.ErrorMessage);
                    }
                }
            }
            return mse;
        }

    }
}
