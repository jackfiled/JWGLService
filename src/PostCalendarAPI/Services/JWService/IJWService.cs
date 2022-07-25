using PostCalendarAPI.Models;

namespace PostCalendarAPI.Services.JWService
{
    /// <summary>
    /// 教务系统服务提供的接口
    /// </summary>
    public interface IJWService
    {
        /// <summary>
        /// 登录教务处
        /// </summary>
        /// <param name="UserName">学号</param>
        /// <param name="Password">密码</param>
        /// <returns>登录是否成功</returns>
        public Task<bool> Login(string UserName, string Password);

        /// <summary>
        /// 获取课表
        /// </summary>
        /// <param name="SemesterName">需要获取的学期</param>
        /// <returns></returns>
        public Task GetSemester(string SemesterName);

        /// <summary>
        /// 获取ICS文件流
        /// </summary>
        /// <returns>ICS流</returns>
        public Task<byte[]?> GetICSStream();

        /// <summary>
        /// 获取课程列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CourseInfo>? GetCourses();
    }
}
