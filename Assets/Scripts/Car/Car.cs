using UnityEngine;
using System.Collections;

//Script inicial para testear el auto, la idea es realizar logica nueva.

public class Car : MonoBehaviour {

	public float maxTorque = 50f;

	public Transform centerOfMass;
	public Transform steeringWheel;

	public WheelCollider[] wheelColliders = new WheelCollider[4];
	public Transform[] tireMeshes = new Transform[4];

	private Rigidbody m_rigidBody;

	//Updates Tire Mesh Positions according to the respective WheelCollider
	void UpdateWheelMeshesPositions(){
		for (int i = 0; i < 4; i++) {
			Quaternion quat;
			Vector3 pos;

			wheelColliders[i].GetWorldPose(out pos, out quat);

			tireMeshes[i].position = pos;
			tireMeshes[i].rotation = quat;

		}
		steeringWheel.transform.localRotation = Quaternion.Euler (0, 0, -Input.GetAxis ("Horizontal")*90);
	}

	// Use this for initialization
	void Start () {
		m_rigidBody = GetComponent<Rigidbody> ();
		m_rigidBody.centerOfMass = centerOfMass.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateWheelMeshesPositions ();
	}

	//Update for physics
	void FixedUpdate(){
		float steer = Input.GetAxis ("Horizontal");
		float accelerate = Input.GetAxis ("Vertical");

		float finalAngle = steer * 45f;

		wheelColliders [0].steerAngle = finalAngle;
		wheelColliders [1].steerAngle = finalAngle;

		wheelColliders[0].motorTorque = accelerate * maxTorque;
		wheelColliders [1].motorTorque = accelerate * maxTorque;
	}
}
