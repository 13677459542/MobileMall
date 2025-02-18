<script setup>
import { ref, onMounted } from 'vue'
import { useUserStore, useCartStore } from '@/stores'
import { showToast } from 'vant'
import { useRouter } from 'vue-router'
// import CountBox from '@/components/CountBox.vue'
const isEdit = ref(false)
const isLogin = ref(false)
const allChecked = ref(false)
const userStore = useUserStore()
const cartStore = useCartStore()
const router = useRouter()

// 生命周期钩子,页面加载完成后调用
onMounted(() => {
  if (userStore.userInfo.token) {
    isLogin.value = true
    cartStore.getCartAction()
    allChecked.value = cartStore.isAllChecked
  }
})

const toggleCheckCart = () => {
  // cartStore.toggleCheck(goodsId)
  allChecked.value = cartStore.isAllChecked()
}
const toggleAllCheckCart = () => {
  const check = !cartStore.isAllChecked()
  cartStore.toggleAllCheck(check)
  allChecked.value = check
}

// 更新购物车数量
const changeCount = (goodsNum, goodsId, goodsSkuId) => {
  if (isNaN(goodsNum) || goodsNum < 1) return
  cartStore.changeCountAction({ goodsNum, goodsId, goodsSkuId })
}

// 删除
const handleDel = async () => {
  if (cartStore.selCount() <= 0) {
    console.info('未选中商品，不可删除')
    return
  }
  await cartStore.delSelect()
  isEdit.value = false
}

// 去结算
const goPay = () => {
  // 判断有没有选中商品
  if (cartStore.selCount() > 0) {
    // 有选中的 商品 才进行结算跳转
    router.push({
      path: '/order/PaymentOrder',
      query: {
        mode: 1,
        cartIds: cartStore.selCartList().map((item) => item.cart_id),
        // .join(','), // 'cartId,cartId,cartId'
      },
    })
  } else {
    console.info('未选中商品，不可结算')
  }
}

// // 监听
// watch(isEdit, (value) => {
//   if (value) {
//     cartStore.toggleAllCheck(false)
//   } else {
//     cartStore.toggleAllCheck(true)
//   }
// })
</script>

<template>
  <div class="cart">
    <van-nav-bar title="购物车" fixed />

    <div v-if="isLogin && cartStore.cartList.length > 0" class="cart-content">
      <!-- 购物车开头 -->
      <div class="cart-title">
        <span class="all"
          >共<i>{{ cartStore.cartTotal() }}</i
          >件商品</span
        >
        <span class="edit" @click="isEdit = !isEdit">
          <van-icon name="edit" />
          {{ isEdit ? '退出编辑' : '编辑' }}
        </span>
      </div>

      <!-- 购物车列表 -->
      <div class="cart-list">
        <div class="cart-item" v-for="item in cartStore.cartList" :key="item.goods_id">
          <van-checkbox @click="toggleCheckCart()" v-model="item.isChecked"></van-checkbox>
          <!-- <van-checkbox v-model="item.isChecked"></van-checkbox> -->
          <div class="show">
            <img :src="item.goods.goods_image" alt="" />
          </div>
          <div class="info">
            <span class="tit text-ellipsis-2">{{ item.goods.goods_name }}</span>
            <span class="bottom">
              <div class="price">
                ¥ <span>{{ item.goods.goods_price_min }}</span>
              </div>
              <!-- 既希望保留原本的形参，又需要通过调用函数传参 => 箭头函数包装一层 -->
              <!-- <CountBox
                @input="(modelValue) => changeCount(modelValue, item.goods_id, item.goods_sku_id)"
                v-model="item.goods_num"
              ></CountBox> -->

              <!-- 进步器  integer：限制输入整数 -->
              <van-stepper
                v-model="item.goods_num"
                integer
                @change="changeCount(item.goods_num, item.goods_id, item.goods_sku_id)"
              />
            </span>
          </div>
        </div>
      </div>

      <div class="footer-fixed">
        <div @click="toggleAllCheckCart" class="all-check">
          <van-checkbox v-model="allChecked" icon-size="18"></van-checkbox>
          全选
        </div>

        <div class="all-total">
          <div class="price">
            <span>合计：</span>
            <span
              >¥ <i class="totalPrice">{{ cartStore.selPrice() }}</i></span
            >
          </div>
          <div
            v-if="!isEdit"
            class="goPay"
            :class="{ disabled: cartStore.selCount() === 0 }"
            @click="goPay"
          >
            结算({{ cartStore.selCount() }})
          </div>
          <div
            v-else
            @click="handleDel"
            class="delete"
            :class="{ disabled: cartStore.selCount() === 0 }"
          >
            删除({{ cartStore.selCount() }})
          </div>
        </div>
      </div>
    </div>

    <div class="empty-cart" v-else>
      <img src="@/assets/empty.png" alt="" />
      <div class="tips">您的购物车是空的, 快去逛逛吧</div>
      <div class="btn" @click="$router.push('/')">去逛逛</div>
    </div>
  </div>
