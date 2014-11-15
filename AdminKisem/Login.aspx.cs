using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SetFocus(txtUsuario);
    }

    protected void click_BtnEntrar(object sender, EventArgs e)
    {
        if (txtUsuario.Text.Length.Equals(0) && txtPassword.Text.Length.Equals(0))
        {
            lblMensaje.Text = "Ingrese Usuario y Contraseña";
        }
        else
        {
            string admin = txtUsuario.Text.Trim();
            LoginAction action = new LoginAction();
            LoginModel model = new LoginModel();
            model = action.ConsAdmin(txtUsuario.Text, txtPassword.Text);
            if (model.IdAdmin == 0 || model.Admin == string.Empty)
            {
                lblMensaje.Text = "Usuario o Contraseña incorrectos";
                this.limpiar();
            }
            else
            {
                Session.Add("User", model.Admin);
                Session.Add("IdUser", model.IdAdmin);
                Response.Redirect("~/Views/Administracion/ValidaCompras.aspx");
            }
        }
    }

    private void limpiar()
    {
        txtUsuario.Text = "";
        txtPassword.Text = "";
    }
}