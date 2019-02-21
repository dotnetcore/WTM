using System;
using WalkingTec.Mvvm.Core.Attributes;

namespace WalkingTec.Mvvm.Core
{
    [MiddleTable]
    public class FrameworkUserRole : BasePoco
    {
        public FrameworkUserBase User { get; set; }
        public FrameworkRole Role { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
