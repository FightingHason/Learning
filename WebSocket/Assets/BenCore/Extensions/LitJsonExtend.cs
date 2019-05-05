//*************************************************
//File:		LitJsonExtend.cs
//
//Brief:    LitJson Extend Utils
//
//Author:   Liuhaixia
//
//E-Mail:   609043941@qq.com
//
//History:  2017/12/13 Created by Liuhaixia
//*************************************************

using System;
using LitJson;
using System.Collections;

namespace Ben {
    public static class LitJsonExtend {
        /// <summary>
        /// ContainKey
        /// </summary>
        public static Boolean ContainKey(this JsonData jsonData, String key)
        {
            if (((IDictionary)jsonData).Contains(key))
                return true;

            return false;
        }

        /// <summary>
        /// Get String info from json
        /// </summary>
        public static String GetString(this JsonData jsonData, String key, String defaultValue="")
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
        /// Get single info from json
        /// </summary>
        public static Single GetFloat(this JsonData jsonData, String key, Single defaultValue = 0f)
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
        /// Get int32 info from json
        /// </summary>
        public static Int32 GetInt(this JsonData jsonData, String key, Int32 defaultValue = 0)
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
        /// Get String info from json
        /// </summary>
        public static Int64 GetLong(this JsonData jsonData, String key, Int64 defaultValue =0)
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
        /// Get Boolean info from json
        /// </summary>
        public static Boolean GetBool(this JsonData jsonData, String key, Boolean defaultValue = false)
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

    } //end class
} //end namespace