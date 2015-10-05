using UnityEngine;
using System.Collections;

public class PrimeraClase : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        //print(Application.loadedLevelName);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown()
    {
        Application.LoadLevel("FirstClass");
    }

}
