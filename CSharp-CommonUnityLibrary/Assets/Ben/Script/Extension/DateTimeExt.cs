/**----------------------------------------------------------------------------//
  @file   : DateTimeExt.cs

  @brief  : DateTime扩展类
  
  @par	  : E-Mail
            609043941@qq.cn

  @par	  : history
			2017/06/05 Ver 0.00 Created by Liuhaixia
//-----------------------------------------------------------------------------*/

using System;
namespace Ben {
    public static class DateTimeExt
    {
        static readonly DateTime START_TIME = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Local);
        static string TIME_FORMAT_1 = "{0}:{1}:{2}";
        static string TIME_FORMAT_DAY_CN = "{0}天{1}时{2}分{3}秒";

        /// <summary>
        /// 毫秒转化为时间
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this long ms)
        {
            return START_TIME.AddMilliseconds(ms);
        }

        /// <summary>
        /// 毫秒转化为时间字符串
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static string ToTimeString(this long ms)
        {
            return ms.ToDateTime().ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 检测时间间隔天数
        /// </summary>
        /// <param name="after">截止时间</param>
        /// <param name="before">开始时间</param>
        /// <returns>间隔天数</returns>
        public static int CheckDateDistance(DateTime after, DateTime before)
        {
            if (after.CompareTo(before) == 0)
                return 0;

            var timeDist = after - before;
            int result = (int)Math.Floor(timeDist.TotalSeconds);

            DateTime tempAfter = new DateTime(2000, 1, 1, after.Hour, after.Minute, after.Second);
            DateTime tempBefore = new DateTime(2000, 1, 1, before.Hour, before.Minute, before.Second);

            if (tempBefore > tempAfter)
                result++;

            return result;
        }

        /// <summary>
        /// 检测日期区间有效性
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">截止时间</param>
        /// <returns>是否有效</returns>
        public static bool CheckDateValid(DateTime currentDate, DateTime startDate, DateTime endDate)
        {
            return (currentDate.CompareTo(startDate) >= 0) && (currentDate.CompareTo(endDate) <= 0);
        }

        /// <summary>
        /// String Time To String
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string TimeToString(string msString)
        {
            long ms = 0;

            try
            {
                ms = Convert.ToInt64(msString);
            }
            catch
            {
                ms = 0;
            }

            int hour = (int)(ms / (3600 * 1000));
            int min = (int)(ms % (3600 * 1000) / (60 * 1000));
            int sec = (int)(ms % (3600 * 1000) % (60 * 1000));

            return string.Format(TIME_FORMAT_1, hour, min, sec);
        }

        /// <summary>
        /// Long Time To String
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string TimeToString(long seconds)
        {
            int hour = (int)(seconds / 3600);
            int min = (int)(seconds % 3600 / 60);
            int sec = (int)(seconds % 3600 % 60);

            return string.Format(TIME_FORMAT_1, hour.ToString("D2"), min.ToString("D2"), sec.ToString("D2"));
        }

        /// <summary>
        /// Long Time To String
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string TimeToStringDay(long seconds)
        {
            int day = (int)(seconds / (24 * 60 * 60));
            int hour = (int)(seconds % (24 * 60 * 60) / 3600);
            int min = (int)(seconds % (24 * 60 * 60) % 3600 / 60);
            int sec = (int)(seconds % (24 * 60 * 60) % 3600 % 60);

            return string.Format(TIME_FORMAT_DAY_CN, day, hour, min, sec);
        }

        /// <summary>
        /// 相对于现在剩余时间(毫秒)
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static string RemainTimeStr(this long ms)
        {
            DateTime now = DateTime.Now;
            DateTime targetTime = START_TIME.AddMilliseconds(ms);

            string result = "00:00:00";
            if (targetTime > now)
            {
                TimeSpan timeSpan = targetTime - now;
                result = string.Format(TIME_FORMAT_1, timeSpan.Hours.ToString("D2"), timeSpan.Minutes.ToString("D2"), timeSpan.Seconds.ToString("D2"));
            }

            return result;
        }

        /// <summary>
        /// 相对于现在剩余时间(毫秒)
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static long RemainTimeLong(this long ms)
        {
            DateTime now = DateTime.Now;
            DateTime targetTime = START_TIME.AddMilliseconds(ms);

            long result = 0;
            if (targetTime > now)
                result = (targetTime - now).Milliseconds;

            return result;
        }

        /// <summary>
        /// 过期检测
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static bool CheckTimeExpire(this long ms)
        {
            DateTime now = DateTime.Now;
            DateTime targetTime = START_TIME.AddMilliseconds(ms);
            if (targetTime > now)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 获取当前时间秒数
        /// </summary>
        /// <returns></returns>
        public static long CurrentTimeSecond()
        {
            return DateTime.Now.Second + DateTime.Now.Minute * 60 + DateTime.Now.Hour * 3600;
        }

        /// <summary>
        /// 获取当前时间毫秒数
        /// </summary>
        /// <returns></returns>
        public static long CurrentTimeMillisecond()
        {
            return DateTime.Now.Hour * 3600000 + DateTime.Now.Minute * 60000 + DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;
        }

    }//end class
}//end namespace