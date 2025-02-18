namespace MobileMallWebApi.ApiModels.Resp
{
    /// <summary>
    /// 订单结算台
    /// </summary>
    public class CheckOrderResp
    {
        /// <summary>
        /// 结算信息
        /// </summary>
        public OrderInfo order { get; set; }
        /// <summary>
        /// 用户个人信息
        /// </summary>
        public Personal personal { get; set; }
        /// <summary>
        /// 结算信息实体
        /// </summary>
        public class OrderInfo
        {
            /// <summary>
            /// 订单商品数量
            /// </summary>
            public int orderTotalNum {  get; set; }
            /// <summary>
            /// 订单商品金额合计
            /// </summary>
            public decimal orderTotalPrice {  get; set; }
            /// <summary>
            /// 商品集合
            /// </summary>
            public List<GoodsListResp> goodsList {  get; set; }
        }
        /// <summary>
        /// 用户个人信息实体
        /// </summary>
        public class Personal
        {
            /// <summary>
            /// 用户id
            /// </summary>
            public int user_id {  get; set; }
            /// <summary>
            /// 余额
            /// </summary>
            public decimal balance {  get; set; }
            /// <summary>
            /// 地址id
            /// </summary>
            public int address_id {  get; set; }
        }
    }
}
