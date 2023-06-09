using System;
using UnityEngine;
using UnityEngine.Audio;

/*
	! Version: 0.1.0
	* 🍑HolyAudio2D
	* Thank you for using this little project of mine, I hope it is helpful in your endeavors! -holypeach
	? If you have any questions, suggestions, or find bugs here is the repo https://github.com/holypeachy/HolyAudio2D
*/

public class HolyLocalAudio : MonoBehaviour
{
	// References to other scripts. The Global HolyAudioManager Required for this script to function properly.
	[SerializeField] public HolyAudioManager GlobalManager;


	// Control Flow. Some options for debugging.
	[Header("Debugging")]
	
	[Tooltip("Enabling might help with making sure sounds are being played. Only applied to this Object's Sounds.")]
	[SerializeField] private bool DisableSpatialBlend = false;
	
	[Tooltip("Applies to optional messages. Important Warnings and Errors will still be displayed but I recommend you keep it on during development or you might miss some things. Make sure to turn it off when you build the project.")]
	[SerializeField] private bool EnableDebug = true;
	

	// Sounds and Mixers
	private AudioMixer[] Mixers;
	private AudioMixerGroup[] MixerGroups;
	
	[Header("Audio Clips")]
	
	[Tooltip("Make your own sounds here. If you need to use custom spatial audio curves use SourceSounds.")]
	[SerializeField] private HolySound[] Sounds;
	
	[Tooltip("Add your clips as AudioSources first and then add those AudioSources here.")]
	[SerializeField] private HolySourceSound[] SourceSounds;
	
	
	// Play Control. These keep track of states in all methods for Play, Pause, and Stop.
	private bool DidExecuteSound = false;
	private bool DidExecuteSourceSound = false;
	private bool SoundNotFound = false;
	private bool SourceSoundNotFound = false;


	// All the important setup happens here.
	private void Awake()
	{
		Mixers = GlobalManager.GetAllMixers();
		MixerGroups = GlobalManager.GetAllMixerGroups();
		
		
		// We set up each sound
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

		if (DisableSpatialBlend)
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
				Debug.LogWarning("HolyLocalAudio|Sound|Play: " + clipName + " has played!");
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
				Debug.LogWarning("HolyLocalAudio|SourceSound|Play: " + clipName + " has played!");
			}
			DidExecuteSourceSound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("HolyLocalAudio|Sounds&SourceSounds|Play: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound)
		{
			Debug.LogWarning("HolyLocalAudio|Play: A clip has been found in Sounds and SourceSounds");
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
				Debug.LogWarning("HolyLocalAudio|Sound|PlayOnce: " + clipName + " has played!");
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
				Debug.LogWarning("HolyLocalAudio|PlayOnce: " + clipName + " has played!");
			}
			DidExecuteSourceSound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("HolyLocalAudio|Sounds & SourceSounds|Play: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound)
		{
			Debug.LogError("HolyLocalAudio|Play: A clip has been found in Sounds and SourceSounds");
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
				Debug.LogWarning("HolyAudioManager|Sound|Pause: " + clipName + " has paused!");
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
				Debug.LogWarning("HolyAudioManager|Pause: " + clipName + " has paused!");
			}
			DidExecuteSourceSound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("HolyAudioManager|Sounds&SourceSounds|Pause: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound)
		{
			Debug.LogWarning("HolyAudioManager|Pause: A clip has been found in Sounds and SourceSounds");
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
				Debug.LogError("HolyAudioManager|PauseAllButMixerGroup: " + mixerGroupName + " does NOT exist!");
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
			Debug.LogWarning("HolyAudioManager|PauseAllFromMixerGroup: Sounds in " + mixerGroupName + " have paused!");
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
			Debug.LogWarning("HolyAudioManager|PauseAll: All sounds paused!");
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
				Debug.LogWarning("HolyAudioManager|Sound|Pause: " + clipName + " has unpaused!");
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
				Debug.LogWarning("HolyAudioManager|Sound|Unpause: " + clipName + " has unpaused!");
			}
			DidExecuteSourceSound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("HolyAudioManager|Sounds&SourceSounds|Unpause: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound && EnableDebug)
		{
			Debug.LogWarning("HolyAudioManager|Unpause: A clip has been found in Sounds and SourceSounds");
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
				Debug.LogError("HolyAudioManager|UnpauseAllButMixerGroup: " + mixerGroupName + " does NOT exist!");
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
			Debug.LogWarning("HolyAudioManager|UnpauseAllButMixerGroup: All sounds except from " + mixerGroupName + " have unpaused!");
		}
	}

	public void UnpauseAllFromMixerGroup(string mixerGroupName)
	{
		if (EnableDebug)
		{
			if (!DoesMixerExist(mixerGroupName))
			{
				Debug.LogError("HolyAudioManager|UnpauseAllButMixerGroup: " + mixerGroupName + " does NOT exist!");
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
			Debug.LogWarning("HolyAudioManager|UnpauseFromMixerGroup: Sounds in " + mixerGroupName + " have unpaused!");
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
			Debug.LogWarning("HolyAudioManager|UnpauseAll: All sounds unpaused!");
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
				Debug.LogWarning("HolyAudioManager|Sound|Stop: " + clipName + " has stopped!");
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
				Debug.LogWarning("HolyAudioManager|Sound|Stop: " + clipName + " has stopped!");
			}
			DidExecuteSourceSound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("HolyAudioManager|Sounds&SourceSounds|Stop: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound)
		{
			Debug.LogWarning("HolyAudioManager|Stop: A clip has been found in Sounds and SourceSounds");
		}

		// We reset all the vars
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
			Debug.LogWarning("HolyAudioManager|StopAllFromMixerGroup: Sounds in " + mixerGroupName + " have stopped!");
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

		Debug.LogError("HolyLocalAudio|GetMixer: " + mixerName + " does NOT exist!");
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
			if (group.name == mixerGroupName)
			{
				return group;
			}
		}

		Debug.LogError("HolyLocalAudio|GetMixerGroup: " + mixerGroupName + " does NOT exist!");
		return null;
	}

	public AudioMixerGroup[] GetAllMixerGroups()
	{
		return MixerGroups;
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
		if (mixer == null)
		{
			if (EnableDebug)
			{
				Debug.LogWarning("HolyLocalAudio|DoesMixerExist: Mixer " + mixerName + " was not found!");
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
				Debug.LogWarning("HolyLocalAudio|DoesMixerGroupExist: MixerGroup " + mixerGroupName + " was not found!");
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
				Debug.LogWarning("HolyLocalAudio|DoesSoundExist: Sound " + soundName + " was not found!");
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
				Debug.LogWarning("HolyLocalAudio|DoesSourceSoundExist: SourceSound " + sourceSoundName + " was not found!");
			}
			return false;
		}
		else
		{
			return true;
		}
	}

}