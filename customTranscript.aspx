<% @ Page Language="VB" aspcompat="true" CodeFile="customTranscript.aspx.vb" Inherits="customTranscript" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
	<head>
<!--Style Sheets-->
        <style type="text/css">
            img
            {
                margin: 10px auto;
            }
            table
            {
                border-collapse: collapse;
                border: 2px solid #000000;
                width: 95%;
                margin: 20px auto
            }
            tr,td,th
            {
                border: 2px solid #000000;
                vertical-align: text-top;
                text-align: left;
	            word-wrap: break-word;
            }
            th
            {
                border: 2px solid #000000;
                vertical-align: text-top;
                text-align: center;
            }
            .name
            {
                width: 25%;
            }
            .date
            {
                width: 20%;
            }
        </style>
<!--Meta Tags-->
		<meta http-equiv="Content-Type" content="text/html; charset=ISO-8859-1" />
		<title>Meeting Rooms: Closed Meeting Transcript</title>
	</head>
	<body>
        <center>
<!--Header-->
            <img src="style/images/SPS_Logo_Transparent.png" alt="SPS" />
<!--Body-->
    <!--Get the list messages from either the msg or msg_history table-->
                <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:spsmeetConnectionString %>" >
                    <SelectParameters>
                        <asp:Parameter Name="eName" type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>

                <asp:ListView ID="ListView1" runat="server" DataSourceID="SqlDataSource1">
                    <EmptyDataTemplate>
                    </EmptyDataTemplate>

                    <LayoutTemplate>
                        <table id="Table1" class="tableList" runat="server">
                            <tr>
							    <th colspan="4" class="tableTitle">
								    "<% Response.Write(Request.Form.Item("eName"))%>" Transcript
							    </th>
						    </tr>
                            <tr>
							    <th class="name" runat="server">
								    Username
							    </th>
							    <th class="date" runat="server">
								    Date
							    </th>
							    <th class="message" runat="server">
								    Message
							    </th>
						    </tr>
                            <tr>
                                <td id="ItemPlaceHolder" runat="server">
                                </td>
                            </tr>
                        </table>
                    </LayoutTemplate>

                    <ItemTemplate>
                        <tr>
						    <td class="name" runat="server">
                                    <!--user getuser() function to display the username if it was selected for display on the previous page-->
							        <asp:Label ID="Label3" runat="server" Text='<%# GetUser(DataBinder.Eval(Container. DataItem,"name")) %>' />
                            </td>
						    <td class="date" runat="server">
							    <asp:Label ID="Label4" runat="server" Text='<%# Eval("mDate")  %>' />
						    </td>
						    <td class="message" runat="server">
							    <asp:Label ID="Label5" runat="server" Text='<%# Eval("mText") %>' />
						    </td>
					    </tr>
                    </ItemTemplate>
                </asp:ListView>
		</center>
	</body>
</html>