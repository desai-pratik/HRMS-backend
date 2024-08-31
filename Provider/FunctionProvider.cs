using Commons.Collections;
using NVelocity.App;
using NVelocity;

namespace Hrms.Provider
{
    public static class StringProvider
    {
        public static string AddSpaceBeforeCapital(this string data) => string.Concat(data.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
    }

    public static class TimeProvider
    {
        public static DateTime CurrentIST() => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
    }

    public static class FunctionProvider
    {
        public static bool IsEqualString(string source, string compare)
        {
            return source.Equals(compare, StringComparison.OrdinalIgnoreCase);
        }

        public static string FillTemplate(object data, string body, string rootName = null)
        {
            VelocityContext velocityContext = new();
            VelocityEngine velocityEngine = new();
            ExtendedProperties extendedProperties = new();
            if (data == null) { return body; }
            _ = velocityContext.Put(string.IsNullOrWhiteSpace(rootName) ? data.GetType().Name : rootName, data);
            velocityEngine.Init(extendedProperties);

            using StringWriter stringWriter = new();
            _ = velocityEngine.Evaluate(velocityContext, stringWriter, string.Empty, body);
            return stringWriter.GetStringBuilder().ToString();
        }
    }
}
