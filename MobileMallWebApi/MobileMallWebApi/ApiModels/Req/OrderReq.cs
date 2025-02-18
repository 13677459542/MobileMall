using System.ComponentModel.DataAnnotations;

namespace MobileMallWebApi.ApiModels.Req
{
    /// <summary>
    /// 订单入参
    /// </summary>
    public class OrderReq
    {
        /// <summary>
        /// 订单类型（查询订单时使用）：0：全部，1：待支付，2：待发货，3：待收货
        /// </summary>
        public int? dataType {  get; set; }
        /// <summary>
        /// 结算模式：0;直接购买，1：购物车结算
        /// </summary>
        public int? mode { get; set; }
        /// <summary>
        /// 交付方式 0：店内消费，1：门店自提、2：快递配送
        /// </summary>
        public int? delivery {  get; set; }
        /// <summary>
        /// 优惠券id 为0时不使用
        /// </summary>
        public int? couponId {  get; set; }
        /// <summary>
        /// 积分  为0时不使用
        /// </summary>
        public int? isUsePoints {  get; set; }
        /// <summary>
        /// 支付方式：0：现金支付，1：余额支付
        /// </summary>
        public int? payType {  get; set; }
        /// <summary>
        /// 购物车id集合
        /// </summary>
        public List<int>? cartIds {  get; set; }
        /// <summary>
        /// 商品id
        /// </summary>
        public int? goodsId { get; set; }
        /// <summary>
        /// 购买数量
        /// </summary>
        public int? goodsNum { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string? remark { get; set; }
    }
}
