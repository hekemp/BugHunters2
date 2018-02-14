using UnityEngine;

/// <summary>
/// If using a weapon that has animation, you can add this script to trigger animations for fire and reload.
/// The included model does not have animations, but if you bring your own, you can use this.
/// Make sure to include an animator component and triggers in there that match names here.
/// </summary>
public class WeaponAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    private void Start()
    {
        GetComponent<Weapon>().OnWeaponFired += () => animator.SetTrigger("Fire");

        GetComponent<WeaponAmmo>().OnReloadStarted += () => animator.SetTrigger("Reload");
    }
}