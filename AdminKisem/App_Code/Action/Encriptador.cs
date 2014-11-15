using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.IO;

/// <summary>
/// Descripción breve de Encriptador
/// </summary>
public class Encriptador
{
    ///IV
    ///constant vector containing the encryption algorithm        
    private const string IV = "3aU@|=mQ";

    ///Key
    ///constant vector containing the key to encrypt / decrypt        
    private const string key = "R%vz2°1<\'+4GhE8~@#4cTs2";

    public static string encriptar(string cadenaOriginal)
    {
        if (cadenaOriginal.Equals(String.Empty))
        {
            return cadenaOriginal;
        }

        byte[] mensaje = UTF8Encoding.UTF8.GetBytes(cadenaOriginal);

        TripleDESCryptoServiceProvider criptoProvider = new TripleDESCryptoServiceProvider();

        ICryptoTransform criptoTransform = criptoProvider.CreateEncryptor(UTF8Encoding.UTF8.GetBytes(key), UTF8Encoding.UTF8.GetBytes(IV));

        MemoryStream memoryStream = new MemoryStream();

        CryptoStream cryptoStream = new CryptoStream(memoryStream, criptoTransform, CryptoStreamMode.Write);

        cryptoStream.Write(mensaje, 0, mensaje.Length);
        cryptoStream.FlushFinalBlock();

        byte[] encriptado = memoryStream.ToArray();

        return Convert.ToBase64String(encriptado);
    }

    public static string desencriptar(string cadenaEncriptada) //Leer cadena de conexion desde archivo de configuración web
    {
        if (cadenaEncriptada.Equals(String.Empty)) //Si esta vacía
        {
            return cadenaEncriptada;
        }

        cadenaEncriptada = cadenaEncriptada.Replace(" ", "+"); //Remplazar los espacios en blanco por el signo +

        byte[] mensaje = Convert.FromBase64String(cadenaEncriptada);//Convierte la cadena especificada, que codifica los datos binarios como dígitos en base 64, en una matriz equivalente de enteros de 8 bits sin signo.

        TripleDESCryptoServiceProvider cryptoProvider = new TripleDESCryptoServiceProvider();//Define un objeto contenedor para obtener acceso a la versión del proveedor de servicios criptográficos (CSP) del algoritmo TripleDES. Esta clase no puede heredarse.

        ICryptoTransform cryptoTransform = cryptoProvider.CreateDecryptor(UTF8Encoding.UTF8.GetBytes(key), UTF8Encoding.UTF8.GetBytes(IV));//Desencripta la clave

        MemoryStream memoryStream = new MemoryStream(mensaje);//Crea una secuencia cuya memoria auxiliar es la memoria.

        CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read);//Define una secuencia que vincula los flujos de datos a las transformaciones criptográficas.

        StreamReader sr = new StreamReader(cryptoStream, true);//Implementa un TextReader que lee los caracteres de una secuencia de bytes en una codificación determinada.

        string cadenaOriginal = sr.ReadToEnd();

        return cadenaOriginal;
    }
}