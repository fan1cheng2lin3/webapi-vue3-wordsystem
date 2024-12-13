<template>
  <div class="container">
    <h1 class="title">导入单词到词书</h1>
    <form @submit.prevent="submitForm" class="form">
      <div class="form-group">
        <label for="wordBookName" class="label">词书名:</label>
        <input type="text" id="wordBookName" v-model="wordBookName" class="input" required />
      </div>
      <div class="form-group">
        <label for="file" class="label">选择文件:</label>
        <input type="file" id="file" @change="onFileChange" class="input" required />
      </div>
      <div class="form-group">
        <button type="submit" class="submit-btn" :disabled="isLoading">
          提交
        </button>
      </div>
    </form>
    <div v-if="isLoading" class="loading">
      <span class="spinner"></span> 正在导入，请稍候...
    </div>
    <p v-if="message" :class="messageClass">{{ message }}</p>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import axios from 'axios';

// 定义响应式变量
const wordBookName = ref("");  // 词书名
const file = ref(null);         // 上传的文件
const message = ref("");        // 提示信息
const messageClass = ref("");  // 用于设置提示信息的样式
const isLoading = ref(false);  // 是否处于加载状态

// 处理文件选择
function onFileChange(event) {
  file.value = event.target.files[0];
}

// 提交表单
async function submitForm() {
  if (!wordBookName.value || !file.value) {
    message.value = "请填写词书名并选择文件。";
    messageClass.value = "error";
    return;
  }

  // 创建 FormData 对象，包含表单字段和文件
  const formData = new FormData();
  formData.append("WordBookName", wordBookName.value);
  formData.append("File", file.value);

  // 开始加载状态
  isLoading.value = true;
  message.value = ""; // 清空之前的提示信息

  try {
    // 发送 POST 请求到后端接口
    await axios.post(
      "AddWordBook/ImportWordsToWordBook",
      formData,
      {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      }
    );

    // 响应成功
    message.value = "文件导入成功！";
    messageClass.value = "success";
  } catch (error) {
    // 错误处理
    message.value = "文件导入失败，请重试。";
    messageClass.value = "error";
    console.error("上传失败:", error);
  } finally {
    // 完成加载状态
    isLoading.value = false;
  }
}
</script>

<style scoped>
/* 通用容器样式 */
.container {
  width: 100%;
  max-width: 600px;
  margin: 0 auto;
  padding: 20px;
  background-color: #f9f9f9;
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

/* 标题样式 */
.title {
  font-size: 24px;
  color: #333;
  text-align: center;
  margin-bottom: 20px;
}

/* 表单样式 */
.form {
  display: flex;
  flex-direction: column;
}

/* 表单项样式 */
.form-group {
  margin-bottom: 20px;
}

/* 标签样式 */
.label {
  font-size: 16px;
  color: #333;
  margin-bottom: 5px;
}

/* 输入框样式 */
.input {
  width: 100%;
  padding: 10px;
  border-radius: 4px;
  border: 1px solid #ccc;
  font-size: 14px;
}

/* 按钮样式 */
.submit-btn {
  padding: 12px;
  background-color: #4CAF50;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 16px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.submit-btn:hover {
  background-color: #45a049;
}

.submit-btn:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}

/* 提示信息样式 */
p {
  font-size: 16px;
  text-align: center;
  padding: 10px;
  border-radius: 4px;
  margin-top: 20px;
}

.success {
  background-color: #4CAF50;
  color: white;
}

.error {
  background-color: #f44336;
  color: white;
}

/* 加载状态 */
.loading {
  text-align: center;
  margin-top: 20px;
}

.spinner {
  border: 4px solid #f3f3f3; /* Light grey */
  border-top: 4px solid #3498db; /* Blue */
  border-radius: 50%;
  width: 30px;
  height: 30px;
  animation: spin 2s linear infinite;
}

/* 添加旋转动画 */
@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
</style>