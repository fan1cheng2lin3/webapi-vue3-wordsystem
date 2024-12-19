<script setup>
import { reactive, ref, computed } from 'vue';
import { useStore } from 'vuex';
import { ElMessageBox } from 'element-plus';
import axios from 'axios';

const store = useStore();
const activeTab = ref('login');
const forms = reactive({
  login: { email: '', password: '' },
  register: { email: '', password: '', confirmPassword: '' },
  forgotPassword: { email: '' }
});

const rules = reactive({
  login: {
    email: [{ required: true, message: '请输入账号！', trigger: 'blur' }],
    password: [{ required: true, message: '请输入密码！', trigger: 'blur' }]
  },
  register: {
    email: [{ required: true, message: '请输入账号！', trigger: 'blur' }],
    password: [{ required: true, message: '请输入密码！', trigger: 'blur' }],
    confirmPassword: [
      { required: true, message: '请确认密码！', trigger: 'blur' },
      {
        validator: (rule, value, callback) => {
          if (value !== forms.register.password) {
            callback(new Error('两次输入密码不一致'));
          } else {
            callback();
          }
        },
        trigger: 'blur'
      }
    ]
  },
  forgotPassword: {
    email: [{ required: true, message: '请输入账号！', trigger: 'blur' }]
  }
});

const loginFormRef = ref(null);
const registerFormRef = ref(null);
const forgotPasswordFormRef = ref(null);

const currentAction = computed(() => {
  const actions = {
    login: '用户登录',
    register: '用户注册',
    forgotPassword: '找回密码'
  };
  return actions[activeTab.value] || '用户登录';
});

const submitForm = async () => {
  const formRefMap = {
    login: loginFormRef,
    register: registerFormRef,
    forgotPassword: forgotPasswordFormRef
  };

  const formRef = formRefMap[activeTab.value];
  if (!formRef.value) return;

  try {
    // Validate form
    await formRef.value.validate();

    if (activeTab.value === 'login') {
      try {
        const response = await store.dispatch('authModule/userLoginAction', forms.login);

        // 检查接口是否返回错误信息
        if (response?.error) {
          throw new Error(response.error.message);
        }

        console.log('登录成功');
      } catch (loginError) {
        if (loginError.response?.status === 400) {
          ElMessageBox.alert('账号或密码错误，请检查后重试！', '登录失败', { type: 'error' });
        } else {
          ElMessageBox.alert(loginError.message || '登录失败，请重试！', '登录失败', { type: 'error' });
        }
      }
    } else if (activeTab.value === 'register') {
      try {
        await axios.post('/Users/register', {
          email: forms.register.email,
          password: forms.register.password
        });
        console.log('注册成功');
      } catch (registerError) {
        ElMessageBox.alert(registerError.response?.data?.message || '注册失败，请重试', '注册失败', { type: 'error' });
      }
    } else if (activeTab.value === 'forgotPassword') {
      try {
        await axios.post('/Users/forgot-password', forms.forgotPassword);
        console.log('找回成功');
      } catch (forgotError) {
        ElMessageBox.alert(forgotError.response?.data?.message || '找回密码失败，请重试', '找回失败', { type: 'error' });
      }
    }
  } catch (formError) {
    // 捕获表单验证错误
    ElMessageBox.alert('请填写完整且正确的信息！', '验证失败', { type: 'warning' });
  }
};

</script>

<template>
  <div class="login">
    <div class="container">
      <h1>{{ currentAction }}</h1>
      <el-tabs v-model="activeTab" class="tabs">
        <el-tab-pane label="登录" name="login">
          <el-form
            :model="forms.login"
            :rules="rules.login"
            ref="loginFormRef"
            class="og-form"
          >
            <el-form-item label="账号" prop="email">
              <el-input v-model="forms.login.email" placeholder="请输入账号" size="large" />
            </el-form-item>
            <el-form-item label="密码" prop="password">
              <el-input type="password" v-model="forms.login.password" placeholder="请输入密码" size="large" />
            </el-form-item>
          </el-form>
          <el-button size="large" type="primary" @click="submitForm">登录</el-button>
        </el-tab-pane>

        <el-tab-pane label="注册" name="register">
          <el-form
            :model="forms.register"
            :rules="rules.register"
            ref="registerFormRef"
            class="og-form"
          >
            <el-form-item label="账号" prop="email">
              <el-input v-model="forms.register.email" placeholder="请输入账号" />
            </el-form-item>
            <el-form-item label="密码" prop="password">
              <el-input type="password" v-model="forms.register.password" placeholder="请输入密码" />
            </el-form-item>
            <el-form-item label="确认密码" prop="confirmPassword">
              <el-input type="password" v-model="forms.register.confirmPassword" placeholder="请确认密码" />
            </el-form-item>
          </el-form>
          <el-button size="large" type="primary" @click="submitForm">注册</el-button>
        </el-tab-pane>

        <el-tab-pane label="找回密码" name="forgotPassword">
          <el-form
            :model="forms.forgotPassword"
            :rules="rules.forgotPassword"
            ref="forgotPasswordFormRef"
            class="og-form"
          >
            <el-form-item label="账号" prop="email">
              <el-input v-model="forms.forgotPassword.email" placeholder="请输入账号" />
            </el-form-item>
          </el-form>
          <el-button size="large" type="primary" @click="submitForm">找回密码</el-button>
        </el-tab-pane>
      </el-tabs>
    </div>
  </div>
</template>

<style scoped>
.login {
  background: url("https://pic.616pic.com/bg_w1180/00/23/35/4uvodsvg7Q.jpg") no-repeat center center / cover;
  height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
}
.container {
  width: 100%;
  max-width: 600px;
  padding: 20px;
  background: rgba(255, 255, 255, 0.9);
  border-radius: 10px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
}
h1 {
  text-align: center;
  color: #333;
}
</style>
