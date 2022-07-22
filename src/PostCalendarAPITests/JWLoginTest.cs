using PostCalendarAPI.Services.JWService;
using PostCalendarAPI.Services.JWService.Models;
using Moq;

namespace PostCalendarAPITests
{
    [TestClass]
    public class JWLoginTest
    {
        /// <summary>
        /// 测试base64加密是否成功
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
    }
}