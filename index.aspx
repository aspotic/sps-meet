<% @ Page Language="VB" aspcompat="true"debug="true" CodeFile="index.aspx.vb" Inherits="index" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
	<head>
<!--Style Sheets-->
		<link rel="StyleSheet" href="style/fonts/<% Response.Write(Request.Cookies("FontTheme").Value) %>.css" type="text/css" />
		<link rel="StyleSheet" href="style/colors/<% Response.Write(Request.Cookies("ColorTheme").Value) %>.css" type="text/css" />
		<link rel="StyleSheet" href="style/layouts/<% Response.Write(Request.Cookies("LayoutTheme").Value) %>.css" type="text/css" />
<!--Meta Tags-->
		<meta http-equiv="Content-Type" content="text/html; charset=ISO-8859-1" />
		<title>SPS Meet</title>
	</head>
	<body>
	    <form id="Form1" method="post" runat="server">
<!--Header-->
		    <div id="pageTitle">
                <div id="pageTitleText">
                    <span>SPS Meet</span>
                </div>
                <div id="pageTitleImage">
                    <img src="style/images/SPS_Logo_Transparent.png" alt="SPS" />
                </div>
		    </div>
            <div id="navigation">
                <!--Show a link to standard view if they are currently in mobile view, and vice versa-->
                <% If Request.Cookies("LayoutTheme").Value = "Mobile" Then%>
                    <a href="index.aspx?mobile=off">Standard Site</a>
                <% else %>
                    <a href="index.aspx?mobile=on">Mobile Site</a>
                <% End If%>
                 | <a href="manual.htm">Guide</a>
                 | <a href="meetinglist.aspx">Existing Meetings</a>
            </div>
            <!--used for css control-->
            <div id="spacer">
            </div>
