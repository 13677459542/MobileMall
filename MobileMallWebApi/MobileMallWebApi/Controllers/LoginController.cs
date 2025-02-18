using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using MobileMallWebApi.ApiModels.Req;
using MobileMallWebApi.ApiModels.Resp;
using MobileMallWebApi.Utils.Other;
using System.Drawing.Imaging;
using System.Drawing;
using System.Collections;
using MobileMallWebApi.Utils;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Asn1.Pkcs;
using MobileMallDB.BusinessInterface;
using MobileMallDB.BusinessService;
using MobileMallDB.Models;
using System.Drawing.Printing;
using System.Linq.Expressions;
using Microsoft.IdentityModel.Tokens;
using MobileMallDB;
using MobileMallWebApi.Utils.Authorization;
using System.IdentityModel.Tokens.Jwt;
using MobileMallWebApi.Utils.Model;

namespace MobileMallWebApi.Controllers
{
    /// <summary>
    /// 登录页
    /// </summary>
    [Route("Api/V1/[controller]/[action]")]
    //[Authorize]
    [AllowAnonymous]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly IBaseService BaseServiceClient;
        private readonly JwtHelper _jwtHelper;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="baseService"></param>
        /// <param name="jwtHelper"></param>
        public LoginController(IBaseService baseService, JwtHelper jwtHelper)
        {
            this.BaseServiceClient = baseService;
            _jwtHelper = jwtHelper;
        }

        /// <summary>
        /// 获取图形验证码
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BaseResp<CaptchaResp>>> GetCaptcha()
        {
            #region 生成图形验证码方案一
            string Number = CaptchaUtils.GenerateRandomText(4);
            var CaptchaData = CaptchaUtils.GetCaptchaImage(208, 65, Number);
            #endregion
            #region 生成图形验证码方案二
            //string captchaText = CaptchaUtils.GenerateRandomText(4); // 生成4位随机验证码
            //Bitmap captchaImage = CaptchaUtils.GenerateCaptchaImage(captchaText);
            //string filePath = "captcha.png";
            //captchaImage.Save(filePath, ImageFormat.Png);
            #endregion
            //string RespStr = @"{
            //    ""status"": 200,
            //    ""message"": ""success"",
            //    ""data"": {
            //        ""base64"": """",
            //        ""md5"": ""a7ae0cbf2a6ec6195047b05f797dcc64""
            //    }
            //}";
            //BaseResp<CaptchaResp> Resp = JsonConvert.DeserializeObject<BaseResp<CaptchaResp>>(RespStr);

            // 将哈希值转换为十六进制字符串
            StringBuilder MD5Data = new StringBuilder();
            // 创建一个 MD5 对象
            using (MD5 md5 = MD5.Create())
            {
                // 将输入字符串转换为字节数组并计算哈希值
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(Number));
                foreach (byte b in hashBytes)
                    MD5Data.Append(b.ToString("x2")); // 转换为小写十六进制
            }
            bool Result = await BaseServiceClient.Insert(new Captcha()
            {
                key = Guid.NewGuid().ToString(),
                md5 = MD5Data.ToString(),
                CaptchaCode = Number,
                create_time = DateTime.Now,
            });
            if (Result)
                return Ok(await ApiResult.SetSuccess("success", new CaptchaResp()
                {
                    base64 = "data:image/png;base64," + CaptchaData,
                    key = MD5Data.ToString(),
                    md5 = MD5Data.ToString()
                }));
            else
                return Ok(await ApiResult.SetFailure("请求二维码异常", new CaptchaResp()));
        }

        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BaseResp<SendSmsCaptchaResp>>> SendSmsCaptcha([FromBody] SendSmsCaptchaReq req)
        {
            //string RespStr = @"{""status"":200,""message"":""^_^小智提示: 测试环境验证码为: 246810"",""data"":[]}";
            //SendSmsCaptchaResp Resp = JsonConvert.DeserializeObject<SendSmsCaptchaResp>(RespStr);

            Expression<Func<Captcha, bool>> funcWhere = p => true;
            funcWhere = funcWhere.And(p => p.md5.Equals(req.captchaKey));
            var Result = await BaseServiceClient.Query(funcWhere);
            if (Result.Count < 1 || DateTime.Now.Subtract(Result.OrderByDescending(x => x.create_time).FirstOrDefault().create_time).TotalSeconds > 60)
                return Ok(await ApiResult.SetFailure("图形验证码已过期，请重新生成！", new SendSmsCaptchaResp()));
            if (Result.OrderByDescending(x => x.create_time)?.FirstOrDefault()?.CaptchaCode != req.captchaCode)
                return Ok(await ApiResult.SetFailure("图形验证码输入错误！", new SendSmsCaptchaResp()));
            return Ok(await ApiResult.SetSuccess("测试环境验证码为: 123456", new SendSmsCaptchaResp()
            {
                SmsCaptchaCode = "123456"
            }));
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BaseResp<LoginResp>>> Login([FromBody] LoginReq req)
        {
            //            string RespStr = @"{
            //    ""status"": 200,
            //    ""message"": ""登录成功"",
            //    ""data"": {
            //        ""userId"": 27191,
            //        ""token"": ""04cf255cd6bea5544989aeb5b0b98f59""
            //    }
            //}";
            //            LoginResp Resp = JsonConvert.DeserializeObject<LoginResp>(RespStr);
            if (req?.smsCode != "123456")
                return Ok(await ApiResult.SetFailure("验证码输入错误！", new LoginResp()));
            Expression<Func<User, bool>> funcWhere = p => true;
            funcWhere = funcWhere.And(p => p.mobile.Equals(req.mobile));
            var GetUser = await BaseServiceClient.Query(funcWhere);
            TokenModelJwt model = new TokenModelJwt()
            {
                //UserId = 1,
                Role = "user",//可以同时赋值多个角色
                UserName = req.mobile,
                NickName = req.mobile,
                Description = "普通用户获取令牌",
            };
            if (GetUser?.Count > 0)
            {
                var UserInfo = GetUser.First();
                UserInfo.last_login_time = DateTime.Now;
                await BaseServiceClient.Update(UserInfo);

                model.UserId = UserInfo.user_id;
                return Ok(await ApiResult.SetSuccess("登录成功", new LoginResp()
                {
                    userId = model.UserId,
                    token = _jwtHelper.CreateToken(model)
                }));
            }
            else
            {
                User user = new User()
                {
                    mobile = req.mobile,
                    nick_name = MethodTool.MaskPhoneNumber(req.mobile),
                    gender = 1,
                    balance = 0.00m,
                    pay_money = 0.00m,
                    last_login_time = DateTime.Now
                };
                bool Result = await BaseServiceClient.Insert(user);
                if (Result)
                {
                    model.UserId = user.user_id;
                    return Ok(await ApiResult.SetSuccess("登录成功", new LoginResp()
                    {
                        userId = model.UserId,
                        token = _jwtHelper.CreateToken(model)
                    }));
                }
                else
                    return Ok(await ApiResult.SetFailure("登录失败，请稍后重新尝试！", new LoginResp()));
            }

        }
    }
}
