using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileMallDB.Models
{
    /// <summary>
    /// 图形验证码
    /// </summary>
    public class Captcha
    {
        /// <summary>
        /// 唯一键
        /// </summary>
        [Required]
        [Key]
        [StringLength(256)] // 限制字符串的最大长度
        [Column(TypeName = "VARCHAR(256)")] // 指定数据库中的列类型
        public string key { get; set; }
        ///// <summary>
        ///// 验证码图片base64数据
        ///// </summary>
        //[Required]
        //[Column(TypeName = "LONGTEXT")] // 这里可以根据需要选择 TEXT、LONGTEXT 或 MEDIUMTEXT
        //public string base64 { get; set; }

        /// <summary>
        /// 图形验证码Code值
        /// </summary>
        [Required]
        [StringLength(20)] // 限制字符串的最大长度
        [Column(TypeName = "VARCHAR(20)")] // 指定数据库中的列类型
        public string CaptchaCode { get; set; }
        /// <summary>
        /// 值的md5加密数据
        /// </summary>
        [StringLength(256)] // 限制字符串的最大长度
        [Column(TypeName = "VARCHAR(256)")] // 指定数据库中的列类型
        public string? md5 { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        [Column(TypeName = "DATETIME")] // 明确指定为 DATETIME 类型
        public DateTime create_time { get; set; }
    }
}
