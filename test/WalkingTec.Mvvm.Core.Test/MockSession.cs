using System;
using System.Collections.Generic;
using System.Text;

namespace WalkingTec.Mvvm.Core.Test
{
    public class MockSession : ISessionService
    {
        private Dictionary<string,object> _inner { get; set; }

        public MockSession()
        {
            _inner = new Dictionary<string, object>();
        }
        public T Get<T>(string key)
        {
            return (T)_inner[key];
        }
        public void Set<T>(string key, T val)
        {
            if (_inner.ContainsKey(key))
            {
                _inner[key] = val;
            }
            else
            {
                _inner.Add(key, val);
            }
        }

        public string SessionId
        {
            get
            {
                return "";
            }
        }
    }

}
