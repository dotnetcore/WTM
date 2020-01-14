using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace WalkingTec.Mvvm.Admin.Test
{
    public class MockHttpSession : ISession
    {
        Dictionary<string, object> sessionStorage = new Dictionary<string, object>();
        public object this[string name]
        {
            get { return sessionStorage[name]; }
            set { sessionStorage[name] = value; }
        }

        string ISession.Id
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        bool ISession.IsAvailable
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        IEnumerable<string> ISession.Keys
        {
            get { return sessionStorage.Keys; }
        }
        void ISession.Clear()
        {
            sessionStorage.Clear();
        }
        Task ISession.CommitAsync(CancellationToken cancellationToken)
        {
            Task t = new Task(() =>
            {
            });
            t.Start();
            return t;
        }

        Task ISession.LoadAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        void ISession.Remove(string key)
        {
            sessionStorage.Remove(key);
        }

        void ISession.Set(string key, byte[] value)
        {
            sessionStorage[key] = value;
        }

        bool ISession.TryGetValue(string key, out byte[] value)
        {
            if (sessionStorage.ContainsKey(key) && sessionStorage[key] != null)
            {
                value = (byte[])sessionStorage[key];
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }
    }
}
