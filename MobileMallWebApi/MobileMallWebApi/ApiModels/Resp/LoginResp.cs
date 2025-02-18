namespace MobileMallWebApi.ApiModels.Resp
{
    /// <summary>
    /// 用户登录
    /// </summary>
    public class LoginResp
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int? userId { get; set; }
        /// <summary>
        /// token
        /// </summary>
        public string? token { get; set; }
    }
}
