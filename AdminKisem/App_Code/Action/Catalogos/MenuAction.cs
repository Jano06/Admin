using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using System.Data;
using MySql.Data.MySqlClient;

/// <summary>
/// Descripción breve de MenuAction
/// </summary>
public class MenuAction
{
    private static readonly ILog _LOGGER = LogManager.GetLogger(typeof(MenuAction));

    public List<MenuModel> getMenu(int idAdmin)
    {
        List<MenuModel> listMenu = new List<MenuModel>();
        Conection conn = new Conection();
        MySqlConnection MysqlConn = conn.conectBDPackage();
        try
        {
            string strQuery = "SELECT MENU.IDMENU, MENU.MENUNAME, MENU.URLMENU, MENU.IDPARENT, MENU.NAMEPAGE FROM MENU, ADMINMENU "
                        + "WHERE MENU.IDMENU=ADMINMENU.IDMENU AND ADMINMENU.IDADMIN=" + idAdmin;
            MySqlCommand queryEx = new MySqlCommand(strQuery, MysqlConn);
            MysqlConn.Open();
            MySqlDataReader reader = queryEx.ExecuteReader();

            while (reader.Read())
            {
                MenuModel model = new MenuModel();
                model.IdMenu = Convert.ToInt32(reader["IDMENU"].ToString());
                model.MenuName = reader["MENUNAME"].ToString();
                model.UrlMenu = reader["URLMENU"].ToString();
                model.IdParent = Convert.ToInt32(reader["IDPARENT"].ToString());
                model.NamePage = reader["NAMEPAGE"].ToString();
                listMenu.Add(model);
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            _LOGGER.Error("Error al Consultar Menu: " + ex.Message);
            throw new Exception("Error MenuAction: " + ex.Message);
        }
        finally
        {
            MysqlConn.Close();
            MysqlConn.Dispose();
            conn.closeConection();
        }
        return listMenu;
    }
}