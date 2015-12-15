using UnityEngine;
using System.Collections;
using System.Text;

//Este componente controla el input del auto.
//Requiere componente Drivetrain en el actor.

public class CarControllerv2 : MonoBehaviour {
	//Logitech
	LogitechGSDK.LogiControllerPropertiesData properties;
	//Logitech Input Constant [LIC]
	private float LIC;

	public Transform steeringWheel;
	private bool debugGUI = true;

	private float timeToSendData;

	Powertrain powertrain;

	void UpdateSteeringWheelRotation(){
		//steeringWheel.transform.localRotation = Quaternion.Euler (0, 0, -Input.GetAxis ("Horizontal")*90);
		//if (Input.GetKeyDown (KeyCode.F12))
			//debugGUI = !debugGUI;
	}


	// Use this for initialization
	void Start () {
		timeToSendData = 0;
		powertrain = GetComponent<Powertrain> ();
		LIC = 32767;
		Debug.Log(LogitechGSDK.LogiSteeringInitialize(false));
	}
	
	// Update is called once per frame
	void Update () {
		timeToSendData += Time.deltaTime;
		SendDataVelocity ();
		if(LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0)){

			//Input from Logitech SDK
			LogitechGSDK.DIJOYSTATE2ENGINES rec;
	    rec = LogitechGSDK.LogiGetStateUnity(0);

	    float steer = rec.lX / LIC;
	    powertrain.Throttle = ConvertLogitechInput(rec.lY);
	    powertrain.Brake = ConvertLogitechInput(rec.lRz);
	    powertrain.Clutch = ConvertLogitechInput(rec.rglSlider[1]);

			
			UpdateSteeringWheelRotation ();

			float finalAngle = steer * 45f;
			
			powertrain.Steering = finalAngle;

			if (rec.rgbButtons[8] == 128) {
				if(powertrain.ShiftTo (1))
					SendDataCambios ();
			} else if (rec.rgbButtons[9] == 128) {
				if(powertrain.ShiftTo (2))
					SendDataCambios ();
			} else if (rec.rgbButtons[10] == 128) {
				if(powertrain.ShiftTo (3))
					SendDataCambios ();
			} else if (rec.rgbButtons[11] == 128) {
				if(powertrain.ShiftTo (4))
					SendDataCambios ();
			} else if (rec.rgbButtons[12] == 128) {
				if(powertrain.ShiftTo (5))
					SendDataCambios ();
			} else if (rec.rgbButtons[13] == 128) {
				if(powertrain.ShiftTo (6))
					SendDataCambios ();
			} else if (rec.rgbButtons[14] == 128) {
				if(powertrain.ShiftTo (7))
					SendDataCambios();
			}
			else
				powertrain.ShiftTo(0);
		}

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

	//Convert Logitech Input to Clamp [0, 1] Input
	private float ConvertLogitechInput(float input){
		return (((-input) + LIC) / (2 * LIC));
	}
}
