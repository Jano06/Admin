using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Views_Administracion_ValidaComprasUnitarias : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            lblTitlePrincipal.Text = "VALIDA COMPRAS UNITARIAS";
            LlenaGrid();
        }
    }

    private void LlenaGrid()
    {
        ValidaCompraUnitAction action = new ValidaCompraUnitAction();
        List<ValidaCompraUnitModel> lista = new List<ValidaCompraUnitModel>();
        lista = action.ConsCompras();
        gvdCompras.DataSource = lista;
        gvdCompras.DataBind();
        Session.Add("GridValidaCompras", gvdCompras);
    }

    protected void click_btnPopUpAceptar(object sender, EventArgs e)
    {
        cellmensaje.Text = string.Empty;
        modalMensaje.Hide();
    }

    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        try
        {
            string mensaje = validaCampos();
            if (mensaje.Length > 0)
            {
                cellmensaje.ForeColor = System.Drawing.Color.Red;
                btnAceptar.BackColor = System.Drawing.Color.Red;
                cellmensaje.Text = mensaje;
                modalMensaje.Show();
            }
            else
            {
                ValidaCompraUnitAction action = new ValidaCompraUnitAction();
                List<ValidaCompraUnitModel> lista = new List<ValidaCompraUnitModel>();
                lista = action.buscaCompra(txtReferencia.Text);
                gvdCompras.DataSource = lista;
                gvdCompras.DataBind();
            }
        }
        catch (Exception ex)
        {
            cellmensaje.ForeColor = System.Drawing.Color.Red;
            btnAceptar.BackColor = System.Drawing.Color.Red;
            cellmensaje.Text = "Error: " + ex;
            modalMensaje.Show();
        }
    }

    private string validaCampos()
    {
        string mensaje = string.Empty;
        if (txtReferencia.Text.Length > 0)
        {
            try
            {
                int referencia = Convert.ToInt32(txtReferencia.Text);
            }
            catch (Exception)
            {
                mensaje = "Ingresar número";
            }
        }
        else
        {
            mensaje = "Ingresa referencia";
        }
        return mensaje;
    }

    protected void btnCancelar_Click(object sender, EventArgs e)
    {
        try
        {
            txtReferencia.Text = "";
            LlenaGrid();
        }
        catch (Exception ex)
        {
            cellmensaje.Text = "Error:" + ex;
            modalMensaje.Show();
        }
    }

    protected void imgBtnPago_Click(object sender, EventArgs e)
    {
        try
        {
            string mensaje = string.Empty;
            ImageButton ImgDetalle;
            GridViewRow gvdCompra;
            Label lblReferencia, lblMonto;
            ValidaCompraModel model = new ValidaCompraModel();
            ImgDetalle = (ImageButton)sender;
            gvdCompra = (GridViewRow)ImgDetalle.Parent.Parent;
            lblReferencia = (Label)gvdCompra.FindControl("lblReferencia");
            model.Referencia = int.Parse(lblReferencia.Text.Trim());
            model.Fecha = DateTime.Today;
            lblMonto = (Label)gvdCompra.FindControl("lblMonto");
            model.Importe = Convert.ToDouble(lblMonto.Text);

            mensaje = actualizaCompra(model);
            LlenaGrid();
            cellmensaje.ForeColor = System.Drawing.Color.DodgerBlue;
            btnAceptar.BackColor = System.Drawing.Color.DodgerBlue;
            cellmensaje.Text = mensaje;
            modalMensaje.Show();
        }
        catch (Exception ex)
        {
            cellmensaje.ForeColor = System.Drawing.Color.Red;
            btnAceptar.BackColor = System.Drawing.Color.Red;
            cellmensaje.Text = "Error:" + ex;
            modalMensaje.Show();
        }
    }

    private string actualizaCompra(ValidaCompraModel model)
    {
        string mensaje = string.Empty;
        try
        {
            ValidaComprasAction action = new ValidaComprasAction();
            SubePuntosAction SubePuntos = new SubePuntosAction();
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

            if (action.actualizaPago(idAsociado, model.Fecha, idCompra)) //Pone Pagado y fechaPago
            {
                List<string> activacion = action.ConsInfoCompra(idCompra);
                if (activacion[0] == "1" && idProspecto == 0) //Compra Activación
                {
                    DateTime FinActivac = action.ConsFinActivacionAsociado(idAsociado);
                    if (FinActivac >= Convert.ToDateTime(activacion[1]))
                    {
                        action.actualizaAsociado(Convert.ToDateTime(activacion[2]), idAsociado, idCompra, puntos);
                        DateTime finsc = SubePuntos.ConsultaFechaInscripcionAsociado(idAsociado);
                        SubePuntos.subePuntosActivacion(idAsociado, idCompra, puntos, finsc);
                        mensaje = "Compra: " + idCompra.ToString() + " Activacion - Subio Puntos";
                        //Subir Puntos tomar en cuenta 
                    }
                    else
                    {
                        //Subir Puntos 
                        action.actualizaAsociado(Convert.ToDateTime(activacion[2]), Convert.ToDateTime(activacion[1]), idAsociado, idCompra, puntos);
                        DateTime finsc = SubePuntos.ConsultaFechaInscripcionAsociado(idAsociado);
                        SubePuntos.subePuntosActivacion(idAsociado, idCompra, puntos, finsc);
                        mensaje = "Compra: " + idCompra.ToString() + " Activacion - Subio Puntos";
                    }
                }
                else
                {
                    if (idProspecto == 0)
                    {
                        if (puntos.Equals(1000))
                        {
                            SubePuntos.subePuntosExcedente(idAsociado, idCompra, 800);//Subir 800 pts para compra de 1150
                            mensaje = "Compra: " + idCompra + " Excedente - Subio Puntos";
                        }
                        else
                        {
                            SubePuntos.subePuntosExcedente(idAsociado, idCompra, puntos);//Subir pts para compra excedente
                            mensaje = "Compra: " + idCompra + " Excedente - Subio Puntos";
                        }
                    }
                    else
                    {
                        SubePuntos.subePuntosInscripcion(idAsociado, idCompra, puntos);
                        mensaje = "Compra: " + idCompra + " Inscripcion " + idAsociado + " - Subió Puntos";
                    }
                }
            }
            else
            {
                mensaje = "Error al registrar referencia:" + model.Referencia;
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
    
    protected void gvdCompras_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        if (Session["GridValidaCompras"] != null)
        {
            gvdCompras.DataSource = ((GridView)Session["GridValidaCompras"]).DataSource;
            gvdCompras.PageIndex = e.NewPageIndex;
            Session.Add("numPageValidaCompras", gvdCompras.PageIndex);
            gvdCompras.DataBind();
        }
    }
}