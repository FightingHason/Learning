//*************************************************
//File:		StopWatchUtils.cs
//
//Brief:    StopWatch Utils
//
//Author:   Liuhaixia
//
//E-Mail:   609043941@qq.com
//
//History:  2017/04/12 Created by Liuhaixia
//*************************************************
using System;
using System.Diagnostics;
namespace Ben {
    public class StopWatchUtils {

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Singleton

        static StopWatchUtils _instance = null;
        public static StopWatchUtils Inst {
            get {
                if (_instance == null) {
                    _instance = new StopWatchUtils ();
                }
                return _instance;
            }
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        Stopwatch _stopwatch = null;

        StopWatchUtils () {
            _stopwatch = new Stopwatch ();
        }

        public void Start () {
            _stopwatch.Reset ();
            _stopwatch.Start ();
        }

        public void Stop (String tag) {
            _stopwatch.Stop ();
            if (tag.IsNullOrEmpty ()) {
                UnityEngine.Debug.Log ("StopWatch Print ElapsedMillseconds: " + _stopwatch.ElapsedMilliseconds);
            } else {
                UnityEngine.Debug.Log ("Tag: " + tag + " | StopWatch Print ElapsedMillseconds: " + _stopwatch.ElapsedMilliseconds);
            }
        }

        public void StopAndStart (String tag) {
            Stop (tag);
            Start ();
        }

        //public long ElapseTicks()
        //{
        //    return _stopwatch.ElapsedTicks;
        //}

    }

} //end namespace