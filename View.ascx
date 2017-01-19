<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="Christoc.Modules.VehiDataCollector.View" %>

<asp:Label ID="lblVehicles" runat="server" resourcekey="lblVehicles" />
<asp:DropDownList runat="server" id="ddlVehicles" DataTextField="VehicleName" DataValueField="VehicleId" />


<asp:Repeater ID="rptEntryList" runat="server" OnItemDataBound="rptEntryListOnItemDataBound" OnItemCommand="rptEntryListOnItemCommand">
    <HeaderTemplate>
        <ul class="tm_tl">
    </HeaderTemplate>

    <ItemTemplate>
        <li class="tm_t">
            <h3>
                <asp:Label ID="lbEntryNmae" runat="server" Text='<%#DataBinder.Eval(Container.DataItem,"EntryName").ToString() %>' />
            </h3>
            <asp:Label ID="lblEntryDescription" runat="server" Text='<%#DataBinder.Eval(Container.DataItem,"EntryDescription").ToString() %>' CssClass="tm_td" />
            <asp:Label ID="lblCreatedOnDate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem,"CreatedOnDate").ToString() %>' CssClass="tm_td" />
            



<%--            <asp:Panel ID="pnlAdmin" runat="server" Visible="false">
                <asp:HyperLink ID="lnkEdit" runat="server" ResourceKey="EditItem.Text" Visible="false" Enabled="false" />
                <asp:LinkButton ID="lnkDelete" runat="server" ResourceKey="DeleteItem.Text" Visible="false" Enabled="false" CommandName="Delete" />
            </asp:Panel>--%>
        </li>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
</asp:Repeater>
