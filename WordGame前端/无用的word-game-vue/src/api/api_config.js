import axios from "axios";
import { getToken } from "@/auth/auth.service";

axios.defaults.baseURL = "/api"; // 配置为相对路径，以便通过代理转发
axios.defaults.headers['X-Requested-With'] = "XMLHttpRequest";
axios.defaults.headers.post['Content-Type'] = 'application/json';

// 请求拦截器
axios.interceptors.request.use(
  (config) => {
    const jwtToken = getToken();
    if (jwtToken) {
      config.headers.Authorization = `Bearer ${jwtToken}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// 响应拦截器
axios.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error("请求失败：", error.response || error.message);
    return Promise.reject(error);
  }
);

export default axios;
