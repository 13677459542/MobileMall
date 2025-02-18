import { createRouter, createWebHistory } from 'vue-router'
import { useUserStore } from '@/stores'

// createRouter 创建路由实例
// 配置 history 模式
// 1. history模式：createWebHistory     地址栏不带 #
// 2. hash模式：   createWebHashHistory 地址栏带 #
// console.log(import.meta.env.DEV)

// vite 中的环境变量 import.meta.env.BASE_URL  就是 vite.config.js 中的 base 配置项
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/login', component: () => import('@/views/login/LoginPage.vue') }, // 登录页
    {
      path: '/',
      component: () => import('@/views/layout/Index.vue'),
      redirect: '/layout/Home',
      children: [
        {
          path: '/layout/Home',
          component: () => import('@/views/layout/Home.vue'),
        },
        {
          path: '/order/PlaceOrder',
          component: () => import('@/views/order/PlaceOrder.vue'),
        },
        {
          path: '/layout/Cart',
          component: () => import('@/views/layout/Cart.vue'),
        },
        {
          path: '/layout/My',
          component: () => import('@/views/layout/My.vue'),
        },
      ],
    },
    { path: '/layout/Detail', component: () => import('@/views/layout/Detail.vue') }, // 详情页
    { path: '/search/SearchPage', component: () => import('@/views/search/SearchPage.vue') }, // 搜索页
    { path: '/search/SearchList', component: () => import('@/views/search/SearchList.vue') }, // 搜索列表页
    // { path: '/order/PlaceOrder', component: () => import('@/views/order/PlaceOrder.vue') }, // 分类商品页
    { path: '/order/PaymentOrder', component: () => import('@/views/order/PaymentOrder.vue') }, // 下单页
    { path: '/order/MyOrder', component: () => import('@/views/order/MyOrder.vue') }, // 我的订单页
  ],
})

// 登录访问拦截 => 默认是直接放行的
// 根据返回值决定，是放行还是拦截
// 返回值：
// 1. undefined / true  直接放行
// 2. false 拦回from的地址页面
// 3. 具体路径 或 路径对象  拦截到对应的地址
//    '/login'   { name: 'login' }
router.beforeEach((to) => {
  // 如果没有token, 且访问的是非登录页，拦截到登录，其他情况正常放行
  const userStore = useUserStore()
  // console.log(userStore.userInfo)
  if ((!userStore.userInfo || !userStore.userInfo.token) && to.path !== '/login') return '/login'
})

export default router
