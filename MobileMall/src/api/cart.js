import request from '@/utils/request'

// 加入购物车
// goodsId    => 商品id     iphone8
// goodsSkuId => 商品规格id  红色的iphone8  粉色的iphone8
export const addCart = (goods_id, goods_num, goodsSkuId) => {
  return request.post('/Cart/AddCart', {
    goods_id,
    goods_num,
    goodsSkuId,
  })
}

// 获取购物车列表
export const getCartList = () => {
  return request.post('/Cart/GetCartList')
}

// 更新购物车商品数量
export const changeCountApi = (goods_id, goods_num, goodsSkuId) => {
  return request.post('/Cart/UpCartCount', {
    goods_id,
    goods_num,
    goodsSkuId,
  })
}

// 删除购物车商品
export const delSelectApi = (cartIds) => {
  return request.post('/Cart/DelCart', {
    cartIds,
  })
}
