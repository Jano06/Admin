using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de SubePuntosModel
/// </summary>
public class SubePuntosModel
{
    private int idPadre;
    private int ptsPadre;
    private int idCompra;
    private string lado;
    private int puntos;
    

    public int IdPadre { get { return idPadre; } set { idPadre = value; } }
    public int PtsPadre { get { return ptsPadre; } set { ptsPadre = value; } }
    public int IdCompra { get { return idCompra; } set { idCompra = value; } }
    public string Lado { get { return lado; } set { lado = value; } }
    public int Puntos { get { return puntos; } set { puntos = value; } }
}