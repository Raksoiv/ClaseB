using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	// Car Prefab
	public GameObject car;

	// Street Prefabs
	public GameObject[] streets;
	public float[] streetsOffset;

	// Quantity of Instantate Prefabs
	public ArrayList prefabs;

	// Private Variables

	// Mapa Importado
	Mapa map;

	//Offset de Generacion
	float offsetX;
	float offsetZ;

	// Use this for initialization
	void Start () {
		// Get the JSON File and Instantiate 
		map = Mapa.CreateFromJSON ("{\"segmentos\":[{\"type\":\"Recto\",\"signs\":[]},{\"type\":\"Recto\",\"signs\":[]},{\"type\":\"Cruce\",\"signs\":[\"Pare\"]},{\"type\":\"Recto\",\"signs\":[]},{\"type\":\"Recto\",\"signs\":[]},{\"type\":\"Cruce\",\"signs\":[\"Pare\"]}]}");
		Instantiate (car, new Vector3 (0, 0, -1.5f), Quaternion.Euler (0, 90, 0));
		prefabs.Add (Instantiate (streets[0], Vector3.zero, Quaternion.identity));

		//Set initial Offset
		offsetX = 20;
		offsetZ = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

[System.Serializable]
public class Segmento{
	public string type;
	public string[] signs;

	public string ToJSON () {
		return JsonUtility.ToJson (this);
	}
}

[System.Serializable]
public class Mapa {
	public Segmento[] segmentos;

	public string ToJSON () {
		return JsonUtility.ToJson (this);
	}

	public static Mapa CreateFromJSON (string JSONString) {
		return JsonUtility.FromJson<Mapa> (JSONString);
	}
}