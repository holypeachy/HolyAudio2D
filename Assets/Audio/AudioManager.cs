using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
	// Instance Var
	public static AudioManager Instance;

	// Control Flow
	public bool AreMixersSetupProperly = true;
	public bool EnableDebug = true;
	public bool DisableSpatialBlend = false;
	
	// Play Control
	private bool HasPlayedSound = false;
	private bool HasPlayedSourceSound = false;
	private bool SoundNotFound = false;
	private bool SourceSoundNotFound = false;
	
	// Stop Control
	private bool HasStoppedSound = false;
	private bool HasStoppedSourceSound = false;
	
	// Sounds and Mixers
	[SerializeField] private AudioMixer[] Mixers;
	[SerializeField] private AudioMixerGroup[] MixerGroups;
	[SerializeField] private Sound[] Sounds;
	[SerializeField] private SourceSound[] SourceSounds;
	
	
	// Settings
	
	
	//Testing
	private bool canPlay = false;
	private bool uwu = false;
	public bool load = false;


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
			s.Source.minDistance = s.MinDistance;
			s.Source.maxDistance = s.MaxDistance;

			if (s.PlayOnAwake)
			{
				s.Source.Play();
			}

		}
		
		if(DisableSpatialBlend)
		{
			foreach (SourceSound s in SourceSounds)
			{
				s.Source.spatialBlend = 0f;
			}
			foreach (Sound s in Sounds)
			{
				s.Source.spatialBlend = 0f;
			}
		}

	}
	
	private void Update() {
		
		// Testing
		StartCoroutine(Cooldown());
		if(canPlay && !uwu)
		{
			uwu = true;
			canPlay = false;
			SaveSettings();
		}
		else if(load)
		{
			LoadSettings();
		}
	}


	public void Play(string ClipName)
	{
		// We look for clip in Sounds
		Sound s = Array.Find(Sounds, sound => sound.ClipName == ClipName);
		
		if (s == null)
		{
			SoundNotFound = true;
		}
		else
		{
			s.Source.PlayOneShot(s.Source.clip);
			HasPlayedSound = true;
		}
		
		
		// Then we look for clip in SourceSounds
		SourceSound ss = Array.Find(SourceSounds, sound => sound.ClipName == ClipName);

		if (ss == null)
		{
			SourceSoundNotFound = true;
		}
		else
		{
			ss.Source.PlayOneShot(ss.Source.clip);
			HasPlayedSourceSound = true;
		}
		
		// Debug
		if(EnableDebug)
		{
			if(SoundNotFound && SourceSoundNotFound)
			{
				Debug.LogError("AudioManager|Sounds & SourceSounds|PlayOneShot: " + ClipName + " does NOT exist!");
			}
			else if(SoundNotFound && !HasPlayedSourceSound)
			{
				Debug.LogError("AudioManager|Sounds|PlayOneShot: " + ClipName + " does NOT exist!");
			}
			else if(SourceSoundNotFound && !HasPlayedSound)
			{
				Debug.LogError("AudioManager|SourceSounds|PlayOneShot: " + ClipName + " does NOT exist!");
			}
			else if(HasPlayedSound && HasPlayedSourceSound)
			{
				Debug.LogError("AudioManager|PlayOneShot: A clip has been found in Sounds and SourceSounds");
			}
		}
	
		// We reset all the vars
		HasPlayedSound = false;
		HasPlayedSourceSound = false;
		SoundNotFound = false;
		SourceSoundNotFound = false;
	}
	
	
	public void PlayOnce(string ClipName)
	{
		// We look for clip in Sounds
		Sound s = Array.Find(Sounds, sound => sound.ClipName == ClipName);

		if (s == null)
		{
			SoundNotFound = true;
		}
		else
		{
			s.Source.Play();
			HasPlayedSound = true;
		}


		// Then we look for clip in SourceSounds
		SourceSound ss = Array.Find(SourceSounds, sound => sound.ClipName == ClipName);

		if (ss == null)
		{
			SourceSoundNotFound = true;
		}
		else
		{
			ss.Source.Play();
			HasPlayedSourceSound = true;
		}


		// Debug
		if (EnableDebug)
		{
			if (SoundNotFound && SourceSoundNotFound)
			{
				Debug.LogError("AudioManager|Sounds & SourceSounds|Play: " + ClipName + " does NOT exist!");
			}
			else if (SoundNotFound && !HasPlayedSourceSound)
			{
				Debug.LogError("AudioManager|Sounds|Play: " + ClipName + " does NOT exist!");
			}
			else if (SourceSoundNotFound && !HasPlayedSound)
			{
				Debug.LogError("AudioManager|SourceSounds|Play: " + ClipName + " does NOT exist!");
			}
			else if (HasPlayedSound && HasPlayedSourceSound)
			{
				Debug.LogError("AudioManager|Play: A clip has been found in Sounds and SourceSounds");
			}
		}


		// We reset all the vars
		HasPlayedSound = false;
		HasPlayedSourceSound = false;
		SoundNotFound = false;
		SourceSoundNotFound = false;
	}
	
	
	public void Stop(string ClipName)
	{
		// We look for clip in Sounds
		Sound s = Array.Find(Sounds, sound => sound.ClipName == ClipName);

		if (s == null)
		{
			SoundNotFound = true;
		}
		else
		{
			s.Source.Stop();
			HasStoppedSound = true;
		}


		// Then we look for clip in SourceSounds
		SourceSound ss = Array.Find(SourceSounds, sound => sound.ClipName == ClipName);

		if (ss == null)
		{
			SourceSoundNotFound = true;
		}
		else
		{
			ss.Source.Stop();
			HasStoppedSourceSound = true;
		}

		// Debug
		if (EnableDebug)
		{
			if (SoundNotFound && SourceSoundNotFound)
			{
				Debug.LogError("AudioManager|Sounds & SourceSounds|Stop: " + ClipName + " does NOT exist!");
			}
			else if (SoundNotFound && !HasStoppedSourceSound)
			{
				Debug.LogError("AudioManager|Sounds|Stop: " + ClipName + " does NOT exist!");
			}
			else if (SourceSoundNotFound && !HasStoppedSound)
			{
				Debug.LogError("AudioManager|SourceSounds|Stop: " + ClipName + " does NOT exist!");
			}
			else if (HasStoppedSound && HasStoppedSourceSound)
			{
				Debug.LogError("AudioManager|Stop: A clip has been found in Sounds and SourceSounds");
			}
		}

		// We reset all the vars
		HasStoppedSound = false;
		HasStoppedSourceSound = false;
		SoundNotFound = false;
		SourceSoundNotFound = false;
	}
	
	
	public void StopAllButMixerGroup(string MixerGroupName)
	{
		foreach (Sound s in Sounds)
		{
			if(s.Source.outputAudioMixerGroup.name == MixerGroupName)
			{
				continue;
			}
			else
			{
				s.Source.Stop();
			}
		}

		foreach (SourceSound ss in SourceSounds)
		{
			if (ss.Source.outputAudioMixerGroup.name == MixerGroupName)
			{
				continue;
			}
			else
			{
				ss.Source.Stop();
			}
		}

		if (EnableDebug)
		{
			Debug.LogWarning("AudioManager|StopAllButMixerGroup: All sounds except from " + MixerGroupName + " have stopped!");
		}
		
	}
	
	
	public void StopAllFromMixerGroup(string MixerGroupName)
	{
		foreach (Sound s in Sounds)
		{
			if (s.Source.outputAudioMixerGroup.name == MixerGroupName)
			{
				s.Source.Stop();
			}
		}

		foreach (SourceSound ss in SourceSounds)
		{
			if (ss.Source.outputAudioMixerGroup.name == MixerGroupName)
			{
				ss.Source.Stop();
			}
		}

		if (EnableDebug)
		{
			Debug.LogWarning("AudioManager|StopFromMixerGroup: Sounds in " + MixerGroupName + " have stopped!");
		}
	}
	
	
	public void StopAll()
	{
		foreach (Sound s in Sounds)
		{
			s.Source.Stop();
		}
		foreach (SourceSound ss in SourceSounds)
		{
			ss.Source.Stop();
		}

		if(EnableDebug)
		{
			Debug.LogWarning("AudioManager|StopAll: All sounds stopped!");
		}
	}


	public AudioMixerGroup GetAudioMixerGroup(string MixerGroupName)
	{
		foreach (AudioMixerGroup group in MixerGroups)
		{
			if(group.name == MixerGroupName)
			{
				return group;
			}
		}

		Debug.LogError("AudioManager|GetAudioMixerGroup: " + MixerGroupName + " does NOT exist!");
		return null;
	}
	
	
	public AudioMixerGroup[] GetAllMixerGroups()
	{
		return MixerGroups;	
	}

	
	// public void SaveSettings()
	// {
	// 	if(AreMixersSetupProperly)
	// 	{
	// 		float[] RawGroupVolumes = new float[MixerGroups.Length + 1];
	// 		float[] RawMasterVolumes = new float[Mixers.Length];
			
	// 		for (int i = 0; i < MixerGroups.Length; i++)
	// 		{
	// 			MixerGroups[i].audioMixer.GetFloat(MixerGroups[i].name + "Volume", out RawGroupVolumes[i]);
	// 			if(EnableDebug)
	// 			{
	// 				Debug.Log("AudioManager|SaveSettings: " + MixerGroups[i].name + "Volume" + RawGroupVolumes[i]);
	// 			}
	// 		}
			
	// 		MixerGroups[0].audioMixer.GetFloat("MasterVolume", out RawGroupVolumes[MixerGroups.Length]);
	// 		if(EnableDebug)
	// 		{
	// 			Debug.Log("AudioManager|SaveSettings: " + "MasterVolume" + " " + RawGroupVolumes[MixerGroups.Length]);
	// 		}

	// 		AudioData data = new AudioData();
	// 		data.Volumes = RawGroupVolumes;
		
	// 		AudioMSave.SaveData(data);
	// 	}

	// }

	public void SaveSettings()
	{
		if (AreMixersSetupProperly)
		{
			float[] RawMasterVolumes = new float[Mixers.Length];
			float[] RawGroupVolumes = new float[MixerGroups.Length];

			for (int i = 0; i < Mixers.Length; i++)
			{
				Mixers[i].GetFloat("MasterVolume", out RawMasterVolumes[i]);
				if (EnableDebug)
				{
					Debug.Log("AudioManager|SaveSettings: " + Mixers[i].name + "'s MasterVolume is " + RawMasterVolumes[i]);
				}
			}
			
			for (int i = 0; i < MixerGroups.Length; i++)
			{
				MixerGroups[i].audioMixer.GetFloat(MixerGroups[i].name + "Volume", out RawGroupVolumes[i]);
				if (EnableDebug)
				{
					Debug.Log("AudioManager|SaveSettings: " + MixerGroups[i].audioMixer.name + "'s -> " + MixerGroups[i].name + "Volume is " + RawGroupVolumes[i]);
				}
			}

			AudioData data = new AudioData();
			data.MasterVolumes = RawMasterVolumes;
			data.GroupVolumes = RawGroupVolumes;

			AudioMSave.SaveData(data);
		}

	}
	
	
	// public void LoadSettings()
	// {
	// 	if(AreMixersSetupProperly)
	// 	{
	// 		AudioData data =  AudioMSave.LoadData();
	
	// 		if(data.Volumes.Length == MixerGroups.Length + 1)
	// 		{
	// 			for (int i = 0; i < MixerGroups.Length; i++)
	// 			{
	// 				MixerGroups[i].audioMixer.SetFloat(MixerGroups[i].name + "Volume", data.Volumes[i]);
	// 				Debug.Log("AudioManager|LoadSettings: " + MixerGroups[i].name + "Volume" + data.Volumes[i]);
	// 			}
	// 			MixerGroups[0].audioMixer.SetFloat("MasterVolume", data.Volumes[data.Volumes.Length - 1]);
	// 			Debug.Log("AudioManager|LoadSettings: " + "MasterVolume " + data.Volumes[data.Volumes.Length - 1]);
	// 		}
	// 		else
	// 		{
	// 			Debug.LogError("AudioManager|LoadSettings: The number of Mixers and data points saved do not match!");
	// 			return;
	// 		}
	// 	}
	// }

	public void LoadSettings()
	{
		if (AreMixersSetupProperly)
		{
			AudioData data = AudioMSave.LoadData();

			if(data == null)
			{
				Debug.LogError("AudioManager|LoadSettings: AudioData saved file not found!");
				return;
			}

			if (data.MasterVolumes.Length == Mixers.Length)
			{
				for (int i = 0; i < Mixers.Length; i++)
				{
					Mixers[i].SetFloat("MasterVolume", data.MasterVolumes[i]);
					if(EnableDebug)
					{
						Debug.Log("AudioManager|LoadSettings: " + Mixers[i].name + "'s MasterVolume is " + data.MasterVolumes[i]);
					}
				}
			}
			else
			{
				Debug.LogError("AudioManager|LoadSettings: The number of Mixers (MasterVolume) and data points saved do not match!");
			}
			
			if(data.GroupVolumes.Length == MixerGroups.Length)
			{
				for (int i = 0; i < MixerGroups.Length; i++)
				{
					MixerGroups[i].audioMixer.SetFloat(MixerGroups[i].name + "Volume", data.GroupVolumes[i]);
					if(EnableDebug)
					{
						Debug.Log("AudioManager|LoadSettings: " + MixerGroups[i].audioMixer.name + "'s -> "  + MixerGroups[i].name + "Volume " + data.GroupVolumes[i]);	
					}
				}
			}
			else
			{
				Debug.LogError("AudioManager|LoadSettings: The number of MixerGroups and data points saved do not match!");
			}
		}
	}
	
	
	// Cooldown for testing
	IEnumerator Cooldown()
	{
		yield return new WaitForSeconds(1f);
		canPlay = true;
	}

}

[System.Serializable]
public class AudioData
{
	// public float[] Volumes{set; get;}
	
	public float[] MasterVolumes{get; set;}
	public float[] GroupVolumes{get; set;}
	
	private string EasterEgg = "Thanks for hacking my game!";
}