using UnityEngine;
using System.Collections;

public static class APIv2 {

	// Private Variables
	private static bool loggedIn;
	private static string id;

	// Lists
	private static ArrayList velocity;
	private static ArrayList velocityTime;
	private static ArrayList velocityGearUp;
	private static ArrayList RPMGearUp;
	private static ArrayList gearUpObjective;

	// Variables
	private static float maxVelocity;
	private static float minVelocity;
	private static float timeOutsideLane;

	public static void InitApi () {
		loggedIn = false;
		id = "";
		velocity = new ArrayList ();
		velocityTime = new ArrayList ();
		velocityGearUp = new ArrayList ();
		RPMGearUp = new ArrayList ();
		gearUpObjective = new ArrayList ();
		maxVelocity = 0;
		minVelocity = 999;
		timeOutsideLane = 0;
		return;
	}

	public static void StartSession (string httpId) {
		if (!loggedIn) {
			loggedIn = true;
			id = httpId;
			return;
		}
	}

	public static void RegisterOutsideLane (float timeOutside) {
		if (loggedIn) {
			timeOutsideLane += timeOutside;
			return;
		}
	}

	public static void RegisterVelocity (float vel) {
		if (loggedIn) {
			velocity.Add (vel);
			velocityTime.Add (Time.timeSinceLevelLoad);
			return;
		}
	}

	public static void RegisterGearUp (float vel, int RPM, int gear) {
		if (loggedIn) {
			int intVel = Mathf.RoundToInt (vel);
			velocityGearUp.Add (intVel);
			RPMGearUp.Add (RPM);
			gearUpObjective.Add (gear);
		}
	}

	public static void EndSession () {
		Statistics s = new Statistics ();

		// Student id
		s.id = id;

		// Time inside Lane and outside Lane
		s.timeOutLane = timeOutsideLane;
		s.timeInLane = Time.timeSinceLevelLoad - timeOutsideLane;

		float acumVelocity = 0;

		// Velocity Information
		float[] arrayVel = (float[]) velocity.ToArray (typeof (float));
		float[] arrayTime = (float[]) velocityTime.ToArray (typeof (float));
		s.velocityStatistics = new Velocity[arrayVel.Length];
		for (int i = 0; i < arrayVel.Length; i++) {
			s.velocityStatistics [i] = new Velocity ();
			s.velocityStatistics [i].velocity = arrayVel [i];
			s.velocityStatistics [i].time = arrayTime [i];
			acumVelocity += arrayVel [i];
			if (arrayVel [i] < minVelocity) {
				minVelocity = arrayVel [i];
			}
			if (arrayVel [i] > maxVelocity) {
				maxVelocity = arrayVel [i];
			}
		}

		//velocity Constants
		s.averageVelocity = acumVelocity / arrayVel.Length;
		s.maxVelocity = maxVelocity;
		s.minVelocity = minVelocity;

		//Gear Shifting Information
		int[] arrayIntVel = (int[])velocityGearUp.ToArray (typeof(int));
		int[] arrayRPM = (int[])RPMGearUp.ToArray (typeof(int));
		int[] arrayGear = (int[])gearUpObjective.ToArray (typeof(int));
		s.gearStatistics = new GearUp[arrayIntVel.Length];
		for (int i = 0; i < arrayIntVel.Length; i++) {
			s.gearStatistics [i] = new GearUp ();
			s.gearStatistics [i].velocity = arrayIntVel [i];
			s.gearStatistics [i].RPM = arrayRPM [i];
			s.gearStatistics [i].gear = arrayGear [i];
		}

		Debug.Log (s.SaveToJSON ());
	}
}

[System.Serializable]
public class GearUp {
	public int velocity;
	public int RPM;
	public int gear;
}

[System.Serializable]
public class Velocity {
	public float velocity;
	public float time;
}

[System.Serializable]
public class Statistics {
	public GearUp[] gearStatistics;
	public Velocity[] velocityStatistics;
	public string id;
	public float timeInLane;
	public float timeOutLane;
	public float averageVelocity;
	public float maxVelocity;
	public float minVelocity;

	public string SaveToJSON () {
		return JsonUtility.ToJson (this);
	}
}