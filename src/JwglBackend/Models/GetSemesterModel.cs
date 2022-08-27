#nullable disable

using Microsoft.AspNetCore.Mvc;

namespace JwglBackend.Models
{
    public class GetSemesterModel
    {
        [ModelBinder(Name = "student_id")]
        public string StudentID { get; set; }

        [ModelBinder(Name = "password")]
        public string Password { get; set; }

        [ModelBinder(Name = "semester")]
        public string Semester { get; set; }
    }
}
