using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using log4net;

public partial class Views_Administracion_ValidaCompras : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            lblTitlePrincipal.Text = "VALIDACIÓN COMPRAS";
        }

    }

    protected void click_btnPopUpAceptar(object sender, EventArgs e)
    {
        cellmensaje.Text = string.Empty;
        modalMensaje.Hide();
    }

    protected void btnCargar_Click(object sender, EventArgs e)
    {
        try
        {
            if (fileUploader.HasFile)
            {
                lblError.Text = "";
                string ext = fileUploader.PostedFile.FileName;
                string exten = System.IO.Path.GetExtension(fileUploader.FileName);
                
                if(!exten.Equals(".txt"))
                {
                    cellmensaje.Text = "Formato de archivo Inválido";
                    modalMensaje.Show();
                }
                else
                {
                    procesaArchivo();
                }
            }
            else
            {
                cellmensaje.Text = "Seleccionar Archivo"; 
                modalMensaje.Show();
            }
        }
        catch (Exception ex)
        {
            cellmensaje.Text = "Error:" + ex;
            modalMensaje.Show();
        }
    }

    private void procesaArchivo()
    {
        string TargetPath = "../../temp/banco.txt";
        fileUploader.PostedFile.SaveAs(Server.MapPath(TargetPath));
        string fic = Server.MapPath(TargetPath).ToString();
        string texto = string.Empty;
        lblError.Text = "";
        
        StreamReader sr = new StreamReader(fic);
        int cont = 0;
        string Line = string.Empty;
        while ((Line = sr.ReadLine()) != null)
        {
            try
            {
                ValidaCompraModel model = new ValidaCompraModel();
                ValidaComprasAction action = new ValidaComprasAction();
                cont++;
                Line = Line.Trim();
                model.Fecha = Convert.ToDateTime(Line.Substring(0, 10));
                model.Referencia = Convert.ToInt32(Line.Substring(10, 10));
                model.Importe = Convert.ToDouble(Line.Substring(20));
                if (action.ValidaDatos(model))
                {
                    if (action.ValidaCompra(model))
                    {
                        lblError.Text += actualizaCompra(model);
                    }
                    else
                    {
                        lblError.Text += "Compra: " + model.Referencia + " ya registrada<br />";
                    }
                }
                else
                {
                    lblError.Text += "Datos Incorrectos en referencia: " + model.Referencia + "</br>";
                }
            }
            catch (Exception)
            {
                lblError.Text += "Formato Inválido en Línea: " + cont + "</br>";
            }
        }
        sr.Close();
    }

    private string actualizaCompra(ValidaCompraModel model)
    {
        string mensaje = string.Empty;
        try
        {
            ValidaComprasAction action = new ValidaComprasAction();
            SubePuntosAction Subepuntos = new SubePuntosAction();
            action.RevisaCompra(model.Referencia); //Valida que coincidan los datos del archivo con la orden de compra
            List<int> datos = action.ConsultaDatos(model.Referencia); //Lee los datos de la compra segun la referencia del archivo
            int idCompra = datos[0];
            int idAsociado = datos[1];
            int idProspecto = datos[2];
            int puntos = datos[3];

            if (idProspecto > 0) //Compra Inscripcion
            {
                mensaje = CompraInscripcion(idProspecto, idCompra, idAsociado, puntos); //Inserta nuevo asociado, manda correos, cambia el asociado de la compra
                action.EliminaProspecto(idProspecto); //Elmina el prospecto cuando ya está dado de alta como asociado
                datos = action.ConsultaDatos(model.Referencia);
                idCompra = datos[0];
                idAsociado = datos[1];
                idProspecto = datos[2];
                puntos = datos[3];
            }

            if (action.actualizaPago(idAsociado, model.Fecha, idCompra)) //Pone Pagado y fechaPago según el archivo
            {
                List<string> activacion = action.ConsInfoCompra(idCompra);
                if (activacion[0] == "1" && idProspecto == 0)
                {
                    DateTime FinActivac = action.ConsFinActivacionAsociado(idAsociado);
                    if (FinActivac >= Convert.ToDateTime(activacion[1]))
                    {
                        action.actualizaAsociado(Convert.ToDateTime(activacion[2]), idAsociado, idCompra, puntos);
                        mensaje = "Compra: " + idCompra + " Activación.<br />";
                    }
                    else
                    {
                        action.actualizaAsociado(Convert.ToDateTime(activacion[2]), Convert.ToDateTime(activacion[1]), idAsociado, idCompra, puntos);
                        mensaje = "Compra: " + idCompra + " Activación.<br />";
                    }
                }
                else
                {
                    if (idProspecto == 0)
                    {
                        if (puntos.Equals(1000))//Subir 800 pts para compra de 1000 pts
                        {
                            Subepuntos.subePuntosExcedente(idAsociado, idCompra, 800);
                            mensaje = "Compra: " + idCompra + " Excedente - Subio Puntos <br />";
                        }
                        else
                        {
                            mensaje = "Compra: " + idCompra + " Tipo Excedente <br />";
                        }
                    }
                    else
                    { mensaje = "Compra: " + idCompra + " Tipo Inscripcion " + idAsociado + " <br />"; }
                }
            }
            else
            {
                mensaje += "Error al registrar referencia:" + model.Referencia + "<br />";
            }
        }
        catch (Exception ex)
        {
            mensaje = "Error " + ex.Message;
        }
        return mensaje;
    }

    private string CompraInscripcion(int idProspecto, int idCompra, int patrocinador, int puntos)
    {
        string mensaje = string.Empty;
        try
        {
            ValidaComprasAction action = new ValidaComprasAction();
            AsociadosModel asociado = new AsociadosModel();
            EmailAction email = new EmailAction();
            asociado = action.DatosProspecto(idProspecto);
            asociado.Padre = action.validaPadre(asociado.Padre, asociado.Lado);
            asociado.Status = 1;
            asociado.Contrasena = creaPassword();
            asociado.InicioActivacion = DateTime.Today;
            asociado.FinActivacion = ValidaFecha(DateTime.Today);
            asociado.FechaInscripcion = DateTime.Today;
            List<string> infoPadre = action.InfoPadre(Convert.ToInt32(asociado.Padre), asociado.Lado);
            asociado.Recorrido = infoPadre[0];
            asociado.LadosRecorrido = infoPadre[1];
            List<string> infoPatroc = action.InfoPatroc(patrocinador);
            asociado.Historia = infoPatroc[0];
            asociado.Bodega = Convert.ToInt32(infoPatroc[1]);
            asociado.Orden = infoPatroc[2];
            asociado.LadoPatrocinador = LadoPatrocinador(asociado.Recorrido, asociado.LadosRecorrido, patrocinador);
            asociado.Patrocinador = patrocinador.ToString();
            asociado.PtsMes = puntos;
            asociado.Rango = 1;
            asociado.RangoPago = 1;
            List<string> infoNuevoAsoc = Alias(asociado.Nombre);
            asociado.IdAsociado = Convert.ToInt32(infoNuevoAsoc[0]);
            asociado.Alias = infoNuevoAsoc[1];
            action.insertaAsociado(asociado);
            email.CorreoBienvenida(asociado);
            action.CambiaIdAsociadoEnCompra(asociado.IdAsociado, idCompra);
            mensaje = "Alta Asociado: " + asociado.IdAsociado + "<br />";
        }
        catch (Exception ex)
        {
            mensaje = "Error: " + ex.Message;
        }
        return mensaje;
    }

    private string creaPassword()
    {
        string password = string.Empty;
        string _alloweChars = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        Random aleatorio = new Random();
        for (int i = 0; i < 2; i++)
        {
            int lugar = aleatorio.Next(i, _alloweChars.Length);
            password += _alloweChars.Substring(lugar, 1);
        }
        _alloweChars = "1234567890";
        for (int i = 0; i < 3; i++)
        {
            int lugar = aleatorio.Next(i, _alloweChars.Length);
            password += _alloweChars.Substring(lugar, 1);
        }
        _alloweChars = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        for (int i = 0; i < 2; i++)
        {
            int lugar = aleatorio.Next(i, _alloweChars.Length);
            password += _alloweChars.Substring(lugar, 1);
        }
        return password;
    }

    private DateTime ValidaFecha(DateTime fecha)
    {
        try
        {
            int mes = fecha.Month;
            int anio = fecha.Year;
            if (fecha.Day >= DateTime.DaysInMonth(fecha.Year, fecha.Month))
            {
                if ((fecha.Month + 1) > 12)
                {
                    fecha = Convert.ToDateTime((DateTime.DaysInMonth((fecha.Year + 1), 1)) + "/01/" + (fecha.Year + 1));
                }
                else
                {
                    fecha = Convert.ToDateTime((DateTime.DaysInMonth((fecha.Year), 1) - 1) + "/" + (fecha.Month + 1) + "/" + fecha.Year);
                }
            }
            else
            {
                if ((fecha.Month + 1) > 12)
                {
                    if ((fecha.Day - 1) > 0)
                    { fecha = Convert.ToDateTime((fecha.Day - 1) + "/01/" + (fecha.Year + 1)); }
                    else
                    { fecha = Convert.ToDateTime(DateTime.DaysInMonth(fecha.Year + 1, 1) + "/01/" + (fecha.Year + 1)); }
                }
                else
                {
                    if ((fecha.Day - 1) > 0)
                    { fecha = Convert.ToDateTime((fecha.Day - 1) + "/" + (fecha.Month + 1) + "/" + (fecha.Year)); }
                    else
                    { fecha = Convert.ToDateTime(DateTime.DaysInMonth(fecha.Year, fecha.Month) + "/" + (fecha.Month) + "/" + (fecha.Year)); }
                }

            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error ValidaComprasAction: " + ex.Message);
        }
        return fecha;
    }

    private string LadoPatrocinador(string recorrido, string ladosRecorrido, int patrocinador)
    {
        string ladoPatroc = string.Empty;
        try
        {
            string[] asociados = recorrido.Split(new char[] { '.' });
            string[] lados = ladosRecorrido.Split(new char[] { '.' });
            int i;
            for (i = 0; i < asociados.Length; i++)
            {
                if (asociados[i] == patrocinador.ToString())
                {
                    break;
                }
            }
            ladoPatroc = lados[i];
        }
        catch
        {
        }
        return ladoPatroc;
    }

    private List<string> Alias(string nombre)
    {
        List<string> lista = new List<string>();
        try
        {
            ValidaComprasAction action = new ValidaComprasAction();
            int id = action.MaxAsoc();
            lista.Add(id.ToString());
            lista.Add(nombre.Substring(0, 3) + id.ToString());
        }
        catch
        { }
        return lista;
    }
}