

Public Class Recording
    Public Property url As String
    Public Property id As Integer
    Public Property label As String
    Public Property camera As Camera
    Public Property description As String
    Public Property timestamp As DateTime
    Public Property duration As Integer
    Public Property status As Integer
    Public Property m3u8 As String
    Public Property thumbnails As String()
    Public Property classification As Integer
    Public Property created_at As DateTime
    Public Property updated_at As DateTime
    Public Property mp4 As String
    Public Property events As Object
End Class

Public Class Recordings
    Public Property [next] As Object
    Public Property previous As Object
    Public Property results As Recording()
End Class
