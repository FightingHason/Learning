//*************************************************
//File:		FileUtils.cs
//
//Brief:    File Utils
//
//Author:   Liuhaixia
//
//E-Mail:   609043941@qq.com
//
//History:  2017/04/12 Created by Liuhaixia
//*************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using LitJson;
using UnityEngine;

namespace Ben {
    public class FileUtils {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Sprite & Texture2D

        /// <summary>
        /// 图片路径
        /// </summary>
        private static String imagePath = "";

        /// <summary>
        /// 以IO方式进行加载图片
        /// </summary>
        public static Texture2D GetTexture2DByIO (String productId) {
            if (imagePath == "")
                imagePath = PathUtils.GetUIPath ();

            //创建Texture
            Texture2D texture = new Texture2D (100, 100);
            //异常处理
            Byte[] bytes = GetBytesByIO (imagePath + productId);
            if (bytes != null)
                texture.LoadImage (bytes);

            //返回Texture
            return texture;
        }

        /// <summary>
        /// 以IO方式进行加载
        /// </summary>
        public static Sprite GetSpriteByIO (String productId) {
            if (imagePath == "")
                imagePath = PathUtils.GetUIPath ();

            //创建Texture
            Texture2D texture = new Texture2D (100, 100);
            //异常处理
            Byte[] bytes = GetBytesByIO (imagePath + productId);
            if (bytes != null)
                texture.LoadImage (bytes);

            //创建Sprite
            return Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 (0.5f, 0.5f));
        }

