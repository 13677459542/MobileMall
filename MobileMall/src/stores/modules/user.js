import { defineStore } from 'pinia'
import { ref } from 'vue'

// 用户模块
export const useUserStore = defineStore(
  'useUserStore',
  () => {
    // 个人信息
    const userInfo = ref()
    // 搜索历史
    const historyList = ref([])
    // 设置个人信息
    const setUserInfo = (obj) => {
      userInfo.value = obj
    }
    // 移除个人信息
    const removeToken = () => {
      setUserInfo({})
    }
    // 退出登录清理数据
    const logout = () => {
      removeToken()
      console.log('userInfo清理结束')
    }
    // 设置搜索历史
    const setHistoryList = (arr) => {
      historyList.value = arr
    }
    return {
      userInfo,
      historyList,
      setUserInfo,
      removeToken,
      logout,
      setHistoryList,
    }
  },
  {
    // Pinia持久化插件
    persist: {
      key: 'my-user-key', // 这是默认的键名，用于在存储介质中引用数据。
      storage: localStorage, // 这是默认的存储位置，用于持久化状态。
      paths: ['userInfo', 'historyList'], // 指定哪些路径应该被持久化
      serializer: {
        serialize: (state) => JSON.stringify(state),
        deserialize: (data) => JSON.parse(data),
      }, // 默认使用 JSON.stringify 和 JSON.parse 方法。这些方法用于将状态转换为字符串以便存储，并在读取时将其转换回 JavaScript 对象。
      // beforeHydrate: (context) => {
      //   // 在恢复数据之前执行操作
      //   console.log(context)
      // },
      // afterHydrate: (context) => {
      //   // 在恢复数据之后执行操作
      //   console.log(context)
      // },
      debug: false, // 调试模式，关闭时错误不会输出到控制台。
    },
  },
)
