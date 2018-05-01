using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunModule : ControlModule
{

    public float angleL = 0;
    public float angleR = 0;

    public Valve.VR.InteractionSystem.CircularDrive leftJoystick;
    public Valve.VR.InteractionSystem.CircularDrive rightJoystick;

    public Transform userConsole;
    public Transform ship;

    [Tooltip("The button used to interact with the object")]
    public Valve.VR.EVRButtonId interactButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

    public float GunRange = 10f;
    public float GunOffset = 0.5f;
    public GameObject laser;

    public float Cooldown = 0.1f;

    private WaitForSeconds cooldownDelay;
    private bool canShootL = true;
    private bool canShootR = true;

    float spinModifier = 0.9f;


    public Valve.VR.InteractionSystem.Hand hand;
    public Valve.VR.InteractionSystem.Hand hand2;

    public Valve.VR.InteractionSystem.Interactable leftGun;
    public Valve.VR.InteractionSystem.Interactable rightGun;

    // Use this for initialization
    void Start () {
        cooldownDelay = new WaitForSeconds(Cooldown);
    }

    public enum LeftRight
    {
        Left,
        Right
    }

    private void _Shoot(LeftRight lr)
    {
        Vector3 laserPos = (Vector3.forward * GunRange);
        float angleDegrees = userConsole.transform.eulerAngles.y;
        if (lr == LeftRight.Left)
        {
            angleDegrees += angleL;
            laserPos -= GunOffset * Vector3.right;

        }
        else
        {
            angleDegrees += angleR;
            laserPos += GunOffset * Vector3.right;

        }
        Vector3 finalPosition = userConsole.transform.rotation * laserPos;

        GameObject g = ObjectPooler.SharedInstance.GetPooledObject();
        g.transform.position = ship.position + finalPosition + Vector3.down;
        g.transform.rotation = Quaternion.Euler(0, angleDegrees, 0);
        g.SetActive(true);

    }

    IEnumerator Shoot(LeftRight lr, Valve.VR.InteractionSystem.Hand h)
    {
        if (canShootL && lr == LeftRight.Left)
        {
            canShootL = false;
            _Shoot(lr);
            h.controller.TriggerHapticPulse(3000);
            h.controller.TriggerHapticPulse(1500);
            yield return cooldownDelay;
            canShootL = true;
        }
        else if (canShootR && lr == LeftRight.Right)
        {
            canShootR = false;
            _Shoot(lr);
            h.controller.TriggerHapticPulse(3000);
            h.controller.TriggerHapticPulse(1500);
            yield return cooldownDelay;
            canShootR = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
        angleL = leftJoystick.outAngle * spinModifier;
        angleR = rightJoystick.outAngle * spinModifier;

        handleTriggerPulls(hand);
        handleTriggerPulls(hand2);

    }

    private void handleTriggerPulls(Valve.VR.InteractionSystem.Hand h)
    {
        // (Button was pressed (initial cilck only)|| button is being held down)
        if (h.controller != null && (h.controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) || h.controller.GetPress(SteamVR_Controller.ButtonMask.Trigger)))
        {
            if (h.hoveringInteractable == rightGun)
            {
                StartCoroutine(Shoot(LeftRight.Right, h));
            }

            if (h.hoveringInteractable == leftGun)
            {
                StartCoroutine(Shoot(LeftRight.Left, h));
            }
        }

        if (h.controller != null && h.controller.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("Trigger released");
        }
    }


}
