/**----------------------------------------------------------------------------//
  @file   : BenBoot.cs

  @brief  : 游戏核心资源加载
  
  @par	  : E-Mail
            609043941@qq.com
  			
  @par	  : Copyright(C) 

  @par	  : history
			2016/08/23 Ver 0.00 Created by Liuhaixia
//-----------------------------------------------------------------------------*/
using System.Collections;
using UnityEngine;

public class BenBoot : MonoBehaviour
{
    public static string Name{ get{ return typeof(BenBoot).Name; } }

    public static BenBoot Inst = null;

    void Awake()
    {
        if(Inst == null)
            Inst = this;
        DontDestroyOnLoad(gameObject);
    }

	// Use this for initialization
	void Start () {

        LocalInit();

        Init();

        StartCoroutine(CollectSystemGC());
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// 本地初始化数据
    /// </summary>
    void LocalInit()
    {
        
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        StopAllCoroutines();
        StartCoroutine(CorInit());
    }

    /// <summary>
    /// 初始化信息
    /// </summary>
    /// <returns></returns>
    IEnumerator CorInit()
    {
        yield return new WaitForSeconds(0.01f);          
    }

    /// <summary>
    /// 清楚缓存
    /// </summary>
    void ClearCaching()
    {
        if (PlayerPrefs.GetInt(ConstUtils.PP_NAME_CACHING_VER, 0) != ConstUtils.RES_VERSION_CODE)
        {
            PlayerPrefs.SetInt(ConstUtils.PP_NAME_CACHING_VER, ConstUtils.RES_VERSION_CODE);
            Caching.CleanCache();
        }
    }

    /// <summary>
    /// 进入主场景或者请求
    /// </summary>
    void EnterMainScene()
    {
    }

    /// <summary>
    /// 回收系统垃圾
    /// </summary>
    IEnumerator CollectSystemGC()
    {
        yield return new WaitForSeconds(10f);

        System.GC.Collect();
        Resources.UnloadUnusedAssets();

        StartCoroutine(CollectSystemGC());
    }
}