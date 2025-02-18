import { defineStore } from 'pinia'
import { ref } from 'vue'
import { changeCountApi, delSelectApi, getCartList } from '@/api/cart'
import { showToast } from 'vant'

// 用户模块
export const useCartStore = defineStore(
  'useCartStore',
  () => {
    const cartList = ref([])
    // 给集合赋新值
    const setCartList = (newList) => {
      cartList.value = newList
    }
    // 修改勾选商品
    const toggleCheck = (goodsId) => {
      // 让对应的 id 的项 状态取反  用find的话，修改find的返回值会影响原数组，用filter则不会，这里用find更方便
      const goods = cartList.value.find((item) => item.goods_id === goodsId)
      goods.isChecked = !goods.isChecked
      // console.log('更改后的选中值：' + goods.isChecked)
    }
    // 全选或全不选
    const toggleAllCheck = (flag) => {
      cartList.value.forEach((item) => {
        item.isChecked = flag
      })
    }
    // 修改指定商品的数量
    const changeCount = (goodsId, goodsNum) => {
      const goods = cartList.value.find((item) => item.goods_id == goodsId)
      if (goods) goods.goods_num = goodsNum
      else getCartAction()
    }

    // 以下是计算方法
    // 求所有的商品累加总数
    const cartTotal = () => {
      return cartList.value.reduce((sum, item) => sum + item.goods_num, 0)
    }
    // 选中的商品项
    const selCartList = () => {
      return cartList.value.filter((item) => item.isChecked)
    }
    // 选中的总数
    const selCount = () => {
      return selCartList().reduce((sum, item) => sum + item.goods_num, 0)
    }
    // 选中的总价
    const selPrice = () => {
      return selCartList()
        .reduce((sum, item) => {
          return sum + item.goods_num * item.goods.goods_price_min
        }, 0)
        .toFixed(2)
    }
    // 是否全选
    const isAllChecked = () => {
      return cartList.value.every((item) => item.isChecked)
    }

    // 接口交互部分
    // 调用接口获取用户购物车数据并设置默认选中
    const getCartAction = async () => {
      const { data } = await getCartList()
      // 后台返回的数据中，不包含复选框的选中状态，为了实现将来的功能
      // 需要手动维护数据，给每一项，添加一个 isChecked 状态 (标记当前商品是否选中)
      data.list.forEach((item) => {
        item.isChecked = true
      })
      // 重新赋值给缓存
      setCartList(data.list)
    }
    // 修改商品数量
    const changeCountAction = async (obj) => {
      const { goodsNum, goodsId, goodsSkuId } = obj
      // 先本地修改
      changeCount(goodsId, goodsNum)
      // 再同步到后台
      await changeCountApi(goodsId, goodsNum, goodsSkuId)
    }
    const delSelect = async () => {
      const cartIds = selCartList().map((item) => item.cart_id)
      const Resp = await delSelectApi(cartIds)
      if (Resp.status === 200) showToast('删除商品成功')
      else showToast('删除商品失败,' + Resp.message)
      // 重新拉取最新的购物车数据 (重新渲染)
      getCartAction()
    }

    return {
      cartList,
      setCartList,
      toggleCheck,
      toggleAllCheck,
      changeCount,
      cartTotal,
      selCartList,
      selCount,
      selPrice,
      isAllChecked,
      getCartAction,
      changeCountAction,
      delSelect,
    }
  },
  // {
  //   // Pinia持久化插件
  //   persist: {
  //     key: 'my-cart-key', // 这是默认的键名，用于在存储介质中引用数据。
  //     storage: localStorage, // 这是默认的存储位置，用于持久化状态。
  //     paths: ['cartList'], // 指定哪些路径应该被持久化
  //     serializer: {
  //       serialize: (state) => JSON.stringify(state),
  //       deserialize: (data) => JSON.parse(data),
  //     },
  //     debug: false, // 调试模式，关闭时错误不会输出到控制台。
  //   },
  // },
)
