using UnityEngine;
using System.Collections;

//Este componente simula y maneja: Motor + Transmision + Ruedas + Diferencial + Eje de Transmision + Frenos = Powertrain,
//generando torque y aplicandoselo a las ruedas.

public class Powertrain : MonoBehaviour {
	[SerializeField]private AnimationCurve torqueCurve;                             //Curva de RPM/torque[Nm] del motor
	[SerializeField]private float[] gearRatios = new float[8];                      //Relacion de reduccion de los cambios (0 = neutro, 7=reversa)
	[SerializeField]private float finalGearRatio;                                   //Relacion de reduccion del diferencial
	[SerializeField]private int minRPM;                                             //RPM de ralenti del motor (usado en cambios automaticos)
	[SerializeField]private int maxRPM; 											//Redline

	[SerializeField]private WheelCollider[] wheelColliders = new WheelCollider[4];  //WheelColliders: Delantera, Delantera, Trasera, Trasera
	[SerializeField]private Transform[] tireMeshes = new Transform[4];              //Meshes de las ruedas (en el mismo orden que los WheelColliders)


	[SerializeField]private int brakingForce;										//Fuerza de Frenado (en Newtons)
	[SerializeField]private int engineInertia;										//Inercia del motor ocupada para acelerar en neutro.
	[SerializeField]private float transmissionEfficiency = 0.7f;					//Eficiencia del sistema de transmision


	int currentGear = 0;
	float throttle;
	float brake;
	float clutch;
	float steering;

	static Rigidbody rigidbody;
	static float engineRPM;

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

