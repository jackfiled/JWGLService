using System.Text.RegularExpressions;
using System.Net;
using PostCalendarAPI.Services.JWService.Models;

namespace PostCalendarAPI.Services.JWService
{
    public class JWService
    {
        public CookieContainer cookies = new CookieContainer();

        private readonly HttpClient _httpClient;
        private ILogger _logger;
        private static readonly string _baseUrl = "https://jwgl.bupt.edu.cn/jsxsd/";

        public JWService(ILogger logger)
        {
            _logger = logger;

            var handler = new HttpClientHandler();
            handler.CookieContainer = cookies;
            handler.UseCookies = true;
            _httpClient = new HttpClient(handler);

            InitService();
        }

        public void InitService()
        {
            _httpClient.DefaultRequestHeaders.Add(
                "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.82 Safari/537.36"
                );

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(_baseUrl),
                Method = HttpMethod.Get,
            };

            _httpClient.Send(request);
        }

        public async Task Login(string studentID, string password)
        {
            var loginModel = new LoginModel(studentID, password);

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(_baseUrl + "xk/LoginToXk"),
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(loginModel.keyValuePairs),
            };

            // 设置请求的Headers
            request.Headers.Host = "jwgl.bupt.edu.cn";
            request.Headers.Referrer = new Uri("http://jwgl.bupt.edu.cn/jsxsd");

            await _httpClient.SendAsync(request);
        }

        public async Task<Byte[]> DownloadExcel(string semester)
        {
            var downloadModel = new DownloadModel(semester);

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(_baseUrl + downloadModel.GetDownloadUri()),
                Method = HttpMethod.Post,
                Content = JsonContent.Create(downloadModel, typeof(DownloadModel))
            };

            var response = await _httpClient.SendAsync(request);

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<bool> CheckLogin()
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(_baseUrl),
                Method = HttpMethod.Get,
            };

            var response = await _httpClient.SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);

            return !Regex.IsMatch(content, "用户登录");
        }
    }
}
