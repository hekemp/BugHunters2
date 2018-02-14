using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Audio Events/Simple")]
public class SimpleAudioEvent : AudioEvent
{
    [SerializeField] private AudioEventId audioEventId;

    [SerializeField] private AudioClip[] clips = new AudioClip[0];
    [SerializeField] private AudioMixerGroup mixer = null;
    [SerializeField] private RangedFloat volume = new RangedFloat(1f, 1f);
    [MinMaxRange(0f, 2f)] [SerializeField] private RangedFloat pitch = new RangedFloat(1f, 1f);
    [MinMaxRange(0f, 1000f)] [SerializeField] private float minDistance = 1f, maxDistance = 500f;

    public AudioEventId AudioEventId { get { return audioEventId; } }

    public override void Play(AudioSource source)
    {
        if (clips.Length == 0) return;

        source.clip = clips[Random.Range(0, clips.Length)];
        source.outputAudioMixerGroup = mixer;
        source.volume = Random.Range(volume.minValue, volume.maxValue);
        source.pitch = Random.Range(pitch.minValue, pitch.maxValue);
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;
        source.Play();

        source.playOnAwake = false;
    }
}

public enum AudioEventId
{
    Countdown,
    GetBackToYourBases,
    Headshot,
    NiceShotSoldier,
    Oww,
    TeammateDown,
    TimesUp,
    WeGotOne,
    YepYouDied,
    YouGotEm,
    PaintballGunShot,
}