using System.Collections;
using UnityEngine;

public class HolyLocalTemp : MonoBehaviour
{
    private HolySound _sound;
    private HolySourceSound _SourceSound;
    private int _iterations;

    public void Init(HolySound sound, int iterations)
    {
        _sound = new HolySound(sound);
        _sound.Source = gameObject.AddComponent<AudioSource>();

        _sound.Source.outputAudioMixerGroup = _sound.MixerGroup;
        _sound.Source.clip = _sound.AudioFile;

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

        _iterations = iterations;

        StartCoroutine(DestroyTimer(_sound.Source));
    }

    public void Init(HolySourceSound sourceSound, int iterations)
    {
        _SourceSound = new HolySourceSound(sourceSound);
        this._iterations = iterations;

        StartCoroutine(DestroyTimer(_SourceSound.Source));
    }

    public IEnumerator DestroyTimer(AudioSource source)
    {
        for (int i = 0; i < _iterations; i++)
        {
            source.Play();
            yield return new WaitForSeconds(source.clip.length);
        }
        Destroy(gameObject);
    }

}
