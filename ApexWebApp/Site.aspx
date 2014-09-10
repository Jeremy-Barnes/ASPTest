<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Site.aspx.cs" Inherits="jabapp.Site" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        Please Select Start and End dates for your report:<br />
        <br />
    
    </div>
        <asp:Calendar ID="CalDateSelector" runat="server" Height="244px" Width="532px" OnSelectionChanged="CalDateSelector_SelectionChanged"></asp:Calendar>
        <p>
            Start Date:</p>
        <p>
            <asp:TextBox ID="TextBoxStartDate" runat="server" Enabled="False"></asp:TextBox>
        </p>
        <p>
            End Date:</p>
        <asp:TextBox ID="TextBoxEndDate" runat="server" Enabled="False"></asp:TextBox>
        <p>
            <asp:Button ID="ButtonSubmit" runat="server" OnClick="ButtonSubmit_Click" Text="Submit" />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="ButtonExport" runat="server" OnClick="ButtonExport_Click" Text="Export" />
        </p>
        <p>
            &nbsp;</p>
    </form>
</body>
</html>
