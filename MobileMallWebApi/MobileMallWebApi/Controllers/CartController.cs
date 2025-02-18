using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileMallDB.BusinessInterface;
using MobileMallWebApi.ApiModels.Resp;
using MobileMallWebApi.Utils.Other;
using MobileMallWebApi.Utils;
using MobileMallWebApi.ApiModels.Req;
using MobileMallDB.Models;
using System.Linq.Expressions;
using MobileMallDB;
using MobileMallWebApi.Utils.Authorization;
using Org.BouncyCastle.Ocsp;
using System.Dynamic;

namespace MobileMallWebApi.Controllers
{
    /// <summary>
    /// 布局页面相关
    /// </summary>
    [Route("Api/V1/[controller]/[action]")]
    [Authorize]
    //[AllowAnonymous]
    [ApiController]
    public class CartController : Controller
    {
        private readonly IBaseService BaseServiceClient;
        private readonly JwtHelper _jwtHelper;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="baseService"></param>
        /// <param name="jwtHelper"></param>
        public CartController(IBaseService baseService, JwtHelper jwtHelper)
        {
            this.BaseServiceClient = baseService;
            _jwtHelper = jwtHelper;
        }

        /// <summary>
        /// 加入购物车
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BaseResp<GoodsDetailResp.Item.BannerData>>> AddCart([FromBody] GoodsDetailReq req)
        {
            //解析token
            string? token = this.Request.Headers["Authorization"];
            var UserToken = _jwtHelper.SerializeJwt(token);
            Expression<Func<Cart, bool>> funcWhere = p => p.user_id.Equals(UserToken.UserId);
            //funcWhere = funcWhere.And(p => p.goods_id.Equals(req.goods_id) && p.user_id.Equals(UserToken.UserId));
            var GetAllCart = await BaseServiceClient.Query(funcWhere);
            var GetCart = GetAllCart.Where(x => x.goods_id.Equals(req.goods_id)).ToList();
            bool Result = false;
            int cartTotalCount = (int)req.goods_num;
            if (GetCart.Count > 0)
            {
                Cart cartInfo = GetCart.First();
                cartInfo.goods_count = (int)req.goods_num;
                cartInfo.update_time = DateTime.Now;
                Result = await BaseServiceClient.Update(cartInfo);
                cartTotalCount += GetAllCart.Where(x => x != cartInfo).Sum(y => y.goods_count);
            }
            else
            {
                cartTotalCount += GetAllCart.Sum(y => y.goods_count);
                Result = await BaseServiceClient.Insert(new Cart()
                {
                    cart_id = (int)req.goods_id,
                    goods_id = (int)req.goods_id,
                    user_id = UserToken.UserId,
                    goods_count = (int)req.goods_num,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                });
            }
            if (!Result)
                return Ok(await ApiResult.SetFailure("加入购物车失败！", new GoodsDetailResp.Item.BannerData()));

            return Ok(await ApiResult.SetSuccess("success", new GoodsDetailResp.Item.BannerData() { cartTotal = cartTotalCount }));
        }

        /// <summary>
        /// 获取用户购物车信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BaseResp<CartListResp>>> GetCartList()
        {
            string HttpPathAddrss = MethodTool.GetServerPath(HttpContext);
            //解析token
            string? token = this.Request.Headers["Authorization"];
            var UserToken = _jwtHelper.SerializeJwt(token);

            CartListResp cartListResult = new CartListResp();
            cartListResult.list = await BaseServiceClient.GetContext().Carts.Where(y => y.user_id == UserToken.UserId).Select(x => new CartListResp.CartList
            {
                cart_id = x.cart_id,
                goods_id = x.goods_id,
                user_id = x.user_id,
                goods_num = x.goods_count,
                goods = new CartListResp.CartList.goodsList()
                {
                    category_id = x.Goods.category_id,
                    goods_id = x.Goods.goods_id,
                    goods_image = HttpPathAddrss + x.Goods.goods_image,
                    goods_name = x.Goods.goods_name,
                    goods_price_max = x.Goods.goods_price_max,
                    goods_price_min = x.Goods.goods_price_min,
                }
            }).ToListAsync();
            cartListResult.cartTotal = cartListResult.list.Sum(x => x.goods_num);
            return Ok(await ApiResult.SetSuccess("success", cartListResult));
        }

        /// <summary>
        /// 修改购物车商品数量
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BaseResp<GoodsDetailResp.Item.BannerData>>> UpCartCount([FromBody] GoodsDetailReq req)
        {
            //解析token
            string? token = this.Request.Headers["Authorization"];
            var UserToken = _jwtHelper.SerializeJwt(token);

            Expression<Func<Cart, bool>> funcWhere = p => true;
            funcWhere = funcWhere.And(p => p.goods_id.Equals(req.goods_id) && p.user_id.Equals(UserToken.UserId));
            var GetCart = await BaseServiceClient.Query(funcWhere);
            if (GetCart.Count > 0)
            {
                var CartInfo = GetCart.First();
                CartInfo.goods_count = (int)req.goods_num;
                CartInfo.update_time = DateTime.Now;
                bool Result = await BaseServiceClient.Update(CartInfo);
                if (!Result)
                    return Ok(await ApiResult.SetFailure("更新失败！", new GoodsDetailResp.Item.BannerData()));
                return Ok(await ApiResult.SetSuccess("success", new GoodsDetailResp.Item.BannerData() { cartTotal = CartInfo.goods_count }));
            }
            return Ok(await ApiResult.SetFailure("更新失败，未查询到指定记录！", new GoodsDetailResp.Item.BannerData()));
        }

        /// <summary>
        /// 删除购物车记录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BaseResp<GoodsDetailResp.Item.BannerData>>> DelCart([FromBody] DelCartReq req)
        {
            //解析token
            string? token = this.Request.Headers["Authorization"];
            var UserToken = _jwtHelper.SerializeJwt(token);

            Expression<Func<Cart, bool>> funcWhere = p => true;
            funcWhere = funcWhere.And(p => req.cartIds.Contains(p.cart_id));
            var GetCart = await BaseServiceClient.Query(funcWhere);
            if (GetCart.Count > 0)
            {
                bool Result = await BaseServiceClient.Delete(GetCart);
                if (!Result)
                    return Ok(await ApiResult.SetFailure("删除失败！", new GoodsDetailResp.Item.BannerData()));
                return Ok(await ApiResult.SetSuccess("success", new GoodsDetailResp.Item.BannerData()));
            }
            return Ok(await ApiResult.SetFailure("删除失败，未查询到指定记录！", new GoodsDetailResp.Item.BannerData()));
        }
    }
}
