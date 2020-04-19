using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace WalkingTec.Mvvm.Test.Mock
{
    public class MockCookie : IRequestCookieCollection
    {

        public Dictionary<string, string> Kvs = new Dictionary<string, string>();

        public string this[string key] => Kvs[key];

        public int Count => Kvs.Count;

        public ICollection<string> Keys => Kvs.Keys;

        public bool ContainsKey(string key)
        {
            return Kvs.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return Kvs.GetEnumerator();
        }

        public bool TryGetValue(string key, out string value)
        {
            return Kvs.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Kvs.GetEnumerator();
        }

        public void Add(string key, string value)
        {
            Kvs.Add(key, value);
        }
    }
}
