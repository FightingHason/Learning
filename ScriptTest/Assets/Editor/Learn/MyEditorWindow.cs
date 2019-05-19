/**----------------------------------------------------------------------------//
  @file   : MyEditorWindow.cs

  @brief  : 编辑器模式下设置Window及其监听
  
  @par	  : E-Mail
            609043941@qq.com
  			
  @par	  : Copyright(C) Ben Inc.

  @par	  : history
			2017/01/07 Ver 0.00 Created by Liuhaixia
//-----------------------------------------------------------------------------*/
using UnityEditor;
using UnityEngine;

public class MyEditorWindow : EditorWindow {

    [MenuItem("MyEditor/Window")]
    static void AddWindow()
    {
        // 创建窗口
        MyEditorWindow window = (MyEditorWindow)EditorWindow.GetWindowWithRect(typeof(MyEditorWindow), new Rect(0, 0, 500, 500), true, "window name");
        window.Show();
    }

    // 输入文字的内容
    private string text;
    // 选择贴图的对象
    private Texture texture;

    public void Awake()
    {
        texture = Resources.Load<Texture>("UI/Texture/WindowTexture1");
    }

    void OnGUI()
    {
        text = EditorGUILayout.TextField("Input", text);

        if (GUILayout.Button("Open Notification", GUILayout.Width(200)))
        {
            // 打开一个通知栏
            this.ShowNotification(new GUIContent("This is a Notification"));
        }

        if (GUILayout.Button("Close Notification", GUILayout.Width(200)))
        {
            // 关闭通知栏
            this.RemoveNotification();
        }

        // 文本框显示鼠标在窗口的位置
        EditorGUILayout.LabelField("鼠标在窗口的位置", Event.current.mousePosition.ToString());

        // 选择贴图
        texture = EditorGUILayout.ObjectField("添加贴图", texture, typeof(Texture), true) as Texture;

        if (GUILayout.Button("Close Window", GUILayout.Width(200)))
        {
            this.Close();
        }
    }

    // 更新
    void Update()
    {
        
    }

    void OnFocus()
    {
        Debug.Log("当窗口获得焦点时调用一次");
    }

    void OnLostFocus()
    {
        Debug.Log("当窗口丢失焦点时调用一次");
    }

    void OnHierarchyChange()
    {
        Debug.Log("当Hierarchy视图中的任何对象发生改变时调用一次");
    }

    void OnProjectChange()
    {
        Debug.Log("当Project视图中资源发生改变时调用一次");
    }

    void OnInspectorUpdate()
    {
        // 这里开启窗口的重绘，不然窗口信息不会刷新
        this.Repaint();
    }

    void OnSelectionChange()
    {
        // 当窗口出去开启状态，并且在Hierarchy视图中选择某对象时调用
        foreach (Transform t in Selection.transforms)
        {
            // 有可能是多选，这里开启一个循环打印选中游戏对象的名称
            Debug.Log("OnSelectChange: " + t.name);
        }
    }

    void OnDestroy()
    {
        Debug.Log("当窗口关闭时调用");
    }
}
