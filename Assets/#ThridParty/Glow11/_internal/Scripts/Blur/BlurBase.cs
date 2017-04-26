using System;
using UnityEngine;
namespace Glow11.Blur
{
	internal abstract class BlurBase
	{
		protected Material blurMaterial = null;
		protected Material composeMaterial = null;
		protected Material downsampleMaterial = null;
		private bool glMode = true;
		protected Vector2[] blurOffsetsHorizontal = new Vector2[]
		{
			new Vector2(0f, 0f),
			new Vector2(-1.38461542f, 0f),
			new Vector2(1.38461542f, 0f),
			new Vector2(-3.23076916f, 0f),
			new Vector2(3.23076916f, 0f)
		};
		protected Vector2[] blurOffsetsVertical = new Vector2[]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, -1.38461542f),
			new Vector2(0f, 1.38461542f),
			new Vector2(0f, -3.23076916f),
			new Vector2(0f, 3.23076916f)
		};
		public BlurBase()
		{
			Shader shader = Shader.Find("Hidden/Glow 11/Compose");
			Shader shader2 = Shader.Find("Hidden/Glow 11/Blur GL");
			if (!shader2.isSupported)
			{
				this.glMode = false;
				shader2 = Shader.Find("Hidden/Glow 11/Blur");
			}
			Shader shader3 = Shader.Find("Hidden/Glow 11/Downsample");
			this.composeMaterial = new Material(shader);
			this.composeMaterial.hideFlags = HideFlags.DontSave;
			this.blurMaterial = new Material(shader2);
			this.blurMaterial.hideFlags = HideFlags.DontSave;
			this.downsampleMaterial = new Material(shader3);
			this.downsampleMaterial.hideFlags = HideFlags.DontSave;
		}
		protected void BlurBuffer(RenderTexture buffer, RenderTexture buffer2)
		{
			buffer2.DiscardContents();
			if (this.glMode)
			{
				Graphics.BlitMultiTap(buffer, buffer2, this.blurMaterial, this.blurOffsetsHorizontal);
				buffer.DiscardContents();
				Graphics.BlitMultiTap(buffer2, buffer, this.blurMaterial, this.blurOffsetsVertical);
			}
			else
			{
				this.blurMaterial.SetFloat("_offset1", 1.38461542f);
				this.blurMaterial.SetFloat("_offset2", 3.23076916f);
				Graphics.Blit(buffer, buffer2, this.blurMaterial, 0);
				buffer.DiscardContents();
				Graphics.Blit(buffer2, buffer, this.blurMaterial, 1);
			}
		}

		protected virtual void ReleaseMaterial()
		{
			if (blurMaterial != null)
				UnityEngine.Object.DestroyImmediate(blurMaterial);
			if (composeMaterial != null)
				UnityEngine.Object.DestroyImmediate(composeMaterial);
			if (downsampleMaterial != null)
				UnityEngine.Object.DestroyImmediate(downsampleMaterial);
		}
	}
}
