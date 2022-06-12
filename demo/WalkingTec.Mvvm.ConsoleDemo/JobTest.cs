using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using WalkingTec.Mvvm.Core.Support.Quartz;

namespace WalkingTec.Mvvm.ConsoleDemo
{
    [QuartzRepeat(5,0,true,30)]
    public class JobTest : WtmJob
    {
        public override Task Execute(IJobExecutionContext context)
        {
            Wtm.DoLog("test");
            return Task.CompletedTask;
        }
    }
}
