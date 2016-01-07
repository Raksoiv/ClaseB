using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Tipos de Caminos:
 * 
 * 	0) Cruce Completo
 *  1) Recto Vertical
 *  2) Recto Horizontal
 *  3) Curvo 1
 *  4) Curvo 2
 *  5) Curvo 3
 *  6) Curvo 4
 *  7) Cruce T 1
 *  8) Cruce T 2
 * 	9) Cruce T 3
 * 10) Cruce T 4
 *  
 */

public class terrenos : MonoBehaviour {

	public GameObject CruceX;
	public GameObject rectaV;
	public GameObject rectaH;
	public GameObject Curva1;
	public GameObject Curva2;
	public GameObject Curva3;
	public GameObject Curva4;
	public GameObject CruceT1;
	public GameObject CruceT2;
	public GameObject CruceT3;
	public GameObject CruceT4;
	public GameObject Auto;
	public string Archivo;

	private GameObject InstanciaAuto;
	private GameObject[] TerrenosGenerados;
	private Dictionary<string, string>[] Terrenos;
	private ArrayList Historial = new ArrayList();
	private int[] dirContrarias;

	private int contadorGenerados;
	private int contadorLeidos;
	private int totalTerrenos;


	private int BloqueX;
	private int BloqueZ;
	private int direccionEntrada;
	private int direccionSalida;

	private int terrenosRepetidos;
	private int totalRepeticiones;
	private int contadorRepeticiones;
	private bool cambioTerreno;
	private bool siguienteGenerado;
	private bool generacionActiva;
	private Vector3 posicionAnterior;

	//Largo de los arreglos de Terrenos y TerrenosGenerados
	private const int TotalElementos = 30;
	

	void Start () {
		//Inicializar variables dinamicas
		Terrenos = new Dictionary<string, string>[TotalElementos];
		TerrenosGenerados = new GameObject[TotalElementos];
		dirContrarias = new int[4] { 1, 0, 3, 2 };

		//Establecer variables iniciales
		contadorGenerados = 0;
		contadorLeidos = 1;
		totalTerrenos = 0;

		BloqueX = 0;
		BloqueZ = 0;
		direccionEntrada = 1;
		direccionSalida = 0;

		terrenosRepetidos = 0;
		totalRepeticiones = 1;
		contadorRepeticiones = totalRepeticiones;
		cambioTerreno = false;
		generacionActiva = true;
		siguienteGenerado = false;

		//Lectura y guardado de informacion de terrenos
		LeerArchivoConfiguracion (Archivo);

		//Instanciar terreno inicial
		generarNuevoTerreno(1, BloqueX, BloqueZ);

		//Instanciar Automovil
		InstanciaAuto = Instantiate (Auto, new Vector3 (252, 0.2f,480), Quaternion.identity) as GameObject;
		posicionAnterior = InstanciaAuto.transform.position;
	}
	

	void Update () {
		Vector3 target = InstanciaAuto.transform.position - posicionAnterior;
		posicionAnterior = InstanciaAuto.transform.position;

		//Coordenadas X y Z del terreno actual
		int anteriorX = ((int[])Historial[contadorGenerados-1])[0];
		int anteriorZ = ((int[])Historial[contadorGenerados-1])[1];

		//Posicion Automovil
		float x = InstanciaAuto.transform.position.x;
		float z = InstanciaAuto.transform.position.z;

		//Si el Automovil entra al siguiente terreno:
		if(z >= BloqueZ * 500 && z <= (BloqueZ + 1) * 500 && x >= BloqueX * 500 && x <= (BloqueX + 1) * 500){
			cambioTerreno = true;
			//print ("x: " + anteriorX + " - " + "z: " + anteriorZ);
			print ("x: " + BloqueX + " - " + "z: " + BloqueZ);
		}
		else { //sale del terreno actual
			cambioTerreno = false;
			siguienteGenerado = false;
		}


		if(cambioTerreno && !siguienteGenerado && generacionActiva){
			//print (direccionEntrada.ToString() + " - " + direccionSalida.ToString()); //aaaaaaaaaaaaaaaaaaaaaaaaaaa

			//Leer datos del terreno que se va a generar
			int tipo = Int32.Parse(Terrenos [contadorLeidos] ["tipo"]);
			direccionEntrada = dirContrarias[direccionSalida];
			direccionSalida = Int32.Parse(Terrenos [contadorLeidos] ["direccion"]);
			totalRepeticiones = Int32.Parse(Terrenos [contadorLeidos] ["repeticiones"]);
			contadorRepeticiones = totalRepeticiones;

			cambioTerreno = true;
			siguienteGenerado = true;

			//print (direccionEntrada.ToString() + " + " + direccionSalida.ToString()); //aaaaaaaaaaaaaaaaaaaaaaaaaaa

			//En caso de un cruce generar demas posibilidades
			if (Terrenos [contadorLeidos-1] ["tipo"].Equals("0")) {
				print ("Generacion multimple en Cruce");
				for (int i = 0; i < 4; i++) {
					if(i != direccionEntrada && i != direccionSalida) {
						switch (i)
						{
						case 0:
							generarNuevoTerreno(0, BloqueX, BloqueZ + 1);
							break;
						case 1:
							generarNuevoTerreno(0, BloqueX, BloqueZ - 1);
							break;
						case 2:
							generarNuevoTerreno(0, BloqueX - 1, BloqueZ);
							break;
						case 3:
							generarNuevoTerreno(0, BloqueX + 1, BloqueZ);
							break;
						default:
							print("Error: Direccion no identificada");
							break;
						}
					}
				}
			}
			contadorLeidos++;

			switch (direccionSalida)
			{
			case 0:
				BloqueZ += 1;
				break;
			case 1:
				BloqueZ -= 1;
				break;
			case 2:
				BloqueX -= 1;
				break;
			case 3:
				BloqueX += 1;
				break;
			default:
				print("Error: Direccion no identificada");
				break;
			}
			generarNuevoTerreno(tipo, BloqueX, BloqueZ);


		}
	}

