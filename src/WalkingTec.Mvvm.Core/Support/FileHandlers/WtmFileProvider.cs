using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Options;

namespace WalkingTec.Mvvm.Core.Support.FileHandlers
{
    public class WtmFileProvider
    {
        public string SaveMode { get; set; }
        private Dictionary<string, ConstructorInfo> _handlers;
        private ConstructorInfo _defaultHandler;
        private Configs _configs;
        private GlobalData _gd;
        public static Func<IWtmFileHandler, string> _subDirFunc;

        public WtmFileProvider(IOptions<Configs> configs, GlobalData gd)
        {
            _configs = configs.Value;
            _gd = gd;
            _handlers = new Dictionary<string, ConstructorInfo>();
            var types = _gd.GetTypesAssignableFrom<IWtmFileHandler>();
            int count = 1;
            foreach (var item in types)
            {
                var cons = item.GetConstructor(new Type[] { typeof(Configs), typeof(string) });
                var nameattr = item.GetCustomAttribute<DisplayAttribute>();
                string name = "";
                if (nameattr == null)
                {
                    name = "FileHandler" + count;
                    count++;
                }
                else
                {
                    name = nameattr.Name;
                }
                name = name.ToLower();
                if (name == _configs.FileUploadOptions.SaveFileMode.ToString().ToLower())
                {
                    _defaultHandler = cons;
                }
                _handlers.Add(name, cons);
            }
            if (_defaultHandler == null && types.Count > 0)
            {
                _defaultHandler = types[0].GetConstructor(new Type[] { typeof(Configs), typeof(string) });
            }
        }

        public IWtmFileHandler CreateFileHandler(string saveMode = null, string csName = "default")
        {
            ConstructorInfo ci = null;

            if (string.IsNullOrEmpty(saveMode))
            {
                ci = _defaultHandler;
            }
            else
            {
                saveMode = saveMode.ToLower();
                if (_handlers.ContainsKey(saveMode))
                {
                    ci = _handlers[saveMode];
                }
            }
            if (ci == null)
            {
                return new WtmDataBaseFileHandler(_configs,csName);
            }
            else
            {
                return ci.Invoke(new object[] { _configs, csName }) as IWtmFileHandler;
            }
        }
    }
}
