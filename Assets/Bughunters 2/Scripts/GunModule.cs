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

    public SteamVR_TrackedObject trackedObj;

    public SteamVR_Controller.Device device;

    public SteamVR_Controller.Device rightDevice;
    public SteamVR_Controller.Device leftDevice;

    // Use this for initialization
    void Start () {
        cooldownDelay = new WaitForSeconds(Cooldown);
        int leftIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
        int rightIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
        leftDevice = SteamVR_Controller.Input(leftIndex);
        rightDevice = SteamVR_Controller.Input(rightIndex);
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
        device = SteamVR_Controller.Input((int)trackedObj.index);
        device.TriggerHapticPulse(750);

    }

    IEnumerator Shoot(LeftRight lr)
    {
        if (canShootL && lr == LeftRight.Left)
        {
            canShootL = false;
            _Shoot(lr);
            yield return cooldownDelay;
            canShootL = true;
        }
        else if (canShootR && lr == LeftRight.Right)
        {
            canShootR = false;
            _Shoot(lr);
            yield return cooldownDelay;
            canShootR = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
        angleL = leftJoystick.outAngle * spinModifier;
        angleR = rightJoystick.outAngle * spinModifier;

        // TODO: Check if we're holding down the trigger

    }

    private void LeftTriggerPressed(object sender, ClickedEventArgs e)
    {
        StartCoroutine(Shoot(LeftRight.Left));
    }

    private void RightTriggerPressed(object sender, ClickedEventArgs e)
    {
        StartCoroutine(Shoot(LeftRight.Right));
    }
}
