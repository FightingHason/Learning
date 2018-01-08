//*************************************************
//File:		StringExtend.cs
//
//Brief:    String Extend Utils
//
//Author:   Liuhaixia
//
//E-Mail:   609043941@qq.com
//
//History:  2017/11/06 Created by Liuhaixia
//*************************************************

using System;

namespace Ben {
    public static class StringExtension {
        /// <summary>
        /// String 是否为null、empty、""
        /// </summary>
        public static Boolean IsNullOrEmpty (this String source) {
            return String.IsNullOrEmpty (source);
        }

        /// <summary>
        /// 替换(Clone)为""
        /// </summary>
        public static String ReplaceCloneName (this String source) {
            if (source.IsNullOrEmpty ()) {
                return source;
            }
            return source.Replace (GlobalConst.REPLACE_CLONE, GlobalConst.STRING_EMPTY);
        }

        /// <summary>
        /// 字符串转float
        /// </summary>
        /// <param name="text">待处理字符串</param>
        /// <param name="defaultValue">默认float值</param>
        /// <returns>转化后float</returns>
        public static Single ToFloat (this String source, Single defaultValue = 0) {
            if (source.IsNullOrEmpty ()) {
                return defaultValue;
            }
            return Single.Parse (source);
        }

        /// <summary>
        /// 字符串转int
        /// </summary>
        /// <param name="text">待处理字符串</param>
        /// <param name="defaultValue">默认int值</param>
        /// <returns>转化后int</returns>
        public static Int32 ToInt (this String source, Int32 defaultValue = 0) {
            if (source.IsNullOrEmpty ())
                return defaultValue;
            else
                return Int32.Parse (source);
        }

        /// <summary>
        /// 获取Text分割后字符串数组
        /// </summary>
        /// <param name="text">需要分割的字符串</param>
        /// <param name="splitStr">分割符</param>
        /// <returns>字符串数组</returns>
        public static String[] GetKVFromText (this String source, String splitStr) {
            return source.Split (new String[] { splitStr }, StringSplitOptions.None);
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="source">原始字符串</param>
        /// <param name="key">字符串key</param>
        /// <param name="isContainKey">是否包含key</param>
        /// <returns>截取成功字符串</returns>
        public static String Substring (this String source, String key, Boolean isContainKey = false) {
            Int32 startPosition = source.IndexOf (key);
            if (isContainKey)
                return source.Substring (startPosition);
            else
                return source.Substring (startPosition + key.Length);
        }

        /// <summary>
        /// 标准化路径分割符
        /// </summary>
        public static String StandardSplitForPath (this String path) {
            return path.Replace ("\\", "/");
        }

        /// <summary>
        /// 切割字符串，开始至第一个"/"
        /// </summary>
        public static String StartToFirstSlash (this String source) {
            Int32 length = source.IndexOf ("/");
            if (length == -1 || length == 0)
                length = source.Length;

            return source.Substring (0, length);
        }

        /// <summary>
        /// 切割字符串，开始至最后一个"/"
        /// </summary>
        public static String StartToLastSlash (this String source) {
            Int32 length = source.LastIndexOf ("/");
            if (length == -1)
                length = source.Length;

            return source.Substring (0, length);
        }

        /// <summary>
        /// 切割字符串，开始至第一个"."
        /// </summary>
        public static String StartToFirstPoint (this String source) {
            Int32 length = source.IndexOf (".");
            if (length == -1 || length == 0)
                length = source.Length;

            return source.Substring (0, length);
        }

        /// <summary>
        /// 切割字符串，开始至最后一个"."
        /// </summary>
        public static String StartToLastPoint (this String source) {
            Int32 length = source.LastIndexOf (".");
            if (length == -1)
                length = source.Length;

            return source.Substring (0, length);
        }

        /// <summary>
        /// 切割字符串，从最后一个"/"开始到字符串结束
        /// </summary>
        public static String LastSlashToEnd (this String source) {
            return source.Substring (source.LastIndexOf ("/") + 1);
        }

        /// <summary>
        /// 切割字符串，从最后一个"/"开始到第一个"."结束
        /// </summary>
        public static String LastSlashToPoint (this String source) {
            String temp = source.LastSlashToEnd ();
            temp = temp.StartToFirstPoint ();
            return temp;
        }

        /// <summary>
        /// 检测字符串，是否以"/"开始，如果是删除
        /// </summary>
        public static String StartWithSlash (this String source) {
            String temp = source;
            while (true) {
                if (temp.StartsWith ("/"))
                    temp = temp.Substring (1, temp.Length - 1);
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
        public static Boolean IsNumber (this String source) {
            if (source == null) return false;
            if (source == "") return false;

            for (Int32 i = 0; i < source.Length; ++i) {
                if (Char.IsNumber (source[i]) == false)
                    return false;
            }

            return true;
        }

    } //end class
} //end namespace