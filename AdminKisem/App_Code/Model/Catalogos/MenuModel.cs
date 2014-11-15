using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de MenuModel
/// </summary>
public class MenuModel
{
    private int idMenu;
    private string menuName;
    private string urlMenu;
    private int idParent;
    private string namePage;

    public int IdMenu { get { return idMenu; } set { idMenu = value; } }
    public string MenuName { get { return menuName; } set { menuName = value; } }
    public string UrlMenu { get { return urlMenu; } set { urlMenu = value; } }
    public int IdParent { get { return idParent; } set { idParent = value; } }
    public string NamePage { get { return namePage; } set { namePage = value; } }
}