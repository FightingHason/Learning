//*************************************************
//File:		GifPlayer.cs
//
//Brief:    Play Gif
//
//Author:   Liuhaixia
//
//E-Mail:   609043941@qq.com
//
//History:  2017/9/20 Created by Liuhaixia
//*************************************************
using System.Collections.Generic;
using UnityEngine;

namespace Ben {
	public class GifPlayer : MonoBehaviour {

		public int Fps = 15;
		public bool IsUI = true;

		float _time = 0F;

		int _lastIndex = 0, _index = 0;

		UnityEngine.UI.Image _displayUIImage;

		Material _renderMaterial;

		List<Texture2D> _tex2DList;

		/// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>
		void Awake () {
			if (IsUI) {
				_displayUIImage = this.gameObject.GetComponent<UnityEngine.UI.Image> ();
			} else {
				Renderer objectRender = this.gameObject.GetComponent<Renderer> ();
				if (objectRender != null) {
					for (int i = 0; i < objectRender.materials.Length; ++i) {
						_renderMaterial = objectRender.materials[0];
					}
				}
			}
		}

		// Use this for initialization
		void Start () {

			_tex2DList = GifToTexture2D (System.IO.Path.Combine (Application.streamingAssetsPath, "Gif/001.gif"));

			SetDisplay ();
		}

		// Update is called once per frame
		void Update () {
			_time += Time.deltaTime;
			_index = (int) (_time * Fps) % _tex2DList.Count;
			if (_lastIndex != _index) {
				SetDisplay ();
			}
		}

		/// <summary>
		/// Display gif
		/// </summary>
		void SetDisplay () {
			if (IsUI) {
				SetDisplayUIImage ();
			} else {
				SetDisplayObjectRender ();
			}
		}

		/// <summary>
		/// Set Display UI Image
		/// </summary>
		void SetDisplayUIImage () {
			if (null != _displayUIImage && _tex2DList.Count > 0) {
				_displayUIImage.sprite = Sprite.Create (_tex2DList[_index], new Rect (0, 0, _tex2DList[_index].width, _tex2DList[_index].height), Vector2.zero);

				_lastIndex = _index;
			}
		}

		/// <summary>
		/// Set Display GameObject Render
		/// </summary>
		void SetDisplayObjectRender () {
			if (null != _renderMaterial && _tex2DList.Count > 0) {
				_renderMaterial.mainTexture = _tex2DList[_index];

				_lastIndex = _index;
			}
		}

		/// <summary>
		/// Gif to textures list
		/// </summary>
		List<Texture2D> GifToTexture2D (string path) {
			List<Texture2D> tex2DList = new List<Texture2D> ();
			try {
				//get Image
				System.Drawing.Image imgGif = System.Drawing.Image.FromFile (path);
				//gif type or not
				if (System.Drawing.ImageAnimator.CanAnimate (imgGif)) {
					System.Drawing.Imaging.FrameDimension imgFrameDimension = new System.Drawing.Imaging.FrameDimension (imgGif.FrameDimensionsList[0]);
					//get frame
					int frameCount = imgGif.GetFrameCount (imgFrameDimension);
					for (int i = 0; i < frameCount; ++i) {
						//push frame data to texture
						imgGif.SelectActiveFrame (imgFrameDimension, i);
						//bitmap
						System.Drawing.Bitmap frameBitmap = new System.Drawing.Bitmap (imgGif.Width, imgGif.Height);
						//create new graphics draw to Bitmap
						using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage (frameBitmap)) {
							graphics.DrawImage (imgGif, System.Drawing.Point.Empty);
						}

						//method1
						tex2DList.Add (FrameTexture2DByBytes (frameBitmap));
						//method2
						//tex2DList.Add(FrameTexture2DByPixel(frameBitmap));
					}
				}
			} catch (System.Exception ex) {
				Debug.LogError ("Gif to Textures Exception: " + ex.Message);
			}

			return tex2DList;
		}

		/// <summary>
		/// Frame gif image to data bytes
		/// </summary>
		Texture2D FrameTexture2DByBytes (System.Drawing.Bitmap frameBitmap) {
			byte[] dataBytes;
			//save png bytes
			using (System.IO.MemoryStream stream = new System.IO.MemoryStream ()) {
				//save as png
				frameBitmap.Save (stream, System.Drawing.Imaging.ImageFormat.Png);
				dataBytes = new byte[stream.Length];
				//reset point
				stream.Seek (0, System.IO.SeekOrigin.Begin);
				//push
				stream.Read (dataBytes, 0, System.Convert.ToInt32 (stream.Length));
			}

			Texture2D tex2D = new Texture2D (frameBitmap.Width, frameBitmap.Height, TextureFormat.ARGB32, false);
			tex2D.LoadImage (dataBytes);
			return tex2D;
		}

		/// <summary>
		/// Frame gif image to texture2D
		/// </summary>
		Texture2D FrameTexture2DByPixel (System.Drawing.Bitmap frameBitmap) {
			Texture2D tex2D = new Texture2D (frameBitmap.Width, frameBitmap.Height, TextureFormat.ARGB32, false);
			//set texture2D by pixel color
			for (int x = 0; x < frameBitmap.Width; ++x) {
				for (int y = 0; y < frameBitmap.Height; ++y) {
					System.Drawing.Color sourceColor = frameBitmap.GetPixel (x, y);
					tex2D.SetPixel (x, frameBitmap.Height - 1 - y, new Color32 (sourceColor.R, sourceColor.G, sourceColor.B, sourceColor.A));
				}
			}

			tex2D.Apply ();
			return tex2D;
		}

	} //end class
} //end namespace