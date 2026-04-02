using System.Globalization;

namespace FlyITA.Web.Services;

public static class SessionExtensions
{
    public static void SetInt32(this ISession session, string key, int value)
    {
        session.SetString(key, value.ToString(CultureInfo.InvariantCulture));
    }

    public static int GetInt32(this ISession session, string key, int defaultValue = 0)
    {
        var value = session.GetString(key);
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) ? result : defaultValue;
    }

    public static void SetBoolean(this ISession session, string key, bool value)
    {
        session.SetString(key, value.ToString());
    }

    public static bool GetBoolean(this ISession session, string key, bool defaultValue = false)
    {
        var value = session.GetString(key);
        return bool.TryParse(value, out var result) ? result : defaultValue;
    }

    public static void SetDouble(this ISession session, string key, double value)
    {
        session.SetString(key, value.ToString(CultureInfo.InvariantCulture));
    }

    public static double GetDouble(this ISession session, string key, double defaultValue = 0)
    {
        var value = session.GetString(key);
        return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? result : defaultValue;
    }
}
