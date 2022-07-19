using System.Text;
using System.Text.Json;
using PostCalendarAPITools.Excel;

namespace PostCalendarAPITools.Network
{
    public class APIHelper
    {
        private readonly string _baseUrl = "https://localhost:7000/api/users";
        private readonly HttpClient _httpClient;

        public APIHelper()
        {
            _httpClient = new HttpClient();

            _httpClient.Timeout = new TimeSpan(0, 0, 100);
        }

        public async Task InitHelper()
        {
            string token = await GetToken();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        }

        public async Task AddUser(ExcelModel item)
        {
            StringContent content = new StringContent(JsonSerializer.Serialize(item), encoding: Encoding.UTF8);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await _httpClient.PostAsync(_baseUrl, content);

            Console.WriteLine(item.ToString() + response.StatusCode);
        }

        private async Task<string> GetToken()
        {
            string url = _baseUrl + "/admin?userName=admin&studentID=123456";
            var response = await _httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
