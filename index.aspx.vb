Imports System.Data.SqlClient

'TODO
'Create some video tutorials


Public Class index
    Inherits common

    'Description: runs anything that needs to be done before the page loads
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'If no theme is chosen, then set the default theme
        setDefaultTheme()

        'if the user has turned mobile mode on or off, then update all theme cookies
        If Request.QueryString("mobile") = "on" Then
            Response.Cookies("ColorTheme").Value = "Mobile"
            Response.Cookies("LayoutTheme").Value = "Mobile"
            Response.Cookies("FontTheme").Value = "Mobile"
            Response.Cookies("Stream").Value = "Hide"
            Response.Redirect("index.aspx")
        ElseIf Request.QueryString("mobile") = "off" Then
            Response.Cookies("ColorTheme").Value = "SPSGreen"
            Response.Cookies("LayoutTheme").Value = "Box"
            Response.Cookies("FontTheme").Value = "SketchRockwell"
            Response.Cookies("Stream").Value = "Hide"
            Response.Redirect("index.aspx")
        End If

        'default to the selected values in the theme drop down menus if the drop down exists (will not exist in mobile mode)
        If Not IsPostBack And Not Request.Cookies("LayoutTheme").Value = "Mobile" Then
            ColorTheme.Items.FindByText(Request.Cookies("ColorTheme").Value).Selected = True
            LayoutTheme.Items.FindByText(Request.Cookies("LayoutTheme").Value).Selected = True
            FontTheme.Items.FindByText(Request.Cookies("FontTheme").Value).Selected = True
            Stream.Items.FindByText(Request.Cookies("Stream").Value).Selected = True
        End If

        'move old table entries to proper locations
        updateTableHistory()

    End Sub

    'Description: Updates the theme cookies to contain the new values set by the user
    Sub changeTheme()
        Response.Cookies("ColorTheme").Value = ColorTheme.SelectedValue

        'Don't allow flow layout if the user is using anything older than ie9
        If LayoutTheme.SelectedValue.ToString().Equals("Flow") Then
            If Request.Browser.Browser.Equals("IE") Then
                If Request.Browser.MajorVersion < 9 Then
                    Response.Cookies("LayoutTheme").Value = "Box"
                Else
                    Response.Cookies("LayoutTheme").Value = "Flow"
                End If
            End If
        Else
            Response.Cookies("LayoutTheme").Value = LayoutTheme.SelectedValue
        End If

        Response.Cookies("FontTheme").Value = FontTheme.SelectedValue
        Response.Cookies("Stream").Value = Stream.SelectedValue
        'take the user back to the home page after updating the theme
        Response.Redirect("index.aspx")
    End Sub

    'Description: creates a meeting
    Sub submit(sender As Object, e As EventArgs)
        'make sure the title only contains allowed characters and does not already exist
        If Not checkName() Then
            Exit Sub
        End If

        'PUT THE MEETING IN THE DATABASE
        'Select DB
        Dim conn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("spsmeetConnectionString").ConnectionString.ToString())
        Dim query As New SqlCommand()

        'Set the DB Query
        query.CommandText = "INSERT INTO meet (eName, dateCreate, dateExpire, uDomain, uName) VALUES ('" & _
                            escapespace(Request.Form.Item("meetingName")) & "', '" & _
                            Format(Now, "yyyy-MM-dd hh:mm:ss tt").ToString() & "', '" & _
                            Request.Form.Item("curDate") & "', '" & _
                            User.Identity.Name.Split("\")(0) & "', '" & User.Identity.Name.Split("\")(1) & "')"
        query.Connection = conn

        'Run SQL Query: Create the meeting in the database   
        Try
            conn.Open()
            query.ExecuteNonQuery()
        Finally
            conn.Close()
        End Try

        'Take the user to the new meeting
        Response.Redirect("meeting.aspx?eName=" & escapespace(Request.Form.Item("meetingName")))
    End Sub

    'Description: Take the user to the requested meeting
    Sub joinMeeting(sender As Object, e As EventArgs)
        Response.Redirect("meeting.aspx?eName=" & escapespace(Request.Form.Item("meetingToJoin")))
    End Sub

    'Description: Updates the date value in the curDate text box to match what was clicked on in the calendar
    Sub updateDate(sender As Object, e As System.EventArgs)
        curDate.Value = Format(Calendar1.SelectedDate, "yyyy-MM-dd").ToString() & " " & hour.Text & ":" & minute.Text & " " & ampm.Text
    End Sub

    'Description: Make sure the the meeting name string does not contain any invalid characters, including existing meeting names
    Function checkName() As Boolean
        Dim intCounter As Integer
        Dim strCompare As String
        Dim strInput As String
        checkName = True

        'Make sure the name field wasn't left blank
        If Len(meetingName.Text).Equals(0) Then
            checkName = False
        End If

        'Make sure the name field contains only alphanumeric characters
        If checkName Then
            For intCounter = 1 To Len(meetingName.Text)
                strCompare = Mid$(meetingName.Text, intCounter, 1)
                strInput = Mid$(meetingName.Text, intCounter + 1, Len(meetingName.Text))
                If strCompare Like ("[A-Z]") Or _
                    strCompare Like ("[a-z]") Or _
                    strCompare Like ("#") Then
                    checkName = True
                Else
                    checkName = False
                End If
            Next intCounter
        End If


        'Select DB
        Dim conn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("spsmeetConnectionString").ConnectionString.ToString())

        'Make sure the name doesn't exist in the meet table
        If checkName Then
            'check if an entry already exist with this name
            Dim query As String = "SELECT TOP 1 eName FROM meet WHERE eName='" & meetingName.Text & "'"

            'Create Command object
            Dim thisCommand As New SqlCommand(query, conn)

            Try
                'Open Connection
                conn.Open()
                'Execute Query
                Dim thisReader As SqlDataReader = thisCommand.ExecuteReader()
                If thisReader.Read() Then
                    checkName = False
                End If
            Finally
                'Close Connection
                conn.Close()
            End Try
        End If


        'Make sure the name doesn't exist in the meet_history table
        If checkName Then
            'check if an entry already exist with this name
            Dim query As String = "SELECT TOP 1 eName FROM meet_history WHERE eName='" & meetingName.Text & "'"

            'Create Command object
            Dim thisCommand As New SqlCommand _
               (query, conn)

            Try
                'Open Connection
                conn.Open()
                'Execute Query
                Dim thisReader As SqlDataReader = thisCommand.ExecuteReader()
                If thisReader.Read() Then
                    checkName = False
                End If
            Finally
                'Close Connection
                conn.Close()
            End Try
        End If

        'Change the background color of the meeting name text box if the meeting is invalid(red)/valid(normal color)
        If Not checkName Then
            meetingName.BackColor = System.Drawing.Color.FromArgb(240, 70, 70)
        Else
            meetingName.BackColor = System.Drawing.Color.Empty
        End If

        Return checkName
    End Function

End Class