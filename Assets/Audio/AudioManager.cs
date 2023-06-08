using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
	// Instance Var
	private static AudioManager Instance;

	// Control Flow
	public bool AreMixersSetupProperly = true;
	public bool EnableDebug = true;
	public bool DisableSpatialBlend = false;
	
	// Play Control
	private bool DidExecuteSound = false;
	private bool DidExecuteSourceSound = false;
	private bool SoundNotFound = false;
	private bool SourceSoundNotFound = false;
	
	// Sounds and Mixers
	[SerializeField] private MixerInfo[] MixersInfo;
	private AudioMixer[] Mixers;
	[SerializeField] private AudioMixerGroup[] MixerGroups;
	[SerializeField] private Sound[] Sounds;
	[SerializeField] private SourceSound[] SourceSounds;
	
	// Saving
	[SerializeField] private string GameAudioVersion;
	
	// Testing
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
		
		Mixers = new AudioMixer[MixersInfo.Length];
		for (int i = 0; i < MixersInfo.Length; i++)
		{
			Mixers[i] = MixersInfo[i].Mixer;
			Mixers[i].updateMode = MixersInfo[i].UpdateMode;
		}

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
			
		}
		else if(load)
		{
			
		}
	}

	
	// Play
	public void Play(string clipName)
	{
		// We look for clip in Sounds
		Sound s = Array.Find(Sounds, sound => sound.ClipName == clipName);
		if (s == null)
		{
			SoundNotFound = true;
		}
		else
		{
			s.Source.PlayOneShot(s.Source.clip);
			if (EnableDebug)
			{
				Debug.LogWarning("AudioManager|Sound|Play: " + clipName + " has played!");
			}
			DidExecuteSound = true;
		}

		// Then we look for clip in SourceSounds
		SourceSound ss = Array.Find(SourceSounds, sound => sound.ClipName == clipName);
		if (ss == null)
		{
			SourceSoundNotFound = true;
		}
		else
		{
			ss.Source.PlayOneShot(ss.Source.clip);
			if (EnableDebug)
			{
				Debug.LogWarning("AudioManager|SourceSound|Play: " + clipName + " has played!");
			}
			DidExecuteSourceSound = true;
		}
		
		// Debug
		if(SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("AudioManager|Sounds&SourceSounds|Play: " + clipName + " does NOT exist!");
		}
		else if(DidExecuteSound && DidExecuteSourceSound)
		{
			Debug.LogWarning("AudioManager|Play: A clip has been found in Sounds and SourceSounds");
		}
	
		// We reset all the vars
		DidExecuteSound = false;
		DidExecuteSourceSound = false;
		SoundNotFound = false;
		SourceSoundNotFound = false;
	}

	public void PlayOnce(string clipName)
	{
		// We look for clip in Sounds
		Sound s = Array.Find(Sounds, sound => sound.ClipName == clipName);
		if (s == null)
		{
			SoundNotFound = true;
		}
		else
		{
			s.Source.Play();
			if (EnableDebug)
			{
				Debug.LogWarning("AudioManager|Sound|PlayOnce: " + clipName + " has played!");
			}
			DidExecuteSound = true;
		}


		// Then we look for clip in SourceSounds
		SourceSound ss = Array.Find(SourceSounds, sound => sound.ClipName == clipName);
		if (ss == null)
		{
			SourceSoundNotFound = true;
		}
		else
		{
			ss.Source.Play();
			if (EnableDebug)
			{
				Debug.LogWarning("AudioManager|PlayOnce: " + clipName + " has played!");
			}
			DidExecuteSourceSound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("AudioManager|Sounds & SourceSounds|Play: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound)
		{
			Debug.LogError("AudioManager|Play: A clip has been found in Sounds and SourceSounds");
		}

		// We reset all the vars
		DidExecuteSound = false;
		DidExecuteSourceSound = false;
		SoundNotFound = false;
		SourceSoundNotFound = false;
	}
	
	
	// Pause
	public void Pause(string clipName)
	{
		// We look for clip in Sounds
		Sound s = Array.Find(Sounds, sound => sound.ClipName == clipName);
		if (s == null)
		{
			SoundNotFound = true;
		}
		else
		{
			s.Source.Pause();
			if (EnableDebug)
			{
				Debug.LogWarning("AudioManager|Sound|Pause: " + clipName + " has paused!");
			}
			DidExecuteSound = true;
		}


		// Then we look for clip in SourceSounds
		SourceSound ss = Array.Find(SourceSounds, sound => sound.ClipName == clipName);
		if (ss == null)
		{
			SourceSoundNotFound = true;
		}
		else
		{
			ss.Source.Pause();
			if (EnableDebug)
			{
				Debug.LogWarning("AudioManager|Pause: " + clipName + " has paused!");
			}
			DidExecuteSourceSound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("AudioManager|Sounds&SourceSounds|Pause: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound)
		{
			Debug.LogWarning("AudioManager|Pause: A clip has been found in Sounds and SourceSounds");
		}

		// We reset all the vars
		DidExecuteSound = false;
		DidExecuteSourceSound = false;
		SoundNotFound = false;
		SourceSoundNotFound = false;
	}
	
	public void PauseAllButMixerGroup(string mixerGroupName)
	{
		if (EnableDebug)
		{
			if (!DoesMixerExist(mixerGroupName))
			{
				Debug.LogError("AudioManager|PauseAllButMixerGroup: " + mixerGroupName + " does NOT exist!");
				return;
			}
		}

		foreach (Sound s in Sounds)
		{
			if (s.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				continue;
			}
			else
			{
				s.Source.Pause();
			}
		}

		foreach (SourceSound ss in SourceSounds)
		{
			if (ss.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				continue;
			}
			else
			{
				ss.Source.Pause();
			}
		}

		if (EnableDebug)
		{
			Debug.LogWarning("AudioManager|PauseAllButMixerGroup: All sounds except from " + mixerGroupName + " have paused!");
		}
	}
	
	public void PauseAllFromMixerGroup(string mixerGroupName)
	{
		if (EnableDebug)
		{
			if (!DoesMixerExist(mixerGroupName))
			{
				Debug.LogError("AudioManager|PauseAllFromMixerGroup: " + mixerGroupName + " does NOT exist!");
				return;
			}
		}
		
		foreach (Sound s in Sounds)
		{
			if (s.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				s.Source.Pause();
			}
		}

		foreach (SourceSound ss in SourceSounds)
		{
			if (ss.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				ss.Source.Pause();
			}
		}

		if (EnableDebug)
		{
			Debug.LogWarning("AudioManager|PauseAllFromMixerGroup: Sounds in " + mixerGroupName + " have paused!");
		}
	}
	
	public void PauseAll()
	{
		foreach (Sound s in Sounds)
		{
			s.Source.Pause();
		}
		foreach (SourceSound ss in SourceSounds)
		{
			ss.Source.Pause();
		}

		if (EnableDebug)
		{
			Debug.LogWarning("AudioManager|PauseAll: All sounds paused!");
		}
	}
	
	
	// Unpause
	public void Unpause(string clipName)
	{
		// We look for clip in Sounds
		Sound s = Array.Find(Sounds, sound => sound.ClipName == clipName);

		if (s == null)
		{
			SoundNotFound = true;
		}
		else
		{
			s.Source.UnPause();
			if (EnableDebug)
			{
				Debug.LogWarning("AudioManager|Sound|Pause: " + clipName + " has unpaused!");
			}
			DidExecuteSound = true;
		}


		// Then we look for clip in SourceSounds
		SourceSound ss = Array.Find(SourceSounds, sound => sound.ClipName == clipName);

		if (ss == null)
		{
			SourceSoundNotFound = true;
		}
		else
		{
			ss.Source.UnPause();
			if (EnableDebug)
			{
				Debug.LogWarning("AudioManager|Sound|Unpause: " + clipName + " has unpaused!");
			}
			DidExecuteSourceSound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("AudioManager|Sounds&SourceSounds|Unpause: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound && EnableDebug)
		{
			Debug.LogWarning("AudioManager|Unpause: A clip has been found in Sounds and SourceSounds");
		}

		// We reset all the vars
		DidExecuteSound = false;
		DidExecuteSourceSound = false;
		SoundNotFound = false;
		SourceSoundNotFound = false;
	}

	public void UnpauseAllButMixerGroup(string mixerGroupName)
	{
		if (EnableDebug)
		{
			if (!DoesMixerExist(mixerGroupName))
			{
				Debug.LogError("AudioManager|UnpauseAllButMixerGroup: " + mixerGroupName + " does NOT exist!");
				return;
			}
		}
		
		foreach (Sound s in Sounds)
		{
			if (s.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				continue;
			}
			else
			{
				s.Source.UnPause();
			}
		}

		foreach (SourceSound ss in SourceSounds)
		{
			if (ss.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				continue;
			}
			else
			{
				ss.Source.UnPause();
			}
		}

		if (EnableDebug)
		{
			Debug.LogWarning("AudioManager|UnpauseAllButMixerGroup: All sounds except from " + mixerGroupName + " have unpaused!");
		}
	}

	public void UnpauseAllFromMixerGroup(string mixerGroupName)
	{
		if (EnableDebug)
		{
			if (!DoesMixerExist(mixerGroupName))
			{
				Debug.LogError("AudioManager|PauseAllButMixerGroup: " + mixerGroupName + " does NOT exist!");
				return;
			}
		}
		
		foreach (Sound s in Sounds)
		{
			if (s.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				s.Source.UnPause();
			}
		}

		foreach (SourceSound ss in SourceSounds)
		{
			if (ss.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				ss.Source.UnPause();
			}
		}

		if (EnableDebug)
		{
			Debug.LogWarning("AudioManager|UnpauseFromMixerGroup: Sounds in " + mixerGroupName + " have unpaused!");
		}
	}

	public void UnpauseAll()
	{
		foreach (Sound s in Sounds)
		{
			s.Source.UnPause();
		}
		foreach (SourceSound ss in SourceSounds)
		{
			ss.Source.UnPause();
		}

		if (EnableDebug)
		{
			Debug.LogWarning("AudioManager|UnpauseAll: All sounds unpaused!");
		}
	}
	
	
	// Stop
	public void Stop(string clipName)
	{
		// We look for clip in Sounds
		Sound s = Array.Find(Sounds, sound => sound.ClipName == clipName);

		if (s == null)
		{
			SoundNotFound = true;
		}
		else
		{
			s.Source.Stop();
			if (EnableDebug)
			{
				Debug.LogWarning("AudioManager|Sound|Stop: " + clipName + " has stopped!");
			}
			DidExecuteSound = true;
		}


		// Then we look for clip in SourceSounds
		SourceSound ss = Array.Find(SourceSounds, sound => sound.ClipName == clipName);

		if (ss == null)
		{
			SourceSoundNotFound = true;
		}
		else
		{
			ss.Source.Stop();
			if (EnableDebug)
			{
				Debug.LogWarning("AudioManager|Sound|Stop: " + clipName + " has stopped!");
			}
			DidExecuteSourceSound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("AudioManager|Sounds&SourceSounds|Stop: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound)
		{
			Debug.LogWarning("AudioManager|Stop: A clip has been found in Sounds and SourceSounds");
		}

		// We reset all the vars
		DidExecuteSound = false;
		DidExecuteSourceSound = false;
		SoundNotFound = false;
		SourceSoundNotFound = false;
	}
	
	public void StopAllButMixerGroup(string mixerGroupName)
	{
		foreach (Sound s in Sounds)
		{
			if(s.Source.outputAudioMixerGroup.name == mixerGroupName)
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
			if (ss.Source.outputAudioMixerGroup.name == mixerGroupName)
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
			Debug.LogWarning("AudioManager|StopAllButMixerGroup: All sounds except from " + mixerGroupName + " have stopped!");
		}
		
	}
	
	public void StopAllFromMixerGroup(string mixerGroupName)
	{
		foreach (Sound s in Sounds)
		{
			if (s.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				s.Source.Stop();
			}
		}

		foreach (SourceSound ss in SourceSounds)
		{
			if (ss.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				ss.Source.Stop();
			}
		}

		if (EnableDebug)
		{
			Debug.LogWarning("AudioManager|StopAllFromMixerGroup: Sounds in " + mixerGroupName + " have stopped!");
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


	// Get MixerGroup(s)
	public AudioMixerGroup GetAudioMixerGroup(string mixerGroupName)
	{
		foreach (AudioMixerGroup group in MixerGroups)
		{
			if(group.name == mixerGroupName)
			{
				return group;
			}
		}

		Debug.LogError("AudioManager|GetAudioMixerGroup: " + mixerGroupName + " does NOT exist!");
		return null;
	}
	
	public AudioMixerGroup[] GetAllMixerGroups()
	{
		return MixerGroups;	
	}


	// Saving and Loading
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
			data.AudioVersion = GameAudioVersion;
			data.MasterVolumes = RawMasterVolumes;
			data.GroupVolumes = RawGroupVolumes;

			AudioMSave.SaveData(data);
		}

	}

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
			else if(data.AudioVersion != GameAudioVersion)
			{
				Debug.LogError("AudioManager|LoadSettings: GameAudioVersion does not match! Settings will be reset!");
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
	
	
	// UI Interface
	// Get Master and Group Volumes
	public float GetMixerMasterVolume(string mixerName)
	{
		AudioMixer m = Array.Find(Mixers, mixer => mixer.name == mixerName);
		float volume;
		if (m == null)
		{
			Debug.LogError("AudioManager|GetMixerMasterVolume: No Mixer found by name: " + mixerName);
			return -69f;
		}
		else
		{
			m.GetFloat("MasterVolume", out volume);
			return volume;
		}
	}
	
	public float GetMixerGroupVolume(string mixerGroupName)
	{
		AudioMixerGroup g = Array.Find(MixerGroups, group => group.name == mixerGroupName);
		float volume;
		if (g == null)
		{
			Debug.LogError("AudioManager|GetMixerGroupVolume: No MixerGroup found by name: " + mixerGroupName);
			return -69f;
		}
		else
		{
			g.audioMixer.GetFloat(g.name + "Volume", out volume);
			return volume;
		}
	}
	
	
	// Set Master and Group Volumes
	public void SetMixerMasterVolume(string mixerName, float volume)
	{
		AudioMixer m = Array.Find(Mixers, mixer => mixer.name == mixerName);
		if (m == null)
		{
			Debug.LogError("AudioManager|SetMixerMasterVolume: No Mixer found by name: " + mixerName);
		}
		else
		{
			m.SetFloat("MasterVolume", volume);
		}
	}

	public void SetMixerGroupVolume(string mixerGroupName, float volume)
	{
		AudioMixerGroup g = Array.Find(MixerGroups, group => group.name == mixerGroupName);
		if (g == null)
		{
			Debug.LogError("AudioManager|SetMixerGroupVolume: No MixerGroup found by name: " + mixerGroupName);
		}
		else
		{
			g.audioMixer.SetFloat(g.name + "Volume", volume);
		}
	}

	
	// Helper methods for converting to and from dB(raw mixer volume) and Percent (0-100)
	public float DeciblesToPercent(float rawVolume)
	{
		return Math.Abs((rawVolume / -80f) - 1f) * 100;
	}
	
	public float PercentToDecibles(float percentVolume)
	{
		return (percentVolume / 100 * 80) - 80;
	}
	
	
	// Helper methods for checking if Mixers, MixerGroups, Sounds, or SourceSounds exist
	public bool DoesMixerExist(string mixerName)
	{
		AudioMixer mixer = Array.Find(Mixers, mixer => mixer.name == mixerName);
		if(mixer == null)
		{
			if(EnableDebug)
			{
				Debug.LogWarning("AudioManager|DoesMixerExist: Mixer " + mixerName + " was not found!");
			}
			return false;
		}
		else
		{
			return true;
		}
	}
	
	public bool DoesMixerGroupExist(string mixerGroupName)
	{
		AudioMixerGroup group = Array.Find(MixerGroups, group => group.name == mixerGroupName);
		if (group == null)
		{
			if (EnableDebug)
			{
				Debug.LogWarning("AudioManager|DoesMixerGroupExist: MixerGroup " + mixerGroupName + " was not found!");
			}
			return false;
		}
		else
		{
			return true;
		}
	}
	
	public bool DoesSoundExist(string soundName)
	{
		Sound sound = Array.Find(Sounds, sound => sound.ClipName == soundName);
		if (sound == null)
		{
			if (EnableDebug)
			{
				Debug.LogWarning("AudioManager|DoesSoundExist: Sound " + soundName + " was not found!");
			}
			return false;
		}
		else
		{
			return true;
		}
	}
	
	public bool DoesSourceSoundExist(string sourceSoundName)
	{
		SourceSound sound = Array.Find(SourceSounds, sound => sound.ClipName == sourceSoundName);
		if (sound == null)
		{
			if (EnableDebug)
			{
				Debug.LogWarning("AudioManager|DoesSourceSoundExist: SourceSound " + sourceSoundName + " was not found!");
			}
			return false;
		}
		else
		{
			return true;
		}
	}
	
	
	// Cooldown for testing
	IEnumerator Cooldown()
	{
		yield return new WaitForSeconds(1f);
		canPlay = true;
	}

}

/*
	* Commit:
	Title: Added methods that check if Mixers and Sounds exist. Sounds can now be paused and unpaused.
	- Added DoesMixerExist, DoesMixerGroupExist, DoesSoundExist, and DoesSourcesSoundExist methods.
	- Added DeciblesToPercent and PercentToDecibles methods to help convert volume levels for use with UI.
	- Added Getters and Setters for MixerMasterVolume and MixerGroupVolume for use with UI.
	- Added Pause, Unpause, PauseAllButMixerGroup, UnpauseAllButMixerGroup, PauseAllFromMixerGroup, UnpauseAllFromMixerGroup, PauseAll, and UnpauseAll methods.
	- Reformatted code for performance and readability.
	- Added MixerInfo class that stores a mixer and the updateMode for that mixer so it can be easily set up from the Inspector.
	- Added GameAudioVersion to keep track of the version of audio. Added AudioVersion string in AudioData class. If an old save doesn't match it will not be loaded.
*/