using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Text;

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
    public GameObject Auto;

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


        //Descarga y lectura de informacion de terrenos
        string mapa = requestHTTP("http://claseb.dribyte.cl/api/v1/mapa");
        TransformarMapa(mapa);

        /*for (int i = 0; i < DatosTerrenos.Length; i++)
        {
            print(DatosTerrenos[i]["tipo"]);
            print(DatosTerrenos[i]["nombre"]);
            print(DatosTerrenos[i]["direccion"]);
            print(DatosTerrenos[i]["senales"]);
        }*/

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
            print(direccionSalida);
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

            int temp = TransformarDireccion(DatosTerrenos[terrenosLeidos-1]["direccion"]);
            if (temp != direccionSalida)
            {
                terrenosLeidos--;
            }
            LeerSiguienteTerreno();
        }
    }

    int TransformarDireccion(string sentido)
    {
        int dirCorrecta = -1;
        switch (dirContrarias[direccionSalida])
        {
            case 0:
                if (sentido.Equals("Izquierda"))
                    dirCorrecta = 3;
                else if (sentido.Equals("Derecha"))
                    dirCorrecta = 2;
                else
                    dirCorrecta = direccionSalida;
                break;
            case 1:
                if (sentido.Equals("Izquierda"))
                    dirCorrecta = 2;
                else if (sentido.Equals("Derecha"))
                    dirCorrecta = 3;
                else
                    dirCorrecta = direccionSalida;
                break;
            case 2:
                if (sentido.Equals("Izquierda"))
                    dirCorrecta = 0;
                else if (sentido.Equals("Derecha"))
                    dirCorrecta = 1;
                else
                    dirCorrecta = direccionSalida;
                break;
            case 3:
                if (sentido.Equals("Izquierda"))
                    dirCorrecta = 1;
                else if (sentido.Equals("Derecha"))
                    dirCorrecta = 0;
                else
                    dirCorrecta = direccionSalida;
                break;
            default:
                break;
        }
        
        
        return dirCorrecta;
    }

    void LeerSiguienteTerreno()
    {
        //Leer datos del terreno que se va a generar
        int tipo = Int32.Parse(DatosTerrenos[terrenosLeidos]["tipo"]);
        int direccionCorrecta = TransformarDireccion(DatosTerrenos[terrenosLeidos]["direccion"]);

        terrenosLeidos++;
        cambioTerreno = false;

        int tempX = BloqueX;
        int tempZ = BloqueZ;
        int cambio = 0;
        switch (direccionCorrecta)
        {
            case 0:
                tempZ += 1;
                cambio = 1;
                break;
            case 1:
                tempZ -= 1;
                cambio = 1;
                break;
            case 2:
                tempX -= 1;
                cambio = 2;
                break;
            case 3:
                tempX += 1;
                cambio = 2;
                break;
            default:
                print("Error: Direccion no identificada");
                break;
        }

        if (tipo == 0)
            GenerarNuevoTerreno(tipo, tempX, tempZ, 0);
        else
            GenerarNuevoTerreno(cambio, tempX, tempZ, 0);
        

        //En caso de un cruce generar demas posibilidades
        try
        {
            if (DatosTerrenos[terrenosLeidos - 2]["tipo"].Equals("0"))
            {
                //direccionCorrecta = TransformarDireccion(DatosTerrenos[terrenosLeidos]["direccion"]);
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

        int DL = 10;
        if (terrenosInstanciados - DL >= 0 && Historial[terrenosInstanciados - DL][4] != 1)
        {
            Destroy(InstanciasTerrenos[terrenosInstanciados - DL], 5.0f);
            Historial[terrenosInstanciados - DL][4] = 1;
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

    int getNombre(string nombre)
    {
        int retorno = -1;
        switch (nombre)
        {
            case "Cruce":
                retorno = 0;
                break;
            case "Recto Vertical":
                retorno = 1;
                break;
            case "Recto Horizontal":
                retorno = 1;
                break;
            default:
                break;
        }
        return retorno;
    }

    void TransformarMapa(string mapa)
    {
        //{"segmentos":[
        //{ "type":"Recto","senales":[]},
        //{ "type":"Cruce","senales":[],"direccion":"Derecha"},
        //{ "type":"Recto","senales":[]},
        //{ "type":"Cruce","senales":[],"direccion":"Izquierda"}]}

        DatosTerrenos[terrenosTotales] = new Dictionary<string, string>() { { "tipo", "1" }, { "nombre", "Recto" }, { "direccion", "Adelante" }, { "senales", "" } };
        terrenosTotales++;

        var dictionary = new Dictionary<string, string>();

        mapa = mapa.Substring(15, (int)mapa.Length-18);
        mapa = mapa.Replace("},{", "&");
        
        string[] partes = mapa.Split('&');
        foreach (var item in partes)
        {
            dictionary = new Dictionary<string, string>();

            //Agregar Tipo
            if (item.IndexOf("Recto") != -1)
            {
                dictionary.Add("tipo", "1");
                dictionary.Add("nombre", "Recto");
            }
            else
            {
                dictionary.Add("tipo", "0");
                dictionary.Add("nombre", "Cruce");
            }

            //Agregar Direccion
            if (item.IndexOf("direccion") != -1)
            {
                if (item.IndexOf("Adelante") == -1)
                {
                    if (item.IndexOf("Izquierda") == -1)
                    {
                        dictionary.Add("direccion", "Derecha");
                    }
                    else
                    {
                        dictionary.Add("direccion", "Izquierda");
                    }
                }
                else
                {
                    dictionary.Add("direccion", "Adelante");
                }
            }
            else
            {
                dictionary.Add("direccion", "Adelante");
            }


            //Agregar Senaleticas
            if (item.IndexOf("senales") != -1)
            {
                string senales = item.Substring(item.IndexOf("[") + 1, item.IndexOf("]") - item.IndexOf("[") - 1);
                senales = senales.Replace("\"", "");
                dictionary.Add("senales", senales);
            }
            else
            {
                dictionary.Add("senales", "");
            }

            DatosTerrenos[terrenosTotales] = dictionary;
            terrenosTotales++;
        }
    }

    string requestHTTP(string pagina)
    {
        string responseFromServer = "";
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(pagina);

            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            responseFromServer = reader.ReadToEnd();
            Debug.Log(responseFromServer);

            reader.Close();
            dataStream.Close();
            response.Close();
        }
        catch (Exception e)
        {
            responseFromServer = e.Message;
        }

        return responseFromServer;
    }
}
