import request from '@/utils/request'

// 获取分类数据
export const getCategoryData = () => {
  return request.post('/Layout/GetCategoryDetailList')
}
