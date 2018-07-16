using System;

namespace WalkingTec.Mvvm.Core
{
    public class FrameworkUserGroup : BasePoco
    {
        public FrameworkUserBase User { get; set; }
        public FrameworkGroup Group { get; set; }
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
    }

}
