using System;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Demo.Models.Virus
{
    public enum HospitalLevel
    { 
       [Display(Name = "三级医院")]
       Class3, 
       [Display(Name = "二级医院")]
       Class2, 
       [Display(Name = "一级医院")]
       Class1, 
    }
    
    public class Hospital: TopBasePoco, ITenant, IWorkflow
    {
        [Display(Name = "医院名称")]
        [Required(ErrorMessage = "医院名称是必填项")]
        [StringLength(3, ErrorMessage = "{0}stringmax{1}")]
        public string Name { get; set; }
        
        [Display(Name = "医院级别")]
        [Required(ErrorMessage = "医院级别是必填项")]
        public HospitalLevel? Level { get; set; }
        
        [Display(Name = "医院地点")]
        public City Location { get; set; }
        
        [Display(Name = "医院地点")]
        [Required(ErrorMessage = "医院地点是必填项")]
        public Guid? LocationId { get; set; }
        public string TenantCode { get; set; }
    }
}
