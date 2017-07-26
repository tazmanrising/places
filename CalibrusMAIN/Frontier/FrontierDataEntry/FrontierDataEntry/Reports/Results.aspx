<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="Results.aspx.cs" Inherits="Reports_Results" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div style="width: 100%; margin: auto;">
        <asp:GridView ID="gvReport" runat="server" AutoGenerateColumns="False" EmptyDataText="No records found."
            Font-Names="Century Gothic" Font-Size="9pt" Width="100%" OnRowDataBound="gvReport_RowDataBound"
            OnRowCommand="gvReport_RowCommand" CellPadding="3" AllowPaging="True" PageSize="15"
            OnPageIndexChanging="gvReport_PageIndexChanging">
            <PagerSettings NextPageText="&gt;&gt;" PageButtonCount="20" Position="TopAndBottom" />
            <RowStyle BackColor="White" />
            <EmptyDataRowStyle CssClass="header" />
            <Columns>
                <asp:TemplateField ItemStyle-Width="0" Visible="true" ShowHeader="false">
                    <ItemTemplate>
                        <asp:ImageButton ID="btnExpand" runat="server" ImageUrl="~/images/plus.gif" Visible="true"
                            CommandName="Expand" />
                        <asp:ImageButton ID="btnCollapse" runat="server" ImageUrl="~/images/minus.gif" Visible="false"
                            CommandName="Collapse" />
                    </ItemTemplate>
                    <ItemStyle Width="0px"></ItemStyle>
                </asp:TemplateField>
                <asp:BoundField DataField="MainId" HeaderText="Record Identifier" ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="DateTime" HeaderText="Statement Date" ItemStyle-HorizontalAlign="Center"
                    NullDisplayText="N/A">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="Dnis" HeaderText="DNIS"></asp:BoundField>
                <asp:BoundField DataField="WaveName" HeaderText="Recording" ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="TPVAgentId" HeaderText="TPV Agent" ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="SalesAgentId" HeaderText="Sales Agent" />
                <asp:BoundField DataField="DecisionMaker" HeaderText="Decision Maker">
                    <ItemStyle Wrap="true" />
                </asp:BoundField>
                <asp:BoundField DataField="CustomerName" HeaderText="Customer Name" ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="CompanyName" HeaderText="Company Name" />
                <asp:BoundField DataField="Product" HeaderText="Product" />
                <asp:BoundField DataField="State" HeaderText="State">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="VerifiedFormatted" HeaderText="Verified">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="Concern" HeaderText="Concern" />
                <asp:TemplateField ItemStyle-Width="0" Visible="true" ShowHeader="false">
                    <ItemTemplate>
                        <asp:Panel ID="pnlDetails" runat="server" Visible="false">
                            </td> </tr>
                            <tr>
                                <td colspan="16" style="padding-left: 20px">
                                    <asp:GridView ID="gvDetail" runat="server" AutoGenerateColumns="False" EmptyDataText="No records found."
                                        Font-Names="Century Gothic" Font-Size="9pt" Width="75%" CellPadding="2" ForeColor="#333333"
                                        GridLines="Both">
                                        <RowStyle ForeColor="#333333" BackColor="#F7F6F3" />
                                        <EmptyDataRowStyle CssClass="header" />
                                        <Columns>
                                            <asp:BoundField DataField="TnFormatted" HeaderText="TN" ItemStyle-Wrap="false" />
                                            <asp:BoundField DataField="DialToneFormatted" HeaderText="PLOC" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField DataField="LocalTollFormatted" HeaderText="ILP/Intra" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField DataField="LdFormatted" HeaderText="PIC/Inter" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField DataField="DialToneFreezeFormatted" HeaderText="PLOC Freeze" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField DataField="LocalTollFreezeFormatted" HeaderText="ILP/Intra Freeze"
                                                ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField DataField="LdFreezeFormatted" HeaderText="PIC/Inter Freeze" ItemStyle-HorizontalAlign="Center" />
                                        </Columns>
                                        <FooterStyle BackColor="#5D7B9D" ForeColor="White" Font-Bold="True" />
                                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                        <HeaderStyle CssClass="header" />
                                        <EditRowStyle BackColor="#999999" />
                                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                    </asp:GridView>
                                </td>
                            </tr>
                        </asp:Panel>
                    </ItemTemplate>
                    <ItemStyle Width="0px"></ItemStyle>
                </asp:TemplateField>
            </Columns>
            <HeaderStyle CssClass="header" />
            <AlternatingRowStyle BackColor="#D8D8D8" />
        </asp:GridView>
    </div>
</asp:Content>
