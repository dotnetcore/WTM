using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Design;
using Elsa.Services.Models;

namespace WalkingTec.Mvvm.Core.WorkFlow
{
    [Trigger(
   Category = "审批",
   DisplayName = "审批",
   Description = "审批流程配置")]
    public class WtmApproveActivity : Elsa.Services.Activity
    {
        [ActivityInput(
            UIHint = ActivityInputUIHints.Dropdown,
            SupportedSyntaxes = new[] { "Json", "Liquid" },
            IsDesignerCritical = true,
            Options = new[] { "placeholder", "placeholder2" },
            Label = "审批人")]
        public ISet<string> user { get; set; } = new HashSet<string>();


        [ActivityInput(
            UIHint = ActivityInputUIHints.MultiText,
            DefaultSyntax = "Json",
            SupportedSyntaxes = new[] { "Json", "Liquid" },
            IsDesignerCritical = true
            )]
        public ISet<string> PossibleOutcomes { get; set; } = new HashSet<string>();

        [ActivityInput(IsBrowsable = false)]
        public string Remark { get; set; }

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            return Suspend();
        }

        protected override IActivityExecutionResult OnResume(ActivityExecutionContext context)
        {
            Remark = (context.Input as WtmApproveInput).Remark;
            return Outcome("啊");
        }

    }
}
