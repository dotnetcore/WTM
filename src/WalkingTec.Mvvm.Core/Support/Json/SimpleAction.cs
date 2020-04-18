using System;
using System.Collections.Generic;
using System.Text;

namespace WalkingTec.Mvvm.Core.Support.Json
{
    public class SimpleAction
    {
        public Guid ID { get; set; }
        public ActionDescriptionAttribute ActionDes { get; set; }
        public string _name;
        public string ActionName
        {
            get
            {
                if (ActionDes?._localizer != null && string.IsNullOrEmpty(ActionDes?.Description) == false)
                {
                    return ActionDes._localizer[ActionDes.Description];
                }
                else
                {
                    return _name ?? "";
                }
            }
            set
            {
                _name = value;
            }
        }

        public string MethodName { get; set; }

        public Guid? ModuleId { get; set; }

        public SimpleModule Module { get; set; }

        public string Parameter { get; set; }

        public List<string> ParasToRunTest { get; set; }

        public bool IgnorePrivillege { get; set; }

        private string _url;
        public string Url
        {
            get
            {
                if (_url == null)
                {
                    if (this.Module.Area != null)
                    {
                        _url = "/" + this.Module.Area.Prefix + "/" + this.Module.ClassName + "/" + this.MethodName;
                    }
                    else
                    {
                        _url = "/" + this.Module.ClassName + "/" + this.MethodName;
                    }
                }
                return _url;
            }
            set
            {
                _url = value;
            }
        }
    }
}
