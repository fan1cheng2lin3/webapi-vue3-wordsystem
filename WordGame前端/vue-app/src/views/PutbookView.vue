<template>
  <!-- 预设词库 -->
  <div class="preset-books" v-if="!isWordsVisible">
    <h2>预设词库</h2>
    <div class="book-frame">

      <div class="book-list">
        <div class="book-container2" v-for="preset in presetBooks" :key="preset.id">
          <img
            :src="preset.imageUrl"
            :alt="preset.title"
            class="dictionary-image"
          />
          <h3 class="s">{{ preset.title }}</h3>
          <button @click="changeWordbook(preset.viewName)">选择</button>
        </div>
      </div>
    </div>
  </div>
  
  <!-- 用户词库 -->
  <div class="my-book">
    <h2>用户词库</h2>
    <div class="book-frame">
   
      <div class="book-list">
        <div class="book-container" v-for="book in books" :key="book.id">
          <img
            :src="book.imageUrl"
            :alt="book.title"
            class="dictionary-image"
          />
          <h3>{{ book.title }}</h3>
          <button @click="changeWordbook(book.viewName)">选择</button>
        </div>
      </div>
    </div>
  </div>
</template>


    <script setup>
    
    import { ref, onMounted  } from 'vue';
    import axios from 'axios'
    
    const presetBooks = ref([
    {
        id: 1,
        title: '生词本',
        imageUrl: 'https://th.bing.com/th/id/OIP.YnDSAk0EwMlWLmlGAkftwwHaHa?rs=1&pid=ImgDetMain',
        viewName: 'xiaoyvtianshi'
      },
      {
        id: 2,
        title: 'Associate Degree',
        imageUrl: 'https://th.bing.com/th/id/OIP.YnDSAk0EwMlWLmlGAkftwwHaHa?rs=1&pid=ImgDetMain',
        viewName: 'View_AssociateDegree'
      },
      {
        id: 3,
        title: 'Bachelor',
        imageUrl: 'https://th.bing.com/th/id/OIP.YnDSAk0EwMlWLmlGAkftwwHaHa?rs=1&pid=ImgDetMain',
        viewName: 'View_Bachelor'
      },
      {
        id: 4,
        title: 'Base',
        imageUrl: 'https://th.bing.com/th/id/OIP.YnDSAk0EwMlWLmlGAkftwwHaHa?rs=1&pid=ImgDetMain',
        viewName: 'View_Base'
      },
     
    ]);
    
    // 定义 books 为响应式变量，并初始化为空数组
    const books = ref([]);
    
    
    // 在组件挂载时获取词书数据
    onMounted(async () => {
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
    
    
    });
    import { useRouter } from 'vue-router';
const router = useRouter();

    
  
    // 添加 changeWordbook 方法
    const changeWordbook = async (viewName) => {
      try {
        const response = await axios.put('/Users/change-wordbook', String(viewName), {
          headers: { 'Content-Type': 'application/json' } // 确保请求头是 application/json
        });
    
        if (response.status === 200) {
          console.log('词书更改成功');
          // 成功后的逻辑处理
          router.push('/aaa');


        } else {
          console.error('词书更改失败', response.data);
        }
      } catch (error) {
        console.error('请求失败', error);
      }
    };
    
    </script>
    
    <style scoped>
    /* 通用样式 */
    .book-list {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
      gap: 30px;
      padding: 30px;
      justify-content: center;
    }
    
    .book-frame {
      border: 1px solid #ccc; /* 添加边框 */
      border-radius: 15px; /* 边框圆角 */
      padding: 30px; /* 增加内边距 */
      background-color: #f9f9f9; /* 背景颜色 */
      margin-bottom: 30px; /* 框之间的间隔 */
      text-align: center;
    }
    
    .book-container,
    .book-container2 {
      text-align: center;
    }
    
    .dictionary-image {
      width: 200px; /* 图片大小 */
      height: 200px; /* 确保图片大小一致 */
      object-fit: cover; /* 确保图片适配框体 */
      border-radius: 15px;
      cursor: pointer;
      transition: transform 0.3s ease, box-shadow 0.3s ease;
    }
    
    .dictionary-image:hover {
      transform: scale(1.1); /* 鼠标悬停放大效果 */
      box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2); /* 阴影效果 */
    }
    
    .s {
      font-size: 18px; /* 增大字体大小 */
      color: #333;
      margin-top: 10px;
      word-wrap: break-word;
    }
    
    button {
      margin-top: 15px; /* 按钮与标题间隔 */
      padding: 10px 20px; /* 按钮内边距 */
      font-size: 16px; /* 按钮字体大小 */
      border: none; /* 无边框 */
      border-radius: 8px; /* 按钮圆角 */
      background-color: #007bff; /* 按钮背景颜色 */
      color: white; /* 按钮文字颜色 */
      cursor: pointer;
      transition: background-color 0.3s ease;
    }
    
    button:hover {
      background-color: #0056b3; /* 鼠标悬停时按钮背景颜色变深 */
    }
    </style>