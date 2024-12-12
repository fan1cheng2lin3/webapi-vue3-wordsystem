import { createStore } from 'vuex'
import authModule from './auth'

export default createStore({
  state: {

    //全局变量
  },
  getters: {

    //全局变量的计算属性
  },
  mutations: {
    //修改全局变量
  },
  actions: {

    //异步修改全局变量

  },
  modules: {
    authModule,
    //模块化
  }
})
