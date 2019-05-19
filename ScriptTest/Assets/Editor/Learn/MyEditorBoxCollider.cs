/**----------------------------------------------------------------------------//
  @file   : CalculateBoxMaxCollider.cs

  @brief  : 自动计算所有子对象包围盒
  
  @par	  : E-Mail
            609043941@qq.com
  			
  @par	  : Copyright(C) Ben Inc.

  @par	  : history
			2017/01/09 Ver 0.00 Created by Liuhaixia
//-----------------------------------------------------------------------------*/
using UnityEngine;
using UnityEditor;

public class MyEditorBoxCollider : Editor {

    [MenuItem("MyEditor/CalAllBoxMaxCollider")]
    static void CalculateAllBoxMaxCollider()
    {
        Transform parent = Selection.activeGameObject.transform;
        Vector3 position = parent.position;
        Quaternion rotation = parent.rotation;
        Vector3 scale = parent.localScale;
        parent.position = Vector3.zero;
        parent.rotation = Quaternion.Euler(Vector3.zero);
        parent.localScale = Vector3.one;

        // 删除原来的collider
        Collider[] colliders = parent.GetComponentsInChildren<Collider>();
        foreach (Collider child in colliders)
        {
            DestroyImmediate(child);
        }

        Vector3 center = Vector3.zero;
        Renderer[] renders = parent.GetComponentsInChildren<Renderer>();
        foreach (Renderer child in renders)
        {
            center += child.bounds.center;
        }

        center /= parent.GetComponentsInChildren<Transform>().Length;
        Bounds bounds = new Bounds(center, Vector3.zero);
        foreach (Renderer child in renders)
        {
            bounds.Encapsulate(child.bounds);
        }

        BoxCollider boxCollider = parent.gameObject.AddComponent<BoxCollider>();
        boxCollider.center = bounds.center - parent.position;
        boxCollider.size = bounds.size;

        parent.position = position;
        parent.rotation = rotation;
        parent.localScale = scale;
    }

    [MenuItem("MyEditor/ResetModelPostion")]
    static void ResetModelPostion()
    {
        Transform parent = Selection.activeGameObject.transform;
        Vector3 position = parent.position;
        Quaternion rotation = parent.rotation;
        Vector3 scale = parent.localScale;
        parent.position = Vector3.zero;
        parent.rotation = Quaternion.Euler(Vector3.zero);
        parent.localScale = Vector3.one;

        Vector3 center = Vector3.zero;
        Renderer[] renders = parent.GetComponentsInChildren<Renderer>();
        foreach (Renderer child in renders)
        {
            center += child.bounds.center;
        }

        Debug.Log("Center: "+center);

        center /= parent.GetComponentsInChildren<Transform>().Length;
        Debug.Log("Center: " + center);

        Bounds bounds = new Bounds(center, Vector3.zero);
        foreach (Renderer child in renders)
        {
            bounds.Encapsulate(child.bounds);
        }

        parent.position = position;
        parent.rotation = rotation;
        parent.localScale = scale;

        foreach (Transform t in parent)
        {
            t.position = t.position - bounds.center;
        }
        parent.transform.position = bounds.center + parent.position;
    }

}
                    