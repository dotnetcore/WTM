using System;
using System.Collections.Generic;

namespace WalkingTec.Mvvm.Core.Support.Json
{
    /// <summary>
    /// 模块(Module)的精简传输类，保存控制器模块的元数据信息及其包含的操作列表，
    /// 常用于登录用户信息缓存和 API 输出
    /// </summary>
    public class SimpleModule 
    {
        /// <summary>
        /// 主键 ID
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 模块描述特性，包含模块的本地化描述信息
        /// </summary>
        public ActionDescriptionAttribute ActionDes { get; set; }

        /// <summary>
        /// 模块名称的后备字段，存储显式设置或本地化后的模块名称
        /// </summary>
        public string _name;
        /// <summary>
        /// 模块名称；后备字段未赋值时优先使用本地化后的 ActionDes 描述，
        /// 否则返回空字符串
        /// </summary>
        public string ModuleName
        {
            get
            {
                // 后备字段未赋值时，尝试从 ActionDes 中获取模块名称
                if (_name == null)
                {
                    // 存在本地化资源且描述不为空时，使用本地化后的描述作为模块名称
                    if (ActionDes?._localizer != null && string.IsNullOrEmpty(ActionDes?.Description) == false)
                    {
                        _name= ActionDes._localizer[ActionDes.Description];
                    }
                    else
                    {
                        // 否则保持为空字符串
                        _name= _name ?? "";
                    }
                }
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// 控制器类名(不含命名空间)
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 模块包含的操作列表
        /// </summary>
        public List<SimpleAction> Actions { get; set; }

        /// <summary>
        /// 所属区域 ID
        /// </summary>
        public Guid? AreaId { get; set; }
        /// <summary>
        /// 所属区域的精简信息
        /// </summary>
        public SimpleArea Area { get; set; }

        /// <summary>
        /// 控制器所在的命名空间
        /// </summary>
        public string NameSpace { get; set; }

        /// <summary>
        /// 是否忽略权限校验
        /// </summary>
        public bool IgnorePrivillege { get; set; }

        /// <summary>
        /// 是否仅允许在主站点访问
        /// </summary>
        public bool MainHostOnly { get; set; }
        /// <summary>
        /// 是否为 API 模块
        /// </summary>
        public bool IsApi { get; set; }

        /// <summary>
        /// 模块的完整名称，由“命名空间,类名”拼接而成，可用于定位控制器类型
        /// </summary>
        public string FullName
        {
            get
            {
                return this.NameSpace + "," + this.ClassName;
            }
        }
    }
}
