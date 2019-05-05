//*************************************************
//File:		CachedMonoBehaviour.cs
//
//Brief:    Cache MonoBehaviour
//
//Author:   Liuhaixia
//
//E-Mail:   609043941@qq.com
//
//History:  2017/12/22 Created by Liuhaixia
//*************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ben {
    public class CachedMonoBehaviour : MonoBehaviour {

        public GameObject CachedGameObject {
            get {
                if (m_CachedGameObj == null)
                    m_CachedGameObj = this.gameObject;
                return m_CachedGameObj;
            }
        }

        public Transform CachedTransform {
            get {
                if (m_CachedTransform == null)
                    m_CachedTransform = this.transform;
                return m_CachedTransform;
            }
        }

        public T GetCachedComponent<T> () where T : UnityEngine.Component {
            System.Type t = typeof (T);
            UnityEngine.Component ret;
            if (_cachedCompentMap == null || !_cachedCompentMap.TryGetValue (t, out ret)) {
                GameObject gameObj = CachedGameObject;
                if (gameObj == null)
                    return null;

                if (m_CahcedCompentInitMap != null) {
                    if (m_CahcedCompentInitMap.Contains (t))
                        return null;
                }

                if (m_CahcedCompentInitMap == null)
                    m_CahcedCompentInitMap = new HashSet<System.Type> ();
                m_CahcedCompentInitMap.Add (t);

                T target = gameObj.GetComponent<T> ();
                if (target == null)
                    return null;
                CheckCachedCompentMap ();
                _cachedCompentMap.Add (t, target);
                return target;
            }

            T comp = ret as T;
            return comp;
        }

        void CheckCachedCompentMap () {
            if (_cachedCompentMap == null)
                _cachedCompentMap = new Dictionary<System.Type, Component> ();
        }

        Dictionary<System.Type, UnityEngine.Component> _cachedCompentMap = null;
        HashSet<System.Type> m_CahcedCompentInitMap = null;
        GameObject m_CachedGameObj = null;
        Transform m_CachedTransform = null;
    }
}