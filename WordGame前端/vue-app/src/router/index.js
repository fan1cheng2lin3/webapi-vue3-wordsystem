import { createRouter, createWebHistory } from 'vue-router'
import { getToken, isTokenFromLocalStorageVaild } from '@/auth/auth.service'

const routes = [
  {
    path: '/login',
    name: 'login',
    component: () => import('@/auth/views/UserLogin.vue')
  },
  {
    path: '/',
    name: '/',
    component: () => import('@/views/LayoutView.vue'),
    redirect: '/zhuyeView',
    children: [
      {
        path: '/Xuexi',
        name: 'Xuexi',
        component: () => import('../views/XuexiView.vue')
      },
      {
        path: '/zhuyeView',
        name: 'zhuyeView',
        component: () => import('../views/zhuyeView.vue')
      },
      {
        path: '/SearchView',
        name: 'SearchView',
        component: () => import('../views/SearchView.vue')
      },
      {
        path: '/CikuView',
        name: 'CikuView',
        component: () => import('../views/CikuView.vue')
      },
      {
        path: '/text',
        name: 'text',
        component: () => import('../views/DaoruView.vue')
      },
      {
        path: '/ddd',
        name: 'ddd',
        component: () => import('../views/TextView.vue')
      },
      {
        path: '/ccc',
        name: 'ccc',
        component: () => import('../views/HHHView.vue')
      },
      {
        path: '/aaa',
        name: 'aaa',
        component: () => import('../views/DataView.vue')
      },
      {
        path: '/putbook',
        name: 'putbook',
        component: () => import('../views/PutbookView.vue')
      },
      {
        path: '/Fuxi',
        name: 'Fuxi',
        component: () => import('../views/FuxiView.vue')
      },
    ]
  },

]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})



router.beforeEach((to, from, next) => {

  //to:可以获取到目的地的路由
  //from:可以获取到从哪里来的路由
  //next():放行,(用法1-next（）表示继续、2-next("/1ogin")去哪里、3-next(flase)取消当前导航）

  if (getToken()) {
    // 已经登录，可以添加一些逻辑，比如检查用户权限
    if (to.path === "/login") {
      next("/")
    }
    else {
      if (isTokenFromLocalStorageVaild()) {
        //已经过期
        next("/login")
      }
      else {
        next()
      }

      next()
    }
    next() // 确保总是调用 next()
  } else {
    if (to.path === "/login") {
      next() // 已经是登录页面，无需重定向
    } else {
      next("/login") // 重定向到登录页面
    }
  }
  console.log(333) // 确保在 next() 调用后输出日志
})



export default router