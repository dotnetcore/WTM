using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using WalkingTec.Mvvm.Core.Support.Json;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 用户登录信息，需要保存在Session中，所以使用Serializable标记
    /// </summary>
    public class LoginUserInfo
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 登录用户
        /// </summary>
        public string ITCode { get; set; }

        public string Name { get; set; }

        public string Memo { get; set; }

        public Guid? PhotoId { get; set; }

        public List<SimpleRole> Roles { get; set; }

        public List<SimpleGroup> Groups { get; set; }

        public Dictionary<string, object> Attributes { get; set; }
        /// <summary>
        /// 用户的页面权限列表
        /// </summary>
        public List<SimpleFunctionPri> FunctionPrivileges { get; set; }
        /// <summary>
        /// 用户的数据权限列表
        /// </summary>
        public List<SimpleDataPri> DataPrivileges { get; set; }


    }
}
