using System;
using UnityEngine;
namespace Glow11
{
	[AddComponentMenu(""), ExecuteInEditMode]
	internal class GlowCameraReuse : BaseGlowCamera
	{
		private RenderTexture tmpRt;
		private void ActivateHelper()
		{
		}
		private void OnEnable()
		{
			this.ActivateHelper();
		}
		private void OnPreCull()
		{
			base.GetComponent<Camera>().CopyFrom(this.parentCamera);
			base.GetComponent<Camera>().cullingMask = this.cullingMask;
			base.GetComponent<Camera>().backgroundColor = Color.black;
			base.GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
			base.GetComponent<Camera>().SetReplacementShader(base.glowOnly, "RenderEffect");
			base.GetComponent<Camera>().renderingPath = RenderingPath.VertexLit;
			this.tmpRt = RenderTexture.GetTemporary(this.screenRt.width, this.screenRt.height);
			RenderTexture.active = this.tmpRt;
			GL.Clear(false, true, Color.black);
			base.GetComponent<Camera>().targetTexture = this.tmpRt;
			this.screenRt.MarkRestoreExpected();
			base.GetComponent<Camera>().SetTargetBuffers(this.tmpRt.colorBuffer, this.screenRt.depthBuffer);
		}
		private void OnPreRender()
		{
			base.GetComponent<Camera>().projectionMatrix = this.parentCamera.projectionMatrix;
		}
		private void OnPostRender()
		{
			this.screenRt.MarkRestoreExpected();
			base.blur.BlurAndBlitBuffer(this.tmpRt, this.screenRt, this.settings, this.highPrecision);
			RenderTexture.ReleaseTemporary(this.tmpRt);
		}
	}
}
