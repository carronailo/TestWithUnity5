using System;
using UnityEngine;
namespace Glow11
{
	[AddComponentMenu("")]
	internal class GlowCameraRerenderOnly : BaseGlowCamera
	{
		private void OnPreCull()
		{
			RenderTexture targetTexture = base.GetComponent<Camera>().targetTexture;
			base.GetComponent<Camera>().CopyFrom(this.parentCamera);
			base.GetComponent<Camera>().cullingMask = this.cullingMask;
			base.GetComponent<Camera>().backgroundColor = Color.black;
			base.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
			base.GetComponent<Camera>().SetReplacementShader(base.glowOnly, "RenderType");
			base.GetComponent<Camera>().renderingPath = RenderingPath.VertexLit;
			base.GetComponent<Camera>().targetTexture = targetTexture;
		}
		private void OnPreRender()
		{
			base.GetComponent<Camera>().projectionMatrix = this.parentCamera.projectionMatrix;
		}
	}
}
