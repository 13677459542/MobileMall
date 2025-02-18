using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using MobileMallWebApi.ApiModels.Req;
using MobileMallWebApi.ApiModels.Resp;
using MobileMallDB.BusinessInterface;
using MobileMallWebApi.Utils.Authorization;
using MobileMallWebApi.Utils;
using Microsoft.EntityFrameworkCore;
using MobileMallWebApi.Utils.Other;

namespace MobileMallWebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("Api/V1/[controller]/[action]")]
    [Authorize]
    //[AllowAnonymous]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IBaseService BaseServiceClient;
        private readonly JwtHelper _jwtHelper;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="baseService"></param>
        /// <param name="jwtHelper"></param>
        public UserController(IBaseService baseService, JwtHelper jwtHelper)
        {
            this.BaseServiceClient = baseService;
            _jwtHelper = jwtHelper;
        }

        /// <summary>
        /// 获取个人信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BaseResp<UserResp>>> GetUserInfo()
        {
            System.Security.Cryptography.Aes.Create();
            //解析token
            string? token = this.Request.Headers["Authorization"];
            var UserToken = _jwtHelper.SerializeJwt(token);

            UserResp userResp = new UserResp();
            userResp.userInfo = await BaseServiceClient.GetContext().Users.Where(x => x.user_id == UserToken.UserId).Select(y => new UserResp.UserInfo()
            {
                user_id = y.user_id,
                mobile = MethodTool.MaskPhoneNumber(y.mobile),
                balance = y.balance,
                avatar_url = y.avatar_url,
                province = y.province,
                city = y.city,
                district = y.district,
                gender = (y.gender == 1 ? "男" : (y.gender == 2 ? "女" : "未知")),
                nick_name = y.nick_name,
                pay_money = y.pay_money,
                last_login_time = y.last_login_time,
            }).FirstAsync();
            if (userResp.userInfo == null)
                return Ok(await ApiResult.SetFailure("未获取到您的用户信息，请尝试重新登录！", userResp));
            //return await ApiResult.SetSuccess("success", userResp);
            return Ok(await ApiResult.SetSuccess("success", userResp));
        }
    }
}
