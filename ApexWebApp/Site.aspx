<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Site.aspx.cs" Inherits="jabapp.Site" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
        <asp:Calendar ID="CalDateSelector" runat="server" Height="244px" Width="532px" OnSelectionChanged="CalDateSelector_SelectionChanged"></asp:Calendar>
        <p>
            Start Date:</p>
        <p>
            <asp:TextBox ID="TextBoxStartDate" runat="server"></asp:TextBox>
        </p>
        <p>
            End Date:</p>
        <asp:TextBox ID="TextBoxEndDate" runat="server"></asp:TextBox>
        <p>
            &nbsp;</p>
    </form>
</body>
</html>
