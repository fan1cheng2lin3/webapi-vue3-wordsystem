const { defineConfig } = require('@vue/cli-service')
module.exports = defineConfig({
  transpileDependencies: true,
  devServer: {
    port: 8888, // 更改为8081端口
    proxy: {
      "/api": {
        target: "https://localhost:5134/api", // 服务器请求的地址
        secure: false, // HTTPS需要配置这个参数
        changeOrigin: true, // 请求头host属性，true表示伪装为目标地址
        pathRewrite: {
          "^/api": "", // 移除 "/api" 前缀
        },
      },
    },

  },

   configureWebpack: {
    resolve: {
      fallback: {
        stream: require.resolve('stream-browserify'),
      },
    },
  },


})
