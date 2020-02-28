Public Class Owner
    Public Property url As String
    Public Property id As Integer
    Public Property email As String
    Public Property first_name As String
    Public Property last_name As String
    Public Property updated_at As DateTime
End Class

Public Class Location
    Public Property type As String
    Public Property coordinates As Double()
End Class

Public Class Camera
    Public Property url As String
    Public Property id As Integer
    Public Property serial_number As String
    Public Property owner As Owner
    Public Property name As String
    Public Property timezone As String
    Public Property status As Integer
    Public Property bulb_on As Boolean
    Public Property alarm_on As Boolean
    Public Property led_mask As Boolean
    Public Property bluetooth_identifier As String
    Public Property updated_at As DateTime
    Public Property recordings_url As String
    Public Property users_url As String
    Public Property sensitivity As Integer
    Public Property build As Integer
    Public Property volume As Integer
    Public Property notifications_enabled As Boolean
    Public Property doorbell_press_notifications_enabled As Boolean
    Public Property location As Location
    Public Property location_address As String
    Public Property subscription As Object
    Public Property dawn_offset As Integer
    Public Property dusk_offset As Integer
    Public Property motion_timeout As Integer
    Public Property mesh_group_id As Integer
    Public Property companions_count As Integer
    Public Property down_at As DateTime
    Public Property sight_option As Integer
    Public Property sight_stationary_filter As Boolean
    Public Property sight_stationary_mse_filter As Boolean
    Public Property play_msg_on_detect As Integer
    Public Property created_at As DateTime
    Public Property sight_on_lite As Boolean
    Public Property ip_address As String
    Public Property recording_active As Boolean
    Public Property brightness As Integer
    Public Property video_flip As Boolean
    Public Property sight_on As Boolean
    Public Property sight_on_override As Boolean
    Public Property autosight As Boolean
    Public Property sight_permissions As Integer
    Public Property server_up As String
    Public Property support_permission_end As Object
    Public Property custom_messages As Object()
    Public Property chime_duration_use_default As Boolean
    Public Property chime_duration As Double
    Public Property on_cam_ai As Boolean
End Class

Public Class Cameras
    Public Property count As Integer
    Public Property [next] As Object
    Public Property previous As Object
    Public Property results As Camera()
End Class