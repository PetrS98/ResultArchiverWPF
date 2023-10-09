using System;

namespace WPFUtilsLib.Helpers
{
    public static class TimeHelper
    {
        public static int LongStrTimeToMs(string Time)
        {
            string[] time = Time.Split(":");

            return (int.Parse(time[0]) * 3600) + (int.Parse(time[1]) * 60) + int.Parse(time[2]);
        }

        public static string MsToStrTime(int Time)
        {
            int _Time = Time;
            string[] time = new string[3];

            time[0] = (_Time / 3600).ToString();
            _Time %= 3600;

            time[1] = (_Time / 60).ToString();
            _Time %= 60;

            time[2] = _Time.ToString();

            return BuildTimeStr(time, ':');
        }

        public static string BuildTimeStr(string[] Time, char Separator)
        {
            return Time[0] + Separator + Time[1] + Separator + Time[2];
        }

        public static string BuildDateTimeStr(TimeSpan DateTime, char Separator)
        {
            return DateTime.Days.ToString() + Separator + DateTime.Hours.ToString() + Separator + DateTime.Minutes.ToString() + Separator + DateTime.Seconds.ToString();
        }

        public static DateTime GetDateTime(ushort year, byte month, byte day, byte hour, byte minute, byte second, byte milisecond = 0)
        {
            DateTime dateTime = new(year, month, day, hour, minute, second, milisecond);
            return dateTime;
        }
    }
}
