<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="E911_Default" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <title>Frontier E911 Call Search</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div style="width: 1000px;">
        <table border="0" cellpadding="3" cellspacing="0" rules="none">
            <tr>
                <td class="content">
                    Subscriber Id:
                </td>
                <td class="dataentry">
                    <asp:TextBox ID="txtSubscriberId" runat="server" CssClass="inputText" Width="100px"></asp:TextBox>
                    <cc1:FilteredTextBoxExtender ID="txtRecordLocator_FilteredTextBoxExtender" runat="server"
                        Enabled="True" FilterType="Numbers" TargetControlID="txtSubscriberId">
                    </cc1:FilteredTextBoxExtender>
                </td>
            </tr>
            <tr>
                <td class="content">
                    Name:
                </td>
                <td class="dataentry">
                    <asp:TextBox ID="txtName" runat="server" CssClass="inputText" Width="210px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="content">
                    Phone Number:
                </td>
                <td class="dataentry">
                    <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="inputText" Width="100px"
                        MaxLength="10"></asp:TextBox>
                    <cc1:FilteredTextBoxExtender ID="txtPhoneNumber_FilteredTextBoxExtender" runat="server"
                        Enabled="True" FilterType="Numbers" TargetControlID="txtPhoneNumber">
                    </cc1:FilteredTextBoxExtender>
                </td>
            </tr>
            <tr>
                <td class="content">
                    Date Range:
                </td>
                <td class="dataentry" style="width: 500px">
                    <asp:TextBox ID="txtStartDate" runat="server" CssClass="inputText" Width="100px"></asp:TextBox>
                    &nbsp;through&nbsp;
                    <asp:TextBox ID="txtEndDate" runat="server" CssClass="inputText" Width="100px"></asp:TextBox>
                    <cc1:CalendarExtender ID="ceStartDate" runat="server" TargetControlID="txtStartDate"
                        CssClass="MyCalendar">
                    </cc1:CalendarExtender>
                    <cc1:CalendarExtender ID="ceEndDate" runat="server" TargetControlID="txtEndDate"
                        CssClass="MyCalendar">
                    </cc1:CalendarExtender>
                    <asp:CompareValidator ID="cvStartDate" runat="server" ControlToValidate="txtStartDate"
                        ErrorMessage="Start Date is invalid." Operator="DataTypeCheck" Type="Date" ValidationGroup="Report"
                        Display="Dynamic"><img alt="!" src="../images/validationerror.png" /></asp:CompareValidator>
                    <asp:CompareValidator ID="cvEndDate" runat="server" ControlToValidate="txtEndDate"
                        ErrorMessage="End Date is invalid." Operator="DataTypeCheck" Type="Date" ValidationGroup="Reporting"
                        Display="Dynamic"><img alt="!" src="../images/validationerror.png" /></asp:CompareValidator>
                    <asp:CustomValidator ID="custvBothDates" runat="server" ErrorMessage="Enter both a Start Date and End Date."
                        OnServerValidate="custvBothDates_ServerValidate" ValidationGroup="Reporting"><img alt="!" src="../images/validationerror.png" /></asp:CustomValidator>
                    <asp:CustomValidator ID="custvDataRange" runat="server" ErrorMessage="Limit your search to 30 days."
                        ValidationGroup="Reporting" OnServerValidate="custvDataRange_ServerValidate"><img alt="!" src="../images/validationerror.png" /></asp:CustomValidator>
                </td>
            </tr>
            <tr>
                <td class="content">
                    Concern:
                </td>
                <td class="dataentry">
                    <asp:DropDownList ID="ddlConcern" runat="server" CssClass="inputText" Style="height: 25px;"
                        AppendDataBoundItems="True">
                        <asp:ListItem Selected="True"></asp:ListItem>
                        <asp:ListItem Value="Incorrect Telephone Number">Incorrect Telephone Number</asp:ListItem>
                        <asp:ListItem Value="Customer Refused">Customer Refused</asp:ListItem>
                        <asp:ListItem Value="Changed Mind">Changed Mind</asp:ListItem>
                        <asp:ListItem Value="Customer Confused">Customer Confused</asp:ListItem>
                        <asp:ListItem Value="Does Not Agree">Does Not Agree</asp:ListItem>
                        <asp:ListItem Value="Not 18 Years or Older">Not 18 Years or Older</asp:ListItem>
                        <asp:ListItem Value="Customer Had Questions">Customer Had Questions</asp:ListItem>
                        <asp:ListItem Value="No Answer">No Answer</asp:ListItem>
                        <asp:ListItem Value="Busy Signal">Busy Signal</asp:ListItem>
                        <asp:ListItem Value="Answering Machine">Answering Machine</asp:ListItem>
                        <asp:ListItem Value="Customer Not Available">Customer Not Available</asp:ListItem>
                        <asp:ListItem Value="Customer Disconnected">Customer Disconnected</asp:ListItem>
                        <asp:ListItem Value="Verified">Verified</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="2" class="footer" style="width: 950px;">
                    <asp:CustomValidator ID="custvSpecific" runat="server" ErrorMessage="Make your search more specific."
                        OnServerValidate="custvSpecific_ServerValidate" ValidationGroup="Reporting">&nbsp;</asp:CustomValidator>
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="submitButton" OnClick="btnSubmit_Click" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
