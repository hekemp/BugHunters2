using System;
using UnityEngine;

/// <summary>
/// Handles input for the weapon.
/// If you want to change the interaction button from the trigger to something else, you can do that here.
/// Switch from LocalHand.GetStandardInteractionButton to 
/// </summary>
public class WeaponInput : MonoBehaviour
{
    public event Action OnTriggerPulled = delegate { };

    private PlayerHand localhand;
    
    private void Update()
    {
        if (localhand == null)
            localhand = GetComponentInParent<PlayerHand>();

        if (localhand == null)
            return;

        // If you want to change the controller button, you can use this inplace of the GetStandardInteractionButtonDown call
        // localhand.LocalHand.controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip); // or some other buttonId

        if (localhand.LocalHand != null &&
            localhand.LocalHand.GetStandardInteractionButtonDown())
        {
            OnTriggerPulled();
        }
    }
}