<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Display.aspx.cs" Inherits="Display" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
        <title>Frontier Data Entry</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div style="text-align: center; margin-top: 100px;">
        <h3>
            Your Record Locator Number is</h3>
        <div style="padding: 20px 0px 20px 0px;">
            <asp:Label ID="lblRecordLocator" runat="server" Text="" CssClass="recordlocator-text" />
        </div>
        <h3>
            For Telephone Numbers</h3>
        <div style="padding: 10px 0px 0px 0px;">
            <asp:Label ID="lblTelephoneNumber" runat="server" Text="" />
        </div>
        <div style="padding: 30px 0px 0px 0px;">
            <asp:Button ID="btnNext" runat="server" Text="Next Order" CssClass="submitButton" onclick="btnSubmit_Click" />
        </div>
    </div>
</asp:Content>

