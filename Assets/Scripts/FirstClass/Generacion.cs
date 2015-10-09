using System;
using System.IO;
using System.Collections;
using UnityEngine;


/*
 * Acciones sobre la generacion de caminos:
 * 
 *  1) Camino Recto
 *  2) Virar hacia la Derecha
 *  3) Virar hacia la Izquierda
 *  4) Curva hacia la Derecha
 *  5) Curva hacia la Izquierda
 *  
 * LLenar con estos numeros el siguiente arreglo:
 * 
 * generacion = new int[]{ ,  ,  ,  ,  ,  ,  , };
 *
 */

public class Generacion : MonoBehaviour {

    public GameObject Cruce;
    public GameObject Curva1;
    public GameObject Curva2;
    public GameObject Curva3;
    public GameObject Curva4;
    public GameObject rectaV;
    public GameObject rectaH;
    public string ArchivoConfiguracion;

    private int[] generacion;
    private int[] dirContrarias;
    private int[] curvosIzquierdo;
    private int[] curvosDerecho;
    private int[,] terrenos;
    private GameObject[] lectura;

    private int posicionX;
    private int posicionZ;
    private int contador;
    private int direccionAnterior;
    private int direccionActual;
    private int terrenoAnterior;
    private int terrenoActual;
    private int posicionBoton;
    private bool generacionTerminada;
    private bool cambioTerreno;

	// Use this for initialization
	void Start () {
        direccionActual = 0;
        direccionAnterior = 0;
        terrenoActual = 5;
        terrenoAnterior = 5;
        posicionX = 0;
        posicionZ = 0;
        contador = 0;
        generacionTerminada = false;
        cambioTerreno = false;

        lectura = new GameObject[10];
        terrenos = new int[7, 4] { { 1, 1, 1, 1 }, { 0, 1, 0, 1 }, { 0, 1, 1, 0 }, { 1, 0, 0, 1 }, { 1, 0, 1, 0 }, { 1, 1, 0, 0 }, { 0, 0, 1, 1 } };
        dirContrarias = new int[4] { 1, 0, 3, 2 };
        curvosIzquierdo = new int[4] { 3, 2, 0, 1 };
        curvosDerecho = new int[4] { 2, 3, 1, 0 };

        //Arreglo de acciones para generacion de Caminos
        LeerArchivoConfiguracion(ArchivoConfiguracion);
	}

    void OnTriggerEnter(Collider other)
    {
        cambioTerreno = false;
        if (direccionActual != 1 && other.transform.position.z >= posicionZ * 500 + 400)
        {
            direccionActual = 0;
            cambioTerreno = true;
        }
        else if (direccionActual != 2 && other.transform.position.x >= posicionX * 500 + 400)
        {
            direccionActual = 3;
            cambioTerreno = true;
        }
        else if (direccionActual != 0 && other.transform.position.z <= posicionZ * 500 + 100)
        {
            direccionActual = 1;
            cambioTerreno = true;
        }
        else if (direccionActual != 3 && other.transform.position.x <= posicionX * 500 + 100)
        {
            direccionActual = 2;
            cambioTerreno = true;
        }
        else
        {
            cambioTerreno = false;
        }
    }
	
