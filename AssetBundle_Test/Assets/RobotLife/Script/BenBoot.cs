/**----------------------------------------------------------------------------//
  @file   : BenBoot.cs

  @brief  : Boot Script
  
  @par	  : E-Mail
            609043941@qq.com
  			
  @par	  : Copyright(C) Ben Technology Inc.

  @par	  : history
			2017/08/18 Ver 0.00 Created by Liuhaixia
//-----------------------------------------------------------------------------*/
using UnityEngine;
using System.Collections;

public class BenBoot : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
		AsyncOperation sceneAsync = Application.LoadLevelAsync("Display");
		yield return sceneAsync;

		Debug.Log ("Load Display Scene Success!");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
