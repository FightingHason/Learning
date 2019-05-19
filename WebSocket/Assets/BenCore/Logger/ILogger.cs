//*************************************************
//File:		Logger.cs
//
//Brief:    Log Enums & Logger Interface
//
//Author:   Liuhaixia
//
//E-Mail:   609043941@qq.com
//
//History:  2017/12/28 Created by Liuhaixia
//*************************************************

using System;

namespace Ben.Logger
{
    /// <summary>
    /// Available logging levels.
    /// </summary>
    public enum Loglevels
    {
        /// <summary>
        /// All message will be logged.
        /// </summary>
        All,

        /// <summary>
        /// Only Informations and above will be logged.
        /// </summary>
        Information,

        /// <summary>
        /// Only Warnings and above will be logged.
        /// </summary>
        Warning,

        /// <summary>
        /// Only Errors and above will be logged.
        /// </summary>
        Error,

        /// <summary>
        /// Only Exceptions will be logged.
        /// </summary>
        Exception,

        /// <summary>
        /// No logging will be occur.
        /// </summary>
        None
    }

    public interface ILogger
    {
        /// <summary>
        /// The minimum severity to log
        /// </summary>
        Loglevels Level { get; set; }
        String FormatVerbose { get; set; }
        String FormatInfo { get; set; }
        String FormatWarn { get; set; }
        String FormatErr { get; set; }
        String FormatEx { get; set; }

        void Verbose(String division, String verb);
        void Information(String division, String info);
        void Warning(String division, String warn);
        void Error(String division, String err);
        void Exception(String division, String msg, Exception ex);
    }
}
