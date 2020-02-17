<% @ Page Language="VB" aspcompat="true" CodeFile="chooseTranscript.aspx.vb" Inherits="chooseTranscript" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
	<head>
<!--Style Sheets-->
		<link rel="StyleSheet" href="style/fonts/<% Response.Write(Request.Cookies("FontTheme").Value) %>.css" type="text/css" />
		<link rel="StyleSheet" href="style/colors/<% Response.Write(Request.Cookies("ColorTheme").Value) %>.css" type="text/css" />
		<link rel="StyleSheet" href="style/layouts/<% Response.Write(Request.Cookies("LayoutTheme").Value) %>.css" type="text/css" />
<!--Meta Tags-->
		<meta http-equiv="Content-Type" content="text/html; charset=ISO-8859-1" />
		<title>Meeting Rooms: Choose Names Visible on Transcript</title>
	</head>
	<body>
<!--Header-->
		<div id="pageTitle">
            <div id="pageTitleText">
                <span>SPS Meet</span>
            </div>
            <div id="pageTitleImage">
                <img src="style/images/SPS_Logo_Transparent.png" alt="" />
            </div>
		</div>
		<div id="navigation">
			<span class='block'><a href="index.aspx">Home</a></span>
			<span class='blockDiv'> > </span>
            <span class='block'><a href="meetinglist.aspx">Meeting List</a></span>
            <span class='blockDiv'> > </span>
            <span class='block'>Choose Names Visible on Transcript</span>
		</div>
        <div id="spacer">
        </div>
<!--Body-->
		<div id="body">
    <!-- display usernames in the given meeting. look in both open and closed meetings  -->
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                ConnectionString="<%$ ConnectionStrings:spsmeetConnectionString %>" 
                SelectCommand="SELECT DISTINCT name FROM msg WHERE eName = @eName UNION SELECT DISTINCT name FROM msg_history WHERE eName = @eName">
                <SelectParameters>
                    <asp:QueryStringParameter Name="eName" QueryStringField="eName" type="String" />
                </SelectParameters>
            </asp:SqlDataSource>

            <asp:ListView ID="transcriptName" runat="server" DataSourceID="SqlDataSource1">
                <EmptyDataTemplate>
                </EmptyDataTemplate>

                <LayoutTemplate>
                    <form action="customTranscript.aspx" method="post">
                        <table class="tableList" runat="server">
                            <tr>
						        <th colspan="2" class="tableTitle">
								    Names Displayed on Transcript
						        </th>
					        </tr>
                            <tr>
						        <th class="transCol1" runat="server">
							        Show
						        </th>
						        <th class="transCol2" runat="server">
							        Username
						        </th>
					        </tr>
                            <tr>
                                <td id="ItemPlaceHolder" runat="server">
                                </td>
                            </tr>
                        </table>

                        <table class="tableList" >
                            <tr>
						        <th colspan="2" class="tableTitle">
								    Message Types Displayed on Transcript
						        </th>
                            </tr>
                            <tr>
						        <td id="Td1" class="transCol1" runat="server">
                                    <input type="checkbox" class="checkbox" name="showVisible">
                                </td>
						        <td id="Td2" class="transCol2" runat="server">
                                    Regular Messages
                                </td>
                            </tr>
                            <tr>
						        <td id="Td3" class="transCol1" runat="server">
                                    <input type="checkbox" class="checkbox" name="showHidden">
                                </td>
						        <td id="Td4" class="transCol2" runat="server">
                                    Hidden Messages
                                </td>
                            </tr>
                           </table>

                           <div class="submitFoot"><input type="submit" name="submit" value="View Transcript"/></div>
                           <div class="submitFoot"><input type="hidden" name="eName" value="<% Response.write(Request.QueryString("eName")) %>" /></div>
                    </form>
                </LayoutTemplate>

                <ItemTemplate>
                    <tr>
						<td class="transCol1" runat="server">
                            <input type="checkbox" class="checkbox" name="<%# Eval("name") %>" /> 
                        </td>
						<td class="transCol2" runat="server">
							<asp:Label ID="Label2" runat="server" Text='<%# Eval("name") %>' />
						</td>
					</tr>
                </ItemTemplate>
            </asp:ListView>
		</div>
	</body>
</html>