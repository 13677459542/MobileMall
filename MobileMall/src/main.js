import { createApp } from 'vue'
// import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router'
import pinia from '@/stores/index' // pinia 独立维护
// 添加样式scss库： pnpm add sass -D

import '@/assets/main.scss'

// import '@/utils/vant-ui' // 按需导入Vant，未生效不可用

// 1、全部导入Vant
import Vant from 'vant'
import 'vant/lib/index.css'

const app = createApp(App)

// app.use(createPinia())
app.use(router)

// 2、全部导入Vant
app.use(Vant)

app.use(pinia)
app.mount('#app')
