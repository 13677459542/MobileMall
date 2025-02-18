using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MobileMallWebApi.ApiModels.Resp
{
    /// <summary>
    /// 订单反参
    /// </summary>
    public class OrderResp
    {
        /// <summary>
        /// 数据实体
        /// </summary>
        public listInfoResp list { get; set; }
        /// <summary>
        /// 数据实体类
        /// </summary>
        public class listInfoResp
        {
            /// <summary>
            /// 订单数量
            /// </summary>
            public int total { get; set; }
            /// <summary>
            /// 起始页
            /// </summary>
            public int current_page { get; set; }
            /// <summary>
            /// 最后页
            /// </summary>
            public int last_page { get; set; }
            /// <summary>
            /// 订单集合
            /// </summary>
            public List<dataList> data { get; set; }
            /// <summary>
            /// 订单集合实体
            /// </summary>
            public class dataList
            {
                /// <summary>
                /// 订单id
                /// </summary>
                public int order_id { get; set; }
                /// <summary>
                /// 订单数量
                /// </summary>
                public int total_num { get; set; }
                /// <summary>
                /// 订单总金额
                /// </summary>
                public decimal total_price { get; set; }
                /// <summary>
                /// 订单状态  1、待支付，2：待发货，3：待收货，4：待评价
                /// </summary>
                public int order_status { get; set; }
                /// <summary>
                /// 订单状态
                /// </summary>
                //[XmlIgnore]
                //[JsonIgnore]
                public string state_text { get => order_status switch { 1 => "待支付", 2 => "待发货", 3 => "待收货", 4 => "待评价", _ => "" }; }
                /// <summary>
                /// 创建时间
                /// </summary>
                public DateTime create_time { get; set; }
                /// <summary>
                /// 商品集合
                /// </summary>
                public List<GoodsListResp> goods { get; set; }
            }
        }
    }
}
