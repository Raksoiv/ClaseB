using UnityEngine;
using System.Collections;

//Este componente simula el motor del auto junto con su transmision (Drivetrain),
//generando torque y aplicandoselo a las ruedas.
public class Drivetrain : MonoBehaviour {
	public AnimationCurve torqueCurve;
	public float[] gearRatios = new float[7];
	public float finalGearRatio;
	public int minRPM;
	
	public WheelCollider[] wheelColliders = new WheelCollider[4];
	public Transform[] tireMeshes = new Transform[4];
	public bool[] poweredWheels = new bool[4]; //ruedas que tienen poder
	
	public bool isAutomatic = true;
	public int maxRPM; 

	public int brakingForce;
	
	int currentGear = 1;
	float throttle;
	float brake;
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
		for (int i = 0; i < 4; i++) {
			if(poweredWheels[i]){
				wheelColliders[i].motorTorque = totalMotorTorque/2;
			}
		}
		
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