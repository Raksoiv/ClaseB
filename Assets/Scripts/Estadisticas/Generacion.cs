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
 *  
 */
public class Generacion : MonoBehaviour {

    public GameObject CruceX;
    public GameObject rectaV;
    public GameObject rectaH;
    public GameObject Curva1;
    public GameObject Curva2;
    public GameObject Curva3;
    public GameObject Curva4;
    public GameObject Auto;
    public string Archivo;

    public GameObject Pare;
    public GameObject SedaElPaso;
    public GameObject NoEntrar;
    public GameObject NoVirarDerecha;
    public GameObject NoVirarIzquierda;
    public GameObject DireccionObligadaDerecha;
    public GameObject DireccionObligadaIzquierda;

    private GameObject InstanciaAuto;
    private GameObject[] InstanciasTerrenos;
    private int[][] Historial;  //[BloqueX, BloqueY, CaminoExtra?, CaminoUsado?, CaminoEliminado?]
    private Dictionary<string, string>[] DatosTerrenos;
    private int[] dirContrarias;

    private int terrenosInstanciados;
    private int terrenosLeidos;
    private int terrenosTotales;

    private int BloqueX;
    private int BloqueZ;

    private int direccionEntrada;
    private int direccionSalida;

    private bool cambioTerreno;

    void Start()
    {
        //Inicializar variables y estructuras
        InstanciasTerrenos = new GameObject[30];
        Historial = new int[30][];
        DatosTerrenos = new Dictionary<string, string>[30];
        dirContrarias = new int[4] { 1, 0, 3, 2 };

        BloqueX = 0;
        BloqueZ = -1;
        terrenosInstanciados = 0;
        terrenosLeidos = 0;
        terrenosTotales = 0;
        cambioTerreno = false;

        //Lectura y guardado de informacion de terrenos
        LeerArchivoConfiguracion(Archivo);

        //Instanciar terreno inicial
        LeerSiguienteTerreno();

        //Instanciar Automovil
        InstanciaAuto = Instantiate(Auto, new Vector3(252, 0.2f, 480), Quaternion.identity) as GameObject;
    }

    void Update()
    {
        //Posicion Automovil
        float x = InstanciaAuto.transform.position.x;
        float z = InstanciaAuto.transform.position.z;

        //Verifica si el automovil se sale del terreno actual
        if (z >= (BloqueZ + 1) * 500)
        {
            direccionSalida = 0;
            cambioTerreno = true;
        }
        else if (z <= BloqueZ * 500)
        {
            direccionSalida = 1;
            cambioTerreno = true;
        }
        else if (x <= BloqueX * 500)
        {
            direccionSalida = 2;
            cambioTerreno = true;
        }
        else if (x >= (BloqueX + 1) * 500)
        {
            direccionSalida = 3;
            cambioTerreno = true;
        }
        else {
            cambioTerreno = false;
        }

        if (cambioTerreno)
        {
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

            if (Int32.Parse(DatosTerrenos[terrenosLeidos - 1]["direccion"]) != direccionSalida)
            {
                terrenosLeidos--;
            }
            LeerSiguienteTerreno();
        }
    }

