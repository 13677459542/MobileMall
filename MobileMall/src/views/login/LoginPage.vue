<script setup>
import { ref, onBeforeUnmount } from 'vue'
import { codeLogin, getMsgCode, getPicCode } from '@/api/login'
import { showToast } from 'vant'
import { useUserStore } from '@/stores'
import { useRouter } from 'vue-router'

const picKey = ref() // 将来请求传递的图形验证码唯一标识
const picUrl = ref() // 存储请求渲染的图片地址
const totalSecond = ref(60) // 总秒数
const second = ref(60) // 当前秒数，开定时器对 second--
const timer = ref() // 定时器 id
const mobile = ref() // 手机号
const picCode = ref() // 用户输入的图形验证码
const msgCode = ref() // 短信验证码

// 获取图形验证码
const ReloadPicCode = async () => {
  const {
    data: { base64, key },
  } = await getPicCode()
  picUrl.value = base64 // 存储地址
  picKey.value = key // 存储唯一标识
  showToast('获取图形验证码成功')
  // this.$toast('获取成功')
  // this.$toast.success('成功文案')
}
ReloadPicCode()

// 校验 手机号 和 图形验证码 是否合法
// 通过校验，返回true
// 不通过校验，返回false
const validFn = () => {
  if (!/^1[3-9]\d{9}$/.test(mobile.value)) {
    showToast('请输入正确的手机号')
    return false
  }
  if (!/^\w{4}$/.test(picCode.value)) {
    showToast('请输入正确的图形验证码')
    return false
  }
  return true
}

// 获取短信验证码
const getCode = async () => {
  if (!validFn()) {
    // 如果没通过校验，没必要往下走了
    return
  }
  // 当前目前没有定时器开着，且 totalSecond 和 second 一致 (秒数归位) 才可以倒计时
  if (!timer.value && second.value === totalSecond.value) {
    // 发送请求
    // 预期：希望如果响应的status非200，最好抛出一个promise错误，await只会等待成功的promise
    await getMsgCode(picCode.value, picKey.value, mobile.value)
    showToast('短信发送成功，注意查收')
    // 开启倒计时
    timer.value = setInterval(() => {
      second.value--
      if (second.value <= 0) {
        clearInterval(timer.value)
        timer.value = null // 重置定时器 id
        second.value = totalSecond.value // 归位
      }
    }, 1000)
  }
}

const userStore = useUserStore()
const router = useRouter()
// 登录
const login = async () => {
  if (!validFn()) {
    return
  }

  if (!/^\d{6}$/.test(msgCode.value)) {
    showToast('请输入正确的手机验证码')
    return
  }

  console.log('发送登录请求')
  const res = await codeLogin(mobile.value, msgCode.value)
  userStore.setUserInfo(res.data)
  showToast('登录成功')
  router.push('/')
}

onBeforeUnmount(() => {
  // 离开页面清除定时器
  clearInterval(timer.value)
  console.log('离开了登录页')
})
</script>

<template>
  <div class="login">
    <!-- 头部标题 -->
    <van-nav-bar title="会员登录" left-arrow @click-left="$router.go(-1)" />

    <div class="container">
      <div class="title">
        <h3>手机号登录</h3>
        <p>未注册的手机号登录后将自动注册</p>
      </div>

      <div class="form">
        <div class="form-item">
          <input
            v-model="mobile"
            class="inp"
            maxlength="11"
            placeholder="请输入手机号码"
            type="text"
          />
        </div>
        <div class="form-item">
          <input
            v-model="picCode"
            class="inp"
            maxlength="5"
            placeholder="请输入图形验证码"
            type="text"
          />
          <img v-if="picUrl" :src="picUrl" @click="ReloadPicCode" alt="" />
        </div>
        <div class="form-item">
          <input v-model="msgCode" class="inp" placeholder="请输入短信验证码" type="text" />
          <button @click="getCode">
            {{ second === totalSecond ? '获取验证码' : second + '秒后重新发送' }}
          </button>
        </div>
      </div>

      <div @click="login" class="login-btn">登录</div>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.container {
  padding: 49px 29px;

  .title {
    margin-bottom: 20px;
    h3 {
      font-size: 26px;
      font-weight: normal;
    }
    p {
      line-height: 40px;
      font-size: 14px;
      color: #b8b8b8;
    }
  }

  .form-item {
    border-bottom: 1px solid #f3f1f2;
    padding: 8px;
    margin-bottom: 14px;
    display: flex;
    align-items: center;
    .inp {
      display: block;
      border: none;
      outline: none;
      height: 32px;
      font-size: 14px;
      flex: 1;
    }
    img {
      width: 94px;
      height: 31px;
    }
    button {
      height: 31px;
      border: none;
      font-size: 13px;
      color: #cea26a;
      background-color: transparent;
      padding-right: 9px;
    }
  }

  .login-btn {
    width: 100%;
    height: 42px;
    margin-top: 39px;
    background: linear-gradient(90deg, #ecb53c, #ff9211);
    color: #fff;
    border-radius: 39px;
    box-shadow: 0 10px 20px 0 rgba(0, 0, 0, 0.1);
    letter-spacing: 2px;
    display: flex;
    justify-content: center;
    align-items: center;
  }
}
</style>
