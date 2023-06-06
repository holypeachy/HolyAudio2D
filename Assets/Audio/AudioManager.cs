using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
	// Instance Vars
	public static AudioManager Instance;

	// Sounds
	[SerializeField] private List<Sound> Sounds;
	[SerializeField] private AudioMixer[] Mixers;
	[SerializeField] private AudioMixerGroup[] MixerGroups;

	private void Awake()
	{

		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
			return;
		}
		// DontDestroyOnLoad(gameObject);


		foreach (Sound s in Sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.audioClip;
			s.source.loop = s.isLoop;
			s.source.volume = s.volume;
			s.source.pitch = s.pitch;

			/*
			 * If you want to add more mixer groups add a new enum in Sounds and add it here. 
			 */

			if (s.playOnAwake)
				s.source.Play();
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

}