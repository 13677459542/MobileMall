import { useUserStore } from '@/stores'
import axios from 'axios'
import { showLoadingToast, showToast, closeToast } from 'vant'
import { encrypt, decrypt } from './aesEncryption'

// 创建 axios 实例，将来对创建出来的实例，进行自定义配置
// 好处：不会污染原始的 axios 实例
const instance = axios.create({
  // baseURL: 'http://smart-shop.itheima.net/index.php?s=/api/',
  // baseURL: 'https://127.0.0.1:7187/Api/V1/',
  baseURL: 'http://127.0.0.1:5263/Api/V1/',
  timeout: 50000,
})

// 自定义配置 - 请求/响应 拦截器
// 添加请求拦截器
instance.interceptors.request.use(
  function (config) {
    // 在发送请求之前做些什么
    // 开启loading，禁止背景点击 (节流处理，防止多次无效触发)
    showLoadingToast({
      message: '加载中...',
      forbidClick: true, // 禁止背景点击
      loadingType: 'spinner', // 配置loading图标
      duration: 0, // 不会自动消失
    })

    // 对请求数据进行加密
    const encryptedData = encrypt(config.data)
    if (config.data) {
      config.headers['Content-Type'] = 'application/json'
    }
    //赋值请求数据
    config.data = `{"EncryptedData": "${encryptedData.EncryptedData}","KEY":"${encryptedData.KEY}","IV":"${encryptedData.IV}"}`
    // 存储 Key 和 IV 到请求配置，供响应解密使用
    config.metadata = {
      key: encryptedData.KEY,
      iv: encryptedData.IV,
    }

    // 只要有token，就在请求时携带，便于请求需要授权的接口
    const userStore = useUserStore()
    // const token = userStore.userInfo.token
    if (userStore.userInfo && userStore.userInfo.token) {
      config.headers['Authorization'] = `Bearer ${userStore.userInfo.token}`
      // config.headers['Access-Token'] = token
      config.headers.platform = 'H5'
    }

    return config
  },
  function (error) {
    // 对请求错误做些什么
    return Promise.reject(error)
  },
)

// 添加响应拦截器
instance.interceptors.response.use(
  function (response) {
    // 2xx 范围内的状态码都会触发该函数。
    // 对响应数据做点什么 (默认axios会多包装一层data，需要响应拦截器中处理一下)
    if (response.data) {
      const { key, iv } = response.config.metadata // 从请求配置中获取 Key 和 IV
      // 对响应数据进行解密
      // response.data = decrypt(response.data.EncryptedData)
      response.data = decrypt(response.data, key, iv)
      console.log(response.data)
      // const IVstr = base64ToString(response.data.IV)
      if (response.data.status !== 200) {
        // 给错误提示, Toast 默认是单例模式，后面的 Toast调用了，会将前一个 Toast 效果覆盖
        // 同时只能存在一个 Toast
        showToast(response.data.message)
        // 抛出一个错误的promise
        return Promise.reject(response.data.message)
      } else {
        // 正确情况，直接走业务核心逻辑，清除loading效果
        closeToast()
      }
    }
    return response.data
  },
  function (error) {
    // closeToast()
    showToast(error.message)
    // 超出 2xx 范围的状态码都会触发该函数。
    // 对响应错误做点什么
    return Promise.reject(error)
  },
)

// 导出配置好的实例
export default instance
