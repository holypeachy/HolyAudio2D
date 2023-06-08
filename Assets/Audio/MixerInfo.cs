using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class MixerInfo
{
	[SerializeField] public AudioMixer Mixer;
	[SerializeField] public AudioMixerUpdateMode UpdateMode;
}
