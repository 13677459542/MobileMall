using System.ComponentModel.DataAnnotations;

namespace MobileMallWebApi.ApiModels.Req
{
    /// <summary>
    /// 获取手机验证码
    /// </summary>
    public class SendSmsCaptchaReq
    {
        /// <summary>
        /// 用户输入的图形验证码
        /// </summary>
        [Required(ErrorMessage = "图形验证码是必填项")]
        public string captchaCode { get; set; }
        /// <summary>
        /// 图形验证码唯一标识
        /// </summary>
        [Required(ErrorMessage = "图形验证码唯一标识是必填项")]
        public string captchaKey { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [Required(ErrorMessage = "手机号是必填项")]
        [MinLength(11, ErrorMessage = "手机号长度不能少于11个字符")]
        public string mobile { get; set; }
    }
}
