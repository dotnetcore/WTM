using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Demo.Models.Virus
{
    public enum VirtusTypeEnum
    {
        DNA, RNA
    }
    
    public class Virus : TopBasePoco
    {
        [Display(Name = "病毒名称")]
        [Required(ErrorMessage = "病毒名称是必填项")]
        public string VirtusName { get; set; }
        
        [Display(Name = "病毒代码")]
        [Required(ErrorMessage = "病毒代码是必填项")]
        [StringLength(10, ErrorMessage = "病毒代码最多10个字符")]
        public string VirtusCode { get; set; }
        
        [Display(Name = "病毒描述")]
        public string Remark { get; set; }
        
        [Display(Name = "病毒种类")]
        [Required(ErrorMessage = "病毒种类是必填项")]
        public VirtusTypeEnum? VirtusType { get; set; }
        
        [Display(Name = "患者")]
        public List<PatientVirus> Patients { get; set; }
    }
}
