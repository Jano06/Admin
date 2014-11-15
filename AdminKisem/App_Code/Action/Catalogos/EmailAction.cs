using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using MySql.Data.MySqlClient;
using log4net;
using System.Text.RegularExpressions;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Configuration;

/// <summary>
/// Descripción breve de EmailAction
/// </summary>
public class EmailAction
{
    private static readonly ILog _LOGGER = LogManager.GetLogger(typeof(EmailAction));

    public void CorreoBienvenida(AsociadosModel nuevoAsociado)
    {
        List<string> infoPatrocinador = new List<string>();
        string mensajeBienvenida = string.Empty;
        string nomPaquete = string.Empty;
        string aviso = string.Empty;
        string lado = string.Empty;

        infoPatrocinador = ConsinfoPatrocinador(nuevoAsociado.Patrocinador);
        mensajeBienvenida = ConsMensajeBienvenida();
        aviso = ConsAvisoPrivacidad();
        if (nuevoAsociado.Lado == "D")
            lado = "Derecho";
        else if (nuevoAsociado.Lado == "I")
            lado = "Izquierdo";

        string[] a = Regex.Split(mensajeBienvenida, "\r\n");
        StringBuilder htmBody = new StringBuilder();
        StringBuilder body = new StringBuilder();

        #region MensajeCompra
        body.Append("<table width='800px' style='text-align:justify; color: #000000; font-size: medium; font-family: Arial;'><tr><td>");
        body.Append("Estimado <span style='font-weight: bold;'>" + nuevoAsociado.Nombre + " " + nuevoAsociado.ApPaterno + " " + nuevoAsociado.ApMaterno + "</span>");
        body.Append("</tr></td>");

        for (int i = a.GetLowerBound(0); i <= a.GetUpperBound(0); i++)
        {
            if (a[i] == "")
            {
                body.Append("<tr><td></td></tr>");
            }
            else
            {
                string cadena = string.Empty;
                string caux = string.Empty;
                caux = a[i];
                body.Append("<tr><td>" + caux + "</td></tr>");
            }
        }
        #endregion
        #region CuerpoCorreo
        body.Append("</table>");
        body.Append("<br /><br /><br />");
        body.Append("<table width='600px' style='font-family: Arial; color: #000000; font-size: medium; font-weight: normal; margin-top: 0px; background-position: bottom; background-repeat: no-repeat;'>");
        body.Append("<tr>");
        body.Append("<td align='center' style='font-weight: bold;' colspan='2'>INFORMACIÓN PERSONAL</td>");
        body.Append("</tr>");
        body.Append("<tr>");
        body.Append("<td align='right'>Número Asociado:</td>");
        body.Append("<td><span style='font-weight: bold;'>" + nuevoAsociado.IdAsociado + "</span></td>");
        body.Append("</tr>");
        body.Append("<tr>");
        body.Append("<td width='150px' align='right'>Password:</td>");
        body.Append("<td><span style='font-weight: bold;'>" + nuevoAsociado.Contrasena + "</span></td>");
        body.Append("</tr>");
        body.Append("<tr>");
        body.Append("<td align='right'>Fecha de Ingreso:</td>");
        body.Append("<td><span style='font-weight: bold;'>" + nuevoAsociado.FechaInscripcion.ToString("dd/MM/yyyy") + "</span></td>");
        body.Append("</tr>");
        body.Append("<tr>");
        body.Append("<td align='right'>Colocación:</td>");
        body.Append("<td><span style='font-weight: bold;'>" + lado + "</span></td>");
        body.Append("</tr>");
        body.Append("<tr>");
        body.Append("<td align='right'>No. Patrocinador:</td>");
        body.Append("<td><span style='font-weight: bold;'>" + infoPatrocinador[0] + "</span></td>");
        body.Append("</tr>");
        body.Append("<tr>");
        body.Append("<td align='right'>Nombre Patrocinador:</td>");
        body.Append("<td><span style='font-weight:bold;'>" + infoPatrocinador[1] + "</span></td>");
        body.Append("</tr>");
        body.Append("</table>");
        body.Append("<br /><br />");
        #endregion
        #region pieCorreo
        body.Append("<span style='font-weight:bold; font-size:18px; font-style:italic; color: #000000'>'Somos lo que hacemos día a día. De modo que la excelencia no es un acto, sino un hábito.' </span><br/>");
        body.Append("<span style='font-weight:bold; font-size:18px;'>Aristóteles</span><br/>");
        body.Append("<br/><br />");
        body.Append("<table width='800px' style='text-align:justify; color: #A4A4A4; font-size: small; font-family: Arial;'><tr><td>");
        body.Append("</tr></td>");
        a = Regex.Split(aviso, "\r\n");
        body.Append("<tr><td>");
        for (int i = a.GetLowerBound(0); i <= a.GetUpperBound(0); i++)
        {
            if (a[i] == "")
            {
                body.Append("<tr><td></td></tr>");
            }
            else
            {
                string cadena = string.Empty;
                string caux = string.Empty;
                caux = a[i];
                body.Append("<tr><td>" + caux + "</td></tr>");
            }
        }
        body.Append("</table>");
        body.Append("<br/><br/>Atentamente. El Equipo de Kísem de México.");
        #endregion
        htmBody.Append(body.ToString());
        htmBody.Append("</body>");
        htmBody.Append("</html>");
        

        enviaEmail(nuevoAsociado.Email, "Bienvenido a Kisem ", htmBody.ToString());
        CorreoPatrocinador(infoPatrocinador[1], infoPatrocinador[2], nuevoAsociado.Nombre + " " + nuevoAsociado.ApPaterno + " " + nuevoAsociado.ApMaterno);
    }

