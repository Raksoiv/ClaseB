using UnityEngine;

//Este componente controla el input del auto.
//Requiere componente Drivetrain en el actor.

public class CarController : MonoBehaviour {
	[SerializeField]private bool isAutomatic = false;							

	public Transform steeringWheel;
	private bool debugGUI = true;

	private float timeToSendData;

	Powertrain powertrain;

	void UpdateSteeringWheelRotation(){
		steeringWheel.transform.localRotation = Quaternion.Euler (0, 0, -Input.GetAxis ("Horizontal")*90);
		if (Input.GetKeyDown (KeyCode.F12))
			debugGUI = !debugGUI;
	}


	// Use this for initialization
	void Start () {
		timeToSendData = 0;
		powertrain = GetComponent<Powertrain> ();
	}
	
	// Update is called once per frame
	void Update () {
		timeToSendData += Time.deltaTime;
		SendDataVelocity ();
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

		if (Input.GetButtonDown ("1stGear")) {
			powertrain.ShiftTo (1);
			SendDataCambios ();
		} else if (Input.GetButtonDown ("2ndGear")) {
			powertrain.ShiftTo (2);
			SendDataCambios ();
		} else if (Input.GetButtonDown ("3rdGear")) {
			powertrain.ShiftTo (3);
			SendDataCambios ();
		} else if (Input.GetButtonDown ("4thGear")) {
			powertrain.ShiftTo (4);
			SendDataCambios ();
		} else if (Input.GetButtonDown ("5thGear")) {
			powertrain.ShiftTo (5);
			SendDataCambios ();
		} else if (Input.GetButtonDown ("6thGear")) {
			powertrain.ShiftTo (6);
			SendDataCambios ();
		} else if (Input.GetButtonDown ("ReverseGear")) {
			powertrain.ShiftTo (7);
			SendDataCambios();
		}
		else if (Input.GetButtonUp ("1stGear") || Input.GetButtonUp ("2ndGear") || Input.GetButtonUp ("3rdGear") || Input.GetButtonUp ("4thGear") || Input.GetButtonUp ("5thGear") || Input.GetButtonUp ("6thGear") || Input.GetButtonUp ("ReverseGear"))
			powertrain.ShiftTo(0);

		if (timeToSendData > 5) {
			timeToSendData = 0;
		}
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

	void SendDataVelocity(){
		if (timeToSendData > 5) {
			float speed = GetComponent<Rigidbody> ().velocity.magnitude * 3.6f;
			API.registrarVelocidad(speed);
			Debug.Log("Speed: "+speed.ToString());
			timeToSendData = 0;
		}
	}

	void SendDataCambios(){
		float speed = GetComponent<Rigidbody> ().velocity.magnitude * 3.6f;
		int rpm = Powertrain.GetCurrentRPM ();
		int gear = (powertrain.GetCurrentGear () == 7) ? 0 : powertrain.GetCurrentGear ();
		API.registrarCambio(speed, rpm, gear);
		Debug.Log("Velocidad: "+speed.ToString()+" RPM: "+rpm.ToString()+" Gear: "+gear.ToString());
	}
}
