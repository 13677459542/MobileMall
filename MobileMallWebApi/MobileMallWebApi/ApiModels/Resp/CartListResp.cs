using System.ComponentModel.DataAnnotations;

namespace MobileMallWebApi.ApiModels.Resp
{
    /// <summary>
    /// 购物车数据
    /// </summary>
    public class CartListResp
    {
        /// <summary>
        /// 购物车总数量
        /// </summary>
        public int cartTotal {  get; set; }
        /// <summary>
        /// 购物车集合
        /// </summary>
        public List<CartList> list {  get; set; }
        /// <summary>
        /// 购物车集合实体
        /// </summary>
        public class CartList
        {
            /// <summary>
            /// 编号
            /// </summary>
            public int cart_id { get; set; }
            /// <summary>
            /// 加入购物车数量
            /// </summary>
            public int goods_num { get; set; }
            /// <summary>
            /// 用户id
            /// </summary>
            public int user_id { get; set; }
            /// <summary>
            /// 商品id
            /// </summary>
            public int goods_id { get; set; }
            /// <summary>
            /// 商品信息
            /// </summary>

            public goodsList goods {  get; set; }

            public class goodsList
            {
                /// <summary>
                /// 商品id
                /// </summary>
                public int goods_id { get; set; }
                /// <summary>
                /// 类别id
                /// </summary>
                public int category_id { get; set; }
                /// <summary>
                /// 商品名称
                /// </summary>
                public string goods_name { get; set; }
                /// <summary>
                /// 商品图片
                /// </summary>
                public string goods_image { get; set; }
                /// <summary>
                /// 折扣价
                /// </summary>
                public decimal goods_price_min { get; set; }
                /// <summary>
                /// 原价
                /// </summary>
                public decimal goods_price_max { get; set; }
            }
        }
    }
}
