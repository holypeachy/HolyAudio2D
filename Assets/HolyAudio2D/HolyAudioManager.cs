using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

/*
	! Version: 0.1.1
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
	[SerializeField] private bool AreMixersSetupProperly = false;
	
	[Tooltip("Enabling might help with making sure sounds are being played. Only applied to HolyAudioManager Sounds.")]
	[SerializeField] private bool DisableSpatialBlend = false;
	
	[Tooltip("Applies to optional messages. Important Warnings and Errors will still be displayed but I recommend you keep it on during development or you might miss some things. Make sure to turn it off when you build the project.")]
	[SerializeField] private bool EnableDebug = true;

	// Sounds and Mixers
	[Header("Mixers and Mixer Groups")]
	
	[Tooltip("Add all of your Mixers here. It also allows you to choose UpdateMode for each.")]
	[SerializeField] private HolyMixerInfo[] MixersInfo;
	private Dictionary<string, AudioMixer> MixerDict;
	
	[Tooltip("Add all of your MixerGroups here. Make sure to include all of the ones being used for sounds.")]
	[SerializeField] private AudioMixerGroup[] MixerGroups;
	private Dictionary<string, AudioMixerGroup> MixerGroupDict;

	[Header("Audio Clips")]
	[Tooltip("Make your own sounds here. If you need to use custom spatial audio curves use SourceSounds.")]
	[SerializeField] private HolySound[] Sounds;
	private Dictionary<string, HolySound> SoundDict;
	
	[Tooltip("Add your clips as AudioSources first and then add those AudioSources here.")]
	[SerializeField] private HolySourceSound[] SourceSounds;
	private Dictionary<string, HolySourceSound> SourceSoundDict;


	// Saving
	[Header("Saving")]
	[Tooltip("Make sure to add something here before working on things. Also make sure to change this value when you make changes to Mixers, MixerGroups, or the order of their respective arrays.")]
	[SerializeField] public string GameAudioVersion;


	// Play Control. These keep track of states in all methods for Play, Pause, and Stop.
	private bool DidExecuteSound = false;
	private bool DidExecuteSourceSound = false;
	private bool SoundNotFound = false;
	private bool SourceSoundNotFound = false;

	// Memory
	HolySound sound;
	

	// All the important setup happens here.
	private void Awake()
	{
		// Keeps only one instance of HolyAudioManager in game.
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
		
		// Initializes Dictionaries
		MixerDict = new Dictionary<string, AudioMixer>();
		MixerGroupDict = new Dictionary<string, AudioMixerGroup>();
		SoundDict = new Dictionary<string, HolySound>();
		SourceSoundDict = new Dictionary<string, HolySourceSound>();
		
		foreach (HolyMixerInfo mixer in MixersInfo)
		{
			if(MixerDict.ContainsKey(mixer.Mixer.name) && EnableDebug)
			{
				Debug.Log("HolyAudioManager|Awake|Storing Mixers: Mixer " + mixer.Mixer.name +  " already exists in MixersDict, there is a duplicate name!");
				continue;
			}
			MixerDict.Add(mixer.Mixer.name, mixer.Mixer);
			mixer.Mixer.updateMode = mixer.UpdateMode;
		}
		
		foreach (AudioMixerGroup group in MixerGroups)
		{
			if (MixerGroupDict.ContainsKey(group.name) && EnableDebug)
			{
				Debug.Log("HolyAudioManager|Awake|Storing MixerGroups: MixerGroup " + group.name + " already exists in MixerGroupsDict, there is a duplicate name!");
				continue;
			}
			MixerGroupDict.Add(group.name, group);
		}
		
		foreach (HolySound sound in Sounds)
		{
			if (SoundDict.ContainsKey(sound.ClipName) && EnableDebug)
			{
				Debug.Log("HolyAudioManager|Awake|Storing Sounds: Sound " + sound.ClipName + " already exists in SoundsDict, there is a duplicate name!");
				continue;
			}
			SoundDict.Add(sound.ClipName, sound);
		}

		foreach (HolySourceSound sourceSound in SourceSounds)
		{
			if (SourceSoundDict.ContainsKey(sourceSound.ClipName) && EnableDebug)
			{
				Debug.Log("HolyAudioManager|Awake|Storing SourceSounds: SourceSound " + sourceSound.ClipName + " already exists in SourceSoundsDict, there is a duplicate name!");
				continue;
			}
			SourceSoundDict.Add(sourceSound.ClipName, sourceSound);
		}
		
		foreach (KeyValuePair<string, HolySound> keyValuePair in SoundDict)
		{
			sound = keyValuePair.Value;
			sound.Source = gameObject.AddComponent<AudioSource>();

			sound.Source.outputAudioMixerGroup = sound.MixerGroup;
			sound.Source.clip = sound.AudioFile;
			
			sound.Source.bypassEffects = sound.BypassEffects;
			sound.Source.bypassListenerEffects = sound.BypassListenerEffects;
			sound.Source.bypassReverbZones = sound.BypassReverbZones;
			
			sound.Source.playOnAwake = sound.PlayOnAwake;
			sound.Source.loop = sound.Loop;
			
			sound.Source.priority = sound.Priority;
			sound.Source.volume = sound.Volume;
			sound.Source.pitch = sound.Pitch;
			sound.Source.panStereo = sound.StereoPan;
			sound.Source.spatialBlend = sound.SpatialBlend;
			sound.Source.reverbZoneMix = sound.ReverbZoneMix;
			sound.Source.dopplerLevel = sound.DopplerLevel;
			sound.Source.spread = sound.Spread;
			
			sound.Source.rolloffMode = sound.VolumeRolloff;
			sound.Source.minDistance = sound.MinDistance;
			sound.Source.maxDistance = sound.MaxDistance;

			if (sound.PlayOnAwake)
			{
				sound.Source.Play();
			}
		}

		if(DisableSpatialBlend)
		{
			foreach (KeyValuePair<string, HolySound> keyValuePair in SoundDict)
			{
				keyValuePair.Value.Source.spatialBlend = 0f;
			}
			foreach (KeyValuePair<string, HolySourceSound> keyValuePair in SourceSoundDict)
			{
				keyValuePair.Value.Source.spatialBlend = 0f;
			}
		}
	}
	
	// ! For testing
	private void Start()
	{
		Play("Theme");
		PauseAllFromMixerGroup("Music");
	}

	// Play
	public void Play(string clipName)
	{
		HolySound s;
		if(SoundDict.TryGetValue(clipName, out s))
		{
			s.Source.PlayOneShot(s.Source.clip);
			DidExecuteSound = true;
			if (EnableDebug)
			{
				Debug.Log("HolyAudioManager|Sound|Play: " + clipName + " has played!");
			}
		}
		else
		{
			SoundNotFound = true;
		}
		
		HolySourceSound ss;
		if (SourceSoundDict.TryGetValue(clipName, out ss))
		{
			ss.Source.PlayOneShot(ss.Source.clip);
			DidExecuteSourceSound = true;
			if (EnableDebug)
			{
				Debug.Log("HolyAudioManager|SourceSound|Play: " + clipName + " has played!");
			}
		}
		else
		{
			SourceSoundNotFound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("HolyAudioManager|Sounds&SourceSounds|Play: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound)
		{
			Debug.LogWarning("HolyAudioManager|Sounds&SourceSounds|Play: A clip has been found in both Sounds and SourceSounds");
		}

		// We reset all the variables
		DidExecuteSound = false;
		DidExecuteSourceSound = false;
		SoundNotFound = false;
		SourceSoundNotFound = false;
	}

	public void PlayOnce(string clipName)
	{
		HolySound s;
		if (SoundDict.TryGetValue(clipName, out s))
		{
			s.Source.Play();
			DidExecuteSound = true;
			if (EnableDebug)
			{
				Debug.Log("HolyAudioManager|Sound|PlayOnce: " + clipName + " has played once!");
			}
		}
		else
		{
			SoundNotFound = true;
		}

		HolySourceSound ss;
		if (SourceSoundDict.TryGetValue(clipName, out ss))
		{
			ss.Source.Play();
			DidExecuteSourceSound = true;
			if (EnableDebug)
			{
				Debug.Log("HolyAudioManager|SourceSound|PlayOnce: " + clipName + " has played once!");
			}
		}
		else
		{
			SourceSoundNotFound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("HolyAudioManager|Sounds&SourceSounds|PlayOnce: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound)
		{
			Debug.LogWarning("HolyAudioManager|Sounds&SourceSounds|PlayOnce: A clip has been found in both Sounds and SourceSounds");
		}

		// We reset all the variables
		DidExecuteSound = false;
		DidExecuteSourceSound = false;
		SoundNotFound = false;
		SourceSoundNotFound = false;
	}
	
	
	// Pause
	public void Pause(string clipName)
	{
		HolySound s;
		if (SoundDict.TryGetValue(clipName, out s))
		{
			s.Source.Pause();
			DidExecuteSound = true;
			if (EnableDebug)
			{
				Debug.Log("HolyAudioManager|Sound|Pause: " + clipName + " has paused!");
			}
		}
		else
		{
			SoundNotFound = true;
		}

		HolySourceSound ss;
		if (SourceSoundDict.TryGetValue(clipName, out ss))
		{
			ss.Source.Pause();
			DidExecuteSourceSound = true;
			if (EnableDebug)
			{
				Debug.Log("HolyAudioManager|SourceSound|Pause: " + clipName + " has paused!");
			}
		}
		else
		{
			SourceSoundNotFound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("HolyAudioManager|Sounds&SourceSounds|Pause: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound)
		{
			Debug.LogWarning("HolyAudioManager|Sounds&SourceSounds|Pause: A clip has been found in both Sounds and SourceSounds");
		}

		// We reset all the variables
		DidExecuteSound = false;
		DidExecuteSourceSound = false;
		SoundNotFound = false;
		SourceSoundNotFound = false;
	}

	public void PauseAllButMixerGroup(string mixerGroupName)
	{
		if (EnableDebug)
		{
			if (!DoesMixerGroupExist(mixerGroupName))
			{
				Debug.LogError("HolyAudioManager|PauseAllButMixerGroup: " + mixerGroupName + " does NOT exist!");
				return;
			}
		}

		foreach (KeyValuePair<string, HolySound> sPair in SoundDict)
		{
			if (sPair.Value.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				continue;
			}
			else
			{
				sPair.Value.Source.Pause();
			}
		}

		foreach (KeyValuePair<string, HolySourceSound> ssPair in SourceSoundDict)
		{
			if (ssPair.Value.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				continue;
			}
			else
			{
				ssPair.Value.Source.Pause();
			}
		}

		if (EnableDebug)
		{
			Debug.Log("HolyAudioManager|PauseAllButMixerGroup: All sounds except from " + mixerGroupName + " have paused!");
		}
	}
	
	public void PauseAllFromMixerGroup(string mixerGroupName)
	{
		if (EnableDebug)
		{
			if (!DoesMixerGroupExist(mixerGroupName))
			{
				Debug.LogError("HolyAudioManager|PauseAllFromMixerGroup: " + mixerGroupName + " does NOT exist!");
				return;
			}
		}

		foreach (KeyValuePair<string, HolySound> sPair in SoundDict)
		{
			if (sPair.Value.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				sPair.Value.Source.Pause();
			}
		}

		foreach (KeyValuePair<string, HolySourceSound> ssPair in SourceSoundDict)
		{
			if (ssPair.Value.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				ssPair.Value.Source.Pause();
			}
		}

		if (EnableDebug)
		{
			Debug.Log("HolyAudioManager|PauseAllFromMixerGroup: Sounds in " + mixerGroupName + " have paused!");
		}
	}
	
	public void PauseAll()
	{
		foreach (KeyValuePair<string, HolySound> sPair in SoundDict)
		{
			sPair.Value.Source.Pause();
		}
		foreach (KeyValuePair<string, HolySourceSound> ssPair in SourceSoundDict)
		{
			ssPair.Value.Source.Pause();
		}

		if (EnableDebug)
		{
			Debug.Log("HolyAudioManager|PauseAll: All sounds paused!");
		}
	}
	
	
	// Unpause
	public void Unpause(string clipName)
	{
		HolySound s;
		if (SoundDict.TryGetValue(clipName, out s))
		{
			s.Source.UnPause();
			DidExecuteSound = true;
			if (EnableDebug)
			{
				Debug.Log("HolyAudioManager|Sound|Unpause: " + clipName + " has unpaused!");
			}
		}
		else
		{
			SoundNotFound = true;
		}

		HolySourceSound ss;
		if (SourceSoundDict.TryGetValue(clipName, out ss))
		{
			ss.Source.UnPause();
			DidExecuteSourceSound = true;
			if (EnableDebug)
			{
				Debug.Log("HolyAudioManager|SourceSound|Unpause: " + clipName + " has unpaused!");
			}
		}
		else
		{
			SourceSoundNotFound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("HolyAudioManager|Sounds&SourceSounds|Unpause: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound)
		{
			Debug.LogWarning("HolyAudioManager|Sounds&SourceSounds|Unpause: A clip has been found in both Sounds and SourceSounds");
		}

		// We reset all the variables
		DidExecuteSound = false;
		DidExecuteSourceSound = false;
		SoundNotFound = false;
		SourceSoundNotFound = false;
	}

	public void UnpauseAllButMixerGroup(string mixerGroupName)
	{
		if (EnableDebug)
		{
			if (!DoesMixerGroupExist(mixerGroupName))
			{
				Debug.LogError("HolyAudioManager|UnpauseAllButMixerGroup: " + mixerGroupName + " does NOT exist!");
				return;
			}
		}

		foreach (KeyValuePair<string, HolySound> sPair in SoundDict)
		{
			if (sPair.Value.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				continue;
			}
			else
			{
				sPair.Value.Source.UnPause();
			}
		}

		foreach (KeyValuePair<string, HolySourceSound> ssPair in SourceSoundDict)
		{
			if (ssPair.Value.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				continue;
			}
			else
			{
				ssPair.Value.Source.UnPause();
			}
		}

		if (EnableDebug)
		{
			Debug.Log("HolyAudioManager|UnpauseAllButMixerGroup: All sounds except from " + mixerGroupName + " have unpaused!");
		}
	}

	public void UnpauseAllFromMixerGroup(string mixerGroupName)
	{
		if (EnableDebug)
		{
			if (!DoesMixerGroupExist(mixerGroupName))
			{
				Debug.LogError("HolyAudioManager|UnpauseAllFromMixerGroup: " + mixerGroupName + " does NOT exist!");
				return;
			}
		}

		foreach (KeyValuePair<string, HolySound> sPair in SoundDict)
		{
			if (sPair.Value.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				sPair.Value.Source.UnPause();
			}
		}

		foreach (KeyValuePair<string, HolySourceSound> ssPair in SourceSoundDict)
		{
			if (ssPair.Value.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				ssPair.Value.Source.UnPause();
			}
		}

		if (EnableDebug)
		{
			Debug.Log("HolyAudioManager|UnpauseAllFromMixerGroup: Sounds in " + mixerGroupName + " have unpaused!");
		}
	}

	public void UnpauseAll()
	{
		foreach (KeyValuePair<string, HolySound> sPair in SoundDict)
		{
			sPair.Value.Source.UnPause();
		}
		foreach (KeyValuePair<string, HolySourceSound> ssPair in SourceSoundDict)
		{
			ssPair.Value.Source.UnPause();
		}

		if (EnableDebug)
		{
			Debug.Log("HolyAudioManager|UnpauseAll: All sounds unpaused!");
		}
	}
	
	
	// Stop
	public void Stop(string clipName)
	{
		HolySound s;
		if (SoundDict.TryGetValue(clipName, out s))
		{
			s.Source.Stop();
			DidExecuteSound = true;
			if (EnableDebug)
			{
				Debug.Log("HolyAudioManager|Sound|Stop: " + clipName + " has stopped!");
			}
		}
		else
		{
			SoundNotFound = true;
		}

		HolySourceSound ss;
		if (SourceSoundDict.TryGetValue(clipName, out ss))
		{
			ss.Source.Stop();
			DidExecuteSourceSound = true;
			if (EnableDebug)
			{
				Debug.Log("HolyAudioManager|SourceSound|Stop: " + clipName + " has stopped!");
			}
		}
		else
		{
			SourceSoundNotFound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("HolyAudioManager|Sounds&SourceSounds|Stop: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound)
		{
			Debug.LogWarning("HolyAudioManager|Sounds&SourceSounds|Stop: A clip has been found in both Sounds and SourceSounds");
		}

		// We reset all the variables
		DidExecuteSound = false;
		DidExecuteSourceSound = false;
		SoundNotFound = false;
		SourceSoundNotFound = false;
	}
	
	public void StopAllButMixerGroup(string mixerGroupName)
	{
		if (EnableDebug)
		{
			if (!DoesMixerGroupExist(mixerGroupName))
			{
				Debug.LogError("HolyAudioManager|StopAllButMixerGroup: " + mixerGroupName + " does NOT exist!");
				return;
			}
		}

		foreach (KeyValuePair<string, HolySound> sPair in SoundDict)
		{
			if (sPair.Value.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				continue;
			}
			else
			{
				sPair.Value.Source.Stop();
			}
		}

		foreach (KeyValuePair<string, HolySourceSound> ssPair in SourceSoundDict)
		{
			if (ssPair.Value.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				continue;
			}
			else
			{
				ssPair.Value.Source.Stop();
			}
		}

		if (EnableDebug)
		{
			Debug.Log("HolyAudioManager|StopAllButMixerGroup: All sounds except from " + mixerGroupName + " have stopped!");
		}
	}
	
	public void StopAllFromMixerGroup(string mixerGroupName)
	{
		if (EnableDebug)
		{
			if (!DoesMixerGroupExist(mixerGroupName))
			{
				Debug.LogError("HolyAudioManager|StopAllFromMixerGroup: " + mixerGroupName + " does NOT exist!");
				return;
			}
		}

		foreach (KeyValuePair<string, HolySound> sPair in SoundDict)
		{
			if (sPair.Value.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				sPair.Value.Source.Stop();
			}
		}

		foreach (KeyValuePair<string, HolySourceSound> ssPair in SourceSoundDict)
		{
			if (ssPair.Value.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				ssPair.Value.Source.Stop();
			}
		}

		if (EnableDebug)
		{
			Debug.Log("HolyAudioManager|StopAllFromMixerGroup: Sounds in " + mixerGroupName + " have stopped!");
		}
	}
	
	public void StopAll()
	{
		foreach (KeyValuePair<string, HolySound> sPair in SoundDict)
		{
			sPair.Value.Source.Stop();
		}
		foreach (KeyValuePair<string, HolySourceSound> ssPair in SourceSoundDict)
		{
			ssPair.Value.Source.Stop();
		}

		if (EnableDebug)
		{
			Debug.Log("HolyAudioManager|StopAll: All sounds stopped!");
		}
	}


	// Get MixerGroup(s)
	public AudioMixer GetMixer(string mixerName)
	{
		AudioMixer mixer;
		if(MixerDict.TryGetValue(mixerName, out mixer))
		{
			return mixer;
		}

		Debug.LogError("HolyAudioManager|GetMixer: " + mixerName + " does NOT exist!");
		return null;
	}
	
	public Dictionary<string, AudioMixer> GetAllMixers()
	{
		return MixerDict;
	}
	
	public AudioMixerGroup GetMixerGroup(string mixerGroupName)
	{
		AudioMixerGroup group;
		if (MixerGroupDict.TryGetValue(mixerGroupName, out group))
		{
			return group;
		}

		Debug.LogError("HolyAudioManager|GetMixerGroup: " + mixerGroupName + " does NOT exist!");
		return null;
	}
	
	public Dictionary<string, AudioMixerGroup> GetAllMixerGroups()
	{
		return MixerGroupDict;	
	}


	// Saving and Loading
	public void SaveSettings()
	{
		if (AreMixersSetupProperly)
		{
			float[] RawMasterVolumes = new float[MixerDict.Count];
			float[] RawGroupVolumes = new float[MixerGroupDict.Count];
			
			int counter = 0;
			foreach (KeyValuePair<string, AudioMixer> mixerPair in MixerDict)
			{
				mixerPair.Value.GetFloat("MasterVolume", out RawMasterVolumes[counter]);
                if (EnableDebug)
                {
                    Debug.Log("HolyAudioManager|SaveSettings: " + mixerPair.Value.name + "'s MasterVolume is " + RawMasterVolumes[counter]);
                }
				counter++;
			}

            counter = 0;
            foreach (KeyValuePair<string, AudioMixerGroup> groupPair in MixerGroupDict)
            {
                groupPair.Value.audioMixer.GetFloat(groupPair.Value.name + "Volume", out RawGroupVolumes[counter]);
                if (EnableDebug)
                {
					Debug.Log("HolyAudioManager|SaveSettings: " + groupPair.Value.audioMixer.name + "'s -> " + groupPair.Value.name + "Volume is " + RawGroupVolumes[counter]);
                }
                counter++;
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

            if (data == null)
            {
                Debug.LogError("HolyAudioManager|LoadSettings: AudioData saved file not found!");
                return;
            }

            else if (data.AudioVersion != GameAudioVersion)
            {
                Debug.LogError("HolyAudioManager|LoadSettings: GameAudioVersion does not match! Settings will be reset!");
                return;
            }

            if (data.MasterVolumes.Length == MixerDict.Count)
            {
                int counter = 0;
                foreach (KeyValuePair<string, AudioMixer> mixerPair in MixerDict)
                {
                    mixerPair.Value.SetFloat("MasterVolume", data.MasterVolumes[counter]);
                    if (EnableDebug)
                    {
                        Debug.Log("HolyAudioManager|LoadSettings: " + mixerPair.Value.name + "'s MasterVolume was set to " + data.MasterVolumes[counter]);
                    }
                    counter++;
                }
            }
            else
            {
                Debug.LogError("HolyAudioManager|LoadSettings: The number of Mixers (MasterVolume) and data points saved do not match!");
            }

            if (data.GroupVolumes.Length == MixerGroups.Length)
            {
                int counter = 0;
                foreach (KeyValuePair<string, AudioMixerGroup> groupPair in MixerGroupDict)
                {
                    groupPair.Value.audioMixer.SetFloat(groupPair.Value.name + "Volume", data.GroupVolumes[counter]);
                    if (EnableDebug)
                    {
						Debug.Log("HolyAudioManager|Load: " + groupPair.Value.audioMixer.name + "'s -> " + groupPair.Value.name + "Volume was set to " + data.GroupVolumes[counter]);
                    }
                    counter++;
                }
            }
            else
            {
                Debug.LogError("HolyAudioManager|LoadSettings: The number of MixerGroups and data points saved do not match!");
            }
        }
    }
	
	
	// UI Interface
	// Get Master and Group Volumes
	public float GetMixerMasterVolume(string mixerName)
	{		
		AudioMixer mixer;
		float volume;
		if(MixerDict.TryGetValue(mixerName, out mixer))
		{
			if(mixer.GetFloat("MasterVolume", out volume))
			{
				
				return volume;
			}
			else
			{
				Debug.LogError("HolyAudioManager|GetMixerMasterVolume: MasterVolume parameter not found!");
				return -99f;
			}
		}
		else
		{
			Debug.LogError("HolyAudioManager|GetMixerMasterVolume: No Mixer found by name: " + mixerName);
			return -99f;
		}
	}
	public float GetMixerMasterVolume(AudioMixer mixer)
	{
		float volume;
		if(mixer.GetFloat("MasterVolume", out volume))
		{
			return volume;
		}
		else
		{
			Debug.LogError("HolyAudioManager|GetMixerMasterVolume: MasterVolume parameter not found!");
			return -99f;
		}
	}
	public float GetMixerMasterVolume(int index)
	{
		AudioMixer mixer;
		float volume;
		if (index < MixerDict.Count && !(index < 0))
		{
			mixer = MixerDict.ElementAt(index).Value;
			if (mixer.GetFloat("MasterVolume", out volume))
			{

				return volume;
			}
			else
			{
				Debug.LogError("HolyAudioManager|GetMixerMasterVolume: MasterVolume parameter not found!");
				return -99f;
			}
		}
		else
		{
			Debug.LogError("HolyAudioManager|GetMixerMasterVolume: No Mixer found at index " + index);
			return -99f;
		}
	}

	public float GetMixerGroupVolume(string mixerGroupName)
	{
		AudioMixerGroup group;
		float volume;
		if (MixerGroupDict.TryGetValue(mixerGroupName, out group))
		{
			if (group.audioMixer.GetFloat(group.name + "Volume", out volume))
			{

				return volume;
			}
			else
			{
				Debug.LogError("HolyAudioManager|GetMixerGroupVolume: " + group.name + "Volume parameter not found!");
				return -99f;
			}
		}
		else
		{
			Debug.LogError("HolyAudioManager|GetMixerGroupVolume: No MixerGroup found by name " + mixerGroupName);
			return -99f;
		}
	}
	public float GetMixerGroupVolume(AudioMixerGroup mixerGroup)
	{
		float volume;
		if(mixerGroup.audioMixer.GetFloat(mixerGroup.name + "Volume", out volume))
		{
			return volume;
		}
		else
		{
			Debug.LogError("HolyAudioManager|GetMixerGroupVolume: " + mixerGroup.name + "Volume parameter not found!");
			return -99f;
		}
	}
	public float GetMixerGroupVolume(int index)
	{
		AudioMixerGroup group;
		float volume;
		if (index < MixerGroupDict.Count && !(index < 0))
		{
			group = MixerGroupDict.ElementAt(index).Value;
			if (group.audioMixer.GetFloat(group.name + "Volume", out volume))
			{

				return volume;
			}
			else
			{
				Debug.LogError("HolyAudioManager|GetMixerGroupVolume: " + group.name + "Volume parameter not found!");
				return -99f;
			}
		}
		else
		{
			Debug.LogError("HolyAudioManager|GetMixerGroupVolume: No MixerGroup found at index " + index);
			return -99f;
		}
	}
	

	// Set Master and Group Volumes
	public void SetMixerMasterVolume(string mixerName, float rawVolume)
	{
		AudioMixer mixer;
		if (MixerDict.TryGetValue(mixerName, out mixer))
		{
			if (mixer.SetFloat("MasterVolume", rawVolume))
			{
				if (EnableDebug)
				{
					Debug.Log("HolyAudioManager|SetMixerMasterVolume: " + mixerName + " Mixer's volume was set to " + rawVolume + "dB");
				}
			}
			else
			{
				Debug.LogError("HolyAudioManager|SetMixerMasterVolume: MasterVolume parameter not found!");
			}
		}
		else
		{
			Debug.LogError("HolyAudioManager|SetMixerMasterVolume: No Mixer found by name " + mixerName);
		}
	}
	public void SetMixerMasterVolume(string mixerName, int percentVolume)
	{
		AudioMixer mixer;
		if (MixerDict.TryGetValue(mixerName, out mixer))
		{
			if (mixer.SetFloat("MasterVolume", PercentToDecibles(percentVolume)))
			{
				if (EnableDebug)
				{
					Debug.Log("HolyAudioManager|SetMixerMasterVolume: " + mixerName + " Mixer's volume was set to " + percentVolume + "%");
				}
			}
			else
			{
				Debug.LogError("HolyAudioManager|SetMixerMasterVolume: MasterVolume parameter not found!");
			}
		}
		else
		{
			Debug.LogError("HolyAudioManager|SetMixerMasterVolume: No Mixer found by name " + mixerName);
		}
	}
	public void SetMixerMasterVolume(AudioMixer mixer, float rawVolume)
	{
		if (mixer.SetFloat("MasterVolume", rawVolume)) 
		{
			if (EnableDebug)
			{
				Debug.Log("HolyAudioManager|SetMixerMasterVolume: " + mixer.name + " Mixer's volume was set to " + rawVolume + "dB");
			}
		}
		else
		{
			Debug.LogError("HolyAudioManager|SetMixerMasterVolume: MasterVolume parameter was not found!");
		}
		
	}
	public void SetMixerMasterVolume(AudioMixer mixer, int percentVolume)
	{
		if (mixer.SetFloat("MasterVolume", PercentToDecibles(percentVolume))) 
		{
			if (EnableDebug)
			{
				Debug.Log("HolyAudioManager|SetMixerMasterVolume: " + mixer.name + " Mixer's volume was set to " + percentVolume + "%");
			}
		}
		else
		{
			Debug.LogError("HolyAudioManager|SetMixerMasterVolume: MasterVolume parameter was not found!");
		}
	}
	public void SetMixerMasterVolume(int index, float rawVolume)
	{
		AudioMixer mixer;
		if (index < MixerDict.Count && !(index < 0))
		{
			mixer = MixerDict.ElementAt(index).Value;
			if (mixer.SetFloat("MasterVolume", rawVolume))
			{
				if (EnableDebug)
				{
					Debug.Log("HolyAudioManager|SetMixerMasterVolume: " + mixer.name + " mixer's volume was set to " + rawVolume + "dB");
				}
			}
			else
			{
				Debug.LogError("HolyAudioManager|SetMixerMasterVolume: MasterVolume parameter was not found!");
			}
		}
		else
		{
			Debug.LogError("HolyAudioManager|SetMixerMasterVolume: Index out of bounds");
		}
	}
	public void SetMixerMasterVolume(int index, int percentVolume)
	{
		AudioMixer mixer;
		if (index < MixerDict.Count && !(index < 0))
		{
			mixer = MixerDict.ElementAt(index).Value;
			if (mixer.SetFloat("MasterVolume", PercentToDecibles(percentVolume)))
			{
				if (EnableDebug)
				{
					Debug.Log("HolyAudioManager|SetMixerMasterVolume: " + mixer.name + " mixer's volume was set to " + percentVolume + "%");
				}
			}
			else
			{
				Debug.LogError("HolyAudioManager|SetMixerMasterVolume: MasterVolume parameter was not found!");
			}
		}
		else
		{
			Debug.LogError("HolyAudioManager|SetMixerMasterVolume: Index out of bounds");
		}
	}

	public void SetMixerGroupVolume(string mixerGroupName, float rawVolume)
	{
		AudioMixerGroup group;
		if (MixerGroupDict.TryGetValue(mixerGroupName, out group))
		{
			if (group.audioMixer.SetFloat(group.name + "Volume", rawVolume))
			{
				if (EnableDebug)
				{
					Debug.Log("HolyAudioManager|SetMixerGroupVolume: " + group.name + " MixerGroup's volume was set to " + rawVolume + "dB");
				}
			}
			else
			{
				Debug.LogError("HolyAudioManager|SetMixerGroupVolume: " + group.name + "Volume parameter not found!");
			}
		}
		else
		{
			Debug.LogError("HolyAudioManager|SetMixerGroupVolume: No MixerGroup found by name " + mixerGroupName);
		}
	}
	public void SetMixerGroupVolume(string mixerGroupName, int percentVolume)
	{
		AudioMixerGroup group;
		if (MixerGroupDict.TryGetValue(mixerGroupName, out group))
		{
			if (group.audioMixer.SetFloat(group.name + "Volume", PercentToDecibles(percentVolume)))
			{
				if (EnableDebug)
				{
					Debug.Log("HolyAudioManager|SetMixerGroupVolume: " + group.name + " MixerGroup's volume was set to " + percentVolume + "%");
				}
			}
			else
			{
				Debug.LogError("HolyAudioManager|SetMixerGroupVolume: " + group.name + "Volume parameter not found!");
			}
		}
		else
		{
			Debug.LogError("HolyAudioManager|SetMixerGroupVolume: No MixerGroup found by name " + mixerGroupName);
		}
	}
	public void SetMixerGroupVolume(AudioMixerGroup mixerGroup, float rawVolume)
	{
		if (mixerGroup.audioMixer.SetFloat(mixerGroup.name + "Volume", rawVolume))
		{ 
			return;
		}
		else
		{
			Debug.LogError("HolyAudioManager|SetMixerGroupVolume: " + mixerGroup.name + "Volume" + " parameter was not found!");
		}
	}
	public void SetMixerGroupVolume(AudioMixerGroup mixerGroup, int percentVolume)
	{
		if (mixerGroup.audioMixer.SetFloat(mixerGroup.name + "Volume", PercentToDecibles(percentVolume)))
		{
			return;
		}
		else
		{
			Debug.LogError("HolyAudioManager|SetMixerGroupVolume: " + mixerGroup.name + "Volume" + " parameter was not found!");
		}
	}
	public void SetMixerGroupVolume(int index, int percentVolume)
	{
		AudioMixerGroup group;
		if (index < MixerGroupDict.Count && !(index < 0))
		{
			group = MixerGroupDict.ElementAt(index).Value;
			if (group.audioMixer.SetFloat(group.name + "Volume", PercentToDecibles(percentVolume)))
			{
				if (EnableDebug)
				{
					Debug.Log("HolyAudioManager|SetMixerGroupVolume: " + group.name + " MixerGroup's volume was set to " + percentVolume + "%");
				}
			}
			else
			{
				Debug.LogError("HolyAudioManager|SetMixerGroupVolume: " + group.name + "Volume parameter not found!");
			}
		}
		else
		{
			Debug.LogError("HolyAudioManager|SetMixerGroupVolume: Index out of bounds");
		}
	}
	public void SetMixerGroupVolume(int index, float rawVolume)
	{
		AudioMixerGroup group;
		if (index < MixerGroupDict.Count && !(index < 0))
		{
			group = MixerGroupDict.ElementAt(index).Value;
			if (group.audioMixer.SetFloat(group.name + "Volume", rawVolume))
			{
				if (EnableDebug)
				{
					Debug.Log("HolyAudioManager|SetMixerGroupVolume: " + group.name + " MixerGroup's volume was set to " + rawVolume + "dB");
				}
			}
			else
			{
				Debug.LogError("HolyAudioManager|SetMixerGroupVolume: " + group.name + "Volume parameter not found!");
			}
		}
		else
		{
			Debug.LogError("HolyAudioManager|SetMixerGroupVolume: Index out of bounds");
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
		if (MixerDict.ContainsKey(mixerName))
		{
			return true;
		}
		else
		{
			if (EnableDebug)
			{
				Debug.LogWarning("HolyAudioManager|DoesMixerExist: Mixer " + mixerName + " was NOT found!");
			}
			return false;
		}
	}
	
	public bool DoesMixerGroupExist(string mixerGroupName)
	{
		if (MixerGroupDict.ContainsKey(mixerGroupName))
		{
			return true;
		}
		else
		{
			if (EnableDebug)
			{
				Debug.LogWarning("HolyAudioManager|DoesMixerGroupExist: MixerGroup " + mixerGroupName + " was NOT found!");
			}
			return false;
		}
	}
	
	public bool DoesSoundExist(string soundName)
	{
		if (SoundDict.ContainsKey(soundName))
		{
			return true;
		}
		else
		{
			if (EnableDebug)
			{
				Debug.LogWarning("HolyAudioManager|DoesSoundExist: Sound " + soundName + " was NOT found!");
			}
			return false;
		}
	}
	
	public bool DoesSourceSoundExist(string sourceSoundName)
	{		
		if (SourceSoundDict.ContainsKey(sourceSoundName))
		{
			return true;
		}
		else
		{
			if (EnableDebug)
			{
				Debug.LogWarning("HolyAudioManager|DoesSourceSoundExist: SourceSound " + sourceSoundName + " was NOT found!");
			}
			return false;
		}
	}

}