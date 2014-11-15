using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using System.IO;

public partial class Master_MenuPrincipal : System.Web.UI.MasterPage
{
    private static readonly ILog _LOGGER = LogManager.GetLogger(typeof(Master_MenuPrincipal));

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["IdUser"] == null)
        {
            _LOGGER.Error("La sesión de usuario ha caducado");//mensaje para log4net
            Response.Redirect("~/Login.aspx");//redirecciona a Login
        }
        if (!IsPostBack)
        {
            try
            {
                string User = Session["User"].ToString();
                int idUser = int.Parse(Session["IdUser"].ToString());

                Menu menus;
                MenuAction actionMenu = new MenuAction();
                FileInfo fi = new FileInfo(this.Page.Request.FilePath);
                string namePag = fi.Name.ToLower();
                List<MenuModel> ListMenu = actionMenu.getMenu(idUser);
                List<string> UrlPermiso = new List<string>();
                if (Session["ListPermisos"] != null)
                {
                    UrlPermiso = (List<string>)Session["ListPermisos"];
                }
                else
                {
                    foreach (MenuModel dataMenu in ListMenu)
                    {
                        UrlPermiso.Add(dataMenu.NamePage.ToLower());
                    }
                    //if (idRol != 1)
                    //{
                    //    string DetallePageName = "DetalleFallas.aspx";
                    //    string DetallePageName2 = "DetalleAccion.aspx";
                    //    UrlPermiso.Add(DetallePageName.ToLower());//Agrega a la lista "DetalleFallas.aspx"
                    //    UrlPermiso.Add(DetallePageName2.ToLower());
                    //}
                    Session.Add("ListPermisos", UrlPermiso);
                }
                if (!UrlPermiso.Contains(namePag))
                {
                    Session.Abandon();
                    Response.Redirect("~/Login.aspx", false);
                }
                menus = (Menu)FindControl("MenuPrincipalaspx");
                if (menus != null)
                {
                    MenuItem MenuParent = null;
                    MenuItem MenuChild = null;
                    MenuItem MenuChild2 = null;

                    foreach (var DataMenu in ListMenu)
                    {

                        if (DataMenu.IdParent.Equals(0))
                        {
                            MenuParent = new MenuItem(DataMenu.MenuName, DataMenu.IdMenu.ToString(), "", DataMenu.UrlMenu);
                            menus.Items.Add(MenuParent);

                            foreach (var DataMenuChild in ListMenu)
                            {
                                if (DataMenuChild.IdParent.Equals(DataMenu.IdMenu))
                                {
                                    MenuChild = new MenuItem(DataMenuChild.MenuName, DataMenuChild.IdMenu.ToString(), "", DataMenuChild.UrlMenu);
                                    MenuParent.ChildItems.Add(MenuChild);

                                    foreach (var DataMenuChild2 in ListMenu)
                                    {
                                        if (DataMenuChild2.IdParent.Equals(DataMenuChild.IdMenu))
                                        {
                                            MenuChild2 = new MenuItem(DataMenuChild2.MenuName, DataMenuChild2.IdMenu.ToString(), "", DataMenuChild2.UrlMenu);
                                            MenuChild.ChildItems.Add(MenuChild2);
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _LOGGER.Error("Error al construir el menu " + ex.Message);
                throw new Exception("Error en Master Page MenuPrincipal" + ex.Message);
            }
        }
    }

    protected void cerrarSesion_Click(object sender, EventArgs e)
    {
        Session.Abandon();
        Response.Redirect("~/Login.aspx");
    }

}
