// 按需导入
import { createApp } from 'vue'
import App from '../App.vue'
// 1. 引入你需要的组件
import {
  Tab,
  Tabs,
  Checkbox,
  Dialog,
  ActionSheet,
  Icon,
  Search,
  Swipe,
  SwipeItem,
  Grid,
  GridItem,
  Toast,
  Button,
  Switch,
  Rate,
  Tabbar,
  TabbarItem,
  NavBar,
} from 'vant'
// 2. 引入组件样式
import 'vant/lib/index.css'

const app = createApp(App)
app.use(Tab)
app.use(Tabs)
app.use(Checkbox)
app.use(Dialog)
app.use(ActionSheet)
app.use(Icon)
app.use(Search)
app.use(Swipe)
app.use(SwipeItem)
app.use(Grid)
app.use(GridItem)
app.use(Toast)
app.use(NavBar)
app.use(Tabbar)
app.use(TabbarItem)
app.use(Rate)
app.use(Button)
app.use(Switch)
