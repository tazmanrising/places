<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="Display.aspx.cs" Inherits="Display" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <title>Liberty Data Entry</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
    <div id="divDisplayForm" runat="server" class="row col-md-5 col-md-offset-4">
        <div class="panel panel-success">
            <div class="panel-heading">
                <h3 class="panel-title">
                    Account Submitted</h3>
            </div>
            <div class="panel-body text-center">
                <div class="form-group">
                    <label class="lead">
                        Thank you for your submission, your confirmation number is:</label>
                </div>
                <div class="lead">
                    <asp:Label ID="lblRecordLocator" runat="server" Text="" CssClass="lead" />
                </div>
                <div class="lead">
                    <asp:Label ID="lblTelephoneNumber" runat="server" Text="TPV Number: 866-386-8251" />
                </div>
                <div class="form-group">
                    <asp:Button ID="btnNext" runat="server" Text="Next Order" CssClass="btn btn-primary"
                        OnClick="btnSubmit_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
