using System.Collections;
using UnityEngine;

public class HolyLocalTemp : MonoBehaviour
{
    private HolySound _sound;
    private HolySourceSound _SourceSound;

    private AudioSource _audioSource;

    public void Init(HolySound sound, int iterations)
    {
        _sound = new HolySound(sound);
        _sound.Source = gameObject.AddComponent<AudioSource>();

        _sound.Source.outputAudioMixerGroup = _sound.MixerGroup;
        _sound.Source.clip = _sound.AudioFile;

        _sound.Source.mute = _sound.Mute;
        _sound.Source.bypassEffects = _sound.BypassEffects;
        _sound.Source.bypassListenerEffects = _sound.BypassListenerEffects;
        _sound.Source.bypassReverbZones = _sound.BypassReverbZones;

        _sound.Source.playOnAwake = _sound.PlayOnAwake;
        _sound.Source.loop = _sound.Loop;

        _sound.Source.priority = _sound.Priority;
        _sound.Source.volume = _sound.Volume;
        _sound.Source.pitch = _sound.Pitch;
        _sound.Source.panStereo = _sound.StereoPan;
        _sound.Source.spatialBlend = _sound.SpatialBlend;
        _sound.Source.reverbZoneMix = _sound.ReverbZoneMix;

        _sound.Source.dopplerLevel = _sound.DopplerLevel;
        _sound.Source.spread = _sound.Spread;
        _sound.Source.rolloffMode = _sound.VolumeRolloff;
        _sound.Source.minDistance = _sound.MinDistance;
        _sound.Source.maxDistance = _sound.MaxDistance;

        StartCoroutine(DestroyTimer(_sound.Source, iterations));
    }

    public void Init(HolySourceSound sourceSound, int iterations)
    {
        _SourceSound = new HolySourceSound(sourceSound);
        _audioSource = gameObject.AddComponent<AudioSource>();

        _audioSource.clip = _SourceSound.Source.clip;
        _audioSource.outputAudioMixerGroup = _SourceSound.Source.outputAudioMixerGroup;

        _audioSource.mute = _SourceSound.Source.mute;
        _audioSource.bypassEffects = _SourceSound.Source.bypassEffects;
        _audioSource.bypassListenerEffects = _SourceSound.Source.bypassListenerEffects;
        _audioSource.bypassReverbZones = _SourceSound.Source.bypassReverbZones;
        
        _audioSource.playOnAwake = _SourceSound.Source.playOnAwake;
        _audioSource.loop = _SourceSound.Source.loop;

        _audioSource.priority = _SourceSound.Source.priority;
        _audioSource.volume = _SourceSound.Source.volume;
        _audioSource.pitch = _SourceSound.Source.pitch;
        _audioSource.panStereo = _SourceSound.Source.panStereo;
        _audioSource.spatialBlend = _SourceSound.Source.spatialBlend;
        _audioSource.reverbZoneMix = _SourceSound.Source.reverbZoneMix;

        _audioSource.dopplerLevel = _SourceSound.Source.dopplerLevel;
        _audioSource.spread = _SourceSound.Source.spread;
        _audioSource.rolloffMode = _SourceSound.Source.rolloffMode;
        _audioSource.minDistance = _SourceSound.Source.minDistance;
        _audioSource.maxDistance = _SourceSound.Source.maxDistance;

        StartCoroutine(DestroyTimer(_audioSource, iterations));
    }

    public IEnumerator DestroyTimer(AudioSource source, int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            source.Play();
            yield return new WaitForSeconds(source.clip.length);
        }
        Destroy(gameObject);
    }

}
