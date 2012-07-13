<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>RUsuarios</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        
        <asp:Chart ID="Chart1" runat="server" Height="300" Width="400">
            <series>
                <asp:Series Name="Series1" BorderColor="180, 26, 59, 105">
                    <Points>
                        <asp:DataPoint AxisLabel="Celtics" YValues="45" />
                        <asp:DataPoint AxisLabel="Celtics" YValues="34" />
                        <asp:DataPoint AxisLabel="Celtics" YValues="67" />
                    </Points>
                </asp:Series>
            </series>
            <chartareas>
                <asp:ChartArea Name="ChartArea1">
                </asp:ChartArea>
            </chartareas>
        </asp:Chart>
        
    </div>
    </form>
</body>
</html>
