<template>
  <div class="login">
    <div class="body">
      <div class="container">
        <h1>用户登录</h1>
        <el-form :model="ruleFrom" ref="loginForm" class="og-form" :rules="rules">
          <el-form-item label="账号" prop="email">
            <el-input v-model="ruleFrom.email" />
          </el-form-item>
          <el-form-item label="密码" prop="password">
            <el-input type="password" v-model="ruleFrom.password" />
          </el-form-item>
        </el-form>
        <el-button size="large" type="primary" @click="submitForm(loginForm)">登录</el-button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, toRefs, ref } from 'vue';
// import { useStore } from 'vuex';
import { ElMessage } from 'element-plus';

// const store = useStore();
const loginForm = ref(null);

const state = reactive({
  ruleFrom: {
    email: "22",
    password: "333"
  }
});

const rules = reactive({
  email: [
    { required: true, message: '请输入账号！', trigger: 'blur' },
  ],
  password: [
    { required: true, message: '请输入密码！', trigger: 'blur' },
  ],
});

const submitForm = async (formEl) => {
  if (!formEl) return;
  await formEl.validate(async (valid) => {
    if (valid) {
      try {
        // 验证通过进行登录
        // await store.dispatch('authModule/userLoginAction', state.ruleFrom);
        console.log('submit!');
      } catch (error) {
        // 处理登录错误，显示提示信息
        ElMessage.error('登录失败，请检查账号和密码是否正确');
      }
    }
  });
};

const { ruleFrom } = toRefs(state);
</script>
  
  <style scoped>
  .login {
    /* background: url("../../assets/logo2.png"); */
    height: 100%;
    width: 100%;
    position: fixed;
    background-size: cover;
  }
  
  .body {
    display: flex;
    justify-content: center;
    align-items: center;
    margin-top: 15%;
  }
  
  .container {
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: column;
    width: 420px;
    height: 300px;
    background-color: #fff;
    border-radius:10px;
    box-shadow: 0px 21px 41px 0px rgba(0, 0, 0, 0.2);
    font-size: 12px;
    color: black;
  }
  </style>