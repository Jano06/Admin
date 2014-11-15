using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data;
using log4net;

/// <summary>
/// Descripción breve de SubePuntosAction
/// </summary>
public class SubePuntosAction
{
    private static readonly ILog _LOGGER = LogManager.GetLogger(typeof(SubePuntosAction));

    public void subePuntosExcedente(int idAsociado, int idCompra, int puntos)
    {
        SubePuntosModel model = new SubePuntosModel();
        int status = 0;
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();
        try
        {
            List<string> datosPadre = ConsultaPadre(idAsociado);
            model.IdPadre = Convert.ToInt32(datosPadre[0]);
            model.Lado = datosPadre[1];
            status = ConsultaStatusPadre(model.IdPadre);

            if (status.Equals(1))
            {
                string strQuery = "INSERT INTO PUNTOSASOCIADOS(ASOCIADO, COMPRA, LADO, PUNTOS, STATUS, PORPAGAR, CORTE, PORPAGARANTERIOR) "
                    + "VALUES(" + model.IdPadre + ", " + idCompra + ", '" + model.Lado + "', " + puntos + ", 0, " + puntos + ", 0, 0)";
                MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
                mySqlConn.Open();
                queryEx.ExecuteNonQuery();
                mySqlConn.Dispose();
                mySqlConn.Close();
            }
            if (!(idAsociado.Equals(11)))
            {
                if (!(model.IdPadre.Equals(11)))
                {
                    subePuntosExcedente(model.IdPadre, idCompra, puntos);
                }
            }
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al subePuntosExcedente: " + ex.Message);
            throw new Exception("Error SubePuntosAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
    }

    public void subePuntosActivacion(int idAsociado, int idCompra, int puntos, DateTime fechaInscr)
    {
        SubePuntosModel model = new SubePuntosModel();
        int status = 0;
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();
        try
        {
            List<string> datosPadre = ConsultaPadre(idAsociado);
            model.IdPadre = Convert.ToInt32(datosPadre[0]);
            model.Lado = datosPadre[1];
            model.PtsPadre = Convert.ToInt32(datosPadre[2]);

            status = ConsultaStatusPadre(model.IdPadre);

            if (status.Equals(1))
            {
                int ts = CalcularMesesDeDiferencia(fechaInscr, DateTime.Today);
                if (ts > 2)
                {
                    string strQuery = "INSERT INTO PUNTOSASOCIADOS(ASOCIADO, COMPRA, LADO, PUNTOS, STATUS, PORPAGAR, CORTE, PORPAGARANTERIOR) "
                        + "VALUES(" + model.IdPadre + ", " + idCompra + ", '" + model.Lado + "', " + puntos + ", 0, " + puntos + ", 0, 0)";
                    MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
                    mySqlConn.Open();
                    queryEx.ExecuteNonQuery();
                    mySqlConn.Dispose();
                    mySqlConn.Close();
                }
                else
                {
                    if (model.PtsPadre.Equals(1000))
                    {
                        string strQuery = "INSERT INTO PUNTOSASOCIADOS(ASOCIADO, COMPRA, LADO, PUNTOS, STATUS, PORPAGAR, CORTE, PORPAGARANTERIOR) "
                            + "VALUES(" + model.IdPadre + ", " + idCompra + ", '" + model.Lado + "', " + (puntos / 2) + ", 0, " + (puntos / 2) + ", 0, 0)";
                        MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
                        mySqlConn.Open();
                        queryEx.ExecuteNonQuery();
                        mySqlConn.Dispose();
                        mySqlConn.Close();
                    }
                }
            }
            if (!(model.IdPadre.Equals(11)))
            {
                subePuntosActivacion(model.IdPadre, idCompra, puntos, fechaInscr);
            }
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al subePuntosActivacion: " + ex.Message);
            throw new Exception("Error SubePuntosAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
    }

    public void subePuntosInscripcion(int idAsociado, int idCompra, int puntos)
    {
        SubePuntosModel model = new SubePuntosModel();
        int status = 0;
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();
        try
        {
            List<string> datosPadre = ConsultaPadre(idAsociado);
            model.IdPadre = Convert.ToInt32(datosPadre[0]);
            model.Lado = datosPadre[1];
            model.PtsPadre = Convert.ToInt32(datosPadre[2]);

            status = ConsultaStatusPadre(model.IdPadre);

            if (status.Equals(1))
            {
                if (model.PtsPadre.Equals(1000))
                {
                    string strQuery = "INSERT INTO PUNTOSASOCIADOS(ASOCIADO, COMPRA, LADO, PUNTOS, STATUS, PORPAGAR, CORTE, PORPAGARANTERIOR) "
                        + "VALUES(" + model.IdPadre + ", " + idCompra + ", '" + model.Lado + "', " + (puntos / 2) + ", 0, " + (puntos / 2) + ", 0, 0)";
                    MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
                    mySqlConn.Open();
                    queryEx.ExecuteNonQuery();
                    mySqlConn.Dispose();
                    mySqlConn.Close();
                }

            }
            if (!(model.IdPadre.Equals(11)))
            {
                subePuntosInscripcion(model.IdPadre, idCompra, puntos);
            }
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al subePuntosInscripcion: " + ex.Message);
            throw new Exception("Error SubePuntosAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
    }

    public int CalcularMesesDeDiferencia(DateTime fechaDesde, DateTime fechaHasta)
    {
        return Math.Abs((fechaDesde.Month - fechaHasta.Month) + 12 * (fechaDesde.Year - fechaHasta.Year));
    }

    private List<string> ConsultaPadre(int idAsociado)
    {
        List<string> lista = new List<string>();
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();
        try
        {
            string strQuery = "SELECT PADRE, LADO, PTSMES FROM ASOCIADOS WHERE ID=" + idAsociado;
            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            MySqlDataReader reader = queryEx.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(reader["PADRE"].ToString());
                lista.Add(reader["LADO"].ToString());
                lista.Add(reader["PTSMES"].ToString());
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al ConsultaPadre: " + ex.Message);
            throw new Exception("Error SubePuntosAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
        return lista;
    }

    private int ConsultaStatusPadre(int idAsociado)
    {
        int status = 0;
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();
        try
        {
            string strQuery = "SELECT STATUS FROM ASOCIADOS WHERE ID=" + idAsociado;
            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            MySqlDataReader reader = queryEx.ExecuteReader();
            while (reader.Read())
            {
                status = Convert.ToInt32(reader["STATUS"].ToString());
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al ConsultaStatusPadre: " + ex.Message);
            throw new Exception("Error SubePuntosAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
        return status;
    }

    private int ConsultaPuntosPadre(int idAsociado)
    {
        int puntos = 0;
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();
        try
        {
            string strQuery = "SELECT PTSMES FROM ASOCIADOS WHERE ID=" + idAsociado;
            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            MySqlDataReader reader = queryEx.ExecuteReader();
            while (reader.Read())
            {
                puntos = Convert.ToInt32(reader["STATUS"].ToString());
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al ConsultaStatusPadre: " + ex.Message);
            throw new Exception("Error SubePuntosAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
        return puntos;
    }

    public DateTime ConsultaFechaInscripcionAsociado(int idAsociado)
    {
        DateTime fechaInscr = new DateTime();
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();
        try
        {
            string strQuery = "SELECT FINSC FROM ASOCIADOS WHERE ID=" + idAsociado;
            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            MySqlDataReader reader = queryEx.ExecuteReader();
            while (reader.Read())
            {
                fechaInscr = Convert.ToDateTime(reader["FINSC"].ToString());
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al ConsultaFechaInscripcionAsociado: " + ex.Message);
            throw new Exception("Error SubePuntosAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
        return fechaInscr;
    }
}