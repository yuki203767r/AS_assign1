<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="AS_assign1.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Registration Form</title>


    <script type="text/javascript">
        function validatepwd() {
            var str = document.getElementById('<%=pwd.ClientID %>').value;
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
            var pwd = document.getElementById('<%=pwd.ClientID %>').value;
            var cfmpwd = document.getElementById('<%=cfmpwd.ClientID %>').value;
            if (cfmpwd.length >1) {
                 document.getElementById("cfmpwdchecker").innerHTML = "";


            }
        }
        function fnClient() {
            var fn = document.getElementById('<%=FName.ClientID %>').value;
            if (fn.length < 1) {
                document.getElementById("fnClient").innerHTML = "first name is required";

            } 

            //else if (/[^a-zA-Z]/.test(fn)) {
            else if (/^[A-Za-z]+[A-Za-z ]*$/ .test(fn)) {

                document.getElementById("fnClient").innerHTML = "";

            }
            else {
                document.getElementById("fnClient").innerHTML = "only alphabet";

            }
        }

        function lnClient() {
            var fn = document.getElementById('<%=LName.ClientID %>').value;
                    if (fn.length < 1) {
                        document.getElementById("lnClient").innerHTML = "last name is required";

                    }
                    else if (/^[A-Za-z]+[A-Za-z ]*$/.test(fn)) {
                        document.getElementById("lnClient").innerHTML = "";

            }
                    else {
                        document.getElementById("lnClient").innerHTML = "only alphabet";

                    }
            
        }
        function ccClient() {
            var str = document.getElementById('<%=cc.ClientID %>').value;
            if (str.length < 1) {
                document.getElementById("ccClient").innerHTML = "credit card info is required";

            }
            else if (str.length >= 13) {
                document.getElementById("ccClient").innerHTML = "exceeds 12 numbers";

            }
            else if(str.length < 12 ){
                document.getElementById("ccClient").innerHTML = "need 12 numbers";

            }
            else {
                document.getElementById("ccClient").innerHTML = "";

            }
                    if (/[^0-9]/.test(str)) {
                        document.getElementById("ccClient").innerHTML = "only numbers";

                    }
                }
        function emailClient() {
            var str = document.getElementById('<%=email.ClientID %>').value;
            if (str.length < 1) {
                document.getElementById("emailClient").innerHTML = "email is required";

            } else {
                document.getElementById("emailClient").innerHTML = "";
            }
        }
                   //if (str.match(/^\w+[\+\.\w-]*@([\w-]+\.)*\w+[\w-]*\.([a-z]{2,4}|\d+)$/i)) {
                   //    document.getElementById("emailClient").innerHTML = "Please enter a valid email address";

                   //}
        function dobClient() {
            var str = document.getElementById('<%=dob.ClientID %>').value;
                   if (str.length < 1) {
                       document.getElementById("dobClient").innerHTML = "dob is required";

                   } else {
                       document.getElementById("dobClient").innerHTML = "";
                   }
               }
    </script>

    <style type="text/css">
        input {
    line-height: 2em; 
}
  /*      .auto-style1 {
            height: 146px;
        }
        .auto-style5 {
            width: 820px;
        }
        .auto-style6 {
            width: 233px;
        }
        .auto-style7 {
            width: 1136px;
        }*/
        .auto-style1 {
            height: 33px;
        }
        </style>
