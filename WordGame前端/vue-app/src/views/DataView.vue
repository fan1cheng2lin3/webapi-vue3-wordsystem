<template>
  <div class="container">
    <!-- 左侧：在学词书列表 -->
    <div class="left-panel">
      <h1>在学词书</h1>
      <div v-for="book in nowbooks" :key="book.userid" class="book-card">
        <img :src="book.imageUrl" @click="goputbook" class="book-image" />
        <p class="book-title">
          {{ book.wordbook === 'xiaoyvtianshi' ? '生词本' : book.wordbook.replace(/^View_/, '') }}
          
        </p>
      
        <button @click="goputbook">更换词书</button>


        <p v-if="progressData[book.wordbook]">
  恭喜你已经攻下本词书 {{ progressData[book.wordbook].toFixed(1) }}% 的词汇， 
  {{ getEncouragement(progressData[book.wordbook]) }}
</p>


         </div>

      <div>
  
  </div>
    </div>

    <!-- 右侧：学习数据概览 -->
    <div class="right-panel">
      <div class="study-summary">
        <h1>数据概览</h1>
        <div class="study-info">
          <div class="info-box">
            <p>今日学习时长</p>
            <span>{{ nowtime }} 分钟</span>
          </div>
          <div class="info-box">
            <p>总学习时长</p>
            <span>{{ Totaltime }} 分钟</span>
          </div>
          <div class="info-box">
            <p>今日学习单词</p>
            <span>{{ wordcoutnowday }} 词</span>
          </div>
          <div class="info-box">
            <p>累计学习单词</p>
            <span>{{ wordcout }} 词</span>
          </div>
        </div>
      </div>

      <!-- 图表容器 -->
      <div class="chart-container">
        <h1>学习趋势</h1>
        <div id="main" style="width: 100%; height: 400px;"></div>
      </div>
    </div>
  </div>





</template>

<script setup>
import { ref, onMounted  } from 'vue';
import axios from 'axios'
import { useRouter } from 'vue-router';
import * as echarts from 'echarts';

const router = useRouter();
const books = ref([]);
const nowbooks = ref([]);
const nowtime = ref([0]);
const Totaltime = ref([0]);
const wordcoutnowday = ref([0]);
const wordcout = ref([0]);
const progressData = ref({});


// 文本数组
const encouragementTexts = [
  "有仪式感的人生从背单词开始",
  "背单词不仅在于方法，还在于坚持",
  "每天进步一点点的感觉是不是很棒？",
  "做一个学霸，你准备好了吗？",
  "你能做到的比想象的更多",
  "相信是成功的起点，坚持是成功的终点",
  "坚持把平凡的事情做好，就是不平凡",
  "罗马不是一日建成的，再接再厉哦",
  "积少成多，滴水穿石",
  "放弃不难，但坚持一定很苦",
  "放慢脚步可以，但不要停滞不前哦",
  "不轻言放弃，幸运总会与你不期而遇",
  "成功不过是在紧要关头多了一份坚持",
  "坚持一件事越久，你收获的惊喜越多",
  "比智慧更强大的是毅力",
  "不忘初心，方得始终",
  "自律及自由",
  "自律既自由",
  "未来永远从现在开始。",
  "要看日出的人必须守到佛晓",
  "21天，背单词就能成为一个习惯",
  "当你知道自己要什么时，整个世界都会为你让",
  "时间不会辜负梦想",
  "心存梦想，机遇就会降临",
  "当机会来临时，你已经准备好了",
  "不鸣则已，一鸣惊人",
  "成功近在咫尺",
  "念念不忘，必有回响",
  "成功上岸"
];




// 获取鼓励文本函数
function getEncouragement(progress) {
  const index = Math.min(Math.floor(progress / 3.5), encouragementTexts.length - 1); // 根据进度计算索引
  return encouragementTexts[index];
}


