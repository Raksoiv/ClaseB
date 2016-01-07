using UnityEngine;
using System.Collections;

public class EstadisticaCarril : MonoBehaviour {

	//Flag fot outside
	bool outside;

	//Offset Time for detector
	float offset;

	//Tiempo fera de la pista
	float timeOutside;

	// Use this for initialization
	void Start () {
		outside = false;
		offset = -1;
		timeOutside = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (outside || (offset < 2 && offset != -1)) {
			timeOutside += Time.deltaTime;
		} else {
			if (timeOutside > 0) {
				API.salidaCarril (timeOutside);
				offset = -1;
				timeOutside = 0;
			}
		}
		if (outside) {
			offset = -1;
		}
		if (!outside) {
			if (offset != -1) {
				offset += Time.deltaTime;
			}
		}
	}

	void OnTriggerEnter (Collider other) {
		if (other.name == "Detector") {
			outside = true;
		}
	}

	void OnTriggerStay (Collider other) {
		if (other.name == "Detector") {
			outside = true;
		}
	}

	void OnTriggerExit (Collider other) {
		if (other.name == "Detector") {
			outside = false;
			offset = 0;
		}
	}
}
