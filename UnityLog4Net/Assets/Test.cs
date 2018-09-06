//************************************************
//Brief: Config Table Load && Data Manager
//
//Author: Liuhaixia
//E-Mail: 609043941@qq.com
//
//History: 2016/09/19 Created by Liuhaixia
//************************************************
using System.Collections;
using System.Collections.Generic;
using System.IO;
using log4net;
using log4net.Config;
using log4net.Unity;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// 
/// </summary>
public class Test {

	private bool m_receivedLogMessage;
        private bool m_atExpectedLevel;
        private LogType m_expectedType;
        private I_IUnityLog m_log;

        // Use this for initialization
        public void Init() {
            OnSetUp();

            const string log4Netconfig = "Log4NetConfig";
            TextAsset textAsset = Resources.Load<TextAsset>(log4Netconfig);
            Assert.IsNotNull(textAsset, string.Format("Could not load log4net config at {0}", log4Netconfig));
            using (Stream stream = GenerateStreamFromString(textAsset.text))
            {
                GlobalContext.Properties["projectid"] = "eve";
                GlobalContext.Properties["channelid"] = "001";
                GlobalContext.Properties["userid"] = "0000";
                XmlConfigurator.Configure(stream);
            }
        }

        public void ChangeUser() {
            GlobalContext.Properties["userid"] = "10001";
        }

        public void TestLog4NetConfig()
        {
            m_expectedType = LogType.Log;

            m_receivedLogMessage = false;
            m_log.Debug("Debug");
            Assert.IsTrue(m_receivedLogMessage, "Did not receive log message");
            Assert.IsTrue(m_atExpectedLevel, "Log message was recieved at an unexpected level");

            m_receivedLogMessage = false;
            m_log.Info("Info");
            Assert.IsTrue(m_receivedLogMessage);
            Assert.IsTrue(m_atExpectedLevel);

            m_expectedType = LogType.Warning;

            m_receivedLogMessage = false;
            m_log.Warn("Warn");
            Assert.IsTrue(m_receivedLogMessage);
            Assert.IsTrue(m_atExpectedLevel);

            m_expectedType = LogType.Error;

            m_receivedLogMessage = false;
            m_log.Error("Error");
            Assert.IsTrue(m_receivedLogMessage);
            Assert.IsTrue(m_atExpectedLevel);

            m_receivedLogMessage = false;
            m_log.Fatal("Fatal");
            Assert.IsTrue(m_receivedLogMessage);
            Assert.IsTrue(m_atExpectedLevel);
        }

        public void OnSetUp()
        {
            m_log = UnityLogManager.GetLogger(typeof(Test));
            Application.logMessageReceived += OnLogCallback;
        }

        private void OnLogCallback(string condition, string stacktrace, LogType type)
        {
            m_receivedLogMessage = true;
            m_atExpectedLevel = type == m_expectedType;
        }

        public void OnTearDown()
        {
            LogManager.Shutdown();
            Application.logMessageReceived -= OnLogCallback;
        }

        public Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
}
