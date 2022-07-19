using PostCalendarAPITools.Excel;

namespace PostCalendarAPITools
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\Users\ricardo.DESKTOP-N6OVBK5\Documents\office\execl\二大班全体成员信息.xlsx";
            ExcelReader reader = new ExcelReader(path);
        }
    }
}
