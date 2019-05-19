//*************************************************
//File:		HeaderParser.cs
//
//Brief:    WebSocket Extension Interface
//
//Author:   Liuhaixia
//
//E-Mail:   609043941@qq.com
//
//History:  2017/12/28 Created by Liuhaixia
//*************************************************

using System.Collections.Generic;
using Ben.Logger;

namespace Ben.Extensions
{
    /// <summary>
    /// Will parse a comma-separeted header value
    /// </summary>
    public sealed class HeaderParser : KeyValuePairList
    {
        public HeaderParser(string headerStr)
        {
            base.Values = Parse(headerStr);
        }

        private List<HeaderValue> Parse(string headerStr)
        {
            List<HeaderValue> result = new List<HeaderValue>();

            int pos = 0;

            try
            {
                while (pos < headerStr.Length)
                {
                    HeaderValue current = new HeaderValue();

                    current.Parse(headerStr, ref pos);

                    result.Add(current);
                }
            }
            catch(System.Exception ex)
            {
                LoggerManager.Logger.Exception("HeaderParser - Parse", headerStr, ex);
            }

            return result;
        }
    }
}
