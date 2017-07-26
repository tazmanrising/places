<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <title>Liberty Data Entry</title>
    <link href="Content/bootstrap.css" rel="stylesheet" type="text/css" />
    <meta name="viewport" content="width=device-width, initial-scale=1">
</head>
<body>
    <form id="Form1" runat="server" class="form-horizontal" role="form">
    <div class="container-fluid">
        <div class="row text-center">
            <h1>
                Liberty Data Entry</h1>
        </div>
        <div class="row">
            <asp:Panel ID="pnlError" runat="server" Visible="False" class="col-md-6 col-md-offset-3">
                <div class="panel panel-danger">
                    <div class="panel-heading">
                        <h3 class="panel-title">
                            You must correct the following errors before continuing:</h3>
                    </div>
                    <div class="panel-body">
                        <asp:BulletedList ID="blErrorList" runat="server" CssClass="">
                        </asp:BulletedList>
                    </div>
                </div>
            </asp:Panel>
        </div>
        <div class="row col-md-8 col-md-offset-2">
            <div class="panel panel-info">
                <div class="panel-heading">
                    <h3 class="panel-title">
                        User Login</h3>
                </div>
                <div class="panel-body text-center">
                    <div class="form-group">                        
                        <label for="txtUserName" class="col-sm-2 control-label">User ID:</label>
                        <div class="col-sm-10">
                            <asp:TextBox ID="txtUserName" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>                        
                    </div>
                    <div class="form-group">
                        <label for="txtPassword" class="col-sm-2 control-label">Password:</label>
                        <div class="col-sm-10">
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                        </div>                        
                    </div>
                    <div class="form-group hidden">
                        <asp:CustomValidator ID="cvCheckLogin" runat="server" ErrorMessage="The entered User ID or Password are incorrect."
                            OnServerValidate="cvCheckLogin_ServerValidate" Display="None">&nbsp;</asp:CustomValidator>
                    </div>
                    <div class="form-group">
                        <asp:Button ID="btnSubmit" runat="server" Text="Login" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    </form>
</body>
</html>