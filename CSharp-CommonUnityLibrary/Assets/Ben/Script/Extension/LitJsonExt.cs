/**----------------------------------------------------------------------------//
  @file   : LitJsonExt.cs

  @brief  : LitJson扩展类
  
  @par	  : E-Mail
            609043941@qq.cn

  @par	  : history
			2017/06/05 Ver 0.00 Created by Liuhaixia
//-----------------------------------------------------------------------------*/
using LitJson;
using System;
using System.Collections;

namespace Ben {
    public static class LitJsonExt
    {
        /// <summary>
        /// 获取Json数据string
        /// </summary>
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
        public static string GetString(this JsonData jsonData, string key, string defaultValue = "")
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
        public static float GetFloat(this JsonData jsonData, string key, float defaultValue = 0f)
        {
            if (jsonData.ContainKey(key))
            {
                object data = jsonData[key];
                if (data == null) return defaultValue;

                if (false == string.IsNullOrEmpty(data.ToString()))
                    return Convert.ToSingle(data.ToString());
            }
            return defaultValue;
        }

        /// <summary>
        /// 获取Json数据int
        /// </summary>
        public static int GetInt(this JsonData jsonData, string key, int defaultValue = 0)
        {
            if (jsonData.ContainKey(key))
            {
                object data = jsonData[key];
                if (data == null) return defaultValue;

                if (false == string.IsNullOrEmpty(data.ToString()))
                    return Convert.ToInt32(data.ToString());
            }
            return defaultValue;
        }

        /// <summary>
        /// 获取Json数据long
        /// </summary>
        public static long GetLong(this JsonData jsonData, string key, long defaultValue = 0)
        {
            if (jsonData.ContainKey(key))
            {
                object data = jsonData[key];
                if (data == null) return defaultValue;

                if (false == string.IsNullOrEmpty(data.ToString()))
                    return Convert.ToInt64(data.ToString());
            }
            return defaultValue;
        }

        /// <summary>
        /// 获取Json数据bool
        /// </summary>
        public static bool GetBool(this JsonData jsonData, string key, bool defaultValue = false)
        {
            if (jsonData.ContainKey(key))
            {
                object data = jsonData[key];
                if (data == null) return defaultValue;

                if (false == string.IsNullOrEmpty(data.ToString()))
                    return Convert.ToBoolean(data.ToString());
            }
            return defaultValue;
        }

    }// end class
}//end namespace

