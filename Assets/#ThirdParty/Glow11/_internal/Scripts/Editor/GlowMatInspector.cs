using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
public class GlowMatInspector : MaterialEditor
{
	private struct KeywordOption
	{
		public string description;
		public string keyword;
		public string property;
	}
	private GlowMatInspector.KeywordOption[] glowModes = new GlowMatInspector.KeywordOption[]
	{
		new GlowMatInspector.KeywordOption
		{
			description = "Base Texture",
			keyword = "GLOW11_GLOW_MAINTEX",
			property = "_MainTex"
		},
		new GlowMatInspector.KeywordOption
		{
			description = "Main Color",
			keyword = "GLOW11_GLOW_MAINCOLOR",
			property = "_Color"
		},
		new GlowMatInspector.KeywordOption
		{
			description = "Glow Texture",
			keyword = "GLOW11_GLOW_GLOWTEX",
			property = "_GlowTex"
		},
		new GlowMatInspector.KeywordOption
		{
			description = "Glow Color",
			keyword = "GLOW11_GLOW_GLOWCOLOR",
			property = "_GlowColor"
		},
		new GlowMatInspector.KeywordOption
		{
			description = "Illumin Texture (RGB)",
			keyword = "GLOW11_GLOW_ILLUMTEX",
			property = "_Illum"
		},
		new GlowMatInspector.KeywordOption
		{
			description = "Vertex Color",
			keyword = "GLOW11_GLOW_VERTEXCOLOR"
		},
		new GlowMatInspector.KeywordOption
		{
			description = "Tint Color",
			keyword = "GLOW11_GLOW_TINTCOLOR",
			property = "_TintColor"
		}
	};
	private GlowMatInspector.KeywordOption[] glowMultipliers = new GlowMatInspector.KeywordOption[]
	{
		new GlowMatInspector.KeywordOption
		{
			description = "-",
			keyword = "GLOW11_MULTIPLY_OFF"
		},
		new GlowMatInspector.KeywordOption
		{
			description = "Glow Color",
			keyword = "GLOW11_MULTIPLY_GLOWCOLOR",
			property = "_GlowColor"
		},
		new GlowMatInspector.KeywordOption
		{
			description = "Vertex Color",
			keyword = "GLOW11_MULTIPLY_VERT"
		},
		new GlowMatInspector.KeywordOption
		{
			description = "Vertex Alpha",
			keyword = "GLOW11_MULTIPLY_VERT_ALPHA"
		},
		new GlowMatInspector.KeywordOption
		{
			description = "Base Texture (A)",
			keyword = "GLOW11_MULTIPLY_MAINTEX_ALPHA",
			property = "_MainTex"
		},
		new GlowMatInspector.KeywordOption
		{
			description = "Illumin Texture (A)",
			keyword = "GLOW11_MULTIPLY_ILLUMTEX_ALPHA",
			property = "_Illum"
		},
		new GlowMatInspector.KeywordOption
		{
			description = "Main Color (A)",
			keyword = "Glow11_MULTIPLY_MAINCOLOR_ALPHA",
			property = "_Color"
		},
		new GlowMatInspector.KeywordOption
		{
			description = "Tint Color (A)",
			keyword = "GLOW11_MULTIPLY_TINTCOLOR_ALPHA",
			property = "_TintColor"
		}
	};
	private List<GlowMatInspector.KeywordOption> glowOptionsMap = new List<GlowMatInspector.KeywordOption>();
	private List<GlowMatInspector.KeywordOption> multiplierOptionsMap = new List<GlowMatInspector.KeywordOption>();
	private List<string> glowOptions = new List<string>();
	private List<string> multiplierOptions = new List<string>();
	private int activeGlowModeIdx = 0;
	private int activeGlowMultiplierIdx = 0;
	private int lastShader;
	public override void OnEnable()
	{
		base.OnEnable();
		this.Init();
	}
	private void Init()
	{
		Material material = base.target as Material;
		this.lastShader = material.shader.GetInstanceID();
		this.glowOptionsMap.Clear();
		this.multiplierOptionsMap.Clear();
		this.glowOptions.Clear();
		this.multiplierOptions.Clear();
		for (int i = 0; i < this.glowModes.Length; i++)
		{
			if (this.glowModes[i].property == null || material.HasProperty(this.glowModes[i].property))
			{
				this.glowOptions.Add(this.glowModes[i].description);
				this.glowOptionsMap.Add(this.glowModes[i]);
			}
		}
		for (int j = 0; j < this.glowMultipliers.Length; j++)
		{
			if (this.glowMultipliers[j].property == null || material.HasProperty(this.glowMultipliers[j].property))
			{
				this.multiplierOptions.Add(this.glowMultipliers[j].description);
				this.multiplierOptionsMap.Add(this.glowMultipliers[j]);
			}
		}
		for (int k = 1; k < base.targets.Length; k++)
		{
			material = (base.targets[k] as Material);
			List<string> list = new List<string>(material.shaderKeywords);
			if (list.FirstOrDefault((string s) => s.StartsWith("GLOW11_GLOW_")) == null)
			{
				for (int l = 0; l < this.glowModes.Length; l++)
				{
					if (this.glowModes[l].property == null || material.HasProperty(this.glowModes[l].property))
					{
						this.addKeywordOption(list, this.glowModes[l], "GLOW11_GLOW_");
						break;
					}
				}
			}
			if (list.FirstOrDefault((string s) => s.StartsWith("GLOW11_MULTIPLY_")) == null)
			{
				for (int m = 0; m < this.glowMultipliers.Length; m++)
				{
					if (this.glowMultipliers[m].property == null || material.HasProperty(this.glowMultipliers[m].property))
					{
						this.addKeywordOption(list, this.glowMultipliers[m], "GLOW11_MULTIPLY_");
						break;
					}
				}
			}
			material.shaderKeywords = list.ToArray();
		}
		for (int n = 1; n < base.targets.Length; n++)
		{
			material = (base.targets[n] as Material);
			for (int num = 0; num < this.glowModes.Length; num++)
			{
				if (this.glowModes[num].property != null && !material.HasProperty(this.glowModes[num].property))
				{
					this.glowOptions.Remove(this.glowModes[num].description);
					this.glowOptionsMap.Remove(this.glowModes[num]);
				}
			}
			for (int num2 = 0; num2 < this.glowMultipliers.Length; num2++)
			{
				if (this.glowMultipliers[num2].property != null && !material.HasProperty(this.glowMultipliers[num2].property))
				{
					this.multiplierOptions.Remove(this.glowMultipliers[num2].description);
					this.multiplierOptionsMap.Remove(this.glowMultipliers[num2]);
				}
			}
		}
	}
	private void loadOptions()
	{
		string[] shaderKeywords = (base.target as Material).shaderKeywords;
		this.activeGlowModeIdx = 0;
		for (int i = 0; i < this.glowOptionsMap.Count; i++)
		{
			if (shaderKeywords.Contains(this.glowOptionsMap[i].keyword))
			{
				this.activeGlowModeIdx = i;
				break;
			}
		}
		this.activeGlowMultiplierIdx = 0;
		for (int j = 0; j < this.multiplierOptionsMap.Count; j++)
		{
			if (shaderKeywords.Contains(this.multiplierOptionsMap[j].keyword))
			{
				this.activeGlowMultiplierIdx = j;
				break;
			}
		}
	}
	public override void OnInspectorGUI()
	{
		this.loadOptions();
		base.OnInspectorGUI();
		Material material = base.target as Material;
		int instanceID = material.shader.GetInstanceID();
		if (instanceID != this.lastShader)
		{
			this.Init();
		}
		bool flag = false;
		List<GlowMatInspector.KeywordOption> list = new List<GlowMatInspector.KeywordOption>(this.glowOptionsMap);
		List<GlowMatInspector.KeywordOption> list2 = new List<GlowMatInspector.KeywordOption>(this.multiplierOptionsMap);
		List<string> list3 = new List<string>(this.glowOptions);
		List<string> list4 = new List<string>(this.multiplierOptions);
		for (int i = 1; i < base.targets.Length; i++)
		{
			List<string> keywords = new List<string>(((Material)base.targets[i]).shaderKeywords);
			if (!keywords.Contains(list[this.activeGlowModeIdx].keyword))
			{
				list3.Insert(0, "—");
				list.Insert(0, list[0]);
				this.activeGlowModeIdx = 0;
				break;
			}
		}
		for (int j = 1; j < base.targets.Length; j++)
		{
			List<string> keywords = new List<string>(((Material)base.targets[j]).shaderKeywords);
			if (!keywords.Contains(list2[this.activeGlowMultiplierIdx].keyword))
			{
				list4.Insert(0, "—");
				list2.Insert(0, list2[0]);
				this.activeGlowMultiplierIdx = 0;
				break;
			}
		}
		int num = EditorGUILayout.Popup("Glow Source", this.activeGlowModeIdx, list3.ToArray(), new GUILayoutOption[0]);
		int num2 = EditorGUILayout.Popup("Glow Multiplier", this.activeGlowMultiplierIdx, list4.ToArray(), new GUILayoutOption[0]);
		if (this.activeGlowModeIdx != num || this.activeGlowMultiplierIdx != num2)
		{
			string text;
			if (this.activeGlowModeIdx != num)
			{
				text = "Modify Glow Source";
			}
			else
			{
				text = "Modify Glow Multiplier";
			}
			if (base.targets.Length == 1)
			{
				text = text + " of " + material.name;
			}
			else
			{
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					" of ",
					base.targets.Length,
					" Materials"
				});
			}
			Undo.RecordObjects(base.targets, text);
			for (int k = 0; k < base.targets.Length; k++)
			{
				Material material2 = (Material)base.targets[k];
				List<string> list5 = new List<string>(material2.shaderKeywords);
				if (this.activeGlowModeIdx != num)
				{
					this.addKeywordOption(list5, list[num], "GLOW11_GLOW_");
				}
				if (this.activeGlowMultiplierIdx != num2)
				{
					this.addKeywordOption(list5, list2[num2], "GLOW11_MULTIPLY_");
				}
				((Material)base.targets[k]).shaderKeywords = list5.ToArray();
			}
			flag = true;
		}
		if (flag)
		{
			EditorUtility.SetDirty(base.target);
		}
	}
	private void addKeywordOption(List<string> keywords, GlowMatInspector.KeywordOption keywordOption, string prefix)
	{
		string text;
		do
		{
			text = keywords.FirstOrDefault((string s) => s.StartsWith(prefix));
			if (text != "")
			{
				keywords.Remove(text);
			}
		}
		while (text != null);
		keywords.Add(keywordOption.keyword);
	}
}
