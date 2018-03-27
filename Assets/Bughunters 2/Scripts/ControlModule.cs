using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlModule : MonoBehaviour {

    public void MakeActive()
    {
        this.gameObject.SetActive(true);
    }

    public void MakeInactive()
    {
        this.gameObject.SetActive(false);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
