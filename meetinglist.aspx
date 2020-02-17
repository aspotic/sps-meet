<% @ Page Language="VB" aspcompat="true" CodeFile="meetinglist.aspx.vb" Inherits="meetinglist" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
	<head>
<!--Stylesheets-->
		<link rel="StyleSheet" href="style/fonts/<% Response.Write(Request.Cookies("FontTheme").Value) %>.css" type="text/css" />
		<link rel="StyleSheet" href="style/colors/<% Response.Write(Request.Cookies("ColorTheme").Value) %>.css" type="text/css" />
		<link rel="StyleSheet" href="style/layouts/<% Response.Write(Request.Cookies("LayoutTheme").Value) %>.css" type="text/css" />
<!--Meta Tags-->
		<meta http-equiv="Content-Type" content="text/html; charset=ISO-8859-1" />
		<title>Meeting Rooms: Meeting List</title>
	</head>
	<body>
<!--Header-->
		<div id="pageTitle">
            <div id="pageTitleText">
                <span>SPS Meet</span>
            </div>
            <div id="pageTitleImage">
                <img src="style/images/SPS_Logo_Transparent.png" />
            </div>
		</div>
		<div id="navigation">
			<span class='block'><a href="index.aspx">Home</a></span>
            <span class='blockDiv'> > </span>
            <span class='block'>Meeting List</span>
		</div>
<!--Spacer (for css control)-->
        <div id="spacer">
        </div>
