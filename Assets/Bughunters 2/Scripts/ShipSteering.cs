using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSteering : MonoBehaviour {

    public float speed = 0f;
    public float maxSpeed = 10f;
    public float angle = 0f;

    public Valve.VR.InteractionSystem.CircularDrive wheel;

    public Transform thruster;
    private float thrusterOffset = 15f;

    private Rigidbody rb;

    bool isHandled;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();

        wheel.onGrabbedByHand += onAttachedToHand;
        wheel.onReleasedByHand += onDetachedFromHand;
	}

    void onAttachedToHand(Valve.VR.InteractionSystem.Hand hand)
    {
        Debug.Log("ATTACHED");
        isHandled = true;
    }

    void onDetachedFromHand(Valve.VR.InteractionSystem.Hand hand)
    {
        Debug.Log("DETACHED");
        isHandled = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (isHandled)
        {
            angle += wheel.outAngle * Time.deltaTime / 100f;
        }

        thruster.localPosition = new Vector3(Mathf.Sin(angle + 180) * thrusterOffset, 1.5f, Mathf.Cos(angle + 180) * thrusterOffset);
        thruster.localRotation = Quaternion.Euler(0, -angle, 0);

        // TODO: Throttle!
        speed = 1f;
	}

    void FixedUpdate()
    {
        rb.AddForce(Quaternion.Euler(0f, angle, 0f) * Vector3.forward * speed * maxSpeed);
    }
}
