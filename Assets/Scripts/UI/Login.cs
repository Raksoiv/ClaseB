using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Login : MonoBehaviour {

    public void ComprobarDatos()
    {

        if (false)
        {
            Application.LoadLevel("ArteTest");
        }
        else
        {
            Text txt = gameObject.GetComponent<Text>();
            txt.text = "Rut incorrecto";
        }
    }
}
