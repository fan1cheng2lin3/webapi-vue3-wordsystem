<template>
  <el-container style="height: 100vh; overflow: auto">
    <el-header style="display: flex; justify-content: space-between; align-items: center; height: 60px; padding: 0 20px; background-color: #2c3e50; color: #ecf0f1; box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);">
  <!-- 导航 -->
  <div class="nav-container" style="display: flex; gap: 30px;">
    <span class="nav-item" @click="goBack" style="font-size: 18px; cursor: pointer; transition: color 0.3s ease;">首页</span>
    <span class="nav-item" @click="gociku" style="font-size: 18px; cursor: pointer; transition: color 0.3s ease;">词库</span>
    <span class="nav-item" @click="godate" style="font-size: 18px; cursor: pointer; transition: color 0.3s ease;">数据</span>
  </div>

  <!-- 搜索框 -->
  <div class="search-container" @click.stop style="flex-grow: 1; display: flex; justify-content: center;">
    <el-input
      v-model="searchQuery"
      :style="{ width: searchFocused ? '600px' : '300px', transition: 'width 0.3s ease' }"
      placeholder="请输入搜索内容"
      @focus="searchFocused = true"
      @blur="onSearchBlur"
      @keyup.enter="search"
      style="border-radius: 20px; background-color: #fff; border: none; box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);"
    >
      <template #append>
        <div class="search-icons" style="display: flex; align-items: center;">
          <el-icon v-show="searchFocused" @click="clearSearch" class="icon-clear" style="cursor: pointer; color: #ccc;"><Close /></el-icon>
          <span class="divider" v-show="searchFocused" style="color: #ccc; margin: 0 8px;">|</span>
          <el-icon @click="search" class="icon-search" style="cursor: pointer; color: #3498db;"><Search /></el-icon>
        </div>
      </template>
    </el-input>
  </div>

  <!-- 用户名和退出按钮 -->
  <div class="toolbar" style="display: flex; align-items: center; gap: 12px;">
    <span style="font-size: 16px; color: #fff; font-weight: bold;">{{ username }}</span>
    <el-dropdown>
      <el-icon style="margin-left: 8px; margin-top: 1px; color: #fff;">
        <setting />
      </el-icon>
      <template #dropdown>
        <el-dropdown-menu>
          <el-dropdown-item @click="logout" style="background-color: #ecf0f1; color: #2c3e50;">退出</el-dropdown-item>
        </el-dropdown-menu>
      </template>
    </el-dropdown>
  </div>
</el-header>



    <el-main style="flex: 1; overflow: auto; padding: 20px;">
      <router-view></router-view>
    </el-main>

    <!-- <el-footer style="height: 40px; line-height: 40px; text-align: center;">
      Footer
    </el-footer> -->
  </el-container>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';
import { useStore } from 'vuex';
import { ElMessage } from 'element-plus';

const searchQuery = ref('');
const username = ref('Tom');
const router = useRouter();
const store = useStore();
const searchFocused = ref(false);

// 返回上一级
const goBack = () => {
  router.push({ name: 'zhuyeView' });
};

const gociku = () => {
  router.push({ name: 'CikuView' });
};

const godate = () => {
  router.push({ name: 'aaa' });
};

// 搜索
const search = async () => {
  if (!searchQuery.value.trim()) {
    console.log('搜索内容为空');
    return;
  }

  try {
    const response = await axios.get('/words/search', { params: { query: searchQuery.value } });
    console.log('搜索结果：', response.data);

    if (response.data && response.data.length > 0) {
      router.push({ name: 'SearchView', query: { q: searchQuery.value } });
    } else {
      ElMessage({
        message: '没有找到与搜索词相关的结果',
        type: 'info',
        duration: 600,
      });
    }
  } catch (error) {
    console.error('搜索失败：', error.response?.data || error.message);
    ElMessage({
      message: '没有找到',
      type: 'warning',
      duration: 600,
    });
  }
};

// 清除搜索内容
const clearSearch = () => {
  searchQuery.value = '';
};

// 退出登录
const logout = () => {
  store.dispatch('authModule/logout');
  router.push('/login');
};

const onSearchBlur = () => {
  setTimeout(() => {
    searchFocused.value = false;
  }, 200); // 延迟以避免失焦后无法点按钮
};

onMounted(() => {
  document.addEventListener('click', () => {
    searchFocused.value = false;
  });
});

onUnmounted(() => {
  document.removeEventListener('click', () => {
    searchFocused.value = false;
  });
});
</script>

<style scoped>
.search-container {
  display: flex;
  align-items: center;
  transition: width 0.3s ease;
  cursor: pointer;
}
.search-icons {
  display: flex;
  align-items: center;
}
.icon-clear,
.icon-search {
  cursor: pointer;
  margin-left: 8px;
}
.divider {
  margin: 0 8px;
  color: #dcdfe6;
}
.el-input {
  transition: width 0.3s ease;
}
.nav-container {
  display: flex;
  justify-content: left;
  flex-grow: 1;
  flex-wrap: nowrap;

}
.nav-item {
  margin: 0 20px;
  cursor: pointer;
  font-size: 26px;
  color: #bb9441;
}
.nav-item:hover {
  text-decoration: underline;
  color: var(--el-color-primary);
}
</style>
