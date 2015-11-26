using UnityEngine;
using System.Collections;

public class VelocityController : MonoBehaviour {

	public GameObject tacometro;
	public GameObject velocimetro;

	private float actualRpm;
	private float actualKmh;

	// Use this for initialization
	void Start () {
		actualRpm = 0;
		actualKmh = 0;
	}
	
	// Update is called once per frame
	void Update () {
		float rpm = Powertrain.GetCurrentRPM ();
		float rpmDifference = rpm - actualRpm;
		if(rpmDifference != 0)
			tacometro.transform.Rotate (0, 0, rpmDifference*(18/630.0f), Space.Self);

		float kmh = GetComponent<Rigidbody> ().velocity.magnitude * 3.6f;
		float kmhDifference = kmh - actualKmh;
		if(kmhDifference != 0)
			velocimetro.transform.Rotate (0, 0, kmhDifference*(180/125f), Space.Self);

		actualRpm = rpm;
		actualKmh = kmh;
	}
}
