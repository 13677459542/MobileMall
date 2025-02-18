using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace MobileMallDB.Models
{
    /// <summary>
    /// 用户
    /// </summary>
    public class User
    {
        /// <summary>
        /// 用户id
        /// </summary>
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int user_id { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [Required]
        [StringLength(20)] // 限制字符串的最大长度
        [Column(TypeName = "VARCHAR(20)")] // 指定数据库中的列类型
        public string mobile { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        [Required]
        [StringLength(256)] // 限制字符串的最大长度
        [Column(TypeName = "VARCHAR(256)")] // 指定数据库中的列类型
        public string nick_name { get; set; }
        /// <summary>
        /// 性别  1:男 、2：女、其他：未知
        /// </summary>
        [Required]
        public int gender { get; set; }
        /// <summary>
        /// 头像地址
        /// </summary>
        [StringLength(256)] // 限制字符串的最大长度
        [Column(TypeName = "VARCHAR(256)")] // 指定数据库中的列类型
        public string? avatar_url { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        [StringLength(256)] // 限制字符串的最大长度
        [Column(TypeName = "VARCHAR(256)")] // 指定数据库中的列类型
        public string? province { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        [StringLength(256)] // 限制字符串的最大长度
        [Column(TypeName = "VARCHAR(256)")] // 指定数据库中的列类型
        public string? city { get; set; }
        /// <summary>
        /// 区
        /// </summary>
        [StringLength(256)] // 限制字符串的最大长度
        [Column(TypeName = "VARCHAR(256)")] // 指定数据库中的列类型
        public string? district { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [DefaultValue(0.00)] // 设置默认值
        public decimal balance { get; set; }
        /// <summary>
        /// 已支付金额
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [DefaultValue(0.00)] // 设置默认值
        public decimal pay_money { get; set; }
        /// <summary>
        /// 最后登录时间
        /// </summary>
        [Required]
        [Column(TypeName = "DATETIME")] // 明确指定为 DATETIME 类型
        public DateTime last_login_time { get; set; }



        /// <summary>
        /// 外键导航属性
        /// </summary>
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Cart> Carts { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
