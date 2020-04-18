using System;

namespace WalkingTec.Mvvm.Core.Support.Json
{
    [Serializable]
    public class SimpleGroup
    {
        public Guid ID { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }

    }
}
