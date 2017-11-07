using System;
using UnityEngine;
namespace Glow11
{
	[AddComponentMenu("")]
	internal class GlowCameraRerender : BaseGlowCamera
	{
		private void OnPreCull()
		{
			base.GetComponent<Camera>().CopyFrom(this.parentCamera);
			base.GetComponent<Camera>().cullingMask = this.cullingMask;
			base.GetComponent<Camera>().backgroundColor = Color.black;
			base.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
			base.GetComponent<Camera>().SetReplacementShader(base.glowOnly, "RenderType");
			base.GetComponent<Camera>().renderingPath = RenderingPath.VertexLit;
			base.GetComponent<Camera>().depth = this.parentCamera.depth + 0.1f;
		}
		private void OnPreRender()
		{
			base.GetComponent<Camera>().projectionMatrix = this.parentCamera.projectionMatrix;
		}
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			base.blur.BlurAndBlitBuffer(source, destination, this.settings, this.highPrecision);
		}
	}
}
