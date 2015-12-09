using UnityEngine;
using System.Collections;

/*
 * This script based only in the following document
 * http://www.asawicki.info/Mirror/Car%20Physics%20for%20Games/Car%20Physics%20for%20Games.html
 */
public class CarControllerv2 : MonoBehaviour {

	//Stats of the Car
	private float velocity;
	private int actualGear;

	//Gear Configuration
	public float[] GearRatios;
	public float DifferentialRatio;

	//Torque Curve
	public AnimationCurve TorqueCurve;

	//Forces
	private float Tdrive;

	//Contants
	private float Cdrag;
	private float Cd;
	private float A;
	private float rho;
	private float Crr; //Contant of the rolling resistance
	private float Cbraking;
	private float efficiency; //Efficiency of the gears

	//WheelColliders
	public WheelCollider[] wheels;

	// Use this for initialization
	void Start () {
		InitializeConstants ();
		velocity = 0;
		actualGear = 0;
	}
	
	// Update is called once per frame
	void Update () {
		CalculateConstants ();
		CalculateForces ();
	}

	void FixedUpdate () {
		//if (velocity < 1 && Flong <= 0)
		//	GetComponent<Rigidbody> ().velocity = new Vector3 (0, 0, 0);

		//wheels [2].motorTorque = Flong / 2f;
		//wheels [3].motorTorque = Flong / 2f;
	}

	void InitializeConstants(){
		rho = 1.29f;
		Cd = .4f;
		A = 3.2f;

		Cbraking = 700;

		efficiency = .7f;
	}

	void CalculateConstants(){
		velocity = GetComponent<Rigidbody> ().velocity.magnitude;
		Cdrag = 0.5f * Cd * A * rho;
		Crr = 30 * Cdrag;
	}

	void CalculateForces (){
		float Fdrag = -Cdrag * velocity * velocity;
		float Frr = -Crr * velocity;
		float Fbraking = -Cbraking;

		float throttle = Input.GetAxis ("Throttle");

		float Tmax = TorqueCurve.Evaluate (RPM);

		float Tengine = Tmax * throttle;

		Tdrive = Tengine * GearRatios [actualGear] * DifferentialRatio * efficiency;
	}
}
