import request from '@/utils/request'

// 获取搜索商品列表的数据
export const getProList = (obj) => {
  const { categoryId, goods_name, page } = obj
  // return request.get('/goods/list', {
  //   params: {
  //     // 当以下参数没传时，axios会自动给它毙掉
  //     categoryId,
  //     goodsName,
  //     page,
  //   },
  // })
  return request.post('/Layout/GetGoodsList', {
    categoryId,
    goods_name,
    page,
  })
}

// 获取商品详情数据
export const getProDetail = (goods_id) => {
  // return request.get('/goods/detail', {
  //   params: {
  //     goodsId,
  //   },
  // })
  return request.post('/Layout/GetGoodsDetail', {
    goods_id,
  })
}

// 获取商品评价
export const getProComments = (goods_id, limit) => {
  return request.post('/Layout/GetGoodsComment', {
    goods_id,
    limit,
  })
}
