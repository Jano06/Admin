using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de ValidaCompraUnitModel
/// </summary>
public class ValidaCompraUnitModel
{
    private string idCompra;
    private string referencia;
    private string asociado;
    private string fechaOrden;
    private string monto;
    private string statusPago;

    public string IdCompra { get { return idCompra; } set { idCompra = value; } }
    public string Referencia { get { return referencia; } set { referencia = value; } }
    public string Asociado { get { return asociado; } set { asociado = value; } }
    public string FechaOrden { get { return fechaOrden; } set { fechaOrden = value; } }
    public string Monto { get { return monto; } set { monto = value; } }
    public string StatusPago { get { return statusPago; } set { statusPago = value; } }
}