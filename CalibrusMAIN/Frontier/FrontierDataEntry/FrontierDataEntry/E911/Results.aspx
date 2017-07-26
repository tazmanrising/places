<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="Results.aspx.cs" Inherits="E911_Results" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div style="width: 100%; margin: auto;">
        <asp:GridView ID="gvE911Report" runat="server" AutoGenerateColumns="False" EmptyDataText="No records found."
            Font-Names="Century Gothic" Font-Size="9pt" Width="100%" OnRowDataBound="gvE911Report_RowDataBound"
            OnRowCommand="gvE911Report_RowCommand" CellPadding="3" AllowPaging="True" PageSize="15"
            OnPageIndexChanging="gvE911Report_PageIndexChanging">
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
                <asp:BoundField DataField="SubscriberId" HeaderText="Subscriber Id" ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="Name" HeaderText="Name" ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="Signature" HeaderText="Signature" ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="BirthYear" HeaderText="Birth Year" ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="TnFormatted" HeaderText="Phone Number" ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="Email" HeaderText="Email" ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="GeneralAction" HeaderText="GeneralAction" ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="GeneralDate" HeaderText="GeneralDate" ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="E911Action" HeaderText="E911Action" ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="E911Date" HeaderText="E911Date" ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="IsData" HeaderText="IsData" ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="IsVoip" HeaderText="IsVoip" ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="User" HeaderText="User" ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:TemplateField ItemStyle-Width="0" Visible="true" ShowHeader="false">
                    <ItemTemplate>
                        <asp:Panel ID="pnlDetails" runat="server" Visible="false">
                            </td> </tr>
                            <tr>
                                <td colspan="16" style="padding-left: 20px">
                                    <asp:GridView ID="gvE911Detail" runat="server" AutoGenerateColumns="False" EmptyDataText="No records found."
                                        OnRowDataBound="gvE911Detail_RowDataBound" Font-Names="Century Gothic" Font-Size="9pt"
                                        Width="75%" CellPadding="2" ForeColor="#333333" GridLines="Both">
                                        <RowStyle ForeColor="#333333" BackColor="#F7F6F3" />
                                        <EmptyDataRowStyle CssClass="header" />
                                        <Columns>
                                            <asp:BoundField DataField="CallDateTime" HeaderText="Call Date Time" ItemStyle-HorizontalAlign="Center"
                                                NullDisplayText="N/A">
                                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="WavName" HeaderText="Recording" ItemStyle-HorizontalAlign="Center">
                                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Disposition" HeaderText="Disposition" ItemStyle-HorizontalAlign="Center">
                                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="TotalTime" HeaderText="Total Time" ItemStyle-HorizontalAlign="Center">
                                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                            </asp:BoundField>
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
