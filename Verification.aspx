<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Verification.aspx.cs" Inherits="AS_assign1.Verification" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Verification</title>
    <style type="text/css">
        .auto-style1 {
            width: 73%;
            height: 135px;
        }
        .auto-style2 {
            height: 54px;
            width: 317px;
        }
        .auto-style3 {
            height: 54px;
            width: 192px;
        }
                input {
    line-height: 2em; 
}
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Verification</h1>
        <p>
            <asp:Label ID="lbl_verify" runat="server" Text="Please check your email address and enter your verification code"></asp:Label>
&nbsp;to activate your account</p>
        <table class="auto-style1">
            <tr>
                <td class="auto-style3">Verification Code</td>
                <td class="auto-style2">
                    <asp:TextBox ID="tb_code" runat="server" Width="402px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="auto-style3">&nbsp;</td>
                <td class="auto-style2">
                    <asp:Label ID="lbl_error" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
<td class="auto-style3"></td>
                <td class="auto-style2">
                    <asp:Button ID="Button1" runat="server" Height="43px" Text="Verify" Width="246px" OnClick="Button1_Click" />
                </td>
            </tr>
        </table>
        <br />
        <br />
    </form>
</body>
</html>
