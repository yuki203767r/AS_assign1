<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Authenticator.aspx.cs" Inherits="AS_assign1.Authenticator" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Authentication</title>
    <style type="text/css">
        .auto-style1 {
            width: 72%;
            height: 139px;
        }
        .auto-style2 {
            width: 448px;
        }
    </style>
</head>
<body>
<form id="form1" runat="server">  
        <div class="container" >  
            <div class="jumbotron">  
                <h1 class="text-info text-center">Google Authenticator</h1>  
                <hr />  
                <div>

                <table class="auto-style1"  align="center">
                    <tr>
                        <td class="auto-style2">  
                            <div class="center" align="center">
                             <asp:Image   ID="imgQrCode" runat="server" />  

                            </div>
                        </td>
                        <td>  
                            <div>  
                                <span style="font-weight: bold; font-size: 14px;">Account Name:</span>  
                            </div>  
                            <div>  
                                <asp:Label runat="server" ID="lblAccountName"></asp:Label>  
                            </div>  
                               
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style2">&nbsp;</td>
                        <td>  
                               
                        <div>  
                            <span style="font-weight: bold; font-size: 14px;">Secret Key:</span>  
                        </div>  
                            <div>  
                                <asp:Label runat="server" ID="lblManualSetupCode"></asp:Label>  
                            </div>  
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style2">  
                            <asp:TextBox runat="server" CssClass="form-control" ID="txtSecurityCode" MaxLength="50" ToolTip="Please enter security code you get on your authenticator application" Width="298px"></asp:TextBox>  
                        </td>
                        <td>  
                        <asp:Button ID="btnValidate" OnClick="btnValidate_Click" CssClass="btn btn-primary" runat="server" Text="Validate" />  
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style2">  
                        <asp:Label class="center" ID="lblResult" runat="server" Text=""></asp:Label>  
                        </td>
                        <td>  
                            &nbsp;</td>
                    </tr>
                </table>
                    <br />
                <div class="row">  
                    <div class="alert alert-success col-md-12" runat="server" role="alert">  
                    </div>  
                </div>  
            </div>  
                </div>
        </div>  
    </form>  
</body>
</html>
