using UnityEngine;
using System.Collections;

public class OculusController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		UnityEngine.VR.VRSettings.enabled = true;
		UnityEngine.VR.InputTracking.Recenter();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButton("OculusReset")){
			UnityEngine.VR.InputTracking.Recenter();
		}
	}
}
