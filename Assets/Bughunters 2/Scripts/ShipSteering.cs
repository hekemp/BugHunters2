using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSteering : MonoBehaviour {

    public float speed = 0f;
    public float maxSpeed = 10f;
    public float angle = 0f;

    public Valve.VR.InteractionSystem.CircularDrive wheel;

    public Valve.VR.InteractionSystem.LinearDrive throttle;
    public float maxThrottle = 5f;
    public float currentThrust = 0f;

    public Transform thruster;
    private float thrusterOffset = 15f;

    private Rigidbody rb;
    public Rigidbody playerRB;

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
        //rb.MoveRotation(Quaternion.Euler(0, wheel.outAngle, 0));

        // TODO: Throttle!
        speed = 1f;

        // .13 = full forward, -.13 is rest
        float currentThrottle = throttle.transform.localPosition.x + .13f;

        Debug.Log(currentThrust);
        Debug.Log(.05 * maxThrottle);

        if (currentThrottle < .03 && currentThrust < .05 * maxThrottle)
        {
            currentThrust = 0;

        }
        else if (currentThrottle < .03 )
        {   // Adds some drag/deceleration so you don't just stop instantly
            currentThrust = currentThrust - .003f * maxThrottle;
        }
        else
        {
            //currentThrust = 5f;

            currentThrust = (currentThrottle / .26f) * maxThrottle;

        }

    }

    void FixedUpdate()
    {
        //rb.AddForce(Quaternion.Euler(0f, angle, 0f) * Vector3.forward * speed * maxSpeed);

        //rb.AddRelativeForce(Vector3.forward * currentThrust);

        rb.MovePosition(rb.position + rb.transform.forward * currentThrust);
        //playerRB.MovePosition(playerRB.position + Vector3.forward * .01f);
        //playerRB.position = playerRB.position + Vector3.forward * .01f;
    }
}
