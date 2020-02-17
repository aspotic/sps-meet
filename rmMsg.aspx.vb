Imports System.Data.SqlClient

Public Class rmMsg
    Inherits common

    'Description: do authentication on page load
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'authenticate that this is the owner of the requested meeting running the script
        If Session("owner") = User.Identity.Name Then

            'change the final value to a 2 so that the message is not displayed, but the data is still kept for record keeping purposes
            'Select DB
            Dim conn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("spsmeetConnectionString").ConnectionString.ToString())

            'Create Query
            Dim query As New SqlCommand()

            'Set the DB Query: update an existing message to be hidden
            query.CommandText = "UPDATE msg SET final='2' WHERE mID = '" & escape(Page.Request.QueryString("mID")) & "'"
            query.Connection = conn

            Try
                'Open Connection
                conn.Open()

                'run
                query.ExecuteNonQuery()
            Finally
                conn.Close()
            End Try
        End If

        'go back to the meeting
        Response.Redirect("meeting.aspx?eName=" & Page.Request.QueryString("eName"))
    End Sub
End Class