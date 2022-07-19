using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace PostCalendarAPITools.Excel
{
    public class ExcelReader
    {
        private readonly string _excelPath;
        public List<ExcelModel> items;

        public ExcelReader(string path)
        {
            _excelPath = path;
            items = new List<ExcelModel>();

            ReadExcel();
        }

        private void ReadExcel()
        {
            items.Clear();

            var app = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbooks workbooks = app.Workbooks;
            workbooks.Open(_excelPath);

            Microsoft.Office.Interop.Excel.Worksheet worksheet = app.Worksheets[1];
            int rows = worksheet.UsedRange.Rows.Count;

            // 在Excel里面数数是从1开始的
            for(int i = 2; i <= rows; i++)
            {
                String[] row = new String[3];
                for(int j = 1; j <= 3; j++)
                {
                    Microsoft.Office.Interop.Excel.Range range = worksheet.Range[app.Cells[i, j], app.Cells[i, j]];
                    range.Select();
                    row[j - 1] = app.ActiveCell.Text.ToString();
                }

                // 第一列是班级
                int classNumber = int.Parse(row[0]);
                // 第二列是学号
                int studentID = int.Parse(row[1]);
                // 第三列是名字，不做转换

                items.Add(new ExcelModel(row[2], classNumber, studentID));
            }

            workbooks.Close();
            app.Quit();

        }
    }
}
