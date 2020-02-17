Public Class meetinglist
    Inherits common

    'Description: runs anything that needs to be done before the page loads
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'If no theme is chosen, then set the default theme
        setDefaultTheme()

        'move old table entries to proper locations
        updateTableHistory()

        'Set the username used for checking the user's activity in open and owned meetings
        SqlDataSource1.SelectParameters("uDomain").DefaultValue = User.Identity.Name.Split("\")(0)
        SqlDataSource1.SelectParameters("uName").DefaultValue = User.Identity.Name.Split("\")(1)

        'Set the username used for checking the user's activity in closed and owned meetings
        SqlDataSource2.SelectParameters("uDomain").DefaultValue = User.Identity.Name.Split("\")(0)
        SqlDataSource2.SelectParameters("uName").DefaultValue = User.Identity.Name.Split("\")(1)

        'Set the username used for checking the user's activity in open and unowned meetings
        SqlDataSource3.SelectParameters("uDomain").DefaultValue = User.Identity.Name.Split("\")(0)
        SqlDataSource3.SelectParameters("uName").DefaultValue = User.Identity.Name.Split("\")(1)

    End Sub

End Class