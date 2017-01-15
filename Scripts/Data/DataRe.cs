using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Data
{
    public class DataRe
    {
        public static long DateTimeToUnixSeconds(DateTime date)
        {
            TimeSpan span = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            return (long) span.TotalSeconds;
        }

        public static string DateTimeToUniString(DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static DateTime UnixTimeToDateTime(long unixSeconds)
        {
            DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return UnixEpoch.AddSeconds(unixSeconds);
        }

        public static string ReadableStringFromUnix(string unix)
        {
            try
            {
                long l = long.Parse(unix);
                return DateTimeToUniString(UnixTimeToDateTime(l));
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string ReadableStringFromUnix(long unix)
        {
            return DateTimeToUniString(UnixTimeToDateTime(unix));
        }

        public static int[] SplitVersionName(string version)
        {
            string[] s = version.Split('.');
            if (s.Length == 2)
            {
                int[] codes = new int[2];
                codes[0] = Convert.ToInt32(s[0]);
                codes[1] = Convert.ToInt32(s[1]);
                return codes;
            }
            return null;
        }
    }
}