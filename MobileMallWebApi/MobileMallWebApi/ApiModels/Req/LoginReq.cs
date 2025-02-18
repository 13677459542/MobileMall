using System.ComponentModel.DataAnnotations;

namespace MobileMallWebApi.ApiModels.Req
{
    /// <summary>
    /// 用户登录
    /// </summary>
    public class LoginReq
    {
        /// <summary>
        /// 手机号
        /// </summary>
        [Required(ErrorMessage = "手机号是必填项")]
        [MinLength(11, ErrorMessage = "手机号长度不能少于11个字符")]
        public string mobile { get;set; }
        /// <summary>
        /// 验证码
        /// </summary>
        [Required(ErrorMessage = "验证码是必填项")]
        public string smsCode { get;set; }
    }
}
