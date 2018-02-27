using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour {
    public float radius = 10f;
    public float speed = 0.1f;
    private float theta = 0f;

    public Transform player;
    public Transform headset;

    private Vector3 origin;
    private Vector3 initialPosition;

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    public Vector3 diff;

    private Rigidbody rb;

	// Use this for initialization
	void Start () {
        initialPosition = transform.position;
        rb = this.GetComponent<Rigidbody>();
        ResetPosition();
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKey(KeyCode.JoystickButton0) && Input.GetKey(KeyCode.JoystickButton2))
        {
            ResetPosition();
        }

        diff = headset.localPosition - origin;

        Debug.Log(diff.x);
        if (Mathf.Abs(diff.x) > 0.05f)
        {
            theta += Time.deltaTime * speed * (diff.x - (Mathf.Sign(diff.x) * 0.05f));
        }

        targetPosition = new Vector3(Mathf.Sin(theta) * radius, 0, Mathf.Cos(theta) * radius) + initialPosition;
        targetRotation = Quaternion.Euler(0, Mathf.Rad2Deg * theta, 0);
	}

    void FixedUpdate()
    {
        rb.MovePosition(targetPosition);
        rb.MoveRotation(targetRotation);
    }

    void ResetPosition()
    {
        Debug.Log("Reset!");
        Vector3 diff = headset.position - transform.position;
        diff.y = 0;
        player.position -= diff;
        diff = headset.rotation.eulerAngles - transform.rotation.eulerAngles;
        diff.x = diff.z = 0;
        player.rotation = Quaternion.Euler(player.rotation.eulerAngles - diff);
        origin = headset.localPosition;
    }
}
