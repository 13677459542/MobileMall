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
    /// 订单
    /// </summary>
    public class Order
    {
        /// <summary>
        /// 订单id
        /// </summary>
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int order_id { get; set; }
       
        /// <summary>
        /// 用户id
        /// </summary>
        [Required]
        public int user_id { get; set; }
        /// <summary>
        /// 订单总数量
        /// </summary>
        [Required]
        public int total_num { get; set; }
        /// <summary>
        /// 订单总金额
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [DefaultValue(0.00)] // 设置默认值
        public decimal total_price { get; set; }
        /// <summary>
        /// 订单状态  1、待支付，2：待发货，3：待收货，4：待评价
        /// </summary>
        [Required]
        public int order_status { get; set; }
        /// <summary>
        /// 结算模式：0;直接购买，1：购物车结算
        /// </summary>
        [Required]
        public int mode { get; set; }
        /// <summary>
        /// 交付方式 0：店内消费，1：门店自提、2：快递配送
        /// </summary>
        [Required]
        public int delivery { get; set; }
        /// <summary>
        /// 优惠券id 为0时不使用
        /// </summary>
        [Required]
        public int couponId { get; set; }
        /// <summary>
        /// 积分  为0时不使用
        /// </summary>
        [Required]
        public int isUsePoints { get; set; }
        /// <summary>
        /// 支付方式：0：现金支付，1：余额支付
        /// </summary>
        [Required]
        public int payType { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        [Column(TypeName = "DATETIME")] // 明确指定为 DATETIME 类型
        public DateTime create_time { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Column(TypeName = "LONGTEXT")] // 这里可以根据需要选择 TEXT、LONGTEXT 或 MEDIUMTEXT
        public string? remark { get; set; }


        /// <summary>
        /// 外键导航属性
        /// </summary>
        public User Users { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
