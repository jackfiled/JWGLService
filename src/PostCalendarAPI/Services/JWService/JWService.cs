using System.Buffers.Text;
using System.Text;
using System.Net.Http.Headers;
using PostCalendarAPI.Services.JWService.Models;


namespace PostCalendarAPI.Services.JWService
{
    public class JWService
    {
        private readonly HttpClient _httpClient;
        private ILogger _logger;
        private static readonly string _baseUrl = "http://jwgl.bupt.edu.cn/jsxsd/";

        public JWService(ILogger logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }

        private void InitService()
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
                Content = JsonContent.Create(loginModel, typeof(LoginModel))
            };

            // 设置请求的Headers
            request.Headers.Host = "jwgl.bupt.edu.cn";
            request.Headers.Referrer = new Uri("http://jwgl.bupt.edu.cn/jsxsd/xk/LoginToXk?method=exit&tktime=1631723647000");

            var response = await _httpClient.SendAsync(request);
        }

        public async Task DownloadExcel(string semester)
        {
            var downloadModel = new DownloadModel(semester);

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(_baseUrl + downloadModel.GetDownloadUri()),
                Method = HttpMethod.Post,
                Content = JsonContent.Create(downloadModel, typeof(DownloadModel))
            };

            var response = await _httpClient.SendAsync(request);
        }
    }
}
