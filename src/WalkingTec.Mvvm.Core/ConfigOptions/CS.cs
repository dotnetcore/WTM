using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace WalkingTec.Mvvm.Core
{
    public class CS
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public DBTypeEnum? DbType { get; set; }
        public string Version { get; set; }
        public string DbContext { get; set; }

        public ConstructorInfo DcConstructor;

        public IDataContext CreateDC()
        {
            return (IDataContext)DcConstructor?.Invoke(new object[] { this });
        }
    }
}
