using UnityEngine;

//Este componente controla el input del auto.
//Requiere componente Drivetrain en el actor.

public class CarController : MonoBehaviour {
	public Transform steeringWheel;
	private bool debugGUI = false;

	Powertrain drivetrain;

	void UpdateSteeringwheelRotation(float deg){
	//	steeringWheel.transform.localRotation.eulerAngles.z = deg;
	}

	// Use this for initialization
	void Start () {
		drivetrain = GetComponent<Powertrain> ();
	}
	
	// Update is called once per frame
	void Update () {
		steeringWheel.transform.localRotation = Quaternion.Euler (0, 0, -Input.GetAxis ("Horizontal")*90);
		if (Input.GetKeyDown (KeyCode.F12))
			debugGUI = !debugGUI;
	}
	//Update for physics
	void FixedUpdate(){
		//Input de teclado
		float steer = Input.GetAxis ("Horizontal");
		float accelerate = Input.GetAxis ("Vertical");

		drivetrain.Throttle = accelerate;
		drivetrain.Brake = accelerate * -1;

		float finalAngle = steer * 45f;
	
		drivetrain.Steering = finalAngle;
	}

	void OnGUI() {
		if (debugGUI) {
			var speed = GetComponent<Rigidbody> ().velocity.magnitude * 3.6f;
			GUI.Box (new Rect (50, 50, 140, 55),
			         "Velocidad: " + speed.ToString ("F0") + " Km/h" + System.Environment.NewLine +
			         "RPM: " + drivetrain.GetCurrentRPM().ToString() + System.Environment.NewLine +
			         "Cambio: " + drivetrain.GetCurrentGear().ToString() + System.Environment.NewLine
			);
		}
	}

}
