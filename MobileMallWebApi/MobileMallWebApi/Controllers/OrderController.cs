using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MobileMallDB;
using MobileMallDB.BusinessInterface;
using MobileMallDB.Models;
using MobileMallWebApi.ApiModels.Req;
using MobileMallWebApi.ApiModels.Resp;
using MobileMallWebApi.Utils;
using MobileMallWebApi.Utils.Authorization;
using MobileMallWebApi.Utils.Other;
using Org.BouncyCastle.Asn1.X9;
using System.Linq.Expressions;
using static MobileMallWebApi.ApiModels.Resp.CartListResp.CartList;
using static MobileMallWebApi.ApiModels.Resp.GoodsDetailResp;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace MobileMallWebApi.Controllers
{
    /// <summary>
    /// 订单相关
    /// </summary>
    [Route("Api/V1/[controller]/[action]")]
    [Authorize]
    //[AllowAnonymous]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IBaseService BaseServiceClient;
        private readonly JwtHelper _jwtHelper;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="baseService"></param>
        /// <param name="jwtHelper"></param>
        public OrderController(IBaseService baseService, JwtHelper jwtHelper)
        {
            this.BaseServiceClient = baseService;
            _jwtHelper = jwtHelper;
        }

        /// <summary>
        /// 获取个人订单信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BaseResp<OrderResp>>> QueryOrder([FromBody] OrderReq req)
        {
            string HttpPathAddrss = MethodTool.GetServerPath(HttpContext);
            //解析token
            string? token = this.Request.Headers["Authorization"];
            var UserToken = _jwtHelper.SerializeJwt(token);

            OrderResp cartListResult = new OrderResp() { list = new OrderResp.listInfoResp() { current_page = 1, last_page = 1 } };

            Expression<Func<Order, bool>> funcWhere = p => true;
            funcWhere = funcWhere.And(p => p.user_id.Equals(UserToken.UserId));
            if(req.dataType != null && req.dataType != 0)
            funcWhere = funcWhere.And(p => p.order_status.Equals(req.dataType));

            cartListResult.list.data = await BaseServiceClient.GetContext().Orders.Where(funcWhere).Select(x => new OrderResp.listInfoResp.dataList
            {
                order_id = x.order_id,
                order_status = x.order_status,
                create_time = x.create_time,
                total_num = x.total_num,
                total_price = x.total_price,
                goods = x.OrderDetails.Select(y => new GoodsListResp
                {
                    goods_id = y.goods_id,
                    total_num = y.goods_num,
                    goods_name = y.Goods.goods_name,
                    goods_image = HttpPathAddrss + y.Goods.goods_image,
                    total_pay_price = y.Goods.goods_price_min
                }).ToList()
            }).OrderByDescending(x => x.create_time).ToListAsync();

            //cartListResult.list.data = await BaseServiceClient.GetContext().OrderDetails.Include(x => x.Orders).Include(x => x.Goods).Select(x => new OrderResp.listInfo.dataList()
            //{
            //    order_status = x.Orders.order_status,
            //    create_time = x.Orders.create_time,
            //    total_num = x.Orders.total_num,
            //    total_price = x.Orders.total_price,
            //    goodsList = x.Goods
            //}).ToListAsync()

            cartListResult.list.total = cartListResult.list.data.Count;
            return Ok(await ApiResult.SetSuccess("success", cartListResult));
        }

        /// <summary>
        /// 结算检查订单接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BaseResp<CheckOrderResp>>> CheckOrder([FromBody] OrderReq req)
        {
            string HttpPathAddrss = MethodTool.GetServerPath(HttpContext);
            //解析token
            string? token = this.Request.Headers["Authorization"];
            var UserToken = _jwtHelper.SerializeJwt(token);

            CheckOrderResp checkOrderResp = new CheckOrderResp() { order = new CheckOrderResp.OrderInfo(), personal = new CheckOrderResp.Personal() };
            if (req.mode == 0)
            {
                checkOrderResp.order.goodsList = await BaseServiceClient.GetContext().Goods.Where(x => x.goods_id.Equals(req.goodsId)).Select(y => new GoodsListResp
                {
                    goods_id = y.goods_id,
                    goods_image = HttpPathAddrss + y.goods_image,
                    goods_name = y.goods_name,
                    total_num = (int)req.goodsNum,
                    total_pay_price = (int)req.goodsNum * y.goods_price_min,
                }).ToListAsync();
                checkOrderResp.order.orderTotalNum = checkOrderResp.order.goodsList.Sum(x => x.total_num);
                checkOrderResp.order.orderTotalPrice = checkOrderResp.order.goodsList.Sum(x => x.total_num * x.total_pay_price);
            }
            else if (req.mode == 1)
            {
                checkOrderResp.order.goodsList = await BaseServiceClient.GetContext().Carts.Where(x => req.cartIds.Contains(x.cart_id)).Select(y => new GoodsListResp
                {
                    goods_id = y.goods_id,
                    goods_image = HttpPathAddrss + y.Goods.goods_image,
                    goods_name = y.Goods.goods_name,
                    total_num = y.goods_count,
                    total_pay_price = y.goods_count * y.Goods.goods_price_min,
                }).ToListAsync();
                checkOrderResp.order.orderTotalNum = checkOrderResp.order.goodsList.Sum(x => x.total_num);
                checkOrderResp.order.orderTotalPrice = checkOrderResp.order.goodsList.Sum(x => x.total_num * x.total_pay_price);
            }
            else
                return Ok(await ApiResult.SetFailure("未知结算模式", checkOrderResp));

            Expression<Func<User, bool>> funcWhere = p => true;
            funcWhere = funcWhere.And(p => p.user_id.Equals(UserToken.UserId));
            checkOrderResp.personal = await BaseServiceClient.GetContext().Users.Where(funcWhere).Select(x => new CheckOrderResp.Personal()
            {
                user_id = x.user_id,
                balance = x.balance,
                address_id = 0
            }).FirstAsync();

            if (checkOrderResp?.order?.goodsList.Count <= 0 || checkOrderResp.personal == null)
                return Ok(await ApiResult.SetFailure("获取结算信息失败！", checkOrderResp));
            return Ok(await ApiResult.SetSuccess("success", checkOrderResp));
        }

        /// <summary>
        /// 提交结算接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BaseResp<Order>>> SubmitOrder([FromBody] OrderReq req)
        {
            //解析token
            string? token = this.Request.Headers["Authorization"];
            var UserToken = _jwtHelper.SerializeJwt(token);

            Order OrderResult = new Order();
            var UserInfo = await BaseServiceClient.GetContext().Users.Where(x => x.user_id == UserToken.UserId).FirstAsync();
            if (UserInfo == null)
                return Ok(await ApiResult.SetFailure("未获取到您的用户信息，请尝试重新登录！", OrderResult));

            Order OrderInfo = new Order()
            {
                user_id = UserInfo.user_id,
                order_status = req.payType == 1 ? 2 : 1,
                mode = (int)req.mode,
                delivery = (int)req.delivery,
                couponId = (int)req.couponId,
                isUsePoints = (int)req.isUsePoints,
                payType = (int)req.payType,
                create_time = DateTime.Now,
                remark = req.remark,
            };
            List<OrderDetails> orderDetails = new List<OrderDetails>();
            if (req.mode == 0)
            {
                Expression<Func<Goods, bool>> funcWhere = p => true;
                funcWhere = funcWhere.And(p => p.goods_id.Equals(req.goodsId));
                var GoodsInfo = await BaseServiceClient.Query(funcWhere);
                if (GoodsInfo.Count <= 0)
                    return Ok(await ApiResult.SetFailure("未查询到加入的商品，请稍后重试！", OrderResult));
                OrderInfo.total_num = (int)req.goodsNum;
                OrderInfo.total_price = (int)req.goodsNum * GoodsInfo.First().goods_price_min;
                orderDetails.Add(new OrderDetails()
                {
                    //order_id = OrderInfo.order_id,
                    goods_id = (int)req.goodsId,
                    goods_num = (int)req.goodsNum,
                });
            }
            else if (req.mode == 1)
            {
                Expression<Func<Cart, bool>> funcWhere = p => true;
                funcWhere = funcWhere.And(p => req.cartIds.Contains(p.cart_id));
                var CartList = await BaseServiceClient.Query(funcWhere);
                var GoodsCartList = await BaseServiceClient.GetContext().Carts.Where(p => req.cartIds.Contains(p.cart_id)).Select(x => new
                {
                    x.goods_id,
                    x.goods_count,
                    x.Goods.goods_name,
                    x.Goods.goods_price_min,
                }).ToListAsync();
                if (GoodsCartList.Count <= 0)
                    return Ok(await ApiResult.SetFailure("未查询到购物车商品信息，请稍后重试！", OrderResult));
                OrderInfo.total_num = GoodsCartList.Sum(x => x.goods_count);
                OrderInfo.total_price = GoodsCartList.Sum(x => x.goods_count * x.goods_price_min);
                foreach (var item in GoodsCartList)
                    orderDetails.Add(new OrderDetails()
                    {
                        //order_id = OrderInfo.order_id,
                        goods_id = item.goods_id,
                        goods_num = item.goods_count,
                    });
            }
            else
                return Ok(await ApiResult.SetFailure("未知结算模式", OrderResult));

            if(OrderInfo.payType == 1 && UserInfo.balance < OrderInfo.total_price)
                return Ok(await ApiResult.SetFailure("您的余额不足，请选择现金支付！", OrderResult));

            var OrderInsertResult = await BaseServiceClient.Insert(OrderInfo); // 插入订单信息
            if (!OrderInsertResult)
                return Ok(await ApiResult.SetFailure("新增订单数据失败，请稍后再试！", OrderResult));

            orderDetails.ForEach(x => x.order_id = OrderInfo.order_id); // 插入订单明细前循环赋值新的订单id
            var OrderDetailsInsertResult = await BaseServiceClient.Insert(orderDetails); // 插入订单明细信息
            if (!OrderDetailsInsertResult)
            {
                await BaseServiceClient.Delete(OrderInfo);
                return Ok(await ApiResult.SetFailure("新增订单明细数据数据失败，请稍后再试！", OrderResult));
            }
            OrderResult.order_id = OrderInfo.order_id;
            OrderResult.payType = (int)req.payType;
            if(req.payType == 1)
            {
                UserInfo.balance -= OrderInfo.total_price;
                UserInfo.pay_money += OrderInfo.total_price;
                var Result = await BaseServiceClient.Update(UserInfo);
                if (!Result)
                {
                    await BaseServiceClient.Delete(orderDetails);
                    await BaseServiceClient.Delete(OrderInfo);
                    return Ok(await ApiResult.SetFailure("更新用户数据失败，请稍后再试！", OrderResult));
                }
            }
            if (req.mode == 1)
            {
                var CartList = await BaseServiceClient.GetContext().Carts.Where(p => req.cartIds.Contains(p.cart_id)).ToListAsync();
                await BaseServiceClient.Delete(CartList);
            }
            return Ok(await ApiResult.SetSuccess("success", OrderResult));
        }
    }
}
