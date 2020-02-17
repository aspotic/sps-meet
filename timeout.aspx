<% @ Page Language="VB" aspcompat="true" Inherits="common" %>

You have been inactive for too long.<br />
<a href="meeting.aspx?eName=<% Response.write(escape(Page.Request.QueryString("eName"))) %>">Return to the meeting</a>