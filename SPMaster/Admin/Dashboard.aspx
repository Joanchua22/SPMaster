<%@ Page Title="" Language="C#" MasterPageFile="~/Dashboard.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="SPMaster.Admin.Dashboard" %>
<asp:Content ID="AdminTitle" ContentPlaceHolderID="DashboardTitle" runat="server">
    My Dashboard
</asp:Content>


<asp:Content ID="AdminContent" ContentPlaceHolderID="DashboardContent" runat="server">
    <h2>Welcome, Admin!</h2>
    <p>Your admin dashboard content will appear here.</p>
</asp:Content>

<asp:Content ID="LecturerUser" ContentPlaceHolderID="DashboardUser" runat="server">
    <div class="user-summary">
        <span class="user-avatar" aria-hidden="true">
            Ad
        </span>
        <div class="user-details">
            <span class="user-name">Admin</span>
            <span class="user-role">Admin</span>
        </div>
    </div>
</asp:Content>