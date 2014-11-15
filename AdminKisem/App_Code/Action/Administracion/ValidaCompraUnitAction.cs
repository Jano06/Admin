using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data;
using log4net;

/// <summary>
/// Descripción breve de ValidaCompraUnitAction
/// </summary>
public class ValidaCompraUnitAction
{
    private static readonly ILog _LOGGER = LogManager.GetLogger(typeof(ValidaCompraUnitAction));

    public List<ValidaCompraUnitModel> ConsCompras()
    {
        List<ValidaCompraUnitModel> lista = new List<ValidaCompraUnitModel>();
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();

        try
        {
            string strQuery = "SELECT COMPRAS.ID, COMPRAS.REFERENCIA, ASOCIADOS.ID AS IDASOCIADO, ASOCIADOS.NOMBRE, ASOCIADOS.APPATERNO, ASOCIADOS.APMATERNO, "
                    + "COMPRAS.FECHAORDEN, COMPRAS.TOTAL, COMPRAS.STATUSPAGO FROM COMPRAS, ASOCIADOS "
                    + "WHERE COMPRAS.ASOCIADO=ASOCIADOS.ID AND STATUSPAGO='PENDIENTE' "
                    + "ORDER BY COMPRAS.ID DESC";
            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            MySqlDataReader reader = queryEx.ExecuteReader();
            while (reader.Read())
            {
                ValidaCompraUnitModel model = new ValidaCompraUnitModel();
                model.IdCompra = reader["ID"].ToString();
                model.Referencia = reader["REFERENCIA"].ToString();
                model.Asociado = reader["IDASOCIADO"].ToString() + " " + reader["NOMBRE"].ToString() + " " + reader["APPATERNO"].ToString() + " " + reader["APMATERNO"].ToString();
                model.FechaOrden = Convert.ToDateTime(reader["FECHAORDEN"].ToString()).ToString("dd/MMMM/yyyy").ToUpper();
                model.Monto = Convert.ToDouble(reader["TOTAL"].ToString()).ToString("n2");
                model.StatusPago = reader["STATUSPAGO"].ToString();
                lista.Add(model);
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al ConsCompras: " + ex.Message);
            throw new Exception("Error ValidaCompraUnitAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
        return lista;
    }

    public List<ValidaCompraUnitModel> buscaCompra(string referencia)
    {
        List<ValidaCompraUnitModel> lista = new List<ValidaCompraUnitModel>();
        
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();

        try
        {
            string strQuery = "SELECT COMPRAS.ID, COMPRAS.REFERENCIA, ASOCIADOS.ID AS IDASOCIADO, ASOCIADOS.NOMBRE, ASOCIADOS.APPATERNO, ASOCIADOS.APMATERNO, "
                    + "COMPRAS.FECHAORDEN, COMPRAS.TOTAL, COMPRAS.STATUSPAGO FROM COMPRAS, ASOCIADOS "
                    + "WHERE COMPRAS.ASOCIADO=ASOCIADOS.ID AND REFERENCIA=" + referencia + " "
                    + "ORDER BY COMPRAS.ID DESC";
            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            MySqlDataReader reader = queryEx.ExecuteReader();
            while (reader.Read())
            {
                ValidaCompraUnitModel model = new ValidaCompraUnitModel();
                model.IdCompra = reader["ID"].ToString();
                model.Referencia = reader["REFERENCIA"].ToString();
                model.Asociado = reader["IDASOCIADO"].ToString() + " " + reader["NOMBRE"].ToString() + " " + reader["APPATERNO"].ToString() + " " + reader["APMATERNO"].ToString();
                model.FechaOrden = Convert.ToDateTime(reader["FECHAORDEN"].ToString()).ToString("dd/MMMM/yyyy").ToUpper();
                model.Monto = Convert.ToDouble(reader["TOTAL"].ToString()).ToString("c2");
                model.StatusPago = reader["STATUSPAGO"].ToString();
                lista.Add(model);
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al BuscaCompra: " + ex.Message);
            throw new Exception("Error ValidaCompraUnitAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
        return lista;
    }

}