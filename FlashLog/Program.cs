using System;
using System.Collections.Generic;
using System.Text;
using Emrys.FlashLog;

namespace BenFastLog
{
    class Program
    {
        static void Main(string[] args)
        {

            FlashLogger.Instance().Register();

            FlashLogger.Debug("Debug");
            FlashLogger.Debug("Debug", new Exception("testexception"));
            FlashLogger.Info("Info");
            FlashLogger.Fatal("Fatal");
            FlashLogger.Error("Error");
            FlashLogger.Warn("Warn", new Exception("testexception"));
        }
    }
}