	void generarNuevoTerreno(int tipo, int x, int z){
		switch (tipo)
		{
		case 0:
			TerrenosGenerados[contadorGenerados] = Instantiate(CruceX, new Vector3(x * 500, 0, z * 500), Quaternion.identity) as GameObject;
			print("Generacion: Cruce Completo");
			break;
		case 1:
			TerrenosGenerados[contadorGenerados] = Instantiate(rectaV, new Vector3(x * 500, 0, z * 500), Quaternion.identity) as GameObject;
			print("Generacion: rectaV");
			break;
		case 2:
			TerrenosGenerados[contadorGenerados] = Instantiate(rectaH, new Vector3(x * 500, 0, z * 500), Quaternion.identity) as GameObject;
			print("Generacion: rectaH");
			break;
		case 3:
			TerrenosGenerados[contadorGenerados] = Instantiate(Curva1, new Vector3(x * 500, 0, z * 500), Quaternion.identity) as GameObject;
			print("Generacion: Curva1");
			break;
		case 4:
			TerrenosGenerados[contadorGenerados] = Instantiate(Curva2, new Vector3(x * 500, 0, z * 500), Quaternion.identity) as GameObject;
			print("Generacion: Curva2");
			break;
		case 5:
			TerrenosGenerados[contadorGenerados] = Instantiate(Curva3, new Vector3(x * 500, 0, z * 500), Quaternion.identity) as GameObject;
			print("Generacion: Curva3");
			break;
		case 6:
			TerrenosGenerados[contadorGenerados] = Instantiate(Curva4, new Vector3(x * 500, 0, z * 500), Quaternion.identity) as GameObject;
			print("Generacion: Curva4");
			break;
		case 7:
			TerrenosGenerados[contadorGenerados] = Instantiate(CruceT1, new Vector3(x * 500, 0, z * 500), Quaternion.identity) as GameObject;
			print("Generacion: Cruce T 1");
			break;
		case 8:
			TerrenosGenerados[contadorGenerados] = Instantiate(CruceT2, new Vector3(x * 500, 0, z * 500), Quaternion.identity) as GameObject;
			print("Generacion: Cruce T 2");
			break;
		case 9:
			TerrenosGenerados[contadorGenerados] = Instantiate(CruceT3, new Vector3(x * 500, 0, z * 500), Quaternion.identity) as GameObject;
			print("Generacion: Cruce T 3");
			break;
		case 10:
			TerrenosGenerados[contadorGenerados] = Instantiate(CruceT4, new Vector3(x * 500, 0, z * 500), Quaternion.identity) as GameObject;
			print("Generacion: Cruce T 4");
			break;
		default:
			print("Error: Terreno no identificado; no se puede cargar ningun Prefab");
			break;
		}

		//Guarda configuracion del terreno anteriormente generado
		//print ("x: " + x + " - " + "z: " + z);
		Historial.Add(new int[] { x, z, direccionEntrada, direccionSalida });
		contadorGenerados++;

		/*if(ContadorGenerados - 2 >= 0)
			Destroy (TerrenosGenerados[ContadorGenerados - 2], 1.0f);*/
		if (contadorLeidos >= totalTerrenos + terrenosRepetidos){
			generacionActiva = false;
			print ("Generacion de Caminos Finalizada");
		}
	}


	void LeerArchivoConfiguracion(string nameFile)
	{
		Terrenos[totalTerrenos] = new Dictionary<string, string>(){{"tipo", "1"},{"nombre", "Recto Vertical"},{"direccion", "0"}};
		totalTerrenos++;

		string line;
		var dictionary = new Dictionary<string, string>();
		
		System.IO.StreamReader file = new System.IO.StreamReader("Estadisticas/" + nameFile);
		while ((line = file.ReadLine()) != null) {
			if(line.Equals("{")){
				dictionary = new Dictionary<string, string>();
				continue;
			}
			else if(line.Equals("}")){
				Terrenos[totalTerrenos] = dictionary;
				totalTerrenos++;
				continue;
			}
			else{
				line = line.Replace(",", "").Replace("\"", "").Replace(": ", ":").Replace("\t", "");
				string[] words = line.Split(':');
				dictionary.Add(words[0], words[1]);
			}
		}

		file.Close();
	}







}
