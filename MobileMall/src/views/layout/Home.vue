<script setup>
import GoodsItem from '@/components/GoodsItem.vue'
import { ref, onMounted } from 'vue'
import { getHomeData } from '@/api/Home'
// import { useCartStore } from '@/stores'
import { useRouter } from 'vue-router'

const router = useRouter()
const bannerList = ref([])
const navList = ref([])
const proList = ref([])

// 获取首页数据
const getHomeList = async () => {
  const res = await getHomeData()
  bannerList.value = res.data.pageData.items[0].data
  navList.value = res.data.pageData.items[1].data
  proList.value = res.data.pageData.items[2].data
}

onMounted(() => {
  getHomeList()
  // const cartStore = useCartStore()
  // cartStore.getCartAction() // 获取购物车内容
})
</script>

<template>
  <div class="home">
    <!-- 导航条 -->
    <van-nav-bar title="手机商城" fixed />

    <!-- 搜索框 -->
    <van-search
      readonly
      shape="round"
      background="#f1f1f2"
      placeholder="请在此输入搜索关键词"
      @click="() => router.push({ path: '/search/SearchPage' })"
    />

    <!-- 轮播图 -->
    <van-swipe class="my-swipe" :autoplay="3000" indicator-color="white">
      <van-swipe-item v-for="item in bannerList" :key="item.imgUrl">
        <img :src="item.imgUrl" alt="" />
      </van-swipe-item>
    </van-swipe>

    <!-- 导航 -->
    <van-grid column-num="5" icon-size="40">
      <van-grid-item
        v-for="item in navList"
        :key="item.imgUrl"
        :icon="item.imgUrl"
        :text="item.text"
        @click="$router.push(`/order/PlaceOrder?categoryId=` + item.category_id)"
      />
    </van-grid>

    <!-- 猜你喜欢 -->
    <!-- <div class="guess">
      <p class="guess-title">—— 开始点单 ——</p>

      <van-grid clickable :column-num="2">
        <van-grid-item icon="wap-home-o" text="堂食" @click="$router.push('/order/PlaceOrder')" />
        <van-grid-item icon="bag-o" text="打包" @click="$router.push('/order/PlaceOrder')" />
      </van-grid>
    </div> -->

    <!-- 猜你喜欢 -->
    <div class="guess">
      <p class="guess-title">—— 猜你喜欢 ——</p>

      <div class="goods-list">
        <GoodsItem v-for="item in proList" :key="item.goods_id" :item="item"></GoodsItem>
      </div>
    </div>
  </div>
</template>

<style lang="scss" scoped>
// 主题 padding
.home {
  padding-top: 100px;
  padding-bottom: 50px;
}

// 导航条样式定制
.van-nav-bar {
  z-index: 999;
  background-color: #c21401;
  ::v-deep(.van-nav-bar__title) {
    color: #fff;
  }
}

// 搜索框样式定制
.van-search {
  position: fixed;
  width: 100%;
  top: 46px;
  z-index: 999;
}

// 分类导航部分
.my-swipe .van-swipe-item {
  height: 185px;
  color: #fff;
  font-size: 20px;
  text-align: center;
  background-color: #39a9ed;
}
.my-swipe .van-swipe-item img {
  width: 100%;
  height: 185px;
}

.van-grid {
  margin-top: 10px;
}

// 猜你喜欢
.guess .guess-title {
  height: 40px;
  line-height: 40px;
  text-align: center;
}

// 商品样式
.goods-list {
  background-color: #f6f6f6;
}
</style>
