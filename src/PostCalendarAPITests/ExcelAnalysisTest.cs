using PostCalendarAPI.Services.JWService.Models;


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


    }
}
