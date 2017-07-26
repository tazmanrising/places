<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="SnetReports_Default" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <title>Frontier-SNET Call Search</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div style="width: 1000px;">
        <table border="0" cellpadding="3" cellspacing="0" rules="none">
            <tr>
                <td class="content">
                    Record Locator:
                </td>
                <td class="dataentry">
                    <asp:TextBox ID="txtRecordLocator" runat="server" CssClass="inputText" 
                        Width="80px" MaxLength="8"></asp:TextBox>
                    <cc1:filteredtextboxextender ID="txtRecordLocator_FilteredTextBoxExtender" 
                        runat="server" Enabled="True" FilterType="Numbers" 
                        TargetControlID="txtRecordLocator">
                    </cc1:filteredtextboxextender>
                    <asp:RangeValidator ID="rcRecordLocator" runat="server" 
                        ErrorMessage="Enter a valid Record Locator." 
                        ControlToValidate="txtRecordLocator" MaximumValue="92000000" MinimumValue="1" 
                        Type="Integer" ValidationGroup="Reporting"><img src="../images/validationerror.png" alt="!"/></asp:RangeValidator>
                </td>
            </tr>
            <tr>
                <td class="content">
                    Phone Number:
                </td>
                <td class="dataentry">
                    <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="inputText" 
                        Width="110px"></asp:TextBox>
                    <cc1:maskededitextender ID="txtPhoneNumber_MaskedEditExtender" runat="server" 
                        CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" 
                        CultureDateFormat="" CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                        CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" 
                        Mask="(999) 999-9999" TargetControlID="txtPhoneNumber" 
                        AutoComplete="False" ClearMaskOnLostFocus="True">
                    </cc1:maskededitextender>
                    <asp:RegularExpressionValidator ID="regexPhoneNumber" runat="server" 
                        ControlToValidate="txtPhoneNumber" Display="Dynamic" 
                        ErrorMessage="Phone Number is invalid." ValidationExpression="\d{10}"
                        ValidationGroup="DataEntry"><img src="../images/validationerror.png" /></asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <td class="content">
                    TPV Agent ID:
                </td>
                <td class="dataentry">
                    <asp:TextBox ID="txtTpvAgentId" runat="server" CssClass="inputText" 
                        Width="110px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="content">
                    Sales Agent ID:
                </td>
                <td class="dataentry">
                    <asp:TextBox ID="txtSalesAgentID" runat="server" CssClass="inputText" Width="110px"></asp:TextBox>
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
                    <cc1:calendarextender ID="ceStartDate" runat="server" 
                        TargetControlID="txtStartDate" CssClass="MyCalendar">
                    </cc1:calendarextender>
                    <cc1:calendarextender ID="ceEndDate" runat="server" 
                        TargetControlID="txtEndDate" CssClass="MyCalendar">
                    </cc1:calendarextender>
                    <asp:CompareValidator ID="cvStartDate" runat="server" 
                        ControlToValidate="txtStartDate" ErrorMessage="Start Date is invalid." 
                        Operator="DataTypeCheck" Type="Date" ValidationGroup="Report" 
                        Display="Dynamic"><img alt="!" src="../images/validationerror.png" /></asp:CompareValidator>
                    <asp:CompareValidator ID="cvEndDate" runat="server" 
                        ControlToValidate="txtEndDate" ErrorMessage="End Date is invalid." 
                        Operator="DataTypeCheck" Type="Date" ValidationGroup="Reporting" 
                        Display="Dynamic"><img alt="!" src="../images/validationerror.png" /></asp:CompareValidator>
                    <asp:CustomValidator ID="custvBothDates" runat="server" 
                        ErrorMessage="Enter both a Start Date and End Date." 
                        onservervalidate="custvBothDates_ServerValidate" ValidationGroup="Reporting"><img alt="!" src="../images/validationerror.png" /></asp:CustomValidator>
                    <asp:CustomValidator ID="custvDataRange" runat="server" 
                        ErrorMessage="Limit your search to 30 days." ValidationGroup="Reporting" 
                        onservervalidate="custvDataRange_ServerValidate"><img alt="!" src="../images/validationerror.png" /></asp:CustomValidator>
                </td>
            </tr>
            <tr>
                <td class="content">
                    Concern:
                </td>
                <td class="dataentry">
                    <asp:DropDownList ID="ddlConcern" runat="server" CssClass="inputText" 
                        style="height: 25px;" AppendDataBoundItems="True">
                        <asp:ListItem Selected="True"></asp:ListItem>
                        <asp:ListItem value="Verified">Verified</asp:ListItem>
                        <asp:ListItem value="SBC/AT&T Name Confusion">SBC/AT&T Name Confusion</asp:ListItem>
                        <asp:ListItem value="Refused not interested">Refused not interested</asp:ListItem>
                        <asp:ListItem value="Refused changed mind">Refused changed mind</asp:ListItem>
                        <asp:ListItem value="Refused too expensive">Refused too expensive</asp:ListItem>
                        <asp:ListItem value="Refused wants to think about it">Refused wants to think about it</asp:ListItem>
                        <asp:ListItem value="Refused only wanted information">Refused only wanted information</asp:ListItem>
                        <asp:ListItem value="Refused verification">Refused verification</asp:ListItem>
                        <asp:ListItem value="Refused local toll verification">Refused local toll verification</asp:ListItem>
                        <asp:ListItem value="Refused to give Security Info">Refused to give Security Info</asp:ListItem>
                        <asp:ListItem value="Incorrect Information">Incorrect Information</asp:ListItem>
                        <asp:ListItem value="Foreign Language">Foreign Language</asp:ListItem>
                        <asp:ListItem value="Confused">Confused</asp:ListItem>
                        <asp:ListItem value="TM rep abandoned">TM rep abandoned</asp:ListItem>
                        <asp:ListItem value="Customer abandoned">Customer abandoned</asp:ListItem>
                        <asp:ListItem value="Prank call">Prank call</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="2" class="footer" style="width: 950px;">
                    <asp:CustomValidator ID="custvSpecific" runat="server" 
                        ErrorMessage="Make your search more specific." 
                        onservervalidate="custvSpecific_ServerValidate" ValidationGroup="Reporting">&nbsp;</asp:CustomValidator>
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" 
                        CssClass="submitButton" onclick="btnSubmit_Click"  />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>