<!--Body-->
		<div id="body">
    <!--Open/Unowned-->
		    <div class="meetingList">
                <!--Display any open and unowned meetings the user become a member of-->
                <asp:SqlDataSource ID="SqlDataSource3" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:spsmeetConnectionString %>" 
                    SelectCommand="SELECT meet.uName AS muName, meet.uDomain AS muDomain,* FROM [members] INNER JOIN [meet] ON members.eName=meet.eName WHERE ((members.uDomain = @uDomain AND members.uName = @uName) AND NOT (meet.uName = @uName AND meet.uDomain = @uDomain)) ORDER BY members.lastPoll DESC">
                    <SelectParameters>
                        <asp:Parameter Name="uName" />
                        <asp:Parameter Name="uDomain" />
                    </SelectParameters>
                </asp:SqlDataSource>

                <asp:ListView ID="ListView3" runat="server" DataSourceID="SqlDataSource3">
                    <EmptyDataTemplate>
						<div class="tableTitle">
							Joined Meetings
						</div>
                        <table class="tableList" runat="server">
                            <tr>
                                <td>Not a member of any open meetings</td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>

                    <LayoutTemplate>
						<div class="tableTitle">
							Joined Meetings
						</div>
                        <table class="tableList" runat="server">
                            <tr>
							    <th class="col1j" runat="server">
								    Name
							    </th>
							    <th class="col2j" runat="server">
								    Start Time
							    </th>
							    <th class="col3j" runat="server">
								    End Time
							    </th>
							    <th class="col4j" runat="server">
								    Creator
							    </th>
						    </tr>
                            <tr id="ItemPlaceHolder" runat="server"></tr>
                        </table>
                    </LayoutTemplate>

                    <ItemTemplate>
                        <tr>
						    <td class="col1j" runat="server">
							    <a href="meeting.aspx?eName=<%# Eval("eName") %>"><asp:Label runat="server" Text='<%# Eval("eName") %>' /></a>
						    </td>
						    <td class="col2j" runat="server">
							    <asp:Label ID="Label3" runat="server" Text='<%# Eval("dateCreate") %>' />
						    </td>
						    <td class="col3j" runat="server">
							    <asp:Label ID="Label4" runat="server" Text='<%# Eval("dateExpire") %>' />
						    </td>
						    <td class="col4j" runat="server">
							    <asp:Label ID="Label5" runat="server" Text='<%# Eval("muDomain") %>' />\<asp:Label runat="server" Text='<%# Eval("muName") %>' />
						    </td>
					    </tr>
                    </ItemTemplate>
                </asp:ListView>
            </div>
    <!--Open/Owned-->
		    <div class="meetingList">
                <!--Display any open meetings belonging to the user-->
                <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:spsmeetConnectionString %>" 
                    SelectCommand="SELECT * FROM [meet] WHERE ([uDomain] = @uDomain AND [uName] = @uName) ORDER BY dateExpire DESC">
                    <SelectParameters>
                        <asp:Parameter Name="uName" />
                        <asp:Parameter Name="uDomain" />
                    </SelectParameters>
                </asp:SqlDataSource>

                <asp:ListView ID="ListView1" runat="server" DataSourceID="SqlDataSource1">
                    <EmptyDataTemplate>
						<div class="tableTitle">
							My Open Meetings
						</div>
                        <table class="tableList" runat="server">
                            <tr>
                                <td>No open meetings found</td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>

                    <LayoutTemplate>
						<div class="tableTitle">
							My Open Meetings
						</div>
                        <table id="Table2" class="tableList" runat="server">
                            <tr>
							    <th class="col1" runat="server">
								    Name
							    </th>
							    <th class="col2" runat="server">
								    Start Time
							    </th>
							    <th class="col3" runat="server">
								    End Time
							    </th>
						    </tr>
                            <tr id="ItemPlaceHolder" runat="server"></tr>
                        </table>
                    </LayoutTemplate>

                    <ItemTemplate>
                        <tr>
						    <td class="col1" runat="server">
							    (<a href="chooseTranscript.aspx?eName=<%# Eval("eName") %>">Trans.</a>)
							    <a href="meeting.aspx?eName=<%# Eval("eName") %>"><asp:Label runat="server" Text='<%# Eval("eName") %>' /></a>
						    </td>
						    <td class="col2" runat="server">
							    <asp:Label runat="server" Text='<%# Eval("dateCreate") %>' />
						    </td>
						    <td class="col3" runat="server">
							    <asp:Label runat="server" Text='<%# Eval("dateExpire") %>' />
						    </td>
					    </tr>
                    </ItemTemplate>
                </asp:ListView>
            </div>
    <!--Closed/Owned-->
		    <div class="meetingList">
                <!--Display any closed meetings belonging to the user-->
                <asp:SqlDataSource ID="SqlDataSource2" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:spsmeetConnectionString %>" 
                    SelectCommand="SELECT * FROM [meet_history] WHERE ([uDomain] = @uDomain AND [uName] = @uName) ORDER BY dateExpire DESC">
                    <SelectParameters>
                        <asp:Parameter Name="uName" />
                        <asp:Parameter Name="uDomain" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:ListView ID="ListView2" runat="server" DataSourceID="SqlDataSource2">

                    <EmptyDataTemplate>
						<div class="tableTitle">
							My closed Meetings
						</div>
                        <table class="tableList" runat="server">
                            <tr>
                                <td>No closed meetings found</td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>

                    <LayoutTemplate>
						<div class="tableTitle">
							My closed Meetings
						</div>
                        <table class="tableList" runat="server">
                            <tr>
							    <th class="col1" runat="server">
								    Name
							    </th>
							    <th class="col2" runat="server">
								    Start Time
							    </th>
							    <th class="col3" runat="server">
								    End Time
							    </th>
						    </tr>
                            <tr id="ItemPlaceHolder" runat="server"></tr>
                        </table>
                    </LayoutTemplate>

                    <ItemTemplate>
                        <tr>
						    <td class="col1" runat="server">
							    <a href="chooseTranscript.aspx?eName=<%# Eval("eName") %>"><asp:Label runat="server" Text='<%# Eval("eName") %>' /></a>
						    </td>
						    <td class="col2" runat="server">
							    <asp:Label runat="server" Text='<%# Eval("dateCreate") %>' />
						    </td>
						    <td class="col3" runat="server">
							    <asp:Label runat="server" Text='<%# Eval("dateExpire") %>' />
						    </td>
					    </tr>
                    </ItemTemplate>
                </asp:ListView>
		    </div>
        </div>
	</body>
</html>