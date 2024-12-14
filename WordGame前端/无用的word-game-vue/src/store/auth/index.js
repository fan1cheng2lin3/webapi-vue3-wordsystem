import { loginUser, logOut } from "@/auth/auth.service"
import router from '@/router'

const authModule = {
  namespaced: true,//告诉使用者调用加上命名空间
  state: {
    //全局变量

    signInState: {
      emial: '',
      exp: Date.now(),
      sub: "",
      token: null,
    }
  },
  getters: {

    //全局变量的计算属性
  },
  mutations: {
    //修改全局变量

    userLogin(state, token) {
      state.signInState.token = token
      localStorage.setItem("tokenAnt", token)

    }

  },
  actions: {

    //异步修改全局变量
    async userLoginAction({ commit }, login) {
      const { data } = await loginUser(login)
      console.log(data)
      commit('userLogin', data.token)
      router.replace('/')
    },

    logout() {
      //移除token
      logOut();
      //重置路由
    }



  },
}
export default authModule;