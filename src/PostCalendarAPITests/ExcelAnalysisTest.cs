using JwglServices.Services.JWService.Models;
using JwglServices.Services.JWService;

namespace PostCalendarAPITests
{
    [TestClass]
    public class ExcelAnalysisTest
    {
        /// <summary>
        /// 测试识别上课的周数
        /// </summary>
        [TestMethod]
        public void AnalyseWeekStringTest1()
        {
            string input = "1-16[周]";
            var result = Course.AnalyseWeekString(input);

            for(int i = 1; i <= 16; i++)
            {
                Assert.IsTrue(result.Contains(i));
            }
        }

        /// <summary>
        /// 测试识别上课的周数
        /// </summary>
        [TestMethod]
        public void AnalyseWeekStringTest2()
        {
            string input = "1-10,14-16[周]";
            var result = Course.AnalyseWeekString(input);

            for(int i = 1; i <= 10; i++)
            {
                Assert.IsTrue(result.Contains(i));
            }

            for(int i = 14; i <= 16; i++)
            {
                Assert.IsTrue(result.Contains(i));
            }
        }

        [TestMethod]
        public void AnalyseWeekStringTest3()
        {
            // 不要笑
            // 这个单测来自真实文档
            string input = "7,9-10,12,14[周]";
            var result = Course.AnalyseWeekString(input);

            foreach(int number in new int[] { 7, 9, 10, 12, 14})    
            {
                Assert.IsTrue(result.Contains(number));
            }
        }

        [TestMethod]
        public void AnalyseTimeStringTest1()
        {
            string input = "[01-02]节";
            var result = Course.AnalyseTimeString(input);

            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(2, result[1]);
        }

        [TestMethod]
        public void AnalyseTimeStringTest2()
        {
            string input = "[06-07-08]节";
            var result = Course.AnalyseTimeString(input);

            Assert.AreEqual(6, result[0]);
            Assert.AreEqual(8, result[1]);
        }

        [TestMethod]
        public void AnalyseExcelTest1()
        {
            // 测试用excel表格甲
            // 一共有14次课程
            string path = @"C:\Users\ricardo.DESKTOP-N6OVBK5\Desktop\a.xls";

            using(var stream = File.OpenRead(path))
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);
                var result = JWService.AnalysisExcel(buffer);

                foreach(var item in result)
                {
                    Console.WriteLine(item.ToString());
                }

                Assert.AreEqual(14, result.Count());
            }
        }

        [TestMethod]
        public void AnalyseExcelTest2()
        {
            // 测试用excel表格乙
            // 一共有10次课程
            string path = @"C:\Users\ricardo.DESKTOP-N6OVBK5\Desktop\b.xls";

            using(var stream = File.OpenRead(path))
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);
                var result = JWService.AnalysisExcel(buffer);

                foreach (var item in result)
                {
                    Console.WriteLine(item.ToString());
                }

                Assert.AreEqual(10, result.Count());
            }
        }

        [TestMethod]
        public void GenerateICSStreamTest1()
        {
            // 测试用excel表格甲
            // 一共有14次课程
            string path = @"C:\Users\ricardo.DESKTOP-N6OVBK5\Desktop\a.xls";
            string outputPath = @"C:\Users\ricardo.DESKTOP-N6OVBK5\Desktop\a.ics";

            using (var file = File.OpenRead(path))
            {
                byte[] buffer = new byte[file.Length];
                file.Read(buffer, 0, (int)file.Length);

                var courses = JWService.AnalysisExcel(buffer);

                var beginTime = new DateTime(2022, 2, 28);

                using (var output = File.Create(outputPath))
                {
                    output.Write(JWService.GenerateICSStream(courses, beginTime));
                }
            }
        }

        [TestMethod]
        public void GenerateICSStreamTest2()
        {
            // 测试用excel表格甲
            // 一共有14次课程
            string path = @"C:\Users\ricardo.DESKTOP-N6OVBK5\Desktop\b.xls";
            string outputPath = @"C:\Users\ricardo.DESKTOP-N6OVBK5\Desktop\b.ics";

            using (var file = File.OpenRead(path))
            {
                byte[] buffer = new byte[file.Length];
                file.Read(buffer, 0, (int)file.Length);

                var courses = JWService.AnalysisExcel(buffer);

                var beginTime = new DateTime(2022, 8, 22);

                using (var output = File.Create(outputPath))
                {
                    output.Write(JWService.GenerateICSStream(courses, beginTime));
                }
            }
        }
    }
}
