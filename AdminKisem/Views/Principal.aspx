<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Master/MenuPrincipal.master" CodeFile="Principal.aspx.cs" Inherits="Views_Principal" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContenidoPrincipal" runat="server">
    <asp:Panel ID="pnlAvisos" runat="server">
        <asp:Table ID="tblInicio" runat="server">
            <asp:TableRow>
                <asp:TableCell>
                    <asp:Label runat="server" Text="Bienvenido a la nueva pagina de Administración" /> 
                </asp:TableCell></asp:TableRow></asp:Table></asp:Panel></asp:Content>