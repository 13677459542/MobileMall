<script setup>
import { ref, onBeforeMount } from 'vue'
import { getCategoryData } from '@/api/category'
import GoodsItem from '@/components/GoodsItem.vue'
import { showToast } from 'vant'
import { useRoute } from 'vue-router'
// const router = useRouter() // 返回router路由实例  通过它可以实现路由跳转
const route = useRoute() // 返回路由信息对象,接收跳转传参时使用
const Datalist = ref([])
const activeIndex = ref(0)
const GoodsList = ref([])
const proList = ref([])

const onClickLeft = async () => {
  history.back()
}

const getCategoryList = async () => {
  const res = await getCategoryData()
  Datalist.value = res.data.pageData.items[0].data
  GoodsList.value = res.data.pageData.items[1].data
}

const CategoryClick = (index) => {
  activeIndex.value = index
  proList.value = GoodsList.value.filter((item) => item.category_id == activeIndex.value) // 因为可能是首页分类导航过来的类型不为数字，所以这里不适用===而是用==
  if (proList.value.length <= 0) showToast('未查询到相关商品')
}

onBeforeMount(async () => {
  await getCategoryList()
  if (route.query.categoryId) activeIndex.value = route.query.categoryId
  else activeIndex.value = Datalist.value[0].category_id
  CategoryClick(activeIndex.value)
})
</script>

<template>
  <div class="category">
    <!-- 分类 -->
    <!-- <van-nav-bar title="下单" fixed /> -->
    <van-nav-bar title="手机商城" left-text="返回" left-arrow @click-left="onClickLeft" fixed />

    <!-- 搜索框 -->
    <van-search
      readonly
      shape="round"
      background="#f1f1f2"
      placeholder="请输入搜索关键词"
      @click="$router.push('/search/SearchPage')"
    />

    <!-- 分类列表 -->
    <div class="list-box">
      <div class="left">
        <ul>
          <li v-for="item in Datalist" :key="item.category_id">
            <a
              :class="{ active: item.category_id == activeIndex }"
              @click="CategoryClick(item.category_id)"
              href="javascript:;"
              >{{ item.text }}</a
            >
          </li>
        </ul>
      </div>
      <div class="right">
        <div class="goods-list">
          <GoodsItem v-for="item in proList" :key="item.goods_id" :item="item"></GoodsItem>
        </div>
      </div>
    </div>
  </div>
</template>

<style lang="scss" scoped>
// 主题 padding
.category {
  padding-top: 100px;
  padding-bottom: 50px;
  height: 100vh;
  box-sizing: border-box;
  overflow: hidden;
  .list-box {
    height: 100%;
    display: flex;
    .left {
      width: 85px;
      height: 100%;
      background-color: #f3f3f3;
      overflow: auto;
      a {
        display: block;
        height: 45px;
        line-height: 45px;
        text-align: center;
        color: #444444;
        font-size: 12px;
        &.active {
          color: #fb442f;
          background-color: #fff;
        }
      }
    }
    .right {
      flex: 1;
      height: 100%;
      background-color: #ffffff;
      display: flex;
      flex-wrap: wrap;
      justify-content: flex-start;
      align-content: flex-start;
      padding: 10px 0;
      overflow: auto;

      .goods-list {
        width: 100%;
      }
    }
  }
}

// 导航条样式定制
.van-nav-bar {
  z-index: 999;
}

// 搜索框样式定制
.van-search {
  position: fixed;
  width: 100%;
  top: 46px;
  z-index: 999;
}
</style>
