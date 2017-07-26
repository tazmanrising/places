<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="DataEntry.aspx.cs" Inherits="DataEntry" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <title>Liberty Data Entry</title>
    <script type="text/javascript" src="Scripts/jquery.maskedinput.js"></script>
    <script language="javascript" type="text/javascript">
        function pageLoad(sender, args) {

            $(document).ready(function () {

                //customer phone mask
                $("#<%=txtPhoneNumber.ClientID%>").mask("(999) 999-9999");

                //EnergyGRT mask
                $("#<%=txtEnergyGRT.ClientID%>").mask("0.9999?9", { placeholder: "" });

                //rate mask
                $("#<%=txtRate.ClientID%>").mask("0.9999?9", { placeholder: "" });
                $("#<%=txtGasRate.ClientID%>").mask("0.9999?9", { placeholder: "" });

                //rate effective date mask
                $("#<%=txtRateEffectiveDate.ClientID%>").mask("99/99/9999");
                $("#<%=txtGasRateEffectiveDate.ClientID%>").mask("99/99/9999");

                //rate expiration date mask
                $("#<%=txtRateExpirationDate.ClientID%>").mask("99/99/9999");

                //txtSubTermMonth1Start
                $("#<%=txtSubTermMonth1Start.ClientID%>").mask("9?9");
                //txtSubTermMonth1End
                $("#<%=txtSubTermMonth1End.ClientID%>").mask("9?9");
                //txtSubTermMonth1Rate
                $("#<%=txtSubTermMonth1Rate.ClientID%>").mask("0.9999?9", { placeholder: "" });

                //txtSubTermMonth2Start
                $("#<%=txtSubTermMonth2Start.ClientID%>").mask("9?9");
                //txtSubTermMonth2End
                $("#<%=txtSubTermMonth2End.ClientID%>").mask("9?9");
                //txtSubTermMonth2Rate
                $("#<%=txtSubTermMonth2Rate.ClientID%>").mask("0.9999?9", { placeholder: "" });

                //txtSubTermMonth3Start
                $("#<%=txtSubTermMonth3Start.ClientID%>").mask("9?9");
                //txtSubTermMonth3End
                $("#<%=txtSubTermMonth3End.ClientID%>").mask("9?9");
                //txtSubTermMonth3Rate
                $("#<%=txtSubTermMonth3Rate.ClientID%>").mask("0.9999?9", { placeholder: "" });

                //txtSubTermMonth4Start
                $("#<%=txtSubTermMonth4Start.ClientID%>").mask("9?9");
                //txtSubTermMonth4End
                $("#<%=txtSubTermMonth4End.ClientID%>").mask("9?9");
                //txtSubTermMonth4Rate
                $("#<%=txtSubTermMonth4Rate.ClientID%>").mask("0.9999?9", { placeholder: "" });

            });

        }

    </script>
    <link href="Styles/Site.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
    <%--Main Form--%>
    <div id="divMainForm" runat="server" class="row col-md-10 col-md-offset-1">
        <div class="panel panel-info">
            <div class="panel-heading">
                <h3 class="panel-title">
                    Main Account</h3>
            </div>
            <div class="panel-body text-center">
                <div class="form-group">
                    <label class="col-sm-5 control-label input-sm">
                        What is your Sales Channel ID?</label>
                </div>
                <div class="form-group">
                    <label for="ddlSalesChannelId" class="col-sm-5 control-label input-sm">
                        Sales Channel ID:</label>
                    <div class="col-sm-3">
                        <asp:DropDownList ID="ddlSalesChannelId" runat="server" CssClass="form-control input-sm"
                            AppendDataBoundItems="True">
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-sm-5 control-label input-sm">
                        What is your Agent ID?</label>
                </div>
                <div class="form-group">
                    <label for="txtSalesAgentId" class="col-sm-5 control-label input-sm">
                        Agent ID:</label>
                    <div class="col-sm-3">
                        <asp:TextBox ID="txtSalesAgentId" runat="server" CssClass="form-control input-sm input-sm"></asp:TextBox>
                    </div>
                    <div class="col-sm-4">
                        <asp:RequiredFieldValidator ID="rfvAgentId" runat="server" ControlToValidate="txtSalesAgentId"
                            ErrorMessage="AgentId is required." ValidationGroup="DataEntry" SetFocusOnError="True"><label for="txtSalesAgentId" class="has-error"><img alt="!" src="images/validationerror.png" /> AgentId is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-sm-5 control-label input-sm">
                        What is the customer's phone number?</label>
                </div>
                <div class="form-group">
                    <label for="txtPhoneNumber" class="col-sm-5 control-label input-sm">
                        Phone Number:</label>
                    <div class="col-sm-3">
                        <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </div>
                    <div class="col-sm-1">
                        <asp:ImageButton ID="imgBtnPhoneSearch" runat="server" AlternateText="Search Customer Phone Number"
                            ImageUrl="~/images/search.png" OnClick="phoneSearch_Click" />
                    </div>
                    <div class="col-sm-3">
                        <asp:RequiredFieldValidator ID="rfvPhoneNumber" runat="server" ControlToValidate="txtPhoneNumber"
                            ErrorMessage="Phone Number is required." ValidationGroup="DataEntry" SetFocusOnError="True"><label for="txtPhoneNumber" class="has-error"><img alt="!" src="images/validationerror.png" /> Phone Number is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <label for="ddlMarketState" class="col-sm-5 control-label input-sm">
                        What is the Market?</label>
                    <div class="col-sm-3">
                        <asp:DropDownList ID="ddlMarketState" runat="server" CssClass="form-control input-sm"
                            DataSourceID="MarketStateDataSource" DataTextField="State" DataValueField="MarketStateId"
                            AppendDataBoundItems="True" AutoPostBack="True" OnSelectedIndexChanged="ddlMarketState_SelectedIndexChanged">
                            <asp:ListItem Text="<-Select One->" Value="0" Selected="True"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-sm-3">
                        <asp:RequiredFieldValidator ID="rfvMarketState" runat="server" ControlToValidate="ddlMarketState"
                            ErrorMessage="Market State is required." ValidationGroup="DataEntry" SetFocusOnError="True"><label for="ddlMarketState" class="has-error"><img alt="!" src="images/validationerror.png" /> Market State is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div id="divUtilityChoice" runat="server" visible="false" class="form-group">
                    <label for="ddlUtilityChoice" class="col-sm-5 control-label input-sm">
                        Is this Electric or Electric & Gas?</label>
                    <div class="col-sm-3">
                        <asp:DropDownList ID="ddlUtilityChoice" runat="server" CssClass="form-control input-sm"
                            DataValueField="Active" AppendDataBoundItems="True" AutoPostBack="True" OnSelectedIndexChanged="ddlUtilityChoice_SelectedIndexChanged">
                            <asp:ListItem Text="<-Select One->" Value="" Selected="True"></asp:ListItem>
                            <asp:ListItem Value="Electric" Text="Electric"></asp:ListItem>
                            <asp:ListItem Value="Gas" Text="Gas"></asp:ListItem>
                            <asp:ListItem Value="Both" Text="Electric & Gas"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-sm-3">
                        <asp:RequiredFieldValidator ID="rfvUtilityChoice" runat="server" ControlToValidate="ddlUtilityChoice"
                            ErrorMessage="Utility Choice is required." ValidationGroup="DataEntry" SetFocusOnError="True"><label for="ddlUtilityChoice" class="has-error"><img alt="!" src="images/validationerror.png" /> Utility Choice is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div id="divElectricUtility" runat="server" class="form-group">
                    <label for="ddlMarketUtility" class="col-sm-5 control-label input-sm">
                        What is the Utility?</label>
                    <div class="col-sm-3">
                        <asp:DropDownList ID="ddlMarketUtility" runat="server" CssClass="form-control input-sm"
                            DataTextField="Utility" DataValueField="MarketUtilityId" AppendDataBoundItems="True"
                            AutoPostBack="True" OnSelectedIndexChanged="ddlMarketUtility_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>
                    <div class="col-sm-3">
                        <asp:RequiredFieldValidator ID="rfvMarketUtility" runat="server" ControlToValidate="ddlMarketUtility"
                            ErrorMessage="Utility is required." ValidationGroup="DataEntry" SetFocusOnError="True"><label for="ddlMarketUtility" class="has-error"><img alt="!" src="images/validationerror.png" /> Utility is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div id="divElectricMarketProduct" runat="server" class="form-group">
                    <label for="ddlMarketProduct" class="col-sm-5 control-label input-sm">
                        What is the correct product for this verification?</label>
                    <div class="col-sm-3">
                        <asp:DropDownList ID="ddlMarketProduct" runat="server" CssClass="form-control input-sm"
                            DataTextField="Product" DataValueField="MarketProductId" AppendDataBoundItems="True"
                            OnSelectedIndexChanged="ddlMarketProduct_SelectedIndexChanged" AutoPostBack="True">
                        </asp:DropDownList>
                    </div>
                    <div class="col-sm-3">
                        <asp:RequiredFieldValidator ID="rfvMarketProduct" runat="server" ControlToValidate="ddlMarketProduct"
                            ErrorMessage="Product is required." ValidationGroup="DataEntry" SetFocusOnError="True"><label for="ddlMarketProduct" class="has-error"><img alt="!" src="images/validationerror.png" /> Product is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div id="divDeliveryZone" runat="server" visible="false" class="form-group">
                    <label for="ddlDeliveryZone" class="col-sm-5 control-label input-sm">
                        What is the Delivery Zone?</label>
                    <div class="col-sm-3">
                        <asp:DropDownList ID="ddlDeliveryZone" runat="server" CssClass="form-control input-sm"
                            DataTextField="Name" DataValueField="DeliveryZoneId" AppendDataBoundItems="True"
                            AutoPostBack="True">
                        </asp:DropDownList>
                    </div>
                    <div class="col-sm-3">
                        <asp:RequiredFieldValidator ID="rfvDeliveryZone" runat="server" ControlToValidate="ddlDeliveryZone"
                            ErrorMessage="Delivery Zone is required." ValidationGroup="DataEntry" SetFocusOnError="True"><label for="ddlDeliveryZone" class="has-error"><img alt="!" src="images/validationerror.png" /> Delivery Zone is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-sm-9 control-label input-sm">
                        What is the name of the person authorized to enroll the account(s) with Liberty
                        Power?</label>
                </div>
                <div class="form-group">
                    <label for="txtAuthorizationFirstName" class="col-sm-5 control-label input-sm">
                        First Name:</label>
                    <div class="col-sm-3">
                        <asp:TextBox ID="txtAuthorizationFirstName" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </div>
                    <div class="col-sm-3">
                        <asp:RequiredFieldValidator ID="rfvAuthorizationFirstName" runat="server" ControlToValidate="txtAuthorizationFirstName"
                            ErrorMessage="First Name is required." ValidationGroup="DataEntry" SetFocusOnError="True"><label for="txtAuthorizationFirstName" class="has-error"><img alt="!" src="images/validationerror.png" /> First Name is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <label for="txtAuthorizationLasttName" class="col-sm-5 control-label input-sm">
                        Last Name:</label>
                    <div class="col-sm-3">
                        <asp:TextBox ID="txtAuthorizationLasttName" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </div>
                    <div class="col-sm-3">
                        <asp:RequiredFieldValidator ID="rfvAuthorizationLastName" runat="server" ControlToValidate="txtAuthorizationLasttName"
                            ErrorMessage="Last Name is required." ValidationGroup="DataEntry" SetFocusOnError="True"><label for="txtAuthorizationLasttName" class="has-error"><img alt="!" src="images/validationerror.png" /> Last Name is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <label for="txtEmail" class="col-sm-5 control-label input-sm">
                        Email:</label>
                    <div class="col-sm-3">
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </div>
                    <div class="col-sm-3">
                        <%--<asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                            ErrorMessage="Email is required." ValidationGroup="DataEntry" SetFocusOnError="True"><label for="txtEmail" class="has-error"><img alt="!" src="images/validationerror.png" /> Email is required.</label></asp:RequiredFieldValidator>--%>
                    </div>
                </div>
                <div id="divElectricBusiness" runat="server" class="form-group">
                    <div class="form-group">
                        <label class="col-sm-5 control-label input-sm">
                            What is theBusiness Tax ID?</label>
                    </div>
                    <div class="form-group">
                        <label for="txtBusinessTaxId" class="col-sm-5 control-label input-sm">
                            Business Tax ID(FEID):</label>
                        <div class="col-sm-3">
                            <asp:TextBox ID="txtBusinessTaxId" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        </div>
                    </div>
                    <div id="divBusinessNameLabel" visible="false" class="form-group" runat="server">
                        <label class="col-sm-8 control-label input-sm">
                            What is the legal name of the business enrolling with Liberty Power?</label>
                    </div>
                    <div id="divBusinessNameControl" visible="false" class="form-group" runat="server">
                        <label for="txtBusinessName" class="col-sm-5 control-label input-sm">
                            Business Name:</label>
                        <div class="col-sm-3">
                            <asp:TextBox ID="txtBusinessName" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        </div>
                        <div class="col-sm-3">
                            <asp:RequiredFieldValidator ID="rfvBusinessName" runat="server" ControlToValidate="txtBusinessName"
                                ErrorMessage="Business Name is required." ValidationGroup="DataEntry" Enabled="false"
                                SetFocusOnError="True"><label for="txtBusinessName" class="has-error"><img alt="!" src="images/validationerror.png" /> Business Name is required.</label></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div id="divEnergyGRT" runat="server" visible="false" class="form-group">
                        <label for="txtEnergyGRT" class="col-sm-5 control-label input-sm">
                            Energy Gross Recipt Tax Rate:</label>
                        <div class="col-sm-2">
                            <asp:TextBox ID="txtEnergyGRT" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        </div>
                        <div class="col-sm-1">
                            <label for="txtEnergyGRT" class="col-sm-2 control-label input-sm">
                                (0.XXXXX)</label>
                        </div>
                        <div class="col-sm-3">
                            <asp:RequiredFieldValidator ID="rfvEnergyGRT" runat="server" ControlToValidate="txtEnergyGRT"
                                ErrorMessage="Energy Gross Recipt Tax Rate is required." ValidationGroup="DataEntry"
                                SetFocusOnError="True"><label for="txtEnergyGRT" class="has-error"><img alt="!" src="images/validationerror.png" /> Energy Gross Recipt Tax Rate is required.</label></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div id="divContractId" runat="server" visible="false" class="form-group">
                        <label for="txtContractId" class="col-sm-5 control-label input-sm">
                            Contract ID:</label>
                        <div class="col-sm-3">
                            <asp:TextBox ID="txtContractId" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        </div>
                        <div class="col-sm-3">
                            <asp:RequiredFieldValidator ID="rfvContractId" runat="server" ControlToValidate="txtContractId"
                                ErrorMessage="Contract ID is required." ValidationGroup="DataEntry" SetFocusOnError="True"><label for="txtContractId" class="has-error"><img alt="!" src="images/validationerror.png" /> Contract ID is required.</label></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div id="divEFLVersionCode" runat="server" visible="false" class="form-group">
                        <label for="txtEFLVersionCode" class="col-sm-5 control-label input-sm">
                            EFL Version Code:</label>
                        <div class="col-sm-3">
                            <asp:TextBox ID="txtEFLVersionCode" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        </div>
                        <div class="col-sm-3">
                            <asp:RequiredFieldValidator ID="rfvEFLVersionCode" runat="server" ControlToValidate="txtEFLVersionCode"
                                ErrorMessage="EFL Version Code is required." ValidationGroup="DataEntry" SetFocusOnError="True"><label for="txtEFLVersionCode" class="has-error"><img alt="!" src="images/validationerror.png" /> EFL Version Code is required.</label></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div id="divSwitchOrMoveIn" runat="server" visible="false" class="form-group">
                        <label for="ddlSwitchOrMoveIn" class="col-sm-5 control-label input-sm">
                            Switch or Move In?</label>
                        <div class="col-sm-3">
                            <asp:DropDownList ID="ddlSwitchOrMoveIn" runat="server" CssClass="form-control input-sm"
                                DataValueField="Active" AppendDataBoundItems="True">
                                <asp:ListItem Text="<-Select One->" Value="" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="Switch" Text="Switch"></asp:ListItem>
                                <asp:ListItem Value="Move In" Text="Move In"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-sm-3">
                            <asp:RequiredFieldValidator ID="rfvSwitchOrMoveIn" runat="server" ControlToValidate="ddlSwitchOrMoveIn"
                                ErrorMessage="Switch or Move In is required." ValidationGroup="DataEntry" SetFocusOnError="True"><label for="ddlSwitchOrMoveIn" class="has-error"><img alt="!" src="images/validationerror.png" /> Switch or Move In is required.</label></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div id="divMaterialPreference" runat="server" visible="false" class="form-group">
                        <label for="ddlMaterialPreference" class="col-sm-5 control-label input-sm">
                            Preference for English or Spanish Material?</label>
                        <div class="col-sm-3">
                            <asp:DropDownList ID="ddlMaterialPreference" runat="server" CssClass="form-control input-sm"
                                DataValueField="Active" AppendDataBoundItems="True">
                                <asp:ListItem Text="<-Select One->" Value="" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="English" Text="English"></asp:ListItem>
                                <asp:ListItem Value="Spanish" Text="Spanish"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-sm-3">
                            <asp:RequiredFieldValidator ID="rfvMaterialPreference" runat="server" ControlToValidate="ddlMaterialPreference"
                                ErrorMessage="Preference for English or Spanish required." ValidationGroup="DataEntry"
                                SetFocusOnError="True"><label for="ddlMaterialPreference" class="has-error"><img alt="!" src="images/validationerror.png" /> Preference for English or Spanish is required.</label></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-8 control-label input-sm">
                            Is this account a SOHO account (Commercial with Residential meter)?</label>
                    </div>
                    <div class="form-group">
                        <label for="ddlSohoAccount" class="col-sm-5 control-label input-sm">
                            SOHO Account:</label>
                        <div class="col-sm-3">
                            <asp:DropDownList ID="ddlSohoAccount" runat="server" CssClass="form-control input-sm"
                                DataValueField="Active" AppendDataBoundItems="True">
                                <asp:ListItem Text="<-Select One->" Value="" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="Yes" Text="Yes"></asp:ListItem>
                                <asp:ListItem Value="No" Text="No"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
                <div id="divElectricAccountNumber" runat="server" class="form-group">
                    <div class="form-group">
                        <label class="col-sm-8 control-label input-sm">
                            What is the account number and corresponding service address?</label>
                    </div>
                    <div class="form-group">
                        <label for="txtAccountNumber" class="col-sm-5 control-label input-sm">
                            Account Number:</label>
                        <div class="col-sm-3">
                            <asp:TextBox ID="txtAccountNumber" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        </div>
                        <div class="col-sm-3">
                            <asp:RequiredFieldValidator ID="rfvAccountNumber" runat="server" ControlToValidate="txtAccountNumber"
                                ErrorMessage="Account Number is required." ValidationGroup="DataEntry" SetFocusOnError="True"
                                Display="Dynamic"><label for="txtAccountNumber" class="has-error"><img alt="!" src="images/validationerror.png" /> Account Number is required.</label></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="regexAccountNumber" runat="server" ErrorMessage=""
                                ControlToValidate="txtAccountNumber" SetFocusOnError="True" ValidationExpression=""
                                Enabled="false" ValidationGroup="DataEntry" Display="Dynamic">
                                <asp:Label ID="lblAccountNumber" Text="" runat="server" class="has-error"><img alt="!" src="images/validationerror.png" /></asp:Label></asp:RegularExpressionValidator>
                        </div>
                    </div>
                    <div id="divNameKeyControl" visible="false" class="form-group" runat="server">
                        <label for="txtNameKey" class="col-sm-5 control-label input-sm">
                            Key:</label>
                        <div class="col-sm-3">
                            <asp:TextBox ID="txtNameKey" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        </div>
                        <div class="col-sm-4">
                            <asp:RequiredFieldValidator ID="rfvNameKey" runat="server" ControlToValidate="txtNameKey"
                                ErrorMessage="Key is required." ValidationGroup="DataEntry" Enabled="false" SetFocusOnError="True"><label for="txtNameKey" class="has-error"><img alt="!" src="images/validationerror.png" /> Key is required.</label></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div id="divServiceNumberControl" visible="false" class="form-group" runat="server">
                        <label for="txtServiceNumber" class="col-sm-5 control-label input-sm">
                            Service Number:</label>
                        <div class="col-sm-3">
                            <asp:TextBox ID="txtServiceNumber" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        </div>
                        <div class="col-sm-4">
                            <asp:RequiredFieldValidator ID="rfvServiceNumber" runat="server" ControlToValidate="txtServiceNumber"
                                ErrorMessage="Service Number is required." ValidationGroup="DataEntry" Enabled="false"
                                SetFocusOnError="True"><label for="txtServiceNumber" class="has-error"><img alt="!" src="images/validationerror.png" /> Service Number is required.</label></asp:RequiredFieldValidator>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <label for="txtServiceAddress1" class="col-sm-5 control-label input-sm">
                        Street Address:</label>
                    <div class="col-sm-3">
                        <asp:TextBox ID="txtServiceAddress1" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </div>
                    <div class="col-sm-4">
                        <asp:RequiredFieldValidator ID="rfvServiceAddress1" runat="server" ControlToValidate="txtServiceAddress1"
                            ErrorMessage="Street Address is required." ValidationGroup="DataEntry" SetFocusOnError="True"><label for="txtServiceAddress1" class="has-error"><img alt="!" src="images/validationerror.png" /> Service Street Address is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <label for="txtServiceAddress2" class="col-sm-5 control-label input-sm">
                        Apt/Suite :</label>
                    <div class="col-sm-3">
                        <asp:TextBox ID="txtServiceAddress2" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </div>
                </div>
                <div class="form-group">
                    <label for="txtServiceZip" class="col-sm-5 control-label input-sm">
                        Zip Code:</label>
                    <div class="col-sm-3">
                        <asp:TextBox ID="txtServiceZip" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </div>
                    <div class="col-sm-1">
                        <asp:ImageButton ID="imgBtnServiceZipSearch" runat="server" AlternateText="Search Service Zip Code"
                            ImageUrl="~/images/search.png" OnClick="serviceZipSearch_Click" />
                    </div>
                    <div class="col-sm-3">
                        <asp:RequiredFieldValidator ID="rfvServiceZip" runat="server" ControlToValidate="txtServiceZip"
                            ErrorMessage="Service Zip Code is required." ValidationGroup="DataEntry" SetFocusOnError="True"><label for="txtServiceZip" class="has-error"><img alt="!" src="images/validationerror.png" /> Service Zip Code is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <label for="ddlServiceCity" class="col-sm-5 control-label input-sm">
                        City:</label>
                    <div class="col-sm-3">
                        <asp:DropDownList ID="ddlServiceCity" runat="server" CssClass="form-control input-sm"
                            AppendDataBoundItems="True">
                        </asp:DropDownList>
                    </div>
                    <div class="col-sm-3">
                        <asp:RequiredFieldValidator ID="rfvServiceCity" runat="server" ControlToValidate="ddlServiceCity"
                            ErrorMessage="Service City is required." ValidationGroup="DataEntry" SetFocusOnError="True"><label for="ddlServiceCity" class="has-error"><img alt="!" src="images/validationerror.png" /> Service City is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <label for="ddlServiceState" class="col-sm-5 control-label input-sm">
                        State:</label>
                    <div class="col-sm-3">
                        <asp:DropDownList ID="ddlServiceState" runat="server" CssClass="form-control input-sm"
                            AppendDataBoundItems="True">
                        </asp:DropDownList>
                    </div>
                    <div class="col-sm-3">
                        <asp:RequiredFieldValidator ID="rfvServiceState" runat="server" ControlToValidate="ddlServiceState"
                            ErrorMessage="Service State is required." ValidationGroup="DataEntry" SetFocusOnError="True"><label for="ddlServiceState" class="has-error"><img alt="!" src="images/validationerror.png" /> Service State is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-sm-8 control-label input-sm">
                        What is the customer's billing address associated with this account?</label>
                </div>
                <div class="form-group">
                    <label for="chkMakeSameAs" class="col-sm-5 control-label input-sm">
                        Same as Service Address?<br />
                        (if not same as service address complete billing address below.)</label>
                    <div class="col-sm-1">
                        <asp:CheckBox ID="chkMakeSameAs" runat="server" CssClass="form-control input-sm"
                            AppendDataBoundItems="True" OnCheckedChanged="chkMakeSameAs_CheckedChanged" AutoPostBack="True" />
                    </div>
                </div>
                <div class="form-group">
                    <label for="txtBillingAddress1" class="col-sm-5 control-label input-sm">
                        Billing Street Address:</label>
                    <div class="col-sm-3">
                        <asp:TextBox ID="txtBillingAddress1" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </div>
                    <div class="col-sm-4">
                        <asp:RequiredFieldValidator ID="rfvBillingAddress1" runat="server" ControlToValidate="txtBillingAddress1"
                            ErrorMessage="Billing Street Address is required." ValidationGroup="DataEntry"
                            SetFocusOnError="True"><label for="txtBillingAddress1" class="has-error"><img alt="!" src="images/validationerror.png" /> Billing Street Address is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <label for="txtServiceAddress2" class="col-sm-5 control-label input-sm">
                        Apt/Suite :</label>
                    <div class="col-sm-3">
                        <asp:TextBox ID="txtBillingAddress2" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </div>
                </div>
                <div class="form-group">
                    <label for="txtBillingZip" class="col-sm-5 control-label input-sm">
                        Zip Code:</label>
                    <div class="col-sm-3">
                        <asp:TextBox ID="txtBillingZip" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </div>
                    <div class="col-sm-1">
                        <asp:ImageButton ID="imgBtnBillingZipSearch" runat="server" AlternateText="Search Billing Zip Code"
                            ImageUrl="~/images/search.png" OnClick="billingZipSearch_Click" />
                    </div>
                    <div class="col-sm-3">
                        <asp:RequiredFieldValidator ID="rfvBillingZip" runat="server" ControlToValidate="txtBillingZip"
                            ErrorMessage="Billing Zip Code is required." ValidationGroup="DataEntry" SetFocusOnError="True"><label for="txtBillingZip" class="has-error"><img alt="!" src="images/validationerror.png" /> Billing Zip Code is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <label for="ddlBillingeCity" class="col-sm-5 control-label input-sm">
                        City:</label>
                    <div class="col-sm-3">
                        <asp:DropDownList ID="ddlBillingCity" runat="server" CssClass="form-control input-sm"
                            AppendDataBoundItems="True">
                        </asp:DropDownList>
                    </div>
                    <div class="col-sm-3">
                        <asp:RequiredFieldValidator ID="rfvBillingCity" runat="server" ControlToValidate="ddlBillingCity"
                            ErrorMessage="Billing City is required." ValidationGroup="DataEntry" SetFocusOnError="True"><label for="ddlBillingCity" class="has-error"><img alt="!" src="images/validationerror.png" /> Billing City is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <label for="ddlBillingeState" class="col-sm-5 control-label input-sm">
                        State:</label>
                    <div class="col-sm-3">
                        <asp:DropDownList ID="ddlBillingState" runat="server" CssClass="form-control input-sm"
                            AppendDataBoundItems="True">
                        </asp:DropDownList>
                    </div>
                    <div class="col-sm-3">
                        <asp:RequiredFieldValidator ID="rfvBillingState" runat="server" ControlToValidate="ddlBillingState"
                            ErrorMessage="Billing State is required." ValidationGroup="DataEntry" SetFocusOnError="True"><label for="ddlBillingState" class="has-error"><img alt="!" src="images/validationerror.png" /> Billing State is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div id="divElectricServiceSection" runat="server" class="form-group">
                    <div class="form-group">
                        <label class="col-sm-5 control-label input-sm">
                            What is the term of the service agreement in months?</label>
                    </div>
                    <div class="form-group">
                        <label for="ddlContractTerm" class="col-sm-5 control-label input-sm">
                            Contract Term:</label>
                        <div class="col-sm-3">
                            <asp:DropDownList ID="ddlContractTerm" runat="server" CssClass="form-control input-sm"
                                AppendDataBoundItems="True">
                            </asp:DropDownList>
                        </div>
                        <div class="col-sm-4">
                            <asp:RequiredFieldValidator ID="rfvContracTerm" runat="server" ControlToValidate="ddlContractTerm"
                                ErrorMessage="Contract Term is required." ValidationGroup="DataEntry" SetFocusOnError="True"><label for="ddlContractTerm" class="has-error"><img alt="!" src="images/validationerror.png" /> Contract Term is required.</label></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-5 control-label input-sm">
                            What is the fixed electricity rate per kwh?</label>
                    </div>
                    <div id="divRate" runat="server" visible="false" class="form-group">
                        <label for="txtRate" class="col-sm-5 control-label input-sm">
                            Rate:</label>
                        <div class="col-sm-2">
                            <asp:TextBox ID="txtRate" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        </div>
                        <div class="col-sm-1">
                            <label for="txtRate" class="col-sm-2 control-label input-sm">
                                (0.XXXXX)</label>
                        </div>
                        <div class="col-sm-3">
                            <asp:RequiredFieldValidator ID="rfvRate" runat="server" ControlToValidate="txtRate"
                                ErrorMessage="Rate is required." ValidationGroup="DataEntry" SetFocusOnError="True"><label for="txtRate" class="has-error"><img alt="!" src="images/validationerror.png" /> Rate is required.</label></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div id="divSubTermRate" visible="false" runat="server">
                        <div class="form-group">
                            <label for="txtSubTermMonth1Start" class="col-sm-2 control-label input-sm">
                                Sub-term #1 Months</label>
                            <div class="col-sm-1">
                                <asp:TextBox ID="txtSubTermMonth1Start" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                            </div>
                            <label for="txtSubTermMonth1End" class="col-sm-1 control-label input-sm">
                                To</label>
                            <div class="col-sm-1">
                                <asp:TextBox ID="txtSubTermMonth1End" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                            </div>
                            <label for="txtSubTermMonth1End" class="col-sm-1 control-label input-sm">
                                Rate</label>
                            <div class="col-sm-2">
                                <asp:TextBox ID="txtSubTermMonth1Rate" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                            </div>
                            <div class="col-sm-3">
                                <label for="txtSubTermMonth1Rate" class="col-sm-6 control-label input-sm">
                                    per KWH (0.XXXXX)</label>
                            </div>
                        </div>
                        <div class="form-group">
                            <label for="txtSubTermMonth2Start" class="col-sm-2 control-label input-sm">
                                Sub-term #1 Months</label>
                            <div class="col-sm-1">
                                <asp:TextBox ID="txtSubTermMonth2Start" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                            </div>
                            <label for="txtSubTermMonth2End" class="col-sm-1 control-label input-sm">
                                To</label>
                            <div class="col-sm-1">
                                <asp:TextBox ID="txtSubTermMonth2End" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                            </div>
                            <label for="txtSubTermMonth2End" class="col-sm-1 control-label input-sm">
                                Rate</label>
                            <div class="col-sm-2">
                                <asp:TextBox ID="txtSubTermMonth2Rate" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                            </div>
                            <div class="col-sm-3">
                                <label for="txtSubTermMonth2Rate" class="col-sm-6 control-label input-sm">
                                    per KWH (0.XXXXX)</label>
                            </div>
                        </div>
                        <div class="form-group">
                            <label for="txtSubTermMonth3Start" class="col-sm-2 control-label input-sm">
                                Sub-term #1 Months</label>
                            <div class="col-sm-1">
                                <asp:TextBox ID="txtSubTermMonth3Start" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                            </div>
                            <label for="txtSubTermMonth3End" class="col-sm-1 control-label input-sm">
                                To</label>
                            <div class="col-sm-1">
                                <asp:TextBox ID="txtSubTermMonth3End" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                            </div>
                            <label for="txtSubTermMonth3End" class="col-sm-1 control-label input-sm">
                                Rate</label>
                            <div class="col-sm-2">
                                <asp:TextBox ID="txtSubTermMonth3Rate" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                            </div>
                            <div class="col-sm-3">
                                <label for="txtSubTermMonth3Rate" class="col-sm-6 control-label input-sm">
                                    per KWH (0.XXXXX)</label>
                            </div>
                        </div>
                        <div class="form-group">
                            <label for="txtSubTermMonth4Start" class="col-sm-2 control-label input-sm">
                                Sub-term #1 Months</label>
                            <div class="col-sm-1">
                                <asp:TextBox ID="txtSubTermMonth4Start" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                            </div>
                            <label for="txtSubTermMonth4End" class="col-sm-1 control-label input-sm">
                                To</label>
                            <div class="col-sm-1">
                                <asp:TextBox ID="txtSubTermMonth4End" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                            </div>
                            <label for="txtSubTermMonth4End" class="col-sm-1 control-label input-sm">
                                Rate</label>
                            <div class="col-sm-2">
                                <asp:TextBox ID="txtSubTermMonth4Rate" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                            </div>
                            <div class="col-sm-3">
                                <label for="txtSubTermMonth4Rate" class="col-sm-6 control-label input-sm">
                                    per KWH (0.XXXXX)</label>
                            </div>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-7 control-label input-sm">
                            What is the estimated month & year that the customer's new rate will take effect?</label>
                    </div>
                    <div class="form-group">
                        <label for="txtRateEffectiveDate" class="col-sm-5 control-label input-sm">
                            Est. Date Effect:</label>
                        <div class="col-sm-3">
                            <asp:TextBox ID="txtRateEffectiveDate" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        </div>
                        <div class="col-sm-4">
                            <asp:RequiredFieldValidator ID="rfvRateEffectiveDate" runat="server" ControlToValidate="txtRateEffectiveDate"
                                ErrorMessage="Rate Effective Date is required." ValidationGroup="DataEntry" SetFocusOnError="True"
                                Display="Dynamic"><label for="txtRateEffectiveDate" class="has-error"><img alt="!" src="images/validationerror.png" /> Rate Effective Date is required.</label></asp:RequiredFieldValidator>
                            <%--<asp:CompareValidator ID="CompareRateEffectiveDateTodayValidator" Operator="GreaterThanEqual" ValidationGroup="DataEntry" type="Date" ControltoValidate="txtRateEffectiveDate" ErrorMessage="The date cannot be in the past." runat="server" Display="Dynamic"><label for="txtRateEffectiveDate" class="has-error"><img alt="!" src="images/validationerror.png" /> The date cannot be in the past.</label></asp:CompareValidator>--%>
                            <asp:RegularExpressionValidator ID="regexRateEffectiveDate" runat="server" ControlToValidate="txtRateEffectiveDate"
                                ErrorMessage="Invalid Date Format MM/01/YYYY." ValidationGroup="DataEntry" SetFocusOnError="True"
                                Display="Dynamic" ValidationExpression="^(0[1-9]|1[012])[/](01)[/](19|20)\d\d$"><label for="txtRateEffectiveDate" class="has-error"><img alt="!" src="images/validationerror.png" /> Invalid Date Format MM/01/YYYY.</label></asp:RegularExpressionValidator>
                        </div>
                    </div>
                    <div id="divRateExpirationDate" runat="server" visible="false" class="form-group">
                        <label for="txtRateExpirationDate" class="col-sm-5 control-label input-sm">
                            Est. Date Expiration:</label>
                        <div class="col-sm-3">
                            <asp:TextBox ID="txtRateExpirationDate" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        </div>
                        <div class="col-sm-4">
                            <asp:RequiredFieldValidator ID="rfvRateExpirationDate" runat="server" ControlToValidate="txtRateExpirationDate"
                                ErrorMessage="Rate Expiration Date is required." ValidationGroup="DataEntry"
                                SetFocusOnError="True" Display="Dynamic"><label for="txtRateExpirationDate" class="has-error"><img alt="!" src="images/validationerror.png" /> Rate Expiration Date is required.</label></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtRateExpirationDate"
                                ErrorMessage="Invalid Date Format MM/01/YYYY." ValidationGroup="DataEntry" SetFocusOnError="True"
                                Display="Dynamic" ValidationExpression="^(0[1-9]|1[012])[/](01)[/](19|20)\d\d$"><label for="txtRateExpirationDate" class="has-error"><img alt="!" src="images/validationerror.png" /> Invalid Date Format MM/01/YYYY.</label></asp:RegularExpressionValidator>
                        </div>
                    </div>
                </div>
                <!--divGas-->
                <div id="divGas" runat="server" visible="false" class="form-group">
                    <div class="form-group">
                        <label class="col-sm-7 control-label input-sm">
                            Complete the following fields for gas enrollment.</label>
                    </div>
                    <div class="form-group">
                        <label for="txtGasLastNameCheck" class="col-sm-5 control-label input-sm">
                            Gas Last Name:</label>
                        <div class="col-sm-3">
                            <asp:TextBox ID="txtGasLastNameCheck" runat="server" CssClass="form-control input-sm input-sm"></asp:TextBox>
                        </div>
                        <div class="col-sm-4">
                            <asp:CompareValidator ID="cmpGasLastNameCheckComparison" runat="server" ControlToValidate="txtGasLastNameCheck"
                                ControlToCompare="txtAuthorizationLasttName" Operator="Equal" Type="String" ErrorMessage="Last Name Must Match Above."
                                ValidationGroup="DataEntry" Display="Dynamic" SetFocusOnError="True"><label for="txtGasLastNameCheck" class="has-error"><img alt="!" src="images/validationerror.png" /> Last Name Must Match Above.</label></asp:CompareValidator>
                            <asp:RequiredFieldValidator ID="rfvGasLastNameCheck" runat="server" ControlToValidate="txtGasLastNameCheck"
                                ErrorMessage="Gas Last Name is required." ValidationGroup="DataEntry" Display="Dynamic"
                                SetFocusOnError="True"><label for="txtGasLastNameCheck" class="has-error"><img alt="!" src="images/validationerror.png" /> Gas Last Name is required.</label></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="ddlGasMarketUtility" class="col-sm-5 control-label input-sm">
                            What is the Gas Utility?</label>
                        <div class="col-sm-3">
                            <asp:DropDownList ID="ddlGasMarketUtility" runat="server" CssClass="form-control input-sm"
                                DataTextField="Utility" DataValueField="GasMarketUtilityId" AppendDataBoundItems="True"
                                AutoPostBack="True" OnSelectedIndexChanged="ddlGasMarketUtility_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                        <div class="col-sm-3">
                            <asp:RequiredFieldValidator ID="rfvGasMarketUtility" runat="server" ControlToValidate="ddlGasMarketUtility"
                                ErrorMessage="Gas Utility is required." ValidationGroup="DataEntry" SetFocusOnError="True"><label for="ddlGasMarketUtility" class="has-error"><img alt="!" src="images/validationerror.png" /> Gas Utility is required.</label></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="txtGasAccountNumber" class="col-sm-5 control-label input-sm">
                            Gas Account Number:</label>
                        <div class="col-sm-3">
                            <asp:TextBox ID="txtGasAccountNumber" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        </div>
                        <div class="col-sm-3">
                            <asp:RequiredFieldValidator ID="rfvGasAccountNumber" runat="server" ControlToValidate="txtGasAccountNumber"
                                ErrorMessage="Gas Account Number is required." ValidationGroup="DataEntry" SetFocusOnError="True"
                                Display="Dynamic"><label for="txtGasAccountNumber" class="has-error"><img alt="!" src="images/validationerror.png" /> Gas Account Number is required.</label></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="regexGasAccountNumber" runat="server" ErrorMessage=""
                                ControlToValidate="txtGasAccountNumber" SetFocusOnError="True" ValidationExpression=""
                                Enabled="false" ValidationGroup="DataEntry" Display="Dynamic">
                                <asp:Label ID="lblGasAccountNumber" Text="" runat="server" class="has-error"><img alt="!" src="images/validationerror.png" /></asp:Label></asp:RegularExpressionValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="txtGasRate" class="col-sm-5 control-label input-sm">
                            Gas Rate:</label>
                        <div class="col-sm-2">
                            <asp:TextBox ID="txtGasRate" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        </div>
                        <div class="col-sm-1">
                            <label for="txtGasRate" class="col-sm-2 control-label input-sm">
                                (0.XXXXX)</label>
                        </div>
                        <div class="col-sm-3">
                            <asp:RequiredFieldValidator ID="rfvGasRate" runat="server" ControlToValidate="txtGasRate"
                                ErrorMessage="Gas Rate is required." ValidationGroup="DataEntry" SetFocusOnError="True"><label for="txtGasRate" class="has-error"><img alt="!" src="images/validationerror.png" /> Gas Rate is required.</label></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="txtGasRateEffectiveDate" class="col-sm-5 control-label input-sm">
                            Gas Est. Date Effect:</label>
                        <div class="col-sm-3">
                            <asp:TextBox ID="txtGasRateEffectiveDate" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        </div>
                        <div class="col-sm-4">
                            <asp:RequiredFieldValidator ID="rfvGasRateEffectiveDate" runat="server" ControlToValidate="txtGasRateEffectiveDate"
                                ErrorMessage="Gas Rate Effective Date is required." ValidationGroup="DataEntry"
                                SetFocusOnError="True" Display="Dynamic"><label for="txtGasRateEffectiveDate" class="has-error"><img alt="!" src="images/validationerror.png" /> Gas Rate Effective Date is required.</label></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="regexGasRateEffectiveDate" runat="server" ControlToValidate="txtGasRateEffectiveDate"
                                ErrorMessage="Invalid Date Format MM/01/YYYY." ValidationGroup="DataEntry" SetFocusOnError="True"
                                Display="Dynamic" ValidationExpression="^(0[1-9]|1[012])[/](01)[/](19|20)\d\d$"><label for="txtGasRateEffectiveDate" class="has-error"><img alt="!" src="images/validationerror.png" /> Invalid Date Format MM/01/YYYY.</label></asp:RegularExpressionValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="ddlGasContractTerm" class="col-sm-5 control-label input-sm">
                            Gas Term:</label>
                        <div class="col-sm-3">
                            <asp:DropDownList ID="ddlGasContractTerm" runat="server" CssClass="form-control input-sm"
                                AppendDataBoundItems="True">
                                <asp:ListItem Text="<-Select Term of Service->" Value="" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="24" Text="24 Month"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-sm-4">
                            <asp:RequiredFieldValidator ID="rfvGasContracTerm" runat="server" ControlToValidate="ddlGasContractTerm"
                                ErrorMessage="Gas Term is required." ValidationGroup="DataEntry" SetFocusOnError="True"><label for="ddlGasContractTerm" class="has-error"><img alt="!" src="images/validationerror.png" /> Gas Term is required.</label></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div id="divMeterNumberControl" visible="false" class="form-group" runat="server">
                        <label for="txtMeterNumber" class="col-sm-5 control-label input-sm">
                            Meter Number:</label>
                        <div class="col-sm-3">
                            <asp:TextBox ID="txtMeterNumber" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        </div>
                        <div class="col-sm-4">
                            <asp:RequiredFieldValidator ID="rfvMeterNumber" runat="server" ControlToValidate="txtMeterNumber"
                                ErrorMessage="Meter Number is required." ValidationGroup="DataEntry" Enabled="false"
                                SetFocusOnError="True"><label for="txtMeterNumber" class="has-error"><img alt="!" src="images/validationerror.png" /> Meter Number is required.</label></asp:RequiredFieldValidator>
                        </div>
                    </div>
                </div>
                <!--divGas-->
                <div class="form-group">
                    <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" ValidationGroup="DataEntry"
                        OnClick="btnSave_Click" />
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-primary"
                        OnClick="btnSubmit_Click" />
                </div>
            </div>
        </div>
    </div>
    <asp:EntityDataSource ID="MarketStateDataSource" runat="server" ConnectionString="name=LibertyEntities"
        DefaultContainerName="LibertyEntities" EnableFlattening="False" EntitySetName="MarketStates"
        Select="it.[State], it.[MarketStateId]" Where="it.[Active]==true" OrderBy="it.[State]">
    </asp:EntityDataSource>
    <%--Main Form--%>
    <%--Deprecated since MODAL no longer happens--%>
    <%--Grid View--%>
    <%--    <div id="divGvAdditionalAccounts" visible="false" runat="server" class="row col-md-10 col-md-offset-1">
        <div class="panel panel-info">
            <div class="panel-heading">
                <h3 class="panel-title">
                    Additional Accounts</h3>
            </div>
            <div class="panel-body text-center">
                <div class="form-group">
                    <asp:GridView ID="gvAdditionalAccounts" runat="server" AutoGenerateColumns="false"
                        EmptyDataText="No accounts found." OnRowDataBound="gvAdditionalAccounts_RowDataBound"
                        Width="100%" CssClass="table-condensed table-striped table-hover">
                        <EmptyDataRowStyle />
                        <Columns>
                            <asp:BoundField DataField="Btn" HeaderText="Phone" NullDisplayText="N/A">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="AccountNumber" HeaderText="Account #" NullDisplayText="N/A">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="ServiceAddress1" HeaderText="Address" NullDisplayText="N/A">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="ServiceCity" HeaderText="City" NullDisplayText="N/A">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="ServiceState" HeaderText="State" NullDisplayText="N/A">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="ServiceZip" HeaderText="Zip" NullDisplayText="N/A">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEdit" runat="server" CommandName="Select" ImageUrl="~/images/edit.png"
                                        OnCommand="btnEdit_Click" CommandArgument='<%# Eval("OrderDetailFormRecordNumber")%>'>
                                        <asp:Image ID="imgEdit" runat="server" ImageUrl="~/images/edit.png" /></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle />
                        <AlternatingRowStyle />
                    </asp:GridView>
                </div>
                <div class="form-group">
                    <asp:Button ID="btnAddAdditionalAccount" runat="server" Text="Add Additional Account"
                        CssClass="btn btn-primary" OnClick="btnAddAdditionalAccount_Click" CausesValidation="False" />
                </div>
            </div>
        </div>
    </div>--%>
    <%--Grid View--%>
    <%--Deprecated Modal Below--%>
    <%--Modal Window--%>
    <%--<asp:Label ID="lblHidden" runat="server" Text=""></asp:Label>
    <ajaxToolkit:ModalPopupExtender ID="mpePopUp" runat="server" TargetControlID="lblHidden"
        PopupControlID="divPopUp" BackgroundCssClass="modalBackground" BehaviorID="mpePopUp">
    </ajaxToolkit:ModalPopupExtender>
    <div class="row col-sm" id="divPopUp" runat="server" style="max-height: 700px; overflow: auto;">
        <div class="panel panel-info">
            <div class="panel-heading">
                <h3 class="panel-title">
                    Add Additional Account</h3>
            </div>
            <div class="panel-body text-center">
                <div class="form-group">
                    <label for="txtModalAccountNumber" class="col-sm-4 control-label input-sm">
                        Account Number:</label>
                    <div class="col-sm-5">
                        <asp:TextBox ID="txtModalAccountNumber" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </div>
                    <div class="col-sm-3">
                        <asp:RequiredFieldValidator ID="rfvModalAccountNumber" runat="server" ControlToValidate="txtModalAccountNumber"
                            ErrorMessage="Account Number is required." ValidationGroup="ModalDataEntry" SetFocusOnError="True"><label for="txtModalAccountNumber" class="has-error"><img alt="!" src="images/validationerror.png" /> Account Number is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div id="divModalGas" runat="server" visible="false" class="form-group">
                    <label for="txtModalGasAccountNumber" class="col-sm-5 control-label input-sm">
                        Gas Account Number:</label>
                    <div class="col-sm-3">
                        <asp:TextBox ID="txtModalGasAccountNumber" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </div>
                    <div class="col-sm-3">
                        <asp:RequiredFieldValidator ID="rfvModalGasAccountNumber" runat="server" ControlToValidate="txtModalGasAccountNumber"
                            ErrorMessage="Gas Account Number is required." ValidationGroup="ModalDataEntry"
                            SetFocusOnError="True" Display="Dynamic"><label for="txtModalGasAccountNumber" class="has-error"><img alt="!" src="images/validationerror.png" /> Gas Account Number is required.</label></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="regexModalGasAccountNumber" runat="server" ErrorMessage=""
                            ControlToValidate="txtModalGasAccountNumber" SetFocusOnError="True" ValidationExpression=""
                            Enabled="false" ValidationGroup="ModalDataEntry" Display="Dynamic">
                            <asp:Label ID="lblModalGasAccountNumber" Text="" runat="server" class="has-error"><img alt="!" src="images/validationerror.png" /></asp:Label></asp:RegularExpressionValidator>
                    </div>
                </div>
                <div id="divModalNameKeyControl" visible="false" class="form-group" runat="server">
                    <label for="txtModalNameKey" class="col-sm-5 control-label input-sm">
                        Key:</label>
                    <div class="col-sm-3">
                        <asp:TextBox ID="txtModalNameKey" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </div>
                    <div class="col-sm-4">
                        <asp:RequiredFieldValidator ID="rfvModalNameKey" runat="server" ControlToValidate="txtModalNameKey"
                            ErrorMessage="Key is required." ValidationGroup="ModalDataEntry" Enabled="false"
                            SetFocusOnError="True"><label for="txtModalNameKey" class="has-error"><img alt="!" src="images/validationerror.png" /> Key is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div id="divModalServiceNumberControl" visible="false" class="form-group" runat="server">
                    <label for="txtModalServiceNumber" class="col-sm-5 control-label input-sm">
                        Service Number:</label>
                    <div class="col-sm-3">
                        <asp:TextBox ID="txtModalServiceNumber" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </div>
                    <div class="col-sm-4">
                        <asp:RequiredFieldValidator ID="rfvModalServiceNumber" runat="server" ControlToValidate="txtModalServiceNumber"
                            ErrorMessage="Service Number is required." ValidationGroup="ModalDataEntry" Enabled="false"
                            SetFocusOnError="True"><label for="txtModalServiceNumber" class="has-error"><img alt="!" src="images/validationerror.png" /> Service Number is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div id="divModalSubTermRate" visible="false" runat="server">
                    <div class="form-group">
                        <label for="txtModalSubTermMonth1Rate" class="col-sm-3 control-label input-sm">
                            Sub-term Rate 1</label>
                        <div class="col-sm-3">
                            <asp:TextBox ID="txtModalSubTermMonth1Rate" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        </div>
                        <label for="txtModalSubTermMonth2Rate" class="col-sm-3 control-label input-sm">
                            Sub-term Rate 2</label>
                        <div class="col-sm-3">
                            <asp:TextBox ID="txtModalSubTermMonth2Rate" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="txtModalSubTermMonth3Rate" class="col-sm-3 control-label input-sm">
                            Sub-term Rate 3</label>
                        <div class="col-sm-3">
                            <asp:TextBox ID="txtModalSubTermMonth3Rate" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        </div>
                        <label for="txtModalSubTermMonth4Rate" class="col-sm-3 control-label input-sm">
                            Sub-term Rate 4</label>
                        <div class="col-sm-3">
                            <asp:TextBox ID="txtModalSubTermMonth4Rate" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <label for="txtModalServiceAddress1" class="col-sm-5 control-label input-sm">
                        Street Address:</label>
                    <div class="col-sm-3">
                        <asp:TextBox ID="txtModalServiceAddress1" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </div>
                    <div class="col-sm-4">
                        <asp:RequiredFieldValidator ID="rfvModalServiceAddress1" runat="server" ControlToValidate="txtModalServiceAddress1"
                            ErrorMessage="Street Address is required." ValidationGroup="ModalDataEntry" SetFocusOnError="True"><label for="txtModalServiceAddress1" class="has-error"><img alt="!" src="images/validationerror.png" /> Service Street Address is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <label for="txtModalServiceAddress2" class="col-sm-5 control-label input-sm">
                        Apt/Suite :</label>
                    <div class="col-sm-3">
                        <asp:TextBox ID="txtModalServiceAddress2" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </div>
                </div>
                <div class="form-group">
                    <label for="txtModalServiceZip" class="col-sm-5 control-label input-sm">
                        Zip Code:</label>
                    <div class="col-sm-3">
                        <asp:TextBox ID="txtModalServiceZip" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </div>
                    <div class="col-sm-1">
                        <asp:ImageButton ID="imgBtnModalServiceZipSearch" runat="server" AlternateText="Search Service Zip Code"
                            ImageUrl="~/images/search.png" OnClick="modalServiceZipSearch_Click" />
                    </div>
                    <div class="col-sm-3">
                        <asp:RequiredFieldValidator ID="rfvModalServiceZip" runat="server" ControlToValidate="txtModalServiceZip"
                            ErrorMessage="Service Zip Code is required." ValidationGroup="ModalDataEntry"
                            SetFocusOnError="True"><label for="txtModalServiceZip" class="has-error"><img alt="!" src="images/validationerror.png" /> Service Zip Code is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <label for="ddlModalServiceCity" class="col-sm-5 control-label input-sm">
                        City:</label>
                    <div class="col-sm-4">
                        <asp:DropDownList ID="ddlModalServiceCity" runat="server" CssClass="form-control input-sm"
                            AppendDataBoundItems="True">
                        </asp:DropDownList>
                    </div>
                    <div class="col-sm-3">
                        <asp:RequiredFieldValidator ID="rfvModalServiceCity" runat="server" ControlToValidate="ddlModalServiceCity"
                            ErrorMessage="Service City is required." ValidationGroup="ModalDataEntry" SetFocusOnError="True"><label for="ddlModalServiceCity" class="has-error"><img alt="!" src="images/validationerror.png" /> Service City is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <label for="ddlModalServiceState" class="col-sm-5 control-label input-sm">
                        State:</label>
                    <div class="col-sm-4">
                        <asp:DropDownList ID="ddlModalServiceState" runat="server" CssClass="form-control input-sm"
                            AppendDataBoundItems="True">
                        </asp:DropDownList>
                    </div>
                    <div class="col-sm-3">
                        <asp:RequiredFieldValidator ID="rfvModalServiceState" runat="server" ControlToValidate="ddlModalServiceState"
                            ErrorMessage="Service State is required." ValidationGroup="ModalDataEntry" SetFocusOnError="True"><label for="ddlModalServiceState" class="has-error"><img alt="!" src="images/validationerror.png" /> Service State is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <label for="chkModalMakeSameAs" class="col-sm-5 control-label input-sm">
                        Same as Service Address?</label>
                    <div class="col-sm-2">
                        <asp:CheckBox ID="chkModalMakeSameAs" runat="server" CssClass="form-control input-sm"
                            AppendDataBoundItems="True" OnCheckedChanged="chkModalMakeSameAs_CheckedChanged"
                            AutoPostBack="True" />
                    </div>
                </div>
                <div class="form-group">
                    <label for="txtModalBillingAddress1" class="col-sm-5 control-label input-sm">
                        Billing Street Address:</label>
                    <div class="col-sm-3">
                        <asp:TextBox ID="txtModalBillingAddress1" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </div>
                    <div class="col-sm-4">
                        <asp:RequiredFieldValidator ID="rfvModalBillingAddress1" runat="server" ControlToValidate="txtModalBillingAddress1"
                            ErrorMessage="Billing Street Address is required." ValidationGroup="ModalDataEntry"
                            SetFocusOnError="True"><label for="txtModalBillingAddress1" class="has-error"><img alt="!" src="images/validationerror.png" /> Billing Street Address is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <label for="txtModalServiceAddress2" class="col-sm-5 control-label input-sm">
                        Apt/Suite :</label>
                    <div class="col-sm-3">
                        <asp:TextBox ID="txtModalBillingAddress2" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </div>
                </div>
                <div class="form-group">
                    <label for="txtModalBillingZip" class="col-sm-5 control-label input-sm">
                        Zip Code:</label>
                    <div class="col-sm-3">
                        <asp:TextBox ID="txtModalBillingZip" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </div>
                    <div class="col-sm-1">
                        <asp:ImageButton ID="imgBtnModalBillingZipSearch" runat="server" AlternateText="Search Billing Zip Code"
                            ImageUrl="~/images/search.png" OnClick="modalBillingZipSearch_Click" />
                    </div>
                    <div class="col-sm-3">
                        <asp:RequiredFieldValidator ID="rfvModalBillingZip" runat="server" ControlToValidate="txtModalBillingZip"
                            ErrorMessage="Billing Zip Code is required." ValidationGroup="ModalDataEntry"
                            SetFocusOnError="True"><label for="txtModalBillingZip" class="has-error"><img alt="!" src="images/validationerror.png" /> Billing Zip Code is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <label for="ddlModalBillingeCity" class="col-sm-5 control-label input-sm">
                        City:</label>
                    <div class="col-sm-4">
                        <asp:DropDownList ID="ddlModalBillingCity" runat="server" CssClass="form-control input-sm"
                            AppendDataBoundItems="True">
                        </asp:DropDownList>
                    </div>
                    <div class="col-sm-3">
                        <asp:RequiredFieldValidator ID="rfvModalBillingCity" runat="server" ControlToValidate="ddlModalBillingCity"
                            ErrorMessage="Billing City is required." ValidationGroup="ModalDataEntry" SetFocusOnError="True"><label for="ddlModalBillingCity" class="has-error"><img alt="!" src="images/validationerror.png" /> Billing City is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <label for="ddlModalBillingeState" class="col-sm-5 control-label input-sm">
                        State:</label>
                    <div class="col-sm-4">
                        <asp:DropDownList ID="ddlModalBillingState" runat="server" CssClass="form-control input-sm"
                            AppendDataBoundItems="True">
                        </asp:DropDownList>
                    </div>
                    <div class="col-sm-3">
                        <asp:RequiredFieldValidator ID="rfvModalBillingState" runat="server" ControlToValidate="ddlModalBillingState"
                            ErrorMessage="Billing State is required." ValidationGroup="ModalDataEntry" SetFocusOnError="True"><label for="ddlModalBillingState" class="has-error"><img alt="!" src="images/validationerror.png" /> Billing State is required.</label></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <asp:Button ID="btnSaveAccount" runat="server" Text="Save Account" ValidationGroup="ModalDataEntry"
                        CssClass="btn btn-primary" OnClick="btnSaveAccount_Click" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-primary"
                        OnClick="btnCancel_Click" />
                </div>
            </div>
        </div>
    </div>--%>
    <%--Modal Window--%>
</asp:Content>
