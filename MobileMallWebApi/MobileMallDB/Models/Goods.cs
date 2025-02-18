using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MobileMallDB.Models
{
    /// <summary>
    /// 商品
    /// </summary>
    public class Goods
    {
        /// <summary>
        /// 商品id
        /// </summary>
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int goods_id { get; set; }
        /// <summary>
        /// 类别id
        /// </summary>
        [Required]
        public int category_id { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [Required]
        [StringLength(256)] // 限制字符串的最大长度
        [Column(TypeName = "VARCHAR(256)")] // 指定数据库中的列类型
        public string goods_name { get; set; }
        /// <summary>
        /// 商品状态  0：上架，其他：下架
        /// </summary>
        [Required]
        public int status { get; set; }
        /// <summary>
        /// 商品封面图片名称
        /// </summary>
        [Required]
        [StringLength(256)] // 限制字符串的最大长度
        [Column(TypeName = "VARCHAR(256)")] // 指定数据库中的列类型
        public string goods_image_name { get; set; }
        /// <summary>
        /// 商品封面图片路径
        /// </summary>
        [Required]
        [StringLength(256)] // 限制字符串的最大长度
        [Column(TypeName = "VARCHAR(256)")] // 指定数据库中的列类型
        public string goods_image { get; set; }
        /// <summary>
        /// 现价
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [DefaultValue(0.00)] // 设置默认值
        public decimal goods_price_min { get; set; }
        /// <summary>
        /// 现价
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [DefaultValue(0.00)] // 设置默认值
        public decimal goods_price_max { get; set; }
        /// <summary>
        /// 商品详情内容
        /// </summary>
        [Column(TypeName = "LONGTEXT")] // 这里可以根据需要选择 TEXT、LONGTEXT 或 MEDIUMTEXT
        public string? content { get; set; }


        /// <summary>
        /// 外键导航属性
        /// </summary>
        public Goods_Category Goods_Categorys { get; set; }
        /// <summary>
        /// 外键导航属性
        /// </summary>
        public ICollection<Goods_File> Goods_Files { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Cart> Carts { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
