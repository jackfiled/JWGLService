using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using PostCalendarAPI.Models;
using PostCalendarAPI.Services.JWService.Models;

namespace PostCalendarAPI.Services.JWService
{
    public class JWService : IDisposable, IJWService
    {
        public CookieContainer cookies = new CookieContainer();

        private byte[]? excelBytes;
        private string? targetSemester;
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly SemesterInfoContext _context;
        private static readonly string _baseUrl = "https://jwgl.bupt.edu.cn/jsxsd/";

        public JWService(ILogger<JWService> logger, SemesterInfoContext context)
        {
            _logger = logger;
            _context = context;

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

        public async Task<bool> Login(string studentID, string password)
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
            request.Headers.Referrer = new Uri(_baseUrl);

            await _httpClient.SendAsync(request);

            return await CheckLogin();
        }

        public async Task GetSemester(string semester)
        {
            targetSemester = semester;
            var downloadModel = new DownloadModel(semester);

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(_baseUrl + downloadModel.GetDownloadUri()),
                Method = HttpMethod.Post,
                Content = JsonContent.Create(downloadModel, typeof(DownloadModel))
            };

            var response = await _httpClient.SendAsync(request);
            
            excelBytes = await response.Content.ReadAsByteArrayAsync();
        }

        public IEnumerable<CourseInfo>? GetCourses()
        {
            if(excelBytes == null)
            {
                return null;
            }
            else
            {
                var courses = AnalysisExcel(excelBytes);
                var infos = new List<CourseInfo>();

                foreach(var course in courses)
                {
                    infos.Add(new CourseInfo(course));
                }

                return infos;
            }
        }

        public async Task<byte[]?> GetICSStream()
        {
            if(excelBytes == null)
            {
                return null;
            }
            else
            {
                var courses = await Task.Run(() => AnalysisExcel(excelBytes));
                return await Task.Run(() => GenerateICSStream(courses, GetSemesterBeginTime()));
            }
        }

        /// <summary>
        /// 检验是否登录
        /// </summary>
        /// <returns></returns>
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
        public static IEnumerable<Course> AnalysisExcel(byte[] bytes)
        {
            var courses = new List<Course>();
            
            using(MemoryStream stream = new MemoryStream(bytes))
            {
                var workbook = new HSSFWorkbook(stream);

                if (workbook != null)
                {
                    ISheet sheet = workbook.GetSheetAt(0);

                    int rows = sheet.LastRowNum;

                    // 从第二列开始循环
                    // 第一列是时间
                    for (int column = 1; column <= 7; column++)
                    {
                        // 从第四行开始
                        // 最后一行是备注
                        for (int row = 4; row <= rows - 1; row++)
                        {
                            ICell? cell = sheet.GetRow(row).GetCell(column);
                            if (cell != null)
                            {
                                string content = cell.StringCellValue;
                                // 判断单元格是否为空
                                if (content.Length != 1)
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
            }

            return courses;
        }

        /// <summary>
        /// 产生ICS文件流
        /// </summary>
        /// <param name="courses">课程列表</param>
        /// <param name="semesterBeginTime">学期开始时间</param>
        /// <returns>ICS文件字节数组</returns>
        public static byte[] GenerateICSStream(IEnumerable<Course> courses, DateTime semesterBeginTime)
        {
            StringBuilder builder = new StringBuilder();
            string timePattern = "yyyyMMddTHHmmss";
            string timeUtcPattern = "yyyyMMddTHHmmssZ";

            builder.AppendLine("BEGIN:VCALENDAR");
            builder.AppendLine("VERSION:2.0");

            foreach(var course in courses)
            {
                foreach(int week in course.Weeks)
                {
                    // 事件开始
                    builder.AppendLine("BEGIN:VEVENT");
                    // 创建事件的时间
                    builder.AppendLine($"DTSTAMP:{DateTime.UtcNow.ToString(timeUtcPattern)}");
                    // 创建GUID
                    builder.AppendLine($"UID:{Guid.NewGuid().ToString()}");
                    // 事件的概述
                    builder.AppendLine($"SUMMARY:{course.Name}");
                    // 事件的地点
                    builder.AppendLine($"LOCATION:{course.Place}");
                    // 事件的详情
                    builder.AppendLine($"DESCRIPTION:{course.Teacher}");
                    // 事件的开始时间
                    var beginTime = semesterBeginTime.AddDays(7 * (week - 1) + course.DayOfWeek - 1);
                    beginTime = beginTime.Add(course.BeginTime - TimeOnly.MinValue);
                    builder.AppendLine($"DTSTART;TZID=Asia/Shanghai:{beginTime.ToString(timePattern)}");
                    // 事件的结束时间
                    var endTime = semesterBeginTime.AddDays(7 * (week - 1) + course.DayOfWeek - 1);
                    endTime = endTime.Add(course.EndTime - TimeOnly.MinValue);
                    builder.AppendLine($"DTEND;TZID=Asia/Shanghai:{endTime.ToString(timePattern)}");

                    // 创建一个15分钟前的提醒
                    builder.AppendLine("BEGIN:VALARM");
                    builder.AppendLine("UID:" + Guid.NewGuid().ToString());
                    builder.AppendLine("ACTION:DISPLAY");
                    builder.AppendLine("TRIGGER:-PT15M");
                    builder.AppendLine("END:VALARM");

                    // 结束事件
                    builder.AppendLine("END:VEVENT");
                }
            }

            builder.AppendLine("END:VCALENDAR");

            string result = builder.ToString();
            return Encoding.UTF8.GetBytes(result);
        }

        /// <summary>
        /// 初始化
        /// </summary>
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

            HttpResponseMessage response = _httpClient.Send(request);
            if(response.IsSuccessStatusCode)
            {
                _logger.LogInformation("JWservice init successful");
            }
            else
            {
                _logger.LogWarning("JWservice init failed, status code {response.StatusCode}", 
                    response.StatusCode);
            }
        }

        /// <summary>
        /// 获取每学期行课第一天的时间
        /// </summary>
        /// <returns></returns>
        private DateTime GetSemesterBeginTime()
        {
            if(targetSemester != null)
            {
                var info = _context.Semesters.SingleOrDefault(s => s.Semester == targetSemester);
                if(info == default)
                {
                    throw new JWAnalysisException("The semester is invalid");
                }
                else
                {
                    return DateTime.Parse(info.BeginDateTimeString!);
                }
            }
            else
            {
                throw new JWAnalysisException("The semster is invalid");
            }
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
