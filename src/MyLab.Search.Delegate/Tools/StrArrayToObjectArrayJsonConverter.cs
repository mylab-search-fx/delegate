using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace MyLab.Search.Delegate.Tools
{
    class StrArrayToObjectArrayJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var arr = (string[])value;

            writer.WriteStartArray();

            if (arr != null)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    writer.WriteRaw(arr[i]);
                    if (i != arr.Length - 1)
                        writer.WriteRaw(",");
                }
            }

            writer.WriteEndArray();
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string[]);
        }
    }
}