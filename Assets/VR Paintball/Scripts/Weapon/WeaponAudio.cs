using UnityEngine;

/// <summary>
/// Plays audio for weapon shots at the guns position.
/// Configure the gunshot sounds in AudioEvents assigned to the SimpleAudioController prefab.
/// </summary>
[RequireComponent(typeof(Weapon))]
public class WeaponAudio : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Weapon>().OnWeaponFired += () =>
            SimpleAudioController.Instance.PlayAtPosition(AudioEventId.PaintballGunShot, transform.position);
    }
}