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

	private GameObject[,] poolRecta;
	private GameObject[,] poolCruce;
	private int [,] index;

	private Map mapObject;
	private GameObject car;
	private float size;
	private int xActual, yActual;
	private int mapItem;
	private string actualDirection;
	private string actualMapItem;

	/** Funciones */

	void Start() {
		string mapa = requestHTTP("http://claseb.dribyte.cl/api/v1/mapa.json");
		//string mapa = "{\"segmentos\":[{\"type\":\"Recto\",\"senales\":[],\"direccion\":\"Adelante\"},{\"type\":\"Cruce\",\"senales\":[],\"direccion\":\"Izquierda\"},{\"type\":\"Recto\",\"senales\":[],\"direccion\":\"Adelante\"},{\"type\":\"Recto\",\"senales\":[],\"direccion\":\"Adelante\"},{\"type\":\"Cruce\",\"senales\":[],\"direccion\":\"Izquierda\"},{\"type\":\"Recto\",\"senales\":[],\"direccion\":\"Adelante\"},{\"type\":\"Recto\",\"senales\":[\"No Virar Izquierda\"],\"direccion\":\"Derecha\"},{\"type\":\"Cruce\",\"senales\":[],\"direccion\":\"Adelante\"},{\"type\":\"Recto\",\"senales\":[],\"direccion\":\"Adelante\"},{\"type\":\"Recto\",\"senales\":[],\"direccion\":\"Adelante\"}]}";
		mapObject = Map.CreateFromJson (mapa);

		//Creando el pool de partes disponibles para el mapa
		//Generando las pool
		poolRecta = new GameObject [4, 10];
		poolCruce = new GameObject [4, 5];
		index = new int[2, 4];
		//Rectas
		for (int j = 0; j < 4; j++) {
			for (int i = 0; i < 10; i++) {
				poolRecta [j, i] = Instantiate (rectaPrefab [j], Vector3.zero, Quaternion.identity) as GameObject;
				poolRecta [j, i].SetActive (false);
			}
			index [0, j] = 0;
		}

		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 5; j++) {
				poolCruce [i, j] = Instantiate (crucePrefab [i], Vector3.zero, Quaternion.identity) as GameObject;
				poolCruce [i, j].SetActive (false);
			}
			index [1, i] = 0;
		}

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
				
				poolRecta [0, index [0, 0]].transform.localPosition = new Vector3 ((xActual + 1) * size, 0, yActual * size);
				poolRecta [0, index [0, 0]].SetActive (true);
				poolRecta [1, index [0, 1]].transform.localPosition = new Vector3 (xActual * size, 0, (yActual + 1) * size);
				poolRecta [1, index [0, 1]].SetActive (true);
				poolRecta [3, index [0, 3]].transform.localPosition = new Vector3 (xActual * size, 0, (yActual - 1) * size);
				poolRecta [3, index [0, 3]].SetActive (true);
				AumentIndex (0, 0);
				AumentIndex (0, 1);
				AumentIndex (0, 3);
				//Instantiate(rectaPrefab[0], new Vector3 ((xActual + 1) * size, 0, yActual * size), Quaternion.identity);
				//Instantiate(rectaPrefab[1], new Vector3 (xActual * size, 0, (yActual + 1) * size), Quaternion.identity);
				//Instantiate(rectaPrefab[3], new Vector3 (xActual * size, 0, (yActual - 1) * size), Quaternion.identity);
			} else if (actualDirection == "East") {
				poolRecta [0, index [0, 0]].transform.localPosition = new Vector3 ((xActual + 1) * size, 0, yActual * size);
				poolRecta [0, index [0, 0]].SetActive (true);
				poolRecta [1, index [0, 1]].transform.localPosition = new Vector3 (xActual * size, 0, (yActual + 1) * size);
				poolRecta [1, index [0, 1]].SetActive (true);
				poolRecta [2, index [0, 2]].transform.localPosition = new Vector3 ((xActual - 1) * size, 0, yActual * size);
				poolRecta [2, index [0, 2]].SetActive (true);
				AumentIndex (0, 0);
				AumentIndex (0, 1);
				AumentIndex (0, 2);
				//Instantiate(rectaPrefab[0], new Vector3 ((xActual + 1) * size, 0, yActual * size), Quaternion.identity);
				//Instantiate(rectaPrefab[1], new Vector3 (xActual * size, 0, (yActual + 1) * size), Quaternion.identity);
				//Instantiate(rectaPrefab[2], new Vector3 ((xActual - 1) * size, 0, yActual * size), Quaternion.identity);
			} else if (actualDirection == "South") {
				poolRecta [1, index [0, 1]].transform.localPosition = new Vector3 (xActual * size, 0, (yActual + 1) * size);
				poolRecta [1, index [0, 1]].SetActive (true);
				poolRecta [2, index [0, 2]].transform.localPosition = new Vector3 ((xActual - 1) * size, 0, yActual * size);
				poolRecta [2, index [0, 2]].SetActive (true);
				poolRecta [3, index [0, 3]].transform.localPosition = new Vector3 (xActual * size, 0, (yActual - 1) * size);
				poolRecta [3, index [0, 3]].SetActive (true);
				AumentIndex (0, 1);
				AumentIndex (0, 2);
				AumentIndex (0, 3);
				//Instantiate(rectaPrefab[1], new Vector3 (xActual * size, 0, (yActual + 1) * size), Quaternion.identity);
				//Instantiate(rectaPrefab[2], new Vector3 ((xActual - 1) * size, 0, yActual * size), Quaternion.identity);
				//Instantiate(rectaPrefab[3], new Vector3 (xActual * size, 0, (yActual - 1) * size), Quaternion.identity);
			} else if (actualDirection == "West") {
				poolRecta [0, index [0, 0]].transform.localPosition = new Vector3 ((xActual + 1) * size, 0, yActual * size);
				poolRecta [0, index [0, 0]].SetActive (true);
				poolRecta [2, index [0, 2]].transform.localPosition = new Vector3 ((xActual - 1) * size, 0, yActual * size);
				poolRecta [2, index [0, 2]].SetActive (true);
				poolRecta [3, index [0, 3]].transform.localPosition = new Vector3 (xActual * size, 0, (yActual - 1) * size);
				poolRecta [3, index [0, 3]].SetActive (true);
				AumentIndex (0, 0);
				AumentIndex (0, 2);
				AumentIndex (0, 3);
				//Instantiate(rectaPrefab[0], new Vector3 ((xActual + 1) * size, 0, yActual * size), Quaternion.identity);
				//Instantiate(rectaPrefab[2], new Vector3 ((xActual - 1) * size, 0, yActual * size), Quaternion.identity);
				//Instantiate(rectaPrefab[3], new Vector3 (xActual * size, 0, (yActual - 1) * size), Quaternion.identity);
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
		Debug.Log ("Instantiate index recta: " + index [0, typeNumber].ToString ());
		if (type == "Recto") {
			poolRecta [typeNumber, index [0, typeNumber]].transform.localPosition = new Vector3 (x, 0, y);
			poolRecta [typeNumber, index [0, typeNumber]].SetActive (true);
			AumentIndex (0, typeNumber);
			//Instantiate (rectaPrefab[typeNumber], new Vector3 (x, 0, y), Quaternion.identity);
		} else if (type == "Cruce") {
			poolCruce [typeNumber, index [1, typeNumber]].transform.localPosition = new Vector3 (x, 0, y);
			poolCruce [typeNumber, index [1, typeNumber]].SetActive (true);
			AumentIndex (1, typeNumber);
			//Instantiate (crucePrefab[typeNumber], new Vector3 (x, 0, y), Quaternion.identity);
		}

		actualMapItem = type;
		return;
	}

	void AumentIndex(int i, int j) {
		if (i == 0) {
			index [i, j] += 1;
			if (index [i, j] >= 10) {
				index [i, j] = 0;
			}
		} else if (i == 1) {
			index [i, j] += 1;
			if (index [i, j] >= 5) {
				index [i, j] = 0;
			}
		}
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