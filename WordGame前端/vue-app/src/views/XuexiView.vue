<template>
  <div class="container">
    <!-- 左边搭配词卡片 -->
    <div class="card" style="width: 60%">
      <div id="match">
        <h2>搭配词</h2>
      </div>
    </div>

    <!-- 右边记忆卡片 -->
    <div class="card" style="width: 30%">
      <!-- 英语部分 -->
      <div id="question">{{ question }}</div>
      <!-- 中文部分 -->
      <div id="answer" v-show="showAnswer">{{ answer }}</div>

      <!-- 随机按钮 -->
      <div v-show="showButtons">
        <button
          v-for="(option, index) in options"
          :key="index"
          @click="checkAnswer(option)"
          class="btn"
          :class="{'incorrect': option.isCorrect === false, 'correct': option.isCorrect === true}"
        >
          {{ option.text }}
        </button>
      </div>

      <!-- 第二阶段的3个按钮 -->
      <div v-show="showSecondButtons">
        <button
          v-for="(option, index) in secondOptions"
          :key="index"
          @click="checkSecondAnswer(option)"
          class="btn"
          :class="{'incorrect': option.isCorrect === false, 'correct': option.isCorrect === true}"
        >
          {{ option.text }}
        </button>
      </div>

      <!-- 拼写部分 -->
      <input
        type="text"
        id="spell-input"
        v-show="showSpellInput"
        v-model="spellInput"
        placeholder="输入拼写..."
        @input="checkSpelling"
      />
      <div id="spell-message" v-show="showSpellMessage">{{ spellMessage }}</div>
    </div>

    <!-- 按钮部分 -->
    <div id="buttons" class="button-container">
      <button @click="toggleAnswer" class="btn" v-show="showAnswerButton">显示答案</button>
      <div class="review-buttons" v-show="showReviewButtons">
        <button @click="retry" class="btn retry">重来</button>
        <button @click="good" class="btn good">良好</button>
        <button @click="easy" class="btn easy">简单</button>
      </div>
    </div>

    
  </div>
</template>

<script setup>
import { ref } from "vue";

// 数据与状态
const showAnswer = ref(false);
const showSpellInput = ref(false);
const showSpellMessage = ref(false);
const showReviewButtons = ref(false);
const showButtons = ref(true);
const showSecondButtons = ref(false);
const showAnswerButton = ref(false);
const spellInput = ref("");
const spellMessage = ref("");
const question = ref("What is the capital of France?");
const answer = ref("Paris");
const options = ref([]);
const secondOptions = ref([]);
let selectedOption = ref(null);

// 生成随机按钮
const generateOptions = () => {
  const correctAnswer = answer.value;
  const incorrectAnswers = ["London", "Berlin", "Madrid"];
  const allOptions = [correctAnswer, ...incorrectAnswers];
  allOptions.sort(() => Math.random() - 0.5); // 随机打乱

  options.value = allOptions.map((option) => ({
    text: option,
    isCorrect: option === correctAnswer,
  }));
};



// 检查用户选择的答案（第一阶段）
const checkAnswer = (selectedOption) => {
  if (selectedOption.isCorrect) {
    showAnswer.value = true;
    showButtons.value = false;
    showAnswerButton.value = true; // 显示“显示答案”按钮
    console.log("用户选择正确！");
  } else {
    console.log("用户选择错误，发送到后台！");
    // 在这里发送错误信息到后台
    // 例如：
    // axios.post('/api/logError', { selectedOption, correctAnswer: answer.value });
  }
};

// 检查第二阶段用户选择的答案
const checkSecondAnswer = (selectedOption) => {
  if (selectedOption.isCorrect) {
    showSpellInput.value = true;
    showReviewButtons.value = true; // 显示拼写部分
    console.log("用户选择正确，进入拼写阶段！");
  } else {
    console.log("用户选择错误，发送到后台！");
    // 发送错误信息到后台
  }
};

// 重试
const retry = () => {
  showSpellInput.value = true;
  spellMessage.value = "请重试拼写";
  showSpellMessage.value = true;
};

// 好
const good = () => {
  spellMessage.value = "做得很好！";
  showSpellMessage.value = true;
};

// 简单
const easy = () => {
  spellMessage.value = "简单！请继续学习。";
  showSpellMessage.value = true;
};

// 显示答案
const toggleAnswer = () => {
  showAnswer.value = !showAnswer.value;
  showReviewButtons.value = true;
  showAnswerButton.value = false; // 隐藏“显示答案”按钮
};

// 判断拼写输入
const checkSpelling = () => {
  if (spellInput.value.toLowerCase() === "true") {
    // 拼写正确，重新开始
    generateOptions();
    showButtons.value = true;
    showSpellInput.value = false;
    showSpellMessage.value = "";
    showAnswer.value = false;
    showReviewButtons.value = false;
    showSecondButtons.value = false;
    selectedOption.value = null;
  } else {
    spellMessage.value = "拼写错误，请重试！";
    showSpellMessage.value = true;
  }
};

// 初始化生成按钮
generateOptions();
</script>

<style scoped>
.container {
  display: flex;
  justify-content: space-between;
  margin-bottom: 20px;
}

.card {
  padding: 20px;
  border: 1px solid #ddd;
  border-radius: 8px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  background-color: #fff;
}

.button-container {
  display: flex;
  justify-content: center;
  gap: 10px;
  margin-top: 20px;
}

.btn {
  padding: 10px 20px;
  border: none;
  border-radius: 5px;
  font-size: 16px;
  cursor: pointer;
}

.btn.retry {
  background-color: #f44336;
  color: white;
}

.btn.good {
  background-color: #4caf50;
  color: white;
}

.btn.easy {
  background-color: #2196f3;
  color: white;
}

.incorrect {
  background-color: #f44336;
}

.correct {
  background-color: #4caf50;
}
</style>
