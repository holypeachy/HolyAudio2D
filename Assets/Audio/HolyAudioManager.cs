using System;
using UnityEngine;
using UnityEngine.Audio;

/*
	! Version: 0.0.0
	* 🍑HolyAudio2D
	* Thank you for using this little project of mine, I hope it is helpful in your endeavors! -holypeach
	? If you have any questions, suggestions, or find bugs here is the repo https://github.com/holypeachy/HolyAudio2D
*/

public class HolyAudioManager : MonoBehaviour
{
	// Instance Variable.
	private static HolyAudioManager HolyAudioManagerInstance;


	// Control Flow. Some options for debugging.
	[Header("Debugging")]
	
	[Tooltip("Only enable if they are so, for saving to work properly.")]
	[SerializeField] private  bool AreMixersSetupProperly = false;
	
	[Tooltip("Enabling might help with making sure sounds are being played. Only applied to HolyAudioM Sounds.")]
	[SerializeField] private bool DisableSpatialBlend = false;
	
	[Tooltip("Only applies to optional messages. Important Warnings and Errors will still be displayed.")]
	[SerializeField] private bool EnableDebug = true;
	
	[Tooltip("Keep this on during development. Turn this off before building the game. See documentation for more information.")]
	[SerializeField] private bool EnableImportantDebug = true;

	// Sounds and Mixers
	[Header("Mixers and Mixer Groups")]
	
	[Tooltip("Add all of your Mixers here. It also allows you to choose UpdateMode for each.")]
	[SerializeField] private HolyMixerInfo[] MixersInfo;
	
	private AudioMixer[] Mixers;
	
	[Tooltip("Add all of your MixerGroups here. Make sure to include all of the ones being used for sounds.")]
	[SerializeField] private AudioMixerGroup[] MixerGroups;

	[Header("Audio Clips")]
	[Tooltip("Make your own sounds here. If you need to use custom spatial audio curves use SourceSounds.")]
	[SerializeField] private HolySound[] Sounds;
	
	[Tooltip("Add your clips as AudioSources first and then add those AudioSources here.")]
	[SerializeField] private HolySourceSound[] SourceSounds;


	// Saving
	[Header("Saving")]
	[Tooltip("Make sure to add something here before working on things. Also make sure to change this value when you make changes to Mixers, MixerGroups, or the order of their respective arrays.")]
	[SerializeField] public string GameAudioVersion;


	// Play Control. These keep track of states in all methods for Play, Pause, and Stop.
	private bool DidExecuteSound = false;
	private bool DidExecuteSourceSound = false;
	private bool SoundNotFound = false;
	private bool SourceSoundNotFound = false;
	

