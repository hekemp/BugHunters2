using System.Linq;
using UnityEngine;

public class SimpleAudioController : MonoBehaviour
{
    public static SimpleAudioController Instance { get; private set; }

    private AudioSource[] audioSources;

    [SerializeField]
    private SimpleAudioEvent[] voEvents;

    private void Awake()
    {
        Instance = this;
        audioSources = GetComponentsInChildren<AudioSource>();
    }

    public void PlayAtPosition(AudioEventId audioEventId, Vector3 position)
    {
        var eventToPlay = voEvents.FirstOrDefault(t => t.AudioEventId == audioEventId);
        if (eventToPlay != null)
        {
            var audioSource = audioSources.FirstOrDefault(t => t.isPlaying == false);
            if (audioSource != null)
            {
                audioSource.transform.position = position;
                eventToPlay.Play(audioSource);
            }
            else
            {
                Debug.LogWarning("No available audio source to play on.  Try adding more.");
            }
        }
    }

    //private void OnEnable()
    //{
    //    foreach(var e in System.Enum.GetValues(typeof(AudioEventId)))
    //    {
    //        PlayAtPosition((AudioEventId)e, transform.position);
    //    }
    //}
}