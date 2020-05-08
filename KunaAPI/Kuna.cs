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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Newtonsoft.Json;

public class KunaAPI
{
    private string API_URL = "https://server.kunasystems.com/api/v1/";
    private string AUTH_ENDPOINT = "account/auth";
    private string CAMERAS_ENDPOINT = "user/cameras";
    private string RECORDINGS_ENDPOINT = "recordings";

    private string USER_AGENT = "Kuna/2.4.4 (iPhone; iOS 12.1; Scale/3.00)";
    private string USER_AGENT_THUMBNAIL = "Kuna/156 CFNetwork/975.0.3 Darwin/18.2.0";

    // Recordings
    private string ENDPOINT_CAMERA = "cameras";
    private string ENDPOINT_USERS = "users";
    private string ENDPOINT_THUMBNAIL = "thumbnail";
    private string ENDPOINT_RECORDINGS = "recordings";

    private string KUNA_STREAM_URL = "wss://server.kunasystems.com/ws/rtsp/proxy";

    private string _username;
    private string _password;
    private string _websession;
    private string _token = null;
    private string cameras;

    public class auth
    {
        public string token { get; set; } = "";
    }
    public async Task Authenticate()
    {
        // Login and get an auth token.
        var json = "{\"email\": \"" + _username + "\", \"password\": \"" + _password + "\"}";
        var result = await _request("post", AUTH_ENDPOINT, json);
        try
        {
            _token = FromJson<auth>(result).token;
        }
        // Console.WriteLine(_token)

        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw ex;
        }
    }
    public async Task<Cameras> update()
    {
        // Refresh the dict of all cameras in the Kuna account."""
        var result="";
        try
        {
            result = await _request("get", CAMERAS_ENDPOINT);
            // Console.WriteLine(result)
            return FromJson<Cameras>(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw ex;
        }
    }

    public async Task<Recordings> GetAllRecordings(Camera camera)
    {
        var result="";
        try
        {
            result = await _request("get", string.Format("{0}/{1}/{2}/", ENDPOINT_CAMERA, camera.serial_number, ENDPOINT_RECORDINGS));
            // Console.WriteLine(result)
            return FromJson<Recordings>(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw ex;
        }
    }
    public async Task<string> GetDownloadLink(Recording recording)
    {
        var result="";
        try
        {
            result = await _request("get", recording.mp4.Replace(API_URL, ""), allow_redirects: false);
            // Console.WriteLine(result)
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw ex;
        }
    }
    public async Task<System.IO.FileInfo> DownloadVideo(string link, string filename)
    {
        try
        {
            using (var client = new HttpClient())
            {
                // client.BaseAddress = New Uri(baseUrl)
                client.Timeout = TimeSpan.FromMinutes(5);
                string requestUrl = link;

                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                var sendTask = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                var response = sendTask.Result.EnsureSuccessStatusCode();
                var httpStream = await response.Content.ReadAsStreamAsync();

                using (var fileStream = File.Create(filename)) // (filename & response.Content.Headers.ContentType.MediaType)
                {
                    using (var reader = new StreamReader(httpStream))
                    {
                        await httpStream.CopyToAsync(fileStream);
                        fileStream.Flush();

                        return new FileInfo(filename);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    static HttpClientHandler handler = new HttpClientHandler();
    public HttpClient Client { get; } = new HttpClient(handler);

    public KunaAPI(string username, string password)
    {
        _username = username;
        _password = password;
        // _websession = websession

        _token = null;
        // cameras ={};

        Client.BaseAddress = new Uri(API_URL);

        Client.DefaultRequestHeaders.Add("Accept", "*/*");
        Client.DefaultRequestHeaders.Add("Accept-Encoding", "deflate, gzip");

        {
            var withBlock = handler;
            withBlock.AllowAutoRedirect = false;
            withBlock.AutomaticDecompression = DecompressionMethods.Deflate & DecompressionMethods.GZip;
        }

        this.Client = Client;
    }

    public async Task<string> _request(string method, string path, string json = "", bool image = false, bool redirected = false, bool allow_redirects = true)
    {
        DateTime last_request = DateTime.Now;
        int timebetweencalls = 500;
        if ((DateTime.Now - last_request).TotalMilliseconds < timebetweencalls)
        {
            // Console.WriteLine("Sleep before next call: " & (Now - last_request).TotalMilliseconds + timebetweencalls)
            // System.Threading.Thread.Sleep((DateTime.Now - last_request).TotalMilliseconds + timebetweencalls);
            last_request = DateTime.Now;
        }

        HttpResponseMessage response = new HttpResponseMessage();

        // If _token <> "" Then Client.DefaultRequestHeaders.Add("Authorization", _token)

        if (image == true | redirected == true)
        {
            Client.DefaultRequestHeaders.Remove("User-Agent");
            Client.DefaultRequestHeaders.Add("User-Agent", USER_AGENT_THUMBNAIL);
        }
        else
        {
            Client.DefaultRequestHeaders.Remove("User-Agent");
            Client.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
        }

        // If method.ToLower = "post" Then
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, path);

        request.Method = new HttpMethod(method);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json"); // CONTENT-TYPE header
        if (request.Headers.Contains("Authorization"))
            request.Headers.Remove("Authorization");
        if (_token != "")
            request.Headers.Add("Authorization", string.Format("token {0}", _token));

        // response = Await Client.PostAsync(path, New StringContent(json))

        response = await Client.SendAsync(request);

        // ElseIf method.ToLower = "get" Then
        // response = Await Client.GetAsync(path)
        // End If

        if (response.StatusCode == HttpStatusCode.Moved | response.StatusCode == HttpStatusCode.Redirect | response.StatusCode == HttpStatusCode.MovedPermanently)
        {
            // Console.WriteLine(response.StatusCode)
            if (allow_redirects == false & response.Headers.Location.OriginalString != "")
                return response.Headers.Location.OriginalString;
            var responsestring = _request(method, response.Headers.Location.OriginalString, json, redirected: true).Result;
            if (responsestring != "")
                return responsestring;
        }

        response.EnsureSuccessStatusCode();

        var responsestream = await response.Content.ReadAsStreamAsync();
        System.IO.StreamReader rdr = new System.IO.StreamReader(responsestream);
        return rdr.ReadToEndAsync().Result;
    }
    public T FromJson<T>(string json)
    {
        if (string.IsNullOrEmpty(json))
            return default(T);
        try
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch (Exception)
        {
            return default(T);
        }
    }
}
