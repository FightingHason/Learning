using LitJson;
/**----------------------------------------------------------------------------//
  @file   : ExtendUtils.cs

  @brief  : 常用数据类型方法扩展
  
  @par	  : E-Mail
            609043941@qq.com
  			
  @par	  : Copyright(C) Ben Technology Inc.

  @par	  : history
			2016/08/25 Ver 0.00 Created by Liuhaixia
//-----------------------------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;

public static class ExtendUtils {

    /// <summary>
    /// RMB 分转为元(1.00)
    /// </summary>
    /// <param name="i">分的数量</param>
    /// <returns>字符串</returns>
    public static string FenToYuan(this int i)
    {
        if (i % 100 == 0)
        {
            return (i / 100).ToString();
        }
        else if (i % 10 == 0)
        {
            return ((float)i / 100f).ToString("f1");
        }
        else
        {
            return ((float)i / 100f).ToString("f2");
        }
    }

    /// <summary>
    /// 字符串转float
    /// </summary>
    /// <param name="text">待处理字符串</param>
    /// <param name="defaultValue">默认float值</param>
    /// <returns>转化后float</returns>
    public static float ToFloat(this string source, float defaultValue = 0)
    {
        if (String.IsNullOrEmpty(source))
        {
            return defaultValue;
        }
        return float.Parse(source);
    }

    /// <summary>
    /// 字符串转int
    /// </summary>
    /// <param name="text">待处理字符串</param>
    /// <param name="defaultValue">默认int值</param>
    /// <returns>转化后int</returns>
    public static int ToInt(this string source, int defaultValue = 0)
    {
        if (string.IsNullOrEmpty(source))
            return defaultValue;
        else
            return int.Parse(source);
    }

    /// <summary>
    /// 获取Text分割后字符串数组
    /// </summary>
    /// <param name="text">需要分割的字符串</param>
    /// <param name="splitStr">分割符</param>
    /// <returns>字符串数组</returns>
    public static string[] GetKVFromText(this string source, string splitStr)
    {
        return source.Split(new string[] { splitStr }, StringSplitOptions.None);
    }

    /// <summary>
    /// 截取字符串
    /// </summary>
    /// <param name="source">原始字符串</param>
    /// <param name="key">字符串key</param>
    /// <param name="isContainKey">是否包含key</param>
    /// <returns>截取成功字符串</returns>
    public static string Substring(this string source, string key, bool isContainKey = false)
    {
        int startPosition = source.IndexOf(key);
        if (isContainKey)
            return source.Substring(startPosition);
        else
            return source.Substring(startPosition + key.Length);
    }

    /// <summary>
    /// 标准化路径分割符
    /// </summary>
    public static string StandardSplitForPath(this string path)
    {
        return path.Replace("\\", "/");
    }

    /// <summary>
    /// 切割字符串，开始至第一个"/"
    /// </summary>
    public static string StartToFirstSlash(this string source)
    {
        int length = source.IndexOf("/");
        if (length == -1 || length == 0)
            length = source.Length;

        return source.Substring(0, length);
    }

    /// <summary>
    /// 切割字符串，开始至最后一个"/"
    /// </summary>
    public static string StartToLastSlash(this string source)
    {
        int length = source.LastIndexOf("/");
        if (length == -1)
            length = source.Length;

        return source.Substring(0, length);
    }

    /// <summary>
    /// 切割字符串，开始至第一个"."
    /// </summary>
    public static string StartToFirstPoint(this string source)
    {
        int length = source.IndexOf(".");
        if (length == -1 || length == 0)
            length = source.Length;

        return source.Substring(0, length);
    }

    /// <summary>
    /// 切割字符串，开始至最后一个"."
    /// </summary>
    public static string StartToLastPoint(this string source)
    {
        int length = source.LastIndexOf(".");
        if (length == -1)
            length = source.Length;

        return source.Substring(0, length);
    }

    /// <summary>
    /// 切割字符串，从最后一个"/"开始到字符串结束
    /// </summary>
    public static string LastSlashToEnd(this string source)
    {
        return source.Substring(source.LastIndexOf("/")+1);
    }

    /// <summary>
    /// 切割字符串，从最后一个"/"开始到第一个"."结束
    /// </summary>
    public static string LastSlashToPoint(this string source)
    {
        string temp = source.LastSlashToEnd();
        temp = temp.StartToFirstPoint();
        return temp;
    }

    /// <summary>
    /// 检测字符串，是否以"/"开始，如果是删除
    /// </summary>
    public static string StartWithSlash(this string source)
    {
        string temp = source;
        while (true)
        {
            if (temp.StartsWith("/"))
                temp = temp.Substring(1, temp.Length - 1);
            else
                break;
        }
        return temp;
    }

    /// <summary>
    /// 是否为数字字符串
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsNumber(this string source)
    {
        if (source == null) return false;
        if (source == "") return false;

        for (int i = 0; i < source.Length; ++i)
        {
            if (Char.IsNumber(source[i]) == false)
                return false;
        }

        return true;
    }

    /// <summary>
    /// List移除所有null
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void RemoveNull<T>(this List<T> list)
    {
        List<int> nullIndeList = new List<int>();
        int listCount = list.Count;
        for (int i = 0; i < listCount; i++)
        {
            if (list[i] == null)
                nullIndeList.Add(i);
        }

        if (nullIndeList.Count > 0)
        {
            for (int i = nullIndeList.Count - 1; i >= 0; --i)
                list.RemoveAt(nullIndeList[i]);   
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Json 转格式

    /// <summary>
    /// 获取Json数据string
    /// </summary>
    /// <param name="jsonData"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool ContainKey(this JsonData jsonData, string key)
    {
        if (jsonData != null)
        {
            if (((IDictionary)jsonData).Contains(key))
                return true;   
        }

        return false;
    }

    /// <summary>
    /// 获取Json数据string
    /// </summary>
    /// <param name="jsonData"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string GetString(this JsonData jsonData, string key, string defaultValue="")
    {
        if (jsonData.ContainKey(key))
        {
            object data = jsonData[key];
            if (data == null) return defaultValue;

            return Convert.ToString(data);
        }
        else
            return defaultValue;
    }

    /// <summary>
    /// 获取Json数据float
    /// </summary>
    /// <param name="jsonData"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static float GetFloat(this JsonData jsonData, string key, float defaultValue = 0f)
    {
        if (jsonData.ContainKey(key))
        {
            object data = jsonData[key];
            if (data == null) return defaultValue;

            return Convert.ToSingle(data.ToString());
        }
        else
            return defaultValue;
    }

    /// <summary>
    /// 获取Json数据int
    /// </summary>
    /// <param name="jsonData"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static int GetInt(this JsonData jsonData, string key, int defaultValue = 0)
    {
        if (jsonData.ContainKey(key))
        {
            object data = jsonData[key];
            if (data == null) return defaultValue;

            return Convert.ToInt32(data.ToString());
        }
        else
            return defaultValue;
    }

    /// <summary>
    /// 获取Json数据long
    /// </summary>
    /// <param name="jsonData"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static long GetLong(this JsonData jsonData, string key, long defaultValue =0)
    {
        if (jsonData.ContainKey(key))
        {
            object data = jsonData[key];
            if (data == null) return defaultValue;

            return Convert.ToInt64(data.ToString());
        }
        else
            return defaultValue;
    }

    /// <summary>
    /// 获取Json数据bool
    /// </summary>
    /// <param name="jsonData"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool GetBool(this JsonData jsonData, string key, bool defaultValue = false)
    {
        if (jsonData.ContainKey(key))
        {
            object data = jsonData[key];
            if (data == null) return defaultValue;

            return Convert.ToBoolean(data.ToString());
        }
        else
            return defaultValue;
    }

    #endregion
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region 时间转换

    /// <summary>
    /// 过期检测
    /// </summary>
    /// <param name="ms"></param>
    /// <returns></returns>
    public static bool CheckTimeExpire(this long ms)
    {
        DateTime now = DateTime.Now;
        DateTime targetTime = ConstString.StartTime.AddMilliseconds(ms);
        if (targetTime > now)
            return false;
        else
            return true;
    }

    /// <summary>
    /// 毫秒转化为时间字符串
    /// </summary>
    /// <param name="ms"></param>
    /// <returns></returns>
    public static string ConvertToTimeString(this long ms)
    {
        DateTime start = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Local);
        DateTime targetTime = start.AddMilliseconds(ms);

        return targetTime.ToString("yyyy-MM-dd HH:mm:ss");
    }

    /// <summary>
    /// 毫秒转化为时间
    /// </summary>
    /// <param name="ms"></param>
    /// <returns></returns>
    public static DateTime ConvertToDateTime(this long ms)
    {
        DateTime start = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Local);
        DateTime targetTime = start.AddMilliseconds(ms);

        return targetTime;
    }

    /// <summary>
    /// 相对于现在剩余时间(毫秒)
    /// </summary>
    /// <param name="ms"></param>
    /// <returns></returns>
    public static long CompareNowRemainDateTime(this long ms)
    {
        DateTime now = DateTime.Now;
        DateTime start = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Local);
        DateTime targetTime = start.AddMilliseconds(ms);

        long result = 0;
        if (targetTime > now)
            result = (targetTime - now).Milliseconds;

        return result;
    }

    /// <summary>
    /// 相对于现在剩余时间(毫秒)
    /// </summary>
    /// <param name="ms"></param>
    /// <returns></returns>
    public static string CompareNowRemainTimeString(this long ms)
    {
        DateTime now = DateTime.Now;
        DateTime start = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Local);
        DateTime targetTime = start.AddMilliseconds(ms);

        string result = "00:00:00";
        if (targetTime > now)
        {
            TimeSpan timeSpan = targetTime - now;
            result = string.Format("{0}:{1}:{2}", timeSpan.Hours.ToString("D2"), timeSpan.Minutes.ToString("D2"), timeSpan.Seconds.ToString("D2"));
        }

        return result;
    }

    #endregion
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
