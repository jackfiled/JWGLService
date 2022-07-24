using System.Text.RegularExpressions;
using System.Net;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using PostCalendarAPI.Services.JWService.Models;

namespace PostCalendarAPI.Services.JWService
{
    public class JWService : IDisposable
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

        public void Dispose()
        {
            _httpClient.Dispose();
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

        /// <summary>
        /// 分析excel表格数据流
        /// </summary>
        /// <param name="excelStream">excel文件流</param>
        /// <returns>包含的课程列表</returns>
        public static IEnumerable<Course> AnalysisExcel(Stream excelStream)
        {
            var courses = new List<Course>();
            var workbook = new HSSFWorkbook(excelStream);

            if(workbook != null)
            {
                ISheet sheet = workbook.GetSheetAt(0);

                int rows = sheet.LastRowNum;

                // 从第二列开始循环
                // 第一列是时间
                for(int column = 1; column <= 7; column ++)
                {
                    // 从第四行开始
                    // 最后一行是备注
                    for(int row = 4; row <= rows - 1; row++)
                    {
                        ICell? cell = sheet.GetRow(row).GetCell(column);
                        if(cell != null)
                        {
                            string content = cell.StringCellValue;
                            // 判断单元格是否为空
                            if(content.Length != 1)
                            {
                                // column在这里可以表示星期几
                                IEnumerable<Course> result = AnalyseSingleCell(content, column);
                                courses.AddRange(result);

                                // 一次课程按节数占据多个单元格
                                // 加上节数以避免重复读取
                                row += result.First().Length;
                            }
                        }
                    }
                }
            }

            return courses;
        }

        /// <summary>
        /// 解析单个单元格中的内容
        /// </summary>
        /// <param name="cellString">单元格字符串</param>
        /// <param name="dayOfWeek">单元格所在的星期几</param>
        /// <returns>课程列表</returns>
        /// <exception cref="JWAnalysisException">解析失败引发的异常</exception>
        private static IEnumerable<Course> AnalyseSingleCell(string cellString, int dayOfWeek)
        {
            // 这里注意
            // lines的第一行是一个空字符串
            // 5行文字有6行
            var lines = cellString.Split("\n");
            var courses = new List<Course>();

            switch(lines.Length)
            {
                case 6:
                    // 只有一门课程的单元格
                    // 没有分组
                    int[] weeks = Course.AnalyseWeekString(lines[3]);
                    int[] classes = Course.AnalyseTimeString(lines[5]);
                    var course = new Course(
                        lines[1],
                        lines[2],
                        lines[4],
                        weeks,
                        classes[0],
                        classes[1],
                        dayOfWeek
                        );
                    courses.Add(course);

                    break;
                case 7:
                    // 只有一门课程的单元格
                    // 含有分组
                    weeks = Course.AnalyseWeekString(lines[4]);
                    classes = Course.AnalyseTimeString(lines[6]);
                    course = new Course(
                        lines[1],
                        lines[3] + lines[2],// 老师和分组合并显示
                        lines[5],
                        weeks,
                        classes[0],
                        classes[1],
                        dayOfWeek
                        );
                    courses.Add(course);

                    break;
                case 11:
                    // 含有两门课程的单元格
                    // 均没有分组
                    weeks = Course.AnalyseWeekString(lines[3]);
                    classes = Course.AnalyseTimeString(lines[5]);
                    course = new Course(
                        lines[1],
                        lines[2],
                        lines[4],
                        weeks,
                        classes[0],
                        classes[1],
                        dayOfWeek
                        );
                    courses.Add(course);

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
                case 12:
                    // 含有两门课程的单元格
                    // 有一门课程有分组
                    // 通过第五行是否存在”节“来判断
                    if (lines[4].Contains('节'))
                    {
                        // 第一门没有分组
                        weeks = Course.AnalyseWeekString(lines[3]);
                        classes = Course.AnalyseTimeString(lines[5]);
                        course = new Course(
                            lines[1],
                            lines[2],
                            lines[4],
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
                        weeks = Course.AnalyseWeekString(lines[4]);
                        classes = Course.AnalyseTimeString(lines[6]);
                        course = new Course(
                            lines[1],
                            lines[3] + lines[2],// 老师和分组合并显示
                            lines[5],
                            weeks,
                            classes[0],
                            classes[1],
                            dayOfWeek
                            ); ;
                        courses.Add(course);

                        // 第二门没有分组
                        weeks = Course.AnalyseWeekString(lines[9]);
                        classes = Course.AnalyseTimeString(lines[11]);
                        course = new Course(
                            lines[7],
                            lines[8],
                            lines[10],
                            weeks,
                            classes[0],
                            classes[1],
                            dayOfWeek
                            );
                        courses.Add(course);

                        break;
                    }
                case 13:
                    // 含有两门课程的单元格
                    // 均有分组
                    weeks = Course.AnalyseWeekString(lines[4]);
                    classes = Course.AnalyseTimeString(lines[6]);
                    course = new Course(
                        lines[1],
                        lines[3] + lines[2],// 老师和分组合并显示
                        lines[5],
                        weeks,
                        classes[0],
                        classes[1],
                        dayOfWeek
                        );
                    courses.Add(course);

                    weeks = Course.AnalyseWeekString(lines[10]);
                    classes = Course.AnalyseTimeString(lines[12]);
                    course = new Course(
                        lines[7],
                        lines[9] + lines[8],// 老师和分组合并显示
                        lines[11],
                        weeks,
                        classes[0],
                        classes[1],
                        dayOfWeek
                        );
                    courses.Add(course);

                    break;
                default:
                    throw new JWAnalysisException($"解析单元格失败,含有{lines.Length}行, 内容为{cellString}");
            }

            return courses;
        }
    }
}
