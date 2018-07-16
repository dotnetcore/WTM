using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc
{
    public class SessionServiceProvider : ISessionService
    {
        private ISession _inner { get; set; }

        public SessionServiceProvider(ISession s)
        {
            _inner = s;
        }
        public T Get<T>(string key)
        {
            return _inner.Get<T>(key);
        }
        public void Set<T>(string key, T val)
        {
            _inner.Set<T>(key, val);
        }

        public string SessionId
        {
            get
            {
                return _inner.Id;
            }
        }
    }
}
