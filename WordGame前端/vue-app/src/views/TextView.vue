<template>
    <div class="container">
      <div class="card">
        <div id="match">
          <div v-if="loading">加载中...</div>
          <div v-if="errorMessage">{{ errorMessage }}</div>
  
          <div v-if="words.length > 0">
            <div v-if="currentWord">
              <!-- 显示当前单词和音标 -->
              <strong class="word">{{ currentWord.wordpre }}</strong> - 
              <span class="phonetic">{{ currentWord.phonetic_uk }}</span>
              <br>
              <!-- 延时显示解释 -->
              <span v-if="showExplain" class="explain"> - {{ currentWord.explain }}</span>
  
              <!-- 选项按钮 -->
              <div class="buttons">
                <button
                  v-for="(word, index) in shuffledWords"
                  :key="index"
                  :style="{ backgroundColor: buttonStyles[index] }"
                  @click="handleAnswer(word, index)"
                  :disabled="disableButtons"
                >
                  {{ word.explain }}
                </button>
              </div>
  
              <!-- 显示标记单词状态按钮 -->
              <div v-if="currentWord && !selectedWord">
                <button @click="markWord('easy')" class="easy-button" :disabled="disableButtons">容易</button>
                <button @click="markWord('hard')" class="hard-button" :disabled="disableButtons">困难</button>
                <button @click="markWord('retry')" class="retry-button" :disabled="disableButtons">重来</button>
              </div>
            </div>
            <div v-else>
              <p>所有单词已显示完毕。</p>
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
  const selectedWord = ref(null); // 存储用户选择的单词
  const shuffledWords = ref([]); // 存储所有选项（包含正确的和相似词）
  const buttonStyles = ref([]); // 存储每个按钮的样式
  const triggeredByFunction = ref(null); // 用于记录触发的函数类型
  const showExplain = ref(false); // 控制解释的显示
const disableButtons = ref(false); // 控制按钮的禁用
  
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
        wordsResponse = await axios.get(`/words/GetUnlearnedWordsByViewName/${wordBook.value}`);
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
      const backupResponse = await axios.get(`/words/GetUnlearnedAllWordsBymyViewName/${wordBook.value}`);
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
  
  const handleAnswer = async (word, index) => {
  const wordIdToSend = triggeredByFunction.value === 'yushe' ? currentWord.value.id : currentWord.value.wordId;

  try {
    // 获取当前用户的学习进度
    const response = await axios.get(`/LearningProgress/getprogress`);
    const progressList = response.data || []; // 如果没有数据，默认为空数组

    // 找到对应 wordId 的记录
    const currentWordProgress = progressList.find(
      (progress) => progress.wordId === wordIdToSend
    );

    // 获取当前分数，如果没有找到记录，默认为 0
    const currentScore = currentWordProgress?.score || 0;

    let newScore = currentScore; // 初始化新的分数

    if (word.wordpre === currentWord.value.wordpre) {
      // 答对：标记绿色，更新分数 +5
      buttonStyles.value[index] = '#66bb6a'; // 柔和绿色
      newScore += 5; // 答对加 5 分
    } else {
      // 答错：标记红色并扣分
      buttonStyles.value[index] = '#e57373'; // 柔和红色
      const temp = word.wordpre;
      word.wordpre = word.explain;
      word.explain = temp;

      // 根据当前答错的数量扣分
      const wrongCount = buttonStyles.value.filter(style => style === '#e57373').length;
      const deduction = Math.min(wrongCount, 3); // 最多扣 3 分
      newScore -= deduction; // 扣分
    }

    // 提交新分数到后端
    await axios.post("/LearningProgress/updateProgress", {
      wordId: wordIdToSend,
      Score: newScore, // 更新后的分数
      Status: word.wordpre === currentWord.value.wordpre ? 'correct' : 'wrong', // 状态
      count: 1,
    });
  } catch (error) {
    console.error("更新分数失败", error);
  }
};



  // 显示下一个单词并准备选项
  const showNextWord = () => {
    if (currentIndex.value < words.value.length) {
      currentWord.value = words.value[currentIndex.value];
      // 使用 similar1 至 similar4 作为选项，并创建副本以避免直接修改原始数据
      shuffledWords.value = [
        { wordpre: currentWord.value.wordpre, explain: currentWord.value.explain },
        { wordpre: currentWord.value.similar1, explain: currentWord.value.similar1_explain },
        { wordpre: currentWord.value.similar2, explain: currentWord.value.similar2_explain },
        { wordpre: currentWord.value.similar3, explain: currentWord.value.similar3_explain },
        { wordpre: currentWord.value.similar4, explain: currentWord.value.similar4_explain }
      ].map(item => ({ ...item }));
  
      // 打乱选项
      shuffledWords.value = shuffledWords.value.sort(() => Math.random() - 0.5);
  
      buttonStyles.value = Array(shuffledWords.value.length).fill(''); // 清空按钮样式
      selectedWord.value = null; // 清空之前的选择
    } else {
      currentWord.value = null; // 所有单词显示完毕
    }
  };
  
  // 标记单词状态并显示下一个单词
  const markWord = async (status) => {
  const scoreMap = {
    easy: 3,
    hard: -1,
    retry: -3,
  };

  const wordIdToSend = currentWord.value.id;

  try {
    const response = await axios.get(`/LearningProgress/getprogress`);
    const progressList = response.data || [];
    const currentWordProgress = progressList.find((p) => p.wordId === wordIdToSend);
    const currentScore = currentWordProgress?.score || 0;
    const newScore = currentScore + scoreMap[status];

    // 禁用按钮并显示解释
    disableButtons.value = true;
    showExplain.value = true;

    // 等待 1 秒后执行后续操作
    setTimeout(async () => {
      await axios.post("/LearningProgress/updateProgress", {
        wordId: wordIdToSend,
        Score: newScore,
        Status: status,
        count: 1,
      });

      // 更新到下一个单词
      showExplain.value = false;
      currentIndex.value++;
      showNextWord();
      disableButtons.value = false;
    }, 2000);
  } catch (error) {
    console.error("标记单词失败", error);
    disableButtons.value = false;
  }
};


  onMounted(() => {
    fetchWordBookAndWords();
  });
  </script>
  
  <style scoped>
