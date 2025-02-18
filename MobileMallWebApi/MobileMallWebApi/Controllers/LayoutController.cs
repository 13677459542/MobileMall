using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using MobileMallWebApi.ApiModels.Req;
using MobileMallWebApi.ApiModels.Resp;
using MobileMallWebApi.Utils;
using MobileMallDB.BusinessInterface;
using Microsoft.EntityFrameworkCore;
using MobileMallDB.Models;
using System.Linq.Expressions;
using MobileMallDB;
using MobileMallWebApi.Utils.Other;

namespace MobileMallWebApi.Controllers
{
    /// <summary>
    /// 布局页面相关
    /// </summary>
    [Route("Api/V1/[controller]/[action]")]
    //[Authorize]
    [AllowAnonymous]
    [ApiController]
    public class LayoutController : Controller
    {
        private readonly IBaseService BaseServiceClient;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="baseService"></param>
        public LayoutController(IBaseService baseService)
        {
            this.BaseServiceClient = baseService;
        }

        /// <summary>
        /// 获取首页数据
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BaseResp<GoodsDetailResp>>> GetHomeDetail()
        {
            string HttpPathAddrss = MethodTool.GetServerPath(HttpContext);
            var navBarList = await BaseServiceClient.GetContext().Goods_Categorys.ToListAsync();
            var GoodsList = await BaseServiceClient.GetContext().Goods.ToListAsync();
            if (navBarList.Count <= 0 || GoodsList.Count <= 0)
                return Ok(await ApiResult.SetFailure("获取数据失败", new GoodsDetailResp()));
            return Ok(await ApiResult.SetSuccess("success", new GoodsDetailResp()
            {
                pageData = new GoodsDetailResp.PageData()
                {
                    items = new List<GoodsDetailResp.Item>
                   {
                       new GoodsDetailResp.Item()
                       {
                           data=new List<GoodsDetailResp.Item.BannerData>()
                           {
                               new GoodsDetailResp.Item.BannerData()
                               {
                                   imgUrl = HttpPathAddrss+"/StaticDataSource/Image/Home/banner/6d239000de9c3f12aafa571349b63d22.jpg",
                               },
                               new GoodsDetailResp.Item.BannerData()
                               {
                                   imgUrl = HttpPathAddrss+"/StaticDataSource/Image/Home/banner/5901a2e13e1075882950af75c98d0007.jpg",
                               },
                               new GoodsDetailResp.Item.BannerData()
                               {
                                   imgUrl = HttpPathAddrss+"/StaticDataSource/Image/Home/banner/7143e84bf3dd41fa67b9675a9e5d81a3.jpg",
                               },
                           }
                       },
                       new GoodsDetailResp.Item()
                       {
                           data= navBarList.Select(x=> new GoodsDetailResp.Item.BannerData()
                           {
                               category_id = x.category_id,
                               text = x.name,
                               imgUrl = HttpPathAddrss+x.imgUrl
                           }).ToList()
                       },
                        new GoodsDetailResp.Item()
                       {
                            data = GoodsList.Select(x=> new GoodsDetailResp.Item.BannerData()
                            {
                                goods_id = x.goods_id,
                                goods_name = x.goods_name,
                                goods_image = HttpPathAddrss+x.goods_image,
                                goods_price_max = x.goods_price_max,
                                goods_price_min = x.goods_price_min,
                            }).ToList()
                       },
                   }
                }
            }));
        }

        /// <summary>
        /// 获取分类商品信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BaseResp<GoodsDetailResp>>> GetCategoryDetailList()
        {
            string HttpPathAddrss = MethodTool.GetServerPath(HttpContext);
            var navBarList = await BaseServiceClient.GetContext().Goods_Categorys.ToListAsync();
            var GoodsList = await BaseServiceClient.GetContext().Goods.ToListAsync();
            if (navBarList.Count <= 0 || GoodsList.Count <= 0)
                return Ok(await ApiResult.SetFailure("获取数据失败", new GoodsDetailResp()));
            return Ok(await ApiResult.SetSuccess("success", new GoodsDetailResp()
            {
                pageData = new GoodsDetailResp.PageData()
                {
                    items = new List<GoodsDetailResp.Item>
                   {
                       new GoodsDetailResp.Item()
                       {
                           data= navBarList.OrderBy(i=>i.category_id).Select(x=> new GoodsDetailResp.Item.BannerData()
                           {
                               text = x.name,
                               imgUrl = HttpPathAddrss + x.imgUrl,
                               category_id = x.category_id,
                           }).ToList()
                       },
                        new GoodsDetailResp.Item()
                       {
                            data = GoodsList.OrderBy(i=>i.category_id).Select(x=> new GoodsDetailResp.Item.BannerData()
                            {
                                goods_id = x.goods_id,
                                goods_name = x.goods_name,
                                goods_image = HttpPathAddrss + x.goods_image,
                                goods_price_max = x.goods_price_max,
                                goods_price_min = x.goods_price_min,
                                category_id=x.category_id,
                            }).ToList()
                       },
                   }
                }
            }));
        }

