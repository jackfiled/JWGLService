using System.Text.RegularExpressions;

namespace PostCalendarAPI.Services.JWService.Models
{
    /// <summary>
    ///  表示学期中的每一个课的模型
    /// </summary>
    public class Course
    {
        /// <summary>
        /// 课程的名称
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 任课教师
        /// </summary>
        public string Teacher { get; }
        /// <summary>
        /// 上课的地点
        /// </summary>
        public string Place { get; }
        /// <summary>
        /// 上课的周数
        /// </summary>
        public int[] Weeks { get; }
        /// <summary>
        /// 课程的开始时间
        /// </summary>
        public TimeOnly BeginTime { get; }
        /// <summary>
        /// 课程的结束时间
        /// </summary>
        public TimeOnly EndTime { get; }
        /// <summary>
        /// 课程所在的星期
        /// </summary>
        public int DayOfWeek { get; }

        /// <summary>
        /// 创建课程对象
        /// </summary>
        /// <param name="name">课程的名称</param>
        /// <param name="teacher">任课教师</param>
        /// <param name="place">上课地点</param>
        /// <param name="weeks">上课的周数</param>
        /// <param name="beginClass">开始的节次</param>
        /// <param name="endClass">结束的节次</param>
        /// <param name="dayOfWeek">星期几</param>

        public Course(
            string name,
            string teacher,
            string place,
            int[] weeks,
            int beginClass,
            int endClass,
            int dayOfWeek
            )
        {
            Name = name;
            Teacher = teacher;
            Place = place;

            Weeks = weeks;

            BeginTime = BeginTimeList[beginClass - 1];
            EndTime = BeginTimeList[endClass - 1].AddMinutes(ClassLength);

            DayOfWeek = dayOfWeek;
        }

        /// <summary>
        /// 解析周数字符串
        /// </summary>
        /// <param name="weeks">表示周数的字符串</param>
        /// <returns>周数数组</returns>
        public static int[] AnalyseWeekString(string weeks)
        {
            Regex pattern = new Regex(@"^(\d+)-(\d+).*");
            List<int> ints = new List<int>();

            // 先按逗号切分一下
            // 再匹配每一组
            var lines = weeks.Split(",");
            for(int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                Match m = pattern.Match(line);
                if(m.Success)
                {
                    int begin = int.Parse(m.Groups[1].Value);
                    int end = int.Parse(m.Groups[2].Value);

                    for(int j = begin; j <= end; j++)
                    {
                        ints.Add(j);
                    }
                }
            }

            return ints.ToArray();
        }

        public static int[] AnalyseTimeString(string times)
        {
            Regex pattern = new Regex(@"\[(.*)\]节");

            Match match = pattern.Match(times);

            if(match.Success)
            {
                // 把[]中的字符串按-切分
                // 去头尾的数据
                var numbers = match.Groups[1].Value.Split("-");

                return new int[]
                {
                    int.Parse(numbers.First()),
                    int.Parse(numbers.Last())
                };
            }
            else
            {
                throw new JWAnalysisException();
            }
        }


        private static TimeOnly[] BeginTimeList = new TimeOnly[]
        {
            new TimeOnly(8, 0),
            new TimeOnly(8, 50),
            new TimeOnly(9, 50),
            new TimeOnly(10, 40),
            new TimeOnly(11, 30),
            new TimeOnly(13, 0),
            new TimeOnly(13, 50),
            new TimeOnly(14, 45),
            new TimeOnly(15, 40),
            new TimeOnly(16, 35),
            new TimeOnly(17, 25),
            new TimeOnly(18, 35),
            new TimeOnly(19, 20),
            new TimeOnly(20, 10),
        };

        private static readonly int ClassLength = 45;
    }
}