.card {
  padding: 30px;
  border: 1px solid #c7c5c9;
  border-radius: 8px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  background-color: #cdcec7;
  width: 75%;
  height: 85%;
  margin: 50px auto; /* 设置居中和页边距 */
}

strong.word {
  font-size: 80px; /* 增大字体 */
  font-weight: bold;
  margin-right: 10px; /* 为单词与音标增加间距 */
}

span.phonetic {
  font-size: 30px; /* 增大音标字体 */
  color: #888; /* 设置音标颜色 */
}

.buttons {
  margin-top: 20px;
  display: flex;
  flex-direction: column; /* 使按钮各自占一行 */
  gap: 10px; /* 设置按钮之间的间距 */
}

/* 为"容易"、"困难"、"重来"按钮设置单独样式 */
.buttons button {
  padding: 15px;
  border: none;
  border-radius: 8px;
  background-color: #fff;
  color: black;
  cursor: pointer;
  font-size: 28px;
  width: 100%; /* 按钮占满一整行 */
  box-sizing: border-box; /* 确保按钮宽度正确 */
  transition: background-color 0.3s ease; /* 设置背景色过渡效果 */
}

button:hover {
  background-color: #f0f0f0;
}

button:active {
  background-color: #552e2e;
}



/* 另外的五个选项按钮，保持它们与"容易"、"困难"、"重来"按钮之间有间隙 */
div#match .buttons button {
  margin-bottom: 20px; /* 为这五个选项按钮添加底部间距 */
}


button {
    
  padding: 15px;
  border: none;
  border-radius: 50px;
  background-color: #fff;
  color: black;
  cursor: pointer;
  font-size: 18px;
  width: 60%; /* 按钮占满一定宽度 */
  box-sizing: border-box;
  transition: background-color 0.3s ease;
  margin-bottom: 10px; /* 按钮之间的间隙 */
  margin-left: auto; /* 自动居左 */
  margin-right: auto; /* 自动居右 */
  display: block; /* 使按钮成为块级元素 */
}


button:hover {
  background-color: #68665d;
}

button:active {
  background-color: #ddd;
}

.easy-button {
  background-color: #008505; /* 绿色 */
  color: white;
  font-size: 28px;
}

.hard-button {
  background-color: #b86f01; /* 橙色 */
  color: white;
  font-size: 28px;
}

.retry-button {
  background-color: rgb(160, 11, 0); /* 红色 */
  color: white;
  font-size: 28px;
}

.explain {
  font-size: 35px;
  color: #040404;
  margin-left: 30px;
}

  </style>
  


