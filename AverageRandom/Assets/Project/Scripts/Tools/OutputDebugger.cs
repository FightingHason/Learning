using System;
using System.IO;
using System.Threading;
/**----------------------------------------------------------------------------//
  @file   : OutputDebugger.cs

  @brief  : 输出日志
  
  @par	  : E-Mail
            liu_haixia@gowild.cn
  			
  @par	  : Copyright(C) Gowild Robotics Inc.

  @par	  : history
			2016/08/30 Ver 0.00 Created by Liuhaixia
//-----------------------------------------------------------------------------*/
using UnityEngine;

public class OutputDebugger
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Singleton

    public static readonly OutputDebugger Inst = new OutputDebugger();

    private OutputDebugger() { }

    #endregion
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public static bool Enable = false;

    private string savePath, currentInfo;

    private string[] javaAppInfos = new string[] { };

    Thread textWriteThread;
    StreamWriter streamWriter;

    // Use this for initialization
    public void Init()
    {
        if (Enable)
        {
            currentInfo = "";
            savePath = Application.persistentDataPath + "/Output/Log_" + System.DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

            textWriteThread = new Thread(new ThreadStart(CheckExistText));
            textWriteThread.Start(); 
        }
    }

    /// <summary>
    /// 检测是否存在文件
    /// </summary>
    void CheckExistText()
    {
        if (!File.Exists(savePath))
        {
            streamWriter = File.CreateText(savePath);
            string tempString = "--------------------Create Line--------------------";
            tempString += "Create Time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n";
            streamWriter.Write(tempString);
            streamWriter.Close();
            textWriteThread.Abort();
        }
        else
        {
            string tempString = "\n--------------------Update Line--------------------";
            tempString += "Update Time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n";
            Write(tempString);
        }
    }

    /// <summary>
    /// 写入日志
    /// </summary>
    public void Write(string info)
    {
        if (Enable)
        {
            currentInfo = info;

            textWriteThread = new Thread(new ThreadStart(UpdateText));
            textWriteThread.Start();   
        }
    }

    /// <summary>
    /// 更新输出信息
    /// </summary>
    void UpdateText()
    {
        streamWriter = File.AppendText(savePath);
        streamWriter.WriteLine(currentInfo);
        streamWriter.Close();

        textWriteThread.Abort();
    }

}
