using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Xml.Linq;

using System;
using System.IO;
using System.Net;
using System.Text;

public static class API {

    private static bool sesionIniciada = false;
    private static string rutActual = string.Empty;
    private static int tiempoTotal = 0;

    private static List<int> Velocidad = new List<int>();
    private static List<int> TiemposVelocidad = new List<int>();

    private static List<int> TiemposFueraCarril = new List<int>();

    private static List<bool> UtilizaLuces = new List<bool>();
    private static List<int> TerrenoLuces = new List<int>();

    private static List<int> VelocidadCambio = new List<int>();
    private static List<int> RPMCambio = new List<int>();
    private static List<int> TipoCambio = new List<int>();
    private static List<int> TiempoCambio = new List<int>();

    

    //Iniciar Sesion para el registro de estadisticas
    public static bool startSesion(string RUT)
    {
        if (sesionIniciada)
        {
            return false;
        }
        else
        {
            sesionIniciada = true;
            rutActual = RUT;
            return true;
            
        }
    }

    //Registra los tiempos del juego en que el alumno se sale del carril
    public static void salidaCarril(int tiempo)
    {
        if (sesionIniciada)
        {
            TiemposFueraCarril.Add(tiempo);
        }
    }

    //Registra si el alumno utilizo las luces en un cruce determinado
    public static void utiLuces(bool accion)
    {
        if (sesionIniciada)
        {
            UtilizaLuces.Add(accion);
        }
    }

    //Registra la velocidad en un tiempo dado
    public static void registrarVelocidad(int velocidad)
    {
        if (sesionIniciada)
        {
            Velocidad.Add(velocidad);
            TiemposVelocidad.Add(timer());
        }
        
    }

    //Registra la velocidad y RPM a las que pasa a un cambio determinado
    public static void registrarCambio(int velocidad, int RPM, int cambio)
    {
        if (sesionIniciada)
        {
            VelocidadCambio.Add(velocidad);
            RPMCambio.Add(RPM);
            TipoCambio.Add(cambio);
            TiempoCambio.Add(timer());
        }
    }


    public static bool finalizarSesion()
    {
        if (sesionIniciada)
        {


            //Reseteando Variables
            sesionIniciada = false;
            rutActual = string.Empty;
            tiempoTotal = timer();

            //Limpiando Arreglos
            Velocidad.Clear();
            TiemposVelocidad.Clear();

            TiemposFueraCarril.Clear();

            UtilizaLuces.Clear();
            TerrenoLuces.Clear();

            VelocidadCambio.Clear();
            RPMCambio.Clear();
            TipoCambio.Clear();
            TiempoCambio.Clear();

            return true;
        }
        else
        {
            return false;
        }
       
    }

    public static string requestHTTP(string pagina, string datos)
    {
        WebRequest request = WebRequest.Create(pagina);
        request.Method = "POST";
        byte[] byteArray = Encoding.UTF8.GetBytes(datos);
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = byteArray.Length;
        Stream dataStream = request.GetRequestStream();
        dataStream.Write(byteArray, 0, byteArray.Length);
        dataStream.Close();

        WebResponse response = request.GetResponse();
        Console.WriteLine(((HttpWebResponse)response).StatusDescription);
        dataStream = response.GetResponseStream();
        StreamReader reader = new StreamReader(dataStream);
        string responseFromServer = reader.ReadToEnd();
        Console.WriteLine(responseFromServer);

        reader.Close();
        dataStream.Close();
        response.Close();

        return responseFromServer;
    }

    public static string filesHTTP(string uriString, string fileName)
    {
        WebClient myWebClient = new WebClient();
        byte[] responseArray = myWebClient.UploadFile(uriString, "POST", fileName);
        string response = System.Text.Encoding.ASCII.GetString(responseArray);

        return response;
    }


    //Metodos Privados//

    private static int timer()
    {
        return (int)Time.realtimeSinceStartup;
    }

    private static void generarJSON()
    {
        int contador1 = 0;
        string fecha = DateTime.Now.ToString();

        StreamWriter file = new StreamWriter("datos.json");
        file.WriteLine("{\"estadisticas\":{");

        //Escritura informaciones
        file.WriteLine("\"informacion\": {");
        file.WriteLine("\"fecha\": \"10/9/2014 9:45:06 PM\",");
        file.WriteLine("\"rut\": \"11222333-4\",");
        file.WriteLine("\"clase\": 1,");
        file.WriteLine("\"generacion\": 14,");
        file.WriteLine("\"key\": \"A3763NF832\"");
        file.WriteLine("},");

        //Escritura velocidad
        file.WriteLine("\"velocidad\": {");
        file.WriteLine("\"promedio\": 1,");
        file.WriteLine("\"maxima\": 1,");
        file.WriteLine("\"minima\": 1,");
        file.WriteLine("\"lista\": [");

        file.WriteLine("{\"t\": 1, \"v\": 50},");
        file.WriteLine("{\"t\": 2, \"v\": 60},");
        file.WriteLine("{\"t\": 3, \"v\": 70},");
        file.WriteLine("{\"t\": 4, \"v\": 70},");
        file.WriteLine("{\"t\": 5, \"v\": 70},");
        file.WriteLine("{\"t\": 6, \"v\": 80},");
        file.WriteLine("{\"t\": 10, \"v\": 90},");
        file.WriteLine("{\"t\": 12, \"v\": 00},");

        file.WriteLine("],");
        file.WriteLine("},");

        //Escritura carril
        file.WriteLine("\"carril\": {");
        file.WriteLine("\"dentro\": 490,");
        file.WriteLine("\"fuera\": 80");
        file.WriteLine("},");

        file.Close();
        contador1++;

    }

    private static void tratamientoDatos()
    {

    }
}
