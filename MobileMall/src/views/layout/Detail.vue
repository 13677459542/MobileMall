<script setup>
import { ref, onBeforeMount } from 'vue'
import { getProComments, getProDetail } from '@/api/product'
import defaultImg from '@/assets/default-avatar.png'
import CountBox from '@/components/CountBox.vue'
import { addCart } from '@/api/cart'
import { useMixin } from '@/mixins/loginConfirm'
import { showToast } from 'vant'
import { useRouter, useRoute } from 'vue-router'
import { useCartStore } from '@/stores'
// const userStore = useUserStore()
const cartStore = useCartStore()
const router = useRouter() // 返回router路由实例  通过它可以实现路由跳转
const route = useRoute() // 返回路由信息对象,接收跳转传参时使用

const goodsId = ref(0) // 商品id
const images = ref([]) // 商品轮播图片地址
const current = ref(0) // 商品当前轮播图片下标
const detailData = ref({}) // 商品明细数据
const assessTotal = ref(0) // 评价总数
const commentList = ref([]) // 评价列表
// const defaultImgData = ref()
const showPannel = ref(false) // 控制 加入购物车/立即购买 公用的弹层的显示隐藏
const mode = ref('cart') // 标记弹层状态
const addCount = ref(1) // 数字框绑定的数据
const badgeValue = ref(0) // 购物车角标
const isShowTips = ref(false) // 是否显示存在购物车提示

const onChange = (index) => {
  current.value = index
}

