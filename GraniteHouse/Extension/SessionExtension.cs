using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;

namespace ChainStore.Extension
{
    public static class SessionExtension
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            string jsonString = JsonConvert.SerializeObject(value, formatting: Formatting.Indented);

            session.SetString(key, jsonString);

            bool temp = Get<Dictionary<int, int>>(session, key).ContainsKey(18);
        }

        public static T Get<T>(this ISession session, string key)
        {
            var type = (typeof(T)).ToString();
            var value = session.GetString(key);
            var v = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
