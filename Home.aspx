<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="AS_assign1.Home" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function SessionExpireAlert(timeout) {
            var seconds = timeout / 1000;
            document.getElementsByName('<%=Label1.ClientID %>').innerHTML = seconds;
            setInterval(function () {
                seconds--;
                document.getElementById('<%=Label1.ClientID %>').innerHTML = seconds;
            }, 1000);
            setTimeout(function () {
                //Show Popup before 20 seconds of timeout.
                if (confirm("20s left! Stay on site?")) {
                    ResetSession();
                }
            }, timeout - 20 * 1000);
            setTimeout(function () {

                window.location = "Login.aspx";
            }, timeout);
        };
        function ResetSession() {
            //Redirect to refresh Session.
            window.location = window.location.href;
        }

<%--        function validatepwd() {
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
                }--%>
    </script>
    <style type="text/css">
        .auto-style1 {
            height: 31px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div >
            <h1  >HomePage</h1>
            <asp:Label ID="message"  runat="server" Text=""></asp:Label>

            <table class="auto-style1"  >
                <tr>
                    <td>Email Address :</td>
                    <td>
                        <asp:Label ID="dis_email" runat="server"></asp:Label>
                    </td>
                    <td>
                        &nbsp;</td>
                </tr>
                <tr>
                    <td>Name :</td>
                    <td>
                        <asp:Label ID="dis_name" runat="server"></asp:Label>
                    </td>
                    <td>
                        &nbsp;</td>
                </tr
                <tr>
                    <td>Credit Card Info :</td>
                    <td>
                        <asp:Label ID="dis_credit" runat="server"></asp:Label>
                </td>
                    <td>
                        &nbsp;</td>
                </tr>
                 <tr>
                    <td>Birth Date:</td>
                    <td>
                        <asp:Label ID="dis_dob" runat="server"></asp:Label>
                </td>
                    <td>
                        &nbsp;</td>
                </tr>
                 <tr>
                    <td>Photo :</td>
                    <td>
                        <asp:Image ID="Image1" runat="server"  Height="30%" Width="30%"/>
                     </td>
                    <td>
                        &nbsp;</td>
                </tr>
                 <tr>
                    <td class="auto-style1">
                        </td>
                    <td class="auto-style1">
                        </td>
                    <td class="auto-style1">
                        </td>
                </tr>
                 <tr>
                    <td class="auto-style1">
                        Change Password?</td>
                    <td class="auto-style1">
                        <asp:LinkButton ID="LinkButton1" runat="server" OnClick="LinkButton1_Click">Click Here</asp:LinkButton>
                        <br />
                     </td>
                    <td class="auto-style1">
                            &nbsp;</td>
                </tr>
                 <tr>
                    <td>
                        <asp:Label ID="home_error" runat="server" ViewStateMode="Enabled" ></asp:Label>
                     </td>
                    <td>
                        &nbsp;</td>
                    <td>
                        &nbsp;</td>
                </tr>
            </table>

            <div >
                <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                <h3>Session time out: 
                    <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
                    <span id="countdown">&nbsp;</span>seconds</h3>

            </div>
                       <asp:Button ID="btn_logout"  runat="server" OnClick="btn_logout_Click" Text="log out" Height="47px" Width="150px" />


        </div>
    </form>
</body>

</html>

