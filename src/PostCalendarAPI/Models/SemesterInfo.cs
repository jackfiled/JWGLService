namespace PostCalendarAPI.Models
{
    public class SemesterInfo
    {
        /// <summary>
        /// 数据库中的编号
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 学期
        /// </summary>
        public string Semester { get; set; }
        /// <summary>
        /// 学期开始的时间
        /// </summary>
        public string BeginDateTimeString { get; set; }

        public SemesterInfo(int id, string semester, DateTime beginDateTime)
        {
            ID = id;
            Semester = semester;
            BeginDateTimeString = beginDateTime.ToString();
        }
        
    }
}
