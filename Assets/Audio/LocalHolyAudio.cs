using System;
using UnityEngine;
using UnityEngine.Audio;

/*
	! Version: 0.1.0
	* 🍑HolyAudio2D
	* Thank you for using this little project of mine, I hope it is helpful in your endeavors! -holypeach
	? If you have any questions, suggestions, or find bugs here is the repo https://github.com/holypeachy/HolyAudio2D
*/

public class LocalHolyAudio : MonoBehaviour
{
	// References to other scripts. The Global HolyAudioM Required for this script to function properly.
	[SerializeField] public HolyAudioManager GlobalManager;


	// Control Flow. Some options for debugging.
	[Header("Debugging")]
	
	[Tooltip("Enabling might help with making sure sounds are being played. Only applied to the Object's Sounds.")]
	[SerializeField] private bool DisableSpatialBlend = false;
	
	[Tooltip("Only applies to optional messages. Important Warnings and Errors will still be displayed.")]
	[SerializeField] private bool EnableDebug = true;
	
	[Tooltip("Keep this on during development. Turn this off before building the game. See documentation for more information.")]
	[SerializeField] private bool EnableImportantDebug = true;

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
				Debug.LogWarning("LocalHolyAudio|Sound|Play: " + clipName + " has played!");
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
				Debug.LogWarning("LocalHolyAudio|SourceSound|Play: " + clipName + " has played!");
			}
			DidExecuteSourceSound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("LocalHolyAudio|Sounds&SourceSounds|Play: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound)
		{
			Debug.LogWarning("LocalHolyAudio|Play: A clip has been found in Sounds and SourceSounds");
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
				Debug.LogWarning("LocalHolyAudio|Sound|PlayOnce: " + clipName + " has played!");
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
				Debug.LogWarning("LocalHolyAudio|PlayOnce: " + clipName + " has played!");
			}
			DidExecuteSourceSound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("LocalHolyAudio|Sounds & SourceSounds|Play: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound)
		{
			Debug.LogError("LocalHolyAudio|Play: A clip has been found in Sounds and SourceSounds");
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
				Debug.LogWarning("LocalHolyAudio|Sound|Pause: " + clipName + " has paused!");
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
				Debug.LogWarning("LocalHolyAudio|Pause: " + clipName + " has paused!");
			}
			DidExecuteSourceSound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("LocalHolyAudio|Sounds&SourceSounds|Pause: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound)
		{
			Debug.LogWarning("LocalHolyAudio|Pause: A clip has been found in Sounds and SourceSounds");
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
				Debug.LogError("LocalHolyAudio|PauseAllButMixerGroup: " + mixerGroupName + " does NOT exist!");
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
			Debug.LogWarning("LocalHolyAudio|PauseAllButMixerGroup: All sounds except from " + mixerGroupName + " have paused!");
		}
	}

	public void PauseAllFromMixerGroup(string mixerGroupName)
	{
		if (EnableDebug)
		{
			if (!DoesMixerExist(mixerGroupName))
			{
				Debug.LogError("LocalHolyAudio|PauseAllFromMixerGroup: " + mixerGroupName + " does NOT exist!");
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
			Debug.LogWarning("LocalHolyAudio|PauseAllFromMixerGroup: Sounds in " + mixerGroupName + " have paused!");
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
			Debug.LogWarning("LocalHolyAudio|PauseAll: All sounds paused!");
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
				Debug.LogWarning("LocalHolyAudio|Sound|Pause: " + clipName + " has unpaused!");
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
				Debug.LogWarning("LocalHolyAudio|Sound|Unpause: " + clipName + " has unpaused!");
			}
			DidExecuteSourceSound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("LocalHolyAudio|Sounds&SourceSounds|Unpause: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound && EnableDebug)
		{
			Debug.LogWarning("LocalHolyAudio|Unpause: A clip has been found in Sounds and SourceSounds");
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
				Debug.LogError("LocalHolyAudio|UnpauseAllButMixerGroup: " + mixerGroupName + " does NOT exist!");
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
			Debug.LogWarning("LocalHolyAudio|UnpauseAllButMixerGroup: All sounds except from " + mixerGroupName + " have unpaused!");
		}
	}

	public void UnpauseAllFromMixerGroup(string mixerGroupName)
	{
		if (EnableImportantDebug)
		{
			if (!DoesMixerExist(mixerGroupName))
			{
				Debug.LogError("LocalHolyAudio|UnpauseAllButMixerGroup: " + mixerGroupName + " does NOT exist!");
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
			Debug.LogWarning("LocalHolyAudio|UnpauseFromMixerGroup: Sounds in " + mixerGroupName + " have unpaused!");
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
			Debug.LogWarning("LocalHolyAudio|UnpauseAll: All sounds unpaused!");
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
				Debug.LogWarning("LocalHolyAudio|Sound|Stop: " + clipName + " has stopped!");
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
				Debug.LogWarning("LocalHolyAudio|Sound|Stop: " + clipName + " has stopped!");
			}
			DidExecuteSourceSound = true;
		}

		// Debug
		if (SoundNotFound && SourceSoundNotFound)
		{
			Debug.LogError("LocalHolyAudio|Sounds&SourceSounds|Stop: " + clipName + " does NOT exist!");
		}
		else if (DidExecuteSound && DidExecuteSourceSound)
		{
			Debug.LogWarning("LocalHolyAudio|Stop: A clip has been found in Sounds and SourceSounds");
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
			if (s.Source.outputAudioMixerGroup.name == mixerGroupName)
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
			Debug.LogWarning("LocalHolyAudio|StopAllButMixerGroup: All sounds except from " + mixerGroupName + " have stopped!");
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
			Debug.LogWarning("LocalHolyAudio|StopAllFromMixerGroup: Sounds in " + mixerGroupName + " have stopped!");
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

		if (EnableDebug)
		{
			Debug.LogWarning("LocalHolyAudio|StopAll: All sounds stopped!");
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

		Debug.LogError("LocalHolyAudio|GetMixer: " + mixerName + " does NOT exist!");
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

		Debug.LogError("LocalHolyAudio|GetMixerGroup: " + mixerGroupName + " does NOT exist!");
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
				Debug.LogWarning("LocalHolyAudio|DoesMixerExist: Mixer " + mixerName + " was not found!");
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
				Debug.LogWarning("LocalHolyAudio|DoesMixerGroupExist: MixerGroup " + mixerGroupName + " was not found!");
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
				Debug.LogWarning("LocalHolyAudio|DoesSoundExist: Sound " + soundName + " was not found!");
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
				Debug.LogWarning("LocalHolyAudio|DoesSourceSoundExist: SourceSound " + sourceSoundName + " was not found!");
			}
			return false;
		}
		else
		{
			return true;
		}
	}

}