<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="AS_assign1.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login</title>
        <script src ="https://www.google.com/recaptcha/api.js?render=6Lc1bTodAAAAAFXtNNoIRPntI_aOMbXNhW4g1M-M"></script>
        <script type="text/javascript">
            function LockedExpireAlert(timeout) {
                var seconds = timeout / 1000;
                document.getElementsByName("countdown").innerHTML = timeout;
                setInterval(function () {
                    timeout--;
                    document.getElementById("countdown").innerHTML = timeout;
                }, 1000);

        </script>

    <style>        input {
    line-height: 2em; 
}</style>
</head>
<body>
    <form id="form1" runat="server">
        <fieldset>
            <h1>Login</h1>
           
            <table class="auto-style1">
                    <asp:Label ID="login_error" runat="server" Text="Error message here "></asp:Label>
                    <asp:Label ID="countdown" runat="server" Text=""></asp:Label>

                <tr>
                    <td style="width:20%">Email address :</td>
                    <td style="width:60%">
            <asp:TextBox ID="login_email" runat="server" Height="25px" Width="540px"></asp:TextBox>
                    </td>
                    <td style="width:20%"><asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="invalid email format" ControlToValidate="login_email" ForeColor="Red" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator></td>

                </tr>
                <tr>
                    <td >Password :</td>
                    <td >
            <asp:TextBox ID="login_pwd" TextMode="Password" runat="server" Height="25px" Width="540px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td >&nbsp;</td>
                    <td>
                        <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
                    </td>
                </tr>
                <tr>
                    <td >            
                        <asp:Button ID="login_btn" runat="server" Text="Login" onclick="Loginbtn" Height="44px" Width="137px"/>
                    </td>

                </tr>
                   
            </table>
        </fieldset>
    </form>

    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6Lc1bTodAAAAAFXtNNoIRPntI_aOMbXNhW4g1M-M', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>

</body>
</html>
