using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.SceneManagement;

//Este componente controla el input del auto.
//Requiere componente Drivetrain en el actor.

public class CarControllerv2 : MonoBehaviour {
	//Logitech
	LogitechGSDK.LogiControllerPropertiesData properties;
	private bool IsCustomProperties;
	//Logitech Input Constant [LIC]
	private float LIC;

	public Transform steeringWheel;
	private bool debugGUI = true;

	private float timeToSendData;

	Powertrain powertrain;

	LightsController lights;

	private float timeToEnd;

	private bool IsOn;

	void UpdateSteeringWheelRotation(float steer){
		//steeringWheel.transform.localRotation = Quaternion.Euler (0, 0, -Input.GetAxis ("Horizontal")*90);
		//if (Input.GetKeyDown (KeyCode.F12))
			//debugGUI = !debugGUI;
		float steer_copy = steer;
		int force = (int)Mathf.Round(Mathf.Abs (steer_copy) * Mathf.Sign (steer_copy) * GetComponent<Rigidbody> ().velocity.magnitude * 10);
		if(force > 0) {
			LogitechGSDK.LogiPlayConstantForce (0, force);
		}
		else if (force < 0) {
			LogitechGSDK.LogiPlayConstantForce (0, force);
		}
		else {
			if (LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_CONSTANT)){
				LogitechGSDK.LogiStopConstantForce (0);
			}
		}
	}


	// Use this for initialization
	void Start () {
		timeToEnd = 0;
		timeToSendData = 0;
		powertrain = GetComponent<Powertrain> ();
		lights = GetComponent<LightsController> ();
		LIC = 32767;
		Debug.Log(LogitechGSDK.LogiSteeringInitialize(false));
		IsCustomProperties = false;
		IsOn = false;
		UnityEngine.VR.VRSettings.enabled = true;
		UnityEngine.VR.InputTracking.Recenter();
	}
	
	// Update is called once per frame
	void Update () {
		timeToSendData += Time.deltaTime;
		timeToEnd += Time.deltaTime;
		if (timeToEnd > 120) {
			//API.finalizarSesion ();
			//API v2
			APIv2.EndSession ();
			SceneManager.LoadScene ("MenuPrincipal");
		}
		SendDataVelocity ();
		if(LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0)){
			if(!IsCustomProperties){
				//Initialice the Custom Properties
				//Commented due to crash Unity | Uncomment on Build
				//LogitechGSDK.LogiGetCurrentControllerProperties(0, ref properties);
        	//	properties.wheelRange = 450;
        		//LogitechGSDK.LogiSetPreferredControllerProperties(properties);
        		//IsCustomProperties = true;
			}

			//Input from Logitech SDK
			LogitechGSDK.DIJOYSTATE2ENGINES rec;
			rec = LogitechGSDK.LogiGetStateUnity (0);

			if (IsOn) {
				//Get the steer
				float steer = rec.lX / LIC;
				// Porcentaje de punto muerto = 5%
				if (steer < .05f && steer > -.05f) {
					steer = 0;
				}

				powertrain.Throttle = ConvertLogitechInput (rec.lY);
				powertrain.Brake = ConvertLogitechInput (rec.lRz);
				powertrain.Clutch = ConvertLogitechInput (rec.rglSlider [1]);

				
				UpdateSteeringWheelRotation (steer);

				float finalAngle = steer * 45f;
				
				powertrain.Steering = finalAngle;

				if (LogitechGSDK.LogiButtonTriggered (0, 8)) {
					if (powertrain.ShiftTo (1)){
						SendDataCambios ();
					}
				} else if (LogitechGSDK.LogiButtonTriggered (0, 9)) {
					if (powertrain.ShiftTo (2)) {
						SendDataCambios ();
					}
				} else if (LogitechGSDK.LogiButtonTriggered (0, 10)) {
					if (powertrain.ShiftTo (3)) {
						SendDataCambios ();
					}
				} else if (LogitechGSDK.LogiButtonTriggered (0, 11)) {
					if (powertrain.ShiftTo (4)) {
						SendDataCambios ();
					}
				} else if (LogitechGSDK.LogiButtonTriggered (0, 12)) {
					if (powertrain.ShiftTo (5)) {
						SendDataCambios ();
					}
				} else if (LogitechGSDK.LogiButtonTriggered (0, 13)) {
					if (powertrain.ShiftTo (6)) {
						SendDataCambios ();
					}
				} else if (LogitechGSDK.LogiButtonTriggered (0, 14)) {
					if (powertrain.ShiftTo (7)) {
						SendDataCambios ();
					}
				} else if (rec.rgbButtons [8] != 128 &&
				           rec.rgbButtons [9] != 128 &&
				           rec.rgbButtons [10] != 128 &&
				           rec.rgbButtons [11] != 128 &&
				           rec.rgbButtons [12] != 128 &&
				           rec.rgbButtons [13] != 128 &&
				           rec.rgbButtons [14] != 128) {
					powertrain.ShiftTo (0);
				}

				if (LogitechGSDK.LogiButtonTriggered (0, 6)) {
					lights.WhiteLights ();
				} else if (LogitechGSDK.LogiButtonTriggered (0, 4)) {
					lights.ActivateTurnRigth ();
				} else if (LogitechGSDK.LogiButtonTriggered (0, 5)) {
					lights.ActivateTurnLeft ();
				} else if (LogitechGSDK.LogiButtonTriggered (0, 1)) {
					UnityEngine.VR.InputTracking.Recenter();
				}

				UpdateLights ();
			}

			if (LogitechGSDK.LogiButtonTriggered (0, 0)) {
				IsOn = !IsOn;
			}

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
			//API.registrarVelocidad(speed);
			//API v2
			APIv2.RegisterVelocity (speed);
			//Debug.Log("Speed: "+speed.ToString());
			timeToSendData = 0;
		}
	}

	void SendDataCambios(){
		float speed = Mathf.RoundToInt (GetComponent<Rigidbody> ().velocity.magnitude * 3.6f);
		int rpm = powertrain.GetCurrentEngagedRPM();
		//Debug.Log ("RPM: " + rpm.ToString ());
		int gear = (powertrain.GetCurrentGear () == 7) ? 0 : powertrain.GetCurrentGear ();
		//API.registrarCambio(speed, rpm, gear);
		//API v2
		APIv2.RegisterGearUp(speed, rpm, gear);
		//Debug.Log("Velocidad: "+speed.ToString()+" RPM: "+rpm.ToString()+" Gear: "+gear.ToString());
	}

	//Convert Logitech Input to Clamp [0, 1] Input
	private float ConvertLogitechInput(float input){
		return (((-input) + LIC) / (2 * LIC));
	}

	private void UpdateLights () {
		int minRPM = powertrain.GetRPMS(0);
		int maxRPM = powertrain.GetRPMS(2);
		int actualRPM = powertrain.GetRPMS(1);
		LogitechGSDK.LogiPlayLeds(0, actualRPM, minRPM, maxRPM);
	}

	public bool CarIsOn () {
		return IsOn;
	}
}
