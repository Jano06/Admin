using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de ValidaCompraModel
/// </summary>
public class ValidaCompraModel
{
	private DateTime fecha;
    private int referencia;
    private double importe;

    public DateTime Fecha { get { return fecha; } set { fecha = value; } }
    public int Referencia { get { return referencia; } set { referencia = value; } }
    public double Importe { get { return importe; } set { importe = value; } }
}