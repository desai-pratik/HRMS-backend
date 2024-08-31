using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Hrms.Provider
{
    public static class JsonProvider
    {
        public static T FromJson<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        public static string ToJson(this object data)
        {
            return JsonConvert.SerializeObject(data, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        public static void ToJsonFile(this object data, string location)
        {
            PathProvider.DeleteFile(location);
            StreamWriter writer = new(location, append: true);
            writer.Write(data.ToJson()); writer.Dispose();
        }
    }
}
