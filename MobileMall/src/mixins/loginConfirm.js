import { ref } from 'vue'
import { showConfirmDialog } from 'vant'
import { useRouter, useRoute } from 'vue-router'
import { useUserStore } from '@/stores'
// const router = useRouter() // 返回router路由实例  通过它可以实现路由跳转
// const route = useRoute() // 返回路由信息对象,接收跳转传参时使用

export const useMixin = (router, route) => {
  const title = ref()

  const loginConfirm = () => {
    const userStore = useUserStore()
    // 判断 token 是否存在
    if (!userStore.userInfo.token) {
      // 弹确认框
      showConfirmDialog({
        title: '温馨提示',
        message: '此时需要先登录才能继续操作哦',
        confirmButtonText: '去登陆',
        cancelButtonText: '再逛逛',
      })
        .then(() => {
          // 这里使用router.replace 而不是 router.push，push会往历史记录不断累加，而replace是直接把原来所在的历史记录页面替换
          router.replace({
            path: '/login',
            // 如果希望，跳转到登录 =》登录后能回跳回来，需要在跳转去携带参数(当前的路径地址)
            // route.fullPath(会包含查询参数)
            query: {
              backUrl: route.fullPath,
            },
          })
        })
        .catch(() => {})
      return true
    }
    return false
  }

  return {
    title,
    loginConfirm,
  }
}

// (在 Vue 3 中，useRouter 和 useRoute 只能在 setup 函数或功能组件（functional components）中使用。
// 如果你在 useMixin 中直接调用它们，就会报错。要解决这个问题，需要将 useRouter 和 useRoute 移到 setup 函数中，然后将它们传递给 useMixin。)
// 在组件的 setup 函数中使用 useMixin
export default {
  setup() {
    const router = useRouter()
    const route = useRoute()
    const { title, loginConfirm } = useMixin(router, route)

    return {
      title,
      loginConfirm,
    }
  },
}
