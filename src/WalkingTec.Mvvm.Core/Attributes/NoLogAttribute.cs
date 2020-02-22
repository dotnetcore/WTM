using System;
using System.Collections.Generic;
using System.Text;

namespace WalkingTec.Mvvm.Core
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class NoLogAttribute : Attribute
    {
    }
}
