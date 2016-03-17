using UnityEngine;
using System.Collections;

public class OculusController : MonoBehaviour {

	public bool ActivateOculus;

	// Use this for initialization
	void Start () {
		UnityEngine.VR.VRSettings.enabled = ActivateOculus;
		//UnityEngine.VR.InputTracking.Recenter();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButton("OculusReset")){
			UnityEngine.VR.InputTracking.Recenter();
		}
	}
}
