using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace MobileMallWebApi.ApiModels.Resp
{
    /// <summary>
    /// 首页数据
    /// </summary>
    public class GoodsDetailResp
    {
        public PageData pageData { get; set; }
        public class PageData
        {
            /// <summary>
            /// 数据集合
            /// </summary>
            public List<Item> items { get; set; }
        }

        public class Item
        {
            /// <summary>
            /// 数据列表集合
            /// </summary>
            public List<BannerData> data { get; set; }

            /// <summary>
            /// 列表
            /// </summary>
            public class BannerData
            {
                /// <summary>
                /// 图片地址
                /// </summary>
                public string imgUrl { get; set; }
                /// <summary>
                /// 节点名称
                /// </summary>
                public string text { get; set; }
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
                /// <summary>
                /// 已售
                /// </summary>
                public decimal goods_sales { get; set; }
                /// <summary>
                /// 商品详情内容或评论内容
                /// </summary>
                public string? content { get; set; }
                /// <summary>
                /// 商品图片集合
                /// </summary>
                public List<Goods_Images> goods_images {  get; set; }
                /// <summary>
                /// 用户头像
                /// </summary>
                public string? avatar_url {  get; set; }
                /// <summary>
                /// 用户昵称
                /// </summary>
                public string? nick_name { get; set; }
                /// <summary>
                /// 用户评分
                /// </summary>
                public int score { get; set; }
                /// <summary>
                /// 创建时间
                /// </summary>
                public DateTime create_time { get; set; }
                /// <summary>
                /// 商品购物车数量
                /// </summary>
                public int cartTotal { get; set; }

                /// <summary>
                /// 商品图片集合实体
                /// </summary>
                public class Goods_Images
                {
                    /// <summary>
                    /// 商品图片地址
                    /// </summary>
                    public string imageUrl { get; set; }
                }
            }
        }
    }
}
