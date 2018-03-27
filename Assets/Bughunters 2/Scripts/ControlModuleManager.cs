using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlModuleManager : MonoBehaviour {

    public SteeringModule SteeringModule;

	// Use this for initialization
	void Start () {
        SteeringModule.MakeActive();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
