<%@ Page Title="" Language="C#" MasterPageFile="~/Dashboard.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="SPMaster.Lecturer.Dashboard" %>
<asp:Content ID="LecturerTitle" ContentPlaceHolderID="DashboardTitle" runat="server">
    My Dashboard
</asp:Content>


<asp:Content ID="LecturerContent" ContentPlaceHolderID="DashboardContent" runat="server">
    <h2>Welcome, Mr. Rahman!</h2>
    <p>Your lecturer dashboard content will appear here.</p>
</asp:Content>

<asp:Content ID="LecturerUser" ContentPlaceHolderID="DashboardUser" runat="server">
    <div class="user-summary">
        <span class="user-avatar" aria-hidden="true">
            R
        </span>
        <div class="user-details">
            <span class="user-name">Mr. Rahman</span>
            <span class="user-role">Lectuer</span>
        </div>
    </div>
</asp:Content>
