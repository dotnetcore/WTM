using System;
using System.Collections.Generic;
using System.Text;

namespace WalkingTec.Mvvm.Core
{
    public class Cors
    {
        public bool EnableAll { get; set; }
        public List<CorsPolicy> Policy { get; set; }
    }

    public class CorsPolicy
    {
        public string Name { get; set; }
        public string Domain { get; set; }
    }
}