<!--Body-->
		    <div id="body">
                <div id="mainContainer">
                    <!--Only show layout theme changer if not in mobile mode since only 1 theme exists for mobile mode-->
                    <% If Not Request.Cookies("LayoutTheme").Value = "Mobile" Then%>
                        <!--selectTheme div contains a form for changing site aesthetics-->
			            <div id="selectTheme">
                            <ul>
						        <li class="title">Change Theme</li>
						        <!--<li>Layout</li>-->
						        <li>
                                    <asp:DropDownList ID="LayoutTheme" runat="server">
                                        <asp:ListItem>Vertical</asp:ListItem>
                                        <asp:ListItem>Box</asp:ListItem>
                                        <asp:ListItem>Flow</asp:ListItem>
                                    </asp:DropDownList>
                                </li>
						        <!--<li>Color Scheme</li>-->
						        <li>
                                    <asp:DropDownList ID="ColorTheme" runat="server">
                                        <asp:ListItem>SPSGreen</asp:ListItem>
                                        <asp:ListItem>Fire</asp:ListItem>
                                        <asp:ListItem>Cold</asp:ListItem>
                                        <asp:ListItem>Colorless</asp:ListItem>
                                    </asp:DropDownList>
                                </li>
						        <!--<li>Font</li>-->
						        <li>
                                    <asp:DropDownList ID="FontTheme" runat="server">
                                        <asp:ListItem>ComicSans</asp:ListItem>
                                        <asp:ListItem>Tahoma</asp:ListItem>
                                        <asp:ListItem>MailRays</asp:ListItem>
                                        <asp:ListItem>SketchRockwell</asp:ListItem>
                                        <asp:ListItem>BirthOfAHero</asp:ListItem>
                                        <asp:ListItem>Rumpelstiltskin</asp:ListItem>
                                        <asp:ListItem>KatyBerry</asp:ListItem>
                                    </asp:DropDownList>
                                </li>
						        <li>
                                    <asp:DropDownList ID="Stream" runat="server">
                                        <asp:ListItem>Stream</asp:ListItem>
                                        <asp:ListItem>Hide</asp:ListItem>
                                    </asp:DropDownList>
                                </li>
						        <li><asp:Button class="submit" OnClick="changeTheme" Text="Submit" runat="server" /></li>
					        </ul>
                        </div>
                    <% End If%>
                    <!--createMeeting div contains a form for creating a new open meeting-->
			        <div  id="createMeeting">
					    <ul>
						    <li class="title">Create Meeting</li>
						    <li>Meeting Name</li>
						    <li><asp:TextBox ID="meetingName" runat="server" AutoPostBack="True" OnTextChanged="checkName" ></asp:TextBox></li>
						    <li>Closing Date</li>
                            <li>
                                <asp:DropDownList ID="ampm" runat="server" AutoPostBack="True" OnSelectedIndexChanged="updateDate">
                                        <asp:ListItem>am</asp:ListItem>
                                        <asp:ListItem>pm</asp:ListItem>
                                </asp:DropDownList>
                                <asp:DropDownList ID="minute" runat="server" AutoPostBack="True" OnSelectedIndexChanged="updateDate">
                                        <asp:ListItem>00</asp:ListItem>
                                        <asp:ListItem>05</asp:ListItem>
                                        <asp:ListItem>10</asp:ListItem>
                                        <asp:ListItem>15</asp:ListItem>
                                        <asp:ListItem>20</asp:ListItem>
                                        <asp:ListItem>25</asp:ListItem>
                                        <asp:ListItem>30</asp:ListItem>
                                        <asp:ListItem>35</asp:ListItem>
                                        <asp:ListItem>40</asp:ListItem>
                                        <asp:ListItem>45</asp:ListItem>
                                        <asp:ListItem>50</asp:ListItem>
                                        <asp:ListItem>55</asp:ListItem>
                                </asp:DropDownList>
                                <asp:DropDownList ID="hour" runat="server" AutoPostBack="True" OnSelectedIndexChanged="updateDate">
                                        <asp:ListItem>1</asp:ListItem>
                                        <asp:ListItem>2</asp:ListItem>
                                        <asp:ListItem>3</asp:ListItem>
                                        <asp:ListItem>4</asp:ListItem>
                                        <asp:ListItem>5</asp:ListItem>
                                        <asp:ListItem>6</asp:ListItem>
                                        <asp:ListItem>7</asp:ListItem>
                                        <asp:ListItem>8</asp:ListItem>
                                        <asp:ListItem>9</asp:ListItem>
                                        <asp:ListItem>10</asp:ListItem>
                                        <asp:ListItem>11</asp:ListItem>
                                        <asp:ListItem>12</asp:ListItem>
                                </asp:DropDownList>
                                <div id="cal1">
			                        <asp:Calendar ID="Calendar1" runat="server" DayNameFormat="Shortest" OnSelectionChanged="updateDate">
                                        <DayHeaderStyle />
                                        <NextPrevStyle />
                                        <OtherMonthDayStyle />
                                        <SelectedDayStyle />
                                        <SelectorStyle />
                                        <TitleStyle BackColor="#EBF7E1" />
                                        <TodayDayStyle />
                                        <WeekendDayStyle />
                                    </asp:Calendar>
                                </div>
                                <asp:HiddenField ID="curDate" runat="server"/>
                            </li>
						    <li><asp:Button id="openMeeting" class="submit" OnClick="submit" Text="Submit" runat="server" /></li>
					    </ul>
			        </div>
                    <!--joinMeeting div contains a form that will take you to a requested meeting-->
			        <div  id="joinMeeting">
                        <ul>
						    <li class="title">Join a Meeting</li>
						    <li>Meeting Name</li>
						    <li><asp:TextBox ID="meetingToJoin" runat="server"></asp:TextBox></li>
						    <li><asp:Button class="submit" ID="Button1" OnClick="joinMeeting" Text="Submit" runat="server" /></li>
					    </ul>
                    </div>
		        </div>
            </div>
	    </form>
	</body>
</html>