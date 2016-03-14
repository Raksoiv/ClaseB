using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Text;

public class Generacionv2 : MonoBehaviour {
	/** Variables Publicas */

	public GameObject[] rectaPrefab;
	public GameObject[] crucePrefab;
	public GameObject carPrefab;

	/** Variables Privadas */

	private Map mapObject;
	private GameObject car;
	private float size;
	private int xActual, yActual;
	private int mapItem;
	private string actualDirection;
	private string actualMapItem;

	/** Funciones */

	void Start() {
		string mapa = requestHTTP("http://claseb.dribyte.cl/api/v1/mapa");
		//string mapa = '{"segmentos":[{"type":"Recto","senales":[],"direccion":"Adelante"},{"type":"Cruce","senales":[],"direccion":"Izquierda"},{"type":"Recto","senales":[],"direccion":"Adelante"},{"type":"Recto","senales":[],"direccion":"Adelante"},{"type":"Cruce","senales":[],"direccion":"Izquierda"},{"type":"Recto","senales":[],"direccion":"Adelante"},{"type":"Recto","senales":["No Virar Izquierda"],"direccion":"Derecha"},{"type":"Cruce","senales":[],"direccion":"Adelante"},{"type":"Recto","senales":[],"direccion":"Adelante"},{"type":"Recto","senales":[],"direccion":"Adelante"}]}';
		mapObject = Map.CreateFromJson (mapa);

		size = 39;
		xActual = yActual = 0;
		mapItem = -1;
		actualDirection = "North";
		actualMapItem = "Recto";

		Instantiate (rectaPrefab[0], Vector3.zero, Quaternion.identity);
		Instantiate (rectaPrefab[0], new Vector3 (size, 0, 0), Quaternion.identity);
		car = Instantiate (carPrefab, new Vector3 (-16, .2f, -1.5f), Quaternion.Euler (0, 90, 0)) as GameObject;
	}

	void Update() {
		if (car.transform.localPosition.x > (size * xActual) + (size / 2)) {
			Debug.Log ("North");
			if (actualDirection == "North") {
				mapItem += 1;
			}
			xActual += 1;
			actualDirection = "North";
			InstantiateMapItem ();
		} else if (car.transform.localPosition.x < (size * xActual) - (size / 2)) {
			Debug.Log ("South");
			if (actualDirection == "South") {
				mapItem += 1;
			}
			xActual -= 1;
			actualDirection = "South";
			InstantiateMapItem ();
		} else if (car.transform.localPosition.z > (size * yActual) + (size / 2)) {
			Debug.Log ("East");
			if (actualDirection == "East") {
				mapItem += 1;
			}
			yActual += 1;
			actualDirection = "East";
			InstantiateMapItem ();
		} else if (car.transform.localPosition.z < (size * yActual) - (size / 2)) {
			Debug.Log ("West");
			if (actualDirection == "West") {
				mapItem += 1;
			}
			yActual -= 1;
			actualDirection = "West";
			InstantiateMapItem ();
		}
	}

	void InstantiateMapItem() {
		//Type of Map Item
		string type;
		if (mapItem == -1) {
			type = "Recto";
		} else if (mapItem < mapObject.segmentos.Length) {
			type = mapObject.segmentos [mapItem].type;
		} else {
			//Transicion fin de la simulacion
			Debug.Log ("Final del mapa");
			return;
		}

		//If Cruce Instaitate all posibilities
		if (actualMapItem == "Cruce") {
			if (actualDirection == "North") {
				Instantiate(rectaPrefab[0], new Vector3 ((xActual + 1) * size, 0, yActual * size), Quaternion.identity);
				Instantiate(rectaPrefab[1], new Vector3 (xActual * size, 0, (yActual + 1) * size), Quaternion.identity);
				Instantiate(rectaPrefab[3], new Vector3 (xActual * size, 0, (yActual - 1) * size), Quaternion.identity);
			} else if (actualDirection == "East") {
				Instantiate(rectaPrefab[0], new Vector3 ((xActual + 1) * size, 0, yActual * size), Quaternion.identity);
				Instantiate(rectaPrefab[1], new Vector3 (xActual * size, 0, (yActual + 1) * size), Quaternion.identity);
				Instantiate(rectaPrefab[2], new Vector3 ((xActual - 1) * size, 0, yActual * size), Quaternion.identity);
			} else if (actualDirection == "South") {
				Instantiate(rectaPrefab[1], new Vector3 (xActual * size, 0, (yActual + 1) * size), Quaternion.identity);
				Instantiate(rectaPrefab[2], new Vector3 ((xActual - 1) * size, 0, yActual * size), Quaternion.identity);
				Instantiate(rectaPrefab[3], new Vector3 (xActual * size, 0, (yActual - 1) * size), Quaternion.identity);
			} else if (actualDirection == "West") {
				Instantiate(rectaPrefab[0], new Vector3 ((xActual + 1) * size, 0, yActual * size), Quaternion.identity);
				Instantiate(rectaPrefab[2], new Vector3 ((xActual - 1) * size, 0, yActual * size), Quaternion.identity);
				Instantiate(rectaPrefab[3], new Vector3 (xActual * size, 0, (yActual - 1) * size), Quaternion.identity);
			}
			actualMapItem = type;
			return;
		}

		//Position and Rotation of Map Item
		float x, y;
		int typeNumber;
		x = y = typeNumber = 0;
		if (actualDirection == "North") {
			x = (xActual + 1) * size;
			y = yActual * size;
			typeNumber = 0;
		} else if (actualDirection == "East") {
			x = xActual * size;
			y = (yActual + 1) * size;
			typeNumber = 1;
		} else if (actualDirection == "South") {
			x = (xActual - 1) * size;
			y = yActual * size;
			typeNumber = 2;
		} else if (actualDirection == "West") {
			x = xActual * size;
			y = (yActual - 1) * size;
			typeNumber = 3;
		}

		//Instantiate
		Debug.Log ("Instantiate position: " + x.ToString () + ", 0, " + y.ToString ());
		Debug.Log ("Instantiate type: " + type.ToString ());
		Debug.Log ("Instantiate typeNumber: " + typeNumber	.ToString ());
		if (type == "Recto") {
			Instantiate (rectaPrefab[typeNumber], new Vector3 (x, 0, y), Quaternion.identity);
		} else if (type == "Cruce") {
			Instantiate (crucePrefab[typeNumber], new Vector3 (x, 0, y), Quaternion.identity);
		}

		actualMapItem = type;
		return;
	}

	string requestHTTP(string pagina) {
		string responseFromServer = "";
		try {
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(pagina);

			WebResponse response = request.GetResponse();
			Stream dataStream = response.GetResponseStream();
			StreamReader reader = new StreamReader(dataStream);
			responseFromServer = reader.ReadToEnd();

			reader.Close();
			dataStream.Close();
			response.Close();
		}
		catch (Exception e) {
			responseFromServer = e.Message;
		}
		return responseFromServer;
	}
}

[System.Serializable]
public class Segmentos {
	public string type;
	public string[] senales;
	public string direccion;
}

[System.Serializable]
public class Map {
	public Segmentos[] segmentos;

	public static Map CreateFromJson(string jsonString) {
		return JsonUtility.FromJson<Map> (jsonString);
	}
}