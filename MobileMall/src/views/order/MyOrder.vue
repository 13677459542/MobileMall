<script setup>
import { ref, onBeforeMount } from 'vue'
import OrderListItem from '@/components/OrderListItem.vue'
import { getMyOrderList } from '@/api/order'
import { useRoute } from 'vue-router'
// const router = useRouter() // 返回router路由实例  通过它可以实现路由跳转
const route = useRoute() // 返回路由信息对象,接收跳转传参时使用

const active = ref(0)
// const page = ref('1')
const listData = ref([])

onBeforeMount(() => {
  active.value = route.query.dataType || 0
  getOrderList()
})

const getOrderList = async () => {
  const {
    data: { list },
  } = await getMyOrderList(active.value)
  list.data.forEach((item) => {
    item.total_num = 0
    item.goods.forEach((goods) => {
      item.total_num += goods.total_num
    })
  })
  listData.value = list.data
}

// // 监听cartTotal
// watch(
//   active,
//   (newValue, oldValue) => {
//     console.info(newValue, oldValue)
//     getOrderList()
//   },
//   // immediate: true 在初始化时立即调用一次回调函数
//   { immediate: true },
// )
</script>

<template>
  <div class="order">
    <van-nav-bar title="我的订单" fixed left-arrow @click-left="$router.go(-1)" />

    <div class="tabsCLass">
      <van-tabs v-model:active="active" sticky @click-tab="getOrderList">
        <van-tab name="0" title="全部"></van-tab>
        <van-tab name="1" title="待支付"></van-tab>
        <van-tab name="2" title="待发货"></van-tab>
        <van-tab name="3" title="待收货"></van-tab>
        <!-- <van-tab name="4" title="待评价"></van-tab> -->
      </van-tabs>
    </div>

    <div class="OrderItem">
      <OrderListItem v-for="item in listData" :key="item.order_id" :item="item"></OrderListItem>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.order {
  width: 100%;
  height: 100vh;
  padding-top: 90px;
  box-sizing: border-box;
  background-color: #fafafa;
}
.tabsCLass {
  position: fixed;
  width: 100%;
  top: 46px;
  left: 0;
}
.van-tabs {
  position: sticky;
  top: 0;
}
.OrderItem {
  overflow: auto;
  height: 100%;
}
</style>
