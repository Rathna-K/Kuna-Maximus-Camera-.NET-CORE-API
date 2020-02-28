Imports System
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Reflection
Imports System.Text
Imports System.Text.Json
Imports Newtonsoft.Json
Public Class KunaAPI


    Dim API_URL As String = "https://server.kunasystems.com/api/v1/"
    Dim AUTH_ENDPOINT As String = "account/auth"
    Dim CAMERAS_ENDPOINT As String = "user/cameras"
    Dim RECORDINGS_ENDPOINT As String = "recordings"

    Dim USER_AGENT As String = "Kuna/2.4.4 (iPhone; iOS 12.1; Scale/3.00)"
    Dim USER_AGENT_THUMBNAIL As String = "Kuna/156 CFNetwork/975.0.3 Darwin/18.2.0"

    'Recordings
    Dim ENDPOINT_CAMERA = "cameras"
    Dim ENDPOINT_USERS = "users"
    Dim ENDPOINT_THUMBNAIL = "thumbnail"
    Dim ENDPOINT_RECORDINGS = "recordings"

    Dim KUNA_STREAM_URL As String = "wss://server.kunasystems.com/ws/rtsp/proxy"

    Dim _username As String
    Dim _password As String
    Dim _websession As String
    Dim _token As String = Nothing
    Dim cameras = {}

    Public Class auth
        Property token As String = ""
    End Class
    Async Function Authenticate() As Task
        'Login and get an auth token.
        Dim json = "{""email"": """ & _username & """, ""password"": """ & _password & """}"
        Dim result = Await _request("post", AUTH_ENDPOINT, json)
        Try
            _token = FromJson(Of auth)(result).token
            'Console.WriteLine(_token)

        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Throw ex
        End Try
    End Function
    Async Function update() As Task(Of Cameras)
        'Refresh the dict of all cameras in the Kuna account."""
        Dim result
        Try
            result = Await _request("get", CAMERAS_ENDPOINT)
            'Console.WriteLine(result)
            Return FromJson(Of Cameras)(result)
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Throw ex
        End Try

    End Function

    Public Async Function GetAllRecordings(camera As Camera) As Task(Of Recordings)
        Dim result
        Try
            result = Await _request("get", String.Format("{0}/{1}/{2}/", ENDPOINT_CAMERA, camera.serial_number, ENDPOINT_RECORDINGS))
            'Console.WriteLine(result)
            Return FromJson(Of Recordings)(result)
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Throw ex
        End Try

    End Function
    Public Async Function GetDownloadLink(recording As Recording) As Task(Of String)
        Dim result
        Try
            result = Await _request("get", recording.mp4.Replace(API_URL, ""), allow_redirects:=False)
            'Console.WriteLine(result)
            Return result
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Throw ex
        End Try

    End Function
    Public Async Function DownloadVideo(link As String, filename As String) As Task(Of IO.FileInfo)
        Try

            Using client = New HttpClient()
                'client.BaseAddress = New Uri(baseUrl)
                client.Timeout = TimeSpan.FromMinutes(5)
                Dim requestUrl As String = link

                Dim request = New HttpRequestMessage(HttpMethod.Get, requestUrl)
                Dim sendTask = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                Dim response = sendTask.Result.EnsureSuccessStatusCode()
                Dim httpStream = Await response.Content.ReadAsStreamAsync()

                Using fileStream = File.Create(filename) ' (filename & response.Content.Headers.ContentType.MediaType)

                    Using reader = New StreamReader(httpStream)
                        Await httpStream.CopyToAsync(fileStream)
                        fileStream.Flush()

                        Return New FileInfo(filename)

                    End Using
                End Using
            End Using

        Catch ex As Exception
            Throw ex
        End Try

    End Function


    Dim handler As New HttpClientHandler
    Public ReadOnly Property Client As New HttpClient(handler)

    Public Sub New(username As String, password As String)
        _username = username
        _password = password
        '_websession = websession

        _token = Nothing
        cameras = {}

        Client.BaseAddress = New Uri(API_URL)

        Client.DefaultRequestHeaders.Add("Accept", "*/*")
        Client.DefaultRequestHeaders.Add("Accept-Encoding", "deflate, gzip")

        With handler
            .AllowAutoRedirect = False
            .AutomaticDecompression = DecompressionMethods.Deflate And DecompressionMethods.GZip
        End With

        Me.Client = Client
    End Sub

    Public Async Function _request(method As String, path As String, Optional json As String = "", Optional image As Boolean = False, Optional redirected As Boolean = False, Optional allow_redirects As Boolean = True) As Task(Of String)
        Dim last_request As DateTime = Now
        Dim timebetweencalls As Integer = 500
        If (Now - last_request).TotalMilliseconds < timebetweencalls Then
            'Console.WriteLine("Sleep before next call: " & (Now - last_request).TotalMilliseconds + timebetweencalls)
            Threading.Thread.Sleep((Now - last_request).TotalMilliseconds + timebetweencalls)
            last_request = Now
        End If

        Dim response As New HttpResponseMessage

        'If _token <> "" Then Client.DefaultRequestHeaders.Add("Authorization", _token)

        If image = True Or redirected = True Then
            Client.DefaultRequestHeaders.Remove("User-Agent")
            Client.DefaultRequestHeaders.Add("User-Agent", USER_AGENT_THUMBNAIL)
        Else
            Client.DefaultRequestHeaders.Remove("User-Agent")
            Client.DefaultRequestHeaders.Add("User-Agent", USER_AGENT)
        End If

        'If method.ToLower = "post" Then
        Dim request As HttpRequestMessage = New HttpRequestMessage(HttpMethod.Post, path)

        request.Method = New HttpMethod(method)
        request.Content = New StringContent(json, Encoding.UTF8, "application/json") 'CONTENT-TYPE header
        If request.Headers.Contains("Authorization") Then
            request.Headers.Remove("Authorization")
        End If
        If _token <> "" Then request.Headers.Add("Authorization", String.Format("token {0}", _token))

        'response = Await Client.PostAsync(path, New StringContent(json))

        response = Await Client.SendAsync(request)

        'ElseIf method.ToLower = "get" Then
        'response = Await Client.GetAsync(path)
        'End If

        If response.StatusCode = HttpStatusCode.Moved Or response.StatusCode = HttpStatusCode.Redirect Or response.StatusCode = HttpStatusCode.MovedPermanently Then
            'Console.WriteLine(response.StatusCode)
            If allow_redirects = False And response.Headers.Location.OriginalString <> "" Then
                Return response.Headers.Location.OriginalString
            End If
            Dim responsestring = _request(method, response.Headers.Location.OriginalString, json, redirected:=True).Result
            If responsestring <> "" Then Return responsestring
        End If

        response.EnsureSuccessStatusCode()

        Dim responsestream = Await response.Content.ReadAsStreamAsync()
        Dim rdr As IO.StreamReader = New IO.StreamReader(responsestream)
        Return rdr.ReadToEndAsync().Result

    End Function
    Public Function FromJson(Of T)(json As String) As T
        If String.IsNullOrEmpty(json) Then
            Return Nothing
        End If

        Try

            Return JsonConvert.DeserializeObject(Of T)(json)

        Catch generatedExceptionName As Exception
            Return Nothing
        End Try
    End Function


End Class
