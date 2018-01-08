//*************************************************
//File:		XmlUtils.cs
//
//Brief:    XML File
//
//Author:   Liuhaixia
//
//E-Mail:   609043941@qq.com
//
//History:  2016/08/24 Created by Liuhaixia
//*************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;
namespace Ben {
    public class XmlUtils {

        /// <summary>
        /// 获取到Table全路径
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>Table全路径</returns>
        public static String GetTablePath (String fileName) {
            String path = null;
            if (Application.platform == RuntimePlatform.IPhonePlayer) {
                path = Application.persistentDataPath + "/Documents";
            } else if (Application.platform == RuntimePlatform.Android) {
                path = Application.persistentDataPath + "/Documents";
            } else {
                path = Application.dataPath + "/Documents";
            }

            if (!Directory.Exists (path)) {
                Directory.CreateDirectory (path);
            }

            return path + "/" + fileName;
        }

        /// <summary>
        /// 将Res下单个文件复制到目标路径
        /// </summary>
        /// <param name="targetPath">目标路径</param>
        /// <param name="resPath">Res下单个文件名路径</param>
        public static void CopyResXmlToPath (String targetPath, String resPath) {
            XmlDocument doc = new XmlDocument ();
            TextAsset text = Resources.Load (resPath, typeof (TextAsset)) as TextAsset;
            doc.LoadXml (text.text);
            doc.Save (targetPath);
        }

        /// <summary>
        /// 指定路径创建xml
        /// </summary>
        /// <param name="filePath"></param>
        public static void CreateXml (String filePath) {
            String dirName = filePath.StartToLastSlash ();
            if (!Directory.Exists (dirName))
                Directory.CreateDirectory (dirName);

            XmlDocument xmlDoc = new XmlDocument ();
            XmlDeclaration xmlDeclare = xmlDoc.CreateXmlDeclaration ("1.0", "utf-8", "");
            xmlDoc.AppendChild (xmlDeclare);

            XmlElement root = xmlDoc.CreateElement ("Data");
            root.SetAttribute ("Version", "1");

            xmlDoc.AppendChild (root);
            xmlDoc.Save (filePath);
        }

        /// <summary>
        /// 为xml中增加item
        /// </summary>
        public static void AddPathMD5ItemToXml (String filePath, String fileName, String itemPath, String itemName, String itemMD5) {
            if (!Directory.Exists (filePath))
                Directory.CreateDirectory (filePath);

            String path = String.Format ("{0}/{1}.xml", filePath, fileName);
            if (!File.Exists (path)) {
                CreateXml (path);
            }

            AddPathMD5NodeToXml (filePath, fileName, itemPath, itemName, itemMD5);
        }

        /// <summary>
        /// 添加节点进入XML
        /// </summary>
        private static void AddPathMD5NodeToXml (String filePath, String fileName, String itemPath, String itemName, String itemMD5) {
            String path = String.Format ("{0}/{1}.xml", filePath, fileName);
            XmlDocument xmlDoc;
            XmlNode rootNode;
            if (GetRootNodeFromXml (path, out xmlDoc, out rootNode)) {
                Boolean hasNodeAlready = false;
                foreach (XmlElement child in rootNode.ChildNodes) {
                    if (child.GetAttribute ("Name") == itemName) {
                        child.SetAttribute ("Path", itemPath);
                        child.SetAttribute ("MD5", itemMD5);
                        hasNodeAlready = true;
                        break;
                    }
                }

                if (!hasNodeAlready) {
                    XmlElement xe = xmlDoc.CreateElement ("Item");
                    xe.SetAttribute ("Name", itemName);
                    xe.SetAttribute ("Path", itemPath);
                    xe.SetAttribute ("MD5", itemMD5);
                    rootNode.AppendChild (xe);
                }
                xmlDoc.Save (path);
            }
        }

        /// <summary>
        /// 为xml中增加item
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="attrValue"></param>
        public static void AddItemToXml (String filePath, String fileName, Hashtable content, Boolean isSetAttribute = true) {
            if (!Directory.Exists (filePath))
                Directory.CreateDirectory (filePath);

            String path = String.Format ("{0}/{1}.xml", filePath, fileName);
            if (!File.Exists (path)) {
                CreateXml (path);
            }

            XmlDocument xmlDoc;
            XmlNode rootNode;
            if (GetRootNodeFromXml (path, out xmlDoc, out rootNode)) {
                XmlElement item = xmlDoc.CreateElement ("Item");

                foreach (DictionaryEntry de in content) {
                    if (isSetAttribute)
                        item.SetAttribute (de.Key.ToString (), de.Value.ToString ());
                    else {
                        XmlElement itemChild = xmlDoc.CreateElement (de.Key.ToString ());
                        itemChild.InnerText = de.Value.ToString ();
                        item.AppendChild (itemChild);
                    }
                }

                rootNode.AppendChild (item);
                xmlDoc.Save (path);
            }
        }

