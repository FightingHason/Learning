/**----------------------------------------------------------------------------//
  @file   : ConstUtils.cs

  @brief  : 全局常量
  
  @par	  : E-Mail
            609043941@qq.com
  			
  @par	  : Copyright(C) 

  @par	  : history
			2016/08/23 Ver 0.00 Created by Liuhaixia
//-----------------------------------------------------------------------------*/
using System;

public class ConstUtils{

    /// <summary>
    /// 资源版本
    /// </summary>
    public static int RES_VERSION_CODE = 0;

    /// <summary>
    /// APP版本
    /// </summary>
    public static int APP_VERSION_CODE = 1;

    /// <summary>
    /// Resource dir
    /// </summary>
    public const string DIR_UI                  = "UI/";
    public const string DIR_XML                 = "Xml/";
    public const string DIR_OTHER               = "Other/";
    public const string DIR_AUDIO               = "Audio/";
    public const string DIR_TEXTURE             = "Texture/";
    public const string DIR_MATERIAL            = "Material/";
    public const string DIR_UI_LOADING          = "LoadingUI/";
    public const string DIR_ASSET_BUNDLES       = "AssetBundles/";

    /// <summary>
    /// Editor下路径
    /// </summary>
    public const string EDITOR_DIR_OUPUT        = "Output/";
    public const string EDITOR_DIR_EDITOR_XML   = "Editor/Xml/";
    public const string EDITOR_DIR_RES_XML      = "Resources/Xml/";

    /// <summary>
    /// 名称
    /// </summary>
    public const string TYPE_NAME_AB            = ".ben";

    /// <summary>
    /// XML节点
    /// </summary>
    public const string XML_NODE_ITEM           = "Item";

    /// <summary>
    /// XML节点属性
    /// </summary>
    public const string XML_NODE_ATTR_ID        = "id";
    public const string XML_NODE_ATTR_NAME      = "name";
    public const string XML_NODE_ATTR_VALUE     = "value";

    /// <summary>
    /// 常用字符
    /// </summary>
    public const string STR_SPIT_PARAM          = ";";
    public const string STR_SPIT_KV             = ":";
    public const string STR_SPIT_KVPAIR         = ",";
    public const string STR_SPIT_V              = "_";
    public const string STR_SPIT_SLASH          = "/";
    public const string CHAR_BAR                = "|";
    public const string CHAR_ADD                = "+";
    public const string CHAR_MINUS              = "-";
    public const string CHAR_MULTIPLE           = "x";
    public const string CHAR_PERCENT            = "%";
    public const string CHAR_RMB                = "¥";
    public const string CHAR_USD                = "$";
    public const string CHAR_CN_TON             = "、";

    /// <summary>
    /// 最大显示数量
    /// </summary>
    public const int MAX_NUM_SHOW               = 999999;
    public const int MAX_NUM_RANDOM             = 100000000;

    /// <summary>
    /// Unix 开始时间
    /// </summary>
    public static readonly DateTime UNIX_START_TIME = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Local);

    // 剩余时间格式（x天x时x分x秒）
    public const string TIME_FORMAT_SHOW_DAY    = "{0}天{1}时{2}分{3}秒";

    /// <summary>
    /// PlayerPrefs Const Key
    /// </summary>
    public const string PP_NAME_CACHING_VER     = "CachingVersion";     // 缓存版本号
}
