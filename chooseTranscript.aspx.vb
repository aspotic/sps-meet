Public Class chooseTranscript
    Inherits common

    'Description: runs anything that needs to be done before the page loads
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Function will redirect to index if the username does not match the meeting owner name
        verifyMeetingOwner("meetinglist.aspx", escape(Request.QueryString.Item("eName")))

        'If no theme is chosen, then set the default theme
        setDefaultTheme()
    End Sub
End Class