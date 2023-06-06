using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
	[HideInInspector] public AudioSource Source;
	
	public string ClipName;
	public AudioMixerGroup MixerGroup;
	public AudioClip AudioFile;
	

	public bool BypassEffects = false;
	public bool BypassListenerEffects = false;
	public bool BypassReverbZones = false;


	public bool PlayOnAwake;
	public bool Loop;
	

	[Range(0, 256)]
	public int Priority = 128;
	
	[Range(0f, 1f)]
	public float Volume = 0.5f;
	
	[Range(-3f, 3f)]
	public float Pitch = 1f;
	
	[Range(-1f, 1f)]
	public float StereoPan = 0f;

	[Range(0f, 1f)]
	public float SpatialBlend = 1f;

	[Range(0f, 1.1f)]
	public float ReverbZoneMix = 1f;

	[Range(0f, 5f)]
	public float DopplerLevel = 1f;

	[Range(0, 360)]
	public int Spread = 0;
	
	public AudioRolloffMode VolumeRolloff = AudioRolloffMode.Logarithmic;
	
	public float MinDistance = 1f;
	public float MaxDistance = 500f;
}
