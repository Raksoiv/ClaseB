using UnityEngine;
using System.Collections;

//Este componente controla el input del auto.
//Requiere componente Drivetrain en el actor.

public class CarController : MonoBehaviour {
	public Transform steeringWheel;

	Drivetrain drivetrain;

	void UpdateSteeringwheelRotation(float deg){
	//	steeringWheel.transform.localRotation.eulerAngles.z = deg;
	}

	// Use this for initialization
	void Start () {
		drivetrain = GetComponent<Drivetrain> ();
	}
	
	// Update is called once per frame
	void Update () {
	}
	//Update for physics
	void FixedUpdate(){
		float steer = Input.GetAxis ("Horizontal");
		float accelerate = Input.GetAxis ("Vertical");

		drivetrain.Throttle = accelerate;
	}
}
