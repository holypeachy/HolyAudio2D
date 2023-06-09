using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class HolyMixerInfo
{
	[SerializeField] public AudioMixer Mixer;
	[SerializeField] public AudioMixerUpdateMode UpdateMode;
}
