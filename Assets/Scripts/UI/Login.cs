using UnityEngine;
using UnityEngine.UI;
using System;


public class Login : MonoBehaviour {

    public GameObject input;

    public void ComprobarDatos()
    {
        string rut = input.GetComponent<Text>().text;
        if (ComprobarRut(rut))
        {
            string retorno = API.requestHTTP("http://claseb.dribyte.cl/api/v1/alumnos", "{\"rut\": \"" + rut + "\"}");
            int primera = retorno.IndexOf(',');
            string id = retorno.Substring(6, primera - 6);

            Debug.Log(API.startSesion(id));
            Application.LoadLevel("Arte");
        }
        else
        {
            Text txt = gameObject.GetComponent<Text>();
            txt.text = "Rut incorrecto";
        }
    }

    public bool ComprobarRut(string rut)
    {
        if(rut.Length == 10)
        {
            if(rut.Substring(8, 1).Equals("-"))
            {
                int num = 0;
                bool result = int.TryParse(rut.Substring(0, 8), out num);
                if (result)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }
        else if(rut.Length == 9)
        {
            if (rut.Substring(7, 1).Equals("-"))
            {
                int num = 0;
                bool result = int.TryParse(rut.Substring(0, 7), out num);
                if (result)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}
