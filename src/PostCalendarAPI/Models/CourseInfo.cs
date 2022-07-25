using PostCalendarAPI.Services.JWService.Models;

namespace PostCalendarAPI.Models
{
    public class CourseInfo
    {
        public string Name { get; set; }
        public string Teacher { get; set; }
        public string Place { get; set; }
        public int[] Weeks { get; set; }
        public string BeginTimeString { get; set; }
        public string EndTimeString { get; set; }
        public int DayOfWeek { get; set; }

        public CourseInfo(Course course)
        {
            Name = course.Name;
            Teacher = course.Teacher;
            Place = course.Place;
            Weeks = course.Weeks;
            BeginTimeString = course.BeginTime.ToString();
            EndTimeString = course.EndTime.ToString();
            DayOfWeek = course.DayOfWeek;
        }
    }
}
