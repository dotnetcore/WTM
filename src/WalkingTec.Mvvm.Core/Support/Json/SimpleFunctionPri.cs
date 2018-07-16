using System;

namespace WalkingTec.Mvvm.Core
{
    [Serializable]
    public class SimpleFunctionPri
    {
        public Guid Id { get; set; }
        public Guid? RoleId { get; set; }

        public Guid? UserId { get; set; }

        public Guid MenuItemId { get; set; }

        public string Url { get; set; }

        public bool? Allowed { get; set; }

    }
}