    private void CorreoPatrocinador(string nombrePatrocinador, string emailPatrocinador, string nombreInvitado)
    {
        StringBuilder htmBody = new StringBuilder();
        StringBuilder body = new StringBuilder();
        #region cuerpoCorreo
        body.Append("<table width='800px' style='font-family: Arial; text-align: justify; color: #000000; font-size: medium; font-weight: normal; margin-top: 0px; background-position: bottom; background-repeat: no-repeat;'>");
        body.Append("<tr>");
        body.Append("<td>Estimado: <span style='font-weight:bold;'>" + nombrePatrocinador + "</span></td>");
        body.Append("</tr>");
        body.Append("<tr><td>");
        body.Append("Nos es grato informarle que se ha integrado a su organización un (a) nuevo (a) líder, su nombre es:<br/><span style='font-weight:bold;'>" + nombreInvitado + "</span>");
        body.Append("</tr></td>");
        body.Append("<tr><td>");
        body.Append("Le invitamos a darle el apoyo que requiere para desarrollar con gran éxito su negocio, le recordamos que usted forma parte de su línea de auspicio y será con usted con quien se apoye para su aprendizaje.");
        body.Append("</tr></td>");
        body.Append("<tr><td>");
        body.Append("Por lo mismo, es importante que usted se apegue al sistema educativo que se le ofrece dentro de Kísem de México, con la finalidad de saber guiar a sus asociados a la cima del éxito.");
        body.Append("</tr></td>");
        body.Append("<tr><td>");
        body.Append("Le felicitamos por el compromiso que adquiere y le invitamos a ser el patrocinador que usted mismo necesita.");
        body.Append("</tr></td>");
        body.Append("<tr><td>");
        body.Append(" Atte. El Equipo de Kísem de México.");
        body.Append("</tr></td>");
        body.Append("</table>");
        #endregion
        htmBody.Append(body.ToString());
        htmBody.Append("</body>");
        htmBody.Append("</html>");
        enviaEmail(emailPatrocinador, "Felicidades, tienes un nuevo asociado", htmBody.ToString());
    }

    private void enviaEmail(string destinatario, string asunto, string cuerpo)
    {
        try
        {
            MailMessage correo = new MailMessage();
            correo.From = new MailAddress("atencionaclientes@kisem.com.mx");
            correo.To.Add(destinatario);
            correo.Subject = asunto;
            correo.Body = cuerpo;
            correo.IsBodyHtml = true;
            correo.Priority = MailPriority.Normal;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "kisem.com.mx";
            smtp.Credentials = new System.Net.NetworkCredential("atencionaclientes@kisem.com.mx", "Atencionaclientes1");
            smtp.Port = 587;
            smtp.Send(correo);
        }
        catch (Exception)
        {

        }
    }

    private string ConsMensajeBienvenida()
    {
        string MensajeCompra = string.Empty;
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();
        try
        {
            string strQuery = "SELECT MENSAJEBIENVENIDA FROM CONFIGURACION";

            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            MySqlDataReader reader = queryEx.ExecuteReader();
            while (reader.Read())
            {
                MensajeCompra = reader["MENSAJEBIENVENIDA"].ToString();
            }
            reader.Close();
            mySqlConn.Close();
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al Consultar Mensaje Bienvenida : " + ex.Message);
            throw new Exception("Error EmailAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
        return MensajeCompra;
    }

    private string ConsAvisoPrivacidad()
    {
        string MensajeCompra = string.Empty;
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();
        try
        {
            string strQuery = "SELECT AVISODEPRIVACIDAD FROM CONFIGURACION";

            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            MySqlDataReader reader = queryEx.ExecuteReader();
            while (reader.Read())
            {
                MensajeCompra = reader["AVISODEPRIVACIDAD"].ToString();
            }
            reader.Close();
            mySqlConn.Close();
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al Consultar Aviso Privacidad : " + ex.Message);
            throw new Exception("Error EmailAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
        return MensajeCompra;
    }

    private List<string> ConsinfoPatrocinador(string idPatrocinador)
    {
        List<string> info = new List<string>();
        Conection con = new Conection();
        MySqlConnection mySqlConn = con.conectBDPackage();
        try
        {
            string strQuery = "SELECT ID, CONCAT(NOMBRE, ' ', APPATERNO, ' ', APMATERNO) AS NOMBRE, EMAIL FROM ASOCIADOS WHERE ID=" + idPatrocinador;
            MySqlCommand queryEx = new MySqlCommand(strQuery, mySqlConn);
            mySqlConn.Open();
            MySqlDataReader reader = queryEx.ExecuteReader();
            while (reader.Read())
            {
                info.Add(reader["ID"].ToString());
                info.Add(reader["NOMBRE"].ToString().ToUpper());
                info.Add(reader["EMAIL"].ToString());
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error en InfoProspecto: " + ex.Message);
            throw new Exception("Error EmailAction: " + ex.Message);
        }
        finally
        {
            mySqlConn.Dispose();
            mySqlConn.Close();
            con.closeConection();
        }
        return info;
    }
}