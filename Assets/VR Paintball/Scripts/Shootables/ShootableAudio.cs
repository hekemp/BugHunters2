using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Shootable))]
public class ShootableAudio : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Sound played when target takes a shot")]
	private AudioClip impactClip;

	private AudioSource audioSource;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		GetComponent<Shootable>().OnTookShot += HandleTookShot;
	}

	private void HandleTookShot(RaycastHit hit)
	{
		audioSource.PlayOneShot(impactClip);
	}
}