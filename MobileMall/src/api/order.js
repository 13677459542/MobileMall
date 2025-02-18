import request from '@/utils/request'

// 订单结算确认
// mode: cart    => obj { cartIds }
// mode: buyNow  => obj { goodsId  goodsNum  goodsSkuId }
export const checkOrder = (mode, obj) => {
  // return request.get('/checkout/order', {
  //   params: {
  //     mode, // cart buyNow
  //     delivery: 10, // 10 快递配送 20 门店自提
  //     couponId: 0, // 优惠券ID 传0 不使用优惠券
  //     isUsePoints: 0, // 积分 传0 不使用积分
  //     ...obj, // 将传递过来的参数对象 动态展开
  //   },
  // })

  return request.post('/Order/CheckOrder', {
    mode,
    ...obj,
  })
}

// 提交订单
// mode: cart    => obj { cartIds, remark }
// mode: buyNow  => obj { goodsId, goodsNum, goodsSkuId, remark }
export const submitOrder = (mode, obj) => {
  // return request.post('/checkout/submit', {
  //   mode,
  //   delivery: 20, // 10 快递配送 20 门店自提
  //   couponId: 0,
  //   isUsePoints: 0,
  //   payType: 10, // 余额支付
  //   ...obj,
  // })
  return request.post('/Order/SubmitOrder', {
    mode,
    ...obj,
  })
}

// 订单列表
export const getMyOrderList = (dataType) => {
  // return request.get('/order/list', {
  //   params: {
  //     dataType,
  //     page, // List
  //   },
  // })
  return request.post('/Order/QueryOrder', {
    dataType,
  })
}
