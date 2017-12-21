//*************************************************
//File:		BenBoot.cs
//
//Brief:    Boot for project
//
//Author:   Liuhaixia
//
//E-Mail:   609043941@qq.com
//
//History:  2017/11/25 Created by Liuhaixia
//*************************************************
using System;
using Ben.Net;
using LitJson;
using UnityEngine;

namespace Ben {
	public class BenBoot : MonoBehaviour {

		public static BenBoot Instance = null;

		public Boolean _isFullScreen = true;
		public Int32 _resolutionWidth = 1920, _resolutionHeight = 1080;

		Boolean _isInitServer = false;
		RobotServer _robotServer = null;

		/// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>
		void Awake () {

			Debug.Log ("Enter Porject's Boot!");

			Instance = this;

			DontDestroyOnLoad (this.gameObject);
		}

		// Use this for initialization
		void Start () {

			//PS: First Initialize Table & Audio (Cost Long Time), and so on, Second Initialize Server
			//Initialize Table
			//Setting
			UnitySettings ();

			//Initialize Audio
			AudioManager.Instance.Initialize (null , () => {

				//Initialie Server
				_robotServer = new RobotServer (Initialize, () => { Debug.LogError ("Create Server Error!"); }, ParseReceiveData);
			});
		}

		// Update is called once per frame
		void Update () {
			if (_robotServer != null) {
				_robotServer.ParseReceive ();
			}
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		void OnDestroy () {
			if (_robotServer != null) {
				_robotServer.OnDestory ();
			}
		}

		/// <summary>
		/// Unity Settings
		/// </summary>
		void UnitySettings () {
			Screen.SetResolution (_resolutionWidth, _resolutionHeight, true);

			QualitySettings.SetQualityLevel (5, true);
			QualitySettings.antiAliasing = 32;
			QualitySettings.shadows = ShadowQuality.Disable;

			Debug.Log ("Current Quality Settings Level: " + QualitySettings.GetQualityLevel ());
		}

		/// <summary>
		/// Unity Play Action Info Send To Client
		/// </summary>
		void SendPlayInfo (String actionType, String sn, String detail) {
			JsonData totalJson = new JsonData (), dataJson = new JsonData ();
			dataJson[CamelNameConst.ACTION_TYPE] = actionType;
			dataJson[CamelNameConst.SN] = sn;
			dataJson[CamelNameConst.DETAIL] = detail;

			totalJson[UpperNameConst.MODULE] = PascalNameConst.SHOW;
			totalJson[UpperNameConst.DATA] = dataJson;

			Send (totalJson.ToJson ());
		}

		/// <summary>
		/// Server Send Message
		/// </summary>
		public void Send (String message) {
			if (_isInitServer) {
				if (_robotServer != null) {
					_robotServer.SendData (message);
				} else {
					Debug.LogError ("Server Not Initialize");
				}
			} else {
				Debug.LogError ("Server Not Initialize Success");
			}
		}

		/// <summary>
		/// Initialize Info
		/// </summary>
		void Initialize () {
			_isInitServer = true;

			//Continue
			Debug.Log ("Init Success!");
		}

		/// <summary>
		/// Parse Receive Data
		/// </summary>
		void ParseReceiveData (String module, String dataInfo) {
			Debug.Log (System.DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss:ffff") + "Parse Receive Data >> Module: " + module + " | Data: " + dataInfo);

		}

	} //end class
} //end namespace