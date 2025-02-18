<script setup>
import { ref, onBeforeMount } from 'vue'
import GoodsItem from '@/components/GoodsItem.vue'
import { getProList } from '@/api/product'
import { useRouter, useRoute } from 'vue-router'
import { showToast } from 'vant'
const router = useRouter() // 返回router路由实例  通过它可以实现路由跳转
const route = useRoute() // 返回路由信息对象,接收跳转传参时使用

const page = ref(1)
const proList = ref([])
const searchStr = ref()

// DOM 元素尚未被插入到页面中时触发
onBeforeMount(async () => {
  console.log(
    `接收到的参数,searchkey: ${route.query.searchkey} ,categoryId:${route.query.categoryId}`,
  )
  searchStr.value = route.query.searchkey
  const resp = await getProList({
    categoryId: route.query.categoryId,
    goods_name: searchStr.value,
    page: page.value,
  })
  proList.value = resp.data
  if (proList.value.length <= 0) {
    showToast('未查询到相关商品')
  }
})
</script>

<template>
  <div class="search">
    <van-nav-bar fixed title="商品列表" left-arrow @click-left="$router.go(-1)" />

    <van-search
      readonly
      shape="round"
      background="#ffffff"
      v-model="searchStr"
      show-action
      @click="() => router.push({ path: '/search/SearchPage' })"
    >
      <template #action>
        <van-icon class="tool" name="apps-o" />
      </template>
    </van-search>

    <!-- 排序选项按钮 -->
    <div class="sort-btns">
      <div class="sort-item">综合</div>
      <div class="sort-item">销量</div>
      <div class="sort-item">价格</div>
    </div>

    <div class="goods-list">
      <GoodsItem v-for="item in proList" :key="item.goods_id" :item="item"></GoodsItem>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.search {
  padding-top: 46px;
  ::v-deep .van-icon-arrow-left {
    color: #333;
  }
  .tool {
    font-size: 24px;
    height: 40px;
    line-height: 40px;
  }

  .sort-btns {
    display: flex;
    height: 36px;
    line-height: 36px;
    .sort-item {
      text-align: center;
      flex: 1;
      font-size: 16px;
    }
  }
}

// 商品样式
.goods-list {
  background-color: #f6f6f6;
}
</style>
