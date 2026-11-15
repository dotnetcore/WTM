using System;
using System.Collections.Generic;

namespace WalkingTec.Mvvm.Core.Support.Json
{
    /// <summary>
    /// 操作(Action)的精简传输类，保存控制器中单个动作方法的元数据信息，
    /// 常用于登录用户信息缓存和 API 输出
    /// </summary>
    public class SimpleAction
    {
        /// <summary>
        /// 主键 ID
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// 操作描述特性，包含操作的本地化描述信息
        /// </summary>
        public ActionDescriptionAttribute ActionDes { get; set; }
        /// <summary>
        /// 操作名称的后备字段，存储显式设置的操作名称
        /// </summary>
        public string _name;
        /// <summary>
        /// 操作名称；优先返回本地化后的 ActionDes 描述文本，
        /// 未提供本地化描述时返回显式设置的名称(未设置则为空字符串)
        /// </summary>
        public string ActionName
        {
            get
            {
                // 存在本地化资源且描述不为空时，返回本地化后的描述文本
                if (ActionDes?._localizer != null && string.IsNullOrEmpty(ActionDes?.Description) == false)
                {
                    return ActionDes._localizer[ActionDes.Description];
                }
                else
                {
                    // 否则返回显式设置的名称，未设置时返回空字符串
                    return _name ?? "";
                }
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// 动作方法的名称
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// 所属模块 ID
        /// </summary>
        public Guid? ModuleId { get; set; }

        /// <summary>
        /// 所属模块的精简信息
        /// </summary>
        public SimpleModule Module { get; set; }

        /// <summary>
        /// 方法的参数信息
        /// </summary>
        public string Parameter { get; set; }

        /// <summary>
        /// 运行测试时使用的参数列表
        /// </summary>
        public List<string> ParasToRunTest { get; set; }

        /// <summary>
        /// 是否忽略权限校验
        /// </summary>
        public bool IgnorePrivillege { get; set; }

        /// <summary>
        /// 是否仅允许在主站点访问
        /// </summary>
        public bool MainHostOnly { get; set; }

        private string _url;
        /// <summary>
        /// 操作对应的访问 Url；未显式设置时按“区域前缀/控制器类名/方法名”的规则自动拼接
        /// </summary>
        public string Url
        {
            get
            {
                // 未显式设置 Url 时，根据所属模块的信息自动拼接
                if (_url == null)
                {
                    // 模块属于某个区域时，Url 需包含区域前缀
                    if (this.Module.Area != null)
                    {
                        _url = "/" + this.Module.Area.Prefix + "/" + this.Module.ClassName + "/" + MethodName;
                    }
                    else
                    {
                        // 不属于任何区域时，直接使用“类名/方法名”
                        _url = "/" + this.Module.ClassName + "/" + MethodName;
                    }
                }
                return _url;
            }
            set
            {
                _url = value;
            }
        }
    }
}
