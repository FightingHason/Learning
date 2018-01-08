//*************************************************
//File:		PathUtils.cs
//
//Brief:    Path Utils
//
//Author:   Liuhaixia
//
//E-Mail:   609043941@qq.com
//
//History:  2017/04/12 Created by Liuhaixia
//*************************************************
using System;
using UnityEngine;

namespace Ben {
    public class PathUtils {

        static String cacheRootPath = "";

        /// <summary>
        /// Get Cache Root Path
        /// </summary>
        public static String GetCachePath (String filePath) {
            if (cacheRootPath == "") {
                cacheRootPath = Application.dataPath.StartToLastSlash () + CharConst.BACK_SLASH + GlobalConst.DIR_ROOT;
            }
            return cacheRootPath + filePath;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region UI Path

        static String uiPath = "";

        /// <summary>
        /// Get UI Path
        /// </summary>
        public static String GetUIPath () {
            if (uiPath == "") {
                uiPath = GetCachePath (GlobalConst.DIR_UI);
            }
            return uiPath;
        }

        /// <summary>
        /// Get UI Path
        /// </summary>
        public static String GetUIPath (String fileName) {
            return GetUIPath () + fileName;
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Table Path

        static String tablePath = "";

        /// <summary>
        /// Get Table Path
        /// </summary>
        public static String GetTablePath () {
            if (tablePath == "") {
                tablePath = GetCachePath (GlobalConst.DIR_TABLE);
            }
            return tablePath;
        }

        /// <summary>
        /// Get Table Path
        /// </summary>
        public static String GetTablePath (String fileName, String fileType = GlobalConst.TB_TYPE_JSON) {
            return GetTablePath () + fileName + fileType;
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Audio Path

        static String audioPath = "";

        /// <summary>
        /// Get UI Path
        /// </summary>
        public static String GetAudioPath () {
            if (audioPath == "") {
                audioPath = String.Format ("file://{0}", GetCachePath (GlobalConst.DIR_AUDIO));
            }
            return audioPath;
        }

        /// <summary>
        /// Get UI Path
        /// </summary>
        public static String GetAudioPath (String fileName) {
            return GetAudioPath () + fileName;
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region AssetBundle Path

        static String assetBundlePath = "";

        /// <summary>
        /// Get AssetBundle Path
        /// </summary>
        public static String GetAssetBundlePath () {
            if (assetBundlePath == "") {
                assetBundlePath = String.Format ("file://{0}", GetCachePath (GlobalConst.DIR_ASSETBUNDLES));
            }
            return assetBundlePath;
        }

        /// <summary>
        /// Get AssetBundle Path
        /// </summary>
        public static String GetAssetBundlePath (String fileName) {
            return GetAssetBundlePath () + fileName;
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Character Path

        static String characterPath = "";

        /// <summary>
        /// Get Character Path
        /// </summary>
        public static String GetCharacterPath () {
            if (characterPath == "") {
                characterPath = GetCachePath (GlobalConst.DIR_CHARACTER);
            }
            return characterPath;
        }

        /// <summary>
        /// Get Character Path
        /// </summary>
        public static String GetCharacterPath (String fileName) {
            return GetCharacterPath () + fileName;
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Animation Path

        static String animationPath = "";

        /// <summary>
        /// Get Animation Directory
        /// </summary>
        public static string GetAnimationPath () {
            if (animationPath == "") {
                animationPath = GetCachePath (GlobalConst.DIR_ANIMATION);
            }
            return animationPath;
        }

        /// <summary>
        /// Get Animation Directory
        /// </summary>
        public static string GetAnimationPath (string fileName) {
            return GetAnimationPath () + fileName;
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Effect Path

        static String effectPath = "";

        /// <summary>
        /// Get Effect Path
        /// </summary>
        public static String GetEffectPath () {
            if (effectPath == "") {
                effectPath = GetCachePath (GlobalConst.DIR_EFFECT);
            }
            return effectPath;
        }

        /// <summary>
        /// Get Effect Path
        /// </summary>
        public static String GetEffectPath (String fileName) {
            return GetEffectPath () + fileName;
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    } // end class
} //end namespace