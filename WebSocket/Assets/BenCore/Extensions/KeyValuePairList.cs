//*************************************************
//File:		KeyValuePairList.cs
//
//Brief:    Key Value Pair List
//
//Author:   Liuhaixia
//
//E-Mail:   609043941@qq.com
//
//History:  2017/12/28 Created by Liuhaixia
//*************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ben.Extensions
{
    /// <summary>
    /// Base class for specialized parsers
    /// </summary>
    public class KeyValuePairList
    {
        public List<HeaderValue> Values { get; protected set; }

        public bool TryGet(string value, out HeaderValue @param)
        {
            @param = null;
            for (int i = 0; i < Values.Count; ++i)
                if (string.CompareOrdinal(Values[i].Key, value) == 0)
                {
                    @param = Values[i];
                    return true;
                }
            return false;
        }
    }
}
