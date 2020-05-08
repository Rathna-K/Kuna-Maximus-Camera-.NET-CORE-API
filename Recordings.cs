
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

public class Recording
{
    public string url { get; set; }
    public int id { get; set; }
    public string label { get; set; }
    public Camera camera { get; set; }
    public string description { get; set; }
    public DateTime timestamp { get; set; }
    public int duration { get; set; }
    public int status { get; set; }
    public string m3u8 { get; set; }
    public string[] thumbnails { get; set; }
    public int classification { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public string mp4 { get; set; }
    public object events { get; set; }
}

public class Recordings
{
    public object next { get; set; }
    public object previous { get; set; }
    public Recording[] results { get; set; }
}
