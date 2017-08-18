/**----------------------------------------------------------------------------//
  @file   : AssetBundlesTest.cs

  @brief  : AssetBundle Demo
  
  @par	  : E-Mail
            609043941@qq.com
  			
  @par	  : Copyright(C) Ben Technology Inc.

  @par	  : history
			2017/08/17 Ver 0.00 Created by Liuhaixia
//-----------------------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class AssetBundlesTest : MonoBehaviour {

    const string ASSET_BUNDLE_MANIFEST = "AssetBundleManifest";
	const string DEPENDENCIES = "Dependencies";
	const string INDEPENDENCIES = "Independencies";

    string _rootPath = null;
    StringBuilder _strBuilder = null;

    AssetBundleManifest _abManifest = null;

    Dictionary<string, Object> _dependObjList = new Dictionary<string, Object>();

	GameObject _characterSkin = null;
	Animator _characterAnimator = null;

    void Awake()
    {
        initRootPath();
    }

	// Use this for initialization
	void Start () {
		// load character
		loadDenpendeciesAssetBundle();

		// load animator
		if (_characterSkin != null) {
			_characterAnimator = _characterSkin.GetComponent<Animator> ();
			if (_characterAnimator!=null)
				_characterAnimator.runtimeAnimatorController = loadIndenpendeciesAssetBundle("animation/controller/autoplay") as RuntimeAnimatorController;
		}

		// load camera
		GameObject objCamera = Instantiate(loadIndenpendeciesAssetBundle("camera/camera_all") as GameObject);
		objCamera.name = objCamera.name.Replace("(Clone)","");
	}

	/// <summary>
	/// Loads the denpendecies asset bundle.
	/// </summary>
    void loadDenpendeciesAssetBundle()
    {
        if (_abManifest == null)
        {
			string manifestPath = getAssetBundlePath(DEPENDENCIES, true);
            if (!File.Exists(manifestPath))
            {
                Debug.LogError("Not exist AssetBundleManifest! Path: " + manifestPath);
                return;
            }
            else
            {
                AssetBundle manifestBundle = AssetBundle.CreateFromFile(manifestPath);
                _abManifest = (AssetBundleManifest)manifestBundle.LoadAsset(ASSET_BUNDLE_MANIFEST);
                manifestBundle.Unload(false);
            }
        }

        if (_abManifest != null)
        {
            string abFileName = "character/skin_gongzhuzhuang_543";

            //Load Dependencies
            string[] dependAssetBundles = _abManifest.GetAllDependencies(abFileName);
            if (dependAssetBundles.Length > 0)
            {
                string abDependPath = "";
                for (int i = 0; i < dependAssetBundles.Length; ++i)
                {
					abDependPath = getAssetBundlePath(dependAssetBundles[i], true);
                    if (false == _dependObjList.ContainsKey(abDependPath))
                    {
                        AssetBundle abDepend = AssetBundle.CreateFromFile(abDependPath);
                        Object dependObj = abDepend.LoadAsset(abDependPath.LastSlashToPoint());
                        if (dependObj != null)
                        {
                            _dependObjList.Add(abDependPath, dependObj);
                        }
                        else
                        {
                            Debug.LogError("AssetBundle is null!");
                        }
						//can't unload, because target assetbundle will lost liner
                        //assetbundle.Unload(false);
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            //Load Target
            float time = Time.realtimeSinceStartup;
			AssetBundle targetAssetBundle = AssetBundle.CreateFromFile(getAssetBundlePath(abFileName, true));
			Object obj = targetAssetBundle.LoadAsset(abFileName.LastSlashToPoint());
            Debug.Log("11111Cost Time: " + (Time.realtimeSinceStartup - time));
            if (obj != null)
            {
				_characterSkin = Instantiate(obj) as GameObject;

				_characterSkin.name = obj.name;

				_characterSkin.transform.parent = this.transform;
				_characterSkin.transform.localPosition = Vector3.zero;
				_characterSkin.transform.localEulerAngles = Vector3.zero;
				_characterSkin.transform.localScale = Vector3.one;

				_characterSkin.AddComponent<SetLostShader>();
            }
            targetAssetBundle.Unload(false);

            Debug.Log("22222Cost Time: " + (Time.realtimeSinceStartup - time));
        }
        else
        {
            Debug.LogError("AssetBundleManifest is null!");
        }
    }

	/// <summary>
	/// Loads the indenpendecies asset bundle.
	/// </summary>
	/// <returns>The indenpendecies asset bundle.</returns>
	/// <param name="relativePath">Relative path.</param>
	Object loadIndenpendeciesAssetBundle(string relativePath)
	{
		//Load Target
		float time = Time.realtimeSinceStartup;
		AssetBundle targetAssetBundle = AssetBundle.CreateFromFile(getAssetBundlePath(relativePath, false));
		Object obj = targetAssetBundle.LoadAsset(relativePath.LastSlashToPoint());
		Debug.Log("11111Cost Time: " + (Time.realtimeSinceStartup - time));
		if (obj != null) {
			targetAssetBundle.Unload(false);
			return obj;
		} else {
			targetAssetBundle.Unload(true);
			return null;
		}
	}

	/// <summary>
	/// Inits the root path.
	/// </summary>
	void initRootPath()
	{
		if (_rootPath == null || _strBuilder == null)
		{
			_strBuilder = new StringBuilder(Application.dataPath.StartToLastSlash());
			_strBuilder.Append("/robot/AssetBundles/");

			_rootPath = _strBuilder.ToString();
		}
	}

	/// <summary>
	/// Gets the asset bundle path.
	/// </summary>
	/// <returns>The asset bundle path.</returns>
	/// <param name="relativePath">Relative path.</param>
	/// <param name="bDepend">If set to <c>true</c> b depend.</param>
	string getAssetBundlePath(string relativePath, bool bDepend=true)
	{
		initRootPath();

		_strBuilder.Remove(0, _strBuilder.Length);

		if (bDepend)
			_strBuilder.Append(Path.Combine(_rootPath, DEPENDENCIES));
		else
			_strBuilder.Append(Path.Combine(_rootPath, INDEPENDENCIES));
		
		_strBuilder.Append("/");
		_strBuilder.Append(relativePath);
		_strBuilder.Replace("\\","/");

		return _strBuilder.ToString();
	}
}
