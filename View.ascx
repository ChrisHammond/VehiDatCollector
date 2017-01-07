<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="Christoc.Modules.VehiDataCollector.View" %>
<asp:Repeater ID="rptVehicleList" runat="server" OnItemDataBound="rptVehicleListOnItemDataBound" OnItemCommand="rptVehicleListOnItemCommand">
    <HeaderTemplate>
        <ul class="tm_tl">
    </HeaderTemplate>

    <ItemTemplate>
        <li class="tm_t">
            <h3>
                <asp:Label ID="lblVehicleName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem,"VehicleName").ToString() %>' />
            </h3>
            <asp:Label ID="lblVehicleDescription" runat="server" Text='<%#DataBinder.Eval(Container.DataItem,"VehicleDescription").ToString() %>' CssClass="tm_td" />

            <asp:Panel ID="pnlAdmin" runat="server" Visible="false">
                <asp:HyperLink ID="lnkEdit" runat="server" ResourceKey="EditItem.Text" Visible="false" Enabled="false" />
                <asp:LinkButton ID="lnkDelete" runat="server" ResourceKey="DeleteItem.Text" Visible="false" Enabled="false" CommandName="Delete" />
            </asp:Panel>
        </li>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
</asp:Repeater>
