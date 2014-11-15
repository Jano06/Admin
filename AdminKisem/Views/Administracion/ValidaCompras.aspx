<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Master/MenuPrincipal.master" CodeFile="ValidaCompras.aspx.cs" Inherits="Views_Administracion_ValidaCompras" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Stylesheets" runat="server">
    <link rel="Stylesheet" href="../../Style/StyleKisem.css" type="text/css" />
    <style type="text/css">
    .subtitulo
    {
        text-align: justify;
        font-family: Arial;
        font-size: medium;
        font-weight: normal;
        color:Black;
        vertical-align:top;
        width: 820px;
        height: 60px;
        margin-top: 0px;
        background-position: bottom; 
        background-repeat: no-repeat;
        border-style:solid;
        border-color:Silver;
        border-width:thin;
    }

    .style2
    {
        height: 33px;
        width: 451px;
    }
    .title
    {
        position: relative;
        font-size: 20px;
        height: 30px;
        font-weight: bold;
        color: #2D96FF;
        text-align: center;
    }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContenidoPrincipal" runat="server">
    <div class="title">
        <asp:Label ID="lblTitlePrincipal" runat="server"  />
    </div>
    <asp:Table ID="tblOpciones" runat="server" CssClass="subtitulo">
        <asp:TableRow>
            <asp:TableCell HorizontalAlign="Right" Width="40%">
                <asp:Label ID="lblFile" AssociatedControlId="fileUploader" runat="server"
	                    Text="Seleccionar archivo del Banco:" />
            </asp:TableCell>
            <asp:TableCell>
                <asp:FileUpload id="fileUploader" runat="server" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell></asp:TableCell>
            <asp:TableCell>
                <asp:Label ID="lblError" runat="server" ForeColor="Red" />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <asp:Table ID="tblBoton" runat="server" width= "820px" >
        <asp:TableRow>
            <asp:TableCell HorizontalAlign="Center">
                <asp:Button ID="btnCargar" Text="Cargar" runat="server" OnClick="btnCargar_Click" />
            </asp:TableCell></asp:TableRow>
    </asp:Table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContenidoPopUps" runat="server">
<asp:UpdatePanel ID="pnlPooUp" runat="server">
        <ContentTemplate>
            <div style="visibility: hidden">
                <asp:Button runat="server" ID="btnHidden" /></div>
            <asp:Panel runat="server" ID="modalPopUpmensaje" CssClass="modal" Height="300px"
                Style="display: none;">
                <div class="popUpMain">
                    <div class="popUpTitle">
                        Mensaje</div><div class="popUpContent">
                        <asp:Table ID="Table1" runat="server" Width="400px">
                            <asp:TableRow>
                                <asp:TableCell ColumnSpan="3"> </asp:TableCell></asp:TableRow><asp:TableRow Height="30px">
                                <asp:TableCell ID="cellmensaje" runat="server" ForeColor="Red"> </asp:TableCell></asp:TableRow></asp:Table></div><div class="popUpFooter">
                        <asp:Button ID="btnAceptar" runat="server" CssClass="botonColor" Text="Cerrar" OnClick="click_btnPopUpAceptar" />
                    </div>
                </div>
            </asp:Panel>
            <AjaxControlToolkit:ModalPopupExtender ID="modalMensaje" runat="server" BackgroundCssClass="modal-bg"
                TargetControlID="btnHidden" PopupControlID="modalPopUpmensaje" Drag="true" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>