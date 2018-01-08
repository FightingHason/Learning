//*************************************************
//File:		DelayDestroyBehaviour.cs
//
//Brief:    Delay To Destroy
//
//Author:   Liuhaixia
//
//E-Mail:   609043941@qq.com
//
//History:  2017/12/21 Created by Liuhaixia
//*************************************************
using System;
using System.Collections;
using UnityEngine;

namespace Ben {
	/// <summary>
	/// Delay to destroy
	/// </summary>
	public class DelayDestroyBehaviour : MonoBehaviour {
		public Single destroyDelay = 3.2f;

		Action _callback = null;

		/// <summary>
		/// Destroy Callback
		/// </summary>
		public Action DestroyCallback {
			set { _callback = value; }
		}

		IEnumerator Start () {

			yield return new WaitForSeconds (destroyDelay);

			if (_callback != null) {
				_callback ();
			}

			Destroy (this);
			Destroy (this.gameObject);
		}

	}
} //end namespace