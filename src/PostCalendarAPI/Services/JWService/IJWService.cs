using PostCalendarAPI.Services.JWService.Models;

namespace PostCalendarAPI.Services.JWService
{
    /// <summary>
    /// 教务系统服务提供的接口
    /// </summary>
    public interface IJWService
    {
        public Task<bool> Login(string UserName, string Password);

        public Task GetSemester(string SemesterName);

        public Task<Stream?> GetICSStream();

        public Task<List<Course>?> GetCourses();
    }
}
