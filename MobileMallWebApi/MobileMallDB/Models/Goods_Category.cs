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
    /// 商品类别
    /// </summary>
    public class Goods_Category
    {
        /// <summary>
        /// 类别id
        /// </summary>
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int category_id { get; set; }
        /// <summary>
        /// 类别名称
        /// </summary>
        [Required]
        [StringLength(256)] // 限制字符串的最大长度
        [Column(TypeName = "VARCHAR(256)")] // 指定数据库中的列类型
        public string name { get; set; }
        /// <summary>
        /// 分类图片地址
        /// </summary>
        [Required]
        [StringLength(256)] // 限制字符串的最大长度
        [Column(TypeName = "VARCHAR(256)")] // 指定数据库中的列类型
        public string imgUrl { get; set; }


        /// <summary>
        /// 外键导航属性
        /// </summary>
        public ICollection<Goods> Goods { get; set; }
    }
}
