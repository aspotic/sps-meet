<% @ Page Language="VB" aspcompat="true" ValidateRequest="false" debug="true" CodeFile="meeting.aspx.vb" Inherits="meeting" %>           
<%@ Import Namespace="System.Data.SqlClient" %>                        
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
	<head>
<!--Stylesheets-->
		<link rel="StyleSheet" href="style/layouts/<% Response.Write(Request.Cookies("LayoutTheme").Value) %>.css" type="text/css" />
		<link rel="StyleSheet" href="style/colors/<% Response.Write(Request.Cookies("ColorTheme").Value) %>.css" type="text/css" />
		<link rel="StyleSheet" href="style/fonts/<% Response.Write(Request.Cookies("FontTheme").Value) %>.css" type="text/css" />
<!--Meta Tags-->
		<meta http-equiv="Content-Type" content="text/html; charset=ISO-8859-1" />
        <meta http-equiv="X-UA-Compatible" content="IE=EmulateIE9">
		<title>SPS Meet: In Meeting</title>

	</head>
	<body>
        <form id="form1" runat="server" style="height:100%">
<!--Header-->
		    <div id="pageTitle">
                <div id="pageTitleText">
                    <span>SPS Meet</span>
                </div>
                <div id="pageTitleImage">
                    <img src="style/images/SPS_Logo_Transparent.png" alt="Saskatoon Public Schools" />
                </div>
		    </div>
		    <div id="navigation">
                <% populateNav() %>
		    </div>
<!--Spacer (for css control)-->
            <div id="spacer">
            </div>
<!--Body-->
		    <div id="bodyMeet">
                <asp:ScriptManager ID="ScriptManager" EnablePartialRendering="true" runat="server" />
                <script type="text/javascript">
                    var prm = Sys.WebForms.PageRequestManager.getInstance();

                    prm.add_beginRequest(beginRequest);

                    function beginRequest() {
                        prm._scrollPosition = null;
                    }
                   </script>
                <asp:Timer ID="Timer" Enabled="true" runat="server" Interval="1000" />
    <!--Member List Dividers-->
				<div id="memberBoxHead">Members</div>
			    <div id="memberBox">
			        <div id="memberBoxLegend">
				        <ul>
					        <li>X - Present</li>
					        <li>O - Absent</li>
				        </ul>
			        </div>
                    <div>
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="Timer" EventName="Tick" />
                            </Triggers>
                            <ContentTemplate>
                                <asp:Literal ID="memberBoxData" runat="server"></asp:Literal>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
			    </div>
                
    <!--Reply Dividers-->
				<div id="replyBoxHead">Write Message</div>
			    <div id="replyBox">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:TextBox ID="replyBoxMsg" Rows="11" runat="server" TextMode="MultiLine"></asp:TextBox>
					        <asp:Button ID="replyBoxSendMsg" Text="Send" runat="server" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>

    <!--Conversation window Dividers-->
				<div id="convBoxHead">Conversation</div>
			    <div id="convBox">
                        <asp:UpdatePanel ID="ConversationPanel" runat="server" UpdateMode="Conditional">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="Timer" EventName="Tick" />
                            </Triggers>
                            <ContentTemplate>
                                <asp:Literal ID="convBoxData" runat="server"></asp:Literal>
                            </ContentTemplate>
                        </asp:UpdatePanel>
			    </div>
            </div>
        </form>
	</body>
</html>