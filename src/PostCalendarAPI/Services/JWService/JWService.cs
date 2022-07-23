using System.Text.RegularExpressions;
using System.Net;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
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

        public async Task AnalysisExcel(Stream excelStream)
        {
            var workbook = new HSSFWorkbook(excelStream);

            if(workbook != null)
            {
                ISheet sheet = workbook.GetSheetAt(0);

                for(int i = sheet.FirstRowNum + 2; i <= sheet.LastRowNum; i++)
                {

                }
            }
        }

        /// <summary>
        /// 解析单个单元格中的内容
        /// </summary>
        /// <param name="cellString">单元格字符串</param>
        /// <param name="dayOfWeek">单元格所在的星期几</param>
        /// <returns>课程列表</returns>
        /// <exception cref="JWAnalysisException">解析失败引发的异常</exception>
        private IEnumerable<Course> AnalyseSingleCell(string cellString, int dayOfWeek)
        {
            var lines = cellString.Split("\n");
            var courses = new List<Course>();

            switch(lines.Length)
            {
                case 5:
                    // 只有一门课程的单元格
                    // 没有分组
                    int[] weeks = Course.AnalyseWeekString(lines[2]);
                    int[] classes = Course.AnalyseTimeString(lines[4]);
                    var course = new Course(
                        lines[0],
                        lines[1],
                        lines[3],
                        weeks,
                        classes[0],
                        classes[1],
                        dayOfWeek
                        );
                    courses.Add(course);

                    break;
                case 6:
                    // 只有一门课程的单元格
                    // 含有分组
                    weeks = Course.AnalyseWeekString(lines[3]);
                    classes = Course.AnalyseTimeString(lines[5]);
                    course = new Course(
                        lines[0],
                        lines[2] + lines[1],// 老师和分组合并显示
                        lines[4],
                        weeks,
                        classes[0],
                        classes[1],
                        dayOfWeek
                        );
                    courses.Add(course);

                    break;
                case 10:
                    // 含有两门课程的单元格
                    // 均没有分组
                    weeks = Course.AnalyseWeekString(lines[2]);
                    classes = Course.AnalyseTimeString(lines[4]);
                    course = new Course(
                        lines[0],
                        lines[1],
                        lines[3],
                        weeks,
                        classes[0],
                        classes[1],
                        dayOfWeek
                        );
                    courses.Add(course);

                    weeks = Course.AnalyseWeekString(lines[7]);
                    classes = Course.AnalyseTimeString(lines[9]);
                    course = new Course(
                        lines[5],
                        lines[6],
                        lines[8],
                        weeks,
                        classes[0],
                        classes[1],
                        dayOfWeek
                        );
                    courses.Add(course);

                    break;
                case 11:
                    // 含有两门课程的单元格
                    // 有一门课程有分组
                    // 通过第五行是否存在”节“来判断
                    if (lines[4].Contains("节"))
                    {
                        // 第一门没有分组
                        weeks = Course.AnalyseWeekString(lines[2]);
                        classes = Course.AnalyseTimeString(lines[4]);
                        course = new Course(
                            lines[0],
                            lines[1],
                            lines[3],
                            weeks,
                            classes[0],
                            classes[1],
                            dayOfWeek
                            );
                        courses.Add(course);

                        // 第二门有分组
                        weeks = Course.AnalyseWeekString(lines[8]);
                        classes = Course.AnalyseTimeString(lines[10]);
                        course = new Course(
                            lines[5],
                            lines[7] + lines[6],// 老师和分组合并显示
                            lines[9],
                            weeks,
                            classes[0],
                            classes[1],
                            dayOfWeek
                            );
                        courses.Add(course);

                        break;
                    }
                    else
                    {
                        // 第一门有分组
                        weeks = Course.AnalyseWeekString(lines[3]);
                        classes = Course.AnalyseTimeString(lines[5]);
                        course = new Course(
                            lines[0],
                            lines[2] + lines[1],// 老师和分组合并显示
                            lines[4],
                            weeks,
                            classes[0],
                            classes[1],
                            dayOfWeek
                            ); ;
                        courses.Add(course);

                        // 第二门没有分组
                        weeks = Course.AnalyseWeekString(lines[8]);
                        classes = Course.AnalyseTimeString(lines[10]);
                        course = new Course(
                            lines[6],
                            lines[7],
                            lines[9],
                            weeks,
                            classes[0],
                            classes[1],
                            dayOfWeek
                            );
                        courses.Add(course);

                        break;
                    }
                case 12:
                    // 含有两门课程的单元格
                    // 均有分组
                    weeks = Course.AnalyseWeekString(lines[3]);
                    classes = Course.AnalyseTimeString(lines[5]);
                    course = new Course(
                        lines[0],
                        lines[2] + lines[1],// 老师和分组合并显示
                        lines[4],
                        weeks,
                        classes[0],
                        classes[1],
                        dayOfWeek
                        );
                    courses.Add(course);

                    weeks = Course.AnalyseWeekString(lines[9]);
                    classes = Course.AnalyseTimeString(lines[11]);
                    course = new Course(
                        lines[6],
                        lines[8] + lines[7],// 老师和分组合并显示
                        lines[10],
                        weeks,
                        classes[0],
                        classes[1],
                        dayOfWeek
                        );
                    courses.Add(course);

                    break;
                default:
                    throw new JWAnalysisException($"解析单元格失败,含有{lines.Length}行");
            }

            return courses;
        }
    }
}
