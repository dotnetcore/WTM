using System;

namespace WalkingTec.Mvvm.Core.Support.Json
{
    [Serializable]
    public class SimpleRole
    {
        public Guid ID { get; set; }
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
        public string Tenant { get; set; }

    }
}
