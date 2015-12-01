using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Xml.Linq;

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



            XDocument doc = new XDocument(
            new XDeclaration("1.0", "utf-8", "no"),
            new XComment("Starbuzz Customer Loyalty Data"),
            new XElement("Contacts",
                new XElement("Contacto1",
                    new XElement("Name", "Patrick Hines"),
                    new XElement("Phone", "206-555-0144"),
                    new XElement("Address",
                        new XElement("Street1", "123 Main St"),
                        new XElement("City", "Mercer Island"),
                        new XElement("State", "WA"),
                        new XElement("Postal", "68042")
                    )
                )
            )
         );


            //Reseteando Variables
            sesionIniciada = false;
            rutActual = string.Empty;
            tiempoTotal = timer();

            //Reseteando Arreglos
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

    public static void crearDocumento()
    {
        XDocument doc = new XDocument(
            new XDeclaration("1.0", "utf-8", "no"),
            new XComment("Starbuzz Customer Loyalty Data"),
            new XElement("Contacts",
                new XElement("Contacto1",
                    new XElement("Name", "Patrick Hines"),
                    new XElement("Phone", "206-555-0144"),
                    new XElement("Address",
                        new XElement("Street1", "123 Main St"),
                        new XElement("City", "Mercer Island"),
                        new XElement("State", "WA"),
                        new XElement("Postal", "68042")
                    )
                )
            )
         );

        XElement nuevo = new XElement("Contacto2",
            new XElement("Name", "Patrick Hines"),
            new XElement("Phone", "206-555-0144"),
            new XElement("Address",
                new XElement("Street1", "123 Main St"),
                new XElement("City", "Mercer Island"),
                new XElement("State", "WA"),
                new XElement("Postal", "68042")
            )
        );
        doc.Root.Add(nuevo);

        XElement nuevo2 = new XElement("Contacto3",
            new XElement("Name", "Patrick Hines"),
            new XElement("Phone", "206-555-0144"),
            new XElement("Address",
                new XElement("Street1", "123 Main St"),
                new XElement("City", "Mercer Island"),
                new XElement("State", "WA"),
                new XElement("Postal", "68042")
            )
        );
        doc.Root.Add(nuevo2);

        XElement nuevo3 = new XElement("Phone2", "206-555-0144");

        doc.Descendants("Name").Last().AddAfterSelf(nuevo3);

        doc.AddFirst(new XElement("NewChild", "new content"));
        doc.AddFirst(new XElement("NewChild", "new content"));
        doc.AddFirst(new XElement("NewChild", "new content"));

        doc.Save("prueba21.xml");
    }



    //Metodos Privados//

    private static int timer()
    {
        return (int)Time.realtimeSinceStartup;
    }

    private static void generarJSON()
    {

    }

    private static void tratamientoDatos()
    {

    }
}
