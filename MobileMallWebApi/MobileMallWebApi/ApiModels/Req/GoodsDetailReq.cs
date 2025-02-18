namespace MobileMallWebApi.ApiModels.Req
{
    /// <summary>
    /// 商品详情入参
    /// </summary>
    public class GoodsDetailReq
    {
        /// <summary>
        /// 商品id
        /// </summary>
        public int? goods_id { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string? goods_name { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        public int? goods_num { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int? user_id { get; set; }
        /// <summary>
        /// 规格id （预留）
        /// </summary>
        public int? goodsSkuId { get; set; }
    }
}
