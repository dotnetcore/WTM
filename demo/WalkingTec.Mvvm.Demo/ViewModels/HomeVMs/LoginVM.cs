using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Demo.ViewModels.HomeVMs
{
    public class LoginVM : BaseVM
    {
        [Display(Name = "账号")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(50)]
        public string ITCode { get; set; }

        [Display(Name = "密码")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(50, ErrorMessage = "{0}最多输入{1}个字符")]
        public string Password { get; set; }

        [Display(Name = "记录我的登录")]
        public bool RememberLogin { get; set; }

        public string Redirect { get; set; }

        [Display(Name = "验证码")]
        public string VerifyCode { get; set; }

        /// <summary>
        /// 进行登录
        /// </summary>
        /// <param name="ignorePris">外部传递的页面权限</param>
        /// <returns>登录用户的信息</returns>
        public LoginUserInfo DoLogin(bool ignorePris = false)
        {
            //根据用户名和密码查询用户
            var user = WtmContext.LoadUserFromDB(null, ITCode, Password).Result;

            //如果没有找到则输出错误
            if (user == null)
            {
                MSD.AddModelError("", "登录失败");
                return null;
            }
            return user;
        }


    }
}
