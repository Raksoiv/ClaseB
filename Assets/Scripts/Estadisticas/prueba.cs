using UnityEngine;
using System.Collections;

public class prueba : MonoBehaviour {

	// Use this for initialization
	void Start () {
        API.startSesion("17921200-0");
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
        if (tiempo == 5)
        {
            bool retorno = API.finalizarSesion();
            //Debug.Log(retorno);
        }
	}
}
