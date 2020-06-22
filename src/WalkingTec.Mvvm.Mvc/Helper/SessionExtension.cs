using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace WalkingTec.Mvvm.Mvc
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
            session.CommitAsync().Wait();
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) :
                                  JsonSerializer.Deserialize<T>(value);
        }
    }
}