        /// <summary>
        /// 为xml中增加item
        /// </summary>
        public static void AddItemArrayToXml (String filePath, String fileName, String[] content, Boolean removeAll = false) {
            if (!Directory.Exists (filePath))
                Directory.CreateDirectory (filePath);

            String path = String.Format ("{0}/{1}.xml", filePath, fileName);
            if (!File.Exists (path)) {
                CreateXml (path);
            }

            XmlDocument xmlDoc;
            XmlNode rootNode;
            if (GetRootNodeFromXml (path, out xmlDoc, out rootNode)) {
                if (removeAll)
                    rootNode.RemoveAll ();

                foreach (String info in content) {
                    XmlElement item = xmlDoc.CreateElement ("Item");

                    item.InnerText = info;

                    rootNode.AppendChild (item);
                }

                xmlDoc.Save (path);
            }
        }

        /// <summary>
        /// 为xml中增加item
        /// </summary>
        public static void AddItemToXml (String filePath, Hashtable content, Hashtable children) {
            XmlDocument xmlDoc;
            XmlNode rootNode;
            if (GetRootNodeFromXml (filePath, out xmlDoc, out rootNode)) {
                XmlElement item = xmlDoc.CreateElement ("Item");
                XmlElement elm = xmlDoc.CreateElement ("Element");

                foreach (DictionaryEntry de in content) {
                    item.SetAttribute (de.Key.ToString (), de.Value.ToString ());
                }
                foreach (DictionaryEntry de in children) {
                    elm.SetAttribute (de.Key.ToString (), de.Value.ToString ());
                }

                rootNode.AppendChild (item);
                item.AppendChild (elm);
                xmlDoc.Save (filePath);
            }
        }

