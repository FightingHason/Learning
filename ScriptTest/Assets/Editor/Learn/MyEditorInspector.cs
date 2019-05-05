/**----------------------------------------------------------------------------//
  @file   : MyEditorInspector.cs

  @brief  : 编辑器模式下在Inspector中显示信息
  
  @par	  : E-Mail
            609043941@qq.com
  			
  @par	  : Copyright(C) Ben Inc.

  @par	  : history
			2017/01/07 Ver 0.00 Created by Liuhaixia
//-----------------------------------------------------------------------------*/
using UnityEditor;
using UnityEngine;

// 自定义Inspector脚本
[CustomEditor(typeof(TestInspector))]
// 请继承Editor
public class MyEditorInspector : Editor
{
    /// <summary>
    /// Example1
    /// </summary>
    //public override void OnInspectorGUI()
    //{
    //    TestInspector thisCompent = (TestInspector)target;
    //    thisCompent.mRectValue = EditorGUILayout.RectField("Rect", thisCompent.mRectValue);
    //    thisCompent.mTexture = EditorGUILayout.ObjectField("Texture", thisCompent.mTexture, typeof(Texture), true) as Texture;
    //}

    /// <summary>
    /// Example2
    /// </summary>
    //TestInspector testInspector;
    //public override void OnInspectorGUI()
    //{
    //    testInspector = target as TestInspector;
    //    int width = EditorGUILayout.IntField("Width", testInspector.mWidth);
    //    if (testInspector.mWidth != width)
    //    {
    //        testInspector.mWidth = width;   
    //    }
    //    base.DrawDefaultInspector();
    //}

}

//public class CameraExtension : Editor
//{
//    /// <summary>
//    /// Example3
//    /// </summary>
//    public override void OnInspectorGUI()
//    {
//        base.DrawDefaultInspector();
//        if (GUILayout.Button("刘大侠Ben"))
//        {
//            Debug.Log("hello");
//        }
//    }
//}

//[CanEditMultipleObjects()]
//[CustomEditor(typeof(Camera), true)]
//public class CustomExtension : CameraExtension
//{

//}
