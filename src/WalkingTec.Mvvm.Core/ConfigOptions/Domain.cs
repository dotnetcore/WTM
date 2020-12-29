using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core.ConfigOptions
{
    public class Domain
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public string InnerAddress { get; set; }

        public string EntryUrl { get; set; }

        public string Url
        {
            get
            {
                var rv = Address;
                if (string.IsNullOrEmpty(rv) == false && rv.ToLower().StartsWith("http://") == false && rv.ToLower().StartsWith("https://") == false)
                {
                    rv = "http://" + rv;
                }
                return rv;
            }
        }

        public string InnerUrl
        {
            get
            {
                var rv = InnerAddress;
                if (string.IsNullOrEmpty(rv) == false && rv.ToLower().StartsWith("http://") == false && rv.ToLower().StartsWith("https://") == false)
                {
                    rv = "http://" + rv;
                }
                return rv;
            }
        }

    }
}