        /// <summary>
        /// 更新xml中item的属性值
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="uid"></param>
        /// <param name="content"></param>
        public static void UpdateAttrToItem (String filePath, ulong uid, Hashtable content) {
            XmlDocument xmlDoc;
            XmlNode rootNode;
            if (GetRootNodeFromXml (filePath, out xmlDoc, out rootNode)) {
                foreach (XmlElement item in rootNode.ChildNodes) {
                    if (item.GetAttribute (PascalNameConst.UID) == uid.ToString ()) {
                        foreach (DictionaryEntry de in content) {
                            item.SetAttribute (de.Key.ToString (), de.Value.ToString ());
                        }
                        if (Application.isEditor)
                            Debug.Log ("Xml Updated to " + filePath);
                        xmlDoc.Save (filePath);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 更新xml中item的属性值
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="uid"></param>
        /// <param name="content"></param>
        /// <param name="children"></param>
        public static void UpdateAttrToItem (String filePath, ulong uid, Hashtable content, Hashtable children) {
            XmlDocument xmlDoc;
            XmlNode rootNode;
            if (GetRootNodeFromXml (filePath, out xmlDoc, out rootNode)) {
                foreach (XmlElement item in rootNode.ChildNodes) {
                    if (item.GetAttribute (PascalNameConst.UID) == uid.ToString ()) {
                        foreach (DictionaryEntry de in content) {
                            item.SetAttribute (de.Key.ToString (), de.Value.ToString ());
                        }
                        Debug.Log ("Xml Attributes updated." + filePath);

                        Boolean updated = false;
                        if (children.ContainsKey ("Id")) {
                            foreach (XmlElement child in item.ChildNodes) {
                                //if (child.GetAttribute ("Id") == children["Id"]) {
                                foreach (DictionaryEntry d in children) {
                                    child.SetAttribute (d.Key.ToString (), d.Value.ToString ());
                                    updated = true;
                                    break;
                                }
                                //}
                                if (updated) break;
                            }
                        }
                        if (!updated) {
                            XmlElement elm = xmlDoc.CreateElement ("Element");
                            foreach (DictionaryEntry d in children) {
                                elm.SetAttribute (d.Key.ToString (), d.Value.ToString ());
                            }
                            item.AppendChild (elm);
                        }
                        xmlDoc.Save (filePath);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 从xml中删除指定属性值的Item
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="attrType"></param>
        /// <param name="attrValue"></param>
        public static void DelItemByAttr (String filePath, String attrType, String attrValue) {
            XmlDocument xmlDoc;
            XmlNode rootNode;
            if (GetRootNodeFromXml (filePath, out xmlDoc, out rootNode)) {
                foreach (XmlElement item in rootNode.ChildNodes) {
                    if (item.GetAttribute (attrType) == attrValue) {
                        item.ParentNode.RemoveChild (item);
                        xmlDoc.Save (filePath);
                        if (Application.isEditor)
                            Debug.Log ("Delete Item for Xml By " + attrValue + " from " + filePath);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 获取xml表数据组
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="xmlDoc"></param>
        /// <param name="nodeRoot"></param>
        /// <returns>是否获取成功</returns>
        public static List<String> GetDirectoryXml (String filePath) {
            XmlDocument xmlDoc;
            XmlNode rootNode;
            List<String> dir = new List<String> ();
            if (GetRootNodeFromXml (filePath, out xmlDoc, out rootNode)) {
                foreach (XmlElement item in rootNode.ChildNodes) {
                    dir.Add (item.InnerText);
                }
            }
            return dir;
        }
        /// <summary>
        /// 获取xml表根节点
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="nodeRoot"></param>
        /// <returns>是否获取成功</returns>
        public static Boolean GetRootNodeFromXml (String filePath, out XmlNode rootNode, Boolean isResourcesLoad = false) {
            XmlDocument xmlDoc;
            return GetRootNodeFromXml (filePath, out xmlDoc, out rootNode, isResourcesLoad);
        }

        /// <summary>
        /// 获取xml表根节点
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="xmlDoc"></param>
        /// <param name="nodeRoot"></param>
        /// <returns>是否获取成功</returns>
        public static Boolean GetRootNodeFromXml (String filePath, out XmlDocument xmlDoc, out XmlNode rootNode, Boolean isResourcesLoad = false) {
            xmlDoc = null;
            rootNode = null;

            xmlDoc = new XmlDocument ();
            if (isResourcesLoad) {
                TextAsset textAsset = Resources.Load (filePath.StartToLastPoint (), typeof (TextAsset)) as TextAsset;
                xmlDoc.LoadXml (textAsset.text);
            } else {
                if (!File.Exists (filePath)) {
                    //Debug.LogError("File Not Exist! Path: " + filePath);
                    return false;
                } else if (CheckTxtIsNull (filePath)) {
                    //File.Delete(filePath); // 打开状态下无法删除
                    Debug.LogError ("File Exist, But Info is null! Path: " + filePath);
                    return false;
                } else {
                    xmlDoc.Load ("file://" + filePath);
                }
            }
            rootNode = xmlDoc.SelectSingleNode ("Data");

            if (rootNode == null) {
                // 如果读表异常，删除下载或保存的表，然后读取Resources下资源
                if (isResourcesLoad == false) {
                    //File.Delete(filePath); // 打开状态下无法删除
                    return GetRootNodeFromXml (filePath.Replace (GetCacheFilePath (""), ""), out xmlDoc, out rootNode, true);
                } else {
                    Debug.LogError ("Get XML Root Node <Data> Is Not Exist! Path: " + filePath);
                    return false;
                }
            }

            // 检测Data下时候有东西
            if (rootNode.ChildNodes.Count == 0) {
                Debug.LogError ("Get XML Root Node <Data> Info Is Null! Path: " + filePath);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取sdcard xml表根节点
        /// </summary>
        /// <returns>是否获取成功</returns>
        public static Boolean GetSdcardXml (String fileName, out XmlNode rootNode) {
            rootNode = null;
            String filePath = GetCacheFilePath (fileName + ".xml");
            XmlDocument xmlDoc = new XmlDocument ();
            if (!File.Exists (filePath)) {
                return false;
            } else if (CheckTxtIsNull (filePath)) {
                Debug.LogError ("File Exist, But Info is null! Path: " + filePath);
                return false;
            } else {
                XmlReaderSettings settings = new XmlReaderSettings ();
                settings.IgnoreComments = true;
                //xmlFilePath:xml文件路径
                XmlReader reader = XmlReader.Create ("file://" + filePath, settings);
                xmlDoc.Load (reader);
            }

            rootNode = xmlDoc.SelectSingleNode ("Data");

            if (rootNode == null) {
                Debug.LogError ("Get XML Root Node <Data> Is Not Exist! Path: " + filePath);
                return false;
            }

            if (rootNode.ChildNodes.Count == 0) {
                Debug.LogError ("Get XML Root Node <Data> Info Is Null! Path: " + filePath);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 检测文件是否为空
        /// </summary>
        public static Boolean CheckTxtIsNull (String filePath) {
            StreamReader sr = new StreamReader (filePath, Encoding.Default);
            String info = sr.ReadToEnd ().Replace (" ", "");
            if (info == String.Empty || info == "") {
                //为空  
                return true;
            } else {
                return false;
            }
        }

        public static String GetCacheFilePath (String filePath) {
            return Application.dataPath.StartToLastSlash () + "/" + CamelNameConst.ROBOT + "/" + filePath;
        }

    } //end class
} //end namespace