	public float Clutch {
		get {
			return clutch;
		}
		set {
			clutch = value;
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

	void Start(){
		rigidbody = GetComponent<Rigidbody> ();
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

	float CalculateWheelTorque(){
		//Calculando el torque con la ayuda de las formulas obtenidas desde:
		//http://www.asawicki.info/Mirror/Car%20Physics%20for%20Games/Car%20Physics%20for%20Games.html
		//http://forum.unity3d.com/threads/how-to-add-clutch-setting-in-truck-physics.65884/
		
		float wheelRPM = (wheelColliders [0].rpm + wheelColliders [1].rpm) / 2;
		float disengagedEngineRPM = wheelRPM * finalGearRatio * gearRatios [currentGear]; //if clutch == 0
		float engagedEngineRPM = engineInertia * throttle + engineRPM;                    //if clutch == 1 
		
		//RPM del motor dependiendo del estado del embrague
		engineRPM = (1 - clutch) * engagedEngineRPM + clutch * disengagedEngineRPM;

		//neutro == clutch = 1
		if (currentGear == 0) 
			engineRPM = engagedEngineRPM;

		if (currentGear == 0 || clutch == 0){ 
			if (throttle == 0)
				engineRPM = engineRPM - engineInertia * 0.3f;
			
			if (engineRPM < minRPM) 
				engineRPM = minRPM;
		}

		if (engineRPM > maxRPM) {
			if (throttle != 0) engineRPM = maxRPM; 
		}
		
		//Calcular torque desde la curva.
		float engineTorque = torqueCurve.Evaluate (engineRPM);
		float drive = throttle * (clutch + 1) / 2;
		float driveTorque =  drive * engineTorque * gearRatios [currentGear] * finalGearRatio * transmissionEfficiency;	

		return driveTorque;
	}

	void ApplyBrakes(){
		foreach (WheelCollider wheel in wheelColliders) {
			float brakeForce = brakingForce * brake;
			float engineBrake = (1 - throttle) * engineRPM * 0.74f * gearRatios [currentGear] * finalGearRatio * transmissionEfficiency;
			wheel.brakeTorque =  brakeForce * engineBrake;
		}
	}

	void Update(){
		UpdateWheelMeshesPositions ();
	}
	void FixedUpdate () {
		ApplyBrakes ();
		
		//pasar torque a las ruedas
		float driveTorque = CalculateWheelTorque ();
		
		//Diferencial: TO DO, considerado en el desarrollo de la direccion
		wheelColliders[0].motorTorque = driveTorque/2;
		wheelColliders[1].motorTorque = driveTorque/2;
		
		//aplicar rotacion
		wheelColliders [0].steerAngle = steering; 
		wheelColliders [1].steerAngle = steering; 
	}

	/*
	void FixedUpdate () {
		//Calculando el torque con la ayuda de las formulas obtenidas desde:
		//http://www.asawicki.info/Mirror/Car%20Physics%20for%20Games/Car%20Physics%20for%20Games.html
		//http://forum.unity3d.com/threads/how-to-add-clutch-setting-in-truck-physics.65884/

		float wheelRPM = (wheelColliders [0].rpm + wheelColliders [1].rpm) / 2;

		//RPM del motor cuando el embrague esta desactivado
		float disengagedEngineRPM = wheelRPM * finalGearRatio * gearRatios [currentGear];

		//RPM del motor cuando el embrague esta completamente activado == 1 (o en neutro)
		float engagedEngineRPM = engineInertia * throttle + engineRPM;


		//RPM del motor dependiendo del estado del embrague
		engineRPM = (1 - clutch) * engagedEngineRPM + clutch * disengagedEngineRPM;

		if (currentGear == 0) {
			engineRPM = engagedEngineRPM;
		}


		if (engineRPM < minRPM) {
			//mantener RPM de ralenti en los casos necesarios
			if (clutch == 0 || currentGear == 0)
				engineRPM = minRPM;
			else if (clutch < 0.9)
				;//AQUI SE PUEDE DETENER EL MOTOR
		} 
		else if (engineRPM > maxRPM)
			engineRPM = maxRPM;

		
		//Calcular torque desde la curva.
		float engineTorque = torqueCurve.Evaluate (engineRPM);
		print (engineRPM + ":" +engineTorque);
		float drive = ((1 - clutch) + throttle) / 2;
		float driveTorque =  clutch * engineTorque * gearRatios [currentGear] * finalGearRatio * transmissionEfficiency;
		
		
		//pasar torque a las ruedas
		//Diferencial: TO DO, considerado en el desarrollo de la direccion
		wheelColliders[0].motorTorque = driveTorque/2;
		wheelColliders[1].motorTorque = driveTorque/2;

		//aplicar rotacion
		wheelColliders [0].steerAngle = steering; 
		wheelColliders [1].steerAngle = steering; 

		// Cambios automaticos.
		if (isAutomatic) {
			if (engineRPM >= maxRPM)
				ShiftUp ();
			else if (engineRPM <= minRPM * 1.1f && currentGear > 2)
				ShiftDown ();
			if (throttle < 0 && engineRPM <= minRPM)
				currentGear = (currentGear == 0 ? 2 : 0);
		} 
		//Cosas cambios manuales
		if (!isAutomatic) {

		}

		//Aplicar Frenos
		if (brake > 0) {
			foreach (WheelCollider wheel in wheelColliders) {
				wheel.brakeTorque = brakingForce * brake / wheelColliders.Length;
			}
		} else {
			foreach (WheelCollider wheel in wheelColliders){
				wheel.brakeTorque = 0;
			}
		}
	}
	*/
	public void ShiftUp(){
		if (currentGear < gearRatios.Length - 1)
			currentGear++; 
	}
	public void ShiftDown(){
		if (currentGear > 0)
			currentGear--;
	}
	public void ShiftTo(int targetGear){
		if (clutch < 0.2) {
			if (targetGear >= 0 && targetGear <= gearRatios.Length)
				currentGear = targetGear;
		}
		else {
			currentGear = 0;
		}
	}
	public int GetCurrentGear(){
		return currentGear;
	}
	static public int GetCurrentRPM(){
		int rpm = new int();
		rpm = (int)engineRPM;
		return rpm;
	}
	static public int GetCurrentSpeed(){
		return Mathf.FloorToInt(rigidbody.velocity.magnitude * 3.6f);
	}
}