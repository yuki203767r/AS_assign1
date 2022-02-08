<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Change.aspx.cs" Inherits="AS_assign1.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Change password</title>
    <script>
        function validatepwd() {
            var str = document.getElementById('<%=changepwd.ClientID %>').value;
            if (str.length < 12) {
                document.getElementById("pwdchecker").innerHTML = "Password length must be at least 12 characters";
                //   document.getElementById("pwdchecker").style.color = "Blue";
                return ("too short");
            }

            else if (str.search(/[A-Z]/) == -1) {
                document.getElementById("pwdchecker").innerHTML = "Password require at least 1 Uppercase ";
                //   document.getElementById("pwdchecker").style.color = "Blue";
                return ("no uppercase");
            }

            else if (str.search(/[a-z]/) == -1) {
                document.getElementById("pwdchecker").innerHTML = "Password require at least 1 lowercase ";
                //   document.getElementById("pwdchecker").style.color = "Blue";
                return ("no lowercase");
            }

            else if (str.search(/[0-9]/) == -1) {
                document.getElementById("pwdchecker").innerHTML = "Password require at least 1 number";
                //document.getElementById("pwdchecker").style.color = "Blue";
                return ("no_number");
            }

            else if (str.search(/\W/) == -1) {
                document.getElementById("pwdchecker").innerHTML = "Password require at least 1 Special Symbol";
                //  document.getElementById("pwdchecker").style.color = "Blue";
                return ("no special symbol");
            }

            document.getElementById("pwdchecker").innerHTML = "Excellent"
            document.getElementById("pwdchecker").style.color = "Green";
        }
        function matchpwd() {
            var pwd = document.getElementById('<%=changepwd.ClientID %>').value;
                   var cfmpwd = document.getElementById('<%=cfmpwd.ClientID %>').value;
            if (cfmpwd == pwd) {
                document.getElementById("cfmpwdchecker").innerHTML = "";

            } else {
                document.getElementById("cfmpwdchecker").innerHTML = "Both password does not match";

            }
               }
    </script>
    <style type="text/css">
        .auto-style1 {
            width: 80%;
            height: 126px;
        }
        .auto-style2 {
            width: 256px;
        } 
        input {
    line-height: 2em; 
}
        .auto-style3 {
            width: 240px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Password Changing</h1>
        <p>
            <asp:Label ID="Label1" runat="server" ForeColor="Red"></asp:Label>
        </p>
        <div>
            <table class="auto-style1">
                <tr>
                    <td class="auto-style2">New Password</td>
                    <td class="auto-style3">
                        <asp:TextBox ID="changepwd" onkeyup="javascript:validatepwd()" TextMode="Password" runat="server" Width="220px"></asp:TextBox>
                        </td>
                    <td>
                        <asp:Label ID="pwdchecker" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                </tr>
                <tr>
                    <td class="auto-style2">Confirm new password</td>
                    <td class="auto-style3">
                        <asp:TextBox ID="cfmpwd" onkeyup="javascript:matchpwd()" TextMode="Password" runat="server" Width="221px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Label runat="server" ForeColor="Red" ID="cfmpwdchecker"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">&nbsp;</td>
                    <td class="auto-style3">
                        <asp:Label runat="server" ForeColor="Red" ID="change_error"></asp:Label>
                    </td>
                    <td>
                        &nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style2">&nbsp;</td>
                    <td class="auto-style3">
                        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Submit" Height="43px" Width="134px" />
                    </td>
                    <td>
                        &nbsp;</td>
                </tr>
                </table>
        </div>
    </form>
</body>
</html>