    void LeerSiguienteTerreno()
    {
        //Leer datos del terreno que se va a generar
        int tipo = Int32.Parse(DatosTerrenos[terrenosLeidos]["tipo"]);
        int direccionCorrecta = Int32.Parse(DatosTerrenos[terrenosLeidos]["direccion"]);

        terrenosLeidos++;
        cambioTerreno = false;

        int tempX = BloqueX;
        int tempZ = BloqueZ;
        switch (direccionCorrecta)
        {
            case 0:
                tempZ += 1;
                break;
            case 1:
                tempZ -= 1;
                break;
            case 2:
                tempX -= 1;
                break;
            case 3:
                tempX += 1;
                break;
            default:
                print("Error: Direccion no identificada");
                break;
        }

        GenerarNuevoTerreno(tipo, tempX, tempZ, 0);

        //En caso de un cruce generar demas posibilidades
        try
        {
            if (DatosTerrenos[terrenosLeidos - 2]["tipo"].Equals("0"))
            {
                direccionCorrecta = Int32.Parse(DatosTerrenos[terrenosLeidos]["direccion"]);
                for (int i = 0; i < 4; i++)
                {
                    if (i != direccionCorrecta && i != dirContrarias[direccionSalida])
                    {
                        switch (i)
                        {
                            case 0:
                                GenerarNuevoTerreno(0, BloqueX, BloqueZ + 1, 1);
                                break;
                            case 1:
                                GenerarNuevoTerreno(0, BloqueX, BloqueZ - 1, 1);
                                break;
                            case 2:
                                GenerarNuevoTerreno(0, BloqueX - 1, BloqueZ, 1);
                                break;
                            case 3:
                                GenerarNuevoTerreno(0, BloqueX + 1, BloqueZ, 1);
                                break;
                            default:
                                print("Error: Direccion no identificada");
                                break;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {

        }
    }

    void GenerarSenaleticas()
    {

        foreach (var tipo in dirContrarias)
        {
            int x = 0;
            int z = 0;
            switch (tipo)
            {
                case 0:
                    InstanciasTerrenos[terrenosInstanciados] = Instantiate(CruceX, new Vector3(x * 500, 0, z * 500), Quaternion.identity) as GameObject;
                    print("Generacion: Cruce Completo -  X: " + x + " -  Z: " + z);
                    break;
                case 1:
                    InstanciasTerrenos[terrenosInstanciados] = Instantiate(rectaV, new Vector3(x * 500, 0, z * 500), Quaternion.identity) as GameObject;
                    print("Generacion: rectaV -  X: " + x + " -  Z: " + z);
                    break;
                case 2:
                    InstanciasTerrenos[terrenosInstanciados] = Instantiate(rectaH, new Vector3(x * 500, 0, z * 500), Quaternion.identity) as GameObject;
                    print("Generacion: rectaH -  X: " + x + " -  Z: " + z);
                    break;
                case 3:
                    InstanciasTerrenos[terrenosInstanciados] = Instantiate(Curva1, new Vector3(x * 500, 0, z * 500), Quaternion.identity) as GameObject;
                    print("Generacion: Curva1 -  X: " + x + " -  Z: " + z);
                    break;
                case 4:
                    InstanciasTerrenos[terrenosInstanciados] = Instantiate(Curva2, new Vector3(x * 500, 0, z * 500), Quaternion.identity) as GameObject;
                    print("Generacion: Curva2 -  X: " + x + " -  Z: " + z);
                    break;
                case 5:
                    InstanciasTerrenos[terrenosInstanciados] = Instantiate(Curva3, new Vector3(x * 500, 0, z * 500), Quaternion.identity) as GameObject;
                    print("Generacion: Curva3 -  X: " + x + " -  Z: " + z);
                    break;
                case 6:
                    InstanciasTerrenos[terrenosInstanciados] = Instantiate(Curva4, new Vector3(x * 500, 0, z * 500), Quaternion.identity) as GameObject;
                    print("Generacion: Curva4 -  X: " + x + " -  Z: " + z);
                    break;
                default:
                    print("Error: Terreno no identificado; no se puede cargar ningun Prefab");
                    break;
            }
        }
    }

    void GenerarNuevoTerreno(int tipo, int x, int z, int extra)
    {
        switch (tipo)
        {
            case 0:
                InstanciasTerrenos[terrenosInstanciados] = Instantiate(CruceX, new Vector3(x * 500, 0, z * 500), Quaternion.identity) as GameObject;
                print("Generacion: Cruce Completo -  X: " + x + " -  Z: " + z);
                break;
            case 1:
                InstanciasTerrenos[terrenosInstanciados] = Instantiate(rectaV, new Vector3(x * 500, 0, z * 500), Quaternion.identity) as GameObject;
                print("Generacion: rectaV -  X: " + x + " -  Z: " + z);
                break;
            case 2:
                InstanciasTerrenos[terrenosInstanciados] = Instantiate(rectaH, new Vector3(x * 500, 0, z * 500), Quaternion.identity) as GameObject;
                print("Generacion: rectaH -  X: " + x + " -  Z: " + z);
                break;
            case 3:
                InstanciasTerrenos[terrenosInstanciados] = Instantiate(Curva1, new Vector3(x * 500, 0, z * 500), Quaternion.identity) as GameObject;
                print("Generacion: Curva1 -  X: " + x + " -  Z: " + z);
                break;
            case 4:
                InstanciasTerrenos[terrenosInstanciados] = Instantiate(Curva2, new Vector3(x * 500, 0, z * 500), Quaternion.identity) as GameObject;
                print("Generacion: Curva2 -  X: " + x + " -  Z: " + z);
                break;
            case 5:
                InstanciasTerrenos[terrenosInstanciados] = Instantiate(Curva3, new Vector3(x * 500, 0, z * 500), Quaternion.identity) as GameObject;
                print("Generacion: Curva3 -  X: " + x + " -  Z: " + z);
                break;
            case 6:
                InstanciasTerrenos[terrenosInstanciados] = Instantiate(Curva4, new Vector3(x * 500, 0, z * 500), Quaternion.identity) as GameObject;
                print("Generacion: Curva4 -  X: " + x + " -  Z: " + z);
                break;
            default:
                print("Error: Terreno no identificado; no se puede cargar ningun Prefab");
                break;
        }

        //Eliminar terrenos sobrepuestos
        for (int i = 0; i < terrenosInstanciados; i++)
        {
            if (Historial[i][0] == x && Historial[i][1] == z && Historial[i][4] != 1)
            {
                Destroy(InstanciasTerrenos[i], 1.0f);
                Historial[i][4] = 1;
                break;
            }
        }

        Historial[terrenosInstanciados] = new int[] { x, z, extra, 0, 0 };
        terrenosInstanciados++;

        if (terrenosInstanciados - 8 >= 0 && Historial[terrenosInstanciados - 8][4] != 1)
        {
            Destroy(InstanciasTerrenos[terrenosInstanciados - 8], 1.0f);
            Historial[terrenosInstanciados - 8][4] = 1;
        }
    }

    bool SobreTerreno(int num_terreno, int x, int z)
    {
        if (z >= BloqueZ * 500 && z <= (BloqueZ + 1) * 500 && x >= BloqueX * 500 && x <= (BloqueX + 1) * 500)
        {
            return true;
        }
        else {
            return false;
        }
    }

    void LeerArchivoConfiguracion(string nameFile)
    {

    }

    void LeerArchivoConfiguracion2(string nameFile)
    {
        DatosTerrenos[terrenosTotales] = new Dictionary<string, string>() { { "tipo", "1" }, { "nombre", "Recto Vertical" }, { "direccion", "0" } };
        terrenosTotales++;

        string line;
        var dictionary = new Dictionary<string, string>();

        System.IO.StreamReader file = new System.IO.StreamReader("Estadisticas/" + nameFile);
        while ((line = file.ReadLine()) != null)
        {
            if (line.Equals("{"))
            {
                dictionary = new Dictionary<string, string>();
                continue;
            }
            else if (line.Equals("	\"segmentos\":["))
            {
                if (line.Equals("{"))
                {
                    dictionary = new Dictionary<string, string>();
                    continue;
                }
            }
            else if (line.Equals("}"))
            {
                DatosTerrenos[terrenosTotales] = dictionary;
                terrenosTotales++;
                continue;
            }
            else {
                line = line.Replace(",", "").Replace("\"", "").Replace(": ", ":").Replace("\t", "");
                string[] words = line.Split(':');
                dictionary.Add(words[0], words[1]);
            }
        }

        file.Close();
    }
}
