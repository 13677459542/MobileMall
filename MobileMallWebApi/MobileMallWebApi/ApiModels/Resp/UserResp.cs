using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MobileMallWebApi.ApiModels.Resp
{
    /// <summary>
    /// 用户
    /// </summary>
    public class UserResp
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        public UserInfo userInfo { get; set; }
        /// <summary>
        /// 用户信息实体
        /// </summary>
        public class UserInfo
        {
            /// <summary>
            /// 用户id
            /// </summary>
            public int user_id { get; set; }
            /// <summary>
            /// 手机号
            /// </summary>
            public string mobile { get; set; }
            /// <summary>
            /// 用户名称
            /// </summary>
            public string nick_name { get; set; }
            /// <summary>
            /// 性别
            /// </summary>
            public string gender { get; set; }
            /// <summary>
            /// 头像地址
            /// </summary>
            public string avatar_url { get; set; }
            /// <summary>
            /// 省
            /// </summary>
            public string? province { get; set; }
            /// <summary>
            /// 市
            /// </summary>
            public string? city { get; set; }
            /// <summary>
            /// 区
            /// </summary>
            public string? district { get; set; }
            /// <summary>
            /// 余额
            /// </summary>
            public decimal balance { get; set; }
            /// <summary>
            /// 已支付金额
            /// </summary>
            public decimal pay_money { get; set; }
            /// <summary>
            /// 最后登录时间
            /// </summary>
            public DateTime last_login_time { get; set; }
        }
    }
}
