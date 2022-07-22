using System.Text;
using System.Buffers.Text;

namespace PostCalendarAPI.Services.JWService.Models
{
    public class LoginModel
    {
        public string userAccount { get; set; }

        public string userPassword { get; set; }

        public string encoded { get; set; }

        public LoginModel(string studentID, string password)
        {
            userAccount = studentID;
            userPassword = "";
            encoded = EncodeInput(studentID) + "%%%" + EncodeInput(password);
        }

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>输出字符串</returns>
        private static string EncodeInput(string input)
        {
            Span<byte> bytes = Encoding.UTF8.GetBytes(input);
            Base64.EncodeToUtf8InPlace(bytes, bytes.Length, out _);

            return Encoding.UTF8.GetString(bytes);
        }
    }
}
