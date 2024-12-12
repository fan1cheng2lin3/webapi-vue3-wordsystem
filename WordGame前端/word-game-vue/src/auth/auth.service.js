import axios from '@/api/api_config'
import router from '@/router'
import * as jwt from "jsonwebtoken"
//登录
export const loginUser = async (login) => {
    return await axios.post('Userslogin/auth', login);
}

const key = 'tokenAnt'

//从浏览器本地存储获取token值
export const getToken = () => {
    return localStorage.getItem(key)
}

//清除token
export const logOut = () => {
    localStorage.removeItem(key)
    router.replace('/login')
}

//检查token过期时间
export const isTokenFromLocalStorageVaild=()=>{
    const token = localStorage.getItem(key)
    if (!token) return false
    const decoded = jwt.decode(token)

    // 当前时间，单位是毫秒
    const dateNow = Date.now();

    // token 中的过期时间，单位是秒，转换为毫秒
    const expiresAt = decoded.exp * 1000;

    // 比较当前时间和 token 的过期时间，如果当前时间大于或等于过期时间，则 token 已经过期
    return dateNow >= expiresAt;



}