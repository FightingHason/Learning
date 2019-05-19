/**----------------------------------------------------------------------------//
  @file   : MyEditorDrawGizmo.cs

  @brief  : 编辑器模式下在Scene中显示信息
  
  @par	  : E-Mail
            609043941@qq.com
  			
  @par	  : Copyright(C) Ben Inc.

  @par	  : history
			2017/01/07 Ver 0.00 Created by Liuhaixia
//-----------------------------------------------------------------------------*/
using UnityEditor;
using UnityEngine;

// 请继承Editor
public class MyEditorDrawGizmo : Editor {

    [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.InSelectionHierarchy)]
    //[DrawGizmo(GizmoType.SelectedOrChild | GizmoType.NotSelected)]
    static void DrawGameObjectName(Transform transform, GizmoType gizmoType)
    {
        Handles.Label(transform.position, transform.gameObject.name);        
    }
}
