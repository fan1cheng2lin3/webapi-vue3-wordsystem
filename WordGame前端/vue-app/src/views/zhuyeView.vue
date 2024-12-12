<template>
  <div class="container">
    <header @click="toggleTitle">
      <h1>首页</h1>
    </header>
    <div class="content">
      <p>文字</p>
      <!-- 添加图片 -->
      <img src="placeholder.jpg" alt="示例图片" class="image" />
    </div>
    <div class="button-footer">
      <el-button class="footer-button" @click="review">复习</el-button>
      <el-button class="footer-button" @click="logout">学习</el-button>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router';
const router = useRouter();

const review = () => {
  router.push('/ccc');
};

const logout = () => {
  router.push('/ddd');
};
</script>

<style scoped>
.container {
  font-family: Arial, sans-serif;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: space-between;
  height: 90vh; /* 占满全屏 */
  background-color: #f4f4f9;
}

header {
  background-color: #333;
  color: #fff;
  padding: 5px;
  text-align: center;
  width: 100%;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.content {
  text-align: center;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  flex-grow: 1; /* 如果内容区域太大，可以考虑移除这个属性 */
}

.image {
  max-width: 80%; /* 调整图片最大宽度 */
  height: 50%;
  margin-top: 20px;
}

.button-footer {
  display: flex;
  justify-content: space-around; /* 如果按钮之间的空间太大，可以考虑使用center */
  width: 100%;
  padding: 20px;
  background-color: #fff;
  box-shadow: 0 -4px 8px rgba(0, 0, 0, 0.1);
}

.footer-button {
  padding: 10px 50px;
  background-color: #4CAF50;
  color: #fff;
  border-radius: 8px;
  font-size: 16px;
  cursor: pointer;
  border: none;
  transition: background-color 0.3s;
}

.footer-button:hover {
  background-color: #45a049;
}
</style>
