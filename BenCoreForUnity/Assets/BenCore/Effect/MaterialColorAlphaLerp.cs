//*************************************************
//File:		MaterialColorAlphaLerp.cs
//
//Brief:    Material Color Alpha Lerp
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
using UnityEngine;

namespace Ben {
	public class MaterialColorAlphaLerp : MonoBehaviour {

		const String COLOR_PROPERTY_NAME = "_TintColor";

		public Int32 _speed = 1;

		Boolean _directionMinColor = true;

		Single _calcTime = 0F;

		Color _minColor, _maxColor;

		Material _colorMaterial = null;

		/// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>
		void Awake () {
			Renderer render = this.gameObject.GetComponent<Renderer> ();
			if (render != null) {
				Material[] materials = render.sharedMaterials;
				for (Int32 i = 0; i < materials.Length;) {
					_colorMaterial = materials[i];
					break;
				}
			}
		}

		// Use this for initialization
		void Start () {
			if (_colorMaterial != null) {
				Color color = _colorMaterial.GetColor (COLOR_PROPERTY_NAME);
				_minColor = new Color (color.r, color.g, color.b, 0);
				_maxColor = new Color (color.r, color.g, color.b, 1);

				_colorMaterial.SetColor (COLOR_PROPERTY_NAME, _maxColor);

				_directionMinColor = true;
			}
		}

		// Update is called once per frame
		void Update () {
			if (_colorMaterial != null) {
				Color color = _colorMaterial.GetColor (COLOR_PROPERTY_NAME);
				if (_directionMinColor) {
					_calcTime += Time.deltaTime;
					color = Color.Lerp (color, _minColor, _calcTime);
					_colorMaterial.SetColor (COLOR_PROPERTY_NAME, color);
					if (color == _minColor) {
						_calcTime = 0;
						_directionMinColor = false;
					}
				} else {
					_calcTime += Time.deltaTime;
					color = Color.Lerp (color, _maxColor, _calcTime);
					_colorMaterial.SetColor (COLOR_PROPERTY_NAME, color);
					if (color == _maxColor) {
						_calcTime = 0;
						_directionMinColor = true;
					}
				}
			}
		}
	}
} //end namespace