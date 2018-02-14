using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(SteamVR_LaserPointer))]
public class VRUIInput : MonoBehaviour
{
    private SteamVR_LaserPointer laserPointer;
    private Hand hand;

    private void OnEnable()
    {
        laserPointer = GetComponent<SteamVR_LaserPointer>();
        laserPointer.PointerIn -= HandlePointerIn;
        laserPointer.PointerIn += HandlePointerIn;
        laserPointer.PointerOut -= HandlePointerOut;
        laserPointer.PointerOut += HandlePointerOut;

        hand = GetComponent<Hand>();
        if (hand == null)
        {
            hand = GetComponentInParent<Hand>();
        }
    }

    private void Update()
    {
        if (hand != null)
        {
            if (hand.GetStandardInteractionButtonDown())
            {
                HandleTriggerClicked(null, new ClickedEventArgs());
            }
        }
    }

    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }
    }

    private void HandlePointerIn(object sender, PointerEventArgs e)
    {
        var selectable = e.target.GetComponent<Selectable>();
        if (selectable != null)
        {
            selectable.Select();
            Debug.Log("HandlePointerIn", e.target.gameObject);
        }
    }

    private void HandlePointerOut(object sender, PointerEventArgs e)
    {
        var selectable = e.target.GetComponent<Selectable>();
        if (selectable != null)
        {
			// Don't deselect input fields
			if (EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>() is InputField == false)
			{
				EventSystem.current.SetSelectedGameObject(null);
			}
            Debug.Log("HandlePointerOut", e.target.gameObject);
        }
    }
}