        /// <summary>
        /// 获取商品信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BaseResp<List<GoodsDetailResp.Item.BannerData>>>> GetGoodsList([FromBody] GoodsDetailReq req)
        {
            string HttpPathAddrss = MethodTool.GetServerPath(HttpContext);
            List<Goods> GoodsList = new List<Goods>();
            if (!string.IsNullOrEmpty(req.goods_name))
            {
                Expression<Func<Goods, bool>> funcWhere = p => p.goods_name.Contains(req.goods_name);
                GoodsList = await BaseServiceClient.Query(funcWhere);
            }
            else
                GoodsList = await BaseServiceClient.Set<Goods>();
            if (GoodsList.Count <= 0)
                return Ok(await ApiResult.SetFailure("未查询到相关数据！", new List<GoodsDetailResp.Item.BannerData>()));
            return Ok(await ApiResult.SetSuccess("success", GoodsList.Select(x => new GoodsDetailResp.Item.BannerData()
            {
                goods_id = x.goods_id,
                goods_name = x.goods_name,
                goods_image = HttpPathAddrss + x.goods_image,
                goods_price_max = x.goods_price_max,
                goods_price_min = x.goods_price_min,
                category_id = x.category_id,
            }).ToList()));
        }

        /// <summary>
        /// 获取商品详情
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BaseResp<GoodsDetailResp.Item.BannerData>>> GetGoodsDetail([FromBody] GoodsDetailReq req)
        {
            string HttpPathAddrss = MethodTool.GetServerPath(HttpContext);
            //var productList = Context.Orders
            //        .Select(o => new
            //        {
            //            o.OrderSn,
            //            CustomerName = o.Users.UserName,
            //            o.CreateTime
            //        })
            //        .ToList();
            //Expression<Func<Goods, bool>> funcWhere = p => true;
            //funcWhere = funcWhere.And(p => p.goods_id.Equals(req.goods_id));
            var GoodsDetailList = await BaseServiceClient.GetContext().Goods.Where(y => y.goods_id == req.goods_id).Select(x => new GoodsDetailResp.Item.BannerData
            {
                goods_name = x.goods_name,
                goods_image = HttpPathAddrss + x.goods_image,
                goods_price_max = x.goods_price_max,
                goods_price_min = x.goods_price_min,
                content = x.content.Replace("\\\"", "\"").Replace("^goods_desc^", (HttpPathAddrss+ $"/StaticDataSource/Image/goods_desc/{x.goods_id}/")), // 1、处理转义，2、处理图片地址
                goods_images = x.Goods_Files.Select(x => new GoodsDetailResp.Item.BannerData.Goods_Images()
                {
                    imageUrl = HttpPathAddrss + x.external_url
                }).ToList(),
            }).ToListAsync();
            if (GoodsDetailList.Count <= 0)
                return Ok(await ApiResult.SetFailure("获取数据失败", new GoodsDetailResp.Item.BannerData()));
            return Ok(await ApiResult.SetSuccess("success", GoodsDetailList.First()));
        }

        /// <summary>
        /// 获取商品评论
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BaseResp<List<GoodsDetailResp.Item.BannerData>>>> GetGoodsComment([FromBody] GoodsDetailReq req)
        {
            string HttpPathAddrss = MethodTool.GetServerPath(HttpContext);
            Expression<Func<Goods, bool>> funcWhere = p => p.goods_id.Equals(req.goods_id);
            var GoodsDetailList = await BaseServiceClient.GetContext().Comments.Where(y => y.goods_id == req.goods_id).Select(x => new GoodsDetailResp.Item.BannerData
            {
                content = x.content,
                avatar_url = string.IsNullOrEmpty(x.Users.avatar_url) ? "" : (HttpPathAddrss + x.Users.avatar_url),
                nick_name = x.Users.nick_name,
                score = x.score,
                create_time = x.create_time,
            }).ToListAsync();
            //if (GoodsDetailList.Count <= 0)
            //    return await ApiResult.SetFailure("获取数据失败", new List<GoodsDetailResp.Item.BannerData>());
            return Ok(await ApiResult.SetSuccess("success", GoodsDetailList));
        }
    }
}
