using System;


    public class Owner
    {
        public string url { get; set; }
        public int id { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class Location
    {
        public string type { get; set; }
        public double[] coordinates { get; set; }
    }


    public class Camera
    {
        public string url { get; set; }
        public int id { get; set; }
        public string serial_number { get; set; }
        public Owner owner { get; set; }
        public string name { get; set; }
        public string timezone { get; set; }
        public int status { get; set; }
        public bool bulb_on { get; set; }
        public bool alarm_on { get; set; }
        public bool led_mask { get; set; }
        public string bluetooth_identifier { get; set; }
        public DateTime updated_at { get; set; }
        public string recordings_url { get; set; }
        public string users_url { get; set; }
        public int sensitivity { get; set; }
        public int build { get; set; }
        public int volume { get; set; }
        public bool notifications_enabled { get; set; }
        public bool doorbell_press_notifications_enabled { get; set; }
        public Location location { get; set; }
        public string location_address { get; set; }
        public object subscription { get; set; }
        public int dawn_offset { get; set; }
        public int dusk_offset { get; set; }
        public int motion_timeout { get; set; }
        public int mesh_group_id { get; set; }
        public int companions_count { get; set; }
        public DateTime down_at { get; set; }
        public int sight_option { get; set; }
        public bool sight_stationary_filter { get; set; }
        public bool sight_stationary_mse_filter { get; set; }
        public int play_msg_on_detect { get; set; }
        public DateTime created_at { get; set; }
        public bool sight_on_lite { get; set; }
        public string ip_address { get; set; }
        public bool recording_active { get; set; }
        public int brightness { get; set; }
        public bool video_flip { get; set; }
        public bool sight_on { get; set; }
        public bool sight_on_override { get; set; }
        public bool autosight { get; set; }
        public int sight_permissions { get; set; }
        public string server_up { get; set; }
        public object support_permission_end { get; set; }
        public object[] custom_messages { get; set; }
        public bool chime_duration_use_default { get; set; }
        public double chime_duration { get; set; }
        public bool on_cam_ai { get; set; }
    }

    public class Cameras
    {
        public int count { get; set; }
        public object next { get; set; }
        public object previous { get; set; }
        public Camera[] results { get; set; }
    }

