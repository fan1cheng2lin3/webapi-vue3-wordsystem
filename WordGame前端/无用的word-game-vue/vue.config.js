const { defineConfig } = require('@vue/cli-service')
const NodePolyfillPlugin = require('node-polyfill-webpack-plugin');
const webpack = require('webpack');

module.exports = defineConfig({
  transpileDependencies: true,
  devServer: { port: 8212,
    proxy: {
      "/api": {
        target: "https://localhost:7074/api", // 服务器请求的地址
        secure: false, // HTTPS需要配置这个参数
        changeOrigin: true, // 请求头host属性，true表示伪装为目标地址
        pathRewrite: {
          "^/api": "", // 移除 "/api" 前缀
        },
      },
    },
    client: {
      overlay: {
        warnings: false,
        errors: false,
      },
    },
  },
  configureWebpack: {
    plugins: [
      new NodePolyfillPlugin(),
      new webpack.ProvidePlugin({
        process: 'process/browser', // 显式提供 process 的 polyfill
      }),
    ],
    resolve: {
      fallback: {
        stream: require.resolve('stream-browserify'),
      },
    },
  },
  
  
  
  

 
});
