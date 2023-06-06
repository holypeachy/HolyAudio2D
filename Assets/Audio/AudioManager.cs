using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
	// Instance Vars
	public static AudioManager Instance;

	// Sounds and Mixers
	[SerializeField] private Sound[] Sounds;
	[SerializeField] private AudioMixer[] Mixers;
	[SerializeField] private AudioMixerGroup[] MixerGroups;

	private void Awake()
	{
		// Keeps only one instance of AudioManager in game
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);

		// We set up each sound
		foreach (Sound s in Sounds)
		{
			s.Source = gameObject.AddComponent<AudioSource>();

			s.Source.outputAudioMixerGroup = s.MixerGroup;
			s.Source.clip = s.AudioFile;
			
			s.Source.bypassEffects = s.BypassEffects;
			s.Source.bypassListenerEffects = s.BypassListenerEffects;
			s.Source.bypassReverbZones = s.BypassReverbZones;
			
			s.Source.playOnAwake = s.PlayOnAwake;
			s.Source.loop = s.Loop;
			
			s.Source.priority = s.Priority;
			s.Source.volume = s.Volume;
			s.Source.pitch = s.Pitch;
			s.Source.panStereo = s.StereoPan;
			s.Source.spatialBlend = s.SpatialBlend;
			s.Source.reverbZoneMix = s.ReverbZoneMix;
			s.Source.dopplerLevel = s.DopplerLevel;
			s.Source.spread = s.Spread;
			
			s.Source.rolloffMode = s.VolumeRolloff;
			s.Source.minDistance = s.MaxDistance;
			s.Source.maxDistance = s.MaxDistance;

			if (s.PlayOnAwake)
			{
				s.Source.Play();
			}

		}

	}


	public void Play()
	{

	}
	
	
	public void PlayOne()
	{
		
	}
	
	
	public void Stop()
	{

	}
	
	public void StopAllButMusic()
	{
		
	}
	
	public void StopAll()
	{
		
	}
	
	public void InitialSetUp()
	{
		
	}

	public void FindMixer()
	{
		
	}
	
	public void FindMixerGroup()
	{
		
	}

}