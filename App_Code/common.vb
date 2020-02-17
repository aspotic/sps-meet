Imports System.Data.SqlClient

Public Class common
    Inherits System.Web.UI.Page

    'Description: Removes unwanted characters and words from user input strings
    Function escape(InputString As String) As String
        Dim a As String
        a = InputString

        'replace basic swear words
        Dim swearString As String = "ass,asses,asshole,assholes,asskisser,asswipe,balls,bastard,beastial,beastiality,beastility,bestial,bestiality,bitch,bitcher,bitchers,bitches,bitchin,bitching,blow job,blowjob,blowjobs,bonehead,boner,browneye,browntown,bullshit,bung hole,butch,buttface,buttfuck,buttfucker,butthead,butthole,buttpicker,clit,cobia,cock,cocks,cocksuck,cocksucked,cocksucker,cocksucking,cocksucks,cooter,crap,cum,cummer,cumming,cums,cumshot,cunilingus,cunillingus,cunnilingus,cunt,cuntlick,cuntlicker,cuntlicking,cunts,cyberfuc,cyberfuck,cyberfucked,cyberfucker,cyberfuckers,cyberfucking,damn,dick,dike,dildo,dildos,dink,dinks,dipshit,dong,dumbass,dyke,fag,fagget,fagging,faggit,faggot,faggs,fagot,fagots,fags,fartings,farty,fatass,fatso,felatio,fellatio,fingerfuck,fingerfucked,fingerfucker,fingerfuckers,fingerfucking,fingerfucks,fistfuck,fistfucked,fistfucker,fistfuckers,fistfucking,fistfuckings,fistfucks,fuck,fucked,fucker,fuckers,fuckin,fucking,fuckings,fuckme,fucks,fuk,fuks,furburger,gangbang,gangbanged,gangbangs,gaysex,gazongers,goddamn,gonads,gook,guinne,hardcoresex,homo,horniest,hotsex,hussy,jackass,jacking off,jackoff,jack-off,jap,jerk,jerk-off,jism,jiz,jizm,jizz,kike,kock,kondum,kondums,kraut,kum,kummer,kumming,kums,kunilingus,lesbo,mick,mothafuck,mothafucka,mothafuckas,mothafuckaz,mothafucked,mothafucker,mothafuckers,mothafuckin,mothafucking,mothafuckings,mothafucks,motherfuck,motherfucked,motherfucker,motherfuckers,motherfuckin,motherfucking,motherfuckings,motherfucks,muff,nigger,niggers,pecker,phonesex,phuk,phuked,phuking,phukked,phukking,phuks,phuq,piss,pissed,pissrr,pissers,pisses,pissin,pissing,pissoff,prick,pricks,pussies,pussy,pussys,queer,retard,schlong,sheister,shit,shited,shitfull,shiting,shitings,shits,shitted,shitter,shitters,shitting,shittings,shitty,sleaze,slut,sluts,smut,spunk,twat,wetback,whore,wop"
        Dim swearList() As String = swearString.Split(",")
        Dim cleanString As String = "Defenestrate,Agnomen,Crapulous,Doppelganger,Ambergris,Deus Ex Machina,Frabjous,Fop,Hemidemisemiquaver,Erinaceous,Lamprophony,Depone,Finnimbrun,floccinaucinihilipilification,Inaniloquent,Limerance,Mesonoxian,Mungo"
        Dim cleanList() As String = cleanString.Split(",")
        Dim upperbound As Integer = 17
        Randomize()
        Dim randVal As Integer = CInt(Int((upperbound + 1) * Rnd()))
        For i As Integer = 0 To swearList.Length - 1 Step 1
            a = Replace(a, " " & swearList(i) & " ", " " & cleanList(randVal) & " ")
        Next

        'Replace html and sql sensitive characters
        a = Replace(a, "'", "''")
        a = Replace(a, "&", "&amp;")
        a = Replace(a, "  ", "&nbsp;&nbsp;")
        a = Replace(a, "<", "&lt;")
        a = Replace(a, ">", "&gt;")
        a = Replace(a, Chr(10), "<br />")

        'only try to create hyperlinks if there are any
        If a.Contains("http://") Then
            'Split up the message into an array of each word to weed out the hyperlinks
            Dim words() As String = a.Split(" ")

            'check to see if each word is a hyperlink and reform the message
            a = ""
            For i As Integer = 0 To words.Length - 1 Step 1
                'if the word is a hyperlink, then format the html properly
                If words(i).StartsWith("http://") Then
                    Dim word As String = words(i)
                    words(i) = "<a href=""" & word.ToString & """ target=""_BLANK"">" & word.ToString & "</a>"
                End If
                a = a.ToString & words(i).ToString & " "
            Next
        End If

        Return a
    End Function

    'Description: removes spaces and apostraphes to keep them from occuring in meeting names that will be placed in urls
    Function escapespace(InputString As String) As String
        Dim a As String
        a = InputString
        a = Replace(a, "'", "''")
        a = Replace(a, " ", "")
        Return a
    End Function

    'Description: Do a basic check to see if the user is on a mobile browser.  if they are, then if no theme cookie is set, set it to the mobile theme mode, otherwise use SPSGreen
    Sub setDefaultTheme()
        Dim bInfo As HttpBrowserCapabilities = Request.Browser
        If Request.Cookies("LayoutTheme") Is Nothing Then
            If bInfo.IsMobileDevice Then
                Response.Cookies("ColorTheme").Value = "Mobile"
                Response.Cookies("LayoutTheme").Value = "Mobile"
                Response.Cookies("FontTheme").Value = "Mobile"
                Response.Cookies("Stream").Value = "Hide"
            Else
                Response.Cookies("ColorTheme").Value = "SPSGreen"
                Response.Cookies("LayoutTheme").Value = "Vertical"
                Response.Cookies("FontTheme").Value = "ComicSans"
                Response.Cookies("Stream").Value = "Hide"
            End If
        End If
    End Sub

    'Description: Updates tables to contain proper entries based on current date
    Sub updateTableHistory()
        'Select DB
        Dim conn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("spsmeetConnectionString").ConnectionString.ToString())
        Dim query As New SqlCommand()

        'Set the DB Query: Move old meetings to history, and purge meetings older than _ days from history   
        query.CommandText = "EXEC MoveToHistory; EXEC PurgeHistory"
        query.Connection = conn

        'Run SQL Query
        Try
            conn.Open()
            query.ExecuteNonQuery()
        Finally
            conn.Close()
        End Try

    End Sub

    'Description: Verifies the user owns the meeting. if they don't, then they are redirected to redirectLocation
    Sub verifyMeetingOwner(redirectLocation As String, eName As String)

        'Select DB
        Dim conn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("spsmeetConnectionString").ConnectionString.ToString())

        'Set Query: check for the user/meeting name combo in the closed meetings list
        Dim query As String = "SELECT eName,uDomain,uName FROM meet_history WHERE ( eName = '" & escape(eName) & "' AND uDomain = '" & User.Identity.Name.Split("\")(0) & "' AND uName = '" & User.Identity.Name.Split("\")(1) & "' )"

        'Create Command object
        Dim command1 As New SqlCommand(query, conn)

        Try
            'Open Connection
            conn.Open()

            'Execute Query
            Dim reader As SqlDataReader = command1.ExecuteReader()

            If Not reader.Read() Then
                reader.Close()

                'Select DB
                Dim conn2 As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("spsmeetConnectionString").ConnectionString.ToString())

                'Set Query: check for the user/meeting name combo in the open meetings list
                Dim query2 As String = "SELECT eName,uDomain,uName FROM meet WHERE ( eName = '" & escape(eName) & "' AND uDomain = '" & User.Identity.Name.Split("\")(0) & "' AND uName = '" & User.Identity.Name.Split("\")(1) & "' )"

                'Create Command object
                Dim command2 As New SqlCommand(query2, conn2)

                Try
                    'Open Connection
                    conn2.Open()

                    'Execute Query
                    Dim reader2 As SqlDataReader = command2.ExecuteReader()
                    'If the owner is still not matched, then redirect the user
                    If Not reader2.Read() Then
                        Response.Redirect(redirectLocation)
                    End If
                Finally
                    'Close Connection
                    conn2.Close()
                End Try

            End If

        Finally
            'Close Connection
            conn.Close()
        End Try

    End Sub

End Class