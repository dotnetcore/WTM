using System;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Demo.Models.Virus
{
    public class Report: BasePoco
    {
        [Required(ErrorMessage = "体温是必填项")]
        [Range(30,50, ErrorMessage = "体温必须在30到50度之间")]
        [Display(Name = "体温")]
        public float? Temperature { get; set; }
        
        [Display(Name = "备注")]
        public string Remark { get; set; }
        
        public Patient Patient { get; set; }
        
        [Display(Name = "患者")]
        [Required(ErrorMessage = "患者是必填项")]
        public Guid? PatientID { get; set; }
    }
}
