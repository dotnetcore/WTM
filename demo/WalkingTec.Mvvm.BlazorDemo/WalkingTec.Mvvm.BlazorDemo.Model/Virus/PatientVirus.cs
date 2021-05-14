using System;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Attributes;

namespace WalkingTec.Mvvm.Demo.Models.Virus
{
    [MiddleTable]
    public class PatientVirus: TopBasePoco
    {
        public Patient Patient { get; set; }
        public int PatientId { get; set; }
        
        public Virus Virus { get; set; }
        public Guid VirusId { get; set; }
    }
}
