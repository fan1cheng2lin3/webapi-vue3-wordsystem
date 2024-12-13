<template>
  <div class="book-list">
    <div class="books-and-info">
      <div v-for="book in nowbooks" :key="book.userid" class="book-container">
        <div class="book-frame">
          <h1>在学词书</h1>
          <img :src="book.imageUrl" @click="goputbook" class="dictionary-image" />
          <p style="font-size: 24px;">
            {{ book.wordbook === 'xiaoyvtianshi' ? '生词本' : book.wordbook.replace(/^View_/, '') }}
          </p>
        </div>
        <button @click="goputbook">更换在学词书</button>

      
      </div>
    </div>
    <div class="additional-content">
      <h1>数据概览</h1>
        <!-- 学习时长和学习词数展示 -->
        <div class="study-info">
          <div class="time-info">
            <ul>
              <li>
                今日学习时长
              </li>
              <span style="font-size: 50px; color: orange;">{{ nowtime }}</span> 分钟
              <li>
                总学习时长 
              </li>
              <span style="font-size: 50px; color: orange;">{{ Totaltime }}</span> 分钟
            </ul>
          </div>
          <div class="word-info">
            <ul>
              <li>
                今日学习
              </li>
              <span style="font-size: 50px; color: orange;">{{ wordcoutnowday }}</span> 词
              <li>
                累计学习
              </li>
              <span style="font-size: 50px; color: orange;">{{ wordcout }}</span> 词
            </ul>
          </div>
        </div>
    </div>
  </div>

  <div>
    <!-- 设置更大的图表容器 -->
    <div id="main" style="width: 80%; height: 600px;"></div>
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
const nowtime = ref([]);
const Totaltime = ref([]);
const wordcoutnowday = ref([]);
const wordcout = ref([]);
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
    }));


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
.book-list {
  display: flex;
  justify-content: space-between;
  padding: 30px;
}

.books-and-info {
  display: flex;
  flex-wrap: wrap;
  gap: 30px;
}

.book-container {
  display: flex;
  flex-direction: column;
  text-align: center;
  border: 1px solid #ccc;
  border-radius: 15px;
  padding: 20px;
  background-color: #f9f9f9;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
  transition: transform 0.3s ease, box-shadow 0.3s ease;
  width: calc(60%); /* 减去间隔 */
  margin-right: 1px; /* 右侧间隔 */
}

.book-container:hover {
  transform: translateY(-5px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.3);
}

.dictionary-image {
  width: 100%;
  height: auto;
  border-radius: 15px;
  margin-bottom: 5px;
  cursor: pointer;
}

button {
  margin-top: 20px;
  padding: 10px 20px;
  font-size: 22px;
  border: none;
  border-radius: 15px;
  background-color: #007bff;
  color: white;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

button:hover {
  background-color: #0056b3;
}

.study-info {
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  gap: 20px;
  margin-left: 20px;
  width: 300px;
  flex-shrink: 0;
}

.study-info .time-info, .study-info .word-info {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.study-info ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.study-info li {
  font-size: 24px;
  color: #555;
}

.study-info li span {
  font-size: 30px;
  color: #ff8c00;
}

.additional-content {
  flex-grow: 1;
  padding: 20px;
  border: 1px solid #ccc;
  border-radius: 15px;
  background-color: #f0f0f0;
  margin-left: 30px;
}
</style>