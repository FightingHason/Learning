/**----------------------------------------------------------------------------//
  @file   : ExportAssetBundles.cs

  @brief  : 打包AssetBundles
  
  @par	  : E-Mail
            609043941@qq.com
  			
  @par	  : Copyright(C) Ben Technology Inc.

  @par	  : history
			2017/12/21 Ver 0.00 Created by Liuhaixia
//-----------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using LitJson;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ben {
    public class ExportAssetBundles {

        const String TB_AUDIO = "AudioTable";
        const String TB_ACTION = "ActionTable";
        const String TB_EFFECT = "EffectTable";

        static String AssetEditorTempletePath = "Editor/Templete/";

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Animation

        static Int32 AnimationTempleteCount = 28;
        static Int32 AnimationStartID = 485;
        static String AnimatorName = "Holoera";
        static String AnimationTempleteName = "AnimationTemplete";
        static String AssetAnimationPath = "Assets/Project/Raw/Animation/";
        static String AssetAnimationClipPath = AssetAnimationPath + "Clip/";
        static String AssetAnimationAnimatorPath = AssetAnimationPath + "Controller/";

        [MenuItem ("Tools/Animation/GetAniClipFromFBX")]
        static void GetAnimFromFBX () {
            String[] fileArray = Directory.GetFiles (AssetAnimationClipPath);
            foreach (String info in fileArray) {
                File.Delete (info);
            }

            AssetDatabase.Refresh ();

            String[] allPath = Directory.GetFiles (AssetAnimationPath, "*.FBX", SearchOption.AllDirectories);

            Int32 count = 0;
            foreach (String path in allPath) {
                AnimationClip originalClip = AssetDatabase.LoadAssetAtPath (path, typeof (AnimationClip)) as AnimationClip;
                if (originalClip != null) {
                    AnimationClip newClip = new AnimationClip ();
                    EditorUtility.CopySerialized (originalClip, newClip);
                    AssetDatabase.CreateAsset (newClip, AssetAnimationClipPath + originalClip.name.Substring (1) + ".anim");

                    ++count;
                } else {
                    Debug.Log ("Animation FBX: " + path + " is not exist AnimmationClip!");
                }

                EditorUtility.DisplayProgressBar ("Get Anim From FBX", "please wait...", count / allPath.Length);
            }

            EditorUtility.ClearProgressBar ();

            AssetDatabase.Refresh ();

            Debug.Log ("Get Animation Clip From FBX Success! Total Animation: " + count);

            // 创建动画表
            RebuildAnimationTable ();

            // 创建Animator
            //CreateAnimator ();
        }

        [MenuItem ("Tools/Animation/Create Animator")]
        static void CreateAnimator () {

            String[] fileArray = Directory.GetFiles (AssetAnimationAnimatorPath);
            foreach (String info in fileArray) {
                File.Delete (info);
            }

            AssetDatabase.Refresh ();

            AnimatorController animatorCon = AnimatorController.CreateAnimatorControllerAtPath (AssetAnimationAnimatorPath + AnimatorName + ".controller");
            AnimatorControllerLayer layer = animatorCon.layers[0];
            AnimatorStateMachine sm = layer.stateMachine;

            String[] allPath = Directory.GetFiles (AssetAnimationClipPath, "*.anim", SearchOption.AllDirectories);

            Int32 count = 0;
            foreach (String path in allPath) {
                AnimationClip newClip = AssetDatabase.LoadAssetAtPath (path, typeof (AnimationClip)) as AnimationClip;
                if (newClip != null) {
                    AnimatorState state = sm.AddState (newClip.name);
                    state.motion = newClip;
                    ++count;
                }

                EditorUtility.DisplayProgressBar ("Create Animator", "Add AnimationClip To Animator, please wait...", count / allPath.Length);
            }

            EditorUtility.ClearProgressBar ();

            AssetDatabase.Refresh ();

            Debug.Log ("Animator Success! Total Animation: " + count);
        }

        [MenuItem ("Tools/Animation/Rebuild Animation Table")]
        public static void RebuildAnimationTable () {
            String[] allFiles = Directory.GetFiles (AssetAnimationClipPath, "*.anim", SearchOption.AllDirectories);

            List<GAnimClipInfo> allAnimationList = new List<GAnimClipInfo> ();

            Int32 count = 0, totalCount = allFiles.Length;
            foreach (String path in allFiles) {
                AnimationClip ac = AssetDatabase.LoadAssetAtPath<AnimationClip> (path.StandardSplitForPath ());
                if (ac != null) {
                    GAnimClipInfo animInfo = new GAnimClipInfo ();

                    Int32 index = 0;
                    //SN
                    animInfo.sn = ac.name.Substring (index, ac.name.Length - index);
                    //ActionType
                    index += 5;
                    animInfo.actionType = ac.name.Substring (index, 3);
                    //Length
                    animInfo.length = System.Convert.ToSingle (ac.length.ToString ("F2"));
                    //Detail
                    animInfo.detail = "";

                    allAnimationList.Add (animInfo);
                } else {
                    Debug.LogError ("Animation Error! Path: " + path);
                }

                EditorUtility.DisplayProgressBar ("Create Animation Config", "Get Animation Info, please wait...", count / totalCount);
            }

            EditorUtility.ClearProgressBar ();

            //Check Templete Table
            JsonData templeteJson;
            ParseAnimationTemplete (allAnimationList, out templeteJson);
            if (templeteJson != null && templeteJson.IsArray) {
                for (Int32 i = 0; i < templeteJson.Count; ++i) {
                    String actionType = templeteJson[i].GetString (CamelNameConst.ACTION_TYPE);
                    for (Int32 k = 0; k < allAnimationList.Count; ++k) {
                        if (actionType == allAnimationList[k].actionType) {
                            GAnimClipInfo temp = allAnimationList[k];
                            String detail = templeteJson[i].GetString (CamelNameConst.DETAIL);
                            Debug.Log (detail);
                            temp.detail = detail;
                            allAnimationList[k] = temp;
                            break;
                        }
                    }

                }
            }

            //Action Table
            String targetPath = PathUtils.GetTablePath (TB_ACTION);
            if (File.Exists (targetPath)) {
                File.Delete (targetPath);
            }

            JsonData totalJson = new JsonData ();
            foreach (var animItem in allAnimationList) {
                JsonData itemJson = new JsonData ();
                itemJson[CamelNameConst.ACTION_TYPE] = animItem.actionType;
                itemJson[CamelNameConst.SN] = animItem.sn;
                itemJson[CamelNameConst.DETAIL] = animItem.detail;
                itemJson[CamelNameConst.LENGTH] = animItem.length.ToString ();

                totalJson.Add (itemJson);
            }

            Debug.Log (totalJson.ToJson ());

            FileUtils.WriteTextByStream (targetPath, totalJson.ToJson ());

            EditorUtility.ClearProgressBar ();

            Debug.Log ("All Animation Count: " + allAnimationList.Count + " || Table Path: " + targetPath);

            AssetDatabase.Refresh ();
        }

        [MenuItem ("Tools/Animation/CheckAnimationTemplete")]
        static void CheckAnimationTemplete () {
            JsonData totalJson;
            ParseAnimationTemplete (null, out totalJson);
        }

        static void ParseAnimationTemplete (List<GAnimClipInfo> animList, out JsonData totalJson) {
            String templetePath = Application.dataPath + "/" + AssetEditorTempletePath + AnimationTempleteName + GlobalConst.TB_TYPE_JSON;

            totalJson = new JsonData ();
            Boolean isLoad = false;
            if (File.Exists (templetePath)) {
                String jsonStr = FileUtils.ReadTextByStream (templetePath);
                if (false == String.IsNullOrEmpty (jsonStr) && jsonStr != "\r" && jsonStr != "\n" && jsonStr != "\r\n") {
                    try {
                        totalJson = JsonMapper.ToObject<JsonData> (jsonStr);
                        isLoad = true;
                    } catch (System.Exception ex) {
                        File.Delete (templetePath);
                        Debug.LogError ("本地信息表转Json格式错误！Path: " + templetePath + " || Exception: " + ex.Message);
                        totalJson = null;
                        isLoad = false;
                    }
                } else {
                    Debug.LogError ("本地信息表为空！Path: " + templetePath);
                    totalJson = null;
                    isLoad = false;
                }
            } else {
                isLoad = false;
            }

            if (false == isLoad) {
                totalJson = new JsonData ();
                if (animList.Count > 0) {
                    for (Int32 i = 0; i < animList.Count; ++i) {
                        JsonData itemJson = new JsonData ();
                        itemJson[CamelNameConst.ACTION_TYPE] = animList[i].actionType;
                        itemJson[CamelNameConst.DETAIL] = "";

                        totalJson.Add (itemJson);
                    }
                } else {
                    for (Int32 i = 0; i < AnimationTempleteCount; ++i) {
                        JsonData itemJson = new JsonData ();
                        itemJson[CamelNameConst.ACTION_TYPE] = AnimationStartID + i;
                        itemJson[CamelNameConst.DETAIL] = "";

                        totalJson.Add (itemJson);
                    }
                }

                FileUtils.WriteTextByStream (templetePath, totalJson.ToJson ());

                AssetDatabase.Refresh ();
            }
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Aduio & Effect

        [MenuItem ("Tools/Get Audio Table")]
        static void GetAudioTable () {
            String resPath = PathUtils.GetCachePath (GlobalConst.DIR_AUDIO);
            String[] allFiles = Directory.GetFiles (resPath, "*" + GlobalConst.TB_TYPE_WAV, SearchOption.AllDirectories);

            JsonData totalJson = new JsonData ();
            foreach (var item in allFiles) {
                JsonData itemJson = new JsonData ();
                itemJson[CamelNameConst.MUSIC_ID] = item.LastSlashToPoint ();

                totalJson.Add (itemJson);
            }

            String targetPath = PathUtils.GetTablePath (TB_AUDIO);
            if (File.Exists (targetPath)) {
                File.Delete (targetPath);
            }

            FileUtils.WriteTextByStream (targetPath, totalJson.ToJson ());
        }

        [MenuItem ("Tools/Get Effect Table")]
        static void GetEffectTable () {
            String resPath = PathUtils.GetCachePath (GlobalConst.DIR_EFFECT);
            String[] allFiles = Directory.GetFiles (resPath);

            JsonData totalJson = new JsonData ();
            foreach (var item in allFiles) {
                JsonData itemJson = new JsonData ();
                itemJson[CamelNameConst.EFFECT_ID] = item.LastSlashToPoint ();

                totalJson.Add (itemJson);
            }

            String targetPath = PathUtils.GetTablePath (TB_EFFECT);
            if (File.Exists (targetPath)) {
                File.Delete (targetPath);
            }

            FileUtils.WriteTextByStream (targetPath, totalJson.ToJson ());
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region AssetBundle

        static readonly String ASSET_BUNDLE_ROOT_PATH = Application.dataPath.Replace ("Assets", "") + "robot/AssetBundles";

        [MenuItem ("Assets/Ben/Build Independencies Selection")]
        static void BuildUncompressedAllIddependenciesSelection () {
            String independPath = getDirAbsolutePath ("/Independencies");
            foreach (Object o in Selection.GetFiltered (typeof (Object), SelectionMode.DeepAssets)) {
                String assetPath = AssetDatabase.GetAssetPath (o);
                if (Directory.Exists (assetPath))
                    continue;

                String name = assetPath.Substring ("Raw/").StartToLastPoint ().ToLower ();

                Debug.Log ("******* Creating path: " + assetPath + "\n assetbundles for: " + name);

                AssetBundleBuild[] smrABBuild = new AssetBundleBuild[1];
                smrABBuild[0].assetBundleName = name;
                smrABBuild[0].assetNames = new String[] { assetPath };

                BuildPipeline.BuildAssetBundles (independPath, smrABBuild, BuildAssetBundleOptions.UncompressedAssetBundle, EditorUserBuildSettings.activeBuildTarget);
            }

            deleteUnusedResources (independPath);
        }

        static void deleteUnusedResources (String absolutePath) {
            String[] existManifests = Directory.GetFiles (absolutePath, "*.manifest", SearchOption.AllDirectories);
            foreach (String manifest in existManifests) {
                Debug.Log ("**Delete Manifest: " + manifest);
                File.Delete (manifest);
            }

            String[] assetbundles = Directory.GetFiles (absolutePath, "*.*", SearchOption.AllDirectories);
            foreach (String bundle in assetbundles) {
                FileInfo bundleFI = new FileInfo (bundle);
                String bundleName = bundleFI.Name.Replace (".assetbundle", "");

                //if (materialName != bundleName) continue;
                if (bundleName.Contains (absolutePath.LastSlashToPoint ())) {
                    Debug.Log ("**Delete Folder: " + bundle);
                    File.Delete (bundle);
                    break;
                }
            }
        }

        static String getDirAbsolutePath (String relativePath) {
            String absolutePath = ASSET_BUNDLE_ROOT_PATH + relativePath;
            if (!Directory.Exists (absolutePath)) {
                Directory.CreateDirectory (absolutePath);
            }

            return absolutePath;
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Animation

        [MenuItem ("Tools/Animation/Move Prop To Skin %l")]
        static void MoveAnimationPropBone () {
            MoveAnimationPropBone ((GameObject) Selection.activeObject);
        }

        static void MoveAnimationPropBone (GameObject selectObj) {
            //GameObject selectObj = (GameObject)Selection.activeObject;
            Transform selectParent = selectObj.transform.parent;
            Transform skinTrans = selectParent.parent.FindChild ("skin");
            Transform moXingTrans = skinTrans.FindChild ("Prop");
            if (moXingTrans == null) {
                GameObject propObject = new GameObject ("Prop");
                propObject.transform.parent = skinTrans;
                propObject.transform.localPosition = Vector3.zero;
                propObject.transform.localScale = Vector3.one;
                propObject.transform.rotation = Quaternion.identity;

                moXingTrans = propObject.transform;
            }

            // 所有骨骼对象
            List<Transform> smrBones = new List<Transform> ();

            // 所有子节点包含SkinnedMeshRenderer
            foreach (SkinnedMeshRenderer smr in selectObj.GetComponentsInChildren<SkinnedMeshRenderer> ())
                smrBones.AddRange (smr.bones);
            smrBones = smrBones.Distinct ().ToList ();

            // 人体骨骼外，单独骨骼
            List<Transform> aloneParent = new List<Transform> ();
            // 人体骨骼对象所对应的父节点路劲
            Dictionary<Transform, String> bodyBoneParentPath = new Dictionary<Transform, String> ();

            // 子节点序号
            List<Int32> childIndexList = new List<Int32> ();

            // 查找骨骼是否存在父子节点关系
            for (Int32 i = 0; i < smrBones.Count; ++i) {
                for (Int32 j = 0; j < smrBones.Count; ++j) {
                    if (i != j) {
                        if (smrBones[i] == smrBones[j].parent) {
                            childIndexList.Add (j);
                        }
                    }
                }
            }

            childIndexList.Sort ();

            // 删除父节点下的子节点
            for (Int32 i = childIndexList.Count - 1; i >= 0; --i) {
                smrBones.RemoveAt (childIndexList[i]);
                //Debug.Log("Action Id: " + selectParent.name.Substring(9, 7) + " || Remove Index: " + childIndexList[i]);
            }

            // 查找人体骨骼下的对应节点
            foreach (Transform bone in smrBones) {
                Boolean bodyBip = false, aloneBone = false;
                String parentPath = null;
                Transform temp = bone;

                while (true) {
                    if (temp.name.Equals ("Bip001")) {
                        bodyBip = true;
                        break;
                    }
                    // 外骨骼唯一父节点
                    if (temp.parent == selectParent) {
                        aloneBone = true;
                        break;
                    }
                    if (temp.parent == null) {
                        Debug.Log ("Parent is null!");
                        break;
                    }

                    if (parentPath == null)
                        parentPath = temp.parent.name;
                    else
                        parentPath = temp.parent.name + "/" + parentPath;

                    temp = temp.parent;
                }
                if (bodyBip) {
                    if (!bodyBoneParentPath.ContainsKey (bone))
                        bodyBoneParentPath.Add (bone, parentPath);
                } else {
                    if (aloneBone)
                        aloneParent.Add (temp);
                    else
                        Debug.Log ("Bone < " + bone.name + " > Top Parent is null!");
                }
            }

            aloneParent = aloneParent.Distinct ().ToList ();

            // 用户检测是Transform
            Transform checkTrans;
            // 单独骨骼移动到skin下面
            for (Int32 i = 0; i < aloneParent.Count; ++i) {
                //删除
                checkTrans = skinTrans.FindChild (aloneParent[i].name);
                if (checkTrans != null) {
                    Debug.Log ("Exist Old Alone Bone, Delete; " + checkTrans.name + " || " + "Action Id: " + selectParent.name.Substring (9, 7));
                    Object.DestroyImmediate (checkTrans.gameObject);
                }
                aloneParent[i].parent = skinTrans;
            }

            // 人体骨骼下物件移动到人体骨骼下面
            foreach (Transform tran in bodyBoneParentPath.Keys) {
                String targetPath = bodyBoneParentPath[tran] + "/" + tran.name;
                checkTrans = skinTrans.FindChild (targetPath);
                // if (checkTrans != null) {
                //     Debug.Log ("Exist Old Body Bone, Delete; " + targetPath);
                //     Object.DestroyImmediate (checkTrans.gameObject);
                // }
                Transform tranParent = skinTrans.FindChild (bodyBoneParentPath[tran]);
                tran.parent = tranParent;
                //Debug.Log(selectObj.name + " || BodyBone: Object: " + tran.name + " || Parent: " + tranParent + " || " + bodyBoneParentPath[tran]);
            }

            // 删除skin下面老物件(原名称)
            checkTrans = skinTrans.FindChild (selectObj.name);
            if (checkTrans != null) {
                Debug.Log ("Exist Old MoXing, Delete; " + checkTrans.name);
                Object.DestroyImmediate (checkTrans.gameObject);
            }

            // 更改名称
            selectObj.name = selectParent.name; //.Substring (9, 7);

            // 删除skin下面老物件(更改后名称)
            checkTrans = skinTrans.FindChild (selectObj.name);
            if (checkTrans != null) {
                Debug.Log ("Exist Old MoXing, Delete; " + checkTrans.name);
                Object.DestroyImmediate (checkTrans.gameObject);
            }

            // 删除skin下面老物件(原名称)
            checkTrans = moXingTrans.FindChild (selectObj.name);
            if (checkTrans != null) {
                Debug.Log ("Exist Old MoXing, Delete; " + checkTrans.name);
                Object.DestroyImmediate (checkTrans.gameObject);
            }

            // 更改名称
            selectObj.name = selectParent.name; //.Substring (9, 7);

            // 删除skin下面老物件(更改后名称)
            checkTrans = moXingTrans.FindChild (selectObj.name);
            if (checkTrans != null) {
                Debug.Log ("Exist Old MoXing, Delete; " + checkTrans.name);
                Object.DestroyImmediate (checkTrans.gameObject);
            }

            // 物件移动到skin下面
            selectObj.transform.parent = moXingTrans;
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    } //end class

} //end namespace