using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Design;
using Elsa.Services.Models;

namespace WalkingTec.Mvvm.Demo
{
    [Trigger(
Category = "其他",
DisplayName = "短信",
Description = "发送短信")]
    public class SMSActivity : Elsa.Services.Activity
    {

        [ActivityInput(
   UIHint = ActivityInputUIHints.SingleLine,
   Label = "手机号码")]
        public string Phone { get; set; }

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            return Done();
        }
    }
}
