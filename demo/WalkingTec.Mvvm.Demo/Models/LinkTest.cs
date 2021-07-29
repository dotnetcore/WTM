using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Demo.Models
{
    public class LinkTest : TopBasePoco
    {
        public string name { get; set; }
        public Student Student { get; set; }
        public string StudentId { get; set; }
    }
}
