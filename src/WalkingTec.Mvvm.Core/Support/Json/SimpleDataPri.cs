using System;

namespace WalkingTec.Mvvm.Core.Support.Json
{
    [Serializable]
    public class SimpleDataPri
    {
        public Guid ID { get; set; }

        public Guid? UserId { get; set; }
        public Guid? GroupId { get; set; }

        public string TableName { get; set; }

        public string RelateId { get; set; }


    }
}
