using System.Collections;
using UnityEngine;

/// <summary>
/// This component will make the controller vibrate on weapon fire.
/// </summary>
[RequireComponent(typeof(Weapon))]
public class WeaponHaptics : MonoBehaviour
{
    [SerializeField]
    [Range(500, 3999)]
    [Tooltip("The strength of the vibration when the weapon fires")]
    private ushort hapticIntensity = 1000;

    [SerializeField]
    [Range(0.1f, 1f)]
    [Tooltip("The length of time to vibrate when the weapon fires")]
    private float hapticDuration = 0.1f;

    private Weapon weapon;

    private void Start()
    {
        weapon = GetComponent<Weapon>();
        weapon.OnWeaponFired += HandleWeaponFired;
    }

    private void HandleWeaponFired()
    {
        StartCoroutine(PlayHapticFeedbackAsync());
    }

    private IEnumerator PlayHapticFeedbackAsync()
    {
        var timeMultiplier = 1f / hapticDuration;
        var device = SteamVR_Controller.Input((int)weapon.ControllerIndex);
        float count = 0;

        while (count < 1)
        {
            device.TriggerHapticPulse(hapticIntensity);
            count += Time.deltaTime * timeMultiplier;
            yield return null;
        }
    }
}