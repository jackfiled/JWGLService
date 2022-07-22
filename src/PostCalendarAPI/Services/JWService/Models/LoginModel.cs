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

        public IEnumerable<KeyValuePair<string, string>> keyValuePairs
        {
            get
            {
                return new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("userAccount", userAccount),
                    new KeyValuePair<string, string>("userPassword", userPassword),
                    new KeyValuePair<string, string>("encoded", encoded),
                };
            }
        }

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>输出字符串</returns>
        private static string EncodeInput(string input)
        {
            Span<byte> bytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes);
        }
    }
}
