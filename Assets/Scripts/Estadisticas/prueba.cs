using UnityEngine;
using System.Collections;

public class prueba : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //API.startSesion("17921200-5");
        //API.crearDocumento();

        string retorno = "";
        /*retorno = API.requestHTTP("http://insive.cl/temp/prueba.php", "perro=bongito3");
        Debug.Log(retorno);

        retorno = API.requestHTTP("http://insive.cl/temp/prueba2.php", "");
        Debug.Log(retorno);*/

        retorno = API.filesHTTP("http://insive.cl/temp/prueba5.php", "Captura.PNG");
        Debug.Log(retorno);
	}
	
	// Update is called once per frame
	void Update () {
        
	}
}
