﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringModule : ControlModule {

    public float currentThrust = 0f;
    public float maxThrust = 10f;
    public float currentAngle = 0f;
    public float angularVelocity = 0.01f;

    public float thrusterAngleRange = 60f;

    public Valve.VR.InteractionSystem.CircularDrive leftJoystick;
    public Valve.VR.InteractionSystem.CircularDrive rightJoystick;
    public Valve.VR.InteractionSystem.LinearDrive throttle;

    public Transform thruster;
    public float thrusterOffset = 15f;

    public Rigidbody shipRigidbody;

    bool leftIsHandled;
    bool rightIsHandled;

    public Transform userConsole;

	// Use this for initialization
	void Start () {
        leftJoystick.onGrabbedByHand += onAttachedToHandLeft;
        leftJoystick.onReleasedByHand += onDetachedFromHandLeft;

        rightJoystick.onGrabbedByHand += onAttachedToHandRight;
        rightJoystick.onReleasedByHand += onDetachedFromHandRight;

        currentAngle = 0f;
    }

    void onAttachedToHandLeft(Valve.VR.InteractionSystem.Hand hand)
    {
        Debug.Log("ATTACHED");
        leftIsHandled = true;
    }

    void onAttachedToHandRight(Valve.VR.InteractionSystem.Hand hand)
    {
        Debug.Log("ATTACHED");
        rightIsHandled = true;
    }

    void onDetachedFromHandLeft(Valve.VR.InteractionSystem.Hand hand)
    {
        Debug.Log("DETACHED");
        leftIsHandled = false;
    }

    void onDetachedFromHandRight(Valve.VR.InteractionSystem.Hand hand)
    {
        Debug.Log("DETACHED");
        rightIsHandled = false;
    }

    // Update is called once per frame
    void Update () {
        currentAngle -= (leftJoystick.outAngle + rightJoystick.outAngle) * Time.deltaTime * angularVelocity;
        if (currentAngle > (thrusterAngleRange / 2f))
        {
            currentAngle = thrusterAngleRange / 2f;
        }
        if (currentAngle < (-1f * thrusterAngleRange / 2f))
        {
            currentAngle = -1f * thrusterAngleRange / 2f;
        }

        Debug.Log(userConsole.transform.eulerAngles.y);



        float currentAngleRadians = (userConsole.transform.eulerAngles.y + currentAngle + 180f) * Mathf.Deg2Rad;

        thruster.transform.localPosition = new Vector3(Mathf.Sin(currentAngleRadians) * thrusterOffset, thruster.transform.localPosition.y, Mathf.Cos(currentAngleRadians) * thrusterOffset);
        thruster.transform.localRotation = Quaternion.Euler(0f, currentAngle + userConsole.transform.eulerAngles.y, 0f);

        // .13 = full forward, -.13 is rest
        float throttleOffset = 0.26f;
        float throttlePercent = (throttle.transform.localPosition.x + 0.5f * throttleOffset) / throttleOffset;

        if (throttlePercent < 0.03f)
        {
            throttlePercent = 0f;
        }

        currentThrust = throttlePercent * maxThrust;
    }

    void FixedUpdate()
    {
        shipRigidbody.AddForceAtPosition(thruster.transform.rotation * Vector3.forward * currentThrust, thruster.transform.position);
    }
}
