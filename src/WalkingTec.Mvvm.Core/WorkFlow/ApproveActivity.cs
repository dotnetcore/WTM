using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Elsa;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Design;
using Elsa.Metadata;
using Elsa.Services.Models;

namespace WalkingTec.Mvvm.Core.WorkFlow
{
    [Trigger(
   Category = "审批",
   DisplayName = "标准审批",
   Description = "审批流程配置")]
    public class WtmApproveActivity : Elsa.Services.Activity, IActivityPropertyOptionsProvider
    {
        [ActivityInput(
            UIHint = ActivityInputUIHints.Dropdown,
            SupportedSyntaxes = new[] { "Json", "Liquid" },
            IsDesignerCritical = true,
            OptionsProvider = typeof(WtmApproveActivity),
            Label = "审批人")]
        public ISet<string> user { get; set; } = new HashSet<string>();


        [ActivityInput(
            UIHint = ActivityInputUIHints.MultiText,
            DefaultSyntax = "Json",
            SupportedSyntaxes = new[] { "Json", "Liquid" },
            DefaultValue = new[] { "同意", "拒绝" },
            Label = "分支",
            ConsiderValuesAsOutcomes = true)]
        public ICollection<string> PossibleOutcomes { get; set; } = new List<string> { "同意", "拒绝" };

        [ActivityInput(IsBrowsable = false)]
        public string Remark { get; set; }

        private WTMContext _wtm;
        public WtmApproveActivity(WTMContext wtm)
        {
            _wtm = wtm;
        }


        public object GetOptions(PropertyInfo property)
        {
            var rv = new List<ComboSelectListItem>();
            try
            {
                var check = _wtm.CallAPI<List<ComboSelectListItem>>("self", "/_admin/workflow/GetFrameworkUsers").Result;
                if (check.Data != null)
                {
                    rv = check.Data;
                }
            }
            catch { }
            return rv.Select(x=> new SelectListItem { Text = x.Text, Value = x.Value.ToString()}).ToList();
        }


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
