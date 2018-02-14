using System;
using UnityEngine;

using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class WeaponInteractable : MonoBehaviour
{
    [EnumFlags]
    [Tooltip("The flags used to attach this object to the hand.")]
    public Valve.VR.InteractionSystem.Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.DetachFromOtherHand;

    [Tooltip("Name of the attachment transform under in the hand's hierarchy which the object should should snap to.")]
    public string attachmentPoint;

    [SerializeField]
    [Tooltip("How far the trigger must be pulled to fire.")]
    private float triggerClickThreshold = 0.8f;

    private bool triggerWasReset;
    private float lastTrigger;

    public Hand AttachedHand { get; private set; }

    public event Action OnTriggerPulled = delegate { };
    public event Action OnGripClicked = delegate { };
    public event Action<float> OnTriggerChanged = delegate { };

    private void HandHoverUpdate(Hand hand)
    {
        //Trigger got pressed
        if (hand.GetStandardInteractionButtonDown())
        {
            hand.AttachObject(gameObject, attachmentFlags, attachmentPoint);
            ControllerButtonHints.HideButtonHint(hand, Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
        }
    }

    private void HandAttachedUpdate(Hand hand)
    {
        var trigger = hand.controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;
        hand.GetComponentInChildren<Animator>().SetFloat("Trigger", trigger);

        Debug.Log("Trigger for weapon: " + trigger);

        if (triggerWasReset && trigger >= triggerClickThreshold)
        {
            triggerWasReset = false;
            OnTriggerPulled();
        }
        else if (trigger <= 0.1f)
        {
            triggerWasReset = true;
        }

        if (trigger != lastTrigger)
        {
            lastTrigger = trigger;
            OnTriggerChanged(trigger);
        }

        if (hand.controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip))
        {
            hand.GetComponentInChildren<Animator>().SetTrigger("Reload");
            OnGripClicked();
        }
    }

    private void OnAttachedToHand(Hand hand)
    {
        AttachedHand = hand;
        hand.GetComponentInChildren<Animator>().SetBool("HoldingWeapon", true);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    private void OnDetachedFromHand(Hand hand)
    {
        AttachedHand = null;
        hand.GetComponentInChildren<Animator>().SetBool("HoldingWeapon", false);
    }
}