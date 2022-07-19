using PostCalendarAPITools.Excel;
using PostCalendarAPITools.Network;

namespace PostCalendarAPITools
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string path = @"C:\Users\ricardo.DESKTOP-N6OVBK5\Documents\office\execl\二大班全体成员信息.xlsx";
            ExcelReader reader = new ExcelReader(path);

            APIHelper helper = new APIHelper();

            await helper.InitHelper();

            foreach(var item in reader.items)
            {
                await helper.AddUser(item);
            }
        }
    }
}
