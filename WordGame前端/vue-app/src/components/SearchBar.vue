<template>
    <div style="display: flex; justify-content: center; align-items: center;">
      <el-input
        v-model="query"
        placeholder="请输入搜索内容"
        style="width: 400px; margin-right: 10px;"
        @keyup.enter="onSearch"
      ></el-input>
      <el-button type="primary" @click="onSearch">搜索</el-button>
    </div>
  </template>
  
  <script setup>
  import { ref } from 'vue'
  import { useRouter } from 'vue-router'
  
  const query = ref('')
  const router = useRouter()
  
  const onSearch = () => {
    if (!query.value.trim()) {
      console.log('搜索内容为空')
      return
    }
    router.push({ name: 'SearchView', query: { q: query.value } })
  }
  </script>
  