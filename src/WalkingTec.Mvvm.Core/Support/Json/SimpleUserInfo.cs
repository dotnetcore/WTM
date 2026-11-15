using System;
using System.Collections.Generic;

namespace WalkingTec.Mvvm.Core.Support.Json
{
    /// <summary>
    /// 登录用户的精简信息类，保存认证通过后需要缓存或输出的用户信息及其角色、权限列表
    /// </summary>
    public class SimpleUserInfo
    {
        /// <summary>
        /// 用户 ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 登录用户
        /// </summary>
        public string ITCode { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 用户的角色列表
        /// </summary>
        public List<SimpleRole> Roles { get; set; }
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
