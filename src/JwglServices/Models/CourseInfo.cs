namespace JwglServices.Models
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

        public CourseInfo(Services.JWService.Models.Course course)
        {
            Name = course.Name;
            Teacher = course.Teacher;
            Place = course.Place;
            Weeks = course.Weeks;
            BeginTimeString = course.BeginTime.ToString("HH:mm");
            EndTimeString = course.EndTime.ToString("HH:mm");
            DayOfWeek = course.DayOfWeek;
        }
    }
}
