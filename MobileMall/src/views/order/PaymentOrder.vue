<script setup>
import { ref, onBeforeMount } from 'vue'
import { checkOrder, submitOrder } from '@/api/order'
// import { getAddressList } from '@/api/address'
import { useMixin } from '@/mixins/loginConfirm'
import { showToast } from 'vant'
import { useRouter, useRoute } from 'vue-router'
// import { useCartStore } from '@/stores'
// const userStore = useUserStore()
// const cartStore = useCartStore()
const router = useRouter() // 返回router路由实例  通过它可以实现路由跳转
const route = useRoute() // 返回路由信息对象,接收跳转传参时使用

// const addressList = ref([]) // 地址数据
const orderData = ref({}) // 订单结算确认接口返回的订单数据
const personalData = ref({}) // 订单结算确认接口返回的用户数据
const remark = ref('') // 备注
const payTypeStr = ref('1')

// const selectedAddress = () => {
//   console.log(addressList.value[0])
//   // 这里地址管理非主线业务，直接获取第一个项作为选中的地址
//   return addressList.value[0] || {}
// }
// const longAddress = () => {
//   const region = selectedAddress().region
//   return region.province + region.city + region.region + selectedAddress().detail
// }

const submitOrderApi = async () => {
  const { loginConfirm } = useMixin()
  if (loginConfirm()) {
    showToast('您当前未登录，请登陆后操作！')
    return
  }
  const submitResp = ref({})
  if (route.query.mode == 1) {
    submitResp.value = await submitOrder(route.query.mode, {
      cartIds: route.query.cartIds,
      payType: payTypeStr.value,
      delivery: 0,
      couponId: 0,
      isUsePoints: 0,
      remark: remark.value,
    })
  }
  if (route.query.mode == 0) {
    submitResp.value = await submitOrder(route.query.mode, {
      goodsId: route.query.goodsId,
      goodsSkuId: route.query.goodsSkuId,
      goodsNum: route.query.goodsNum,
      payType: payTypeStr.value,
      delivery: 0,
      couponId: 0,
      isUsePoints: 0,
      remark: remark.value,
    })
  }
  if (submitResp.value.status === 200) {
    showToast('下单成功')
    router.replace('/order/MyOrder')
  }
}

// const getAddressListApi = async () => {
//   const {
//     data: { list },
//   } = await getAddressList()
//   addressList.value = list
// }

const getOrderList = async () => {
  // 购物车结算
  if (route.query.mode == 1) {
    const {
      data: { order, personal },
    } = await checkOrder(route.query.mode, {
      cartIds: route.query.cartIds,
    })
    orderData.value = order
    personalData.value = personal
  }
  // 立刻购买结算
  else if (route.query.mode == 0) {
    const {
      data: { order, personal },
    } = await checkOrder(route.query.mode, {
      goodsId: route.query.goodsId,
      // goodsSkuId: route.query.goodsSkuId,
      goodsNum: route.query.goodsNum,
    })
    orderData.value = order
    personalData.value = personal
  }
  if (orderData.value.orderTotalPrice < personalData.value.balance) payTypeStr.value = '1'
  else payTypeStr.value = '0'
}

onBeforeMount(() => {
  // getAddressListApi()
  getOrderList()
})
</script>

