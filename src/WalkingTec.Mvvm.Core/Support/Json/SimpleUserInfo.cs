using System;
using System.Collections.Generic;

namespace WalkingTec.Mvvm.Core.Support.Json
{
    public class SimpleUserInfo
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 登录用户
        /// </summary>
        public string ITCode { get; set; }

        public string Name { get; set; }

        public string Memo { get; set; }

        public List<SimpleRole> Roles { get; set; }
        /// <summary>
        /// 用户的页面权限列表
        /// </summary>
        public List<SimpleFunctionPri> FunctionPrivileges { get; set; }

        public List<SimpleDataPri> DataPrivileges { get; set; }
    }
}
