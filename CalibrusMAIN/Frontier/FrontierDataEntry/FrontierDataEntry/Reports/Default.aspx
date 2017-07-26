<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Reports_Default" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <title>Frontier Call Search</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div style="width: 1000px;">
        <table border="0" cellpadding="3" cellspacing="0" rules="none">
            <tr>
                <td class="content">
                    Record Locator:
                </td>
                <td class="dataentry">
                    <asp:TextBox ID="txtRecordLocator" runat="server" CssClass="inputText" Width="80px"
                        MaxLength="8"></asp:TextBox>
                    <cc1:FilteredTextBoxExtender ID="txtRecordLocator_FilteredTextBoxExtender" runat="server"
                        Enabled="True" FilterType="Numbers" TargetControlID="txtRecordLocator">
                    </cc1:FilteredTextBoxExtender>
                    <asp:RangeValidator ID="rcRecordLocator" runat="server" ErrorMessage="Enter a valid Record Locator."
                        ControlToValidate="txtRecordLocator" MaximumValue="9200000" MinimumValue="1"
                        Type="Integer" ValidationGroup="Reporting"><img src="../images/validationerror.png" alt="!"/></asp:RangeValidator>
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
                    TPV Agent ID:
                </td>
                <td class="dataentry">
                    <asp:TextBox ID="txtTpvAgentId" runat="server" CssClass="inputText" Width="110px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="content">
                    Billing Name:
                </td>
                <td class="dataentry">
                    <asp:TextBox ID="txtBillingName" runat="server" CssClass="inputText" Width="210px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="content">
                    Company Name:
                </td>
                <td class="dataentry">
                    <asp:TextBox ID="txtCompanyName" runat="server" CssClass="inputText" Width="210px"></asp:TextBox>
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
                        <asp:ListItem Value="Abandoned Call">Abandoned Call</asp:ListItem>
                        <asp:ListItem Value="Birth/SSN">Birth/SSN</asp:ListItem>
                        <asp:ListItem Value="Changed Mind">Changed Mind</asp:ListItem>
                        <asp:ListItem Value="Customer Hung Up">Customer Hung Up</asp:ListItem>
                        <asp:ListItem Value="Dropped at Transfer">Dropped at Transfer</asp:ListItem>
                        <asp:ListItem Value="Language/Confused">Language/Confused</asp:ListItem>
                        <asp:ListItem Value="Refused Verification">Refused Verification</asp:ListItem>
                        <asp:ListItem Value="Think/Spouse">Think/Spouse</asp:ListItem>
                        <asp:ListItem Value="Too Expensive">Too Expensive</asp:ListItem>
                        <asp:ListItem Value="All Failures">All Failures</asp:ListItem>
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
