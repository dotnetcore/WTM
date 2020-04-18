using System;
using System.Collections.Generic;
using System.Text;

namespace WalkingTec.Mvvm.Core.Support.Json
{
    public class SimpleArea
    {
        public Guid ID { get; set; }
        public string AreaName { get; set; }
        public string Prefix { get; set; }

        public List<SimpleModule> Modules { get; set; }

    }
}
