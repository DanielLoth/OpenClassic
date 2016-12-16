using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace OpenClassic.Server.Util
{
    public class SingleObjectOrArrayJsonConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(List<T>));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var results = new List<T>();

            var token = JToken.Load(reader);
            if (token.Type == JTokenType.Array)
            {
                var multipleObjects = token.ToObject<List<T>>();

                results.AddRange(multipleObjects);
            }
            else if (token.Type == JTokenType.Object)
            {
                var singleObject = token.ToObject<T>();

                results.Add(singleObject);
            }

            return results;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {

        }
    }
}