	// Update is called once per frame
	void Update () {
        /*
        bool cambioTerreno = false;
        
        //Determina la direccion a la cual se dirige el automovil
        if (direccionActual != 1 && transform.position.z >= posicionZ * 500 + 400){
            direccionActual = 0;
            cambioTerreno = true;
        }
        else if (direccionActual != 2 && transform.position.x >= posicionX * 500 + 400){
            direccionActual = 3;
            cambioTerreno = true;
        }
        else if (direccionActual != 0 && transform.position.z <= posicionZ * 500 + 100){
            direccionActual = 1;
            cambioTerreno = true;
        }
        else if (direccionActual != 3 && transform.position.x <= posicionX * 500 + 100){
            direccionActual = 2;
            cambioTerreno = true;
        }
        else{
            cambioTerreno = false;
        }
        */

        if (cambioTerreno && !generacionTerminada)
        {
			int direccionSubAnt = direccionAnterior;
			direccionAnterior = direccionActual;
            terrenoAnterior = terrenoActual;

            //Toma una desicion dependiendo de la accion de conducción deseada (recto, virar, curva)
            switch (generacion[contador])
            {
                case 1:
                    if (direccionActual == 0 || direccionActual == 1)
                    {
                        terrenoActual = 5;
                    }
                    if (direccionActual == 2 || direccionActual == 3)
                    {
                        terrenoActual = 6;
                    }
                    break;
                case 2:
                    terrenoActual = 0;
                    break;
                case 3:
                    terrenoActual = 0;
                    break;
                case 4:
                    for (int i = 1; i <= 4; i++)
                        if (terrenos[i, dirContrarias[direccionActual]] == 1 && terrenos[i, curvosDerecho[dirContrarias[direccionActual]]] == 1)
                        {
                            terrenoActual = i;
                            break;
                        }
                    break;
                case 5:
                    for (int i = 1; i <= 4; i++)
                        if (terrenos[i, dirContrarias[direccionActual]] == 1 && terrenos[i, curvosIzquierdo[dirContrarias[direccionActual]]] == 1)
                        {
                            terrenoActual = i;
                            break;
                        }
                    break;
                default:
                    print("Error: Accion no identificada en la lista Generacion");
                    break;
            }

            //Determina la posicion donde se instanciara el nuevo terreno
            switch (direccionActual)
            {
                case 0:
                    posicionZ += 1;
                    break;
                case 1:
                    posicionZ -= 1;
                    break;
                case 2:
                    posicionX -= 1;
                    break;
                case 3:
                    posicionX += 1;
                    break;
                default:
                    print("Error: Direccion no identificada");
                    break;
            }

            //Si en un cruce no se dobla donde debe volvera a aparecer otro cruce
            if (terrenoAnterior == 0)
            {
				if (generacion[contador - 1] == 2 && curvosDerecho[dirContrarias[direccionSubAnt]] != direccionActual){
                    terrenoActual = terrenoAnterior;
                    contador -= 1;
                }

				else if (generacion[contador - 1] == 3 && curvosIzquierdo[direccionSubAnt] != direccionActual)
                {
                    terrenoActual = terrenoAnterior;
                    contador -= 1;
                }
            }

            //Genera el terreno instanciando el Prefab adecuado
            switch (terrenoActual)
            {
                case 1:
                    lectura[contador] = Instantiate(Curva1, new Vector3(posicionX * 500, 0, posicionZ * 500), Quaternion.identity) as GameObject;
                    print("Generacion: Curva1");
                    break;
                case 2:
                    lectura[contador] = Instantiate(Curva2, new Vector3(posicionX * 500, 0, posicionZ * 500), Quaternion.identity) as GameObject;
                    print("Generacion: Curva2");
                    break;
                case 3:
                    lectura[contador] = Instantiate(Curva3, new Vector3(posicionX * 500, 0, posicionZ * 500), Quaternion.identity) as GameObject;
                    print("Generacion: Curva3");
                    break;
                case 4:
                    lectura[contador] = Instantiate(Curva4, new Vector3(posicionX * 500, 0, posicionZ * 500), Quaternion.identity) as GameObject;
                    print("Generacion: Curva4");
                    break;
                case 5:
                    lectura[contador] = Instantiate(rectaV, new Vector3(posicionX * 500, 0, posicionZ * 500), Quaternion.identity) as GameObject;
                    print("Generacion: rectaV");
                    break;
                case 6:
                    lectura[contador] = Instantiate(rectaH, new Vector3(posicionX * 500, 0, posicionZ * 500), Quaternion.identity) as GameObject;
                    print("Generacion: rectaH");
                    break;
                case 0:
                    lectura[contador] = Instantiate(Cruce, new Vector3(posicionX * 500, 0, posicionZ * 500), Quaternion.identity) as GameObject;
                    print("Generacion: Cruce");
                    break;
                default:
                    print("Error: Terreno no identificado; no se puede cargar ningun Prefab");
                    break;
            }

            //Compruba y reinicia la lectura de la generacion de terreno
            if (contador + 1 >= generacion.Length)
            {
                contador = 0;
                generacionTerminada = true;
                print("============= CONTADOR RESETEADO ==============");
            }
            else
                contador++;

            cambioTerreno = false;
        }

	}


    void LeerArchivoConfiguracion(string nameFile)
    {
        string line;
        ArrayList datos = new ArrayList();

        System.IO.StreamReader file = new System.IO.StreamReader(nameFile);
        while ((line = file.ReadLine()) != null)
            datos.Add(line);

        generacion = new int[datos.Count];
        int contador = 0;
        foreach(string item in datos) {
            generacion[contador] = Int32.Parse(item);
            contador++;
        }
        file.Close();
    }
}
