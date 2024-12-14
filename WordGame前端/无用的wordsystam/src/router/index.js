import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'

const routes = [
  {
    path: '/login',
    name: 'login',
    component:() => import( '@/auth/views/UserLogin.vue')
  },
  {
    path: '/',
    name: '/',
    component:() => import( '@/views/LayoutView.vue'),
    redirect:'/home',
    children:[
      {
        path: '/home',
        name: 'home',
        component:HomeView
      },
      {
        path: '/about',
        name: 'about',
        component: () => import('../views/AboutView.vue')
      }

    ]
  },

]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

export default router
