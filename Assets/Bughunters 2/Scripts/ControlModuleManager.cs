using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlModuleManager : MonoBehaviour {

    public static ControlModuleManager Instance;

    public List<ControlModule> ControlModules = new List<ControlModule>();

    public int ActiveIndex;

	// Use this for initialization
	void Start () {
        Instance = this;
        ActiveIndex = 0;
        UpdateModules();
	}
	
    public void Swap()
    {
        ActiveIndex = (ActiveIndex + 1) % ControlModules.Count;
        UpdateModules();
    }

    void UpdateModules()
    {
        for(int i = 0; i < ControlModules.Count; i++)
        {
            if (i == ActiveIndex)
            {
                ControlModules[i].MakeActive();
            }
            else
            {
                ControlModules[i].MakeInactive();
            }
        }
    }
}
