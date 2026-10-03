<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Register.aspx.cs"
    Inherits="SPMaster.Register" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">

    <meta charset="utf-8" />

    <meta name="viewport"
          content="width=device-width, initial-scale=1.0" />

    <title>Sign Up - SPMaster</title>

    <link href="Content/Register.css" rel="stylesheet" />

</head>


<body>

<form id="form1" runat="server">

    <main class="register-page">

        <!-- Decorative background -->
        <div class="background-shape shape-one"></div>
        <div class="background-shape shape-two"></div>
        <div class="background-shape shape-three"></div>


        <div class="register-wrapper">

            <section class="register-card">


                <!-- Logo -->
                <div class="logo-container">

                    <img src="Images/spmaster-logo.png"
                         alt="SPMaster Logo"
                         class="register-logo" />

                </div>


                <!-- Heading -->
                <h1>Join SPMaster!</h1>

                <p class="register-subtitle">
                    Create your free account and start your SPM streak today.
                </p>


                <!-- General error / success message -->
                <asp:Label
                    ID="lblMessage"
                    runat="server"
                    CssClass="message"
                    Visible="false">
                </asp:Label>


                <!-- ==============================
                     ROLE SELECTION
                =============================== -->

                <div class="role-selector">

                    <button
                        type="button"
                        id="btnStudentRole"
                        class="role-button active"
                        onclick="selectRole('Student')">

                        Student

                    </button>


                    <button
                        type="button"
                        id="btnLecturerRole"
                        class="role-button"
                        onclick="selectRole('Lecturer')">

                        Lecturer

                    </button>


                    <button
                        type="button"
                        class="role-button admin-button"
                        disabled="disabled">

                        🔒 Admin

                    </button>

                </div>


                <asp:HiddenField
                    ID="hfRole"
                    runat="server"
                    Value="Student" />


                <!-- ==============================
                     FULL NAME
                =============================== -->

                <div class="form-group">

                    <label for="<%= txtFullName.ClientID %>">
                        Full name
                    </label>

                    <asp:TextBox
                        ID="txtFullName"
                        runat="server"
                        CssClass="form-input"
                        MaxLength="50"
                        placeholder="Aina Lee">
                    </asp:TextBox>


                    <asp:RequiredFieldValidator
                        ID="rfvFullName"
                        runat="server"
                        ControlToValidate="txtFullName"
                        ErrorMessage="Full name is required."
                        CssClass="validation-message"
                        Display="Dynamic">
                    </asp:RequiredFieldValidator>

                </div>


                <!-- ==============================
                     EMAIL
                =============================== -->

                <div class="form-group">

                    <label for="<%= txtEmail.ClientID %>">
                        Email
                    </label>

                    <asp:TextBox
                        ID="txtEmail"
                        runat="server"
                        TextMode="Email"
                        CssClass="form-input"
                        MaxLength="150"
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
                        ErrorMessage="Please enter a valid email address."
                        CssClass="validation-message"
                        Display="Dynamic"
                        ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$">
                    </asp:RegularExpressionValidator>

                </div>

                <div class="form-group">

                    <label for="<%= ddlGender.ClientID %>">
                        Gender
                    </label>

                    <asp:DropDownList
                        ID="ddlGender"
                        runat="server"
                        CssClass="form-input form-select">

                        <asp:ListItem
                            Text="Select your gender"
                            Value="">
                        </asp:ListItem>

                        <asp:ListItem
                            Text="Male"
                            Value="Male">
                        </asp:ListItem>

                        <asp:ListItem
                            Text="Female"
                            Value="Female">
                        </asp:ListItem>

                    </asp:DropDownList>

                    <asp:RequiredFieldValidator
                        ID="rfvGender"
                        runat="server"
                        ControlToValidate="ddlGender"
                        InitialValue=""
                        ErrorMessage="Please select your gender."
                        CssClass="validation-message"
                        Display="Dynamic">
                    </asp:RequiredFieldValidator>

                </div>

                <!-- ==============================
                     PASSWORD
                =============================== -->

                <div class="form-group">

                    <label for="<%= txtPassword.ClientID %>">
                        Password
                    </label>


                    <div class="password-container">

                        <asp:TextBox
                            ID="txtPassword"
                            runat="server"
                            TextMode="Password"
                            CssClass="form-input password-input"
                            MaxLength="100"
                            placeholder="Enter password">
                        </asp:TextBox>


                        <button
                            type="button"
                            class="password-toggle"
                            onclick="togglePassword(
                                '<%= txtPassword.ClientID %>',
                                'passwordEye'
                            )"
                            aria-label="Show or hide password">

                            <span id="passwordEye">◉</span>

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


                    <asp:RegularExpressionValidator
                        ID="revPassword"
                        runat="server"
                        ControlToValidate="txtPassword"
                        ErrorMessage="Password must contain at least 8 characters."
                        ValidationExpression="^.{8,}$"
                        CssClass="validation-message"
                        Display="Dynamic">
                    </asp:RegularExpressionValidator>

                </div>


                <!-- ==============================
                     CONFIRM PASSWORD
                =============================== -->

                <div class="form-group">

                    <label for="<%= txtConfirmPassword.ClientID %>">
                        Confirm password
                    </label>


                    <div class="password-container">

                        <asp:TextBox
                            ID="txtConfirmPassword"
                            runat="server"
                            TextMode="Password"
                            CssClass="form-input password-input"
                            MaxLength="100"
                            placeholder="Confirm password">
                        </asp:TextBox>


                        <button
                            type="button"
                            class="password-toggle"
                            onclick="togglePassword(
                                '<%= txtConfirmPassword.ClientID %>',
                                'confirmEye'
                            )"
                            aria-label="Show or hide password">

                            <span id="confirmEye">◉</span>

                        </button>

                    </div>


                    <asp:RequiredFieldValidator
                        ID="rfvConfirmPassword"
                        runat="server"
                        ControlToValidate="txtConfirmPassword"
                        ErrorMessage="Please confirm your password."
                        CssClass="validation-message"
                        Display="Dynamic">
                    </asp:RequiredFieldValidator>


                    <asp:CompareValidator
                        ID="cvPassword"
                        runat="server"
                        ControlToValidate="txtConfirmPassword"
                        ControlToCompare="txtPassword"
                        ErrorMessage="Passwords do not match."
                        CssClass="validation-message"
                        Display="Dynamic">
                    </asp:CompareValidator>

                </div>


                <!-- ==============================
                     SCHOOL
                =============================== -->

                <div class="form-group">

                    <label for="<%= ddlSchool.ClientID %>">
                        School / Institution
                    </label>


                    <asp:DropDownList
                        ID="ddlSchool"
                        runat="server"
                        CssClass="form-input form-select">

                        <asp:ListItem
                            Text="Select your school"
                            Value="">
                        </asp:ListItem>

                        <asp:ListItem
                            Text="Asia Pacific University (APU)"
                            Value="Asia Pacific University">
                        </asp:ListItem>

                        <asp:ListItem
                            Text="SMK"
                            Value="SMK">
                        </asp:ListItem>

                        <asp:ListItem
                            Text="Private School"
                            Value="Private School">
                        </asp:ListItem>

                        <asp:ListItem
                            Text="International School"
                            Value="International School">
                        </asp:ListItem>

                        <asp:ListItem
                            Text="Other"
                            Value="Other">
                        </asp:ListItem>

                    </asp:DropDownList>


                    <asp:RequiredFieldValidator
                        ID="rfvSchool"
                        runat="server"
                        ControlToValidate="ddlSchool"
                        InitialValue=""
                        ErrorMessage="Please select your school."
                        CssClass="validation-message"
                        Display="Dynamic">
                    </asp:RequiredFieldValidator>

                </div>


                <!-- ==============================
                     TERMS
                =============================== -->

                <div class="terms-container">

                    <asp:CheckBox
                        ID="chkTerms"
                        runat="server"
                        CssClass="terms-checkbox" />

                    <span>

                        I agree to the

                        <a href="#"
                           class="terms-link">

                            Terms &amp; Privacy Policy

                        </a>

                    </span>

                </div>


                <!-- ==============================
                     CREATE ACCOUNT
                =============================== -->

                <asp:Button
                    ID="btnRegister"
                    runat="server"
                    Text="Create account"
                    CssClass="register-button"
                    OnClick="btnRegister_Click" />


                <!-- Login -->
                <p class="login-text">

                    Already have an account?

                    <asp:HyperLink
                        ID="lnkLogin"
                        runat="server"
                        NavigateUrl="~/Login.aspx"
                        CssClass="login-link">

                        Log in

                    </asp:HyperLink>

                </p>


            </section>

        </div>

    </main>

</form>


<script>

    function selectRole(role) {

        const studentButton =
            document.getElementById("btnStudentRole");

        const lecturerButton =
            document.getElementById("btnLecturerRole");

        const hiddenRole =
        document.getElementById("<%= hfRole.ClientID %>");


    studentButton.classList.remove("active");
    lecturerButton.classList.remove("active");


    if (role === "Student") {

        studentButton.classList.add("active");

    }

    else if (role === "Lecturer") {

        lecturerButton.classList.add("active");

    }


    hiddenRole.value = role;
}



function togglePassword(inputId, iconId) {

    const passwordInput =
        document.getElementById(inputId);

    const eyeIcon =
        document.getElementById(iconId);


    if (passwordInput.type === "password") {

        passwordInput.type = "text";

        eyeIcon.textContent = "⊘";

    }

    else {

        passwordInput.type = "password";

        eyeIcon.textContent = "◉";

    }
}

</script>


</body>

</html>