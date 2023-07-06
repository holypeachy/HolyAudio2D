using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class HolyLocalAudio : MonoBehaviour
{
	// Reference to the HolyAudioManager
	[HideInInspector] public HolyAudioManager GlobalHolyAudioManager;

    // Control Flow and some options for debugging.
    [Header("Debugging")]

    [Tooltip("Enabling might help with making sure sounds are being played. Only applied to HolyLocalAudio Sounds.")]
    [SerializeField] private bool DisableSpatialBlend = false;

    [Tooltip("Applies to optional messages. Important Warnings and Errors will still be displayed but I recommend you keep it on during development or you might miss some things. Make sure to turn it off when you build the project.")]
    [SerializeField] private bool EnableDebug = true;

    //Sounds.
    [Header("Audio Clips")]
    [Tooltip("Make your own sounds here. If you need to use custom spatial audio curves use SourceSounds.")]
    [SerializeField] private HolySound[] Sounds;
    private Dictionary<string, HolySound> SoundDict;

    [Tooltip("Add your clips as AudioSources first and then add those AudioSources here.")]
    [SerializeField] private HolySourceSound[] SourceSounds;
    private Dictionary<string, HolySourceSound> SourceSoundDict;


    // Reproduction Control. These keep track of states in all methods for Play, Pause, and Stop.
    private bool DidExecuteSound = false;
    private bool DidExecuteSourceSound = false;
    private bool SoundNotFound = false;
    private bool SourceSoundNotFound = false;


    // Memory. Variables that hold temporary information throughout this class.
    private HolySound soundTemp;
	private HolySourceSound sourceSoundTemp;
	private AudioMixer mixerTemp;
	private AudioMixerGroup mixerGroupTemp;

	private GameObject objectTemp;
	private Transform transformTemp;
	private HolyLocalTemp localAudioTemp;


    // All the important setup happens here.
    private void Awake()
    {
        // Initializes Dictionaries
        SoundDict = new Dictionary<string, HolySound>();
        SourceSoundDict = new Dictionary<string, HolySourceSound>();

        // Adds the Sounds to the Sounds Dictionary
        foreach (HolySound sound in Sounds)
        {
            if (SoundDict.ContainsKey(sound.ClipName) && EnableDebug)
            {
                Debug.Log("HolyLocalAudio|Awake|Storing Sounds: Sound " + sound.ClipName + " already exists in SoundsDict, there is a duplicate name!");
                continue;
            }
            SoundDict.Add(sound.ClipName, sound);
        }

        // Adds the Source Sounds to the SourceSounds Dictionary
        foreach (HolySourceSound sourceSound in SourceSounds)
        {
            if (SourceSoundDict.ContainsKey(sourceSound.ClipName) && EnableDebug)
            {
                Debug.Log("HolyLocalAudio|Awake|Storing SourceSounds: SourceSound " + sourceSound.ClipName + " already exists in SourceSoundsDict, there is a duplicate name!");
                continue;
            }
            SourceSoundDict.Add(sourceSound.ClipName, sourceSound);
        }

        // Adds an AudioSource component to the object for each Sound
        foreach (KeyValuePair<string, HolySound> keyValuePair in SoundDict)
        {
            soundTemp = keyValuePair.Value;
            soundTemp.Source = gameObject.AddComponent<AudioSource>();

            soundTemp.Source.outputAudioMixerGroup = soundTemp.MixerGroup;
            soundTemp.Source.clip = soundTemp.AudioFile;

			soundTemp.Source.mute = soundTemp.Mute;
            soundTemp.Source.bypassEffects = soundTemp.BypassEffects;
            soundTemp.Source.bypassListenerEffects = soundTemp.BypassListenerEffects;
            soundTemp.Source.bypassReverbZones = soundTemp.BypassReverbZones;

            soundTemp.Source.playOnAwake = soundTemp.PlayOnAwake;
            soundTemp.Source.loop = soundTemp.Loop;

            soundTemp.Source.priority = soundTemp.Priority;
            soundTemp.Source.volume = soundTemp.Volume;
            soundTemp.Source.pitch = soundTemp.Pitch;
            soundTemp.Source.panStereo = soundTemp.StereoPan;
            soundTemp.Source.spatialBlend = soundTemp.SpatialBlend;
            soundTemp.Source.reverbZoneMix = soundTemp.ReverbZoneMix;
            soundTemp.Source.dopplerLevel = soundTemp.DopplerLevel;
            soundTemp.Source.spread = soundTemp.Spread;

            soundTemp.Source.rolloffMode = soundTemp.VolumeRolloff;
            soundTemp.Source.minDistance = soundTemp.MinDistance;
            soundTemp.Source.maxDistance = soundTemp.MaxDistance;

            if (soundTemp.PlayOnAwake)
            {
                soundTemp.Source.Play();
            }
        }

        if (DisableSpatialBlend)
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


	private void Start()
	{
        GlobalHolyAudioManager = HolyAudioManager.HolyAudioManagerInstance;
		
	}


    // Play
	public void Play(string clipName)
	{
		if(SoundDict.TryGetValue(clipName, out soundTemp))
		{
			soundTemp.Source.PlayOneShot(soundTemp.Source.clip);
			DidExecuteSound = true;
			if (EnableDebug)
			{
				Debug.Log("HolyLocalAudio|Sound|Play: " + clipName + " has played!");
			}
		}
		else
		{
			SoundNotFound = true;
		}
		
		if (SourceSoundDict.TryGetValue(clipName, out sourceSoundTemp))
		{
			sourceSoundTemp.Source.PlayOneShot(sourceSoundTemp.Source.clip);
			DidExecuteSourceSound = true;
			if (EnableDebug)
			{
				Debug.Log("HolyLocalAudio|SourceSound|Play: " + clipName + " has played!");
			}
		}
		else
		{
			SourceSoundNotFound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("HolyLocalAudio|Sounds&SourceSounds|Play: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound)
		{
			Debug.LogWarning("HolyLocalAudio|Sounds&SourceSounds|Play: A clip has been found in both Sounds and SourceSounds");
		}

		DidExecuteSound = false;
		DidExecuteSourceSound = false;
		SoundNotFound = false;
		SourceSoundNotFound = false;
	}

    public void PlayRepeat(string clipName, int iterations)
    {
        if (SoundDict.TryGetValue(clipName, out soundTemp))
        {
            StartCoroutine(ReapeatPlayer(soundTemp.Source, iterations));
            DidExecuteSound = true;
            if (EnableDebug)
            {
                Debug.Log("HolyAudioManager|Sound|PlayRepeat: " + clipName + " has played " + iterations + " times!");
            }
        }
        else
        {
            SoundNotFound = true;
        }

        if (SourceSoundDict.TryGetValue(clipName, out sourceSoundTemp))
        {
            StartCoroutine(ReapeatPlayer(sourceSoundTemp.Source, iterations));
            DidExecuteSourceSound = true;
            if (EnableDebug)
            {
                Debug.Log("HolyAudioManager|SourceSound|PlayRepeat: " + clipName + " has played " + iterations + " times!");
            }
        }
        else
        {
            SourceSoundNotFound = true;
        }

        // Debug
        if (SoundNotFound && SourceSoundNotFound)
        {
            Debug.LogError("HolyAudioManager|Sounds&SourceSounds|PlayRepeat: " + clipName + " does NOT exist!");
        }
        else if (DidExecuteSound && DidExecuteSourceSound)
        {
            Debug.LogWarning("HolyAudioManager|Sounds&SourceSounds|PlayRepeat: A clip has been found in both Sounds and SourceSounds");
        }

        DidExecuteSound = false;
        DidExecuteSourceSound = false;
        SoundNotFound = false;
        SourceSoundNotFound = false;
    }

	public void PlayOnce(string clipName)
	{
		if (SoundDict.TryGetValue(clipName, out soundTemp))
		{
			soundTemp.Source.Play();
			DidExecuteSound = true;
			if (EnableDebug)
			{
				Debug.Log("HolyLocalAudio|Sound|PlayOnce: " + clipName + " has played once!");
			}
		}
		else
		{
			SoundNotFound = true;
		}

		if (SourceSoundDict.TryGetValue(clipName, out sourceSoundTemp))
		{
			sourceSoundTemp.Source.Play();
			DidExecuteSourceSound = true;
			if (EnableDebug)
			{
				Debug.Log("HolyLocalAudio|SourceSound|PlayOnce: " + clipName + " has played once!");
			}
		}
		else
		{
			SourceSoundNotFound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("HolyLocalAudio|Sounds&SourceSounds|PlayOnce: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound)
		{
			Debug.LogWarning("HolyLocalAudio|Sounds&SourceSounds|PlayOnce: A clip has been found in both Sounds and SourceSounds");
		}

		DidExecuteSound = false;
		DidExecuteSourceSound = false;
		SoundNotFound = false;
		SourceSoundNotFound = false;
	}

    public void PlayInPlace(string clipName, int iterations)
    {
		if (SoundDict.TryGetValue(clipName, out soundTemp))
		{
            transformTemp = transform;
            objectTemp = new GameObject();
            objectTemp.transform.SetPositionAndRotation(transform.position, transform.rotation);

            localAudioTemp = objectTemp.AddComponent<HolyLocalTemp>();

			localAudioTemp.Init(soundTemp, iterations);
			DidExecuteSound = true;

            if (EnableDebug)
            {
                Debug.Log("HolyLocalAudio|Sound|PlayInPlace: " + clipName + " has played " + iterations + " times!");
            }
		}
		else
		{
			SoundNotFound = true;
		}

		if (SourceSoundDict.TryGetValue(clipName, out sourceSoundTemp))
		{
			objectTemp = new GameObject();
			objectTemp.transform.SetPositionAndRotation(transform.position, transform.rotation);

			localAudioTemp = objectTemp.AddComponent<HolyLocalTemp>();

			localAudioTemp.Init(sourceSoundTemp, iterations);
			DidExecuteSourceSound = true;

			if (EnableDebug)
			{
				Debug.Log("HolyLocalAudio|Sound|PlayInPlace: " + clipName + " has played " + iterations + " times!");
			}
		}
		else
		{
			SourceSoundNotFound = true;
		}

        if (SoundNotFound && SourceSoundNotFound)
        {
            Debug.LogError("HolyLocalAudio|Sounds&SourceSounds|PlayInPlace: " + clipName + " does NOT exist!");
        }
		else if(DidExecuteSound && DidExecuteSourceSound)
		{
            Debug.LogWarning("HolyLocalAudio|Sounds&SourceSounds|PlayInPlace: A clip has been found in both Sounds and SourceSounds.");
		}
    }


    // Pause
	public void Pause(string clipName)
	{
		if (SoundDict.TryGetValue(clipName, out soundTemp))
		{
			soundTemp.Source.Pause();
			DidExecuteSound = true;
			if (EnableDebug)
			{
				Debug.Log("HolyLocalAudio|Sound|Pause: " + clipName + " has paused!");
			}
		}
		else
		{
			SoundNotFound = true;
		}

		if (SourceSoundDict.TryGetValue(clipName, out sourceSoundTemp))
		{
			sourceSoundTemp.Source.Pause();
			DidExecuteSourceSound = true;
			if (EnableDebug)
			{
				Debug.Log("HolyLocalAudio|SourceSound|Pause: " + clipName + " has paused!");
			}
		}
		else
		{
			SourceSoundNotFound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("HolyLocalAudio|Sounds&SourceSounds|Pause: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound)
		{
			Debug.LogWarning("HolyLocalAudio|Sounds&SourceSounds|Pause: A clip has been found in both Sounds and SourceSounds");
		}

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
				Debug.LogError("HolyLocalAudio|PauseAllButMixerGroup: " + mixerGroupName + " does NOT exist!");
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
			Debug.Log("HolyLocalAudio|PauseAllButMixerGroup: All sounds except from " + mixerGroupName + " have paused!");
		}
	}
	
	public void PauseAllFromMixerGroup(string mixerGroupName)
	{
		if (EnableDebug)
		{
			if (!DoesMixerGroupExist(mixerGroupName))
			{
				Debug.LogError("HolyLocalAudio|PauseAllFromMixerGroup: " + mixerGroupName + " does NOT exist!");
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
			Debug.Log("HolyLocalAudio|PauseAllFromMixerGroup: Sounds in " + mixerGroupName + " have paused!");
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
			Debug.Log("HolyLocalAudio|PauseAll: All sounds paused!");
		}
	}
	

    // Unpause
	public void Unpause(string clipName)
	{
		if (SoundDict.TryGetValue(clipName, out soundTemp))
		{
			soundTemp.Source.UnPause();
			DidExecuteSound = true;
			if (EnableDebug)
			{
				Debug.Log("HolyLocalAudio|Sound|Unpause: " + clipName + " has unpaused!");
			}
		}
		else
		{
			SoundNotFound = true;
		}

		if (SourceSoundDict.TryGetValue(clipName, out sourceSoundTemp))
		{
			sourceSoundTemp.Source.UnPause();
			DidExecuteSourceSound = true;
			if (EnableDebug)
			{
				Debug.Log("HolyLocalAudio|SourceSound|Unpause: " + clipName + " has unpaused!");
			}
		}
		else
		{
			SourceSoundNotFound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("HolyLocalAudio|Sounds&SourceSounds|Unpause: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound)
		{
			Debug.LogWarning("HolyLocalAudio|Sounds&SourceSounds|Unpause: A clip has been found in both Sounds and SourceSounds");
		}

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
				Debug.LogError("HolyLocalAudio|UnpauseAllButMixerGroup: " + mixerGroupName + " does NOT exist!");
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
			Debug.Log("HolyLocalAudio|UnpauseAllButMixerGroup: All sounds except from " + mixerGroupName + " have unpaused!");
		}
	}

	public void UnpauseAllFromMixerGroup(string mixerGroupName)
	{
		if (EnableDebug)
		{
			if (!DoesMixerGroupExist(mixerGroupName))
			{
				Debug.LogError("HolyLocalAudio|UnpauseAllFromMixerGroup: " + mixerGroupName + " does NOT exist!");
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
			Debug.Log("HolyLocalAudio|UnpauseAllFromMixerGroup: Sounds in " + mixerGroupName + " have unpaused!");
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
			Debug.Log("HolyLocalAudio|UnpauseAll: All sounds unpaused!");
		}
	}
	

    // Stop
	public void Stop(string clipName)
	{
		if (SoundDict.TryGetValue(clipName, out soundTemp))
		{
			soundTemp.Source.Stop();
			DidExecuteSound = true;
			if (EnableDebug)
			{
				Debug.Log("HolyLocalAudio|Sound|Stop: " + clipName + " has stopped!");
			}
		}
		else
		{
			SoundNotFound = true;
		}

		if (SourceSoundDict.TryGetValue(clipName, out sourceSoundTemp))
		{
			sourceSoundTemp.Source.Stop();
			DidExecuteSourceSound = true;
			if (EnableDebug)
			{
				Debug.Log("HolyLocalAudio|SourceSound|Stop: " + clipName + " has stopped!");
			}
		}
		else
		{
			SourceSoundNotFound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("HolyLocalAudio|Sounds&SourceSounds|Stop: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound)
		{
			Debug.LogWarning("HolyLocalAudio|Sounds&SourceSounds|Stop: A clip has been found in both Sounds and SourceSounds");
		}

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
				Debug.LogError("HolyLocalAudio|StopAllButMixerGroup: " + mixerGroupName + " does NOT exist!");
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
			Debug.Log("HolyLocalAudio|StopAllButMixerGroup: All sounds except from " + mixerGroupName + " have stopped!");
		}
	}
	
	public void StopAllFromMixerGroup(string mixerGroupName)
	{
		if (EnableDebug)
		{
			if (!DoesMixerGroupExist(mixerGroupName))
			{
				Debug.LogError("HolyLocalAudio|StopAllFromMixerGroup: " + mixerGroupName + " does NOT exist!");
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
			Debug.Log("HolyLocalAudio|StopAllFromMixerGroup: Sounds in " + mixerGroupName + " have stopped!");
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
			Debug.Log("HolyLocalAudio|StopAll: All sounds stopped!");
		}
	}


    // Get MixerGroup(s)
    public AudioMixer GetMixer(string mixerName)
    {
        mixerTemp = HolyAudioManager.HolyAudioManagerInstance.GetMixer(mixerName);
        if (mixerTemp == null)
        {
            Debug.LogError("HolyLocalAudio|GetMixer: " + mixerName + " does NOT exist!");
            return null;
        }
		return mixerTemp;
    }

    public Dictionary<string, AudioMixer> GetAllMixers()
    {
		return HolyAudioManager.HolyAudioManagerInstance.GetAllMixers();
    }

    public AudioMixerGroup GetMixerGroup(string mixerGroupName)
    {
		mixerGroupTemp = HolyAudioManager.HolyAudioManagerInstance.GetMixerGroup(mixerGroupName);
		if (mixerGroupTemp == null){
            Debug.LogError("HolyLocalAudio|GetMixerGroup: " + mixerGroupName + " does NOT exist!");
			return null;
		}
		return mixerGroupTemp;
    }

    public Dictionary<string, AudioMixerGroup> GetAllMixerGroups()
    {
		return HolyAudioManager.HolyAudioManagerInstance.GetAllMixerGroups();
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


	// Helper methods for checking if a Mixer and MixerGroup exists
    public bool DoesMixerExist(string mixerName)
    {
		return HolyAudioManager.HolyAudioManagerInstance.DoesMixerExist(mixerName);
    }

    public bool DoesMixerGroupExist(string mixerGroupName)
    {
		return HolyAudioManager.HolyAudioManagerInstance.DoesMixerGroupExist(mixerGroupName);
    }


	// Helper methods for checking if a Sound or SourceSound exists
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
                Debug.LogWarning("HolyLocalAudio|DoesSoundExist: Sound " + soundName + " was NOT found!");
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
                Debug.LogWarning("HolyLocalAudio|DoesSourceSoundExist: SourceSound " + sourceSoundName + " was NOT found!");
            }
            return false;
        }
    }


	// Timer
    public IEnumerator ReapeatPlayer(AudioSource source, int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            source.Play();
            yield return new WaitForSeconds(source.clip.length);
        }
    }
}