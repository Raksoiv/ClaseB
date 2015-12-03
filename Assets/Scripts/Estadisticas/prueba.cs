using UnityEngine;
using System.Collections;

public class prueba : MonoBehaviour {

    bool retorno = false;

	// Use this for initialization
	void Start () {
        Debug.Log(API.startSesion("17921200-5"));
        API.salidaCarril(2);
    }
	
	// Update is called once per frame
	void Update () {
        int tiempo = (int)Time.realtimeSinceStartup;
        if (tiempo < 5)
        {
            API.utiLuces(true);
            API.registrarVelocidad(tiempo * 10);
            API.registrarCambio(50, 4000, 3);
            
        }
        if (tiempo >= 5 && !retorno)
        {
            retorno = API.finalizarSesion();
            Debug.Log(retorno);
        }
	}
}
