using System;
using System.Reflection;
using UnityEngine;

public static class Component_Clone_Extension
{
	public static T GetCopyOf<T>(this Component comp, T other) where T : Component
	{
		Type type = comp.GetType();
		if (type != other.GetType()) return null; // type mis-match
		BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
		PropertyInfo[] pinfos = type.GetProperties(flags);
		foreach (PropertyInfo pinfo in pinfos)
		{
			if (pinfo.CanWrite)
			{
				try
				{
					pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
				}
				catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
			}
		}
		FieldInfo[] finfos = type.GetFields(flags);
		foreach (FieldInfo finfo in finfos)
		{
			finfo.SetValue(comp, finfo.GetValue(other));
		}
		return comp as T;
	}

	public static void ApplyTo(this AudioSource comp, AudioSource other)
	{
		// 暂时把要应用的属性逐个修改，如果真的又需要的话，再改成用Component的GetCopyOf
		other.clip = comp.clip;
		//other.mute = comp.mute;
		//other.bypassEffects = comp.bypassEffects;
		//other.bypassListenerEffects = comp.bypassListenerEffects;
		//other.bypassReverbZones = comp.bypassReverbZones;
		//other.playOnAwake = comp.playOnAwake;
		//other.loop = comp.loop;
		//other.priority = comp.priority;
		other.volume = comp.volume;
		other.pitch = comp.pitch;
		//other.dopplerLevel = comp.dopplerLevel;
		other.rolloffMode = comp.rolloffMode;
		other.minDistance = comp.minDistance;
		//other.panLevel = comp.panLevel;
		//other.spread = comp.spread;
		other.maxDistance = comp.maxDistance;
		//other.pan = comp.pan;
	}
}
