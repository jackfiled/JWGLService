using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace PostCalendarAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        /// <summary>
        /// 不需验证即可访问的API
        /// </summary>
        /// <returns>字符串数组</returns>
        [HttpGet("value1")]
        public ActionResult<IEnumerable<String>> GetValue1()
        {
            return new String[] {"value1"};
        }

        /// <summary>
        /// 需要验证才可访问的API
        /// </summary>
        /// <returns>字符串数组</returns>
        [HttpGet("value2")]
        [Authorize]
        public ActionResult<IEnumerable<String>> GetValue2()
        {
            return new String[] { "value2" };
        }
    }
}
