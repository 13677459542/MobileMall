namespace MobileMallWebApi.ApiModels.Resp
{
    /// <summary>
    /// 商品集合实体
    /// </summary>
    public class GoodsListResp
    {
        /// <summary>
        /// 商品id
        /// </summary>
        public int goods_id { get; set; }
        /// <summary>
        /// 商品图片地址
        /// </summary>
        public string goods_image { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string goods_name { get; set; }
        /// <summary>
        /// 商品支付单价
        /// </summary>
        public decimal total_pay_price { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        public int total_num { get; set; }
    }
}
