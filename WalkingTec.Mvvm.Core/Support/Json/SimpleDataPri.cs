using System;

namespace WalkingTec.Mvvm.Core
{
    [Serializable]
    public class SimpleDataPri
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string TableName { get; set; }

        public Guid? RelateId { get; set; }


    }
}
