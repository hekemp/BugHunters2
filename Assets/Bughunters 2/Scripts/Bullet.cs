using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float Speed = 1f;
    public Quaternion rotation;

	// Use this for initialization
	void OnEnable () {
        rotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = rotation;
        transform.position += Time.deltaTime * Speed * (transform.rotation * Vector3.forward);
	}
}
