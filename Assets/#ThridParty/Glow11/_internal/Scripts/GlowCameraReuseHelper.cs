using System;
using UnityEngine;
namespace Glow11
{
	[AddComponentMenu(""), ExecuteInEditMode]
	internal class GlowCameraReuseHelper : MonoBehaviour
	{
		internal GlowCameraReuse glowCam = null;
		private void OnPreCull()
		{
			if (!this.glowCam)
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(this);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(this);
				}
			}
		}
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (!this.glowCam)
			{
				this.OnPreCull();
				return;
			}
			if (!this.glowCam.glow11.CheckSupport())
			{
				return;
			}
			this.glowCam.screenRt = source;
			this.glowCam.GetComponent<Camera>().Render();
			Graphics.Blit(source, destination);
		}
	}
}
