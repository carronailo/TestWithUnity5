using System;
using UnityEngine;
namespace Glow11.Blur
{
	internal class UnityBlur : IBlur
	{
		private Material material = null;
		private Material composeMaterial = null;
		internal UnityBlur()
		{
			this.material = new Material(Shader.Find("Hidden/Glow 11/BlurEffectConeTap"));
			this.composeMaterial = new Material(Shader.Find("Hidden/Glow 11/Compose"));
		}
		private void FourTapCone(RenderTexture source, RenderTexture dest, int iteration, float blurSpread)
		{
			float num = 0.5f + (float)iteration * blurSpread;
			Graphics.BlitMultiTap(source, dest, this.material, new Vector2[]
			{
				new Vector2(-num, -num),
				new Vector2(-num, num),
				new Vector2(num, num),
				new Vector2(num, -num)
			});
		}
		private void DownSample4x(RenderTexture source, RenderTexture dest)
		{
			float num = 1f;
			Graphics.BlitMultiTap(source, dest, this.material, new Vector2[]
			{
				new Vector2(-num, -num),
				new Vector2(-num, num),
				new Vector2(num, num),
				new Vector2(num, -num)
			});
		}
		public void BlurAndBlitBuffer(RenderTexture source, RenderTexture destination, Settings settings, bool highPrecision)
		{
			this.material.SetFloat("_Strength", settings.innerStrength / 4f);
			this.composeMaterial.SetFloat("_Strength", settings.boostStrength);
			RenderTexture temporary = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0, (!highPrecision) ? RenderTextureFormat.Default : RenderTextureFormat.ARGBHalf);
			RenderTexture temporary2 = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0, (!highPrecision) ? RenderTextureFormat.Default : RenderTextureFormat.ARGBHalf);
			this.DownSample4x(source, temporary);
			bool flag = true;
			for (int i = 0; i < settings.iterations; i++)
			{
				if (flag)
				{
					temporary2.MarkRestoreExpected();
					this.FourTapCone(temporary, temporary2, i, settings.blurSpread);
				}
				else
				{
					temporary.MarkRestoreExpected();
					this.FourTapCone(temporary2, temporary, i, settings.blurSpread);
				}
				flag = !flag;
			}
			if (flag)
			{
				Graphics.Blit(temporary, destination, this.composeMaterial);
			}
			else
			{
				Graphics.Blit(temporary2, destination, this.composeMaterial);
			}
			RenderTexture.ReleaseTemporary(temporary);
			RenderTexture.ReleaseTemporary(temporary2);
		}

		public void Release()
		{
			if (material != null)
				UnityEngine.Object.DestroyImmediate(material);
			if (composeMaterial != null)
				UnityEngine.Object.DestroyImmediate(composeMaterial);
		}
	}
}
