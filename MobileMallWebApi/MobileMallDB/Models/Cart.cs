using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MobileMallDB.Models
{
    /// <summary>
    /// 购物车
    /// </summary>
    public class Cart
    {
        /// <summary>
        /// 商品记录id
        /// </summary>
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int cart_id { get; set; }
        /// <summary>
        /// 商品id
        /// </summary>
        [Required]
        public int goods_id { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        [Required]
        public int user_id { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        [Required]
        public int goods_count { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        [Column(TypeName = "DATETIME")] // 明确指定为 DATETIME 类型
        public DateTime create_time { get; set; }
        /// <summary>
        /// 最后更新
        /// </summary>
        [Required]
        [Column(TypeName = "DATETIME")] // 明确指定为 DATETIME 类型
        public DateTime update_time { get; set; }


        /// <summary>
        /// 外键导航属性
        /// </summary>
        public Goods Goods { get; set; }
        public User Users { get; set; }
    }
}
