//*************************************************
//File:		AudioManager.cs
//
//Brief:    Audio MonoBehaviour Manager
//
//Author:   Liuhaixia
//
//E-Mail:   609043941@qq.com
//
//History:  2017/12/14 Created by Liuhaixia
//*************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Ben {
    public class AudioManager : MonoBehaviour {

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Singleton

        static AudioManager _instance = null;
        public static AudioManager Instance {
            get {
                if (_instance == null) {
                    _instance = new GameObject (typeof (AudioManager).ToString (), typeof (AudioManager)).GetComponent<AudioManager> ();

                    DontDestroyOnLoad (_instance);
                }
                return _instance;
            }
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        const Int32 SOUND_EFFECT_COUNT = 2;
        const Single MUSIC_TRANSFER_TIME = 0.04f;

        delegate void VolumeChangeEvent ();

        AudioSource _bgmAudioSource;
        List<AudioSource> _sfxAudioSourceList = new List<AudioSource> ();

        HashSet<String> _allAudioList = new HashSet<String> ();
        Dictionary<String, Single> _lastPlayTimeDict = new Dictionary<String, Single> ();
        Dictionary<String, AudioClip> _sfxClipDict = new Dictionary<String, AudioClip> ();

        Boolean isLoaded = false;
        Boolean isMusicTransfer = false;

        public void Initialize (HashSet<String> allAudioList, Action callBack) {

            if (allAudioList != null) {
                _allAudioList = allAudioList;
            }

            this.gameObject.AddComponent<AudioListener> ();

            _bgmAudioSource = gameObject.AddComponent<AudioSource> ();

            for (Int32 i = 0; i < SOUND_EFFECT_COUNT; ++i) {
                _sfxAudioSourceList.Add (gameObject.AddComponent<AudioSource> ());
            }

            StartCoroutine (LoadSFXList (callBack));
        }

        /// <summary>
        /// Load SFX Audio List
        /// </summary>
        IEnumerator LoadSFXList (Action callBack) {

            yield return 0;

            foreach (var item in _allAudioList) {

                if (false == _lastPlayTimeDict.ContainsKey (item)) {
                    _lastPlayTimeDict.Add (item, 0);
                }

                //方案一
                String audioPath = PathUtils.GetAudioPath (item + GlobalConst.TB_TYPE_WAV);
                WWW www = new WWW (audioPath);
                yield return www;

                if (www.error == null) {
                    AudioClip clip = www.audioClip;
                    if (clip) {
                        if (false == _sfxClipDict.ContainsKey (item)) {
                            _sfxClipDict.Add (item, clip);
                        }
                    } else {
                        Debug.Log ("Object is null");
                    }
                } else {
                    Debug.LogError ("WWW Load Fail: " + www.error);
                }

                www.Dispose ();
                www = null;

                //方案二 Resources Load AudioClip
                //yield return null;
                //AudioClip ac = Resources.Load(CommonConst.DIR_AUDIO + soundList[i], typeof(AudioClip)) as AudioClip;
                //ac.name = soundList[i].ToString();
                //_sfxClipDict.Add(soundList[i], ac);
            }

            isLoaded = true;

            if (callBack != null) {
                callBack ();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Sound

        public void PlaySound (String index, Boolean isLoop = false, Single waitTime = 0.0F) {
            if (!isLoaded)
                return;

            String audioName = GetAudioFullName (index);

            if (!_sfxClipDict.ContainsKey (audioName)) {
                //Debug.LogError ("Not Exist This Audio");
                return;
            }

            StopSound ();

            if (CheckLastPlayTime (audioName, waitTime)) {
                StartPlaySound (audioName, isLoop);
            }
        }

        public void StopSound () {
            for (Int32 i = 0; i < _sfxAudioSourceList.Count; ++i) {
                if (_sfxAudioSourceList[i].isPlaying) {
                    _sfxAudioSourceList[i].Stop ();
                    break;
                }
            }
        }

        public void StopSound (String index) {
            String audioName = GetAudioFullName (index);

            if (!_sfxClipDict.ContainsKey (audioName)) {
                Debug.LogError ("Not Exist This Audio");
                return;
            }

            for (Int32 i = 0; i < _sfxAudioSourceList.Count; ++i) {
                if (_sfxAudioSourceList[i].isPlaying && _sfxAudioSourceList[i].clip == GetAudioClip (audioName)) {
                    _sfxAudioSourceList[i].Stop ();
                    break;
                }
            }
        }

        public Boolean IsPlaySound (String index, Single waitTime) {
            String audioName = GetAudioFullName (index);

            if (!_sfxClipDict.ContainsKey (audioName)) {
                Debug.LogError ("Not Exist This Audio");
                return false;
            }

            if (Time.time - _lastPlayTimeDict[audioName] > waitTime) {
                return true;
            } else return false;
        }

        void StartPlaySound (String audioName, Boolean isLooping) {
            AudioSource source = GetUnusedAudioSource ();
            if (source != null) {
                source.loop = isLooping;
                source.clip = GetAudioClip (audioName);
                source.Play ();
            } else
                Debug.LogError ("AudioManager<GetAudioClip> No Exist AudioSource!=>AudioName: " + audioName);
        }

        AudioClip GetAudioClip (String audioName) {
            try {
                return _sfxClipDict[audioName];
            } catch (System.Exception ex) {
                Debug.LogError ("AudioManager<GetAudioClip>=>AudioName: " + audioName + " Exception: " + ex.ToString ());
                return null;
            }
        }

        Boolean CheckLastPlayTime (String audioName, Single waitTime) {
            if (!_sfxClipDict.ContainsKey (audioName)) {
                Debug.LogError ("Not Exist This Audio");
                return false;
            }

            if (Time.time - _lastPlayTimeDict[audioName] > waitTime) {
                _lastPlayTimeDict[audioName] = Time.time;
                return true;
            } else return false;
        }

        AudioSource GetUnusedAudioSource () {
            foreach (AudioSource source in _sfxAudioSourceList) {
                if (!source.isPlaying)
                    return source;
            }

            //Debug.LogError("No Available AudioSource To Play Sound!");
            return null;
        }

        /// <summary>
        /// 获取音效全名
        /// </summary>
        String GetAudioFullName (String index) {
            return "se_" + index;
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Music

        public void ChangeBGMVolume (Single volume) {
            StartCoroutine (BGMVolumeTransfer (volume));
        }

        IEnumerator BGMVolumeTransfer (Single targetVolume, VolumeChangeEvent eventType = null) {
            isMusicTransfer = true;
            if (_bgmAudioSource.volume > targetVolume) {
                while (_bgmAudioSource.volume > targetVolume) {
                    _bgmAudioSource.volume -= MUSIC_TRANSFER_TIME;
                    yield return new WaitForEndOfFrame ();
                }
            } else {
                while (_bgmAudioSource.volume < targetVolume) {
                    _bgmAudioSource.volume += MUSIC_TRANSFER_TIME;
                    yield return new WaitForEndOfFrame ();
                }
            }

            _bgmAudioSource.pitch = 1;
            _bgmAudioSource.volume = targetVolume;
            yield return new WaitForEndOfFrame ();

            if (eventType != null) eventType ();
            isMusicTransfer = false;
        }

        public void PlayMusic (String index, Int32 count) {
            //String audioName = GetAudioFullName(index);

            //if (!_sfxClipDict.ContainsKey(audioName))
            //{
            //    Debug.LogError("Not Exist This Audio");
            //    return;
            //}

            if (isMusicTransfer) {
                StopAllCoroutines ();
                isMusicTransfer = false;
            }
            StartCoroutine (MusicTransfer (index, count));
        }

        IEnumerator MusicTransfer (String audioName, Int32 count) {
            isMusicTransfer = true;

            while (_bgmAudioSource.volume > 0) {
                _bgmAudioSource.volume -= MUSIC_TRANSFER_TIME;
                yield return new WaitForEndOfFrame ();
            }

            if (_bgmAudioSource) {
                _bgmAudioSource.Stop ();

                if (_bgmAudioSource.clip != null)
                    _bgmAudioSource.clip.UnloadAudioData ();
            }
            yield return new WaitForEndOfFrame ();

            String audioPath = PathUtils.GetAudioPath (audioName + GlobalConst.TB_TYPE_WAV);
            WWW www = new WWW (audioPath);
            yield return www;

            if (www.error == null) {
                AudioClip clip = www.audioClip;
                //AudioClip clip = Resources.Load<AudioClip>(ConstUtils.DIR_AUDIO + audioName);
                www.Dispose ();

                if (clip) {
                    _bgmAudioSource.pitch = 1;
                    _bgmAudioSource.clip = clip;
                    _bgmAudioSource.volume = 0.0F;
                    _bgmAudioSource.loop = count == 1 ? false : true; //true;
                    _bgmAudioSource.Play ();
                }

                while (_bgmAudioSource.volume < 1.0F) {
                    _bgmAudioSource.volume += MUSIC_TRANSFER_TIME;
                    if (_bgmAudioSource.volume >= 1.0F) _bgmAudioSource.volume = 1.0F;
                    yield return new WaitForEndOfFrame ();
                }

                if (!_bgmAudioSource.isPlaying) {
                    StartCoroutine (BGMVolumeTransfer (1, () => { _bgmAudioSource.Play (); }));
                }
                isMusicTransfer = false;
            }
        }

        public void StopMusic () {
            if (_bgmAudioSource.isPlaying) {
                if (isMusicTransfer) {
                    StopAllCoroutines ();
                    isMusicTransfer = false;
                }
                StartCoroutine (BGMVolumeTransfer (0, () => {
                    _bgmAudioSource.Stop ();

                    if (_bgmAudioSource.clip != null)
                        _bgmAudioSource.clip.UnloadAudioData ();
                }));
            }
        }

        public void PauseMusic () {
            if (_bgmAudioSource.isPlaying) {
                if (isMusicTransfer) {
                    StopAllCoroutines ();
                    isMusicTransfer = false;
                }
                StartCoroutine (BGMVolumeTransfer (0, () => { _bgmAudioSource.Pause (); }));
            }
        }

        public void ResumeMusic () {
            if (!_bgmAudioSource.isPlaying) {
                StartCoroutine (BGMVolumeTransfer (1, () => { _bgmAudioSource.Play (); }));
            }
        }

        public void ErrorMusic () {
            if (_bgmAudioSource.isPlaying) {
                _bgmAudioSource.pitch = Mathf.Lerp (-3, 3, Time.time);
                StartCoroutine (BGMErrorPitch ());
            }
        }

        IEnumerator BGMErrorPitch () {
            if (_bgmAudioSource) {
                //_bgmAudioSource.pitch = 3;
                //yield return new WaitForSeconds(0.0001f);

                //_bgmAudioSource.pitch = -3;
                //yield return new WaitForSeconds(0.0001f);
                yield return new WaitForSeconds (1f);
                PauseMusic ();
            }
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        void OnDestroy () {
            Destroy (_bgmAudioSource);
            _bgmAudioSource = null;

            _sfxAudioSourceList.Clear ();
            _sfxClipDict.Clear ();
        }

    } //end class
} //end namespace