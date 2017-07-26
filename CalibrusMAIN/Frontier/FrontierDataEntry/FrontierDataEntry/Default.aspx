<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>
<%@ Register Src="~/PhoneRecord.ascx" TagName="PhoneRecord" TagPrefix="uc1" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
        <title>Frontier Data Entry</title>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div>
        <table border="0" cellpadding="3" cellspacing="0" rules="none">
            <tr>
                <td class="content">
                    Consultant ID:
                </td>
                <td class="dataentry">
                    <asp:TextBox ID="txtAgentId" runat="server" CssClass="inputText" 
                        Width="150px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfAgentId" runat="server" 
                        ControlToValidate="txtAgentId" ErrorMessage="Consultant ID is required." 
                        ValidationGroup="DataEntry"><img alt="!" src="images/validationerror.png" /></asp:RequiredFieldValidator>
                </td>
                <td class="content">
                    State:
                </td>
                <td class="dataentry">
                    <select id="ddlState" runat="server" class="inputText" style="height: 26px;">
                        <option selected="selected" value=""></option>
                        <option value="AL">Alabama</option>
                        <option value="AK">Alaska</option>
                        <option value="AZ">Arizona</option>
                        <option value="AR">Arkansas</option>
                        <option value="CA">California</option>
                        <option value="CO">Colorado</option>
                        <option value="CT">Connecticut</option>
                        <option value="DE">Delware</option>
                        <option value="FL">Florida</option>
                        <option value="GA">Georgia</option>
                        <option value="HI">Hawaii</option>
                        <option value="ID">Idaho</option>
                        <option value="IL">Illinois</option>
                        <option value="IN">Indiana</option>
                        <option value="IA">Iowa</option>
                        <option value="KS">Kansas</option>
                        <option value="KY">Kentucky</option>
                        <option value="LA">Louisiana</option>
                        <option value="ME">Maine</option>
                        <option value="MD">Maryland</option>
                        <option value="MA">Massachusetts</option>
                        <option value="MI">Michigan</option>
                        <option value="MN">Minnesota</option>
                        <option value="MS">Mississippi</option>
                        <option value="MO">Missouri</option>
                        <option value="MT">Montana</option>
                        <option value="NE">Nebraska</option>
                        <option value="NV">Nevada</option>
                        <option value="NH">New Hampshire</option>
                        <option value="NJ">New Jersey</option>
                        <option value="NM">New Mexico</option>
                        <option value="NY">New York</option>
                        <option value="NC">North Carolina</option>
                        <option value="ND">North Dakota</option>
                        <option value="OH">Ohio</option>
                        <option value="OK">Oklahoma</option>
                        <option value="OR">Oregon</option>
                        <option value="PA">Pennsylvania</option>
                        <option value="RI">Rhode Island</option>
                        <option value="SC">South Carolina</option>
                        <option value="SD">South Dakota</option>
                        <option value="TN">Tennessee</option>
                        <option value="TX">Texas</option>
                        <option value="UT">Utah</option>
                        <option value="VT">Vermont</option>
                        <option value="VA">Virgina</option>
                        <option value="WA">Washington</option>
                        <option value="WV">West Virginia</option>
                        <option value="WI">Wisconsin</option>
                        <option value="WY">Wyoming</option>
                    </select>
                    <asp:RequiredFieldValidator ID="rfState" runat="server" 
                        ControlToValidate="ddlState" ErrorMessage="State is required." 
                        ValidationGroup="DataEntry"><img alt="!" src="images/validationerror.png" /></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="content">
                    Customer First Name:
                </td>
                <td class="dataentry">
                    <asp:TextBox ID="txtCustomerFirstName" runat="server" CssClass="inputText" 
                        Width="210px" MaxLength="50"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfCustomerFirstName" runat="server" 
                        ControlToValidate="txtCustomerFirstName" ErrorMessage="Customer First Name is required." 
                        ValidationGroup="DataEntry"><img alt="!" src="images/validationerror.png" /></asp:RequiredFieldValidator>
                </td>
                <td class="content">
                    Customer Last Name:
                </td>
                <td class="dataentry">
                    <asp:TextBox ID="txtCustomerLastName" runat="server" CssClass="inputText" 
                        Width="210px" MaxLength="50"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfCustomerLastName" runat="server" 
                        ControlToValidate="txtCustomerLastName" ErrorMessage="Customer Last Name is required." 
                        ValidationGroup="DataEntry"><img alt="!" src="images/validationerror.png" /></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="content">
                    Billing First Name:
                </td>
                <td class="dataentry">
                    <asp:TextBox ID="txtBillingFirstName" runat="server" CssClass="inputText" Width="210px" MaxLength="50"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                        ControlToValidate="txtBillingFirstName" ErrorMessage="Billing First Name is required." 
                        ValidationGroup="DataEntry"><img alt="!" src="images/validationerror.png" /></asp:RequiredFieldValidator>
                </td>
                <td class="content">
                    Billing Last Name:
                </td>
                <td class="dataentry">
                    <asp:TextBox ID="txtBillingLastName" runat="server" CssClass="inputText" Width="210px" MaxLength="50"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfClaimNumber2" runat="server" 
                        ControlToValidate="txtBillingLastName" ErrorMessage="Billing Last Name is required." 
                        ValidationGroup="DataEntry"><img alt="!" src="images/validationerror.png" /></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="content">
                    Company Name:
                </td>
                <td class="dataentry">
                    <asp:TextBox ID="txtCompanyName" runat="server" CssClass="inputText" Width="210px" MaxLength="100"></asp:TextBox>
                </td>
                <td class="content">
                    Product:
                </td>
                <td class="dataentry">
                    <asp:TextBox ID="txtProduct" runat="server" CssClass="inputText" Width="210px" MaxLength="20"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="content">
                    Type of Account:
                </td>
                <td class="dataentry" colspan="3">
                    <asp:RadioButton ID="rbAccountTypeBusiness" runat="server" CssClass="inputText" GroupName="AccountType" />
                    Business
                    <asp:RadioButton ID="rbAccountTypeResidential" runat="server" CssClass="inputText" GroupName="AccountType" />
                    Residential
                    <asp:CustomValidator ID="cvAccountTYpe" runat="server" 
                        ErrorMessage="Type of Account is required." 
                        onservervalidate="cvAccountTYpe_ServerValidate"><img alt="!" src="images/validationerror.png" /></asp:CustomValidator>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <uc1:PhoneRecord ID="Tn1" runat="server" ShowHeader="true" TnRequired="true" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <uc1:PhoneRecord ID="Tn2" runat="server" ShowHeader="false" TnRequired="false" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <uc1:PhoneRecord ID="Tn3" runat="server" ShowHeader="false" TnRequired="false" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <uc1:PhoneRecord ID="Tn4" runat="server" ShowHeader="false" TnRequired="false" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <uc1:PhoneRecord ID="Tn5" runat="server" ShowHeader="false" TnRequired="false" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <uc1:PhoneRecord ID="Tn6" runat="server" ShowHeader="false" TnRequired="false" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <uc1:PhoneRecord ID="Tn7" runat="server" ShowHeader="false" TnRequired="false" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <uc1:PhoneRecord ID="Tn8" runat="server" ShowHeader="false" TnRequired="false" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <uc1:PhoneRecord ID="Tn9" runat="server" ShowHeader="false" TnRequired="false" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <uc1:PhoneRecord ID="Tn10" runat="server" ShowHeader="false" TnRequired="false" />
                </td>
            </tr>
            <tr>
                <td class="footer" colspan="4">
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" 
                        CssClass="submitButton" onclick="btnSubmit_Click" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
