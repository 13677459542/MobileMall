<script setup>
import { ref, watch, onBeforeMount } from 'vue'
import { useCartStore } from '@/stores'
const badgeValue = ref(0)
// const userStore = useUserStore()
const cartStore = useCartStore()

onBeforeMount(() => {
  cartStore.getCartAction() // 获取购物车内容
  badgeValue.value = cartStore.cartTotal()
})

// // 徽标值处理
// const GetBadgeVlaue = () => {
//   // console.log('徽标值处理')
//   if (!userStore.userInfo.token) {
//     return ''
//   }
//   if (badgeValue.value <= 0) {
//     return ''
//   }
//   return badgeValue.value
// }

// 监听cartTotal
watch(cartStore.cartTotal, () => {
  // 当未登录或购物车没有商品时赋值为空，不展示徽标
  badgeValue.value = cartStore.cartTotal()
  // console.log('重新赋值badgeValue.value：' + badgeValue.value)
})
</script>

<template>
  <div>
    <!-- 二级路由出口：二级组件展示的位置 -->
    <router-view></router-view>

    <van-tabbar route active-color="#ee0a24" inactive-color="#000">
      <van-tabbar-item to="/layout/Home" icon="wap-home-o">首页</van-tabbar-item>
      <van-tabbar-item to="/order/PlaceOrder" icon="apps-o">分类页</van-tabbar-item>
      <van-tabbar-item
        to="/layout/Cart"
        icon="shopping-cart-o"
        :badge="badgeValue <= 0 ? '' : badgeValue"
      >
        购物车</van-tabbar-item
      >
      <van-tabbar-item to="/layout/My" icon="user-o">我的</van-tabbar-item>
    </van-tabbar>
  </div>
</template>

<style></style>
