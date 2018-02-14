using UnityEngine;

/// <summary>
/// This class can be used to animate a trigger on your weapon.
/// The included model does not have an animatable trigger, but many guns do so this is included to help there.
/// To use it, add a new layer to your Animator that controlls just the trigger, with an animation named to match
/// the animation referenced below "Trigger Pull".  Then add this component to the weapon and it will set that animation
/// to a point that lines up with how far the trigger is pulled.
/// </summary>
public class WeaponTrigger : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    private void Start()
    {
        GetComponent<WeaponInteractable>().OnTriggerChanged += WeaponTrigger_OnTriggerChanged;
    }

    private void WeaponTrigger_OnTriggerChanged(float triggerPullPct)
    {
        animator.Play("Trigger Pull", 1, triggerPullPct);
    }
}