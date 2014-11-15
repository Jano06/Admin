<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Master/MenuPrincipal.master" CodeFile="ValidaComprasUnitarias.aspx.cs" Inherits="Views_Administracion_ValidaComprasUnitarias" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Stylesheets" runat="server">
    <link rel="Stylesheet" href="../../Style/StyleKisem.css" type="text/css" />
    <style type="text/css">
    .subtitulo
    {
        text-align: justify;
        font-family: Arial;
        font-size: medium;
        font-weight: normal;
        color: Black;
        vertical-align: top;
        width: 820px;
        height: 60px;
        margin-top: 0px;
        background-position: bottom; 
        background-repeat: no-repeat;
        border-style: solid;
        border-color: Silver;
        border-width: thin;
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
            <asp:TableCell HorizontalAlign="Center">
                <asp:Label ID="lblReferencia" runat="server" Text="Referencia:" />
                <asp:TextBox ID="txtReferencia" runat="server" />&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" />&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClick="btnCancelar_Click" />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <br />
    <div style="border-color:Silver; border-width:thin; border-style: solid; overflow:auto; width: 818px; height: 500px; ">
        <asp:UpdatePanel ID="pnlGridView" runat="server">
            <ContentTemplate>
                <asp:Table ID="tblGridView" runat="server" Width="800px" CellPadding="2" Font-Size="Small">
                    <asp:TableRow>
                        <asp:TableCell HorizontalAlign="Center">
                            <asp:GridView ID="gvdCompras" runat="server" AutoGenerateColumns="false" EmptyDataText="No hay información a mostrar."
                                ForeColor="#333333" GridLines="Vertical" Width="100%" PageSize="15" BorderWidth="1" BorderColor="#808080"
                                BorderStyle="Ridge" RowStyle-BorderStyle="Groove" AllowPaging="true" AllowSorting="false"
                                OnPageIndexChanging="gvdCompras_PageIndexChanging" CellPadding="4" HorizontalAlign="Center">
                                <AlternatingRowStyle BackColor="White" />
                                <EditRowStyle BackColor="#2461BF" />
                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                <RowStyle BackColor="#EFF3FB" />
                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Pagar" Visible="true" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="imgBtnPago" runat="server" ImageUrl="~/Imagenes/pago.png" OnClick="imgBtnPago_Click" Width="20" Height="20" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="" Visible="false" >
                                        <ItemTemplate >
                                            <asp:Label ID="lblIdCompra" runat="server" Text ='<%#Eval("IdCompra")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Referencia" Visible="true" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblReferencia" runat="server" Text ='<%#Eval("Referencia")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Asociado" Visible="true">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAsociado" runat="server" Text ='<%#Eval("Asociado")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Fecha Orden" Visible="true" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblFechaOrden" runat="server" Text ='<%#Eval("FechaOrden")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Monto" Visible="true" ItemStyle-HorizontalAlign="Right" ItemStyle-Font-Bold="true">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPrecio" runat="server" Text ="$"></asp:Label>
                                            <asp:Label ID="lblMonto" runat="server" Text ='<%#Eval("Monto")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Status" Visible="true" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblStatusPago" runat="server" Text ='<%#Eval("StatusPago")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                   
                                </Columns>
                            </asp:GridView>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
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
