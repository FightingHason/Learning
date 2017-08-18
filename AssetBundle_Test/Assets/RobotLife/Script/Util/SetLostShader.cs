using System.Collections.Generic;
/**----------------------------------------------------------------------------//
  @file   : SetLostShader.cs

  @brief  : 重新添加丢失Shader
  
  @par	  : E-Mail
            609043941@qq.com
  			
  @par	  : Copyright(C) Ben Technology Inc.

  @par	  : history
			2017/8/17 Ver 0.00 Created by Liuhaixia
//-----------------------------------------------------------------------------*/
using UnityEngine;

public class SetLostShader : MonoBehaviour
{

    /// <summary>
    /// 是否初始化完成
    /// </summary>
    bool bInitFinish = false;

    /// <summary>
    /// 丢失的shader的名称
    /// </summary>
    List<string> lostShaderList = new List<string>();

    // Use this for initialization
    void Start()
    {
        if (bInitFinish == false)
        {
            //lostShaderList = ConfigManager.Inst.LostShaderList;
            lostShaderList.Add("UnityChan/Skin");
            lostShaderList.Add("UnityChan/Eyelash - Transparent");

            SetShader(transform);

            bInitFinish = true;
        }
    }

    /// <summary>
    /// 重设shader
    /// </summary>
    /// <param name="mTran"></param>
    void SetShader(Transform mTran)
    {
        for (int i = 0; i < mTran.childCount; i++)
        {
            Renderer render = mTran.GetChild(i).GetComponent<Renderer>();

            if (render != null)
            {
                Material[] matArray = render.materials;
                for (int j = 0; j < matArray.Length; ++j)
                {
                    if (lostShaderList.Contains(matArray[j].shader.name))
                    {
                        Shader shader = Shader.Find(matArray[j].shader.name);
                        if (shader != null)
                            matArray[j].shader = shader;
                    }
                }
            }

            SetShader(mTran.GetChild(i));
        }

        Destroy(this);
    }

    /// <summary>
    /// 执行
    /// </summary>
    public void Excute()
    {
        if (bInitFinish == false)
            Start();
        else
            SetShader(transform);
    }

}//end class