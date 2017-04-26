using System;
using UnityEngine;
namespace Glow11.Blur
{
	internal class DefaultBlur : BlurBase, IBlur
	{
		private bool maxFallback = false;
		protected Material composeMaxMaterial = null;
		public DefaultBlur()
		{
			Shader shader = Shader.Find("Hidden/Glow 11/Compose Max");
			if (!shader.isSupported)
			{
				shader = Shader.Find("Hidden/Glow 11/Compose Max Fallback");
				this.maxFallback = true;
			}
			this.composeMaxMaterial = new Material(shader);
			this.composeMaxMaterial.hideFlags = HideFlags.DontSave;
		}
		public void BlurAndBlitBuffer(RenderTexture rbuffer, RenderTexture destination, Settings settings, bool highPrecision)
		{
			int baseResolution = (int)settings.baseResolution;
			int downsampleResolution = (int)settings.downsampleResolution;
			RenderTexture[] array = new RenderTexture[settings.downsampleSteps * 2];
			RenderTextureFormat renderTextureFormat = (!highPrecision) ? RenderTextureFormat.Default : RenderTextureFormat.ARGBHalf;
			this.downsampleMaterial.SetFloat("_Strength", settings.innerStrength / (float)((baseResolution != 4) ? 1 : 4));
			RenderTexture temporary = RenderTexture.GetTemporary(rbuffer.width / baseResolution, rbuffer.height / baseResolution, 0, renderTextureFormat);
			RenderTexture temporary2 = RenderTexture.GetTemporary(temporary.width, temporary.height, 0, renderTextureFormat);
			Graphics.Blit(rbuffer, temporary, this.downsampleMaterial, (baseResolution != 4) ? 1 : 0);
			this.downsampleMaterial.SetFloat("_Strength", settings.innerStrength / (float)((downsampleResolution != 4) ? 1 : 4));
			RenderTexture renderTexture = temporary;
			for (int i = 0; i < settings.downsampleSteps; i++)
			{
				int num = renderTexture.width / downsampleResolution;
				int num2 = renderTexture.height / downsampleResolution;
				if (num == 0 || num2 == 0)
				{
					break;
				}
				array[i * 2] = RenderTexture.GetTemporary(num, num2, 0, renderTextureFormat);
				array[i * 2 + 1] = RenderTexture.GetTemporary(num, num2, 0, renderTextureFormat);
				Graphics.Blit(renderTexture, array[i * 2], this.downsampleMaterial, (downsampleResolution != 4) ? 1 : 0);
				renderTexture = array[i * 2];
			}
			for (int j = settings.downsampleSteps - 1; j >= 0; j--)
			{
				if (!(array[j * 2] == null))
				{
					base.BlurBuffer(array[j * 2], array[j * 2 + 1]);
					RenderTexture renderTexture2 = (j <= 0) ? temporary : array[(j - 1) * 2];
					renderTexture2.MarkRestoreExpected();
					if (settings.downsampleBlendMode == DownsampleBlendMode.Max)
					{
						if (this.maxFallback)
						{
							this.composeMaxMaterial.SetTexture("_DestTex", renderTexture2);
						}
						this.composeMaxMaterial.SetFloat("_Strength", settings.outerStrength / ((float)j / 2f + 1f));
						Graphics.Blit(array[j * 2], renderTexture2, this.composeMaxMaterial);
					}
					else
					{
						this.composeMaterial.SetFloat("_Strength", settings.outerStrength / ((float)j / 2f + 1f));
						Graphics.Blit(array[j * 2], renderTexture2, this.composeMaterial, (int)settings.downsampleBlendMode);
					}
				}
			}
			base.BlurBuffer(temporary, temporary2);
			this.composeMaterial.SetFloat("_Strength", settings.boostStrength);
			Graphics.Blit(temporary, destination, this.composeMaterial, (int)settings.blendMode);
			RenderTexture.ReleaseTemporary(temporary);
			RenderTexture.ReleaseTemporary(temporary2);
			for (int k = 0; k < settings.downsampleSteps; k++)
			{
				RenderTexture.ReleaseTemporary(array[k * 2]);
				RenderTexture.ReleaseTemporary(array[k * 2 + 1]);
			}
		}

		public void Release()
		{
			ReleaseMaterial();
		}

		protected override void ReleaseMaterial()
		{
			base.ReleaseMaterial();
			if (composeMaxMaterial != null)
				UnityEngine.Object.DestroyImmediate(composeMaxMaterial);
		}
	}
}
