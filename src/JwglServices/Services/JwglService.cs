using Grpc.Core;
using JwglServices.Services.JWService;

namespace JwglServices.Services
{
    public class JwglService : Jwgler.JwglerBase
    {
        private readonly IJWService _jwService;

        // 依赖注入教务系统服务
        public JwglService(IJWService jwService)
        {
            _jwService = jwService;
        }

        public override async Task<GetSemesterResponse> GetSemester(GetSemesterRequest request, ServerCallContext context)
        {
            string username = request.StudengID;
            string password = request.Password;
            string semester = request.Semester;

            bool loginResult = await _jwService.Login(username, password);

            if (!loginResult)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "The studentID or password is wrong"));
            }

            await _jwService.GetSemester(semester);

            var courses = _jwService.GetCourses();
            var icsStream = await _jwService.GetICSStream();

            if (courses == null || icsStream == null)
            {
                throw new RpcException(new Status(StatusCode.Internal, "The courses is invalid"));
            }
            else
            {
                var response = new GetSemesterResponse();
                
                foreach(var course in courses)
                {
                    response.Courses.Add(ConvertInternalCourse2RpcCourse(course));
                }

                response.IcsStream = Google.Protobuf.ByteString.CopyFrom(icsStream);
                
                return response;
            }
        }

        /// <summary>
        /// 将内部定义的课程消息类转换为RPC中定义的课程类
        /// </summary>
        /// <param name="courseInfo">需要转换的内部课程信息类</param>
        /// <returns>RPC中定义的课程类</returns>
        private Course ConvertInternalCourse2RpcCourse(Models.CourseInfo courseInfo)
        {
            var course = new Course();
            
            course.Name = courseInfo.Name;
            course.Place = courseInfo.Place;
            course.Teacher = courseInfo.Teacher;
            course.BeginTimeString = courseInfo.BeginTimeString;
            course.EndTimeString = courseInfo.EndTimeString;
            course.Weeks.Clear();
            course.Weeks.AddRange(courseInfo.Weeks);
            course.DayOfWeek = courseInfo.DayOfWeek;

            return course;
        }
    }
}
