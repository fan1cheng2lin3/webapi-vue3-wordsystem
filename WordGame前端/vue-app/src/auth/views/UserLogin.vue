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
          <el-button size="larg" type="primary" @click="submitForm(loginForm)">登录</el-button>
        </div>
      </div>
    </div>
  </template>
  
  <script setup>
  import { reactive, toRefs ,ref} from 'vue';
  import { useStore } from 'vuex';
  import { ElMessageBox } from 'element-plus';

  const store = useStore()
  const loginForm = ref()
  
  const state = reactive({
    ruleFrom: {
      email: "111",
      password: "111"
    }
  })

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
  try {
    const valid = await formEl.validate();
    if (valid) {
      // 验证通过进行登录
      const response = await store.dispatch('authModule/userLoginAction', state.ruleFrom);
      if (response && response.error) {
        // 假设后端返回的错误信息在response.error中
        ElMessageBox({
          title: '登录失败',
          message: response.error.message,
          confirmButtonText: '确定',
          type: 'error',
        });
      } else {
        console.log('登录成功');
      }
    } else {
      // 验证失败，显示表单错误信息
      ElMessageBox({
        title: '登录失败',
        message: '账号或密码不正确，请重新输入',
        confirmButtonText: '确定',
        type: 'error',
      });
    }
  } catch (error) {
    // 捕获validate方法抛出的错误
    ElMessageBox({
      title: '错误',
      message: '账号或密码不正确，请重新输入',
      confirmButtonText: '确定',
      type: 'error',
    });
    console.error('表单验证出错:', error);
  }
}
  
  const { ruleFrom } = toRefs(state); // 如果您不需要操作表单，可以删除这一行
  </script>
  
  <style scoped>
  .login {
    background: url("../../assets/logo2.png");
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