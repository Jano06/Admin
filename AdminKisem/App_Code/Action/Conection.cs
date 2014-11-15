using System.Configuration;
using MySql.Data.MySqlClient;

/// <summary>
/// Descripción breve de Conection
/// </summary>
public class Conection
{
    private MySqlConnection _Connection;

    public MySqlConnection conectBDPackage()
    {
        string cadenaConexionEncriptada = ConfigurationManager.ConnectionStrings["conexionKisem"].ToString(); //Toma la cadena de conexión ubicada en el WebConfig
        string cadenaConexion = Encriptador.desencriptar(cadenaConexionEncriptada); //Cadena para desencriptar la cadena conexión
        _Connection = new MySqlConnection(cadenaConexion);
        return _Connection;
    }

    public void closeConection()
    {
        if (_Connection != null)
        {
            _Connection.Close();
        }
    }
}
