using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de LoginModel
/// </summary>
public class LoginModel
{
    private int idAdmin;
    private string admin;

    public int IdAdmin { get { return idAdmin; } set { idAdmin = value; } }
    public string Admin { get { return admin; } set { admin = value; } }
}