using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data;
using log4net;

/// <summary>
/// Descripción breve de LoginAction
/// </summary>
public class LoginAction
{
    private static readonly ILog _LOGGER = LogManager.GetLogger(typeof(LoginAction));

    public LoginModel ConsAdmin(string admin, string contrasena)
    {
        LoginModel model = new LoginModel();
        Conection conn = new Conection();
        MySqlConnection mySqlConn = conn.conectBDPackage();
        try
        {
            string strQuery = "SELECT IDADMIN, ADMIN FROM ADMINUSERS WHERE ADMIN='" + admin + "' "
                + "AND CONTRASENA='" + contrasena + "' AND STATUS=1 ";
            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            MySqlDataReader reader = queryEx.ExecuteReader();
            while (reader.Read())
            {
                model.IdAdmin = int.Parse(reader["IDADMIN"].ToString());
                model.Admin = reader["ADMIN"].ToString();
            }
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al consultar el usuario." + admin + ex.Message);// mensaje de error del usuario
            throw new Exception("ConsultUser Error: " + ex.Message);
        }
        finally
        {
            mySqlConn.Close();
            mySqlConn.Dispose();
            conn.closeConection();
        }
        return model;
    }
}