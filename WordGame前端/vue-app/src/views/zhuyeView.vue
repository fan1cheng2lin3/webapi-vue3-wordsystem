<template>
  <div class="container">

    <div class="content">
      <!-- 底部的全屏图片 -->
      <div class="image-container">
        <img src="https://pic3.zhimg.com/v2-1714485227e3f79159c72d213eda42d5_r.jpg?source=1940ef5c" alt="示例图片" class="image" />
      </div>
      <!-- 覆盖在图片上的文字 -->
      <p class="text">"Everything you can imagine is real."<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;—Pablo Picasso<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;你能想象的一切都能成为现实。</p>
      <el-button class="footer-button" @click="review">复习</el-button>
      <el-button class="footer-button" @click="logout">学习</el-button>
    </div>
  


  </div>
</template>

<style scoped>
.container {
  font-family: "Segoe UI", Arial, sans-serif;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: space-between;
  height: 100vh;
  background: linear-gradient(135deg, #e3e6f0, #ffffff); /* 背景渐变 */
  overflow: hidden; /* 确保内容不超出视口 */
}

header {
  background-color: #2c3e50;
  color: #ecf0f1;
  padding: 10px;
  text-align: center;
  width: 100%;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
  font-size: 20px;
}

.content {
  position: relative; /* 设置相对定位，为覆盖层和背景层提供定位参照 */
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100%;
  width: 100%;
}

.image-container {
  position: absolute; /* 图片占据整个框架 */
  top: 0;
  left: 50%; /* 居中 */
  transform: translateX(-50%); /* 使用负的 translate 来精确居中 */
  width: 100%; /* 设置图片的宽度为容器的 90% */
  height: 100%; /* 设置图片的高度为容器的 100% */
  overflow: hidden;
  z-index: 0; /* 背景层，文字在上面 */
}


.image {
  width: 100%; /* 图片稍微放大以适应视差移动 */
  height: auto;
  position: absolute;
  top: 0;
  left: 0;
  transition: transform 0.05s ease-out; /* 设置平滑动画效果 */
}

.text {
  padding-right: 300px; /* 控制整个段落的左侧缩进 */
  position: absolute;
  z-index: 1; /* 文字层级高于背景图片 */
  font-size: 4rem; /* 超大字体 */
  color: rgba(0, 0, 0, 0.9); /* 半透明白色文字 */
  font-weight: bold; /* 加粗字体 */
  top: 10%; /* 文字位于图片中间偏上 */
  text-align: center;
  text-shadow: 2px 2px 5px rgba(40, 38, 38, 0.5); /* 文字阴影增强可读性 */
  width: 100%;
  user-select: none; /* 禁止文字选中 */
  pointer-events: none; /* 忽略鼠标事件 */
}




.footer-button {
  z-index: 1; /* 文字层级高于背景图片 */
  padding: 40px 200px; /* 增加按钮的内边距，保持大小 */
  background-color: rgba(11, 29, 45, 0.8); /* 背景稍微透明，0.2 的透明度 */
  color: #c8c4b7; /* 按钮文字颜色为蓝色 */
  border: 2px solid #d6d4ce; /* 添加边框，确保按钮可见 */
  border-radius: 8px;
  font-size: 50px; /* 字体大小 */
  cursor: pointer;
  transition: all 0.3s ease;
  transform: translateY(300px); /* 向下移动 20px */
  margin: 0 200px ; /* 增加按钮上下间隔，左右不增加间隔 */
  
}

.footer-button:hover {
  background-color: rgba(202, 162, 122, 0.4); /* 悬停时背景颜色变得更明显 */
  color: #161616; /* 悬停时文字变色 */
  transform: scale(1.01) translateY(300px); /* 放大并保持向下移动 */
  
}

</style>

<script setup>
import { useRouter } from 'vue-router';
import { onMounted } from 'vue';

const router = useRouter();

const review = () => {
  router.push('/Fuxi');
};

const logout = () => {
  router.push('/Xuexi');
};

// 在 DOM 渲染完成后绑定事件
onMounted(() => {
  const container = document.querySelector('.image-container');
  const image = document.querySelector('.image');

  if (container && image) {
    container.addEventListener('mousemove', (e) => {
      const rect = container.getBoundingClientRect();
      const x = ((e.clientX - rect.left) / rect.width - 0.05) * 10; // -5 to +5 range
      const y = ((e.clientY - rect.top) / rect.height - 0.05) * 10; // -5 to +5 range

      image.style.transform = `translate(${x}px, ${y}px) scale(1.03)`; // 微放大并偏移
    });

    container.addEventListener('mouseleave', () => {
      image.style.transform = 'translate(0, 0) scale(1)'; // 恢复初始位置
    });
  }
});
</script>
