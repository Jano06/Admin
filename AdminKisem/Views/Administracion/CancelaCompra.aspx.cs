using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Views_Administracion_CancelaCompra : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            lblTitlePrincipal.Text = "CANCELACIÓN DE COMPRAS";
        }
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
                CancelaCompraAction action = new CancelaCompraAction();
                List<CancelaCompraModel> lista = new List<CancelaCompraModel>();
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

    protected void imgBtnCancela_Click(object sender, EventArgs e)
    {
        string mensaje = string.Empty;
        try
        {
            ImageButton ImgDetalle;
            GridViewRow gvdCompra;
            Label lblIdCompra;
            string idCompra = string.Empty;
            CancelaCompraAction action = new CancelaCompraAction();
            ImgDetalle = (ImageButton)sender;
            gvdCompra = (GridViewRow)ImgDetalle.Parent.Parent;
            lblIdCompra = (Label)gvdCompra.FindControl("lblIdCompra");
            idCompra = lblIdCompra.Text.Trim();
            
            mensaje = action.cancelaCompra(idCompra);
            cellmensaje.ForeColor = System.Drawing.Color.DodgerBlue;
            btnAceptar.BackColor = System.Drawing.Color.DodgerBlue;
            cellmensaje.Text = mensaje;
            modalMensaje.Show();
            List<CancelaCompraModel> lista = new List<CancelaCompraModel>();
            lista = action.buscaCompra(txtReferencia.Text);
            gvdCompras.DataSource = lista;
            gvdCompras.DataBind();
        }
        catch (Exception ex)
        {
            cellmensaje.ForeColor = System.Drawing.Color.Red;
            btnAceptar.BackColor = System.Drawing.Color.Red;
            cellmensaje.Text = "Error:" + ex;
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
}