using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Demo.Models
{
    public class ShipperInfo : TopBasePoco
    {
        /// <summary>
        /// 发件人code 
        /// </summary>
        [Display(Name = "发件人code")]
        [Required(ErrorMessage = "{0}是必填项")]
        [StringLength(100, ErrorMessage = "{0}最多输入{1}个字符")]
        public string shipper_code { get; set; }

        /// <summary>
        /// 发件人姓名 
        /// </summary>
        [Display(Name = "发件人姓名")]
        [Required(ErrorMessage = "{0}是必填项")]
        [StringLength(100, ErrorMessage = "{0}最多输入{1}个字符")]
        public string shipper_name { get; set; }

        /// <summary>
        /// 发件人公司名 
        /// </summary>
        [Display(Name = "发件人公司名")]
        [StringLength(50, ErrorMessage = "{0}最多输入{1}个字符")]
        public string shipper_company { get; set; }

        /// <summary>
        /// 发件人国家代码
        /// </summary>
        [Display(Name = "发件人国家代码")]
        [Required(ErrorMessage = "{0}是必填项")]
        [StringLength(10, ErrorMessage = "{0}最多输入{1}个字符")]
        public string shipper_countrycode { get; set; }
        /// <summary>
        /// 发件人省
        /// </summary>
        [Display(Name = "发件人省")]
        [StringLength(10, ErrorMessage = "{0}最多输入{1}个字符")]
        public string shipper_province { get; set; }
        /// <summary>
        /// 发件人城市
        /// </summary>
        [Display(Name = "发件人城市")]
        [StringLength(50, ErrorMessage = "{0}最多输入{1}个字符")]
        public string shipper_city { get; set; }
        /// <summary>
        /// 发件人地址
        /// </summary>
        [Display(Name = "发件人地址")]
        [Required(ErrorMessage = "{0}是必填项")]
        [StringLength(300, ErrorMessage = "{0}最多输入{1}个字符")]
        public string shipper_street { get; set; }
        /// <summary>
        /// 发件人邮编 
        /// </summary>
        [Display(Name = "发件人邮编")]
        [StringLength(10, ErrorMessage = "{0}最多输入{1}个字符")]
        public string shipper_postcode { get; set; }

        /// <summary>
        /// 发件人区域代码
        /// </summary>
        [Display(Name = "发件人区域代码")]
        [StringLength(10, ErrorMessage = "{0}最多输入{1}个字符")]
        public string shipper_areacode { get; set; }

        /// <summary>
        /// 发件人电话 
        /// </summary>
        [Display(Name = "发件人电话")]
        [StringLength(20, ErrorMessage = "{0}最多输入{1}个字符")]
        public string shipper_telephone { get; set; }

        /// <summary>
        /// 发件人手机 
        /// </summary>
        [Display(Name = "发件人手机")]
        [Required(ErrorMessage = "{0}是必填项")]
        [StringLength(20, ErrorMessage = "{0}最多输入{1}个字符")]
        public string shipper_mobile { get; set; }

        /// <summary>
        /// 发件人邮箱
        /// </summary>
        [Display(Name = "发件人邮箱")]
        [StringLength(20, ErrorMessage = "{0}最多输入{1}个字符")]
        public string shipper_email { get; set; }

        /// <summary>
        /// 发件人传真
        /// </summary>
        [Display(Name = "发件人传真")]
        [StringLength(20, ErrorMessage = "{0}最多输入{1}个字符")]
        public string shipper_fax { get; set; }
    }

}