// 在组件挂载时获取词书数据
onMounted(async () => {

 // 获取 DOM 元素来绑定图表
 const chartDom = document.getElementById('main');
  const myChart = echarts.init(chartDom);


  try {
    // 获取所有词书数据
    const response = await axios.get('/AddWordBook/getWordBooks');
    if (response.data && Array.isArray(response.data)) {
      // 处理词书数据
      books.value = response.data.map((name, index) => ({
        id: index + 1,
        title: name,
        imageUrl: 'https://th.bing.com/th/id/OIP.YnDSAk0EwMlWLmlGAkftwwHaHa?rs=1&pid=ImgDetMain', 
        viewName: name
      }));
    }
  } catch (error) {
    console.error('未能获取词书数据', error);
  }

  try {
    const response = await axios.get('/Users/getbook');
    nowbooks.value = response.data.map(word => ({
      ...word,
      imageUrl: 'https://th.bing.com/th/id/OIP.YnDSAk0EwMlWLmlGAkftwwHaHa?rs=1&pid=ImgDetMain',
      tableName: word.wordbook // 将 `wordbook` 直接作为 `tableName`
    }));


      // 2. 遍历词书，处理学习进度
      const progressPromises = nowbooks.value.map(async (book) => {
      if (book.wordbook === 'xiaoyvtianshi') {
        // 特殊处理：调用 /words/learning-progress-start
        try {
          const res = await axios.get('/words/learning-progress-start');
          progressData.value[book.wordbook] = res.data.learningProgress || 0; // 如果返回空，设置默认值0
        } catch (error) {
          console.error('特殊接口获取进度失败', error);
          progressData.value[book.wordbook] = 0;
        }
      } else {
        try {
          // 优先调用主接口 /words/learning-progress/{tableName}
          const res = await axios.get(`/words/learning-progress/${book.tableName}`);
          progressData.value[book.wordbook] = res.data.learningProgress || 0; // 设置默认值0
        } catch (error) {
          console.warn(`主接口获取 ${book.tableName} 进度失败，尝试备用接口`, error);

          // 如果主接口失败，调用备用接口
          try {
            const fallbackRes = await axios.get(`/words/learning-progress-wordbook?wordBookName=${book.wordbook}`);
            progressData.value[book.wordbook] = fallbackRes.data.learningProgress || 0;
          } catch (fallbackError) {
            console.error(`备用接口获取 ${book.wordbook} 进度失败`, fallbackError);
            progressData.value[book.wordbook] = 0; // 设置默认值
          }
        }
      }
    });

    // 3. 等待所有进度请求完成
    await Promise.all(progressPromises);

  } catch (error) {
    console.error('未能获取用户词书进度', error);
  }


  try {
    const response = await axios.get('/LearningProgress/getstudytime');
    nowtime.value = response.data
  } catch (error) {
    console.error('未能获取今日学习时长', error);
  }


  try {
    const response = await axios.get('/LearningProgress/gettotalstudytime');
    Totaltime.value = response.data
  } catch (error) {
    console.error('未能获取总学习时长', error);
  }

  try {
    const response = await axios.get('/LearningProgress/Getwordcoutnowday');
    wordcoutnowday.value = response.data
  } catch (error) {
    console.error('未能获取今日学习数量', error);
  }

  try {
    const response = await axios.get('/LearningProgress/Getwordcoutquanbu');
    wordcout.value = response.data
  } catch (error) {
    console.error('未能获取全部学习数量', error);
  }

  try {
  const response = await axios.get('/LearningProgress/getstudyalltime');
  console.log("asdads", response.data); // 打印后端数据

  // 假设后端返回的数据如下
  const studyData = response.data; // [{date: '2024-12-13', studyTime: 3}, ...]

  // 获取今天的日期
  const today = new Date();

  // 创建一个日期数组（过去7天）
  const dateLabels = [];
  for (let i = 6; i >= 0; i--) {
    const date = new Date(today);
    date.setDate(today.getDate() - i); // 获取过去7天的日期
    const formattedDate = date.toISOString().split('T')[0]; // 格式化日期为 'YYYY-MM-DD'
    dateLabels.push(formattedDate);
  }

  // 创建对应的学习时长数组
  const studyTimes = dateLabels.map(date => {
    const entry = studyData.find(item => item.date === date); // 查找对应日期的学习时长
    return entry ? entry.studyTime : 0; // 如果找不到数据，默认学习时长为0
  });

  // 将数据传递给 ECharts 配置
  const option = {
    tooltip: {
      trigger: 'axis', // 鼠标悬停时触发提示框
      axisPointer: {
        type: 'line', // 鼠标悬停时的指示器类型（可以选择line, shadow等）
      },
      formatter: function (params) {
        const date = params[0].name; // 获取x轴日期
        const studyTime = params[0].value; // 获取y轴学习时长
        return `${date}<br/>学习时长: ${studyTime} 分钟`; // 自定义显示格式
      },
      textStyle: {
        fontWeight: 'bold', // 加粗
        fontSize: 14, // 字体大小
        fontFamily: 'Arial Black', // 使用黑体字体
      }
    },
    xAxis: {
      type: 'category',
      data: dateLabels, // 将过去7天的日期传递给 xAxis
      axisLabel: {
        textStyle: {
          fontWeight: 'bold', // 加粗
          fontSize: 14, // 字体大小
          fontFamily: 'Arial Black', // 使用黑体字体
        }
      }
    },
    yAxis: {
      type: 'value',
      axisLabel: {
        formatter: '{value} 分钟', // 在Y轴标签中添加单位 '分钟'
        textStyle: {
          fontWeight: 'bold', // 加粗
          fontSize: 14, // 字体大小
          fontFamily: 'Arial Black', // 使用黑体字体
        }
      }
    },
    series: [
      {
        data: studyTimes, // 将学习时长数组传递给 series
        type: 'line',
        smooth: true, // 平滑曲线
        lineStyle: {
          width: 3, // 设置线条宽度
        }
      }
    ]
  };

  // 这里是将 option 应用到 ECharts 实例中的代码
  myChart.setOption(option);

} catch (error) {
  console.error('未能获取全部学习数量', error);
}


});


