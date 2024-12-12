<template>
    <div class="container">
      <!-- 左边搭配词卡片 -->
      <div class="card" style="width: 60%">
        <div id="match">
          <div v-if="loading">加载中...</div>
          <div v-if="errorMessage">{{ errorMessage }}</div>
          <div v-if="words.length > 0">
            <div v-if="currentWord">
              <strong>{{ currentWord.wordpre }}</strong> - <span>{{ currentWord.explain }}</span>
              
              
              
              <div>音标：{{ currentWord.phonetic }}</div>
              <div>例句：{{ currentWord.sentence_en }} | {{ currentWord.sentence_cn }}</div>
            
            
            
            </div>
            <div v-else>
              <p>所有单词已显示完毕。</p>
            </div>
  
            <!-- 按钮操作 -->
            <div v-if="currentWord" class="buttons">
              <button @click="markWord('easy')">容易</button>
              <button @click="markWord('hard')">困难</button>
              <button @click="markWord('retry')">重来</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </template>
  
  <script setup>
  import axios from "axios";
  import { ref, onMounted } from "vue";
  
  // 定义响应式数据
  const wordBook = ref(""); // 存储 wordbook 名称
  const words = ref([]); // 存储单词信息
  const currentIndex = ref(0); // 当前显示的单词索引
  const currentWord = ref(null); // 当前显示的单词
  const loading = ref(false); // 用于显示加载状态
  const errorMessage = ref(""); // 错误提示
  const triggeredByFunction = ref(null); // 用于记录触发的函数类型
  
  // 请求数据
  const fetchWordBookAndWords = async () => {
    loading.value = true;
    try {
      // 1. 获取用户词书名称
      const progressResponse = await axios.get("/Users/getprogress");
      if (progressResponse.data && progressResponse.data.length > 0) {
        wordBook.value = progressResponse.data[0].wordbook; // 假设取第一个词书
      } else {
        throw new Error("未找到用户词书");
      }
  
      // 2. 尝试从第一个接口获取单词数据
      let wordsResponse;
      try {
        wordsResponse = await axios.get(`/words/GetWordsByViewName/${wordBook.value}`);
        if (wordsResponse.data && wordsResponse.data.length > 0) {
            triggeredByFunction.value = 'yushe'; // 更新触发函数类型
          words.value = wordsResponse.data; // 如果找到数据，直接使用
          showNextWord(); // 显示第一个单词
          return; // 成功获取数据后直接返回
        } else {
          throw new Error("未找到数据");
        }
      } catch (error) {
        console.warn(`从第一个接口未找到数据，尝试使用备用接口。错误信息: ${error.message}`);
      }
  
      // 3. 从备用接口获取单词数据
      const backupResponse = await axios.get(`/words/GetAllWordsBymyViewName/${wordBook.value}`);
      if (backupResponse.data && backupResponse.data.length > 0) {
        triggeredByFunction.value = 'zdy'; // 更新触发函数类型
        words.value = backupResponse.data;
        showNextWord(); // 显示第一个单词
      } else {
        throw new Error(`未找到词书 "${wordBook.value}" 的单词数据`);
      }
    } catch (error) {
      errorMessage.value = error.message || "获取数据失败";
    } finally {
      loading.value = false;
    }
  };
  

  
  // 显示下一个单词
  const showNextWord = () => {
    if (currentIndex.value < words.value.length) {
      currentWord.value = words.value[currentIndex.value];
    } else {
      currentWord.value = null; // 所有单词显示完毕
    }
  };
  
  // 标记单词状态并显示下一个单词
  const markWord = async (status) => {
  // 定义不同状态下的分数
  const scoreMap = {
    easy: 5,
    hard: 3,
    retry: 0,
  };

  const wordIdToSend = triggeredByFunction.value === 'yushe'? currentWord.value.id:currentWord.value.wordId;


  const progressData = {


    wordId: wordIdToSend,
    Score: scoreMap[status], // 根据状态设置分数
    Status: status, // 'easy', 'hard', 'retry'
    count: 1, // 这里可以根据需要修改，假设每次点击都是一次新的尝试
  };
  try {
    // 发送请求更新学习进度
    await axios.post("/LearningProgress/updateProgress", progressData);

    // 更新到下一个单词
    currentIndex.value++; 
    showNextWord();
  } catch (error) {
    console.error("标记单词失败", error);
  }
};

  
  // 生命周期钩子
  onMounted(() => {
    fetchWordBookAndWords();
  });
  </script>
  
  
  <style scoped>
  .card {
    padding: 20px;
    border: 1px solid #ddd;
    border-radius: 8px;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
    background-color: #fff;
  }
  
  ul {
    list-style: none;
    padding: 0;
  }
  
  li {
    margin-bottom: 10px;
  }
  
  strong {
    font-weight: bold;
  }
  
  div {
    margin-top: 5px;
  }
  
  .buttons {
    margin-top: 20px;
  }
  
  button {
    margin-right: 10px;
    padding: 10px;
    border: none;
    border-radius: 5px;
    background-color: #4caf50;
    color: white;
    cursor: pointer;
  }
  
  button:hover {
    background-color: #45a049;
  }
  </style>
  