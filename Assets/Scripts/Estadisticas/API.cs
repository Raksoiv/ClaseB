using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net;
using System.Text;

public static class API {

    private static bool sesionIniciada = false;
    private static bool procesandoFinalizar = false;
    private static string rutActual = string.Empty;
    private static int contadorEstadistica = 0;

    private static List<float> Velocidad = new List<float>();
    private static List<int> TiemposVelocidad = new List<int>();

    private static List<float> TiemposFueraCarril = new List<float>();
    private static List<int> TiemposRegistroCarril = new List<int>();

    private static List<bool> UtilizaLuces = new List<bool>();
    private static List<int> TerrenoLuces = new List<int>();

    private static List<float> VelocidadCambio = new List<float>();
    private static List<int> RPMCambio = new List<int>();
    private static List<int> TipoCambio = new List<int>();
    private static List<int> TiempoCambio = new List<int>();

    

    //Iniciar Sesion para el registro de estadisticas
    public static bool startSesion(string RUT)
    {
        if (sesionIniciada || procesandoFinalizar)
        {
            return false;
        }
        else
        {
            sesionIniciada = true;
            rutActual = RUT;
            contadorEstadistica++;
            return true;
        }
    }

    //Registra los tiempos del juego en que el alumno se sale del carril
    public static void salidaCarril(float tiempo)
    {
        if (sesionIniciada)
        {
			Debug.Log (tiempo);
            TiemposFueraCarril.Add(tiempo);
            TiemposRegistroCarril.Add(timer());
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
    public static void registrarVelocidad(float velocidad)
    {
        if (sesionIniciada)
        {
            Velocidad.Add(velocidad);
            TiemposVelocidad.Add(timer());
        }
        
    }

    //Registra la velocidad y RPM a las que pasa a un cambio determinado
    public static void registrarCambio(float velocidad, int RPM, int cambio)
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
        if (sesionIniciada && !procesandoFinalizar)
        {
            sesionIniciada = false;
            procesandoFinalizar = true;

            WWWForm form = new WWWForm();
            form = generarJSON(form);

            string file = volcarArchivo("Estadisticas/estadistica" + contadorEstadistica.ToString() + ".json");
            Debug.Log(file);
            string retorno = requestHTTP("http://claseb.dribyte.cl/api/v1/estadisticas", file);
            Debug.Log(retorno);

            //Limpiando Arreglos y Variables
            Velocidad.Clear();
            TiemposVelocidad.Clear();
            TiemposFueraCarril.Clear();
            TiemposRegistroCarril.Clear();
            UtilizaLuces.Clear();
            TerrenoLuces.Clear();
            VelocidadCambio.Clear();
            RPMCambio.Clear();
            TipoCambio.Clear();
            TiempoCambio.Clear();

            procesandoFinalizar = false;
            rutActual = string.Empty;
            return true;
        }
        else
        {
            return false;
        }
       
    }

    public static string requestHTTP(string pagina, string datos)
    {
        string responseFromServer = "";
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(pagina);
            request.Method = "POST";
            byte[] byteArray = Encoding.UTF8.GetBytes(datos);
            request.Accept = "application/json";
            request.ContentType = "application/json";
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse response = request.GetResponse();
            //Debug.Log(((HttpWebResponse)response).StatusDescription);
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            responseFromServer = reader.ReadToEnd();
            //Debug.Log(responseFromServer);

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

    public static string filesHTTP(string uriString, string fileName)
    {
        string response = "";
        try
        {
            WebClient myWebClient = new WebClient();
            myWebClient.Headers.Add("Content-Type", "application/json");
            byte[] responseArray = myWebClient.UploadFile(uriString, "POST", fileName);
            response = Encoding.ASCII.GetString(responseArray);
        }
        catch (Exception e)
        {
            response = e.Message;
        }
        
        return response;
    }

    private static int timer()
    {
        return (int)Time.realtimeSinceStartup;
    }

    private static string volcarArchivo(string nameFile)
    {
        string line = "";
        string data = "";

        StreamReader file = new StreamReader(nameFile);
        while ((line = file.ReadLine()) != null)
            data += line;

        return data;
    }

    private static WWWForm generarJSON(WWWForm form)
    {
        //Preparando variables
        // string fecha = DateTime.Now.ToString();
        string itemVelocidad = "";
        string itemTiempo = "";
        string itemRuta = "";
        string itemVelCambio = "";
        string itemRPMCambio = "";
        string itemFueraCarril = "";

        int tiempoTotalJuego = timer();
        float tiempoFueraCarril = 0;
        float tiempoDentroCarril = 0;
        int contador = 0;
        int velMedia = 0;
        int velMaxima = 0;
        int velMinima = 0;

        //Tratamiento de datos
        foreach (int collection in Velocidad)
        {
            itemVelocidad += collection.ToString() + ",";
            velMedia += collection;
            contador++;
            if (collection > velMaxima)
                velMaxima = collection;
            if (collection < velMinima)
                velMinima = collection;
        }
        itemVelocidad = itemVelocidad.Substring(0, itemVelocidad.Length - 1);
        velMedia = velMedia / contador;

        foreach (var collection in TiemposVelocidad)
            itemTiempo += collection.ToString() + ",";
        itemTiempo = itemTiempo.Substring(0, itemTiempo.Length - 1);

        foreach (var collection in VelocidadCambio)
            itemVelCambio += collection.ToString() + ",";
        itemVelCambio = itemVelCambio.Substring(0, itemVelCambio.Length - 1);

        foreach (var collection in RPMCambio)
            itemRPMCambio += collection.ToString() + ",";
        itemRPMCambio = itemRPMCambio.Substring(0, itemRPMCambio.Length - 1);

        foreach (float collection in TiemposFueraCarril)
        {
            itemFueraCarril += collection.ToString() + ",";
            tiempoFueraCarril += collection;
        }
        itemFueraCarril = itemFueraCarril.Substring(0, itemFueraCarril.Length - 1);
        tiempoDentroCarril = tiempoTotalJuego - tiempoFueraCarril;

        //LLenar Form HTML
        form.AddField("estadisticas[velocidad]", "[" + itemVelocidad + "]");
        form.AddField("estadisticas[tiempoVelocidad]", "[" + itemTiempo + "]");
        form.AddField("estadisticas[velocidadMedia]", velMedia.ToString());
        form.AddField("estadisticas[velocidadMaxima]", velMaxima.ToString());
        form.AddField("estadisticas[velocidadMinima]", velMinima.ToString());
        form.AddField("estadisticas[ruta]", itemRuta);
        form.AddField("estadisticas[cambiosVelocidad]", "[" + itemVelCambio + "]");
        form.AddField("estadisticas[cambiosRpm]", "[" + itemRPMCambio + "]");
        form.AddField("estadisticas[alumno_id]", rutActual);
		form.AddField("estadisticas[tiempoCarril]", tiempoDentroCarril.ToString ());
		form.AddField("estadisticas[tiempoFueraCarril]", tiempoFueraCarril.ToString ());

        //Escritura Archivo
        StreamWriter file = new StreamWriter("Estadisticas/estadistica" + contadorEstadistica.ToString() + ".json");
        file.WriteLine("{");
        file.WriteLine("\"velocidad\":[" + itemVelocidad + "],");
        file.WriteLine("\"tiempoVelocidad\":[" + itemTiempo + "],");
        file.WriteLine("\"velocidadMedia\":" + velMedia.ToString() + ",");
        file.WriteLine("\"velocidadMaxima\":" + velMaxima.ToString() + ",");
        file.WriteLine("\"velocidadMinima\":" + velMinima.ToString() + ",");
        file.WriteLine("\"ruta\": \"" + itemRuta + "\",");
        file.WriteLine("\"cambiosVelocidad\":[" + itemVelCambio + "],");
        file.WriteLine("\"cambiosRpm\":[" + itemRPMCambio + "],");
        file.WriteLine("\"alumno_id\":" + rutActual + ",");
        file.WriteLine("\"tiempoCarril\":" + tiempoDentroCarril + ",");
        file.WriteLine("\"tiempoFueraCarril\":" + tiempoFueraCarril);
        file.WriteLine("}");
        file.Close();

        return form;
    }
}
