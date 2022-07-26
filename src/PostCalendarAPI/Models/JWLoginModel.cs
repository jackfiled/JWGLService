#nullable disable

namespace PostCalendarAPI.Models
{
    /// <summary>
    /// 表示登录教务系统的模型
    /// </summary>
    public class JWLoginModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public string semester { get; set; }
    }
}
