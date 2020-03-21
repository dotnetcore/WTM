using System;

namespace WalkingTec.Mvvm.Core.Support.Json
{
    [Serializable]
    public class SimpleRole
    {
        public Guid Id { get; set; }
        public string RoleCode { get; set; }

    }
}
