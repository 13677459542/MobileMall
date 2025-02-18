<script>
import { useRouter } from 'vue-router'
export default {
  name: 'GoodsItem',
  props: {
    item: {
      type: Object,
      // 这里设置默认值为函数，里面返回一个空对象
      default: () => {
        return {}
      },
    },
  },
  setup() {
    const router = useRouter() // 获取路由实例
    const navigateToDetail = (id) => {
      // 路由跳转逻辑
      router.push({ path: '/layout/Detail', query: { goods_id: id } })
    }
    return {
      navigateToDetail,
    }
  },
}
</script>

<template>
  <div v-if="item.goods_id" class="goods-item" @click="navigateToDetail(item.goods_id)">
    <div class="left">
      <img :src="item.goods_image" alt="" />
    </div>
    <div class="right">
      <div class="tit text-ellipsis-2">
        {{ item.goods_name }}
      </div>
      <p class="count">已售 {{ item.goods_sales }} 件</p>
      <p class="price">
        <span class="new">¥{{ item.goods_price_min }}</span>
        <span class="old">¥{{ item.goods_price_max }}</span>
      </p>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.goods-item {
  height: 148px;
  margin-bottom: 6px;
  padding: 10px;
  background-color: #fff;
  display: flex;
  .left {
    width: 127px;
    img {
      display: block;
      width: 100%;
    }
  }
  .right {
    flex: 1;
    font-size: 14px;
    line-height: 1;
    padding: 0 10px;
    display: flex;
    flex-direction: column;
    justify-content: space-evenly;
    overflow: hidden;

    .tit {
      display: -webkit-box; /* 使用弹性盒子布局 */
      -webkit-box-orient: vertical; /* 设置盒子方向为垂直 */
      -webkit-line-clamp: 3; /* 限制显示的行数为3行 */
      overflow: hidden; /* 隐藏超出部分 */
      text-overflow: ellipsis; /* 添加省略号 */
      line-height: 1.5; /* 设置行高，以确保行数计算正确 */
      max-height: 4.5em; /* 根据行高设置最大高度，3行 */
    }
    .count {
      color: #999;
      font-size: 12px;
    }
    .price {
      color: #999;
      font-size: 16px;
      .new {
        color: #f03c3c;
        margin-right: 10px;
      }
      .old {
        text-decoration: line-through;
        font-size: 12px;
      }
    }
  }
}
</style>
