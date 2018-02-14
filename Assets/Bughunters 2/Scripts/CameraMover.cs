using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour {
    public float radius = 10f;
    public float speed = 0.1f;
    private float theta = 0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // TODO: Alter theta based on player lean
        theta += Time.deltaTime * speed;

        this.transform.position = new Vector3(Mathf.Sin(theta) * radius, 0, Mathf.Cos(theta) * radius);
        this.transform.rotation = Quaternion.Euler(0, Mathf.Rad2Deg * theta, 0);
	}
}
