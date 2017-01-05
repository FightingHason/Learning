using UnityEditor;

public class ModifyAssetType : AssetPostprocessor
{
    //string[] m_modelTexPath = new string[1] { "Hello" };          //UI目录

    //string[] m_uiPath = new string[1] { "Hello" };          //UI目录

    //string[] m_texPath = new string[1] { "ssdfewqer" };        //贴图目录

    //string[] m_specialPath = new string[1] { "specialImage" };            //特殊路径不处理

    public void OnPreprocessTexture()
    {
        //if (Check(m_specialPath, assetPath))
        //{
        //    return;
        //}
        //if (Check(m_uiPath, assetPath))
        //{
        //    PreprocessUI();
        //}
        //if (Check(m_texPath, assetPath))
        //{
        //    PreprocessTex();
        //}
        //if (Check(m_modelTexPath, assetPath))
        //{
        //    PreprocessModelTex();
        //}
    }


    //检测目录匹配
    bool Check(string[] strs,string path)
    {
        foreach (string item in strs)
        {
            if (path.Contains(item))
            {
                return true;
            }
        }
        return false;
    }

    void PreprocessUI()
    {
        TextureImporter texImporter = (TextureImporter)assetImporter;
        texImporter.textureType = TextureImporterType.Default;
        //texImporter.spriteImportMode = SpriteImportMode.Single;
        //texImporter.spritePixelsPerUnit = 100;
        //texImporter.spritePivot = new UnityEngine.Vector2(0, 0);
        texImporter.isReadable = true;
        texImporter.mipmapEnabled = false;
        texImporter.filterMode = UnityEngine.FilterMode.Bilinear;
        texImporter.maxTextureSize = 2048;
        texImporter.textureFormat = TextureImporterFormat.ASTC_RGBA_4x4;
    }

    void PreprocessTex()
    {
        TextureImporter texImporter = (TextureImporter)assetImporter;
        texImporter.textureType = TextureImporterType.Default;
        texImporter.wrapMode = UnityEngine.TextureWrapMode.Clamp;
        texImporter.filterMode = UnityEngine.FilterMode.Bilinear;
        texImporter.maxTextureSize = 2048;
        texImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
    }

    /// <summary>
    /// 模型贴图
    /// </summary>
    //void PreprocessModelTex()
    //{
    //    TextureImporter texImporter = (TextureImporter)assetImporter;
    //    texImporter.textureType = TextureImporterType.Advanced;
    //    //Non Power Of 2
    //    texImporter.npotScale = TextureImporterNPOTScale.ToNearest;
    //    texImporter.isReadable = false;
    //    texImporter.spriteImportMode = SpriteImportMode.None;
    //    texImporter.mipmapEnabled = false;
    //    texImporter.wrapMode = UnityEngine.TextureWrapMode.Clamp;
    //    texImporter.filterMode = UnityEngine.FilterMode.Bilinear;
    //    texImporter.anisoLevel = 1;
    //    texImporter.maxTextureSize = 1024;
    //    texImporter.SetPlatformTextureSettings("iPhone", 1024, TextureImporterFormat.PVRTC_RGBA4, 100);
    //    texImporter.SetPlatformTextureSettings("Android", 1024, TextureImporterFormat.ETC2_RGBA8, 100);
    //    texImporter.SetPlatformTextureSettings("Standalone", 1024, TextureImporterFormat.DXT5, 100);
    //}
}
