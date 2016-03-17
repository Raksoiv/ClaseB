using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net;
using System.Text;
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
		// Statistics s = new Statistics ();
		NigaStatistics s = new NigaStatistics();

		// Student id
		//s.id = id;
		s.alumno_id = id;

		// Time inside Lane and outside Lane
		// s.timeOutLane = timeOutsideLane;
		// s.timeInLane = Time.timeSinceLevelLoad - timeOutsideLane;
		s.tiempoFueraCarril = timeOutsideLane;
		s.tiempoCarril = Time.timeSinceLevelLoad - timeOutsideLane;

		float acumVelocity = 0;

		// Velocity Information
		/*
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
		*/

		float[] arrayVel = (float[]) velocity.ToArray (typeof (float));
		float[] arrayTime = (float[]) velocityTime.ToArray (typeof (float));
		s.velocidad = new float[arrayVel.Length];
		s.tiempoVelocidad = new float[arrayTime.Length];
		for (int i = 0; i < arrayVel.Length; i++) {
			s.velocidad [i] = arrayVel [i];
			s.tiempoVelocidad [i] = arrayTime [i];
			acumVelocity += arrayVel [i];
			if (arrayVel [i] < minVelocity) {
				minVelocity = arrayVel [i];
			}
			if (arrayVel [i] > maxVelocity) {
				maxVelocity = arrayVel [i];
			}
		}

		//velocity Constants
		/*
		s.averageVelocity = acumVelocity / arrayVel.Length;
		s.maxVelocity = maxVelocity;
		s.minVelocity = minVelocity;
		*/
		s.velocidadMedia = acumVelocity / arrayVel.Length;
		s.velocidadMaxima = maxVelocity;
		s.velocidadMinima = minVelocity;

		//Gear Shifting Information
		/*
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
		*/
		int[] arrayGearVelocity = new int[7];
		int[] arrayGearRPM = new int[7];
		int[] arrayGearProm = new int[7];
		for (int i = 0; i < 7; i++) {
			arrayGearVelocity [i] = 0;
			arrayGearRPM [i] = 0;
			arrayGearProm [i] = 0;
		}
		int[] arrayIntVel = (int[])velocityGearUp.ToArray (typeof(int));
		int[] arrayRPM = (int[])RPMGearUp.ToArray (typeof(int));
		int[] arrayGear = (int[])gearUpObjective.ToArray (typeof(int));
		for (int i = 0; i < arrayIntVel.Length; i++) {
			arrayGearVelocity [arrayGear [i]] += arrayIntVel [i];
			arrayGearRPM [arrayGear [i]] += arrayRPM [i];
			arrayGearProm [arrayGear [i]] += 1;
		}

		s.cambiosVelocidad = new int[7];
		s.cambiosRpm = new int[7];
		for (int i = 0; i < 7; i++) {
			if (arrayGearProm [i] != 0) {
				s.cambiosVelocidad [i] = arrayGearVelocity [i] / arrayGearProm [i];
				s.cambiosRpm [i] = arrayGearRPM [i] / arrayGearProm [i];
			} else {
				s.cambiosVelocidad [i] = 0;
				s.cambiosRpm [i] = 0;
			}
		}

		string file = s.SaveToJSON ();
		string retorno = requestHTTP("http://claseb.dribyte.cl/api/v1/estadisticas", file);
		Debug.Log(retorno);
	}

	public static string requestHTTP(string pagina, string datos)
	{
		string responseFromServer = "";
		try
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(pagina);
			request.Method = "POST";
			byte[] byteArray = Encoding.UTF8.GetBytes(datos);
			request.Accept = "application/json";
			request.ContentType = "application/json";
			request.ContentLength = byteArray.Length;
			Stream dataStream = request.GetRequestStream();
			dataStream.Write(byteArray, 0, byteArray.Length);
			dataStream.Close();

			WebResponse response = request.GetResponse();
			//Debug.Log(((HttpWebResponse)response).StatusDescription);
			dataStream = response.GetResponseStream();
			StreamReader reader = new StreamReader(dataStream);
			responseFromServer = reader.ReadToEnd();
			//Debug.Log(responseFromServer);

			reader.Close();
			dataStream.Close();
			response.Close();
		}
		catch (Exception e)
		{
			responseFromServer = e.Message;
		}

		return responseFromServer;
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

[System.Serializable]
public class NigaStatistics {
	// Velocidad
	public float[] velocidad;
	public float[] tiempoVelocidad;
	public float velocidadMedia;
	public float velocidadMaxima;
	public float velocidadMinima;

	// Ruta
	public string ruta;

	// Cambios
	public int[] cambiosVelocidad;
	public int[] cambiosRpm;

	//Informacion adicional
	public string alumno_id;

	//Tiempo en el carril
	public float tiempoCarril;
	public float tiempoFueraCarril;

	public string SaveToJSON () {
		return JsonUtility.ToJson (this);
	}
}