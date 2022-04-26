using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;

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
        private static List<ConstructorInfo> _cis;
        public static List<ConstructorInfo> Cis
        {
            get
            {
                if (_cis == null)
                {
                    var AllAssembly = Utils.GetAllAssembly();
                     _cis = new List<ConstructorInfo>();
                    if (AllAssembly != null)
                    {
                        foreach (var ass in AllAssembly)
                        {
                            try
                            {
                                var t = ass.GetExportedTypes().Where(x => typeof(DbContext).IsAssignableFrom(x) && x.Name != "DbContext" && x.Name != "FrameworkContext" && x.Name != "EmptyContext").ToList();
                                foreach (var st in t)
                                {
                                    var ci = st.GetConstructor(new Type[] { typeof(CS) });
                                    if (ci != null)
                                    {
                                        _cis.Add(ci);
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }
                return _cis;
            }
        }

        private static List<ConstructorInfo> _cisFull;
        public static List<ConstructorInfo> CisFull
        {
            get
            {
                if (_cisFull == null)
                {
                    var AllAssembly = Utils.GetAllAssembly();
                    _cisFull = new List<ConstructorInfo>();
                    if (AllAssembly != null)
                    {
                        foreach (var ass in AllAssembly)
                        {
                            try
                            {
                                var t = ass.GetExportedTypes().Where(x => typeof(DbContext).IsAssignableFrom(x) && x.Name != "DbContext" && x.Name != "FrameworkContext" && x.Name != "EmptyContext").ToList();
                                foreach (var st in t)
                                {
                                    var ci = st.GetConstructor(new Type[] { typeof(string), typeof(DBTypeEnum) });
                                    if (ci != null)
                                    {
                                        _cisFull.Add(ci);
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }
                return _cisFull;
            }
        }

        public IDataContext CreateDC()
        {
            if (DcConstructor == null)
            {
                string dcname = DbContext;
                if (string.IsNullOrEmpty(dcname))
                {
                    dcname = "DataContext";
                }
                DcConstructor = Cis.Where(x => x.DeclaringType.Name.ToLower() == dcname.ToLower()).FirstOrDefault();
                if (DcConstructor == null)
                {
                    DcConstructor = Cis.FirstOrDefault();
                }
            }
            return (IDataContext)DcConstructor?.Invoke(new object[] { this });
        }
    }
}
