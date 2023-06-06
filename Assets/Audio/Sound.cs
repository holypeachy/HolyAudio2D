using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
	[HideInInspector] public AudioSource source;
	[SerializeField] public AudioMixerGroup mixerGroup;

	public string clipName;
	public AudioClip audioClip;
	public bool isLoop;
	public bool playOnAwake;

	[Range(0f, 1f)]
	public float volume = 0.5f;
	[Range(0.1f, 3f)]
	public float pitch = 1f;
}
