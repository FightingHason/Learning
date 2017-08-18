/**----------------------------------------------------------------------------//
  @file   : ExportAssetBundles.cs

  @brief  : 打包AssetBundles
  
  @par	  : E-Mail
            609043941@qq.com
  			
  @par	  : Copyright(C) Ben Technology Inc.

  @par	  : history
			2017/08/17 Ver 0.00 Created by Liuhaixia
//-----------------------------------------------------------------------------*/
using LitJson;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class ExportAssetBundles {

    // 黑名单后缀名列表，遍历文件夹时忽略
    static readonly string[] SKIP_EXTENSION_ALL = { ".cs", ".meta" };
    // 白名单关键字列表，遍历文件夹时不能忽略忽略   
	static readonly string[] NO_SKIP_DIRS_ALL = { "Dependencies" };

    static readonly string ASSET_BUNDLE_ROOT_PATH = Application.dataPath.Replace("Assets", "") + "robot/AssetBundles";

	[MenuItem("Assets/UncompressedAssetBundle/Rename Dependencies")]
	static void RenameUncompressedAllDependencies()
	{
		string[] assetsAllPath = Directory.GetFiles(Application.dataPath,"*.*",SearchOption.AllDirectories);
		foreach(string path in assetsAllPath)
		{
			if (IsBlockedByExtension (path))
				continue;
			if (Directory.Exists (path))
				continue;

			string tempPath = path.Replace ("\\", "/").Replace(Application.dataPath.StartToLastSlash()+"/", "");
			AssetImporter importer = AssetImporter.GetAtPath(tempPath);
			if (importer != null){
				if (IsBlockedByWhiteList (tempPath)) {
					string assetBundlePath = tempPath.Substring("/Prefabs/Dependencies/");
					importer.assetBundleName = assetBundlePath.StartToLastPoint().ToLower();
					importer.assetBundleVariant = "";
				} else {
					if (importer.assetBundleName != null || importer.assetBundleName!="")
						importer.assetBundleName = "";
				}
			}
		}

		AssetDatabase.RemoveUnusedAssetBundleNames();
		AssetDatabase.Refresh();
	}

    [MenuItem("Assets/UncompressedAssetBundle/Build Dependencies")]
    static void BuildUncompressedAllDependencies()
    {
        BuildPipeline.BuildAssetBundles(getDirAbsolutePath("/Dependencies"), BuildAssetBundleOptions.UncompressedAssetBundle, EditorUserBuildSettings.activeBuildTarget);
    }

	[MenuItem("Assets/UncompressedAssetBundle/Build Independencies")]
    static void BuildUncompressedAllIddependencies()
    {
        string independPath = getDirAbsolutePath("/Independencies");
        foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
        {
			string assetPath = AssetDatabase.GetAssetPath(o);
			if (Directory.Exists (assetPath))
				continue;
			if (!assetPath.Contains("/Prefabs/Independencies/"))
				continue;

            string name = assetPath.Substring("Independencies/").StartToLastPoint().ToLower();

			Debug.Log("******* Creating path: " + assetPath + "\n assetbundles for: " + name);

            AssetBundleBuild[] smrABBuild = new AssetBundleBuild[1];
            smrABBuild[0].assetBundleName = name;
            smrABBuild[0].assetNames = new string[] { assetPath };

            BuildPipeline.BuildAssetBundles(independPath, smrABBuild, BuildAssetBundleOptions.UncompressedAssetBundle, EditorUserBuildSettings.activeBuildTarget);
        }

		deleteUnusedResources(independPath);
    }

    static void deleteUnusedResources(string absolutePath)
    {
		string[] existManifests = Directory.GetFiles(absolutePath, "*.manifest", SearchOption.AllDirectories);
        foreach (string manifest in existManifests)
        {
            Debug.Log("**Delete Manifest: " + manifest);
            File.Delete(manifest);
        }

		string[] assetbundles = Directory.GetFiles(absolutePath, "*.*", SearchOption.AllDirectories);
        foreach (string bundle in assetbundles)
        {
            FileInfo bundleFI = new FileInfo(bundle);
            string bundleName = bundleFI.Name.Replace(".assetbundle", "");

            //if (materialName != bundleName) continue;
            if (bundleName.Contains(absolutePath.LastSlashToPoint()))
            {
                Debug.Log("**Delete Folder: " + bundle);
                File.Delete(bundle);
                break;
            }
        }
    }

    /// <summary>  
    /// 判断是否被黑名单后缀名列表过滤  
    /// </summary>  
    static bool IsBlockedByExtension(string filePath)
    {
        string extension = Path.GetExtension(filePath);
        foreach (string ext in SKIP_EXTENSION_ALL)
        {
            if (string.Compare(extension.ToLower(), ext, true) == 0)
                return true;
        }
        return false;
    }

    /// <summary>  
    /// 判断是否被白名单文件夹列表
    /// </summary>  
    static bool IsBlockedByWhiteList(string filePath)
    {
        string[] folderNames = filePath.Split('/');
        foreach (string path in NO_SKIP_DIRS_ALL)
        {
            for (int i = 0; i < folderNames.Length; ++i)
            {
                if (string.Compare(path, folderNames[i], true) == 0)
                    return true;
            }
        }
        return false;
    } 

	static string getDirAbsolutePath(string relativePath)
	{
		string absolutePath = ASSET_BUNDLE_ROOT_PATH + relativePath;
		if (!Directory.Exists(absolutePath))
		{
			Directory.CreateDirectory(absolutePath);
		}

		return absolutePath;
	}

	/// <summary>
	/// Gets the prefab.
	/// </summary>
	/// <returns>The prefab.</returns>
	/// <param name="go">Go.</param>
	/// <param name="name">Name.</param>
    static Object GetPrefab(GameObject go, string name)
    {
        Object tempPrefab = PrefabUtility.CreateEmptyPrefab("Assets/" + name + ".prefab");
        tempPrefab = PrefabUtility.ReplacePrefab(go, tempPrefab);
        Object.DestroyImmediate(go);
        return tempPrefab;
    }

}
