using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Support.FileHandlers;

namespace WalkingTec.Mvvm.Mvc.Helper
{
    /// <summary>
    /// WTM 上下文配置选项：注册框架服务时用于指定连接串选择器、数据权限、
    /// 文件子目录选择器和登录用户重载函数等自定义行为
    /// </summary>
    public class WtmContextOption
    {
        /// <summary>
        /// A Func that return ConnectionString key name, used for multi database program to select a db at run time
        /// </summary>
        public Func<ActionExecutingContext, string> CsSelector { get; set; }

        /// <summary>
        /// Set the data privileges that this system support
        /// </summary>
        public List<IDataPrivilege> DataPrivileges { get; set; }

        /// <summary>
        /// Set the sub directory of uploaded file, if you want to save file in different directories according to datetime or other properties, use this selector
        /// </summary>
        public Func<IWtmFileHandler, string> FileSubDirSelector { get; set; }

        /// <summary>
        /// 自定义的登录用户信息重载函数，框架在需要重新加载当前登录用户信息时调用
        /// </summary>
        public Func<WTMContext,string,LoginUserInfo> ReloadUserFunc { get; set; }

    }

}
