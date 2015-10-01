using UnityEngine;
using System.Collections;

//Este componente simula y maneja: Motor + Transmision + Ruedas + Diferencial + Eje de Transmision = Powertrain,
//generando torque y aplicandoselo a las ruedas.

public class Powertrain : MonoBehaviour {
	[SerializeField]private AnimationCurve torqueCurve;                             //Curva de RPM/torque[Nm] del motor
	[SerializeField]private float[] gearRatios = new float[7];                      //Relacion de reduccion de los cambios (0 = reversa)
	[SerializeField]private float finalGearRatio;                                   //Relacion de reduccion del diferencial
	[SerializeField]private int minRPM;                                             //RPM de ralenti del motor (usado en cambios automaticos)
	[SerializeField]private int maxRPM; 											//Redline

	[SerializeField]private WheelCollider[] wheelColliders = new WheelCollider[4];  //WheelColliders: Delantera, Delantera, Trasera, Trasera
	[SerializeField]private Transform[] tireMeshes = new Transform[4];              //Meshes de las ruedas (en el mismo orden que los WheelColliders)

	[SerializeField]private bool isAutomatic = true;							

	[SerializeField]private int brakingForce;										//Fuerza de Frenado (en Newtons)
	
	int currentGear = 1;
	float throttle;
	float brake;
	float steering;
	int RPM;

	public float Throttle {
		get {
			return throttle;
		}
		set {
			throttle = value;
		}
	}

	public float Brake {
		get {
			return brake;
		}
		set {
			brake = value;
		}
	}

	public float Steering {
		get {
			return steering;
		}
		set {
			steering = value;
		}
	}

	void UpdateWheelMeshesPositions(){
		for (int i = 0; i < 4; i++) {
			Quaternion quat;
			Vector3 pos;
			
			wheelColliders[i].GetWorldPose(out pos, out quat);
			
			tireMeshes[i].position = pos;
			tireMeshes[i].rotation = quat;
			
		}
	}
	
	void Update(){
		UpdateWheelMeshesPositions ();
	}
	
	void FixedUpdate () {
		//Calculando el torque con la ayuda de las formulas obtenidas desde:
		//http://www.asawicki.info/Mirror/Car%20Physics%20for%20Games/Car%20Physics%20for%20Games.html
		
		float wheelRPM = (wheelColliders [0].rpm + wheelColliders [1].rpm) / 2;
		//Calcular RPM del motor cuando el embrague esta desactivado
		//if (clutch != 0)
		float engineRPM = wheelRPM * finalGearRatio * gearRatios [currentGear];

		//test
		if (engineRPM < minRPM)
			engineRPM = minRPM;

		//Calcular torque desde la curva.
		float totalMotorTorque = torqueCurve.Evaluate (engineRPM) * gearRatios [currentGear] * finalGearRatio * 0.7f * throttle;
		
		
		//pasar torque a las ruedas
		//Diferencial: TO DO, considerado en el desarrollo de la direccion
		wheelColliders[0].motorTorque = totalMotorTorque/2;
		wheelColliders[1].motorTorque = totalMotorTorque/2;

		
		// Cambios automaticos.
		if (isAutomatic)
		{
			if (engineRPM >= maxRPM)
				ShiftUp ();
			else if (engineRPM <= minRPM * 1.1f && currentGear > 2)
				ShiftDown ();
			if (throttle < 0 && engineRPM <= minRPM)
				currentGear = (currentGear == 0?2:0);
		}
		
		RPM = (int)engineRPM;

		//Aplicar Frenos
		if (brake > 0) {
			foreach (WheelCollider wheel in wheelColliders) {
				wheel.brakeTorque = brakingForce * brake / wheelColliders.Length;
			}
			Debug.Log (brake);
		} else {
			foreach (WheelCollider wheel in wheelColliders){
				wheel.brakeTorque = 0;
			}
		}
	}
	
	public void ShiftUp(){
		if (currentGear < gearRatios.Length - 1)
			currentGear++; 
	}
	public void ShiftDown(){
		if (currentGear > 0)
			currentGear--;
	}
	public void ShiftTo(int targetGear){
		if (targetGear >= 0 && targetGear <= gearRatios.Length)
			currentGear = targetGear;
	}
	public int GetCurrentGear(){
		return currentGear;
	}
	public int GetCurrentRPM(){
		return RPM;
	}
}