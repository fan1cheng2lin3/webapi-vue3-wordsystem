<template>
  <div class="container" >
    <header @click="toggleTitle">

      <h1>{{ currentTitle }}</h1>
    </header>
          <el-button @click="goBack" type="primary" style="margin-bottom: 20px;">返回</el-button>
    <div class="main-content" v-if="!isWordsVisible">
      <div class="collect-word-book">
        <div class="book-list">
          <div class="book-container">
            <img :src='"https://th.bing.com/th/id/OIP.YnDSAk0EwMlWLmlGAkftwwHaHa?rs=1&pid=ImgDetMain"'  @click="getstartWordbooks()"/>
         
            <h3>生词本</h3>
          </div>
        </div>
      </div>

      
      <div class="my-book">
        <div class="book-list">
          <div class="book-container" v-for="book in books" :key="book.id">
            <div class="book-info">
              <img :src="book.imageUrl" :alt="book.title" @click="getmyWordbooks(book.viewName,book) ">
              <h3>{{ book.title }}</h3>
            </div>
            <button @click="deleteBook(book.title)" class="delete-button">删除</button>
          </div>
        </div>
      </div>

      <button @click="gotext">自定义词本</button>
    </div>


  <!-- ... 预设词库 ... -->
  <div class="preset-books" v-if="!isWordsVisible">
    <div class="book-list">
      <div class="book-container2" v-for="preset in presetBooks" :key="preset.id">
        <img :src="preset.imageUrl" :alt="preset.title" @click="fetchWords(preset.viewName,preset) " class="dictionary-image">
        <h3 class="s">{{ preset.title }}</h3>
      </div>
    </div>
  </div>





     <!-- 显示词汇列表 -->
     <div v-if="isWordsVisible">
      <div v-if="loading" style="text-align: center;">加载中...</div>
      <div v-else-if="error" style="text-align: center; color: red;">{{ error }}</div>
      <div v-else class="results-container">
        <el-row :gutter="20">
          <el-col :span="6" v-for="word in results" :key="word.wordpre">
            <el-card class="result-card" @click="showDetails(word)">
              <div class="card-header">
                <span>{{ word.wordpre }}</span>
                <!-- <span>{{ word.status }}</span> -->


                <button v-if="word.status === 'start'" @click.stop="toggleFavorite(word)">
                  已收藏
                </button>
                <button v-else @click.stop="toggleFavorite(word)">
                  收藏
                </button>

              </div>
              <div class="card-body">
                <p><strong>音标:</strong> {{ word.phonetic || word.phonetic_uk }}</p>
                <p><strong>释义:</strong> {{ word.explain }}</p>
              </div>
            </el-card>
          </el-col>
        </el-row>
      </div>

    <!-- 详情弹窗 -->
    <el-dialog 
  v-model="dialogVisible" 
  title="详细信息" 
  :width="800" 
  class-name="detail-dialog large-font">
  <div v-if="selectedWord" class="detail-info">
    <div class="info-item">
      <strong>单词:</strong> {{ selectedWord.wordpre }}
    </div>
    <div class="info-item">
      <strong>美式音标:</strong> {{ selectedWord.phonetic }}
      <strong>英式音标:</strong> {{ selectedWord.phonetic_uk }}
    </div>
    <div class="info-item">
      <strong>释义:</strong> {{ selectedWord.explain }}
    </div>
    <div class="info-item">
      <strong>例句:</strong>
      <div>{{ selectedWord.sentence_en }}</div>
      <div>{{ selectedWord.sentence_cn }}</div>
    </div>
    <div class="info-item">
      <strong>辅助记忆:</strong> {{ selectedWord.ancillary || '无' }}
    </div>
    <div class="info-item">
      <el-button @click="speakWord(selectedWord)" type="primary">朗读单词</el-button>
    </div>
  </div>
  <template #footer>
    <el-button @click="dialogVisible = false">关闭</el-button>
  </template>
</el-dialog>

    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios'
import { useRouter } from 'vue-router';
const router = useRouter();

//转天
const gotext = () => {
  router.push('/text');
};

const presetBooks = ref([
  {
    id: 1,
    title: 'Associate Degree',
    imageUrl: 'https://th.bing.com/th/id/OIP.YnDSAk0EwMlWLmlGAkftwwHaHa?rs=1&pid=ImgDetMain',
    viewName: 'View_AssociateDegree'
  },
  {
    id: 2,
    title: 'Bachelor',
    imageUrl: 'https://th.bing.com/th/id/OIP.YnDSAk0EwMlWLmlGAkftwwHaHa?rs=1&pid=ImgDetMain',
    viewName: 'View_Bachelor'
  },
  {
    id: 3,
    title: 'Base',
    imageUrl: 'https://th.bing.com/th/id/OIP.YnDSAk0EwMlWLmlGAkftwwHaHa?rs=1&pid=ImgDetMain',
    viewName: 'View_Base'
  },
 
]);


