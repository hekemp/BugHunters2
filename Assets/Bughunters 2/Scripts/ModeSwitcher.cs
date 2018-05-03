using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
    [RequireComponent(typeof(Interactable))]
    [RequireComponent(typeof(LineRenderer))]
    public class ModeSwitcher : MonoBehaviour
    {
        public float value = 0f;

        public delegate void OnGrabbedByHandDelegate(Hand hand);
        public delegate void OnReleasedByHandDelegate(Hand hand);

        public event OnGrabbedByHandDelegate onGrabbedByHand;
        public event OnReleasedByHandDelegate onReleasedByHand;

        [Tooltip("Child GameObject which has the Collider component to initiate interaction, only needs to be set if there is more than one Collider child")]
        public Collider childCollider = null;

        [Tooltip("Child GameObject to be moved by the user")]
        public GameObject Handle = null;

        [Tooltip("A LinearMapping component to drive, if not specified one will be dynamically added to this GameObject")]
        public LinearMapping linearMapping;

        [Tooltip("If true, the drive will stay manipulating as long as the button is held down, if false, it will stop if the controller moves out of the collider")]
        public bool hoverLock = false;

        [Tooltip("The maximum range of motion")]
        public float maxDistance = 1f;

        [Tooltip("The maximum distance required to reset the mode switcher")]
        public float resetDistance = 0.3f;

        [Tooltip("The minimum distance required to activate the mode switcher")]
        public float activeDistance = 0.8f;

        [Tooltip("The button used to interact with the object")]
        public EVRButtonId interactButton = EVRButtonId.k_EButton_Grip;

        [Tooltip("The line renderer connecting the handle to the origin")]
        public LineRenderer lr;

        private Hand handHoverLocked = null;

        private bool driving = false;

        private Vector3 handPosition = Vector3.zero;

        private bool isActive = false;

        // Use this for initialization
        void Start()
        {
            if (childCollider == null)
            {
                childCollider = GetComponentInChildren<Collider>();
            }

            if (linearMapping == null)
            {
                linearMapping = GetComponent<LinearMapping>();
            }

            if (linearMapping == null)
            {
                linearMapping = gameObject.AddComponent<LinearMapping>();
            }

            if (lr == null)
            {
                lr = GetComponent<LineRenderer>();
            }
        }

        void Activate()
        {
            Debug.Log("@ SWAPPING MODES...");
            ControlModuleManager.Instance.Swap();
        }

        private void Update()
        {
            // Go back to neutral if not driving
            if (!driving)
            {
                Handle.transform.position = Vector3.Lerp(Handle.transform.position, transform.position, 0.1f);
                value = Vector3.Distance(Handle.transform.position, transform.position);
            }

            if (value > activeDistance && !isActive)
            {
                isActive = true;
                Activate();
            }

            if (value < resetDistance && isActive)
            {
                isActive = false;
            }

            lr.SetPositions(new Vector3[] { transform.position, Handle.transform.position });
        }

        private IEnumerator HapticPulses(SteamVR_Controller.Device controller, float flMagnitude, int nCount)
        {
            if (controller != null)
            {
                int nRangeMax = (int)Util.RemapNumberClamped(flMagnitude, 0.0f, 1.0f, 100.0f, 900.0f);
                nCount = Mathf.Clamp(nCount, 1, 10);

                for (ushort i = 0; i < nCount; ++i)
                {
                    ushort duration = (ushort)Random.Range(100, nRangeMax);
                    controller.TriggerHapticPulse(duration);
                    yield return new WaitForSeconds(.01f);
                }
            }
        }

        private void OnHandHoverBegin(Hand hand)
        {
            ControllerButtonHints.ShowButtonHint(hand, interactButton);
        }

        private void OnHandHoverEnd(Hand hand)
        {
            ControllerButtonHints.HideButtonHint(hand, interactButton);

            if (driving && hand.GetStandardInteractionButton())
            {
                StartCoroutine(HapticPulses(hand.controller, 1.0f, 10));
            }

            driving = false;
            handHoverLocked = null;

            if (onReleasedByHand != null)
            {
                onReleasedByHand.Invoke(hand);
            }
        }

        private void HandHoverUpdate(Hand hand)
        {
            if (hand.controller.GetPressDown(interactButton))
            {
                if (hoverLock)
                {
                    hand.HoverLock(GetComponent<Interactable>());
                    handHoverLocked = hand;
                }

                driving = true;

                GetHandPosition(hand);
                UpdateHandle();

                ControllerButtonHints.HideButtonHint(hand, interactButton);

                if (onGrabbedByHand != null)
                {
                    onGrabbedByHand.Invoke(hand);
                }
            }
            else if (hand.controller.GetPressDown(interactButton))
            {
                // Trigger was just released
                if (hoverLock)
                {
                    hand.HoverUnlock(GetComponent<Interactable>());
                    handHoverLocked = null;
                }

                if (onReleasedByHand != null)
                {
                    onReleasedByHand.Invoke(hand);
                }
            }
            else if (driving && hand.controller.GetPress(interactButton) && hand.hoveringInteractable == GetComponent<Interactable>())
            {
                GetHandPosition(hand);
                UpdateHandle();
            }
        }

        void GetHandPosition(Hand h)
        {
            handPosition = h.transform.position;
        }

        void UpdateHandle()
        {
            Vector3 offset = handPosition - transform.position;
            if (offset.magnitude > maxDistance)
            {
                offset = offset.normalized * maxDistance;
            }
            Handle.transform.position = transform.position + offset;
            linearMapping.value = offset.magnitude;
            value = linearMapping.value;
        }
    }
}
