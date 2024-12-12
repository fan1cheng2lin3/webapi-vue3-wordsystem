<template>
  <div>
    <div v-if="loading" style="text-align: center;">加载中...</div>
    <div v-else-if="error" style="text-align: center; color: red;">{{ error }}</div>
    <div v-else class="results-container">
      <el-row :gutter="20">
        <el-col :span="6" v-for="word in results" :key="word.wordpre">
          <el-card class="result-card" @click="showDetails(word)">
            <div class="card-header">
              <span>{{ word.wordpre }}</span>
              <!-- 收藏按钮 -->
              <el-button 
                @click.stop="toggleFavorite(word)" 
                :type="word.isFavorite ? 'warning' : 'text'" 
                class="favorite-button">
                {{ word.isFavorite ? '已收藏' : '收藏' }}
              </el-button>
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
    <el-dialog v-model="dialogVisible" title="详细信息" width="600px">
      <div v-if="selectedWord">
        <p><strong>单词:</strong> {{ selectedWord.wordpre }}</p>
        <p><strong>美式音标:</strong> {{ selectedWord.phonetic }}</p>
        <p><strong>英式音标:</strong> {{ selectedWord.phonetic_uk }}</p>
        <p><strong>释义:</strong> {{ selectedWord.explain }}</p>
        <p><strong>例句:</strong></p>
        <p>{{ selectedWord.sentence_en }}</p>
        <p>{{ selectedWord.sentence_cn }}</p>
        <p><strong>辅助记忆:</strong> {{ selectedWord.ancillary || '无' }}</p>
        <el-button @click="speakWord" type="primary">朗读单词</el-button>
        
      </div>
      <template #footer>
        <el-button @click="dialogVisible = false">关闭</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, watch, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import axios from 'axios'

const route = useRoute()
const results = ref([])
const loading = ref(true)
const error = ref(null)
const dialogVisible = ref(false)
const selectedWord = ref(null)

// 搜索方法
const fetchResults = async (query) => {
  if (!query.trim()) {
    error.value = '搜索关键词为空'
    loading.value = false
    return
  }

  try {
    loading.value = true
    error.value = null
    const response = await axios.get('/words/search', { params: { query } })
    results.value = response.data.map((item) => ({ ...item, isFavorite: item.isFavorite || false }))
  } catch (err) {
    error.value = err.response?.data || '加载失败'
  } finally {
    loading.value = false
  }
}

// 切换收藏状态并更新数据库
const toggleFavorite = async (word) => {
  word.isFavorite = !word.isFavorite
  try {
    await axios.post('/words/favorite', { wordId: word.id, isFavorite: word.isFavorite })
  } catch (err) {
    console.error('更新收藏状态失败', err.response?.data || err.message)
    // 如果失败，恢复原状态
    word.isFavorite = !word.isFavorite
  }
}

// 显示详情并朗读单词
const showDetails = (word) => {
  selectedWord.value = word
  dialogVisible.value = true
  speakWord(word) // 点击时也调用朗读方法
}

// 朗读单词
const speakWord = (word) => {
  if (!word) return

  const textToSpeak = `${word.wordpre}`

  const utterance = new SpeechSynthesisUtterance(textToSpeak)
  utterance.lang = 'en-US' // 设置语言为美式英语
  speechSynthesis.speak(utterance)
}

// 监听路由变化
watch(
  () => route.query.q,
  (newQuery) => {
    fetchResults(newQuery || '')
  },
  { immediate: true } // 初始化时也执行
)

onMounted(() => {
  fetchResults(route.query.q || '')
})
</script>

<style scoped>
.results-container {
  margin-top: 20px;
}
.result-card {
  cursor: pointer;
  transition: transform 0.3s;
  width: 100%;
  height: 300px; /* 固定卡片高度 */
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
.favorite-button {
  cursor: pointer;
}
.card-body {
  margin-top: 10px;
  flex-grow: 1;
}
</style>