</template>

<style lang="scss" scoped>
// 主题 padding
.cart {
  padding-top: 46px;
  padding-bottom: 100px;
  background-color: #f5f5f5;
  // min-height: 100vh;
  height: 100vh;
  box-sizing: border-box;
  overflow: hidden;
  .cart-content {
    height: 100%;
    // overflow: hidden;
    // padding-bottom: 100px;
    .cart-title {
      height: 40px;
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 0 10px;
      font-size: 14px;
      .all {
        i {
          font-style: normal;
          margin: 0 2px;
          color: #fa2209;
          font-size: 16px;
        }
      }
      .edit {
        .van-icon {
          font-size: 18px;
        }
      }
    }

    .cart-list {
      height: 100%;
      overflow: auto;
      box-sizing: border-box;
      padding-bottom: 40px;
      .cart-item {
        margin: 0 10px 10px 10px;
        padding: 10px;
        display: flex;
        justify-content: space-between;
        background-color: #ffffff;
        border-radius: 5px;

        .show img {
          width: 100px;
          height: 100px;
        }
        .info {
          width: 210px;
          padding: 10px 5px;
          font-size: 14px;
          display: flex;
          flex-direction: column;
          justify-content: space-between;

          .bottom {
            display: flex;
            justify-content: space-between;
            .price {
              display: flex;
              align-items: flex-end;
              color: #fa2209;
              font-size: 12px;
              span {
                font-size: 16px;
              }
            }
          }
        }
      }
    }
  }
}

.footer-fixed {
  position: fixed;
  left: 0;
  bottom: 50px;
  height: 50px;
  width: 100%;
  border-bottom: 1px solid #ccc;
  background-color: #fff;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 10px;

  .all-check {
    display: flex;
    align-items: center;
    .van-checkbox {
      margin-right: 5px;
    }
  }

  .all-total {
    display: flex;
    line-height: 36px;
    .price {
      font-size: 14px;
      margin-right: 10px;
      .totalPrice {
        color: #fa2209;
        font-size: 18px;
        font-style: normal;
      }
    }

    .goPay,
    .delete {
      min-width: 100px;
      height: 36px;
      line-height: 36px;
      text-align: center;
      background-color: #fa2f21;
      color: #fff;
      border-radius: 18px;
      &.disabled {
        background-color: #ff9779;
      }
    }
  }
}

.empty-cart {
  padding: 80px 30px;
  img {
    width: 140px;
    height: 92px;
    display: block;
    margin: 0 auto;
  }
  .tips {
    text-align: center;
    color: #666;
    margin: 30px;
  }
  .btn {
    width: 110px;
    height: 32px;
    line-height: 32px;
    text-align: center;
    background-color: #fa2c20;
    border-radius: 16px;
    color: #fff;
    display: block;
    margin: 0 auto;
  }
}
</style>
