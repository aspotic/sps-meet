Imports System.Data.SqlClient
Imports System.DirectoryServices

Public Class meeting
    Inherits common
    Const INACTIVE_TIME As Integer = 10 ' time in seconds. The time for a member to be considered inactive. (no activity received on the server for x seconds, so inactive)
    Const REFRESH_RATE As Integer = 1 ' number of seconds between checking the server for new information. must also be changed in the timer within meeting.aspx (in ms)
    Const TIMEOUT As Integer = 3600 'number of seconds between when a user posts a message and when they should be considered inactive and sent to the timeout page


    'Description: runs anything that needs to be done before the page loads
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then
            Session("timeout") = 0


            'Load the user's first and last names into the FnameLname session variable
            Dim entry As DirectoryEntry = New DirectoryEntry("LDAP://DC=spsd,DC=sk,DC=ca")
            Dim Dsearch As DirectorySearcher = New DirectorySearcher(entry)
            Dsearch.SearchScope = SearchScope.Subtree
            'setup fitler to search the active directory for current user only
            Dsearch.Filter = "(&(objectClass=user)(sAMAccountName=" & Page.User.Identity.Name.Split("\")(1) & "))"
            'do the search, and place their FnameLname combo in FnameLname
            Dim resEnt As SearchResult = Dsearch.FindOne()
            Dim prop As Object
            For Each prop In resEnt.Properties("cn")
                Session("FnameLname") = [prop].ToString()
            Next prop




            'If no theme is chosen, then set the default theme
            setDefaultTheme()

            'Select DB
            Dim conn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("spsmeetConnectionString").ConnectionString.ToString())

            'Set DB Query: get meeting owner and meeting expiration date
            Dim query As String = "SELECT eName,dateExpire,uDomain,uName FROM meet WHERE eName = '" & escape(Page.Request.QueryString("eName")) & "'"

            'Create Command object
            Dim thisCommand As New SqlCommand(query, conn)

            Try
                'Open Connection
                conn.Open()
                'Execute Query
                Dim thisReader As SqlDataReader = thisCommand.ExecuteReader()
                If thisReader.Read() Then
                    Session("expireDate") = thisReader.GetValue(1)
                    Session("owner") = thisReader.GetValue(2) & "\" & thisReader.GetValue(3)
                    Session("meeting") = Page.Request.QueryString("eName")
                End If

                'Close Connection
                conn.Close()
            Catch
                'Close Connection
                conn.Close()
                Response.Redirect("index.aspx")
            End Try
        End If

        'TIMEOUT CHECK
        'Increment timeout counter
        Session("timeout") += REFRESH_RATE
        'If the timeout is reached, then redirect them to the timed out page
        If Session("timeout") > TIMEOUT Then
            Response.Redirect("timeout.aspx?eName=" & Session("meeting"))
        End If

        'refresh the meeting
        Timer_Tick()
    End Sub





    'Definition: Sets up the navigation bar contents, including meeting name closing date
    Sub populateNav()
        Response.Write("<span class='block'><a href='index.aspx'>Home</a></span><span class='blockDiv'> > </span> <span class='block'><a href='meetinglist.aspx'>Existing Meetings</a></span><span class='blockDiv'> > </span> <span class='block'>" & Session("meeting") & " </span><span class='block'>Closes on " & Format(Session("expireDate"), "dddd, MMM d yyyy hh:mm:ss tt") & "</span>")
    End Sub





    'Definition: Updates the member list, and conversation window to reflect any changes in the database
    Sub Timer_Tick()
        Dim streamOfConsciousness As Boolean
        Dim CD As String = ""
        'Dim query3 As String = ""
        Dim loggedUsr As String = Page.User.Identity.Name
        Dim dbUsr As String = ""

        'Enable or disable stream of consciousness based on cookie value
        If Request.Cookies("Stream").Value = "Stream" Then
            streamOfConsciousness = True
        Else
            streamOfConsciousness = False
        End If

        'If the meeting is now old, then show the transcript
        Dim expire As DateTime = Session("expireDate")
        If DateTime.Compare(expire, DateTime.Now) = -1 Then
            'move old table entries to proper locations
            updateTableHistory()

            'Time up, so kick the user from the meeting
            Response.Redirect("index.aspx")
        End If

        'CONVERSATION DATA GOES INTO ConversationData.text AS HTML
        'UPDATE MEMBER STATUS
        'Select DB
        Dim conn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("spsmeetConnectionString").ConnectionString.ToString())
        Dim query As New SqlCommand()

        'Set the DB Query: Update member status
        query.CommandText = "ClientMadeChatRequest" & _
                                " @uDomain='" & Page.User.Identity.Name.Split("\")(0) & _
                                "', @uName='" & Page.User.Identity.Name.Split("\")(1) & _
                                "', @eName='" & escape(Request.QueryString("eName")) & _
                                "', @name='" & Session("FnameLname") & "'"
        query.Connection = conn

        'Run SQL Query: Update member status
        Try
            conn.Open()
            query.ExecuteNonQuery()
        Finally
            conn.Close()
        End Try



        'update the db with the message currently being written by the user if they have streamOfConsciousness turned on
        If streamOfConsciousness Then
            'Select DB
            conn = New SqlConnection(ConfigurationManager.ConnectionStrings("spsmeetConnectionString").ConnectionString.ToString())

            'Select Query:
            Dim query2 As String = "SELECT * FROM msg WHERE eName = '" & escape(Page.Request.QueryString("eName")) & _
                                    "' AND uDomain = '" & Page.User.Identity.Name.Split("\")(0) & _
                                    "' AND uName = '" & Page.User.Identity.Name.Split("\")(1) & _
                                    "' AND final = '0' ORDER BY mID DESC"

            'Create Command object: get meeting owner and meeting expiration date
            Dim thisCommand As New SqlCommand(query2, conn)

            Try
                'Open Connection
                conn.Open()

                'Execute Query
                Dim thisReader As SqlDataReader = thisCommand.ExecuteReader()
                If thisReader.Read() Then
                    'If there is text in the reply box, then post it
                    If replyBoxMsg.Text.Length > 0 Then
                        'They have text in their textbox, so try to update their existing message box with the current text, and if that fails then create a new message box
                        'Select DB
                        Dim conn2 As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("spsmeetConnectionString").ConnectionString.ToString())

                        'Create Query
                        Dim query3 As New SqlCommand()

                        'Set the DB Query: update an existing message
                        query3.CommandText = "UPDATE msg SET mText = '" & escape(replyBoxMsg.Text) & _
                                            "',mDate = '" & Format(Now, "yyyy/MM/dd hh:mm:ss tt") & _
                                            "',final='0' WHERE mID = '" & thisReader.GetValue(0) & "'"
                        query3.Connection = conn2

                        Try
                            'Open Connection
                            conn2.Open()

                            'run
                            query3.ExecuteNonQuery()
                        Finally
                            conn2.Close()
                        End Try

                    Else
                        'They no longer have any text in their text box so pull the empty messag box from the database to remove clutter
                        'Select DB
                        Dim conn2 As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("spsmeetConnectionString").ConnectionString.ToString())

                        'Set the DB Query: update an existing message
                        Dim query3 As New SqlCommand()
                        query3.CommandText = "DELETE FROM msg WHERE mID = '" & thisReader.GetValue(0) & "'"
                        query3.Connection = conn2

                        Try
                            'Open Connection
                            conn2.Open()

                            'run
                            query3.ExecuteNonQuery()
                        Finally
                            conn2.Close()
                        End Try
                    End If
                Else
                    'There is no message written yet, so create it instead of updating it
                    If replyBoxMsg.Text.Length > 0 Then
                        'Select DB
                        Dim conn2 As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("spsmeetConnectionString").ConnectionString.ToString())

                        'Create Query
                        Dim query3 As New SqlCommand()

                        'Set the DB Query: write a new message
                        query3.CommandText = "INSERT INTO msg (eName, mText, uDomain, uName, mDate, final) VALUES ('" & _
                                            escape(Page.Request.QueryString("eName")) & "', '" & escape(replyBoxMsg.Text) & "', '" & _
                                            Page.User.Identity.Name.Split("\")(0) & "', '" & Page.User.Identity.Name.Split("\")(1) & "', '" & _
                                            Format(Now, "yyyy/MM/dd hh:mm:ss tt") & "', '0')"
                        query3.Connection = conn2

                        Try
                            'Open Connection
                            conn2.Open()

                            'run
                            query3.ExecuteNonQuery()
                        Finally
                            conn2.Close()
                        End Try
                    End If
                End If

                'Close Connection
                conn.Close()
            Catch
                'Close Connection
                conn.Close()
                Response.Redirect("index.aspx")
            End Try
        End If



        'Set DB Query: Display all incomplete messages currently being written by meeting members
        Dim query4 As String = "SELECT * FROM msg WHERE eName = '" & escape(Page.Request.QueryString("eName")) & "' AND final = '0' ORDER BY mDate DESC"

        'Create Command object
        Dim thisCommand2 As New SqlCommand(query4, conn)

        'Run SQL Query
        Try
            'Open Connection
            conn.Open()
            'Execute Query
            Dim thisReader As SqlDataReader = thisCommand2.ExecuteReader()
            Do While thisReader.Read()
                dbUsr = thisReader.GetValue(2) & "\" & thisReader.GetValue(3)
                If dbUsr.Equals(loggedUsr) Then
                    'Set up the html for displaying the user's own incomplete message
                    CD = CD & MessageEntry(loggedUsr, thisReader.GetValue(1), thisReader.GetValue(7), thisReader.GetValue(4), thisReader.GetValue(0), "4")
                Else
                    'Set up the html for displaying any incomplete messages not written by the user
                    CD = CD & MessageEntry(loggedUsr, thisReader.GetValue(1), thisReader.GetValue(7), thisReader.GetValue(4), thisReader.GetValue(0), "3")
                End If
            Loop

            'Close Connection
            conn.Close()
        Catch
            'Close Connection
            conn.Close()
            Response.Redirect("index.aspx")
        End Try

        'Put the html code into the conversation box
        convBoxData.Text = CD.ToString



        'Set DB Query: Display completed conversation messages
        Dim query5 As String = "SELECT * FROM msg WHERE eName = '" & escape(Page.Request.QueryString("eName")) & "' AND final = '1' ORDER BY mDate DESC"

        'Create Command object
        Dim thisCommand3 As New SqlCommand(query5, conn)

        'Run SQL Query
        Try
            'Open Connection
            conn.Open()
            'Execute Query
            Dim thisReader As SqlDataReader = thisCommand3.ExecuteReader()
            Do While thisReader.Read()
                dbUsr = thisReader.GetValue(2) & "\" & thisReader.GetValue(3)
                If dbUsr.Equals(loggedUsr) Then
                    'Set up the html for displaying the user's own incomplete message
                    CD = CD & MessageEntry(loggedUsr, thisReader.GetValue(1), thisReader.GetValue(7), thisReader.GetValue(4), thisReader.GetValue(0), "2")
                Else
                    'Set up the html for displaying any incomplete messages not written by the user
                    CD = CD & MessageEntry(loggedUsr, thisReader.GetValue(1), thisReader.GetValue(7), thisReader.GetValue(4), thisReader.GetValue(0), "1")
                End If
            Loop

            'Close Connection
            conn.Close()
        Catch
            'Close Connection
            conn.Close()
            Response.Redirect("index.aspx")
        End Try

        'Sets the conversation div to contain the list of meeting messages
        convBoxData.Text = CD.ToString



        'MEMBER LIST DATA GOES INTO MemberData.text AS HTML
        Dim offlineDate As DateTime = DateTime.Now.AddSeconds(-1 * INACTIVE_TIME)

        'get present meeting member list
        Dim MD As String = ""
        Dim query6 As String = "SELECT * FROM members WHERE ( eName = '" & _
                                escape(Page.Request.QueryString("eName")) & _
                                "' AND lastPoll > '" & offlineDate.Year & "-" & offlineDate.Month & "-" & offlineDate.Day & _
                                " " & offlineDate.ToLongTimeString & "' ) ORDER BY uName ASC"

        'Create Command object
        Dim thisCommand4 As New SqlCommand(query6, conn)

        Try
            'Open Connection
            conn.Open()
            'Execute Query
            Dim thisReader As SqlDataReader = thisCommand4.ExecuteReader()
            Do While thisReader.Read()
                MD = MD & MemberListEntry(thisReader.GetValue(4), thisReader.GetValue(0), "X")
            Loop
        Finally
            'Close Connection
            conn.Close()
        End Try


        'get absent meeting member list and display it
        Dim query7 As String = "SELECT * FROM members WHERE ( eName = '" & escape(Page.Request.QueryString("eName")) & _
                                "' AND lastPoll < '" & offlineDate.Year & "-" & offlineDate.Month & "-" & offlineDate.Day & _
                                " " & offlineDate.ToLongTimeString & "' ) ORDER BY uName ASC"

        'Create Command object
        Dim thisCommand5 As New SqlCommand(query7, conn)

        Try
            'Open Connection
            conn.Open()
            'Execute Query
            Dim thisReader As SqlDataReader = thisCommand5.ExecuteReader()
            Do While thisReader.Read()
                MD = MD & MemberListEntry(thisReader.GetValue(4), thisReader.GetValue(0), "O")
            Loop
        Finally
            'Close Connection
            conn.Close()
        End Try

        'Sets the member list div to contain the list of active and inactive meeting members
        memberBoxData.Text = MD.ToString
    End Sub





    'Definition: Returns a String containing the html formatted code for a message entry
    Private Function MessageEntry(loggedUsr As String, userMessage As String, name As String, timeStamp As String, mID As String, version As String) As String
        Dim message As String = vbNewLine & _
            "<div class='msg" & version & "'>" & vbNewLine & _
            "   <div class='msg" & version & "Body'>" & vbNewLine & _
            "       " & userMessage & vbNewLine & _
            "   </div>" & vbNewLine & _
            "   <div class='msg" & version & "Title'>" & vbNewLine & _
            "       <span class='name" & version & "'>" & name & "</span>" & vbNewLine & _
            "       <span class='date" & version & "'>(" & timeStamp & ")</span>" & vbNewLine

        'Allow the user to hide messages if they own the room
        If loggedUsr = Session("owner") Then
            message = message & "       <span class='delete" & version & "'><a href='rmMsg.aspx?eName=" & escape(Page.Request.QueryString("eName")) & "&mID=" & mID & "'>Hide</a></span>" & vbNewLine
        End If

        message = message & _
            "   </div>" & vbNewLine & _
            "</div>" & vbNewLine

        Return message
    End Function





    'Definition: Returns a String containing the html formatted code for a member list entry
    Private Function MemberListEntry(name As String, domain As String, status As String) As String
        Return vbNewLine & _
            "<div class='member'>" & vbNewLine & _
            "   " & status & " " & name & vbNewLine & _
            "</div>" & vbNewLine '& _
        '"<div class='domain'>" & vbNewLine & _
        '"   (" & domain & ")" & vbNewLine & _
        '"</div>" & vbNewLine
    End Function





    'Definition: Submits the user's text to the msg table in the database as a complete message
    Protected Sub Send(ByVal sender As Object, ByVal e As System.EventArgs) Handles replyBoxSendMsg.Click
        'Reset Timeout
        Session("timeout") = 0

        'Select DB
        Dim conn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("spsmeetConnectionString").ConnectionString.ToString())
        Dim query As New SqlCommand()

        'Set the DB Query
        query.CommandText = "INSERT INTO msg (eName, mText, uDomain, uName, mDate, final, name) VALUES ('" & _
                            escape(Page.Request.QueryString("eName")) & "', '" & _
                            escape(replyBoxMsg.Text) & "', '" & _
                            Page.User.Identity.Name.Split("\")(0) & "', '" & _
                            Page.User.Identity.Name.Split("\")(1) & "', '" & _
                            Format(Now, "yyyy-MM-dd hh:mm:ss tt") & "', '1'" & _
                            ", '" & Session("FnameLname") & "')"
        query.Connection = conn

        'Run SQL Query: Create the meeting in the database   
        Try
            conn.Open()
            query.ExecuteNonQuery()
        Finally
            conn.Close()
        End Try

        'Clear Msg
        replyBoxMsg.Text = ""

        'Refresh the Conversation Window
        Timer_Tick()
    End Sub
End Class