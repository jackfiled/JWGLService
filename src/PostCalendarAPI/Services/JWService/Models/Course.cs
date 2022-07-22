namespace PostCalendarAPI.Services.JWService.Models
{
#nullable disable

    /// <summary>
    ///  表示学期中的每一个课的模型
    /// </summary>
    internal class Course
    {
        /// <summary>
        /// 课程的名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 任课教师
        /// </summary>
        public string Teacher { get; set; }
        /// <summary>
        /// 上课的地点
        /// </summary>
        public string Place { get; set; }
        /// <summary>
        /// 上课的周数
        /// </summary>
        public int[] Weeks { get; set; }
        /// <summary>
        /// 课程的开始时间
        /// </summary>
        public TimeOnly BeginTime { get; set; }
        /// <summary>
        /// 课程的结束时间
        /// </summary>
        public TimeOnly EndTime { get; set; }
        /// <summary>
        /// 课程所在的星期
        /// </summary>
        public DayOfWeek DayOfWeek { get; set; }
    }
}
