using PostCalendarAPI.Services.JWService;
using PostCalendarAPI.Services.JWService.Models;
using Moq;

namespace PostCalendarAPITests
{
    [TestClass]
    public class JWLoginTest
    {
        /// <summary>
        /// ����base64�����Ƿ�ɹ�
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestBase64()
        {
            var loginModel = new LoginModel("2021211180", "2021211180");

            HttpContent content = new FormUrlEncodedContent(loginModel.keyValuePairs);

            string result = await content.ReadAsStringAsync();

            Assert.AreEqual("userAccount=2021211180&userPassword=&encoded=MjAyMTIxMTE4MA%3D%3D%25%25%25MjAyMTIxMTE4MA%3D%3D", result);
        }

        [TestMethod]
        public async Task TestLogin()
        {
            var mockLogger = new Mock<ILogger>();
            var mockHttpClient = new Mock<IHttpClientFactory>();
            var service = new JWService(mockLogger.Object, mockHttpClient.Object);

            PrintCookies(service.cookies);

            await service.Login("", "");

            PrintCookies(service.cookies);

            var result = await service.CheckLogin();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task TestDownload()
        {
            var mockLogger = new Mock<ILogger>();
            var mockHttpClient = new Mock<IHttpClientFactory>();
            var service = new JWService(mockLogger.Object, mockHttpClient.Object);

            await service.Login("", "");

            await service.GetSemester("2022-2023-1");
        }

        private void PrintCookies(CookieContainer container)
        {
            foreach(Cookie cookie in container.GetAllCookies())
            {
                Console.WriteLine(cookie.Name + "=" + cookie.Value);
            }
        }
    }
}