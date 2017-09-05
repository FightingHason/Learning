/**----------------------------------------------------------------------------//
  @file   : DictionaryExt.cs

  @brief  : Dictionary扩展类
  
  @par	  : E-Mail
            609043941@qq.cn

  @par	  : history
			2017/06/05 Ver 0.00 Created by Liuhaixia
//-----------------------------------------------------------------------------*/
using System.Collections.Generic;

namespace Ben {
    public static class DictionaryExt
    {
        /// <summary>
        /// 复制Dictionary
        /// </summary>
        public static Dictionary<TKey, TValue> TryCopy<TKey, TValue>(this Dictionary<TKey, TValue> source)
        {
            if (source == null)
                return null;

            Dictionary<TKey, TValue> resultDic = new Dictionary<TKey, TValue>();
            foreach (var item in source)
            {
                resultDic.Add(item.Key, item.Value);
            }
            return resultDic;
        }

        /// <summary>
        /// 两个Dictionary比较
        /// </summary>
        public static bool TryEqual<TKey>(this Dictionary<TKey, object> source, Dictionary<TKey, object> compare)
        {
            if (source == null && compare == null)
                return true;
            else
            {
                if ((source == null && compare != null)
                    || (source != null && compare == null))
                {
                    return false;
                }
            }

            foreach (var item in source)
            {
                if (compare.ContainsKey(item.Key))
                {
                    if (item.Value != compare[item.Key])
                        return false;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

    }//end class
}//end namespace