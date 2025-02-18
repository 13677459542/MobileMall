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
    /// 订单明细
    /// </summary>
    public class OrderDetails
    {
        /// <summary>
        /// 订单id
        /// </summary>
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int order_details_id { get; set; }
        /// <summary>
        /// 订单id
        /// </summary>
        [Required]
        public int order_id { get; set; }
        /// <summary>
        /// 商品id
        /// </summary>
        [Required]
        public int goods_id { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        public int goods_num { get; set; }


        /// <summary>
        /// 外键导航属性
        /// </summary>
        public Goods Goods { get; set; }
        public Order Orders { get; set; }
    }
}
