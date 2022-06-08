using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core.Support.Json
{
    public class ApiListModel<T>
    {
        public List<T> Data { get; set; }
        public int Count { get; set; }
        public int Page { get; set; }
        public int PageCount { get; set; }
    }
}
