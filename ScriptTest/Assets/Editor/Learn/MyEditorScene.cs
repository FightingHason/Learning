/**----------------------------------------------------------------------------//
  @file   : MyEditorScene.cs

  @brief  : 编辑器模式下在Scene中显示信息
  
  @par	  : E-Mail
            609043941@qq.com
  			
  @par	  : Copyright(C) Ben Inc.

  @par	  : history
			2017/01/07 Ver 0.00 Created by Liuhaixia
//-----------------------------------------------------------------------------*/
using UnityEditor;
using UnityEngine;

// 自定义TestScene脚本（！！！如果没有这个在scene中无法生效）
[CustomEditor(typeof(TestScene))]
// 请继承Editor
public class MyEditorScene : Editor {

    void OnSceneGUI()
    {
        // 得到Test脚本
        TestScene test = (TestScene)target;

        // 绘制文本框
        Handles.Label(test.transform.position + Vector3.up*2, test.transform.name + " : "+test.transform.position.ToString());

        // 开始绘制GUI
        Handles.BeginGUI();

        // 规定GUI显示区域
        GUILayout.BeginArea(new Rect(100, 100, 100, 100));

        // GUI绘制一个按钮
        if (GUILayout.Button("这是一个按钮！"))
        {
            Debug.Log("test");
        }

        // GUI绘制文本框
        GUILayout.Label("我在编辑Scene视图");

        GUILayout.EndArea();

        Handles.EndGUI();
    }

}
