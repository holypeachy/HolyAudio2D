using UnityEngine;

[System.Serializable]
public class HolySourceSound
{
	public string ClipName;
	public AudioSource Source;

	public HolySourceSound(HolySourceSound sourceSound)
	{
		ClipName = sourceSound.ClipName;
		Source = sourceSound.Source;
	}
}
