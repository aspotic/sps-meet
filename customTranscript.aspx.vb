Public Class customTranscript
    Inherits common

    'Description: runs anything that needs to be done before the page loads
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Function will redirect to index if the username does not match the meeting owner name
        verifyMeetingOwner("meetinglist.aspx", Request.Form.Item("eName"))

        'store the ename in a parameter for use in the customTranscript.aspx page
        SqlDataSource1.SelectParameters("eName").DefaultValue = Request.Form.Item("eName")

        'Set the select query to use. depends on whether visible or hidden messages are to be shown
        If Request.Form.Item("showVisible") = "" Then
            If Request.Form.Item("showHidden") = "" Then
                SqlDataSource1.SelectCommand = "SELECT * FROM [msg] WHERE [eName] = @eName AND [final] > 3 UNION SELECT * FROM [msg_history] WHERE [eName] = @eName AND [final] > 3 ORDER BY [mID] ASC"
            Else
                SqlDataSource1.SelectCommand = "SELECT * FROM [msg] WHERE [eName] = @eName AND [final] = 2 UNION SELECT * FROM [msg_history] WHERE [eName] = @eName AND [final] = 2 ORDER BY [mID] ASC"
            End If
        Else
            If Request.Form.Item("showHidden") = "" Then
                SqlDataSource1.SelectCommand = "SELECT * FROM [msg] WHERE [eName] = @eName AND [final] = 1 UNION SELECT * FROM [msg_history] WHERE [eName] = @eName AND [final] = 1 ORDER BY [mID] ASC"
            Else
                SqlDataSource1.SelectCommand = "SELECT * FROM [msg] WHERE [eName] = @eName AND [final] > 0 UNION SELECT * FROM [msg_history] WHERE [eName] = @eName AND [final] > 0 ORDER BY [mID] ASC"
            End If
        End If
    End Sub

    'Return the username if it was in the list of usernames to display (selected on the chooseTranscript page)
    Function GetUser(name As String) As String
        If Request.Form.Item(name) <> "" Then
            Return name
        Else
            Return ""
        End If
    End Function
End Class