// 定义 books 为响应式变量，并初始化为空数组
const books = ref([]);
// const showModal = ref(false);
// const newBookTitle = ref('');
const results = ref([])
const Startresults = ref([])
const loading = ref(false)
const error = ref(null)
const dialogVisible = ref(false)
const selectedWord = ref(null)
const isWordsVisible = ref(false)  // 控制词汇是否显示
const triggeredByFunction = ref(null); // 用于记录触发的函数类型

// 假设 word 是一个响应式对象，其中 status 是一个响应式属性
ref({
  status: 'start' // 或者其他值，根据实际情况设置
});

// 在组件挂载时获取词书数据
onMounted(async () => {
  try {
    const response = await axios.get('/AddWordBook/getWordBooks');
    if (response.data && Array.isArray(response.data)) {
      books.value = response.data.map((name, index) => ({
        id: index + 1, // 自动生成 id
        title: name, // title 与 viewName 相同
        imageUrl: 'https://th.bing.com/th/id/OIP.YnDSAk0EwMlWLmlGAkftwwHaHa?rs=1&pid=ImgDetMain', // 不变的图片 URL
        viewName: name // viewName 与 title 相同
      }));
    }
  } catch (error) {
    console.error('未能获取词书数据', error);
  }
});

async function deleteBook(bookname) {
  try {
    // 调用后端接口
    await axios.delete('/AddWordBook/deleteWordBooks', {
      data: { WordBookName: bookname } // 确保 `id` 字段与后端模型一致
    });

    // 更新前端数据
    books.value = books.value.filter(book => book.title!== bookname);
    console.log(`Book with id ${bookname} deleted successfully.`);
  } catch (error) {
    console.error(`Failed to delete book with id ${bookname}:`, error);
    alert('删除失败，请稍后重试');
  }


}

// 自定义词书方法
const getmyWordbooks = async (viewName, book) => {
  loading.value = true;
  error.value = null;
  currentTitle.value = book.title;
  isWordsVisible.value = true;
  triggeredByFunction.value = 'getmyWordbooks'; // 更新触发函数类型

  try {
    const response = await axios.get(`/words/GetAllWordsBymyViewName/${viewName}`);
    results.value = response.data.map(word => ({ ...word }));
  } catch (err) {
    error.value = '加载失败';
  } finally {
    loading.value = false;
  }
}

// 生词本
const getstartWordbooks = async () => {
  loading.value = true;
  error.value = null;
  isWordsVisible.value = true;
  triggeredByFunction.value = 'getstartWordbooks'; // 更新触发函数类型

  try {
    const response = await axios.get(`/words/GetStartWordsBymyViewName`);
    results.value = response.data.map(word => ({ ...word }));
  } catch (err) {
    error.value = '加载失败';
  } finally {
    loading.value = false;
  }
}

// 点击图片时触发的函数，加载对应词库的词汇
const fetchWords = async (viewName, book) => {
  loading.value = true;
  error.value = null;
  currentTitle.value = book.title;
  isWordsVisible.value = true;
  triggeredByFunction.value = 'fetchWords'; // 更新触发函数类型

  try {
    // 获取 Startresults 数据
    const Startresponse = await axios.get(`/words/GetStartWordsBymyViewName`);
    Startresults.value = Startresponse.data.map(word => ({ ...word }));
    console.log('Startresults:', Startresults.value);

    // 获取主词汇数据
    const response = await axios.get(`/words/GetWordsByViewName/${viewName}`);
    results.value = response.data.map(word => {
      // 查找 Startresults 中对应的 wordId
      const startWord = Startresults.value.find(
        startWord => String(startWord.wordId) === String(word.wordId) // 转换为相同类型进行比较
      );

      // 合并 status 数据
      if (startWord) {
        return { ...word, status: startWord.status };
      } else {
        return { ...word };
      }
    });

    console.log('Merged results:', results.value); // 检查最终结果
  } catch (err) {
    error.value = '加载失败';
    console.error('Error fetching words:', err); // 打印错误日志
  } finally {
    loading.value = false;
  }
};


// 返回到图片选择页面
const goBack = () => {
  currentTitle.value = '单词书',
  isWordsVisible.value = false  // 隐藏词汇列表
}


// 假设 word 是一个响应式对象，其中 status 是一个响应式属性
ref({
  status: 'unstart' // 初始状态，根据实际情况设置
});

