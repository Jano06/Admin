using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Web.UI;

/// <summary>
/// Descripción breve de NavegacionSegura
/// </summary>
public class NavegacionSegura
{
    private static Hashtable navTo = new Hashtable() 
        {
            //{"ConsultaAccionesParam", "~/views/Acciones/ConsultaAcciones.aspx?fechaInicio={0}&fechaFin={1}&linea={2}"},
            //{"Grafica","~/views/Graficas/GeneraGrafica.aspx"}
        };

    private static string NavTo(string navId)
    {
        return navTo[navId] == null ? String.Empty : navTo[navId].ToString();
    }

    public static Boolean navega(Page pagina)
    {
        string navId = pagina.Request.QueryString["navTo"] == null ? String.Empty : pagina.Request.QueryString["navTo"];

        string url = NavTo(navId);

        if (url.Equals(String.Empty))
        {
            return false;
        }

        pagina.Response.Redirect(url);

        return true;
    }

    public static Boolean navega(Page pagina, string[] parametros)
    {
        string navId = pagina.Request.QueryString["navTo"] == null ? String.Empty : pagina.Request.QueryString["navTo"];

        return navega(pagina, navId, parametros);
    }

    public static Boolean navega(Page pagina, string navId, string[] parametros)
    {
        string url = NavTo(navId);

        if (url.Equals(String.Empty))
        {
            return false;
        }

        for (int i = 0; i < parametros.Count(); i++)
        {
            url = url.Replace("{" + i + "}", Encriptador.encriptar(parametros[i]));
        }

        pagina.Response.Redirect(url);

        return true;
    }

    public static string vinculoNavegacion(string navId, string[] parametros)
    {
        string url = NavTo(navId);

        for (int i = 0; i < parametros.Count(); i++)
        {
            url = url.Replace("{" + i + "}", parametros[i]);
        }

        return url;
    }
}