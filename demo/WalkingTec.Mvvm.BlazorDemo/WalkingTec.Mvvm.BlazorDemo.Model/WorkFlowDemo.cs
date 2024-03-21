/*************************************************************************************
 *
 * 文 件 名:	WorkFlowDemo
 * 描    述: 
 * 
 * 版    本:	V0.1
 * 创 建 者:	粗制乱造 (QQ:195593710)
 * 创建时间:	2024/3/21 18:24:36
 * ==================================================
 * 历史更新记录
 * 版本：V          修改时间：         修改人：
 * 修改内容：
 * ==================================================
*************************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Elsa.Builders;
using Microsoft.EntityFrameworkCore;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.BlazorDemo.Model
{
    /// <summary>
    /// 流程测试
    /// </summary>
    [Table("WorkFlowDemo")]

    [Display(Name = "流程测试")]
    public class WorkFlowDemo : BasePoco, IPersistPoco, Core.IWorkflow
    {

        [Display(Name = "_Model._Trains._IsValid")]
        [Comment("是否有效")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public bool IsValid { get; set; } = true;

        [Display(Name ="内容")]
        [Comment("内容")]
        [JsonConverter(typeof(JsonStringConverter))]
        public string Content { get; set; }

    }
}
