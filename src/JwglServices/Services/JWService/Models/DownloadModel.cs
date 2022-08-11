namespace JwglServices.Services.JWService.Models
{
    public class DownloadModel
    {
        public string xnxq01id { get; set; }
        public string zc { get; set; }
        public string kbjcmsid { get; set; }

        public DownloadModel(string semester)
        {
            xnxq01id = semester;
            zc = "";
            kbjcmsid = "9475847A3F3033D1E05377B5030AA94D";
        }

        /// <summary>
        /// 获取下载连接
        /// </summary>
        /// <returns>下载连接</returns>
        public string GetDownloadUri()
        {
            return $"xskb/xskb_print.do?xnxq01id={xnxq01id}&zc={zc}&kbjcmsid={kbjcmsid}";
        }
    }
}
