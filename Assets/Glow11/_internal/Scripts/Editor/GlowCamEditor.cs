using System;
using UnityEditor;
using UnityEngine;
namespace Glow11
{
	[CustomEditor(typeof(Glow11))]
	public class GlowCamEditor : Editor
	{
		private SerializedProperty cullingMask;
		private SerializedProperty innerStrength;
		private SerializedProperty outerStrength;
		private SerializedProperty boostStrength;
		private SerializedProperty downsampleSteps;
		private SerializedProperty iterations;
		private SerializedProperty blendMode;
		private SerializedProperty blurSpread;
		private SerializedProperty downsampleBlendMode;
		private SerializedProperty downsampleResolution;
		private SerializedProperty baseResolution;
		private SerializedProperty falloffScale;
		private SerializedProperty falloff;
		private SerializedProperty radius;
		private SerializedProperty normalize;
		private SerializedObject serObj;
		private string[] blurModeOptions = new string[]
		{
			"Default",
			"Advanced (Desktop only)",
			"High Quality",
			"Unity Blur"
		};
		private int[] blurModeValues = new int[]
		{
			0,
			5,
			10,
			100
		};
		private void OnEnable()
		{
			this.serObj = new SerializedObject((Glow11)base.target);
			this.cullingMask = this.serObj.FindProperty("cullingMask");
			this.innerStrength = this.serObj.FindProperty("settings.innerStrength");
			this.outerStrength = this.serObj.FindProperty("settings.outerStrength");
			this.boostStrength = this.serObj.FindProperty("settings.boostStrength");
			this.downsampleSteps = this.serObj.FindProperty("settings.downsampleSteps");
			this.iterations = this.serObj.FindProperty("settings.iterations");
			this.blendMode = this.serObj.FindProperty("settings.blendMode");
			this.blurSpread = this.serObj.FindProperty("settings.blurSpread");
			this.downsampleBlendMode = this.serObj.FindProperty("settings.downsampleBlendMode");
			this.downsampleResolution = this.serObj.FindProperty("settings.downsampleResolution");
			this.baseResolution = this.serObj.FindProperty("settings.baseResolution");
			this.falloff = this.serObj.FindProperty("settings.falloff");
			this.falloffScale = this.serObj.FindProperty("settings.falloffScale");
			this.radius = this.serObj.FindProperty("settings.radius");
			this.normalize = this.serObj.FindProperty("settings.normalize");
		}
		public override void OnInspectorGUI()
		{
			Glow11 glow = (Glow11)base.target;
			this.serObj.Update();
			EditorGUILayout.PropertyField(this.cullingMask, new GUIContent("Effected Layer"));
			if (!PlayerSettings.use32BitDisplayBuffer)
			{
				EditorGUILayout.HelpBox("It is recommended you use a 32-bit display buffer (can be set in the player settings).", MessageType.Warning);
			}
			bool flag = false;
			bool flag2 = EditorGUILayout.Toggle("High Precision", glow.highPrecision, new GUILayoutOption[0]);
			if (glow.highPrecision != flag2)
			{
				Undo.RecordObject(glow, "High Precision");
				glow.highPrecision = flag2;
				EditorUtility.SetDirty(glow);
			}
			if (glow.highPrecision && !glow.highPrecisionActive)
			{
				EditorGUILayout.HelpBox("Your graphics card doesn't support High Precision.", MessageType.Warning);
			}
			if (QualitySettings.antiAliasing != 0)
			{
				GUI.enabled = false;
			}
			bool flag3 = EditorGUILayout.Toggle("Reuse Depth Buffer", glow.reuseDepth, new GUILayoutOption[0]);
			GUI.enabled = true;
			if (flag3 != glow.reuseDepth)
			{
				Undo.RecordObject(glow, "Reuse Depthbuffer");
				glow.reuseDepth = flag3;
				EditorUtility.SetDirty(glow);
				flag = true;
			}
			if (QualitySettings.antiAliasing != 0)
			{
				EditorGUILayout.HelpBox("Reuse Depth Buffer is only available when antialiasing is disabled.", MessageType.Info);
			}
			if (flag3 && QualitySettings.antiAliasing == 0)
			{
				GUI.enabled = false;
			}
			Resolution resolution = (Resolution)EditorGUILayout.EnumPopup("Rerender Resolution", glow.rerenderResolution, new GUILayoutOption[0]);
			if (resolution != glow.rerenderResolution)
			{
				Undo.RecordObject(glow, "Rerender Resolution");
				glow.rerenderResolution = resolution;
				EditorUtility.SetDirty(glow);
				flag = true;
			}
			GUI.enabled = true;
			EditorGUILayout.Space();
			BlurMode blurMode = (BlurMode)EditorGUILayout.IntPopup("Blur Mode", (int)glow.blurMode, this.blurModeOptions, this.blurModeValues, new GUILayoutOption[0]);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			if (blurMode != glow.blurMode)
			{
				Undo.RecordObject(glow, "Blur Mode");
				glow.blurMode = blurMode;
				EditorUtility.SetDirty(glow);
				flag = true;
			}
			if (glow.blurMode == BlurMode.Advanced)
			{
				this.falloff.animationCurveValue = EditorGUILayout.CurveField("Falloff", this.falloff.animationCurveValue, Color.green, new Rect(0f, 0f, 1f, 1f), new GUILayoutOption[]
				{
					GUILayout.Height(100f)
				});
				EditorGUILayout.PropertyField(this.normalize, new GUILayoutOption[0]);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				this.falloffScale.floatValue = EditorGUILayout.Slider("Scale", this.falloffScale.floatValue, 0.1f, 2f, new GUILayoutOption[0]);
				if (GUILayout.Button("Reset", new GUILayoutOption[0]))
				{
					GUI.FocusControl("");
					this.falloffScale.floatValue = 1f;
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				this.radius.intValue = EditorGUILayout.IntSlider("Radius", this.radius.intValue, 1, 30, new GUILayoutOption[0]);
				if (GUILayout.Button("Reset", new GUILayoutOption[0]))
				{
					GUI.FocusControl("");
					this.radius.intValue = 3;
				}
				GUILayout.EndHorizontal();
				if (this.radius.intValue <= 12)
				{
					EditorGUILayout.HelpBox("A radius greater then 12 requires Shader Model 3.", MessageType.Info);
				}
				else
				{
					EditorGUILayout.HelpBox("You've set a radius greater then 12, with this setting shader model 3 is required.", MessageType.Warning);
				}
			}
			if (glow.blurMode == BlurMode.HighQuality || glow.blurMode == BlurMode.UnityBlur || glow.blurMode == BlurMode.Advanced)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				this.iterations.intValue = EditorGUILayout.IntSlider("Iterations", this.iterations.intValue, 1, 20, new GUILayoutOption[0]);
				if (GUILayout.Button("Reset", new GUILayoutOption[0]))
				{
					GUI.FocusControl("");
					this.iterations.intValue = 3;
				}
				GUILayout.EndHorizontal();
			}
			if (glow.blurMode == BlurMode.UnityBlur)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				this.blurSpread.floatValue = EditorGUILayout.Slider("Blur Spread", this.blurSpread.floatValue, 0.01f, 10f, new GUILayoutOption[0]);
				if (GUILayout.Button("Reset", new GUILayoutOption[0]))
				{
					GUI.FocusControl("");
					this.blurSpread.floatValue = 0.6f;
				}
				GUILayout.EndHorizontal();
			}
			if (glow.blurMode == BlurMode.Default || glow.blurMode == BlurMode.HighQuality || glow.blurMode == BlurMode.Advanced)
			{
				EditorGUILayout.PropertyField(this.baseResolution, new GUILayoutOption[0]);
			}
			if (glow.blurMode == BlurMode.Default)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				this.downsampleSteps.intValue = EditorGUILayout.IntSlider("Downsample Steps", this.downsampleSteps.intValue, 0, 10, new GUILayoutOption[0]);
				if (GUILayout.Button("Reset", new GUILayoutOption[0]))
				{
					GUI.FocusControl("");
					this.downsampleSteps.intValue = 2;
				}
				GUILayout.EndHorizontal();
				GUI.enabled = this.downsampleSteps.intValue >= 1;
				EditorGUILayout.PropertyField(this.downsampleResolution, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.downsampleBlendMode, new GUILayoutOption[0]);
				GUI.enabled = true;
			}
			if (glow.blurMode != BlurMode.Advanced)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				this.innerStrength.floatValue = EditorGUILayout.Slider("Inner Strength", this.innerStrength.floatValue, 0.01f, 10f, new GUILayoutOption[0]);
				if (GUILayout.Button("Reset", new GUILayoutOption[0]))
				{
					GUI.FocusControl("");
					this.innerStrength.floatValue = 1f;
				}
				GUILayout.EndHorizontal();
			}
			if (glow.blurMode == BlurMode.Default)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				this.outerStrength.floatValue = EditorGUILayout.Slider("Outer Strength", this.outerStrength.floatValue, 0.01f, 10f, new GUILayoutOption[0]);
				if (GUILayout.Button("Reset", new GUILayoutOption[0]))
				{
					GUI.FocusControl("");
					this.outerStrength.floatValue = 1f;
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			this.boostStrength.floatValue = EditorGUILayout.Slider("Boost Strength", this.boostStrength.floatValue, 0.01f, 10f, new GUILayoutOption[0]);
			if (GUILayout.Button("Reset", new GUILayoutOption[0]))
			{
				GUI.FocusControl("");
				this.boostStrength.floatValue = 1f;
			}
			GUILayout.EndHorizontal();
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.blendMode, new GUILayoutOption[0]);
			if ((this.serObj.ApplyModifiedProperties() || flag) && EditorApplication.isPaused)
			{
				((Glow11)base.target).InitCamera();
			}
		}
	}
}
