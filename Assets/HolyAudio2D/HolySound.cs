using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class HolySound
{	
	// Basic Info
	public string ClipName;
	[HideInInspector] public AudioSource Source;
	public AudioClip AudioFile;
	public AudioMixerGroup MixerGroup;
	

	// Options
	public bool Mute = false;
	public bool BypassEffects = false;

	[Tooltip("Warning: Can only be enabled if the Mixer for this sound is set to None")]
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
	public float DopplerLevel = 0f;

	[Range(0, 360)]
	public int Spread = 0;
	
	public AudioRolloffMode VolumeRolloff = AudioRolloffMode.Logarithmic;
	
	public float MinDistance = 0f;
	public float MaxDistance = 500f;


	public HolySound(HolySound sound)
	{
		ClipName = sound.ClipName;
		Source = sound.Source;
		MixerGroup = sound.MixerGroup;
		AudioFile = sound.AudioFile;

		Mute = sound.Mute;
		BypassEffects = sound.BypassEffects;
		BypassListenerEffects = sound.BypassListenerEffects;
		BypassReverbZones = sound.BypassReverbZones;

		PlayOnAwake = sound.PlayOnAwake;
		Loop = sound.Loop;

		Priority = sound.Priority;
		Volume = sound.Volume;
		Pitch = sound.Pitch;
		StereoPan = sound.StereoPan;
		SpatialBlend = sound.SpatialBlend;
		ReverbZoneMix = sound.ReverbZoneMix;
		DopplerLevel = sound.DopplerLevel;
		Spread = sound.Spread;

		VolumeRolloff = sound.VolumeRolloff;
		MinDistance = sound.MinDistance;
		MaxDistance = sound.MaxDistance;
	}
}
