using System;
using System.Collections.Generic;
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
	
	private AudioMixer[] Mixers;
	private Dictionary<string, AudioMixer> _MixersDict;
	
	[Tooltip("Add all of your MixerGroups here. Make sure to include all of the ones being used for sounds.")]
	[SerializeField] private AudioMixerGroup[] MixerGroups;
	private Dictionary<string, AudioMixerGroup> _MixerGroupsDict;

	[Header("Audio Clips")]
	[Tooltip("Make your own sounds here. If you need to use custom spatial audio curves use SourceSounds.")]
	[SerializeField] private HolySound[] Sounds;
	private Dictionary<string, HolySound> _SoundsDict;
	
	[Tooltip("Add your clips as AudioSources first and then add those AudioSources here.")]
	[SerializeField] private HolySourceSound[] SourceSounds;
	private Dictionary<string, HolySourceSound> _SourceSoundsDict;


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
		_MixersDict = new Dictionary<string, AudioMixer>();
		_MixerGroupsDict = new Dictionary<string, AudioMixerGroup>();
		_SoundsDict = new Dictionary<string, HolySound>();
		_SourceSoundsDict = new Dictionary<string, HolySourceSound>();
		
		foreach (HolyMixerInfo mixer in MixersInfo)
		{
			if(_MixersDict.ContainsKey(mixer.Mixer.name) && EnableDebug)
			{
				Debug.Log("HolyAudioManager|Awake|Storing Mixers: Mixer " + mixer.Mixer.name +  " already exists in MixersDict, there is a duplicate name!");
				continue;
			}
			_MixersDict.Add(mixer.Mixer.name, mixer.Mixer);
			mixer.Mixer.updateMode = mixer.UpdateMode;
		}
		
		foreach (AudioMixerGroup group in MixerGroups)
		{
			if (_MixerGroupsDict.ContainsKey(group.name) && EnableDebug)
			{
				Debug.Log("HolyAudioManager|Awake|Storing MixerGroups: MixerGroup " + group.name + " already exists in MixerGroupsDict, there is a duplicate name!");
				continue;
			}
			_MixerGroupsDict.Add(group.name, group);
		}
		
		foreach (HolySound sound in Sounds)
		{
			if (_SoundsDict.ContainsKey(sound.ClipName) && EnableDebug)
			{
				Debug.Log("HolyAudioManager|Awake|Storing Sounds: Sound " + sound.ClipName + " already exists in SoundsDict, there is a duplicate name!");
				continue;
			}
			_SoundsDict.Add(sound.ClipName, sound);
		}

		foreach (HolySourceSound sourceSound in SourceSounds)
		{
			if (_SourceSoundsDict.ContainsKey(sourceSound.ClipName) && EnableDebug)
			{
				Debug.Log("HolyAudioManager|Awake|Storing SourceSounds: SourceSound " + sourceSound.ClipName + " already exists in SourceSoundsDict, there is a duplicate name!");
				continue;
			}
			_SourceSoundsDict.Add(sourceSound.ClipName, sourceSound);
		}
		
		foreach (KeyValuePair<string, HolySound> keyValuePair in _SoundsDict)
		{
			HolySound s = keyValuePair.Value;
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
			foreach (KeyValuePair<string, HolySound> keyValuePair in _SoundsDict)
			{
				keyValuePair.Value.Source.spatialBlend = 0f;
			}
			foreach (KeyValuePair<string, HolySourceSound> keyValuePair in _SourceSoundsDict)
			{
				keyValuePair.Value.Source.spatialBlend = 0f;
			}
		}
	}
	
	private void Start() {

	}
	
	
	// Play
	public void Play(string clipName)
	{
		HolySound s;
		if(_SoundsDict.TryGetValue(clipName, out s))
		{
			s.Source.PlayOneShot(s.Source.clip);
			DidExecuteSound = true;
			if (EnableDebug)
			{
				Debug.LogWarning("HolyAudioManager|Sound|Play: " + clipName + " has played!");
			}
		}
		else
		{
			SoundNotFound = true;
		}
		
		HolySourceSound ss;
		if (_SourceSoundsDict.TryGetValue(clipName, out ss))
		{
			ss.Source.PlayOneShot(ss.Source.clip);
			DidExecuteSourceSound = true;
			if (EnableDebug)
			{
				Debug.LogWarning("HolyAudioManager|SourceSound|Play: " + clipName + " has played!");
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
		if (_SoundsDict.TryGetValue(clipName, out s))
		{
			s.Source.Play();
			DidExecuteSound = true;
			if (EnableDebug)
			{
				Debug.LogWarning("HolyAudioManager|Sound|PlayOnce: " + clipName + " has played once!");
			}
		}
		else
		{
			SoundNotFound = true;
		}

		HolySourceSound ss;
		if (_SourceSoundsDict.TryGetValue(clipName, out ss))
		{
			ss.Source.Play();
			DidExecuteSourceSound = true;
			if (EnableDebug)
			{
				Debug.LogWarning("HolyAudioManager|SourceSound|PlayOnce: " + clipName + " has played once!");
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
		if (_SoundsDict.TryGetValue(clipName, out s))
		{
			s.Source.Pause();
			DidExecuteSound = true;
			if (EnableDebug)
			{
				Debug.LogWarning("HolyAudioManager|Sound|Pause: " + clipName + " has paused!");
			}
		}
		else
		{
			SoundNotFound = true;
		}

		HolySourceSound ss;
		if (_SourceSoundsDict.TryGetValue(clipName, out ss))
		{
			ss.Source.Pause();
			DidExecuteSourceSound = true;
			if (EnableDebug)
			{
				Debug.LogWarning("HolyAudioManager|SourceSound|Pause: " + clipName + " has paused!");
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
			if (!DoesMixerExist(mixerGroupName))
			{
				Debug.LogError("HolyAudioManager|PauseAllButMixerGroup: " + mixerGroupName + " does NOT exist!");
				return;
			}
		}

        foreach (KeyValuePair<string, HolySound> sPair in _SoundsDict)
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

        foreach (KeyValuePair<string, HolySourceSound> ssPair in _SourceSoundsDict)
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
			Debug.LogWarning("HolyAudioManager|PauseAllButMixerGroup: All sounds except from " + mixerGroupName + " have paused!");
		}
	}
	
	
	public void PauseAllFromMixerGroup(string mixerGroupName)
	{
		if (EnableDebug)
		{
			if (!DoesMixerExist(mixerGroupName))
			{
				Debug.LogError("HolyAudioManager|PauseAllFromMixerGroup: " + mixerGroupName + " does NOT exist!");
				return;
			}
		}

        foreach (KeyValuePair<string, HolySound> sPair in _SoundsDict)
		{
			if (sPair.Value.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				sPair.Value.Source.Pause();
			}
		}

        foreach (KeyValuePair<string, HolySourceSound> ssPair in _SourceSoundsDict)
		{
			if (ssPair.Value.Source.outputAudioMixerGroup.name == mixerGroupName)
			{
				ssPair.Value.Source.Pause();
			}
		}

		if (EnableDebug)
		{
			Debug.LogWarning("HolyAudioManager|PauseAllFromMixerGroup: Sounds in " + mixerGroupName + " have paused!");
		}
	}
	
	public void PauseAll()
	{
        foreach (KeyValuePair<string, HolySound> sPair in _SoundsDict)
		{
			sPair.Value.Source.Pause();
		}
        foreach (KeyValuePair<string, HolySourceSound> ssPair in _SourceSoundsDict)
		{
			ssPair.Value.Source.Pause();
		}

		if (EnableDebug)
		{
			Debug.LogWarning("HolyAudioManager|PauseAll: All sounds paused!");
		}
	}
	
	
	// Unpause
	public void Unpause(string clipName)
	{
        HolySound s;
        if (_SoundsDict.TryGetValue(clipName, out s))
        {
            s.Source.UnPause();
            DidExecuteSound = true;
            if (EnableDebug)
            {
                Debug.LogWarning("HolyAudioManager|Sound|Unpause: " + clipName + " has unpaused!");
            }
        }
        else
        {
            SoundNotFound = true;
        }

        HolySourceSound ss;
        if (_SourceSoundsDict.TryGetValue(clipName, out ss))
        {
            ss.Source.UnPause();
            DidExecuteSourceSound = true;
            if (EnableDebug)
            {
                Debug.LogWarning("HolyAudioManager|SourceSound|Unpause: " + clipName + " has unpaused!");
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
            if (!DoesMixerExist(mixerGroupName))
            {
                Debug.LogError("HolyAudioManager|UnpauseAllButMixerGroup: " + mixerGroupName + " does NOT exist!");
                return;
            }
        }

        foreach (KeyValuePair<string, HolySound> sPair in _SoundsDict)
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

        foreach (KeyValuePair<string, HolySourceSound> ssPair in _SourceSoundsDict)
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
            Debug.LogWarning("HolyAudioManager|UnpauseAllButMixerGroup: All sounds except from " + mixerGroupName + " have unpaused!");
        }
	}

	public void UnpauseAllFromMixerGroup(string mixerGroupName)
	{
        if (EnableDebug)
        {
            if (!DoesMixerExist(mixerGroupName))
            {
                Debug.LogError("HolyAudioManager|UnpauseAllFromMixerGroup: " + mixerGroupName + " does NOT exist!");
                return;
            }
        }

        foreach (KeyValuePair<string, HolySound> sPair in _SoundsDict)
        {
            if (sPair.Value.Source.outputAudioMixerGroup.name == mixerGroupName)
            {
                sPair.Value.Source.UnPause();
            }
        }

        foreach (KeyValuePair<string, HolySourceSound> ssPair in _SourceSoundsDict)
        {
            if (ssPair.Value.Source.outputAudioMixerGroup.name == mixerGroupName)
            {
                ssPair.Value.Source.UnPause();
            }
        }

        if (EnableDebug)
        {
            Debug.LogWarning("HolyAudioManager|UnpauseAllFromMixerGroup: Sounds in " + mixerGroupName + " have unpaused!");
        }
	}

	public void UnpauseAll()
	{
        foreach (KeyValuePair<string, HolySound> sPair in _SoundsDict)
        {
            sPair.Value.Source.UnPause();
        }
        foreach (KeyValuePair<string, HolySourceSound> ssPair in _SourceSoundsDict)
        {
            ssPair.Value.Source.UnPause();
        }

        if (EnableDebug)
        {
            Debug.LogWarning("HolyAudioManager|UnpauseAll: All sounds unpaused!");
        }
	}
	
	
	// Stop
	public void Stop(string clipName)
	{
        HolySound s;
        if (_SoundsDict.TryGetValue(clipName, out s))
        {
            s.Source.Stop();
            DidExecuteSound = true;
            if (EnableDebug)
            {
                Debug.LogWarning("HolyAudioManager|Sound|Stop: " + clipName + " has stopped!");
            }
        }
        else
        {
            SoundNotFound = true;
        }

        HolySourceSound ss;
        if (_SourceSoundsDict.TryGetValue(clipName, out ss))
        {
            ss.Source.Stop();
            DidExecuteSourceSound = true;
            if (EnableDebug)
            {
                Debug.LogWarning("HolyAudioManager|SourceSound|Stop: " + clipName + " has stopped!");
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
            if (!DoesMixerExist(mixerGroupName))
            {
                Debug.LogError("HolyAudioManager|StopAllButMixerGroup: " + mixerGroupName + " does NOT exist!");
                return;
            }
        }

        foreach (KeyValuePair<string, HolySound> sPair in _SoundsDict)
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

        foreach (KeyValuePair<string, HolySourceSound> ssPair in _SourceSoundsDict)
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
            Debug.LogWarning("HolyAudioManager|StopAllButMixerGroup: All sounds except from " + mixerGroupName + " have stopped!");
        }
	}
	
	public void StopAllFromMixerGroup(string mixerGroupName)
	{
        if (EnableDebug)
        {
            if (!DoesMixerExist(mixerGroupName))
            {
                Debug.LogError("HolyAudioManager|StopAllFromMixerGroup: " + mixerGroupName + " does NOT exist!");
                return;
            }
        }

        foreach (KeyValuePair<string, HolySound> sPair in _SoundsDict)
        {
            if (sPair.Value.Source.outputAudioMixerGroup.name == mixerGroupName)
            {
                sPair.Value.Source.Stop();
            }
        }

        foreach (KeyValuePair<string, HolySourceSound> ssPair in _SourceSoundsDict)
        {
            if (ssPair.Value.Source.outputAudioMixerGroup.name == mixerGroupName)
            {
                ssPair.Value.Source.Stop();
            }
        }

        if (EnableDebug)
        {
            Debug.LogWarning("HolyAudioManager|StopAllFromMixerGroup: Sounds in " + mixerGroupName + " have stopped!");
        }
	}
	
	public void StopAll()
	{
        foreach (KeyValuePair<string, HolySound> sPair in _SoundsDict)
        {
            sPair.Value.Source.Stop();
        }
        foreach (KeyValuePair<string, HolySourceSound> ssPair in _SourceSoundsDict)
        {
            ssPair.Value.Source.Stop();
        }

        if (EnableDebug)
        {
            Debug.LogWarning("HolyAudioManager|StopAll: All sounds stopped!");
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

		Debug.LogError("HolyAudioManager|GetMixer: " + mixerName + " does NOT exist!");
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

		Debug.LogError("HolyAudioManager|GetMixerGroup: " + mixerGroupName + " does NOT exist!");
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
					Debug.Log("HolyAudioManager|SaveSettings: " + Mixers[i].name + "'s MasterVolume is " + RawMasterVolumes[i]);
				}
			}
			
			for (int i = 0; i < MixerGroups.Length; i++)
			{
				MixerGroups[i].audioMixer.GetFloat(MixerGroups[i].name + "Volume", out RawGroupVolumes[i]);
				if (EnableDebug)
				{
					Debug.Log("HolyAudioManager|SaveSettings: " + MixerGroups[i].audioMixer.name + "'s -> " + MixerGroups[i].name + "Volume is " + RawGroupVolumes[i]);
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
				Debug.LogError("HolyAudioManager|LoadSettings: AudioData saved file not found!");
				return;
			}
			else if(data.AudioVersion != GameAudioVersion)
			{
				Debug.LogError("HolyAudioManager|LoadSettings: GameAudioVersion does not match! Settings will be reset!");
				return;
			}

			if (data.MasterVolumes.Length == Mixers.Length)
			{
				for (int i = 0; i < Mixers.Length; i++)
				{
					Mixers[i].SetFloat("MasterVolume", data.MasterVolumes[i]);
					if(EnableDebug)
					{
						Debug.Log("HolyAudioManager|LoadSettings: " + Mixers[i].name + "'s MasterVolume is " + data.MasterVolumes[i]);
					}
				}
			}
			else
			{
				Debug.LogError("HolyAudioManager|LoadSettings: The number of Mixers (MasterVolume) and data points saved do not match!");
			}
			
			if(data.GroupVolumes.Length == MixerGroups.Length)
			{
				for (int i = 0; i < MixerGroups.Length; i++)
				{
					MixerGroups[i].audioMixer.SetFloat(MixerGroups[i].name + "Volume", data.GroupVolumes[i]);
					if(EnableDebug)
					{
						Debug.Log("HolyAudioManager|LoadSettings: " + MixerGroups[i].audioMixer.name + "'s -> "  + MixerGroups[i].name + "Volume " + data.GroupVolumes[i]);	
					}
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
		AudioMixer m = Array.Find(Mixers, mixer => mixer.name == mixerName);
		float volume;
		if (m == null)
		{
			Debug.LogError("HolyAudioManager|GetMixerMasterVolume: No Mixer found by name: " + mixerName);
			return -69f;
		}
		else if(m.GetFloat("MasterVolume", out volume))
		{
			return volume;
		}
		else
		{
			Debug.LogError("HolyAudioManager|GetMixerMasterVolume: MasterVolume parameter not found!");
			return -69f;
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
			return -69f;
		}
	}
	public float GetMixerMasterVolume(int index)
	{
		float volume;
		if (index >= Mixers.Length)
		{
			Debug.LogError("HolyAudioManager|GetMixerMasterVolume: Index out of bounds");
			return 0;
		}
		else if(Mixers[index].GetFloat("MasterVolume", out volume))
		{
			return volume;
		}
		else
		{
			Debug.LogError("HolyAudioManager|GetMixerMasterVolume: MasterVolume parameter not found!");
			return -69f;
		}
	}

	public float GetMixerGroupVolume(string mixerGroupName)
	{
		AudioMixerGroup g = Array.Find(MixerGroups, group => group.name == mixerGroupName);
		float volume;
		if (g == null)
		{
			Debug.LogError("HolyAudioManager|GetMixerGroupVolume: No MixerGroup found by name: " + mixerGroupName);
			return -69f;
		}
		else if (g.audioMixer.GetFloat(g.name + "Volume", out volume))
		{
			return volume;
		}
		else
		{
			Debug.LogError("HolyAudioManager|GetMixerGroupVolume: " + g.name + "Volume " + " parameter not found!");
			return -69f;
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
			Debug.LogError("HolyAudioManager|GetMixerGroupVolume: " + mixerGroup.name + "Volume " + " parameter not found!");
			return -69f;
		}
	}
	public float GetMixerGroupVolume(int index)
	{
		float volume;
		if (index >= MixerGroups.Length)
		{
			Debug.LogError("HolyAudioManager|GetMixerGroupVolume: Index out of bounds");
			return -69f;
		}
		else if(MixerGroups[index].audioMixer.GetFloat(MixerGroups[index].name + "Volume", out volume))
		{
			return volume;
		}
		else
		{
			Debug.LogError("HolyAudioManager|GetMixerGroupVolume: " + MixerGroups[index].name + "Volume " + " parameter not found!");
			return -69f;
		}
	}
	
	
	// Set Master and Group Volumes
	public void SetMixerMasterVolume(string mixerName, float rawVolume)
	{
		AudioMixer m = Array.Find(Mixers, mixer => mixer.name == mixerName);
		if (m == null)
		{
			Debug.LogError("HolyAudioManager|SetMixerMasterVolume: No Mixer found by name: " + mixerName);
		}
		else if (m.SetFloat("MasterVolume", rawVolume))
		{
			return;
		}
		else
		{
			Debug.LogError("HolyAudioManager|SetMixerMasterVolume: MasterVolume parameter not found!");
		}
	}
	public void SetMixerMasterVolume(string mixerName, int percentVolume)
	{
		AudioMixer m = Array.Find(Mixers, mixer => mixer.name == mixerName);
		if (m == null)
		{
			Debug.LogError("HolyAudioManager|SetMixerMasterVolume: No Mixer found by name: " + mixerName);
		}
		else if (m.SetFloat("MasterVolume", PercentToDecibles(percentVolume)))
		{
			return;
		}
		else
		{
			Debug.LogError("HolyAudioManager|SetMixerMasterVolume: MasterVolume parameter not found!");
		}
	}
	public void SetMixerMasterVolume(AudioMixer mixer, float rawVolume)
	{
		if (mixer.SetFloat("MasterVolume", rawVolume)) 
		{
			return;
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
			return;
		}
		else
		{
			Debug.LogError("HolyAudioManager|SetMixerMasterVolume: MasterVolume parameter was not found!");
		}
	}
	public void SetMixerMasterVolume(int index, float rawVolume)
	{
		if (index >= Mixers.Length)
		{
			Debug.LogError("HolyAudioManager|SetMixerMasterVolume: Index out of bounds");
		}
		else if (Mixers[index].SetFloat("MasterVolume", rawVolume))
		{
			return;
		}
		else
		{
			Debug.LogError("HolyAudioManager|SetMixerMasterVolume: MasterVolume parameter was not found!");
		}
	}
	public void SetMixerMasterVolume(int index, int percentVolume)
	{
		if (index >= Mixers.Length)
		{
			Debug.LogError("HolyAudioManager|SetMixerMasterVolume: Index out of bounds");
		}
		else if (Mixers[index].SetFloat("MasterVolume", PercentToDecibles(percentVolume)))
		{
			return;
		}
		else
		{
			Debug.LogError("HolyAudioManager|SetMixerMasterVolume: MasterVolume parameter was not found!");
		}
	}

	public void SetMixerGroupVolume(string mixerGroupName, float rawVolume)
	{
		AudioMixerGroup g = Array.Find(MixerGroups, group => group.name == mixerGroupName);
		if (g == null)
		{
			Debug.LogError("HolyAudioManager|SetMixerGroupVolume: No MixerGroup found by name " + mixerGroupName);
		}
		else if(g.audioMixer.SetFloat(g.name + "Volume", rawVolume))
		{
			return;
		}
		else
		{
			Debug.LogError("HolyAudioManager|SetMixerGroupVolume: " + g.name + "Volume " + " parameter not found!");
		}
		
	}
	public void SetMixerGroupVolume(string mixerGroupName, int percentVolume)
	{
		AudioMixerGroup g = Array.Find(MixerGroups, group => group.name == mixerGroupName);
		if (g == null)
		{
			Debug.LogError("HolyAudioManager|SetMixerGroupVolume: No MixerGroup found by name " + mixerGroupName);
		}
		else if(g.audioMixer.SetFloat(g.name + "Volume", PercentToDecibles(percentVolume)))
		{
			return;
		}
		else
		{
			Debug.LogError("HolyAudioManager|SetMixerGroupVolume: " + g.name + "Volume " + " parameter not found!");
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
		if (index >= MixerGroups.Length)
		{
			Debug.LogError("HolyAudioManager|SetMixerMasterVolume: Index out of bounds");
		}
		else if (MixerGroups[index].audioMixer.SetFloat(MixerGroups[index].name + "Volume", PercentToDecibles(percentVolume)))
		{
			return;
		}
		else
		{
			Debug.LogError("HolyAudioManager|SetMixerGroupVolume: " + MixerGroups[index].name + "Volume" + " parameter was not found!");
		}
	}
	public void SetMixerGroupVolume(int index, float rawVolume)
	{
		if (index >= MixerGroups.Length)
		{
			Debug.LogError("HolyAudioManager|SetMixerMasterVolume: Index out of bounds");
		}
		else if (MixerGroups[index].audioMixer.SetFloat(MixerGroups[index].name + "Volume", rawVolume))
		{
			return;
		}
		else
		{
			Debug.LogError("HolyAudioManager|SetMixerGroupVolume: " + MixerGroups[index].name + "Volume" + " parameter was not found!");
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
				Debug.LogWarning("HolyAudioManager|DoesMixerExist: Mixer " + mixerName + " was not found!");
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
				Debug.LogWarning("HolyAudioManager|DoesMixerGroupExist: MixerGroup " + mixerGroupName + " was not found!");
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
				Debug.LogWarning("HolyAudioManager|DoesSoundExist: Sound " + soundName + " was not found!");
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
				Debug.LogWarning("HolyAudioManager|DoesSourceSoundExist: SourceSound " + sourceSoundName + " was not found!");
			}
			return false;
		}
		else
		{
			return true;
		}
	}

}