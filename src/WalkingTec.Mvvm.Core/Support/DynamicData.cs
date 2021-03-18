using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core
{
    public class DynamicData : DynamicObject
    {
        public Dictionary<string, object> Fields = new Dictionary<string, object>();

        public int Count { get { return Fields.Keys.Count; } }

        public void Add(string name, object val = null)
        {
            if (!Fields.ContainsKey(name))
            {
                Fields.Add(name, val);
            }
            else
            {
                Fields[name] = val;
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (Fields.ContainsKey(binder.Name))
            {
                result = Fields[binder.Name];
                return true;
            }
            return base.TryGetMember(binder, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (!Fields.ContainsKey(binder.Name))
            {
                Fields.Add(binder.Name, value);
            }
            else
            {
                Fields[binder.Name] = value;
            }
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (Fields.ContainsKey(binder.Name) &&
                Fields[binder.Name] is Delegate)
            {
                Delegate del = Fields[binder.Name] as Delegate;
                result = del.DynamicInvoke(args);
                return true;
            }
            return base.TryInvokeMember(binder, args, out result);
        }
    }
}
