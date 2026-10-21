<%@ Page Language="C#"
    AutoEventWireup="true"
    CodeBehind="ForgotPassword.aspx.cs"
    Inherits="SPMaster.ForgotPassword" %>

<!DOCTYPE html>

<html lang="en">

<head runat="server">

    <meta charset="utf-8" />

    <meta name="viewport"
          content="width=device-width, initial-scale=1" />


    <title>
        Forgot Password - SPMaster
    </title>


    <link runat="server"
          href="~/Content/ForgotPassword.css"
          rel="stylesheet"
          type="text/css" />


    <link rel="stylesheet"
          href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" />

</head>


<body class="forgot-body">


    <form id="forgotForm"
          runat="server">


        <main class="forgot-page">


            <section class="forgot-card">


                <%-- LOGO --%>

                <div class="forgot-logo-wrap">

                    <img
                        src="<%= ResolveUrl("~/Images/spmaster-logo.png") %>"
                        alt="SPMaster"
                        class="forgot-logo" />

                </div>



                <%-- =================================================
                     STEP 1 - EMAIL
                     ================================================= --%>

                <asp:Panel
                    ID="pnlEmailStep"
                    runat="server">


                    <div class="forgot-heading">

                        <h1>
                            Forgot password?
                        </h1>


                        <p>
                            Enter your registered email address and we'll send you a reset code.
                        </p>

                    </div>



                    <asp:Label
                        ID="lblMessage"
                        runat="server"
                        Visible="false"
                        CssClass="forgot-message">
                    </asp:Label>



                    <div class="forgot-form-group">

                        <label for="<%= txtEmail.ClientID %>">

                            Email

                        </label>


                        <asp:TextBox
                            ID="txtEmail"
                            runat="server"
                            CssClass="forgot-input"
                            TextMode="Email"
                            MaxLength="254"
                            placeholder="you@example.com">
                        </asp:TextBox>


                        <asp:RequiredFieldValidator
                            ID="rfvEmail"
                            runat="server"
                            ControlToValidate="txtEmail"
                            ErrorMessage="Email address is required."
                            CssClass="forgot-validation"
                            Display="Dynamic"
                            ValidationGroup="EmailStep">
                        </asp:RequiredFieldValidator>


                        <asp:RegularExpressionValidator
                            ID="revEmail"
                            runat="server"
                            ControlToValidate="txtEmail"
                            ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                            ErrorMessage="Please enter a valid email address."
                            CssClass="forgot-validation"
                            Display="Dynamic"
                            ValidationGroup="EmailStep">
                        </asp:RegularExpressionValidator>

                    </div>



                    <asp:Button
                        ID="btnSendCode"
                        runat="server"
                        Text="Send Reset Code"
                        CssClass="forgot-primary-button"
                        ValidationGroup="EmailStep"
                        OnClick="btnSendCode_Click" />



                    <div class="forgot-back">

                        <a href="<%= ResolveUrl("~/Login.aspx") %>">

                            <i class="bi bi-arrow-left"></i>

                            Back to login

                        </a>

                    </div>


                </asp:Panel>



                <%-- =================================================
                     STEP 2 - RESET PASSWORD
                     Hidden until code is sent
                     ================================================= --%>

                <asp:Panel
                    ID="pnlResetStep"
                    runat="server"
                    Visible="false">


                    <div class="forgot-heading">

                        <h1>
                            Reset password
                        </h1>


                        <p>
                            Enter the verification code sent to your email and choose a new password.
                        </p>

                    </div>



                    <asp:Label
                        ID="lblResetMessage"
                        runat="server"
                        Visible="false"
                        CssClass="forgot-message">
                    </asp:Label>



                    <%-- CODE --%>

                    <div class="forgot-form-group">

                        <label for="<%= txtResetCode.ClientID %>">

                            Verification Code

                        </label>


                        <asp:TextBox
                            ID="txtResetCode"
                            runat="server"
                            CssClass="forgot-input"
                            MaxLength="6"
                            placeholder="Enter 6-digit code">
                        </asp:TextBox>


                        <asp:RequiredFieldValidator
                            ID="rfvResetCode"
                            runat="server"
                            ControlToValidate="txtResetCode"
                            ErrorMessage="Verification code is required."
                            CssClass="forgot-validation"
                            Display="Dynamic"
                            ValidationGroup="ResetStep">
                        </asp:RequiredFieldValidator>

                    </div>



                    <%-- NEW PASSWORD --%>

                    <div class="forgot-form-group">

                        <label for="<%= txtNewPassword.ClientID %>">

                            New Password

                        </label>


                        <asp:TextBox
                            ID="txtNewPassword"
                            runat="server"
                            CssClass="forgot-input"
                            TextMode="Password"
                            placeholder="Enter new password">
                        </asp:TextBox>


                        <asp:RequiredFieldValidator
                            ID="rfvNewPassword"
                            runat="server"
                            ControlToValidate="txtNewPassword"
                            ErrorMessage="New password is required."
                            CssClass="forgot-validation"
                            Display="Dynamic"
                            ValidationGroup="ResetStep">
                        </asp:RequiredFieldValidator>

                    </div>



                    <%-- CONFIRM PASSWORD --%>

                    <div class="forgot-form-group">

                        <label for="<%= txtConfirmPassword.ClientID %>">

                            Confirm Password

                        </label>


                        <asp:TextBox
                            ID="txtConfirmPassword"
                            runat="server"
                            CssClass="forgot-input"
                            TextMode="Password"
                            placeholder="Confirm new password">
                        </asp:TextBox>


                        <asp:RequiredFieldValidator
                            ID="rfvConfirmPassword"
                            runat="server"
                            ControlToValidate="txtConfirmPassword"
                            ErrorMessage="Please confirm your new password."
                            CssClass="forgot-validation"
                            Display="Dynamic"
                            ValidationGroup="ResetStep">
                        </asp:RequiredFieldValidator>


                        <asp:CompareValidator
                            ID="cvPasswords"
                            runat="server"
                            ControlToValidate="txtConfirmPassword"
                            ControlToCompare="txtNewPassword"
                            ErrorMessage="Passwords do not match."
                            CssClass="forgot-validation"
                            Display="Dynamic"
                            ValidationGroup="ResetStep">
                        </asp:CompareValidator>

                    </div>



                    <asp:Button
                        ID="btnResetPassword"
                        runat="server"
                        Text="Reset Password"
                        CssClass="forgot-primary-button"
                        ValidationGroup="ResetStep"
                        OnClick="btnResetPassword_Click" />



                    <div class="forgot-back">

                        <asp:LinkButton
                            ID="btnBackToEmail"
                            runat="server"
                            CausesValidation="false"
                            OnClick="btnBackToEmail_Click">

                            <i class="bi bi-arrow-left"></i>

                            Use another email

                        </asp:LinkButton>

                    </div>


                </asp:Panel>


            </section>


        </main>


    </form>


</body>

</html>