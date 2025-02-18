using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileMallDB.Models
{
    /// <summary>
    /// 商品文件列表
    /// </summary>
    public class Goods_File
    {
        /// <summary>
        /// 文件id
        /// </summary>
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int file_id { get; set; }
        /// <summary>
        /// 商品id
        /// </summary>
        [Required]
        public int goods_id { get; set; }
        /// <summary>
        /// 文件类型  1:图片 、2：视频
        /// </summary>
        [Required]
        public int file_type { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public int? file_size { get; set; }
        /// <summary>
        /// 文件后缀
        /// </summary>
        [StringLength(20)] // 限制字符串的最大长度
        [Column(TypeName = "VARCHAR(20)")] // 指定数据库中的列类型
        public string? file_ext { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        [Required]
        [StringLength(256)] // 限制字符串的最大长度
        [Column(TypeName = "VARCHAR(256)")] // 指定数据库中的列类型
        public string image_name { get; set; }
        /// <summary>
        /// 文件外部地址
        /// </summary>
        [Required]
        [StringLength(256)] // 限制字符串的最大长度
        [Column(TypeName = "VARCHAR(256)")] // 指定数据库中的列类型
        public string external_url { get; set; }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        [Required]
        [Column(TypeName = "DATETIME")] // 明确指定为 DATETIME 类型
        public DateTime update_time { get; set; }


        /// <summary>
        /// 外键导航属性
        /// </summary>
        public Goods Goods { get; set; }
    }
}