const goputbook = async () => {

  router.push('/putbook');

}

</script>

<style scoped>
/* 容器布局 */
.container {
  display: flex;
  gap: 20px;
  padding: 20px;
  background-color: #f7f7f7;
}

/* 左侧：词书列表 */
.left-panel {
  flex: 1;
  background-color: #fff;
  padding: 20px;
  border-radius: 10px;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
}

.book-card {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-bottom: 20px;
  padding: 15px;
  border: 1px solid #ddd;
  border-radius: 10px;
  background-color: #f9f9f9;
}

.book-image {
  width: 80%;
  border-radius: 10px;
  cursor: pointer;
}

.book-title {
  font-size: 18px;
  margin: 10px 0;
  font-weight: bold;
  color: #333;
}

button {
  padding: 8px 15px;
  background-color: #007bff;
  color: #fff;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-size: 16px;
}

button:hover {
  background-color: #0056b3;
}

/* 右侧：数据和图表 */
.right-panel {
  flex: 2;
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.study-summary {
  background-color: #fff;
  padding: 20px;
  border-radius: 10px;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
}

.study-info {
  display: flex;
  gap: 20px;
  flex-wrap: wrap;
}

.info-box {
  flex: 1;
  text-align: center;
  padding: 15px;
  border: 1px solid #ddd;
  border-radius: 10px;
  background-color: #f9f9f9;
}

.info-box p {
  font-size: 18px;
  margin-bottom: 10px;
}

.info-box span {
  font-size: 24px;
  font-weight: bold;
  color: #ff8c00;
}

.chart-container {
  background-color: #fff;
  padding: 20px;
  border-radius: 10px;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
}
</style>