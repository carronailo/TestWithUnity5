using Glow11.Blur;
using System;
using UnityEngine;
namespace Glow11
{
	[AddComponentMenu(""), ExecuteInEditMode]
	internal class BaseGlowCamera : MonoBehaviour
	{
		internal IBlur _blur;
		public Camera parentCamera;
		public LayerMask cullingMask;
		public Settings settings;
		public Glow11 glow11;
		public RenderTexture screenRt;
		public bool highPrecision;
		private Shader _glowOnly;
		internal IBlur blur
		{
			get
			{
				return this._blur;
			}
			set
			{
				this._blur = value;
			}
		}
		protected Shader glowOnly
		{
			get
			{
				if (!this._glowOnly)
				{
					this._glowOnly = Shader.Find("Hidden/Glow 11/GlowObjects");
				}
				return this._glowOnly;
			}
		}
		internal virtual void Init()
		{
		}

		internal virtual void OnDestroy()
		{
			if(_blur != null)
				_blur.Release();
		}
	}
}
