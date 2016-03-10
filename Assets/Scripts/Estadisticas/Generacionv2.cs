using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Text;

public class Generacionv2 : MonoBehaviour {
	/** Variables Publicas */

	public GameObject rectaPrefab;
	public GameObject crucePrefab;
	public GameObject carPrefab;

	/** Variables Privadas */

	private Map mapObject;
	private GameObject car;
	private float size;
	private int xActual, yActual;
	private int mapItem;
	private string actualMapItem;
	private string actualDirection;

	/** Funciones */

	void Start() {
		string mapa = requestHTTP("http://claseb.dribyte.cl/api/v1/mapa");
		mapObject = Map.CreateFromJson (mapa);

		size = 39;
		xActual = yActual = 1;
		mapItem = 0;
		actualMapItem = "Recta";
		actualDirection = "North";

		Instantiate (rectaPrefab, Vector3.zero, Quaternion.identity);
		Instantiate (rectaPrefab, new Vector3 (size, 0, 0), Quaternion.identity);
		car = Instantiate (carPrefab, new Vector3 (-16, .2f, -1.5f), Quaternion.Euler (0, 90, 0)) as GameObject;
	}

	void Update() {
		if (mapItem < mapObject.segmentos.Length) {
			if (actualMapItem == "Recta") {
				if (actualDirection == "North") {
					if (car.transform.localPosition.x > size * xActual) {
						xActual += 1;

					}
				}
			}
			Debug.Log (car.transform.localPosition);
		} else {
			Debug.Log ("Final del Camino");
		}
	}

	void InstantiateMapItem(Segmento segment) {
		
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