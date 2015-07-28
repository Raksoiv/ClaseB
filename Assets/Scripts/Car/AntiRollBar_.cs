using UnityEngine;
using System.Collections;

public class AntiRollBar_ : MonoBehaviour {

	public WheelCollider WheelL;
	public WheelCollider WheelR;
	public float AntiRoll = 5000f;


	// Update for physics objects
	void FixedUpdate() {
		WheelHit hit;
		float travelL = 1f;
		float travelR = 1f;

		var groundedL = WheelL.GetGroundHit (out hit);
		if (groundedL)
			travelL = (-WheelL.transform.InverseTransformPoint (hit.point).y - WheelL.radius) / WheelL.suspensionDistance;

		var groundedR = WheelR.GetGroundHit (out hit);
		if (groundedR)
			travelR = (-WheelR.transform.InverseTransformPoint (hit.point).y - WheelR.radius) / WheelR.suspensionDistance;

		var antiRollForce = (travelL - travelR) * AntiRoll;

		if (groundedL) 
			GetComponent<Rigidbody>().AddForceAtPosition(WheelL.transform.up * -antiRollForce, WheelL.transform.position); 
		if (groundedR) 
			GetComponent<Rigidbody>().AddForceAtPosition(WheelR.transform.up * -antiRollForce, WheelR.transform.position); 
	}
}