</head>
<body>
    <form method="post" id="form1" runat="server">
        <div  >
            <fieldset>

                <h1>Registration</h1>
                <asp:Label ID="error_message" runat="server" Text="&amp;nbsp" ></asp:Label>
                            <asp:Label ID="lbl_error" runat="server" ForeColor="Red"></asp:Label>
                <br />
                <table class="auto-style1" width="100%"  >
                    <tr>
                        <td class="auto-style6">First Name :</td>
                        <td class="auto-style7" >
                            <asp:TextBox ID="FName" onkeyup="javascript:fnClient()" runat="server" Width="50%"></asp:TextBox>
                            <asp:Label ID="fnClient" runat="server" ForeColor="#FF6600">*</asp:Label>
                        </td>
                        <td class="auto-style5" >

                            <asp:Label ID="fnamechecker" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style6">Last Name :</td>
                        <td class="auto-style7">
                            <asp:TextBox ID="LName" onkeyup="javascript:lnClient()" runat="server" Width="50%"></asp:TextBox>
                            <asp:Label ID="lnClient" runat="server" ForeColor="#FF6600">*</asp:Label>

                        </td>
                        <td class="auto-style5">

                            <asp:Label ID="lnchecker" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style6">Credit Card Info :</td>
                        <td class="auto-style7">
                            <asp:TextBox ID="cc" onkeyup="javascript:ccClient()" runat="server" Width="50%"></asp:TextBox>
                            <asp:Label ID="ccClient" runat="server" ForeColor="#FF6600">*</asp:Label>
                        </td>
                        <td class="auto-style5">
                            <asp:Label ID="ccchecker" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style6">Email Address :</td>
                        <td class="auto-style7">
                            <asp:TextBox ID="email" onkeyup="javascript:emailClient()" runat="server" Width="50%"></asp:TextBox>

                            <asp:Label ID="emailClient" runat="server" ForeColor="#FF6600">*</asp:Label>

                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="invalid email format" ControlToValidate="email" ForeColor="Blue" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>

                        </td>
                        <td class="auto-style5">

                            <asp:Label ID="emailchecker" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style6">Password :</td>
                        <td class="auto-style7">
                            <asp:TextBox ID="pwd" runat="server"  TextMode="Password" onkeyup="javascript:validatepwd()"   Width="50%"></asp:TextBox>
                            <asp:Label ID="pwdchecker" runat="server" ForeColor="#FF6600" >*</asp:Label>

                        </td >
                        <td class="auto-style5">

                            <asp:Label ID="passchecker" runat="server" ForeColor="Red"></asp:Label>
                        </td >
                    </tr>
                    <tr>
                        <td class="auto-style6">Confirm Password :</td>
                        <td class="auto-style7">
                            <asp:TextBox ID="cfmpwd" runat="server" TextMode="Password" onkeyup="javascript:matchpwd()"  Width="50%"></asp:TextBox>
                            <asp:Label ID="cfmpwdchecker" runat="server" ForeColor="#FF6600" >*</asp:Label>
                            <asp:CompareValidator ID="CompareValidator1" runat="server" ErrorMessage="Both password does not match" ControlToCompare="pwd" ControlToValidate="cfmpwd" ForeColor="Blue"></asp:CompareValidator>
                        </td >
                        <td class="auto-style5">
                            <asp:Label ID="cfmchecker" runat="server" ForeColor="Red"></asp:Label>
                        </td >
                    </tr>
                    <tr>
                        <td class="auto-style1">Date of Birth :</td>
                        <td class="auto-style1">
                            <%--                  <asp:Calendar ID="dob" runat="server" BackColor="White" BorderColor="Black" DayNameFormat="Shortest" Font-Names="Times New Roman" Font-Size="10pt" ForeColor="Black" Height="220px" NextPrevFormat="FullMonth" OnSelectionChanged="dob_SelectionChanged" TitleFormat="Month" Width="400px">
                                <DayHeaderStyle BackColor="#CCCCCC" Font-Bold="True" Font-Size="7pt" ForeColor="#333333" Height="10pt" />
                                <DayStyle Width="14%" />
                                <NextPrevStyle Font-Size="8pt" ForeColor="White" />
                                <OtherMonthDayStyle ForeColor="#999999" />
                                <SelectedDayStyle BackColor="#CC3333" ForeColor="White" />
                                <SelectorStyle BackColor="#CCCCCC" Font-Bold="True" Font-Names="Verdana" Font-Size="8pt" ForeColor="#333333" Width="1%" />
                                <TitleStyle BackColor="Black" Font-Bold="True" Font-Size="13pt" ForeColor="White" Height="14pt" />
                                <TodayDayStyle BackColor="#CCCC99" />
                            </asp:Calendar>--%>
                            <asp:TextBox ID="dob"  onmouseup="javascript:dobClient()" onmousedown="javascript:dobClient()" runat="server" TextMode="Date" Width="50%"></asp:TextBox>
                            <asp:Label ID="dobClient" runat="server" ForeColor="#FF6600" >*</asp:Label>
                        </td>
                        <td class="auto-style1">
                            <asp:Label ID="dobchecker" runat="server" ForeColor="Red" ></asp:Label>
                            </td>
                    </tr>
                    <tr>
                        <td class="auto-style6">Photo : </td>
                        <td class="auto-style7"><asp:Image ID="photo" runat="server" Height="30%" Width="30%"/>

                        </td>

                    </tr>  
                    <tr>
                        <td></td>
                        <td class="auto-style7">
                            <asp:FileUpload ID="FileUpload1" runat="server" />
                            <asp:Button ID="uploadbtn" runat="server" OnClick="uploadbtn_Click" Text="upload" />
                            <asp:Label ID="photochecker" runat="server" Text="&amp;nbsp" ForeColor="Red"></asp:Label>

                            <asp:Label ID="photocheker" runat="server" ForeColor="Red">*</asp:Label>

                        </td>
                        <td class="auto-style5">
                            &nbsp;</td>
                    </tr>


                    <tr>    

                        <td class="auto-style6"></td>
                        <td class="auto-style7">
                            &nbsp;</td>
                        <td class="auto-style5"></td>
                    </tr>


                    <tr>    

                        <td class="auto-style6">
                            <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Submit" Height="51px" Width="145px" />
                            <br />
                        </td>

                    </tr>

                </table>
            </fieldset>
        </div>
    </form>
</body>


</html>
