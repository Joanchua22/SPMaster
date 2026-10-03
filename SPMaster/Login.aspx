<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Login.aspx.cs"
    Inherits="SPMaster.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />

    <!-- Important for responsive mobile design -->
    <meta name="viewport"
          content="width=device-width, initial-scale=1.0" />

    <title>Login - SPMaster</title>

    <link href="Content/Login.css" rel="stylesheet" />
</head>

<body>

    <form id="form1" runat="server">

        <main class="login-page">

            <!-- Decorative background shapes -->
            <div class="background-shape shape-one"></div>
            <div class="background-shape shape-two"></div>
            <div class="background-shape shape-three"></div>

            <div class="login-wrapper">

                <!-- Login Card -->
                <section class="login-card">

                    <!-- Logo -->
                    <div class="logo-container">
                        <img src="Images/spmaster-logo.png"
                             alt="SPMaster Logo"
                             class="login-logo" />
                    </div>

                    <!-- Heading -->
                    <h1>Welcome back!</h1>

                    <p class="login-subtitle">
                        Log in to keep your streak alive.
                    </p>


                    <!-- Error message -->
                    <asp:Label
                        ID="lblMessage"
                        runat="server"
                        CssClass="error-message"
                        Visible="false">
                    </asp:Label>


                    <!-- Email -->
                    <div class="form-group">

                        <label for="txtEmail">
                            Email
                        </label>

                        <asp:TextBox
                            ID="txtEmail"
                            runat="server"
                            CssClass="form-input"
                            TextMode="Email"
                            placeholder="you@example.com">
                        </asp:TextBox>

                        <asp:RequiredFieldValidator
                            ID="rfvEmail"
                            runat="server"
                            ControlToValidate="txtEmail"
                            ErrorMessage="Email is required."
                            CssClass="validation-message"
                            Display="Dynamic">
                        </asp:RequiredFieldValidator>

                        <asp:RegularExpressionValidator
                            ID="revEmail"
                            runat="server"
                            ControlToValidate="txtEmail"
                            ErrorMessage="Please enter a valid email."
                            CssClass="validation-message"
                            Display="Dynamic"
                            ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$">
                        </asp:RegularExpressionValidator>

                    </div>


                    <!-- Password -->
                    <div class="form-group">

                        <label for="txtPassword">
                            Password
                        </label>

                        <div class="password-container">

                            <asp:TextBox
                                ID="txtPassword"
                                runat="server"
                                CssClass="form-input password-input"
                                TextMode="Password"
                                placeholder="Enter your password">
                            </asp:TextBox>

                            <button
                                type="button"
                                class="password-toggle"
                                onclick="togglePassword()"
                                aria-label="Show or hide password">

                                <span id="eyeIcon">◉</span>

                            </button>

                        </div>

                        <asp:RequiredFieldValidator
                            ID="rfvPassword"
                            runat="server"
                            ControlToValidate="txtPassword"
                            ErrorMessage="Password is required."
                            CssClass="validation-message"
                            Display="Dynamic">
                        </asp:RequiredFieldValidator>

                    </div>


                    <!-- Remember + Forgot Password -->
                    <div class="login-options">

                        <label class="remember-container">

                            <asp:CheckBox
                                ID="chkRemember"
                                runat="server"
                                CssClass="remember-checkbox" />

                            <span>Remember me</span>

                        </label>

                        <asp:HyperLink
                            ID="lnkForgotPassword"
                            runat="server"
                            NavigateUrl="~/ForgotPassword.aspx"
                            CssClass="forgot-link">

                            Forgot password?

                        </asp:HyperLink>

                    </div>


                    <!-- Login button -->
                    <asp:Button
                        ID="btnLogin"
                        runat="server"
                        Text="Log in"
                        CssClass="login-button"
                        OnClick="btnLogin_Click" />


                    <!-- Register -->
                    <p class="signup-text">

                        Don't have an account?

                        <asp:HyperLink
                            ID="lnkRegister"
                            runat="server"
                            NavigateUrl="~/Register.aspx"
                            CssClass="signup-link">

                            Sign up free

                        </asp:HyperLink>

                    </p>

                </section>


                <!-- Back Home -->
                <asp:HyperLink
                    ID="lnkHome"
                    runat="server"
                    NavigateUrl="~/Home.aspx"
                    CssClass="back-home">

                    Back to home

                </asp:HyperLink>

            </div>

        </main>

    </form>


    <script>

        function togglePassword() {

            const password =
                document.getElementById('<%= txtPassword.ClientID %>');

            const eyeIcon =
                document.getElementById('eyeIcon');

            if (password.type === "password") {

                password.type = "text";
                eyeIcon.textContent = "⊘";

            }
            else {

                password.type = "password";
                eyeIcon.textContent = "◉";

            }
        }

    </script>

</body>
</html>