//*************************************************
//File:		IExtension.cs
//
//Brief:    WebSocket Extension Interface
//
//Author:   Liuhaixia
//
//E-Mail:   609043941@qq.com
//
//History:  2017/12/28 Created by Liuhaixia
//*************************************************

using System;
using System.Text;

namespace Ben.Logger
{
    /// <summary>
    /// A basic logger implementation to be able to log intelligently additional informations about the plugin's internal mechanism.
    /// </summary>
    public class DefaultLogger : Ben.Logger.ILogger
    {
        public Loglevels Level { get; set; }
        public String FormatVerbose { get; set; }
        public String FormatInfo { get; set; }
        public String FormatWarn { get; set; }
        public String FormatErr { get; set; }
        public String FormatEx { get; set; }

        public DefaultLogger()
        {
            FormatVerbose = "I [{0}]: {1}";
            FormatInfo = "I [{0}]: {1}";
            FormatWarn = "W [{0}]: {1}";
            FormatErr = "Err [{0}]: {1}";
            FormatEx = "Ex [{0}]: {1} - Message: {2}  StackTrace: {3}";

            Level = UnityEngine.Debug.isDebugBuild ? Loglevels.Warning : Loglevels.Error;
        }

        public void Verbose(String division, String verb)
        {
            if (Level <= Loglevels.All)
            {
                try
                {
                    UnityEngine.Debug.Log(String.Format(FormatVerbose, division, verb));
                }
                catch
                { }
            }
        }

        public void Information(String division, String info)
        {
            if (Level <= Loglevels.Information)
            {
                try
                {
                    UnityEngine.Debug.Log(String.Format(FormatInfo, division, info));
                }
                catch
                { }
            }
        }

        public void Warning(String division, String warn)
        {
            if (Level <= Loglevels.Warning)
            {
                try
                {
                    UnityEngine.Debug.LogWarning(String.Format(FormatWarn, division, warn));
                }
                catch
                { }
            }
        }

        public void Error(String division, String err)
        {
            if (Level <= Loglevels.Error)
            {
                try
                {
                    UnityEngine.Debug.LogError(String.Format(FormatErr, division, err));
                }
                catch
                { }
            }
        }

        public void Exception(String division, String msg, Exception ex)
        {
            if (Level <= Loglevels.Exception)
            {
                try
                {
                    String exceptionMessage = String.Empty;
                    if (ex == null)
                        exceptionMessage = "null";
                    else
                    {
                        StringBuilder sb = new StringBuilder();

                        Exception exception = ex;
                        int counter = 1;
                        while (exception != null)
                        {
                            sb.AppendFormat("{0}: {1} {2}", counter++.ToString(), ex.Message, ex.StackTrace);

                            exception = exception.InnerException;

                            if (exception != null)
                                sb.AppendLine();
                        }

                        exceptionMessage = sb.ToString();
                    }

                    UnityEngine.Debug.LogError(String.Format(FormatEx,
                                                                division,
                                                                msg,
                                                                exceptionMessage,
                                                                ex != null ? ex.StackTrace : "null"));
                }
                catch
                { }
            }
        }
    }
}
