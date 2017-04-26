using Glow11.Blur;
using System;
using UnityEngine;
namespace Glow11
{
	[AddComponentMenu("Glow 11"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
	public class Glow11 : MonoBehaviour
	{
		[SerializeField]
		private LayerMask cullingMask = -1;
		private Camera glowCam;
		private GameObject glowObj;
		private BaseGlowCamera ActiveGlow;
		private bool needsReinitialisation = false;
		[SerializeField]
		private BlurMode _blurMode = BlurMode.Default;
		[SerializeField]
		private bool _reuseDepth = false;
		private bool reuseDepthDisabled = false;
		private bool useRt = false;
		[SerializeField]
		private Resolution _rerenderResolution = Resolution.Full;
		private bool _highPrecsionActive = false;
		[SerializeField]
		private bool _highPrecsion = false;
		public Settings settings;
		public BlurMode blurMode
		{
			get
			{
				return this._blurMode;
			}
			set
			{
				this._blurMode = value;
				this.needsReinitialisation = true;
			}
		}
		public bool reuseDepth
		{
			get
			{
				return this._reuseDepth;
			}
			set
			{
				this._reuseDepth = value;
				this.needsReinitialisation = true;
			}
		}
		public Resolution rerenderResolution
		{
			get
			{
				return this._rerenderResolution;
			}
			set
			{
				this._rerenderResolution = value;
				this.needsReinitialisation = true;
			}
		}
		public bool highPrecisionActive
		{
			get
			{
				return this._highPrecsionActive;
			}
		}
		public bool highPrecision
		{
			get
			{
				return this._highPrecsion;
			}
			set
			{
				this._highPrecsion = value;
				this._highPrecsionActive = (value && this.supportsHighPrecision);
				if (this.ActiveGlow)
				{
					this.ActiveGlow.highPrecision = this._highPrecsionActive;
				}
			}
		}
		public bool supportsHighPrecision
		{
			get
			{
				return SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
			}
		}
		private void OnDestroy()
		{
			if (this.glowCam)
			{
				if (Application.isEditor)
				{
					UnityEngine.Object.DestroyImmediate(this.glowObj);
				}
				else
				{
					UnityEngine.Object.Destroy(this.glowObj);
				}
			}
		}
		private void Awake()
		{
			this.glowObj = new GameObject();
			this.glowObj.name = "You should never see me";
			this.glowObj.hideFlags = HideFlags.HideAndDontSave;
			this.glowCam = this.glowObj.AddComponent<Camera>();
			this.glowCam.cullingMask = cullingMask;
			this.glowCam.enabled = false;
			if (this.settings == null)
			{
				this.settings = new Settings();
			}
			this.highPrecision = this._highPrecsion;
		}
		private void OnDisable()
		{
			this.DestroyCamera();
		}
		private void OnEnable()
		{
			this.InitCamera();
		}
		private void DestroyCamera()
		{
			if (Application.isEditor)
			{
				UnityEngine.Object.DestroyImmediate(this.ActiveGlow);
			}
			else
			{
				UnityEngine.Object.Destroy(this.ActiveGlow);
			}
			this.glowObj.SetActive(false);
		}
		private void AutoDisable()
		{
			Debug.LogWarning("The image effect " + this.ToString() + " has been disabled as it's not supported on the current platform.");
			base.enabled = false;
		}
		internal bool CheckSupport()
		{
			if (!SystemInfo.supportsImageEffects || SystemInfo.graphicsShaderLevel < 20)
			{
				this.AutoDisable();
				return false;
			}
			return true;
		}
		public void InitCamera()
		{
			this.needsReinitialisation = false;
			if (this.ActiveGlow)
			{
				this.DestroyCamera();
			}
			this.glowObj.SetActive(true);
			IBlur blur = null;
			this.useRt = false;
			BlurMode blurMode = this.blurMode;
			if (blurMode != BlurMode.Default)
			{
				if (blurMode != BlurMode.Advanced)
				{
					if (blurMode != BlurMode.HighQuality)
					{
						if (blurMode == BlurMode.UnityBlur)
						{
							blur = new UnityBlur();
						}
					}
					else
					{
						blur = new HqBlur();
					}
				}
				else
				{
					blur = new AdvancedBlur();
				}
			}
			else
			{
				blur = new DefaultBlur();
			}
			this.glowCam.enabled = false;
			if (this._reuseDepth && QualitySettings.antiAliasing == 0)
			{
				this.reuseDepthDisabled = false;
				this.ActiveGlow = this.glowCam.gameObject.AddComponent<GlowCameraReuse>();
			}
			else
			{
				this.reuseDepthDisabled = true;
				this.useRt = true;
				this.ActiveGlow = this.glowCam.gameObject.AddComponent<GlowCameraRerenderOnly>();
			}
			this.ActiveGlow.glow11 = this;
			this.ActiveGlow.cullingMask = this.cullingMask;
			this.ActiveGlow.highPrecision = this._highPrecsionActive;
			this.ActiveGlow.parentCamera = base.GetComponent<Camera>();
			this.ActiveGlow.blur = blur;
			this.ActiveGlow.settings = this.settings;
			this.ActiveGlow.Init();
		}
		private void Update()
		{
			if (!this.CheckSupport())
			{
				return;
			}
			if (this.reuseDepthDisabled && this.reuseDepth && QualitySettings.antiAliasing == 0)
			{
				this.needsReinitialisation = true;
			}
			else
			{
				if (!this.reuseDepthDisabled && QualitySettings.antiAliasing != 0)
				{
					this.needsReinitialisation = true;
				}
				else
				{
					if (!this.useRt && (QualitySettings.antiAliasing != 0 || (this.reuseDepthDisabled && this._rerenderResolution != Resolution.Full)))
					{
						this.needsReinitialisation = true;
					}
				}
			}
			if (this.needsReinitialisation)
			{
				this.InitCamera();
			}
		}
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (this.useRt)
			{
				RenderTexture temporary = RenderTexture.GetTemporary((int)base.GetComponent<Camera>().pixelWidth / (int)this._rerenderResolution, (int)base.GetComponent<Camera>().pixelHeight / (int)this._rerenderResolution, 16);
				this.glowCam.targetTexture = temporary;
				this.glowCam.Render();
				source.MarkRestoreExpected();
				temporary.MarkRestoreExpected();
				RenderTexture.active = null;
				this.ActiveGlow.blur.BlurAndBlitBuffer(temporary, source, this.settings, this._highPrecsionActive);
				Graphics.Blit(source, destination);
				RenderTexture.ReleaseTemporary(temporary);
			}
			else
			{
				this.ActiveGlow.screenRt = source;
				this.glowCam.Render();
				Graphics.Blit(source, destination);
			}
		}
		private void OnPostRender()
		{
			if (this.CheckSupport())
			{
				if (!this.ActiveGlow)
				{
					this.InitCamera();
				}
				return;
			}
		}
	}
}
