using System;
using UnityEngine;
namespace Glow11.Blur
{
	internal class AdvancedBlur : IBlur
	{
		protected Material blurMaterial = null;
		protected Material composeMaterial = null;
		protected Material downsampleMaterial = null;
		private Matrix4x4 offsetsMatrix;
		private Matrix4x4 weightsMatrix;
		public AdvancedBlur()
		{
			Shader shader = Shader.Find("Hidden/Glow 11/Compose");
			Shader shader2 = Shader.Find("Hidden/Glow 11/Advanced Blur");
			Shader shader3 = Shader.Find("Hidden/Glow 11/Downsample");
			this.composeMaterial = new Material(shader);
			this.composeMaterial.hideFlags = HideFlags.DontSave;
			this.blurMaterial = new Material(shader2);
			this.blurMaterial.hideFlags = HideFlags.DontSave;
			this.downsampleMaterial = new Material(shader3);
			this.downsampleMaterial.hideFlags = HideFlags.DontSave;
		}
		protected void BlurBuffer(RenderTexture buffer, RenderTexture buffer2, int passOffset)
		{
			this.blurMaterial.SetMatrix("_offsets", this.offsetsMatrix);
			this.blurMaterial.SetMatrix("_weights", this.weightsMatrix);
			buffer2.DiscardContents();
			Graphics.Blit(buffer, buffer2, this.blurMaterial, passOffset);
			buffer.DiscardContents();
			Graphics.Blit(buffer2, buffer, this.blurMaterial, passOffset + 1);
		}
		public void BlurAndBlitBuffer(RenderTexture rbuffer, RenderTexture destination, Settings settings, bool highPrecision)
		{
			int radius = settings.radius;
			float[] array = new float[radius + 2];
			float num = 1f / (float)(radius + 1);
			float num2 = 0f;
			for (int i = 0; i <= radius; i++)
			{
				float num3 = settings.falloff.Evaluate(1f - (float)i * num);
				array[i] = num3;
				num2 += num3;
			}
			num2 = num2 * 2f - array[0];
			for (int j = 0; j <= radius; j++)
			{
				array[j] *= settings.falloffScale;
			}
			if (settings.normalize)
			{
				for (int k = 0; k <= radius; k++)
				{
					array[k] /= num2;
				}
			}
			int num4 = Mathf.CeilToInt((float)radius / 2f);
			this.weightsMatrix[0] = array[0];
			this.offsetsMatrix[0] = 0f;
			for (int l = 0; l <= num4 - 1; l++)
			{
				float num5 = array[l * 2 + 1];
				float num6 = array[l * 2 + 2];
				float num7 = (float)(l * 2 + 1);
				float num8 = (float)(l * 2 + 2);
				this.weightsMatrix[l + 1] = num5 + num6;
				this.offsetsMatrix[l + 1] = (num7 * num5 + num8 * num6) / this.weightsMatrix[l + 1];
			}
			int baseResolution = (int)settings.baseResolution;
			this.downsampleMaterial.SetFloat("_Strength", 1f / (float)((baseResolution != 4) ? 1 : 4));
			RenderTexture temporary = RenderTexture.GetTemporary(rbuffer.width / baseResolution, rbuffer.height / baseResolution, 0, (!highPrecision) ? RenderTextureFormat.Default : RenderTextureFormat.ARGBHalf);
			RenderTexture temporary2 = RenderTexture.GetTemporary(temporary.width, temporary.height, 0, (!highPrecision) ? RenderTextureFormat.Default : RenderTextureFormat.ARGBHalf);
			Graphics.Blit(rbuffer, temporary, this.downsampleMaterial, (baseResolution != 4) ? 1 : 0);
			int passOffset = num4 * 2 - 2;
			for (int m = 0; m < settings.iterations; m++)
			{
				this.BlurBuffer(temporary, temporary2, passOffset);
			}
			this.composeMaterial.SetFloat("_Strength", settings.boostStrength);
			Graphics.Blit(temporary, destination, this.composeMaterial, (int)settings.blendMode);
			RenderTexture.ReleaseTemporary(temporary);
			RenderTexture.ReleaseTemporary(temporary2);
		}

		public void Release()
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
