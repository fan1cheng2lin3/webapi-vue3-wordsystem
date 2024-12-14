<template>
  <el-container>
    <el-aside width="200px" style="height: 100vh;">
      <el-scrollbar style="background-color:#303133">
        <div class="mb-2 logo">水果ERP管理系统</div>
        <el-menu 
          :default-openeds="['1', '2']"
          active-text-color="#ffd04b"
          background-color="#303133"
          text-color="#fff"
          :router="true"
        >
          <el-sub-menu index="1">
            <template #title>
              <el-icon><message /></el-icon>档案管理
            </template>
            <el-menu-item-group>
              <!-- 自定义档案，点击时不跳转 -->
              <el-menu-item @click="handleCustomCategoryClick">
                <el-icon><HomeFilled /></el-icon>自定义档案
              </el-menu-item>

              <!-- 动态加载分类信息 -->
              <el-menu-item
                v-for="(item) in categoryList"
                :key="item.id"
                :index="'category-' + item.id"
                @click="handleCategoryClick(item)"
              >
                {{ item.name }}
              </el-menu-item>

              <!-- 其他档案 -->
              <el-menu-item index="/home">其他档案</el-menu-item>
            </el-menu-item-group>
          </el-sub-menu>

          <el-sub-menu index="2">
            <template #title>
              <el-icon><Setting /></el-icon>管理功能
            </template>
            <el-menu-item-group>
              <el-menu-item index="/Users"><el-icon><UserFilled /></el-icon>用户管理</el-menu-item>
              <el-menu-item index="/Roles"><el-icon><Avatar /></el-icon>角色管理</el-menu-item>
              <el-menu-item index="/Permissions"><el-icon><EditPen /></el-icon>权限管理</el-menu-item>
            </el-menu-item-group>
          </el-sub-menu>


          <el-sub-menu index="2">
            <template #title>
              <el-icon><Setting /></el-icon>模块管理
            </template>
            <el-menu-item-group>
              <el-menu-item index="/Users"><el-icon><UserFilled /></el-icon>采购</el-menu-item>
              <el-menu-item index="/Roles"><el-icon><Avatar /></el-icon>销售</el-menu-item>
              <el-menu-item index="/Permissions"><el-icon><EditPen /></el-icon>库存</el-menu-item>
              <el-menu-item index="/"><el-icon><EditPen /></el-icon>财务</el-menu-item>
            </el-menu-item-group>
          </el-sub-menu>


        </el-menu>
      </el-scrollbar>
    </el-aside>

    <el-container>
      <el-header style="text-align: right; font-size: 12px">
        <div class="toolbar">
          <el-dropdown>
            <el-icon style="margin-right: 8px; margin-top: 1px">
              <setting />
            </el-icon>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item @click="Logout">退出登陆</el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
          <span>Tom</span>
        </div>
      </el-header>

      <el-main>
        <router-view></router-view>
      </el-main>
      <el-footer>Footer</el-footer>
    </el-container>
  </el-container>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import axios from 'axios';
import { HomeFilled, Setting, UserFilled, Avatar, EditPen } from '@element-plus/icons-vue';

const store = useStore();
const router = useRouter();

// 定义分类列表的响应式数据
const categoryList = ref([]);

// 获取分类信息的函数
const getList = () => {
  axios.get('/category').then((res) => {
    categoryList.value = res.data;  // 假设返回的数据是一个包含分类的数组
    console.log(res.data);  // 调试输出分类数据
  });
};

// 在组件加载时获取分类数据
onMounted(() => {
  getList();
});

// 处理点击分类
const handleCategoryClick = (item) => {
  console.log('点击了分类:', item);
  // 跳转到分类详情页

};

// 处理点击“自定义档案”，不做任何路由跳转
const handleCustomCategoryClick = () => {
  console.log('点击了自定义档案');
  // 可以在这里做一些操作，比如展开其他菜单项或显示内容
  router.push({ name: 'category' });
};

// 注销函数
const Logout = () => {
  store.dispatch('authModule/logout');
  router.push('/login');
};
</script>

<style scoped>
.el-header {
  position: relative;
  background-color: white;
  color: var(--el-text-color-primary);
  box-shadow: var(--el-box-shadow-dark);
}

.layout-container-demo .el-aside {
  color: var(--el-text-color-primary);
  background-color: #303133;
}

.layout-container-demo .el-menu {
  border-right: none;
}

.layout-container-demo .el-main {
  padding: 0;
  box-shadow: var(--el-box-shadow);
  margin: 6px 0px;
}

.layout-container-demo .toolbar {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  height: 100%;
  right: 20px;
}

.logo {
  height: 50px;
  color: white;
  text-align: center;
  line-height: 50px;
  font-weight: bold;
}

.layout-container-demo {
  height: 100vh;
}

.el-footer {
  box-shadow: var(--el-box-shadow);
}
</style>
