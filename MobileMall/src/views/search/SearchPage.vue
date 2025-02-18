<script setup>
import { ref, onMounted } from 'vue'
import { useUserStore } from '@/stores'
import { showToast } from 'vant'
import { useRouter } from 'vue-router'
const userStore = useUserStore()
const router = useRouter() // 返回router路由实例  通过它可以实现路由跳转

const search = ref('')
const history = ref([])

// DOM 元素被插入到页面后触发
onMounted(() => {
  history.value = userStore.historyList
})

const goSearch = (key) => {
  key = key.trim()
  if (!key) {
    search.value = ''
    showToast('请输入搜索内容有效的关键词')
    return
  }
  // console.log('进行了搜索，搜索历史要更新', key)
  const index = history.value.indexOf(key) // 查找key的值在history的下标
  if (index !== -1) {
    // 存在相同的项，将原有关键字移除
    // splice(从哪开始, 删除几个, 项1, 项2)
    history.value.splice(index, 1)
  }
  history.value.unshift(key) // 在history最前方追加项
  userStore.setHistoryList(history.value)

  // 跳转到搜索列表页
  router.push({
    path: '/search/SearchList',
    query: { searchkey: key },
  })
}

const clear = () => {
  history.value = []
  userStore.setHistoryList([])
}
</script>

<template>
  <div class="search">
    <van-nav-bar title="商品搜索" left-arrow @click-left="$router.go(-1)" />

    <van-search v-model="search" show-action placeholder="请输入搜索关键词" clearable>
      <template #action>
        <div @click="goSearch(search)">搜索</div>
      </template>
    </van-search>

    <!-- 搜索历史 -->
    <div class="search-history" v-if="history && history.length > 0">
      <div class="title">
        <span>最近搜索</span>
        <van-icon @click="clear" name="delete-o" size="16" />
      </div>
      <div class="list">
        <div v-for="item in history" :key="item" class="list-item" @click="goSearch(item)">
          {{ item }}
        </div>
      </div>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.search {
  .searchBtn {
    background-color: #fa2209;
    color: #fff;
  }
  ::v-deep .van-search__action {
    background-color: #c21401;
    color: #fff;
    padding: 0 20px;
    border-radius: 0 5px 5px 0;
    margin-right: 10px;
  }
  ::v-deep .van-icon-arrow-left {
    color: #333;
  }
  .title {
    height: 40px;
    line-height: 40px;
    font-size: 14px;
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0 15px;
  }
  .list {
    display: flex;
    justify-content: flex-start;
    flex-wrap: wrap;
    padding: 0 10px;
    gap: 5%;
  }
  .list-item {
    width: 30%;
    text-align: center;
    padding: 7px;
    line-height: 15px;
    border-radius: 50px;
    background: #fff;
    font-size: 13px;
    border: 1px solid #efefef;
    overflow: hidden;
    white-space: nowrap;
    text-overflow: ellipsis;
    margin-bottom: 10px;
  }
}
</style>
