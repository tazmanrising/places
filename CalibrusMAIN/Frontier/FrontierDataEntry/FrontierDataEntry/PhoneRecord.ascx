<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PhoneRecord.ascx.cs" Inherits="PhoneRecord" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>

    <table cellpadding="2" style="width: 800px; text-align: left;">   
        <tr id="serviceHeading" runat="server" style="text-align: center;">
            <td style="width: 200px;" colspan="2"></td>
            <td style="text-align: center;width: 75px;">PLOC Change</td>
            <td style="text-align: center;width: 75px;">ILP/Intra</td>
            <td style="text-align: center;width: 75px;">PIC/Inter</td>
            <td style="text-align: center;width: 75px;">PLOC Freeze</td>
            <td style="text-align: center;width: 75px;">ILP/Intra Freeze</td>
            <td style="text-align: center;width: 75px;">PIC/Inter Freeze</td>
        </tr>               
        <tr style="padding-bottom: 10px;">
            <td style="width: 50px;">
                <asp:CustomValidator ID="cvEmtyOrder" runat="server" 
                    ErrorMessage="No Services Selected." onservervalidate="cvEmtyOrder_ServerValidate"   
                    ValidationGroup="DataEntry" Display="Dynamic"><img alt="!" src="images/validationerror.png" /></asp:CustomValidator>
                TN:
            </td>
            <td style="width: 150px;">
                <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="inputText" Width="110px"></asp:TextBox>
                <cc1:MaskedEditExtender ID="txtPhoneNumber_MaskedEditExtender" runat="server" 
                    CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" 
                    CultureDateFormat="" CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                    CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" 
                    Mask="(999) 999-9999" TargetControlID="txtPhoneNumber" 
                    AutoComplete="False" ClearMaskOnLostFocus="True">
                </cc1:MaskedEditExtender>
                <asp:RequiredFieldValidator ID="rfPhoneNumber" runat="server" 
                    ControlToValidate="txtPhoneNumber" Display="Dynamic" Enabled="True" 
                    ErrorMessage="TN is required." ValidationGroup="DataEntry"><img alt="!" src="images/validationerror.png" /></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="regexPhoneNumber" runat="server" 
                    ControlToValidate="txtPhoneNumber" Display="Dynamic" 
                    ErrorMessage="TN is invalid." ValidationExpression="\d{10}"
                    ValidationGroup="DataEntry"><img alt="!" src="images/validationerror.png" /></asp:RegularExpressionValidator>
            </td>                                     
            <td style="text-align: center;width: 75px;">
                <asp:CheckBox ID="chkLocal" runat="server" Text="" /> 
            </td>   
            <td style="text-align: center;width: 75px;">
                <asp:CheckBox ID="chkIntralata" runat="server" Text="" /> 
            </td>   
            <td style="text-align: center;width: 75px;">
                <asp:CheckBox ID="chkInterlata" runat="server" Text="" /> 
            </td>                                             
            <td style="text-align: center;width: 75px;">
                <asp:CheckBox ID="chkLocalFreeze" runat="server" Text="" /> 
            </td>   
            <td style="text-align: center;width: 75px;">
                <asp:CheckBox ID="chkIntralataFreeze" runat="server" Text="" /> 
            </td>   
            <td style="text-align: center;width: 75px;">
                <asp:CheckBox ID="chkInterlataFreeze" runat="server" Text="" /> 
            </td>                                       
        </tr>
    </table>