        /// <summary>
        /// 以IO方式进行加载LoadingUI
        /// </summary>
        public static Sprite[] GetLoadingSpriteByIO () {
            List<Sprite> loadingImageList = new List<Sprite> ();

            String loadingImagePath = PathUtils.GetCachePath (GlobalConst.DIR_UI_LOADING);
            if (Directory.Exists (loadingImagePath)) {
                String[] allFiles = Directory.GetFiles (loadingImagePath);
                for (Int32 i = 0; i < allFiles.Length; ++i) {
                    //创建Texture
                    Texture2D texture = new Texture2D (273, 114);
                    //异常处理
                    Byte[] bytes = GetBytesByIO (allFiles[i]);
                    if (bytes != null)
                        texture.LoadImage (bytes);

                    loadingImageList.Add (Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 (0.5f, 0.5f)));
                }
            }
            return loadingImageList.ToArray ();
        }

        /// <summary>
        /// 通过WWW获取图片
        /// </summary>
        public static IEnumerator GetSpriteByWWW (String srcPath, Sprite sprite) {
            WWW www = new WWW (srcPath);
            yield return www;
            if (www != null && String.IsNullOrEmpty (www.error)) {
                //获取Texture
                Texture2D texture = www.texture;
                //创建Sprite
                sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 (0.5f, 0.5f));
            } else {
                Debug.LogError ("Load Error Path: " + srcPath);
            }
            www.Dispose ();
            www = null;
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Text

        /// <summary>
        /// 通过StreamReader逐行读取文本
        /// </summary>
        static String ReadTextByStreamLine (String path) {
            String result = "";

            if (File.Exists (path)) {
                try {
                    StreamReader sr = new StreamReader (path, Encoding.UTF8);
                    String line;
                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine ()) != null) {
                        result += line;
                    }
                    sr.Dispose ();
                    sr.Close ();
                    sr = null;
                } catch (Exception e) {
                    // 向用户显示出错消息
                    Debug.LogError ("The file could not be read! Path: " + path + " || Message: " + e.Message);
                }
            } else {
                Debug.LogError ("No exist file! Path: " + path);
            }

            return result;
        }

        /// <summary>
        /// 通过FileStream写入信息
        /// </summary>
        static void WriteTextByIO (String path, String info) {
            try {
                FileStream fs = new FileStream (path, FileMode.Create);

                Byte[] data = Encoding.UTF8.GetBytes (info);
                fs.Write (data, 0, data.Length);
                fs.Flush ();
                fs.Dispose ();
                fs.Close ();
                fs = null;
            } catch (Exception e) {
                // 向用户显示出错消息
                Debug.LogError ("The file could not be write! Path: " + path + " || Message: " + e.Message);
            }
        }

        /// <summary>
        /// 通过FileStream以IO方式加载txt
        /// </summary>
        public static String ReadTextByIO (String path) {
            if (File.Exists (path)) {
                Byte[] bytes = GetBytesByIO (path);
                if (bytes != null) {
                    return Encoding.UTF8.GetString (bytes);
                } else {
                    Debug.LogError ("The file could not be read! Path: " + path);
                }
            } else {
                Debug.LogError ("No exist file! Path: " + path);
            }
            return "";
        }

        /// <summary>
        /// 通过StreamReader一次性读取文本
        /// </summary>
        public static String ReadTextByStream (String path) {
            String result = "";

            if (File.Exists (path)) {
                try {
                    // 创建一个 StreamReader 的实例来读取文件 
                    // using 语句也能关闭 StreamReader
                    //using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
                    //{
                    //    result = sr.ReadToEnd();
                    //}

                    StreamReader sr = new StreamReader (path, Encoding.UTF8);
                    result = sr.ReadToEnd ();
                    sr.Dispose ();
                    sr.Close ();
                    sr = null;
                } catch (Exception e) {
                    // 向用户显示出错消息
                    Debug.LogError ("The file could not be read! Path: " + path + " || Message: " + e.Message);
                }
            } else {
                Debug.LogError ("No exist file! Path: " + path);
            }

            return result;
        }

        /// <summary>
        /// 通过StreamWriter写入信息
        /// </summary>
        public static void WriteTextByStream (String path, String info) {
            try {
                FileStream fs = new FileStream (path, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter (fs, Encoding.UTF8);
                sw.WriteLine (info);
                sw.Flush ();
                sw.Dispose ();
                sw.Close ();
                sw = null;

                fs.Dispose ();
                fs.Close ();
                fs = null;
            } catch (Exception e) {
                // 向用户显示出错消息
                Debug.LogError ("The file could not be write! Path: " + path + " || Message: " + e.Message);
            }
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region File or Directory Operation (Delete, Create...)

        /// <summary>
        /// 删除目标路径文件
        /// </summary>
        public static void DeleteFile (String pathName) {
            if (String.IsNullOrEmpty (pathName)) {
                Debug.LogError ("DeleteFile Path is empty!");
                return;
            }

            if (File.Exists (pathName))
                File.Delete (pathName);

            CheckDirectory (pathName);
        }

        /// <summary>
        /// 检测目标路径文件的文件夹是否存在
        /// </summary>
        public static void CheckDirectory (String pathName) {
            if (String.IsNullOrEmpty (pathName)) {
                Debug.LogError ("CheckDirectory Path is empty!");
                return;
            }

            String dirName = pathName.StartToLastSlash ();

            if (String.IsNullOrEmpty (dirName)) {
                Debug.LogError ("CreateDirectory Path is empty!");
                return;
            }
            if (!Directory.Exists (dirName))
                Directory.CreateDirectory (dirName);
        }

        /// <summary>
        /// 删除路径文件夹下所有文件（包括子文件夹）
        /// </summary>
        public static void ClearDirFiles (String srcPath) {
            try {
                DirectoryInfo dir = new DirectoryInfo (srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos (); //返回目录中所有文件和子目录
                foreach (FileSystemInfo i in fileinfo) {
                    if (i is DirectoryInfo) //判断是否文件夹
                    {
                        DirectoryInfo subdir = new DirectoryInfo (i.FullName);
                        subdir.Delete (true); //删除子目录和文件
                    } else {
                        File.Delete (i.FullName); //删除指定文件
                    }
                }

                Debug.Log ("Delete Path: " + srcPath + " All Info Success!");
            } catch (Exception e) {
                Debug.LogError ("FileClearDir: " + srcPath + " Exception: " + e.Message);
                throw;
            }
        }

        /// <summary>
        /// 检测文件是否为空
        /// </summary>
        public static Boolean CheckTxtIsNull (String filePath) {
            String info = "";
            if (File.Exists (filePath)) {
                info = ReadTextByStream (filePath);
                info = info.Replace (" ", "");
                if (false == String.IsNullOrEmpty (info)) {
                    return false;
                } else {
                    return true;
                }
            } else {
                return true;
            }
        }

        /// <summary>
        /// 读取sdcard文本
        /// </summary>
        public static Boolean ReadTxtInfo (String filePath, out String info) {
            info = "";
            if (File.Exists (filePath)) {
                info = ReadTextByStream (filePath);
                if (false == String.IsNullOrEmpty (info)) {
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }

        /// <summary>
        /// Read Table Text Info
        /// </summary>
        public static Boolean ReadTableTextInfo (out JsonData totalJson, String tableName, String fileType = GlobalConst.TB_TYPE_JSON) {
            Boolean isSdcard = true, isRet = false;
            String jsonStr = null;
            String tablePath = PathUtils.GetTablePath (tableName, fileType);
            if (File.Exists (tablePath)) {
                isSdcard = true;
                jsonStr = ReadTextByStream (tablePath);
            } else {
                isSdcard = false;
                tablePath = GlobalConst.DIR_TABLE + tableName;
                TextAsset textAsset = Resources.Load (tablePath) as TextAsset;
                if (textAsset != null)
                    jsonStr = textAsset.text;
            }

            if (false == String.IsNullOrEmpty (jsonStr) && jsonStr != "\r" && jsonStr != "\n" && jsonStr != "\r\n") {
                try {
                    totalJson = JsonMapper.ToObject<JsonData> (jsonStr);
                    isRet = true;
                } catch (Exception ex) {
                    Debug.LogError ("本地信息表转Json格式错误！Path: " + tablePath + " || Exception: " + ex.Message);
                    totalJson = null;
                    isRet = false;
                }
            } else {
                Debug.LogError ("本地信息表为空！Path: " + tablePath);
                totalJson = null;
                isRet = false;
            }

            if (totalJson == null && isSdcard) {
                DeleteFile (tablePath);
                isRet = ReadResourceTableTextInfo (out totalJson, tableName, fileType);
            }

            return isRet;
        }

        /// <summary>
        /// Read Table Text Info
        /// </summary>
        public static Boolean ReadResourceTableTextInfo (out JsonData totalJson, String tableName, String fileType = GlobalConst.TB_TYPE_JSON) {
            String jsonStr = null;
            String tablePath = GlobalConst.DIR_TABLE + tableName;
            TextAsset textAsset = Resources.Load (tablePath) as TextAsset;
            if (textAsset != null)
                jsonStr = textAsset.text;

            if (false == String.IsNullOrEmpty (jsonStr)) {
                try {
                    totalJson = JsonMapper.ToObject<JsonData> (jsonStr);
                    return true;
                } catch (Exception ex) {
                    Debug.LogError ("本地信息表转Json格式错误！Path: " + tablePath + " || Exception: " + ex.Message);
                    totalJson = null;
                    return false;
                }
            } else {
                Debug.LogError ("本地信息表为空！Path: " + tablePath);
                totalJson = null;
                return false;
            }
        }

        /// <summary>
        /// Read Table Text Info
        /// </summary>
        public static Boolean ReadTableTextInfo (out String tableInfo, String tableName, String fileType = GlobalConst.TB_TYPE_JSON) {
            tableInfo = null;
            String tablePath = PathUtils.GetTablePath (tableName, fileType);
            if (File.Exists (tablePath)) {
                tableInfo = ReadTextByStream (tablePath);
            } else {
                tablePath = GlobalConst.DIR_TABLE + tableName;
                TextAsset textAsset = Resources.Load (tablePath) as TextAsset;
                if (textAsset != null)
                    tableInfo = textAsset.text;
            }

            if (false == String.IsNullOrEmpty (tableInfo)) {
                return true;
            } else {
                Debug.LogError ("本地信息表为空！Path: " + tablePath);
                return false;
            }
        }

        /// <summary>
        /// Read Table Text Info
        /// </summary>
        public static Boolean ReadSdcardTableTextInfo (out JsonData totalJson, String tableName, String fileType = GlobalConst.TB_TYPE_JSON) {
            totalJson = null;
            String jsonStr = null;
            String tablePath = PathUtils.GetTablePath (tableName, fileType);
            if (File.Exists (tablePath)) {
                jsonStr = ReadTextByStream (tablePath);
            }

            if (false == String.IsNullOrEmpty (jsonStr) && jsonStr != "\r" && jsonStr != "\n" && jsonStr != "\r\n") {
                try {
                    totalJson = JsonMapper.ToObject<JsonData> (jsonStr);
                    return true;
                } catch (Exception ex) {
                    Debug.LogError ("本地信息表转Json格式错误！Path: " + tablePath + " || Exception: " + ex.Message);
                    return false;
                }
            } else {
                Debug.LogError ("本地信息表为空！Path: " + tablePath);
                return false;
            }
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region MD5加解密字符串

        private const String EncryptKey = "vksxkwlvkd@rlarudals%*&@";
        private const Boolean UseEncrypt = true;
        private const Boolean UseDecrypt = true;

        /// <summary>
        /// Encrypt script
        /// </summary>
        public static String EnscriptionMD5 (String normalString) {
            if (normalString == null)
                return null;
            if (normalString.Equals (String.Empty))
                return String.Empty;

            Byte[] Results;
            UTF8Encoding UTF8 = new UTF8Encoding ();

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider ();
            Byte[] TDESKey = HashProvider.ComputeHash (UTF8.GetBytes (EncryptKey));

            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider ();

            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            try {
                Byte[] DataToEncrypt = UTF8.GetBytes (normalString);
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor ();
                Results = Encryptor.TransformFinalBlock (DataToEncrypt, 0, DataToEncrypt.Length);
            } catch (Exception e) {
                return "Error : " + e.ToString () + " normalString : " + normalString;
            } finally {
                TDESAlgorithm.Clear ();
                HashProvider.Clear ();
                TDESAlgorithm.Clear ();
                HashProvider.Clear ();
            }

            return Convert.ToBase64String (Results);
        }

        /// <summary>
        /// Decryption script
        /// </summary>
        public static String DescriptionMD5 (String encryptedString) {
            if (encryptedString == null)
                return null;
            if (encryptedString.Equals (String.Empty))
                return String.Empty;

            Byte[] Results;

            UTF8Encoding UTF8 = new UTF8Encoding ();

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider ();
            Byte[] TDESKey = HashProvider.ComputeHash (UTF8.GetBytes (EncryptKey));

            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider ();
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            try {
                encryptedString = encryptedString.Replace (" ", "+");
                Byte[] DataToDecrypt = Convert.FromBase64String (encryptedString);
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor ();
                Results = Decryptor.TransformFinalBlock (DataToDecrypt, 0, DataToDecrypt.Length);
            } catch (Exception e) {
                throw new Exception (e.ToString () + " encryptedString : " + encryptedString);
                //return "Error";
            } finally {
                TDESAlgorithm.Clear ();
                HashProvider.Clear ();
                TDESAlgorithm.Clear ();
                HashProvider.Clear ();
            }
            return UTF8.GetString (Results);
        }

        /// <summary>
        /// Create File MD5
        /// </summary>
        public static Boolean GetFileMD5 (String filePath, out String fileMD5) {
            fileMD5 = "";
            if (!File.Exists (filePath)) {
                return false;
            }
            MD5CryptoServiceProvider md5Generator = new MD5CryptoServiceProvider ();
            FileStream file = new FileStream (filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            Byte[] hash = md5Generator.ComputeHash (file);
            fileMD5 = BitConverter.ToString (hash).Replace ("-", "");
            file.Close ();
            return true;
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Private Methods

        /// <summary>
        /// 通过IO加载文件
        /// </summary>
        private static Byte[] GetBytesByIO (String path) {
            if (File.Exists (path)) {
                try {
                    //创建文件读取流
                    FileStream fileStream = new FileStream (path, FileMode.Open, FileAccess.Read);
                    fileStream.Seek (0, SeekOrigin.Begin);
                    //创建文件长度缓冲区
                    Byte[] bytes = new Byte[fileStream.Length];
                    //读取文件
                    fileStream.Read (bytes, 0, (Int32) fileStream.Length);
                    //释放文件读取流
                    fileStream.Dispose ();
                    fileStream.Close ();
                    fileStream = null;

                    return bytes;
                } catch (Exception e) {
                    // 向用户显示出错消息
                    Debug.LogError ("The file could not be read! Path: " + path + " || Message: " + e.Message);
                    return null;
                }
            } else {
                Debug.LogError ("No Exist File! Path: " + path);
                return null;
            }
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    } // end class
} //end namespace