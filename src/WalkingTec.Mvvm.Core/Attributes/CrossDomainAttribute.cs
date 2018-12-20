using System;
using System.Collections.Generic;
using System.Text;

namespace WalkingTec.Mvvm.Core
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class CrossDomainAttribute : Attribute
    {
        public string AllowOrigin { get; set; }

        public CrossDomainAttribute(string AllowOrigin = "*")
        {
            this.AllowOrigin = AllowOrigin;
        }
    }
}
