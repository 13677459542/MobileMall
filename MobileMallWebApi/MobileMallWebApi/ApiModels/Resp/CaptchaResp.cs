namespace MobileMallWebApi.ApiModels.Resp
{
    /// <summary>
    /// 获取图形验证码
    /// </summary>
    public class CaptchaResp
    {
        /// <summary>
        /// 验证码图片base64数据
        /// </summary>
        public string? base64 { get; set; }
        /// <summary>
        /// 唯一键
        /// </summary>
        public string? key { get; set; }
        /// <summary>
        /// 值的md5加密数据
        /// </summary>
        public string? md5 { get; set; }
    }
}
