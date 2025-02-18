namespace MobileMallWebApi.ApiModels.Resp
{
    /// <summary>
    /// 获取手机验证码
    /// </summary>
    public class SendSmsCaptchaResp
    {
        /// <summary>
        /// 手机验证码
        /// </summary>
        public string? SmsCaptchaCode { get; set; }
    }
}
