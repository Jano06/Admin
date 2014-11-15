using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data;
using log4net;

/// <summary>
/// Descripción breve de CancelaCompraAction
/// </summary>
public class CancelaCompraAction
{
    private static readonly ILog _LOGGER = LogManager.GetLogger(typeof(CancelaCompraAction));

    public List<CancelaCompraModel> buscaCompra(string referencia)
    {
        List<CancelaCompraModel> lista = new List<CancelaCompraModel>();

        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();

        try
        {
            string strQuery = "SELECT COMPRAS.ID, COMPRAS.REFERENCIA, ASOCIADOS.ID AS IDASOCIADO, ASOCIADOS.NOMBRE, ASOCIADOS.APPATERNO, ASOCIADOS.APMATERNO, "
                    + "COMPRAS.FECHAORDEN, COMPRAS.TOTAL, COMPRAS.STATUSPAGO, COMPRAS.STATUSENTREGA FROM COMPRAS, ASOCIADOS "
                    + "WHERE COMPRAS.ASOCIADO=ASOCIADOS.ID AND REFERENCIA=" + referencia + " "
                    + "ORDER BY COMPRAS.ID DESC";
            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            MySqlDataReader reader = queryEx.ExecuteReader();
            while (reader.Read())
            {
                CancelaCompraModel model = new CancelaCompraModel();
                model.IdCompra = reader["ID"].ToString();
                model.Referencia = reader["REFERENCIA"].ToString();
                model.Asociado = reader["IDASOCIADO"].ToString() + " " + reader["NOMBRE"].ToString() + " " + reader["APPATERNO"].ToString() + " " + reader["APMATERNO"].ToString();
                model.FechaOrden = Convert.ToDateTime(reader["FECHAORDEN"].ToString()).ToString("dd/MMMM/yyyy").ToUpper();
                model.Monto = Convert.ToDouble(reader["TOTAL"].ToString()).ToString("c2");
                model.StatusPago = reader["STATUSPAGO"].ToString();
                model.StatusEntrega = reader["STATUSENTREGA"].ToString();
                lista.Add(model);
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al BuscaCompra: " + ex.Message);
            throw new Exception("Error CancelaCompraAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
        return lista;
    }

    public string cancelaCompra(string idCompra)
    {
        string mensaje = string.Empty;
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();
        try
        {
            string strQuery = "UPDATE COMPRAS SET STATUSPAGO='CANCELADO', STATUSENTREGA='CANCELADO', FECHA=NULL WHERE ID="+idCompra;
            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            queryEx.ExecuteNonQuery();
            mensaje = "Compra: " + idCompra + " Cancelada";
        }
        catch (Exception ex)
        {
            mensaje = "Error: " + ex.Message;
        }
        finally
        {
            mySqlConn.Close();
            mySqlConn.Dispose();
            con.closeConection();
        }
        return mensaje;
    }


}