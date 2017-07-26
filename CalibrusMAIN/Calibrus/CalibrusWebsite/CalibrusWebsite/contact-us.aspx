<%@ Page Language="C#" AutoEventWireup="true" CodeFile="contact-us.aspx.cs" Inherits="contact_us" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Calibrus - Contact Us</title>
    <script>
        (function (i, s, o, g, r, a, m) {
            i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
                (i[r].q = i[r].q || []).push(arguments)
            }, i[r].l = 1 * new Date(); a = s.createElement(o),
  m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
        })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');

        ga('create', 'UA-45757200-2', 'calibrus.com');
        ga('send', 'pageview');

    </script>
    <link rel="stylesheet" type="text/css" href="css/application.css">
    <script type="text/javascript" src="scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="scripts/jquery.maskedinput.js"></script>
</head>
<body id="contact-us" class="simple">
    <header>
		<div class="centered960">
			<a href="index.html"><img src="images/Calibrus_logo.png" alt="logo" border="0"></a>
			<ul class="clearfix">
				<li><a href="index.html">Home</a></li>
				<li><a href="what-we-do/contact-management-solutions.html">What We Do</a></li>
				<li><a href="about-us/about-us.html">About Us</a></li>
			</ul>
		</div>
	</header>
    <div id="main" class="centered960">
        <div id="innerMain" class="clearfix">
            <div id="header-spacer">
            </div>
            <h2>
                Contact Us</h2>

            <div id="address-phone" class="clearfix">
                <img alt="Contact Us Image" src="images/Contact_Us_Image.PNG" />
            </div>
            <div id="online-required" class="clearfix">
                <span id="online">Online Feedback Form</span> <span id="required">* Required fields</span>
            </div>
            <form id="form1" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
                <Scripts>
                    <asp:ScriptReference Path="~/Scripts/Webkit.js" />
                </Scripts>
            </asp:ScriptManager>
            <script language="javascript" type="text/javascript">
                function pageLoad(sender, args) {

                    $(document).ready(function () {

                        //phone mask
                        $("#<%=txtPhone.ClientID%>").mask("(999) 999-9999");

                    });
                }
            </script>
            <table>
                <tr>
                    <td colspan="2">
                        <div class="pnlContainer">
                            <asp:Panel ID="pnlError" runat="server" Visible="False" CssClass="pnlerror">
                                <table border="0" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td align="left" valign="top" rowspan="2">
                                            <asp:Image ID="imgMasterError" runat="server" ImageUrl="images/error.png" AlternateText="Error" />
                                        </td>
                                        <td align="left" style="padding-left: 5px; width: 100%" valign="middle">
                                            <asp:Label ID="lblErrorText" runat="server" Font-Bold="True"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" valign="middle" style="padding-left: 5px">
                                            <asp:BulletedList ID="blErrorList" runat="server" CssClass="errorlist">
                                            </asp:BulletedList>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label for="txtName">
                            *Name:</label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtName" runat="server" MaxLength="50"></asp:TextBox><asp:RequiredFieldValidator
                            ID="rfName" runat="server" ControlToValidate="txtName" ErrorMessage="Name is required."
                            ValidationGroup="DataEntry"><img alt="!" src="images/validationerror.png" /></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label for="txtTitle">
                            Title:</label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtTitle" runat="server" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label for="txtCompany">
                            *Company:</label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtCompany" runat="server" MaxLength="50"></asp:TextBox><asp:RequiredFieldValidator
                            ID="rfvCompany" runat="server" ControlToValidate="txtCompany" ErrorMessage="Company is required."
                            ValidationGroup="DataEntry"><img alt="!" src="images/validationerror.png" /></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label for="txtPhone">
                            Phone:</label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtPhone" runat="server"></asp:TextBox><asp:RegularExpressionValidator
                            ID="regexPhoneNumber" runat="server" ControlToValidate="txtPhone" Display="Dynamic"
                            ErrorMessage="Phone is invalid." ValidationExpression="^\(\d{3}\) ?\d{3}( |-)?\d{4}|^\d{3}( |-)?\d{3}( |-)?\d{4}"
                            ValidationGroup="DataEntry"><img alt="!" src="images/validationerror.png" /></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label for="txtEmail">
                            *Email:</label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtEmail" runat="server" MaxLength="100"></asp:TextBox><asp:RequiredFieldValidator
                            ID="rfvEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Email is required."
                            ValidationGroup="DataEntry" Display="Dynamic"><img alt="!" src="images/validationerror.png" /></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="regexEmail" runat="server" ControlToValidate="txtEmail"
                            Display="Dynamic" ErrorMessage="Email address is invalid." ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                            ValidationGroup="DataEntry"><img alt="!" src="images/validationerror.png" /></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label for="ddlState">
                            State:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlState" runat="server" DataTextField="StateName" DataSourceID="CalibrusEntityDataSource"
                            DataValueField="StateAbbr" AppendDataBoundItems="true">
                            <asp:ListItem Selected="True" Text="" Value="">Select a State</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label id="message-label" for="txtMessage">
                            *Message:</label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtMessage" Rows="5" cols="100" runat="server" TextMode="MultiLine"
                            onkeypress="return this.value.length<=500"></asp:TextBox><asp:RequiredFieldValidator
                                ID="rfvMessage" runat="server" ControlToValidate="txtMessage" ErrorMessage="Message is required."
                                ValidationGroup="DataEntry"><img alt="!" src="images/validationerror.png" /></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" />
                    </td>
                </tr>
            </table>
            <asp:EntityDataSource ID="CalibrusEntityDataSource" runat="server" ConnectionString="name=CalibrusEntities"
                DefaultContainerName="CalibrusEntities" EnableFlattening="false" EntitySetName="tblStates">
            </asp:EntityDataSource>
            </form>
        </div>
        <div class="push">
        </div>
    </div>
    <footer>
		<div class="centered960">
			<ul>
				<li><a href="privacy-copyright.html">Privacy Policy</a></li>
				<li><a href="contact-us.aspx">Contact Us</a></li>
			</ul>
			<p><a href="privacy-copyright.html">Copyright, Calibrus Call Center Services, LLC. All rights reserved.</a></p>
		</div>
	</footer>
    <script type="text/javascript">
        var llaJsHost = (("https:" == document.location.protocol) ? "https://" : "http://"); document.write(unescape("%3Cscript src='" + llaJsHost + "analytics.leadlifesolutions.net/3/lla.js' type='text/javascript'%3E%3C/script%3E")); 
    </script>
    <script type="text/javascript">
        _llat.scriptHost = "analytics.leadlifesolutions.net"; _llat.load("6c6b1092-f9c2-4bd9-8f2a-90f77ee207b3"); 
    </script>
</body>
</html>
