﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour {
    public float radius = 10f;
    public float speed = 0.1f;
    private float theta = 0f;

    public Transform player;
    public Transform headset;

    public Vector3 origin;
    private Vector3 initialPosition;

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    public Vector3 diff;
    public float offsetDeadZone = 0.05f;

    public Rigidbody shipRB;

	// Use this for initialization
	void Start () {
        initialPosition = transform.position - shipRB.transform.position;
        ResetPosition();
	}
	
	// Update is called once per frame
	void Update () {
        if((Input.GetKey(KeyCode.JoystickButton0) && Input.GetKey(KeyCode.JoystickButton2)) || Input.GetKey(KeyCode.Space))
        {
            ResetPosition();
        }

        diff = GetLocalHeadsetPosition() - origin;

        Debug.DrawLine(GetLocalHeadsetPosition(), origin, Color.red);

        if (Mathf.Abs(diff.x) > offsetDeadZone)
        {
            theta += Time.deltaTime * speed * (diff.x - (Mathf.Sign(diff.x) * offsetDeadZone));
        }

        targetPosition = new Vector3(Mathf.Sin(theta) * radius, 0, Mathf.Cos(theta) * radius) + initialPosition + shipRB.transform.position;
        targetRotation = Quaternion.Euler(0, Mathf.Rad2Deg * theta, 0);

        transform.position = targetPosition;
        transform.rotation = targetRotation;
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

        origin = GetLocalHeadsetPosition();
    }

    Vector3 GetLocalHeadsetPosition()
    {
        return Quaternion.Inverse(transform.rotation) * (headset.position - transform.position);
    }
}
