// src/composables/useStudyTime.js
import { ref, onMounted, onBeforeUnmount } from 'vue';
import axios from 'axios';

export function useStudyTime() {
  const startTime = ref(null);
  const timeElapsed = ref(0);
  let timer = null;

  // 页面加载时记录开始时间
  onMounted(() => {
    startTime.value = Date.now();

    // 每秒更新一次已过的时间
    timer = setInterval(() => {
      timeElapsed.value = Math.floor((Date.now() - startTime.value) / 1000);
    }, 1000);

    window.addEventListener('beforeunload', handleBeforeUnload);
  });

  // 页面卸载时停止计时并保存学习时间
  onBeforeUnmount(() => {
    clearInterval(timer);
    window.removeEventListener('beforeunload', handleBeforeUnload);
  });

  // 页面关闭时的处理逻辑
  async function handleBeforeUnload(event) {
    clearInterval(timer);
    const totalTime = Math.floor((Date.now() - startTime.value) / 1000);

    try {
      // 异步发送学习时间
      await axios.post('/LearningProgress/addstudytime', {
        userId: 1,
        adduptime: totalTime.toString(),
        day: "",
      });
      console.log('学习时间已成功记录');
    } catch (error) {
      console.error('记录学习时间失败:', error);
    }

    event.returnValue = ''; // 标明你想触发 unload 时显示警告
  }

  return {
    timeElapsed,
    handleManualSend: handleBeforeUnload, // 手动触发发送学习时间
  };
}