	// All the important setup happens here.
	private void Awake()
	{
		// Keeps only one instance of HolyAudioM in game.
		if (HolyAudioManagerInstance == null)
		{
			HolyAudioManagerInstance = this;
		}
		else
		{
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);
		
		
		// We add the Mixers from the MixersInfo and set the UpdateMode.
		Mixers = new AudioMixer[MixersInfo.Length];
		for (int i = 0; i < MixersInfo.Length; i++)
		{
			Mixers[i] = MixersInfo[i].Mixer;
			Mixers[i].updateMode = MixersInfo[i].UpdateMode;
		}


		// We set up each sound as an AudioSource.
		foreach (HolySound s in Sounds)
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
			foreach (HolySourceSound s in SourceSounds)
			{
				s.Source.spatialBlend = 0f;
			}
			foreach (HolySound s in Sounds)
			{
				s.Source.spatialBlend = 0f;
			}
		}
	}
	
	
	// Play
	public void Play(string clipName)
	{
		// We look for clip in Sounds
		HolySound s = Array.Find(Sounds, sound => sound.ClipName == clipName);
		if (s == null)
		{
			SoundNotFound = true;
		}
		else
		{
			s.Source.PlayOneShot(s.Source.clip);
			if (EnableDebug)
			{
				Debug.LogWarning("HolyAudioM|Sound|Play: " + clipName + " has played!");
			}
			DidExecuteSound = true;
		}

		// Then we look for clip in SourceSounds
		HolySourceSound ss = Array.Find(SourceSounds, sound => sound.ClipName == clipName);
		if (ss == null)
		{
			SourceSoundNotFound = true;
		}
		else
		{
			ss.Source.PlayOneShot(ss.Source.clip);
			if (EnableDebug)
			{
				Debug.LogWarning("HolyAudioM|SourceSound|Play: " + clipName + " has played!");
			}
			DidExecuteSourceSound = true;
		}
		
		// Debug
		if(SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("HolyAudioM|Sounds&SourceSounds|Play: " + clipName + " does NOT exist!");
		}
		else if(DidExecuteSound && DidExecuteSourceSound)
		{
			Debug.LogWarning("HolyAudioM|Play: A clip has been found in Sounds and SourceSounds");
		}
	
		// We reset all the variables
		DidExecuteSound = false;
		DidExecuteSourceSound = false;
		SoundNotFound = false;
		SourceSoundNotFound = false;
	}

	public void PlayOnce(string clipName)
	{
		// We look for clip in Sounds
		HolySound s = Array.Find(Sounds, sound => sound.ClipName == clipName);
		if (s == null)
		{
			SoundNotFound = true;
		}
		else
		{
			s.Source.Play();
			if (EnableDebug)
			{
				Debug.LogWarning("HolyAudioM|Sound|PlayOnce: " + clipName + " has played!");
			}
			DidExecuteSound = true;
		}


		// Then we look for clip in SourceSounds
		HolySourceSound ss = Array.Find(SourceSounds, sound => sound.ClipName == clipName);
		if (ss == null)
		{
			SourceSoundNotFound = true;
		}
		else
		{
			ss.Source.Play();
			if (EnableDebug)
			{
				Debug.LogWarning("HolyAudioM|PlayOnce: " + clipName + " has played!");
			}
			DidExecuteSourceSound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("HolyAudioM|Sounds & SourceSounds|Play: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound)
		{
			Debug.LogError("HolyAudioM|Play: A clip has been found in Sounds and SourceSounds");
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
		HolySound s = Array.Find(Sounds, sound => sound.ClipName == clipName);
		if (s == null)
		{
			SoundNotFound = true;
		}
		else
		{
			s.Source.Pause();
			if (EnableDebug)
			{
				Debug.LogWarning("HolyAudioM|Sound|Pause: " + clipName + " has paused!");
			}
			DidExecuteSound = true;
		}


		// Then we look for clip in SourceSounds
		HolySourceSound ss = Array.Find(SourceSounds, sound => sound.ClipName == clipName);
		if (ss == null)
		{
			SourceSoundNotFound = true;
		}
		else
		{
			ss.Source.Pause();
			if (EnableDebug)
			{
				Debug.LogWarning("HolyAudioM|Pause: " + clipName + " has paused!");
			}
			DidExecuteSourceSound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("HolyAudioM|Sounds&SourceSounds|Pause: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound)
		{
			Debug.LogWarning("HolyAudioM|Pause: A clip has been found in Sounds and SourceSounds");
		}

		// We reset all the vars
		DidExecuteSound = false;
		DidExecuteSourceSound = false;
		SoundNotFound = false;
		SourceSoundNotFound = false;
	}

	public void PauseAllButMixerGroup(string mixerGroupName)
	{
		if (EnableImportantDebug)
		{
			if (!DoesMixerExist(mixerGroupName))
			{
				Debug.LogError("HolyAudioM|PauseAllButMixerGroup: " + mixerGroupName + " does NOT exist!");
				return;
			}
		}

		foreach (HolySound s in Sounds)
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

		foreach (HolySourceSound ss in SourceSounds)
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
			Debug.LogWarning("HolyAudioM|PauseAllButMixerGroup: All sounds except from " + mixerGroupName + " have paused!");
		}
	}
	
	public void PauseAllFromMixerGroup(string mixerGroupName)
	{
		if (EnableImportantDebug)
		{
			if (!DoesMixerExist(mixerGroupName))
			{
				Debug.LogError("HolyAudioM|PauseAllFromMixerGroup: " + mixerGroupName + " does NOT exist!");
				return;
			}
		}
		
		foreach (HolySound s in Sounds)
		{
			if (s.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				s.Source.Pause();
			}
		}

		foreach (HolySourceSound ss in SourceSounds)
		{
			if (ss.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				ss.Source.Pause();
			}
		}

		if (EnableDebug)
		{
			Debug.LogWarning("HolyAudioM|PauseAllFromMixerGroup: Sounds in " + mixerGroupName + " have paused!");
		}
	}
	
	public void PauseAll()
	{
		foreach (HolySound s in Sounds)
		{
			s.Source.Pause();
		}
		foreach (HolySourceSound ss in SourceSounds)
		{
			ss.Source.Pause();
		}

		if (EnableDebug)
		{
			Debug.LogWarning("HolyAudioM|PauseAll: All sounds paused!");
		}
	}
	
	
	// Unpause
	public void Unpause(string clipName)
	{
		// We look for clip in Sounds
		HolySound s = Array.Find(Sounds, sound => sound.ClipName == clipName);

		if (s == null)
		{
			SoundNotFound = true;
		}
		else
		{
			s.Source.UnPause();
			if (EnableDebug)
			{
				Debug.LogWarning("HolyAudioM|Sound|Pause: " + clipName + " has unpaused!");
			}
			DidExecuteSound = true;
		}


		// Then we look for clip in SourceSounds
		HolySourceSound ss = Array.Find(SourceSounds, sound => sound.ClipName == clipName);

		if (ss == null)
		{
			SourceSoundNotFound = true;
		}
		else
		{
			ss.Source.UnPause();
			if (EnableDebug)
			{
				Debug.LogWarning("HolyAudioM|Sound|Unpause: " + clipName + " has unpaused!");
			}
			DidExecuteSourceSound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("HolyAudioM|Sounds&SourceSounds|Unpause: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound && EnableDebug)
		{
			Debug.LogWarning("HolyAudioM|Unpause: A clip has been found in Sounds and SourceSounds");
		}

		// We reset all the vars
		DidExecuteSound = false;
		DidExecuteSourceSound = false;
		SoundNotFound = false;
		SourceSoundNotFound = false;
	}

	public void UnpauseAllButMixerGroup(string mixerGroupName)
	{
		if (EnableImportantDebug)
		{
			if (!DoesMixerExist(mixerGroupName))
			{
				Debug.LogError("HolyAudioM|UnpauseAllButMixerGroup: " + mixerGroupName + " does NOT exist!");
				return;
			}
		}
		
		foreach (HolySound s in Sounds)
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

		foreach (HolySourceSound ss in SourceSounds)
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
			Debug.LogWarning("HolyAudioM|UnpauseAllButMixerGroup: All sounds except from " + mixerGroupName + " have unpaused!");
		}
	}

	public void UnpauseAllFromMixerGroup(string mixerGroupName)
	{
		if (EnableImportantDebug)
		{
			if (!DoesMixerExist(mixerGroupName))
			{
				Debug.LogError("HolyAudioM|UnpauseAllButMixerGroup: " + mixerGroupName + " does NOT exist!");
				return;
			}
		}
		
		foreach (HolySound s in Sounds)
		{
			if (s.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				s.Source.UnPause();
			}
		}

		foreach (HolySourceSound ss in SourceSounds)
		{
			if (ss.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				ss.Source.UnPause();
			}
		}

		if (EnableDebug)
		{
			Debug.LogWarning("HolyAudioM|UnpauseFromMixerGroup: Sounds in " + mixerGroupName + " have unpaused!");
		}
	}

	public void UnpauseAll()
	{
		foreach (HolySound s in Sounds)
		{
			s.Source.UnPause();
		}
		foreach (HolySourceSound ss in SourceSounds)
		{
			ss.Source.UnPause();
		}

		if (EnableDebug)
		{
			Debug.LogWarning("HolyAudioM|UnpauseAll: All sounds unpaused!");
		}
	}
	
	
	// Stop
	public void Stop(string clipName)
	{
		// We look for clip in Sounds
		HolySound s = Array.Find(Sounds, sound => sound.ClipName == clipName);

		if (s == null)
		{
			SoundNotFound = true;
		}
		else
		{
			s.Source.Stop();
			if (EnableDebug)
			{
				Debug.LogWarning("HolyAudioM|Sound|Stop: " + clipName + " has stopped!");
			}
			DidExecuteSound = true;
		}


		// Then we look for clip in SourceSounds
		HolySourceSound ss = Array.Find(SourceSounds, sound => sound.ClipName == clipName);

		if (ss == null)
		{
			SourceSoundNotFound = true;
		}
		else
		{
			ss.Source.Stop();
			if (EnableDebug)
			{
				Debug.LogWarning("HolyAudioM|Sound|Stop: " + clipName + " has stopped!");
			}
			DidExecuteSourceSound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("HolyAudioM|Sounds&SourceSounds|Stop: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound)
		{
			Debug.LogWarning("HolyAudioM|Stop: A clip has been found in Sounds and SourceSounds");
		}

		// We reset all the vars
		DidExecuteSound = false;
		DidExecuteSourceSound = false;
		SoundNotFound = false;
		SourceSoundNotFound = false;
	}
	
	public void StopAllButMixerGroup(string mixerGroupName)
	{
		foreach (HolySound s in Sounds)
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

		foreach (HolySourceSound ss in SourceSounds)
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
			Debug.LogWarning("HolyAudioM|StopAllButMixerGroup: All sounds except from " + mixerGroupName + " have stopped!");
		}
		
	}
	
	public void StopAllFromMixerGroup(string mixerGroupName)
	{
		foreach (HolySound s in Sounds)
		{
			if (s.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				s.Source.Stop();
			}
		}

		foreach (HolySourceSound ss in SourceSounds)
		{
			if (ss.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				ss.Source.Stop();
			}
		}

		if (EnableDebug)
		{
			Debug.LogWarning("HolyAudioM|StopAllFromMixerGroup: Sounds in " + mixerGroupName + " have stopped!");
		}
	}
	
	public void StopAll()
	{
		foreach (HolySound s in Sounds)
		{
			s.Source.Stop();
		}
		foreach (HolySourceSound ss in SourceSounds)
		{
			ss.Source.Stop();
		}

		if(EnableDebug)
		{
			Debug.LogWarning("HolyAudioM|StopAll: All sounds stopped!");
		}
	}


	// Get MixerGroup(s)
	public AudioMixer GetMixer(string mixerName)
	{
		foreach (AudioMixer mixer in Mixers)
		{
			if (mixer.name == mixerName)
			{
				return mixer;
			}
		}

		Debug.LogError("HolyAudioM|GetMixer: " + mixerName + " does NOT exist!");
		return null;
	}
	
	public AudioMixer[] GetAllMixers()
	{
		return Mixers;
	}
	
	public AudioMixerGroup GetMixerGroup(string mixerGroupName)
	{
		foreach (AudioMixerGroup group in MixerGroups)
		{
			if(group.name == mixerGroupName)
			{
				return group;
			}
		}

		Debug.LogError("HolyAudioM|GetMixerGroup: " + mixerGroupName + " does NOT exist!");
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
					Debug.Log("HolyAudioM|SaveSettings: " + Mixers[i].name + "'s MasterVolume is " + RawMasterVolumes[i]);
				}
			}
			
			for (int i = 0; i < MixerGroups.Length; i++)
			{
				MixerGroups[i].audioMixer.GetFloat(MixerGroups[i].name + "Volume", out RawGroupVolumes[i]);
				if (EnableDebug)
				{
					Debug.Log("HolyAudioM|SaveSettings: " + MixerGroups[i].audioMixer.name + "'s -> " + MixerGroups[i].name + "Volume is " + RawGroupVolumes[i]);
				}
			}

			HolyAudioData data = new HolyAudioData();
			data.AudioVersion = GameAudioVersion;
			data.MasterVolumes = RawMasterVolumes;
			data.GroupVolumes = RawGroupVolumes;

			HolyAudioSaver.SaveData(data);
		}

	}

	public void LoadSettings()
	{
		if (AreMixersSetupProperly)
		{
			HolyAudioData data = HolyAudioSaver.LoadData();

			if(data == null)
			{
				Debug.LogError("HolyAudioM|LoadSettings: AudioData saved file not found!");
				return;
			}
			else if(data.AudioVersion != GameAudioVersion)
			{
				Debug.LogError("HolyAudioM|LoadSettings: GameAudioVersion does not match! Settings will be reset!");
				return;
			}

			if (data.MasterVolumes.Length == Mixers.Length)
			{
				for (int i = 0; i < Mixers.Length; i++)
				{
					Mixers[i].SetFloat("MasterVolume", data.MasterVolumes[i]);
					if(EnableDebug)
					{
						Debug.Log("HolyAudioM|LoadSettings: " + Mixers[i].name + "'s MasterVolume is " + data.MasterVolumes[i]);
					}
				}
			}
			else
			{
				Debug.LogError("HolyAudioM|LoadSettings: The number of Mixers (MasterVolume) and data points saved do not match!");
			}
			
			if(data.GroupVolumes.Length == MixerGroups.Length)
			{
				for (int i = 0; i < MixerGroups.Length; i++)
				{
					MixerGroups[i].audioMixer.SetFloat(MixerGroups[i].name + "Volume", data.GroupVolumes[i]);
					if(EnableDebug)
					{
						Debug.Log("HolyAudioM|LoadSettings: " + MixerGroups[i].audioMixer.name + "'s -> "  + MixerGroups[i].name + "Volume " + data.GroupVolumes[i]);	
					}
				}
			}
			else
			{
				Debug.LogError("HolyAudioM|LoadSettings: The number of MixerGroups and data points saved do not match!");
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
			Debug.LogError("HolyAudioM|GetMixerMasterVolume: No Mixer found by name: " + mixerName);
			return -69f;
		}
		else
		{
			m.GetFloat("MasterVolume", out volume);
			return volume;
		}
	}
	public float GetMixerMasterVolume(AudioMixer mixer)
	{
		float volume;
		if(mixer.GetFloat("MasterVolume", out volume)){}
		else
		{
			Debug.LogError("HolyAudioM|GetMixerMasterVolume: MasterVolume parameter was not found!");
		}
		return volume;
	}
	
	public float GetMixerGroupVolume(string mixerGroupName)
	{
		AudioMixerGroup g = Array.Find(MixerGroups, group => group.name == mixerGroupName);
		float volume;
		if (g == null)
		{
			Debug.LogError("HolyAudioM|GetMixerGroupVolume: No MixerGroup found by name: " + mixerGroupName);
			return -69f;
		}
		else
		{
			g.audioMixer.GetFloat(g.name + "Volume", out volume);
			return volume;
		}
	}
	public float GetMixerGroupVolume(AudioMixerGroup mixerGroup)
	{
		float volume;
		if (mixerGroup.audioMixer.GetFloat(mixerGroup.name + "Volume", out volume)) { }
		else
		{
			Debug.LogError("HolyAudioM|GetMixerGroupVolume: " + mixerGroup.name + "Volume" + " parameter was not found!");
		}
		return volume;
	}
	
	
	// Set Master and Group Volumes
	public void SetMixerMasterVolume(string mixerName, float rawVolume)
	{
		AudioMixer m = Array.Find(Mixers, mixer => mixer.name == mixerName);
		if (m == null)
		{
			Debug.LogError("HolyAudioM|SetMixerMasterVolume: No Mixer found by name: " + mixerName);
		}
		else
		{
			m.SetFloat("MasterVolume", rawVolume);
		}
	}
	public void SetMixerMasterVolume(AudioMixer mixer, float rawVolume)
	{
		if (mixer.SetFloat("MasterVolume", rawVolume)) { }
		else
		{
			Debug.LogError("HolyAudioM|SetMixerMasterVolume: MasterVolume parameter was not found!");
		}
	}

	public void SetMixerGroupVolume(string mixerGroupName, float rawVolume)
	{
		AudioMixerGroup g = Array.Find(MixerGroups, group => group.name == mixerGroupName);
		if (g == null)
		{
			Debug.LogError("HolyAudioM|SetMixerGroupVolume: No MixerGroup found by name " + mixerGroupName);
		}
		else
		{
			g.audioMixer.SetFloat(g.name + "Volume", rawVolume);
		}
	}
	public void SetMixerGroupVolume(AudioMixerGroup mixerGroup, float rawVolume)
	{
		if (mixerGroup.audioMixer.SetFloat(mixerGroup.name + "Volume", rawVolume)) { }
		else
		{
			Debug.LogError("HolyAudioM|SetMixerGroupVolume: " + mixerGroup.name + "Volume" + " parameter was not found!");
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
				Debug.LogWarning("HolyAudioM|DoesMixerExist: Mixer " + mixerName + " was not found!");
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
				Debug.LogWarning("HolyAudioM|DoesMixerGroupExist: MixerGroup " + mixerGroupName + " was not found!");
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
		HolySound sound = Array.Find(Sounds, sound => sound.ClipName == soundName);
		if (sound == null)
		{
			if (EnableDebug)
			{
				Debug.LogWarning("HolyAudioM|DoesSoundExist: Sound " + soundName + " was not found!");
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
		HolySourceSound sound = Array.Find(SourceSounds, sound => sound.ClipName == sourceSoundName);
		if (sound == null)
		{
			if (EnableDebug)
			{
				Debug.LogWarning("HolyAudioM|DoesSourceSoundExist: SourceSound " + sourceSoundName + " was not found!");
			}
			return false;
		}
		else
		{
			return true;
		}
	}

}