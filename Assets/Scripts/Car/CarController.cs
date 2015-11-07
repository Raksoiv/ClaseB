using UnityEngine;

//Este componente controla el input del auto.
//Requiere componente Drivetrain en el actor.

public class CarController : MonoBehaviour {
	[SerializeField]private bool isAutomatic = false;							

	public Transform steeringWheel;
	private bool debugGUI = true;

	Powertrain powertrain;

	void UpdateSteeringWheelRotation(){
		steeringWheel.transform.localRotation = Quaternion.Euler (0, 0, -Input.GetAxis ("Horizontal")*90);
		if (Input.GetKeyDown (KeyCode.F12))
			debugGUI = !debugGUI;
	}


	// Use this for initialization
	void Start () {
		powertrain = GetComponent<Powertrain> ();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateSteeringWheelRotation ();

		//Input de teclado
		float steer = Input.GetAxis ("Horizontal");
		float combinedInput = Input.GetAxis ("Vertical");
		float clutch = Input.GetAxis ("Clutch");

		powertrain.Throttle = Mathf.Clamp01(combinedInput);
		powertrain.Brake = - Mathf.Clamp (combinedInput, -1, 0);
		powertrain.Clutch = (clutch + 1)/2f;

		float finalAngle = steer * 45f;
		
		powertrain.Steering = finalAngle;

		if (Input.GetButtonDown ("1stGear"))
			powertrain.ShiftTo(1);
		else if (Input.GetButtonDown ("2ndGear"))
			powertrain.ShiftTo(2);
		else if (Input.GetButtonDown ("3rdGear"))
			powertrain.ShiftTo(3);
		else if (Input.GetButtonDown ("4thGear"))
			powertrain.ShiftTo(4);
		else if (Input.GetButtonDown ("5thGear"))
			powertrain.ShiftTo(5);
		else if (Input.GetButtonDown ("6thGear"))
			powertrain.ShiftTo(6);
		else if (Input.GetButtonDown ("ReverseGear"))
			powertrain.ShiftTo(7);
		else if (Input.GetButtonUp ("1stGear") || Input.GetButtonUp ("2ndGear") || Input.GetButtonUp ("3rdGear") || Input.GetButtonUp ("4thGear") || Input.GetButtonUp ("5thGear") || Input.GetButtonUp ("6thGear") || Input.GetButtonUp ("ReverseGear"))
			powertrain.ShiftTo(0);
	}


	void OnGUI() {
		if (debugGUI) {
			float speed = GetComponent<Rigidbody> ().velocity.magnitude * 3.6f;
			string rpm = Powertrain.GetCurrentRPM().ToString();
			string gear = (powertrain.GetCurrentGear() == 7)? "R" : powertrain.GetCurrentGear().ToString();
			GUI.Box (new Rect (50, 50, 140, 55),
			         "Speed : " + speed.ToString ("F0") + " Km/h" + System.Environment.NewLine +
			         "RPM   : " + rpm + System.Environment.NewLine +
			         "Gear  : " + gear + System.Environment.NewLine
			);
		}
	}

}
