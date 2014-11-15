using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data;
using log4net;

/// <summary>
/// Descripción breve de ValidaComprasAction
/// </summary>
public class ValidaComprasAction
{
    private static readonly ILog _LOGGER = LogManager.GetLogger(typeof(ValidaComprasAction));

    public bool ValidaDatos(ValidaCompraModel Compra)
    {
        bool resp = false;
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();

        try
        {
            string strQuery = "SELECT TOTAL, FECHAORDEN FROM COMPRAS WHERE REFERENCIA=" + Compra.Referencia;
            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            MySqlDataReader reader = queryEx.ExecuteReader();
            while (reader.Read())
            {
                double total = Convert.ToDouble(reader["TOTAL"].ToString());
                DateTime fechaOrden = Convert.ToDateTime(reader["FECHAORDEN"].ToString());
                if (Compra.Importe.Equals(total) && Compra.Fecha >= fechaOrden)
                { resp = true; }
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al ValidarDatos: " + ex.Message);
            throw new Exception("Error ValidaComprasAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
        return resp;
    }

    public bool ValidaCompra(ValidaCompraModel Compra)
    {
        bool resp = false;
        string status = string.Empty;
        string fecha = string.Empty;
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();
        try
        {
            string strQuery = "SELECT STATUSPAGO, FECHA FROM COMPRAS WHERE REFERENCIA=" + Compra.Referencia;
            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            MySqlDataReader reader = queryEx.ExecuteReader();
            while (reader.Read())
            {
                status = reader["STATUSPAGO"].ToString();
                if (status != "PAGADO")
                {
                    resp = true;
                }
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al ValidaCompra: " + ex.Message);
            throw new Exception("Error ValidaComprasAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
        return resp;
    }

    public void RevisaCompra(int referencia)
    {
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();

        try
        {
            string strQuery = "SELECT ID, ASOCIADO, FECHAORDEN, ACTIVACION, EXCEDENTE, PUNTOS, INSCRIPCION, INICIOACTIVACION, FINACTIVACION "
                    + "FROM COMPRAS WHERE REFERENCIA=" + referencia;

            int idAsociado = 0, activacion = 0, excedente = 0, puntos = 0, inscripcion = 0, idCompra = 0;
            DateTime inicioActivac = new DateTime(), finactivacion = new DateTime(), fechaOrden = new DateTime();
            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            MySqlDataReader reader = queryEx.ExecuteReader();
            while (reader.Read())
            {
                idCompra = Convert.ToInt32(reader["ID"].ToString());
                idAsociado = Convert.ToInt32(reader["ASOCIADO"].ToString());
                fechaOrden = Convert.ToDateTime(reader["FECHAORDEN"].ToString());
                activacion = Convert.ToInt32(reader["ACTIVACION"].ToString());
                excedente = Convert.ToInt32(reader["EXCEDENTE"].ToString());
                puntos = Convert.ToInt32(reader["PUNTOS"].ToString());
                if (reader["INSCRIPCION"] != DBNull.Value)
                    inscripcion = Convert.ToInt32(reader["INSCRIPCION"].ToString());
                if (reader["INICIOACTIVACION"] != DBNull.Value)
                    inicioActivac = Convert.ToDateTime(reader["INICIOACTIVACION"].ToString());
                if (reader["FINACTIVACION"] != DBNull.Value)
                    finactivacion = Convert.ToDateTime(reader["FINACTIVACION"].ToString());
            }
            reader.Close();
            mySqlConn.Dispose();
            mySqlConn.Close();

            //Sacar el rango de la semana en curso
            SemanaCurso(fechaOrden);

            if (inscripcion > 0)
            {
                return;
            }
            
            int activo = StatusAsociado(idAsociado);
            if (activacion.Equals(1)) //Si la compra es de activación
            {
                if (activo.Equals(1)) //Si esta activo, cambiar a Excedente
                {
                    if (finactivacion <= DateTime.Today)
                    {
                        mySqlConn.Open();
                        strQuery = "UPDATE COMPRAS SET ACTIVACION=0, EXCEDENTE=1, INICIOACTIVACION=NULL, FINACTIVACION=NULL "
                            + "WHERE REFERENCIA=" + referencia;
                        queryEx = new MySqlCommand(strQuery, mySqlConn);
                        queryEx.ExecuteNonQuery();
                    }
                }
                else
                {
                    if (finactivacion < DateTime.Today) //Para cambiar fechas de activación
                    {
                        DateTime fechaInicio = new DateTime();
                        fechaInicio = DateTime.Today;//Nueva fecha inicio
                        DateTime fechaFin = new DateTime();
                        fechaFin = SigActivacion(idAsociado, fechaInicio);//Nuevas fecha fin
                        mySqlConn.Open();
                        strQuery = "UPDATE COMPRAS SET ACTIVACION=1, EXCEDENTE=0, INICIOACTIVACION='" + fechaInicio.ToString("yyyy/MM/dd") + "', FINACTIVACION='" + fechaFin.ToString("yyyy/MM/dd") + "' "
                            + "WHERE REFERENCIA=" + referencia;
                        queryEx = new MySqlCommand(strQuery, mySqlConn);
                        queryEx.ExecuteNonQuery();
                    }
                }
            }
            else if (excedente.Equals(1)) //Si la compra es excedente
            {
                if (activo.Equals(0)) //Revisar si está inactivo... Si esta inactivo entonces cambiar la compra como activación
                {
                    DateTime fechaInicio = new DateTime();
                    fechaInicio = DateTime.Today;
                    DateTime fechaFin = new DateTime();
                    fechaFin = SigActivacion(idAsociado, fechaInicio);
                    mySqlConn.Open();
                    strQuery = "UPDATE COMPRAS SET ACTIVACION=1, EXCEDENTE=0, INICIOACTIVACION='" + fechaInicio.ToString("yyyy/MM/dd") + "', FINACTIVACION='" + fechaFin.ToString("yyyy/MM/dd") + "' "
                        + "WHERE REFERENCIA=" + referencia;
                    queryEx = new MySqlCommand(strQuery, mySqlConn);
                    queryEx.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al RevisaCompra: " + ex.Message);
            throw new Exception("Error ValidaComprasAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
    }

    private DateTime SemanaCurso(DateTime fechaOrden)
    {
        DateTime rango = new DateTime();
        DateTime hoy = DateTime.Today;
        DateTime desde = new DateTime(), hasta = new DateTime();

        for (int i = 0; i < 8; i++)
        {
            hoy = hoy.AddDays(-1);
            if (hoy.DayOfWeek == DayOfWeek.Friday)
            {
                desde = hoy.AddDays(-6);
                hasta = hoy.AddDays(7);
                break;
            }
        }
        //fechaOrden.Date >= desde.Date
        if ((fechaOrden.Date <= hasta.Date) && (fechaOrden.Date >= desde.Date))
        {
            rango = fechaOrden;
        }
        else
        {
            rango = DateTime.Today;
        }

        return rango;
    }

    public List<int> ConsultaDatos(int referencia)
    {
        List<int> datos = new List<int>();
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();

        try
        {
            string strQuery = "SELECT ID, ASOCIADO, INSCRIPCION, PUNTOS "
                    + "FROM COMPRAS WHERE REFERENCIA=" + referencia;

            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            MySqlDataReader reader = queryEx.ExecuteReader();
            while (reader.Read())
            {
                datos.Add(Convert.ToInt32(reader["ID"].ToString()));
                datos.Add(Convert.ToInt32(reader["ASOCIADO"].ToString()));
                if (reader["INSCRIPCION"] != DBNull.Value)
                    datos.Add(Convert.ToInt32(reader["INSCRIPCION"].ToString()));
                else
                    datos.Add(0);
                datos.Add(Convert.ToInt32(reader["PUNTOS"].ToString()));
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al ConsultaDatos: " + ex.Message);
            throw new Exception("Error ValidaComprasAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
        return datos;
    }

    public AsociadosModel DatosProspecto(int idProspecto)
    {
        AsociadosModel DatosAsociado = new AsociadosModel();
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();

        try
        {
            string strQuery = "SELECT NOMBRE, APPATERNO, APMATERNO, FNAC, LUGARNACIM, ESTADOCIVIL, RFC, CURP, COMPANIA, "
                + "TELLOCAL, TELMOVIL, NEXTEL, EMAIL, PAIS, IDIOMA, CALLECASA, NUMCASA, INTCASA, COLCASA, CPCASA, MUNICIPIOCASA, "
                + "CIUDADCASA, ESTADOCASA, OBSERVCASA, CALLEPAQ, NUMPAQ, INTPAQ, COLPAQ, CPPAQ, MUNICIPIOPAQ, CIUDADPAQ, "
                + "ESTADOPAQ, OBSERVPAQ, TIPO, PATROCINADOR, PADRE, LADO FROM PROSPECTOS WHERE ID=" + idProspecto;

            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            MySqlDataReader reader = queryEx.ExecuteReader();
            while (reader.Read())
            {
                DatosAsociado.Nombre = reader["NOMBRE"].ToString();
                DatosAsociado.ApPaterno = reader["APPATERNO"].ToString();
                DatosAsociado.ApMaterno = reader["APMATERNO"].ToString();
                DatosAsociado.FechaNacim = Convert.ToDateTime(reader["FNAC"].ToString());
                DatosAsociado.LugarNacim = reader["LUGARNACIM"].ToString();
                DatosAsociado.EdoCivil = reader["ESTADOCIVIL"].ToString();
                DatosAsociado.RFC = reader["RFC"].ToString();
                DatosAsociado.Curp = reader["CURP"].ToString();
                DatosAsociado.Compania = reader["COMPANIA"].ToString();
                DatosAsociado.Telefono = reader["TELLOCAL"].ToString();
                DatosAsociado.Celular = reader["TELMOVIL"].ToString();
                DatosAsociado.Otro = reader["NEXTEL"].ToString();
                DatosAsociado.Email = reader["EMAIL"].ToString();
                DatosAsociado.Pais = reader["PAIS"].ToString();
                DatosAsociado.Idioma = reader["IDIOMA"].ToString();
                DatosAsociado.CalleCasa = reader["CALLECASA"].ToString();
                DatosAsociado.NumCasa = reader["NUMCASA"].ToString();
                DatosAsociado.IntCasa = reader["INTCASA"].ToString();
                DatosAsociado.ColoniaCasa = reader["COLCASA"].ToString();
                DatosAsociado.CpCasa = reader["CPCASA"].ToString();
                DatosAsociado.MunicipioCasa = reader["MUNICIPIOCASA"].ToString();
                DatosAsociado.CiudadCasa = reader["CIUDADCASA"].ToString();
                DatosAsociado.EdoCasa = reader["ESTADOCASA"].ToString();
                DatosAsociado.ObservCasa = reader["OBSERVCASA"].ToString();
                DatosAsociado.CallePaq = reader["CALLEPAQ"].ToString();
                DatosAsociado.NumPaq = reader["NUMPAQ"].ToString();
                DatosAsociado.IntPaq = reader["INTPAQ"].ToString();
                DatosAsociado.ColoniaPaq = reader["COLPAQ"].ToString();
                DatosAsociado.CpPaq = reader["CPPAQ"].ToString();
                DatosAsociado.MunicipioPaq = reader["MUNICIPIOPAQ"].ToString();
                DatosAsociado.CiudadPaq = reader["CIUDADPAQ"].ToString();
                DatosAsociado.EdoPaq = reader["ESTADOPAQ"].ToString();
                DatosAsociado.ObservPaq = reader["OBSERVPAQ"].ToString();
                DatosAsociado.Tipo = reader["TIPO"].ToString();
                DatosAsociado.Padre = reader["PADRE"].ToString();
                DatosAsociado.Lado = reader["LADO"].ToString();
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al DatosProspecto: " + ex.Message);
            throw new Exception("Error ValidaComprasAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
        return DatosAsociado;
    }

    public string validaPadre(string padre, string lado)
    {
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();

        try
        {
            string strQuery = "SELECT ID FROM ASOCIADOS WHERE PADRE=" + padre + " AND LADO='" + lado + "'";

            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            MySqlDataReader reader = queryEx.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    padre = validaPadre(reader["ID"].ToString(), lado);
                }
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al validaPadre: " + ex.Message);
            throw new Exception("Error ValidaComprasAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
        return padre;
    }

    private int StatusAsociado(int idAsociado)
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
            _LOGGER.Error("Error al StatusAsociado: " + ex.Message);
            throw new Exception("Error ValidaComprasAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
        return status;
    }

    public void EliminaProspecto(int idProspecto)
    {
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();

        try
        {
            string strQuery = "DELETE FROM PROSPECTOS WHERE ID=" + idProspecto;
            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            queryEx.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al EliminaProspecto: " + ex.Message);
            throw new Exception("Error ValidaComprasAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
    }

    public bool actualizaPago(int idAsociado, DateTime fechaPago, int idCompra)
    {
        bool res = false;
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();

        try
        {
            string strQuery = "UPDATE COMPRAS SET STATUSPAGO='PAGADO', FECHA='" + fechaPago.ToString("yyyy/MM/dd") + "' WHERE ASOCIADO=" + idAsociado + " AND ID=" + idCompra;
            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            queryEx.ExecuteNonQuery();
            res = true;
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al actualizaPago: " + ex.Message);
            throw new Exception("Error ValidaComprasAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
        return res;
    }

    public List<string> ConsInfoCompra(int idCompra)
    {
        List<string> lista = new List<string>();
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();

        try
        {
            string strQuery = "SELECT ACTIVACION, INICIOACTIVACION, FINACTIVACION FROM COMPRAS WHERE ID=" + idCompra;

            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            MySqlDataReader reader = queryEx.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(reader["ACTIVACION"].ToString());
                lista.Add(reader["INICIOACTIVACION"].ToString());
                lista.Add(reader["FINACTIVACION"].ToString());
            }

            reader.Close();
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al ConsInfoCompra: " + ex.Message);
            throw new Exception("Error ValidaComprasAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
        return lista;
    }

    public void actualizaAsociado(DateTime finActivacion, int idAsociado, int idCompra, int puntos)
    {
        //string mensaje = string.Empty;
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();
        try
        {
            string strQuery = "UPDATE ASOCIADOS "
                + "SET STATUS=1, FINACTIVACION='" + finActivacion.ToString("yyyy/MM/dd") + "', PTSMES=" + puntos + " "
                + "WHERE ID=" + idAsociado;
            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            queryEx.ExecuteNonQuery();
            //mensaje = "Compra: " + idCompra.ToString() + " Activacion";
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al actualizaAsociado: " + ex.Message);
            throw new Exception("Error ValidaComprasAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
        //return mensaje;
    }

    public void actualizaAsociado(DateTime finActivacion, DateTime inicioActivacion, int idAsociado, int idCompra, int puntos)
    {
        //string mensaje = string.Empty;
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();
        try
        {
            string strQuery = "UPDATE ASOCIADOS "
                + "SET STATUS=1, INICIOACTIVACION='" + inicioActivacion.ToString("yyyy/MM/dd") + "', "
                + "FINACTIVACION='" + finActivacion.ToString("yyyy/MM/dd") + "', PTSMES=" + puntos + " "
                + "WHERE ID=" + idAsociado;
            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            queryEx.ExecuteNonQuery();
            //mensaje = "Compra: " + idCompra.ToString() + " Activacion";
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al actualizaAsociado: " + ex.Message);
            throw new Exception("Error ValidaComprasAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
        //return mensaje;
    }

    public DateTime ConsFinActivacionAsociado(int idAsociado)
    {
        DateTime finActivacion = new DateTime();
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();

        try
        {
            string strQuery = "SELECT FINACTIVACION FROM ASOCIADOS WHERE ID=" + idAsociado;

            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            MySqlDataReader reader = queryEx.ExecuteReader();

            while (reader.Read())
            {
                finActivacion = Convert.ToDateTime(reader["FINACTIVACION"].ToString());
            }

            reader.Close();
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al ConsFinActivacionAsociado: " + ex.Message);
            throw new Exception("Error ValidaComprasAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
        return finActivacion;
    }

    public void CambiaIdAsociadoEnCompra(int idAsociado, int idCompra)
    {
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();

        try
        {
            string strQuery = "UPDATE COMPRAS SET ASOCIADO=" + idAsociado + " WHERE ID=" + idCompra;
            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            queryEx.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al CambiaIdAsociadoEnCompra: " + ex.Message);
            throw new Exception("Error ValidaComprasAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
    }

    public void insertaAsociado(AsociadosModel asociado)
    {
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();

        try
        {
            string strQuery = "INSERT INTO ASOCIADOS(ID, NOMBRE, APPATERNO, APMATERNO, FNAC, LUGARNACIM, "
                + "ESTADOCIVIL, RFC, CURP, COMPANIA, TELLOCAL, TELMOVIL, NEXTEL, EMAIL, ALIAS, PASSWORD, "
                + "PAIS, IDIOMA, CALLECASA, NUMCASA, INTCASA, COLCASA, CPCASA, MUNICIPIOCASA, "
                + "CIUDADCASA, ESTADOCASA, OBSERVCASA, CALLEPAQ, NUMPAQ, INTPAQ, COLPAQ, CPPAQ, MUNICIPIOPAQ, "
                + "CIUDADPAQ, ESTADOPAQ, OBSERVPAQ, TIPO, FINSC, PATROCINADOR, PADRE, LADO, ORDEN, "
                + "RANGO, STATUS, PTSMES, BONO6, RANGOPAGO, HISTORIA, RECORRIDO, LADOSRECORRIDO, "
                + "LADOPATROCINADOR, NIVEL, BODEGA, INICIOACTIVACION, FINACTIVACION, FACTURA) "
                + "VALUES (" + asociado.IdAsociado + ", '" + asociado.Nombre + "', '" + asociado.ApPaterno + "', '" + asociado.ApMaterno + "', '" + asociado.FechaNacim.ToString("yyyy/MM/dd") + "', '" + asociado.LugarNacim + "', "
                + "'" + asociado.EdoCivil + "', '" + asociado.RFC + "', '" + asociado.Curp + "', '" + asociado.Compania + "', '" + asociado.Telefono + "', '" + asociado.Celular + "', '" + asociado.Otro + "', '" + asociado.Email + "', '" + asociado.Alias + "', '" + asociado.Contrasena + "',"
                + "'" + asociado.Pais + "', '" + asociado.Idioma + "', '" + asociado.CalleCasa + "', '" + asociado.NumCasa + "', '" + asociado.IntCasa + "', '" + asociado.ColoniaCasa + "', '" + asociado.CpCasa + "', '" + asociado.MunicipioCasa + "', "
                + "'" + asociado.CiudadCasa + "', '" + asociado.EdoCasa + "', '" + asociado.ObservCasa + "', '" + asociado.CallePaq + "', '" + asociado.NumPaq + "', '" + asociado.IntPaq + "', '" + asociado.ColoniaPaq + "', '" + asociado.CpPaq + "', '" + asociado.MunicipioPaq + "',"
                + "'" + asociado.CiudadPaq + "', '" + asociado.EdoPaq + "', '" + asociado.ObservPaq + "', " + asociado.Tipo + ", '" + asociado.FechaInscripcion.ToString("yyyy/MM/dd") + "', " + asociado.Patrocinador + ", " + asociado.Padre + ", '" + asociado.Lado + "', " + asociado.Orden + ", "
                + asociado.Rango + ", " + asociado.Status + ", " + asociado.PtsMes + ", 0, " + asociado.RangoPago + ", '" + asociado.Historia + "', '" + asociado.Recorrido + "', '" + asociado.LadosRecorrido + "', "
                + "'" + asociado.LadoPatrocinador + "', " + asociado.Nivel + ", " + asociado.Bodega + ", '" + asociado.InicioActivacion.ToString("yyyy/MM/dd") + "', '" + asociado.FinActivacion.ToString("yyyy/MM/dd") + "', 0) ";
            
            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            queryEx.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al InsertaAsociado: " + ex.Message);
            throw new Exception("Error ValidaComprasAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
    }

    public int MaxAsoc()
    {
        int max = 0;
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();

        try
        {
            string strQuery = "SELECT MAX(ID) AS MAX FROM ASOCIADOS";

            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            MySqlDataReader reader = queryEx.ExecuteReader();
            while (reader.Read())
            {
                max = Convert.ToInt32(reader["MAX"].ToString());
            }
            reader.Close();
            max++;
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error en MaxAsoc: " + ex.Message);
            throw new Exception("Error ValidaComprasAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
        return max;
    }

    public List<string> InfoPadre(int idPadre, string lado)
    {
        List<string> lista = new List<string>();
        string recorrid = string.Empty, recorridLados = string.Empty;
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();

        try
        {
            string strQuery = "SELECT RECORRIDO, LADOSRECORRIDO FROM ASOCIADOS WHERE ID=" + idPadre;

            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            MySqlDataReader reader = queryEx.ExecuteReader();
            while (reader.Read())
            {
                recorrid = reader["RECORRIDO"].ToString();
                recorridLados = reader["LADOSRECORRIDO"].ToString();
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al InfoPadre: " + ex.Message);
            throw new Exception("Error ValidaComprasAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }

        recorrid += idPadre + ".";
        lista.Add(recorrid);
        recorridLados += lado + ".";
        lista.Add(recorridLados);

        return lista;
    }

    public List<string> InfoPatroc(int idPatrocinador)
    {
        string historia = string.Empty;
        List<string> infoPatroc = new List<string>();
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();

        try
        {
            string strQuery = "SELECT HISTORIA, BODEGA FROM ASOCIADOS WHERE ID=" + idPatrocinador;

            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            MySqlDataReader reader = queryEx.ExecuteReader();
            while (reader.Read())
            {
                historia = reader["HISTORIA"].ToString();
                infoPatroc.Add(historia + idPatrocinador + ".");
                infoPatroc.Add(reader["BODEGA"].ToString());
            }
            reader.Close();
            mySqlConn.Dispose();
            mySqlConn.Close();

            strQuery = "SELECT MAX(ORDEN) AS MAX FROM ASOCIADOS WHERE PATROCINADOR=" + idPatrocinador;

            queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            reader = queryEx.ExecuteReader();
            while (reader.Read())
            {
                int max = Convert.ToInt32(reader["MAX"].ToString());
                infoPatroc.Add((max + 1).ToString());
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al InfoPatroc: " + ex.Message);
            throw new Exception("Error ValidaComprasAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
        
        return infoPatroc;
    }

    private DateTime SigActivacion(int idAsociado, DateTime fechaInicio)
    {
        DateTime fecha = new DateTime();
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
                fecha = Convert.ToDateTime(reader["FINSC"].ToString());
            }
            reader.Close();
            fecha = ValidaFecha(Convert.ToDateTime(fecha.Day + "/" + fechaInicio.Month + "/" + fechaInicio.Year));
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al SigActivacion: " + ex.Message);
            throw new Exception("Error ValidaComprasAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
        return fecha;
    }

    private DateTime ValidaFecha(DateTime inicioActivacion)
    {
        DateTime finActivacion = new DateTime();

        try
        {
            int mes = inicioActivacion.Month;
            int anio = inicioActivacion.Year;
            if (inicioActivacion.AddMonths(1).Month > 12)
            {
                if (inicioActivacion.AddMonths(1).AddDays(-1).Day < 1)
                {
                    finActivacion = Convert.ToDateTime(DateTime.DaysInMonth(anio, 12) + "/" + mes + "/" + anio);
                }
                else
                {
                    finActivacion = Convert.ToDateTime(inicioActivacion.AddDays(-1) + "/01/" + (anio + 1));
                }
            }
            else
            {
                if (inicioActivacion.AddMonths(1).AddDays(-1).Day < 1)
                {
                    finActivacion = Convert.ToDateTime(DateTime.DaysInMonth(anio, mes) + "/" + mes + "/" + anio);
                }
                else
                {
                    finActivacion = inicioActivacion.AddMonths(1).AddDays(-1);
                }
            }
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al ValidaFecha: " + ex.Message);
            throw new Exception("Error ValidaComprasAction: " + ex.Message);
        }
        return finActivacion;
    }

    
}