// 切换收藏状态并更新数据库
const toggleFavorite = async (word) => {
  const newStatus = word.status === 'start' ? 'unstart' : 'start';
  const wordIdToSend = triggeredByFunction.value === 'getmyWordbooks' || triggeredByFunction.value === 'getstartWordbooks' ? word.wordId : word.id;

  try {
    await axios.post('/words/AddstartWord', { WordId: wordIdToSend, Status: newStatus });
    word.status = newStatus;
  } catch (err) {
    console.error('更新收藏状态失败', err.response?.data || err.message);
    word.status = word.status === 'start' ? 'unstart' : 'start';
  }
};

// 显示详情并朗读单词
const showDetails = (word) => {
  selectedWord.value = word
  dialogVisible.value = true
  speakWord(word)  // 在点击卡片时播放单词的朗读
}

const speakWord = (word) => {
  if (!word) return
  const textToSpeak = `${word.wordpre}`
  const utterance = new SpeechSynthesisUtterance(textToSpeak)
  utterance.lang = 'en-US' // 设置语言为美式英语
  speechSynthesis.speak(utterance)
}

const currentTitle = ref('单词书'); // 默认标题


// 切换标题的显示
const toggleTitle = () => {
  if (isWordsVisible.value) {
    isWordsVisible.value = false; // 隐藏词汇列表
  } else {
    // 如果不在词汇列表视图，可以在这里添加其他逻辑，例如显示关于页面的更多信息
  }
};
</script>



<style scoped>


.el-card {
  font-size: 1rem; /* 适当放大卡片内容的字体 */
}
.el-button {
  font-size: 1.2rem; /* 增大按钮的字体 */
}


html, body {
  font-size: 16px; /* 统一设置基础字体大小，可按需调整 */
  line-height: 1.5; /* 增加可读性 */
}



.book-list {
  display: flex;
  flex-wrap: wrap;
  justify-content: flex-start; /* 左对齐 */
}
.book-container {
  display: flex;
  width: 150px; /* 或者其他合适的宽度 */
  margin: 10px;
  text-align: center;
}
.book-container2 {
  width: 150px; /* 或者其他合适的宽度 */
  margin: 10px;
  text-align: center;
}
.book-info {
  display: flex;
  flex-direction: column;
  margin-right: 10px; /* 在书信息和删除按钮之间添加一些空间 */
}
.btsc  {
  width: 30px; /* 或者其他合适的宽度 */
  height: 50px;
  text-align: center;
  font-size: 12px;
}
.book-container img {
  width: 100%;
  height: auto;
  cursor: pointer; /* 当鼠标悬停时显示为可点击状态 */
}
.book-container2 img {
  width: 100%;
  height: auto;
  cursor: pointer; /* 当鼠标悬停时显示为可点击状态 */
}
.container {
  font-family: Arial, sans-serif;
  margin: 0;
  padding: 0;
}
header {
  background-color: #333;
  color: #fff;
  padding: 20px;
  text-align: center;
}
h1, h2 {
  margin: 0;
}


.main-content {
  display: flex;
  justify-content: space-between;
  padding: 20px;
}

.collect-word-book{
  width: 10%;
  background-color: #f1f1f1;
  padding: 30px;
  border-radius: 1px;
}


.my-book {
  width: 70%;
  background-color: #f1f1f1;
  padding: 20px;
  border-radius: 5px;
}
.book-list {
  display: flex;
  flex-wrap: wrap;
  justify-content: left;
}
.book-container {
  width: 150px;
  margin: 10px;
  text-align: center;
}
.book-container img {
  width: 100%;
  height: auto;
}
.add-book {
  display: flex;
  justify-content: center;
  align-items: center;
  background-color: #f1f1f1;
  border: 2px dashed #aaa;
  border-radius: 5px;
  height: 150px;
}
.add-book button {
  background: none;
  border: none;
  cursor: pointer;
}
.preset-books {
  display: flex;
  flex-wrap: wrap;
  justify-content: left;
  padding: 20px;
}
.modal {
  position: fixed;
  z-index: 1;
  left: 0;
  top: 0;
  width: 100%;
  height: 100%;
  overflow: auto;
  background-color: rgba(0, 0, 0, 0.4);
}
.modal-content {
  background-color: #fefefe;
  margin: 15% auto;
  padding: 20px;
  border: 1px solid #888;
  width: 30%;
}
.close-button {
  color: #aaa;
  float: right;
  font-size: 28px;
  font-weight: bold;
}
.close-button:hover,
.close-button:focus {
  color: black;
  text-decoration: none;
  cursor: pointer;
}
.results-container {
  margin-top: 20px;
}
.result-card {
  cursor: pointer;
  transition: transform 0.3s;
  width: 100%;
  height: 300px;
  display: flex;
  flex-direction: column;
}
.result-card:hover {
  transform: translateY(-5px);
}
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 30px;
  font-weight: bold;
}

.dictionary-image {
  cursor: pointer;
  margin: 10px;
}




</style>