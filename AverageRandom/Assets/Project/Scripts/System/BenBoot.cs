/**----------------------------------------------------------------------------//
  @file   : BenBoot.cs

  @brief  : 游戏核心资源加载
  
  @par	  : E-Mail
            liu_haixia@gowild.cn
  			
  @par	  : Copyright(C) Gowild Robotics Inc.

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

        StartCoroutine(Init());

        InvokeRepeating("CollectSystemGC", 10, 900);
	}

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// 初始化信息
    /// </summary>
    /// <returns></returns>
    IEnumerator Init()
    {
        if (false)
        {
            OutputDebugger.Enable = true;

            OutputDebugger.Inst.Init();
        }

        yield return new WaitForEndOfFrame();
    }

    /// <summary>
    /// 回收系统垃圾
    /// </summary>
    void CollectSystemGC()
    {
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
    }
}