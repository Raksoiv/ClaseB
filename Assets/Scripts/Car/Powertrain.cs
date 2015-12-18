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
	[SerializeField]private float Cmotor = .3f;														//Tasa de perdida de RPM del motor

	private float driveTorque;


	int currentGear = 0;
	float throttle;
	float brake;
	float clutch;
	float steering;

	//static Rigidbody rigidbody;
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

	public int GetRPMS(int select){
		switch (select) {
			case 0:
				return minRPM;
			case 1:
				return (int)engineRPM;
			case 2:
				return maxRPM;
		}
		return 0;
	}

	void Start(){
		//rigidbody = GetComponent<Rigidbody> ();
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

	private float CalculateWheelTorque(){
		//Calculando el torque con la ayuda de las formulas obtenidas desde:
		//http://www.asawicki.info/Mirror/Car%20Physics%20for%20Games/Car%20Physics%20for%20Games.html
		//http://forum.unity3d.com/threads/how-to-add-clutch-setting-in-truck-physics.65884/
		//Torque curve and Gear Shifts http://www.automobile-catalog.com/car/2014/1978790/hyundai_i10_1_2.html
		
		//float wheelRPM = (wheelColliders [0].rpm + wheelColliders [1].rpm) / 2;
		float wheelRPM = (GetComponent<Rigidbody>().velocity.magnitude / .33f) * (60 / (2 * Mathf.PI));
		//Desengaged Engine
		float disengagedEngineRPM;
		if(throttle > 0){
			disengagedEngineRPM = (maxRPM - 1000) * throttle + 1000;
		}
		else{
			if(engineRPM > minRPM){
				disengagedEngineRPM = engineRPM - engineInertia * Cmotor;
			}
			else{
				disengagedEngineRPM = minRPM;
			}
		}

		//Engaged Engine
		float engagedEngineRPM;
		engagedEngineRPM = wheelRPM * gearRatios[currentGear] * finalGearRatio;
		
		//RPM del motor dependiendo del estado del embrague
		engineRPM = disengagedEngineRPM * clutch + engagedEngineRPM * (1 - clutch);
		if(engineRPM > maxRPM){
			engineRPM = maxRPM;
		}
		if(currentGear == 0){
			engineRPM = disengagedEngineRPM;
		}
		
		//Calcular torque desde la curva.
		float engineTorque = torqueCurve.Evaluate (engineRPM) * throttle;
		float driveTorque =  engineTorque * gearRatios[currentGear] * finalGearRatio * (1 - clutch) * transmissionEfficiency;
		if(currentGear == 7){
			driveTorque *= -1;
		}

		return driveTorque;
	}

	private void ApplyBrakes(){
		foreach (WheelCollider wheel in wheelColliders) {
			float brakeForce = brakingForce * brake;
			wheel.brakeTorque =  brakeForce;
		}
	}

	void Update(){
		UpdateWheelMeshesPositions ();
		driveTorque = CalculateWheelTorque ();
		if(engineRPM >= maxRPM - 10){
			driveTorque = 0;
		}
		if(engineRPM < minRPM - 200){
			ShiftTo(0);
		}
	}

	void FixedUpdate () {
		ApplyBrakes ();
		
		//Diferencial: TO DO, considerado en el desarrollo de la direccion
		wheelColliders[0].motorTorque = driveTorque/2;
		wheelColliders[1].motorTorque = driveTorque/2;
		
		//aplicar rotacion
		wheelColliders [0].steerAngle = steering; 
		wheelColliders [1].steerAngle = steering; 
	}

	public void ShiftUp(){
		if (currentGear < gearRatios.Length - 1)
			currentGear++; 
	}
	public void ShiftDown(){
		if (currentGear > 0)
			currentGear--;
	}
	public bool ShiftTo(int targetGear){
		if (clutch > 0.8) {
			if (targetGear >= 0 && targetGear <= gearRatios.Length && currentGear != targetGear){
				currentGear = targetGear;
				return true;
			}
		}
		else if(currentGear != targetGear) {
			currentGear = 0;
		}
		return false;
	}
	public int GetCurrentGear(){
		return currentGear;
	}
	static public int GetCurrentRPM(){
		int rpm = new int();
		rpm = (int)engineRPM;
		return rpm;
	}
	public int GetCurrentSpeed(){
		return Mathf.FloorToInt(this.GetComponent<Rigidbody>().velocity.magnitude * 3.6f);
	}
}