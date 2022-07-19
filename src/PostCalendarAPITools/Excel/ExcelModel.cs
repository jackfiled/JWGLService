using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostCalendarAPITools.Excel
{
    public class ExcelModel
    {
        public string UserName { get; set; }
        public int StudentID { get; set; }
        public int ClassNumber { get; set; }

        public ExcelModel(string name, int studentID, int classNumber)
        {
            UserName = name;
            StudentID = studentID;
            ClassNumber = classNumber;
        }

        public override String ToString()
        {
            return UserName + "-" + StudentID.ToString() + "-" + ClassNumber.ToString();
        }
    }
}
