using System;

namespace WalkingTec.Mvvm.Core.Support.Json
{
    [Serializable]
    public class SimpleFunctionPri
    {
        public Guid ID { get; set; }
        public string RoleCode{ get; set; }

        public Guid MenuItemId { get; set; }

        public string Url { get; set; }

        public bool? Allowed { get; set; }

    }
}
