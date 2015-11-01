using UnityEngine;
using System.Collections;

public class LightsController : MonoBehaviour {

	public GameObject LowLights;
	public GameObject HighLights;
	public GameObject PannelLight;

	private int lightState;

	// Use this for initialization
	void Start () {
		LowLights.SetActive(false);
		HighLights.SetActive(false);
		PannelLight.SetActive(false);
		lightState = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("LightControl")) {
			if (lightState == 0) {
				LowLights.SetActive(true);
				HighLights.SetActive(false);
				PannelLight.SetActive(true);
				lightState = 1;
			}
			else if (lightState == 1){
				LowLights.SetActive(false);
				HighLights.SetActive(true);
				PannelLight.SetActive(true);
				lightState = 2;
			}
			else if (lightState == 2){
				LowLights.SetActive(false);
				HighLights.SetActive(false);
				PannelLight.SetActive(false);
				lightState = 0;
			}
		}
	}
}
