<%@ Page Title="" Language="C#" MasterPageFile="~/Dashboard.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="SPMaster.Student.Dashboard" %>

<asp:Content ID="StudentTitle" ContentPlaceHolderID="DashboardTitle" runat="server">
    My Dashboard
</asp:Content>


<asp:Content ID="StudentContent" ContentPlaceHolderID="DashboardContent" runat="server">
    <h2>Welcome, Aina!</h2>
    <p>Your student dashboard content will appear here.</p>
</asp:Content>