<template>
  <div class="pay">
    <van-nav-bar fixed title="订单结算台" left-arrow @click-left="$router.go(-1)" />

    <!-- 订单明细 -->
    <div class="pay-list" v-if="orderData.goodsList">
      <!-- 地址相关 -->
      <!-- <div class="address">
        <div class="left-icon">
          <van-icon name="logistics" />
        </div>

        <div class="info" v-if="selectedAddress.address_id">
          <div class="info-content">
            <span class="name">{{ selectedAddress.name }}</span>
            <span class="mobile">{{ selectedAddress.phone }}</span>
          </div>
          <div class="info-address">
            {{ longAddress }}
          </div>
        </div>

        <div class="info" v-else>请选择配送地址</div>

        <div class="right-icon">
          <van-icon name="arrow" />
        </div>
      </div> -->

      <div class="list">
        <div class="goods-item" v-for="item in orderData.goodsList" :key="item.goods_id">
          <div class="left">
            <img :src="item.goods_image" alt="" />
          </div>
          <div class="right">
            <div class="tit text-ellipsis-2">
              {{ item.goods_name }}
            </div>
            <p class="info">
              <span class="count">x{{ item.total_num }}</span>
              <span class="price">¥{{ item.total_pay_price }}</span>
            </p>
          </div>
        </div>
      </div>

      <div class="flow-num-box">
        <span>共 {{ orderData.orderTotalNum }} 件商品，合计：</span>
        <span class="money">￥{{ orderData.orderTotalPrice }}</span>
      </div>

      <div class="pay-detail">
        <div class="pay-cell">
          <span>订单总金额：</span>
          <span class="red">￥{{ orderData.orderTotalPrice }}</span>
        </div>

        <div class="pay-cell">
          <span>优惠券：</span>
          <span>无优惠券可用</span>
        </div>

        <div class="pay-cell">
          <span>配送费用(功能未开放)：</span>
          <!-- <span v-if="!selectedAddress">请先选择配送地址</span>
          <span v-else class="red">+￥0.00</span> -->
          <span class="red">+￥0.00</span>
        </div>
      </div>

      <!-- 支付方式 -->
      <div class="pay-way">
        <span class="tit">支付方式</span>
        <div class="pay-type">
          <!-- <span><van-icon name="balance-o" />余额支付（可用 ¥ {{ personalData.balance }} 元）</span>
          <span class="red"><van-icon name="passed" /></span> -->
          <van-radio-group v-model="payTypeStr">
            <van-cell-group inset>
              <van-cell title="现金支付（收银台处付款）" clickable @click="payTypeStr = '0'">
                <template #right-icon>
                  <van-radio name="0" />
                </template>
              </van-cell>
              <van-cell
                :title="
                  '余额支付（可用 ¥' +
                  personalData.balance +
                  '元' +
                  (orderData.orderTotalPrice > personalData.balance ? '，余额不足' : '') +
                  '）'
                "
                clickable
                @click="payTypeStr = '1'"
              >
                <template #right-icon>
                  <van-radio name="1" />
                </template>
              </van-cell>
            </van-cell-group>
          </van-radio-group>
        </div>
      </div>

      <!-- 买家留言 -->
      <div class="buytips">
        <textarea
          v-model="remark"
          placeholder="选填：买家留言（50字内）"
          name=""
          id=""
          cols="30"
          rows="10"
        ></textarea>
      </div>
    </div>

    <!-- 底部提交 -->
    <div class="footer-fixed">
      <div class="left">
        实付款：<span>￥{{ orderData.orderTotalPrice }}</span>
      </div>
      <div class="tipsbtn" @click="submitOrderApi">提交订单</div>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.pay {
  height: 100vh;
  box-sizing: border-box;
  padding: 46px 0px;
  // overflow: hidden;
  ::v-deep(.van-nav-bar__arrow) {
    color: #333;
  }

  .pay-list {
    height: 100%;
    box-sizing: border-box;
    overflow: auto;

    // .address {
    //   display: flex;
    //   align-items: center;
    //   justify-content: flex-start;
    //   padding: 20px;
    //   font-size: 14px;
    //   color: #666;
    //   position: relative;
    //   background: url(@/assets/border-line.png) bottom repeat-x;
    //   background-size: 60px auto;
    //   .left-icon {
    //     margin-right: 20px;
    //   }
    //   .right-icon {
    //     position: absolute;
    //     right: 20px;
    //     top: 50%;
    //     transform: translateY(-7px);
    //   }
    // }

    .goods-item {
      height: 100px;
      margin: 6px 0px 6px 0px;
      padding: 10px;
      background-color: #fff;
      display: flex;
      .left {
        width: 100px;
        img {
          display: block;
          width: 80px;
          margin: 10px auto;
        }
      }
      .right {
        flex: 1;
        font-size: 14px;
        line-height: 1.3;
        padding: 10px;
        padding-right: 0px;
        color: #333;
        display: flex;
        flex-direction: column;
        justify-content: space-between;
        .tit {
          display: -webkit-box; /* 使用弹性盒子布局 */
          -webkit-box-orient: vertical; /* 设置盒子方向为垂直 */
          -webkit-line-clamp: 3; /* 限制显示的行数为3行 */
          overflow: hidden; /* 隐藏超出部分 */
          text-overflow: ellipsis; /* 添加省略号 */
          line-height: 1.5; /* 设置行高，以确保行数计算正确 */
          max-height: 4.5em; /* 根据行高设置最大高度，3行 */
        }
        .info {
          // margin-top: 5px;
          margin: 0;
          display: flex;
          justify-content: space-between;
          .price {
            color: #fa2209;
          }
        }
      }
    }

    .flow-num-box {
      display: flex;
      justify-content: flex-end;
      padding: 10px 10px;
      font-size: 14px;
      border-bottom: 1px solid #efefef;
      .money {
        color: #fa2209;
      }
    }

    .pay-cell {
      font-size: 14px;
      padding: 10px 12px;
      color: #333;
      display: flex;
      justify-content: space-between;
      .red {
        color: #fa2209;
      }
    }
    .pay-detail {
      border-bottom: 1px solid #efefef;
    }

    .pay-way {
      font-size: 14px;
      padding: 10px 12px;
      border-bottom: 1px solid #efefef;
      color: #333;
      .tit {
        line-height: 30px;
      }
      /*.pay-cell {
        padding: 10px 0;
      }*/
      .van-icon {
        font-size: 20px;
        margin-right: 5px;
      }
      .pay-type {
        width: 100%;
        ::v-deep(.van-cell-group) {
          margin: 0px;
        }
      }
    }

    .buytips {
      display: block;
      padding: 12px;
      textarea {
        display: block;
        width: 100%;
        border: none;
        font-size: 14px;
        height: 100px;
        box-sizing: border-box;
      }
    }
  }

  .footer-fixed {
    position: fixed;
    background-color: #fff;
    left: 0;
    bottom: 0;
    width: 100%;
    height: 46px;
    line-height: 46px;
    box-sizing: border-box;
    border-top: 1px solid #efefef;
    font-size: 14px;
    display: flex;
    .left {
      flex: 1;
      padding-left: 12px;
      color: #666;
      span {
        color: #fa2209;
      }
    }
    .tipsbtn {
      width: 121px;
      background: linear-gradient(90deg, #f9211c, #ff6335);
      color: #fff;
      text-align: center;
      line-height: 46px;
      display: block;
      font-size: 14px;
    }
  }
}
</style>