const getDetail = async () => {
  const resp = await getProDetail(goodsId.value)
  detailData.value = resp.data
  images.value = resp.data.goods_images
  detailData.value.content = detailData.value.content.replace(/\\\\"/g, '"')
}

const getComments = async () => {
  const respComments = await getProComments(goodsId.value, 3)
  if (respComments.status === 200) {
    commentList.value = respComments.data
    assessTotal.value = respComments.data.length
  }
}

const addFn = () => {
  try {
    // goodsId.value 的类型不是number,不能使用 === 而是使用 ==
    const goodsInfo = cartStore.cartList.find((item) => item.goods_id == goodsId.value)
    if (goodsInfo) {
      addCount.value = goodsInfo.goods_num // 如果已经加入购物车，需要获取加入的数量
      isShowTips.value = true
    }
    mode.value = 'cart'
    showPannel.value = true
  } catch (error) {
    console.info(error)
  }
}

const buyNow = () => {
  isShowTips.value = false
  mode.value = 'buyNow'
  showPannel.value = true
}

const addCartApi = async () => {
  const { loginConfirm } = useMixin()
  if (loginConfirm()) {
    return
  }
  // const { data } = await addCart(
  //   goodsId.value,
  //   addCount.value,
  //   detailData.value.skuList[0].goods_sku_id,
  // )
  // badgeValue.value = data.badgeValue
  const resp = await addCart(
    goodsId.value,
    addCount.value,
    // detailData.value.skuList[0].goods_sku_id,
  )
  if (resp.status === 200) {
    badgeValue.value = resp.data.cartTotal // 获取加入购物车后返回的商品数量
    cartStore.changeCount(goodsId.value, addCount.value) // 加入购物车成功后修改pinia中cart的数量
    showToast('加入购物车成功')
  } else showToast('加入购物车失败')
  showPannel.value = false
}

const goBuyNow = () => {
  const { loginConfirm } = useMixin()
  if (loginConfirm()) {
    return
  }
  router.push({
    path: '/order/PaymentOrder',
    query: {
      mode: 0,
      goodsId: goodsId.value,
      // goodsSkuId: detailData.value.skuList[0].goods_sku_id,
      goodsNum: addCount.value,
    },
  })
}

onBeforeMount(() => {
  goodsId.value = route.query.goods_id
  getDetail()
  getComments()
  badgeValue.value = cartStore.cartTotal() // 获取加入购物车商品数量
})
</script>

<template>
  <div class="prodetail">
    <van-nav-bar fixed title="商品详情页" left-arrow @click-left="$router.go(-1)" />

    <van-swipe :autoplay="4000" @change="onChange">
      <van-swipe-item v-for="(image, index) in images" :key="index">
        <img :src="image.imageUrl" />
      </van-swipe-item>

      <template #indicator>
        <div class="custom-indicator">{{ current + 1 }} / {{ images.length }}</div>
      </template>
    </van-swipe>

    <!-- 商品说明 -->
    <div class="info">
      <div class="title">
        <div class="price">
          <span class="now">￥{{ detailData.goods_price_min }}</span>
          <span class="oldprice">￥{{ detailData.goods_price_max }}</span>
        </div>
        <div class="sellcount">已售 {{ detailData.goods_sales }} 件</div>
      </div>
      <div class="msg text-ellipsis-2">
        {{ detailData.goods_name }}
      </div>

      <div class="service">
        <div class="left-words">
          <span><van-icon name="passed" />七天无理由退货</span>
          <span><van-icon name="passed" />48小时发货</span>
        </div>
        <div class="right-icon">
          <van-icon name="arrow" />
        </div>
      </div>
    </div>

    <!-- 商品评价 -->
    <div class="comment">
      <div class="comment-title">
        <div class="left">商品评价 ({{ assessTotal }}条)</div>
        <div class="right">查看更多 <van-icon name="arrow" /></div>
      </div>
      <div class="comment-list">
        <div class="comment-item" v-for="item in commentList" :key="item.comment_id">
          <div class="top">
            <img :src="item.avatar_url || defaultImg" alt="" />
            <div class="name">{{ item.nick_name }}</div>
            <van-rate
              :size="16"
              v-model="item.score"
              color="#ffd21e"
              void-icon="star"
              void-color="#eee"
            />
          </div>
          <div class="content">
            {{ item.content }}
          </div>
          <div class="time">
            {{ item.create_time }}
          </div>
        </div>
      </div>
    </div>

    <!-- 商品描述 -->
    <div class="desc" v-html="detailData.content"></div>

    <!-- 底部 -->
    <div class="footer">
      <div @click="$router.push('/')" class="icon-home">
        <van-icon name="wap-home-o" />
        <span>首页</span>
      </div>
      <div @click="$router.push('/layout/Cart')" class="icon-cart">
        <!-- <span v-if="badgeValue > 0" class="num">{{ badgeValue }}</span> -->
        <van-icon name="shopping-cart-o" :badge="badgeValue <= 0 ? '' : badgeValue" />
        <span>购物车</span>
      </div>
      <div @click="addFn" class="btn-add">加入购物车</div>
      <div @click="buyNow" class="btn-buy">立刻购买</div>
    </div>

    <!-- 加入购物车/立即购买 公用的弹层 -->
    <van-action-sheet
      v-model:show="showPannel"
      :title="mode === 'cart' ? '加入购物车' : '立刻购买'"
    >
      <div class="product">
        <div class="product-title">
          <div class="left">
            <img :src="detailData.goods_image" alt="" />
          </div>
          <div class="right">
            <div class="price">
              <span>¥</span>
              <span class="nowprice">{{ detailData.goods_price_min }}</span>
            </div>
            <div class="count">
              <span>库存</span>
              <span>{{ detailData.stock_total }}</span>
            </div>
          </div>
        </div>
        <div class="num-box">
          <span
            >数量<span class="number_Tips">{{
              isShowTips ? '(该商品已在购物车中，请修改数量)' : ''
            }}</span></span
          >
          <!-- v-model 本质上 :value 和 @input 的简写 -->
          <CountBox v-model="addCount"></CountBox>
        </div>

        <!-- 有库存才显示提交按钮 -->
        <!-- <div class="showbtn" v-if="detailData.stock_total > 0"> -->
        <div class="showbtn">
          <div class="btn" v-if="mode === 'cart'" @click="addCartApi">加入购物车</div>
          <div class="btn now" v-else @click="goBuyNow">立刻购买</div>
        </div>

        <!-- <div class="btn-none" v-else>该商品已抢完</div> -->
      </div>
    </van-action-sheet>
  </div>
</template>

<style lang="scss" scoped>
.prodetail {
  padding-top: 46px;
  // 深度作用选择器，因为vant的样式是全局的。组件中使用了 scoped 样式只对当前组件生效，但是仍然需要对这些全局样式进行修改。
  ::v-deep(.van-icon-arrow-left) {
    color: #333;
  }
  img {
    display: block;
    width: 100%;
  }
  .custom-indicator {
    position: absolute;
    right: 10px;
    bottom: 10px;
    padding: 5px 10px;
    font-size: 12px;
    background: rgba(0, 0, 0, 0.1);
    border-radius: 15px;
  }
  .desc {
    width: 100%;
    overflow: scroll;
    // 深度作用选择器
    ::v-deep(img) {
      display: block;
      width: 100% !important;
    }
  }
  .info {
    padding: 10px;
  }
  .title {
    display: flex;
    justify-content: space-between;
    .now {
      color: #fa2209;
      font-size: 20px;
    }
    .oldprice {
      color: #959595;
      font-size: 16px;
      text-decoration: line-through;
      margin-left: 5px;
    }
    .sellcount {
      color: #959595;
      font-size: 16px;
      position: relative;
      top: 4px;
    }
  }
  .msg {
    font-size: 16px;
    line-height: 24px;
    margin-top: 5px;
  }
  .service {
    display: flex;
    justify-content: space-between;
    line-height: 40px;
    margin-top: 10px;
    font-size: 16px;
    background-color: #fafafa;
    .left-words {
      span {
        margin-right: 10px;
      }
      .van-icon {
        margin-right: 4px;
        color: #fa2209;
      }
    }
  }

  .comment {
    padding: 10px;
    .comment-title {
      display: flex;
      justify-content: space-between;
      .right {
        color: #959595;
      }
    }
    .comment-list {
      .comment-item {
        font-size: 16px;
        line-height: 30px;
        .top {
          height: 30px;
          display: flex;
          align-items: center;
          margin-top: 20px;
          img {
            width: 20px;
            height: 20px;
          }
          .name {
            margin: 0 10px;
          }
        }
        .content {
          overflow: hidden; /* 隐藏超出部分 */
          // display: -webkit-box; /* 使用弹性盒子布局 */
          // -webkit-box-orient: vertical; /* 设置盒子方向为垂直 */
          // -webkit-line-clamp: 3; /* 限制显示的行数为3行 */
          // text-overflow: ellipsis; /* 添加省略号 */
          // line-height: 1.5; /* 设置行高，以确保行数计算正确 */
          // max-height: 4.5em; /* 根据行高设置最大高度，3行 */
        }
        .time {
          color: #999;
        }
      }
    }
  }

  .footer {
    position: fixed;
    left: 0;
    bottom: 0;
    width: 100%;
    height: 55px;
    background-color: #fff;
    border-top: 1px solid #ccc;
    display: flex;
    justify-content: space-evenly;
    align-items: center;
    .icon-home,
    .icon-cart {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      font-size: 14px;
      .van-icon {
        font-size: 24px;
      }
    }
    .btn-add,
    .btn-buy {
      height: 36px;
      line-height: 36px;
      width: 120px;
      border-radius: 18px;
      background-color: #ffa900;
      text-align: center;
      color: #fff;
      font-size: 14px;
    }
    .btn-buy {
      background-color: #fe5630;
    }
  }
}

.tips {
  padding: 10px;
}

// 弹层的样式
.product {
  .product-title {
    display: flex;
    .left {
      img {
        width: 90px;
        height: 90px;
      }
      margin: 10px;
    }
    .right {
      flex: 1;
      padding: 10px;
      .price {
        font-size: 14px;
        color: #fe560a;
        .nowprice {
          font-size: 24px;
          margin: 0 5px;
        }
      }
    }
  }

  .num-box {
    display: flex;
    justify-content: space-between;
    padding: 10px;
    align-items: center;
    .number_Tips {
      color: #fe560a;
      font-size: 13px;
    }
  }

  .btn,
  .btn-none {
    height: 40px;
    line-height: 40px;
    margin: 20px;
    border-radius: 20px;
    text-align: center;
    color: rgb(255, 255, 255);
    background-color: rgb(255, 148, 2);
  }
  .btn.now {
    background-color: #fe5630;
  }
  .btn-none {
    background-color: #cccccc;
  }
}

.footer .icon-cart {
  position: relative;
  padding: 0 6px;
  .num {
    z-index: 999;
    position: absolute;
    top: -2px;
    right: 0;
    min-width: 16px;
    padding: 0 4px;
    color: #fff;
    text-align: center;
    background-color: #ee0a24;
    border-radius: 50%;
  }
}
</style>
