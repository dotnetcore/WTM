using System.Collections.Generic;

namespace WalkingTec.Mvvm.Demo.ViewModels.ImVMs
{
    public class FriendInfoVM
    {
        public string groupname { get; set; }

        public string id { get; set; }

        public List<ImUserInfoVM> list { get; set; }
    }
}
