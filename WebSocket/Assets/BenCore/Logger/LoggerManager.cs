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
    public class LoggerManager
    {
        static ILogger logger;

        // Static constructor. Setup default values
        static LoggerManager()
        {
            // Set the default logger mechanism
            logger = new DefaultLogger();
        }

        /// <summary>
        /// A basic BestHTTP.Logger.ILogger implementation to be able to log intelligently additional informations about the plugin's internal mechanism.
        /// </summary>
        public static ILogger Logger
        {
            get
            {
                // Make sure that it has a valid logger instance.
                if (logger == null)
                {
                    logger = new DefaultLogger();
                    logger.Level = Loglevels.None;
                }

                return logger;
            }

            set { logger = value; }
        }

    }
}
