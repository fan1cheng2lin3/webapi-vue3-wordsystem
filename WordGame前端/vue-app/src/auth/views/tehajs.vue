
<template>
  <div class="auth-container">
    <div class="auth-form">
      <h1>{{ currentFormTitle }}</h1>
      <el-form :model="form" ref="authForm" class="auth-form" :rules="rules">
        <template v-if="currentForm === 'login'">
          <el-form-item label="账号" prop="email">
            <el-input v-model="form.email" />
          </el-form-item>
          <el-form-item label="密码" prop="password">
            <el-input type="password" v-model="form.password" />
          </el-form-item>
        </template>
        <template v-if="currentForm === 'register'">
          <el-form-item label="账号" prop="email">
            <el-input v-model="form.email" />
          </el-form-item>
          <el-form-item label="密码" prop="password">
            <el-input type="password" v-model="form.password" />
          </el-form-item>
          <el-form-item label="确认密码" prop="confirmPassword">
            <el-input type="password" v-model="form.confirmPassword" />
          </el-form-item>
        </template>
        <template v-if="currentForm === 'forgotPassword'">
          <el-form-item label="账号" prop="email">
            <el-input v-model="form.email" />
          </el-form-item>
        </template>
      </el-form>
      <el-button size="large" type="primary" @click="submitForm">{{ submitButtonText }}</el-button>
      <div class="form-switch">
        <el-button @click="switchForm('login')">登录</el-button>
        <el-button @click="switchForm('register')">注册</el-button>
        <el-button @click="switchForm('forgotPassword')">找回密码</el-button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';

const form = ref({
  email: '',
  password: '',
  confirmPassword: ''
});

const currentForm = ref('login');

const rules = ref({
  email: [
    { required: true, message: '请输入账号', trigger: 'blur' },
    { type: 'email', message: '请输入正确的邮箱地址', trigger: 'blur,change' }
  ],
  password: [
    { required: true, message: '请输入密码', trigger: 'blur' },
    { min: 6, message: '密码长度不能小于6位', trigger: 'blur' }
  ],
  confirmPassword: [
    { required: true, message: '请确认密码', trigger: 'blur' },
    { validator: (rule, value, callback) => {
      if (value !== form.value.password) {
        callback(new Error('两次输入密码不一致'));
      } else {
        callback();
      }
    }, trigger: 'blur' }
  ]
});

const currentFormTitle = computed(() => {
  const titles = {
    login: '用户登录',
    register: '用户注册',
    forgotPassword: '找回密码'
  };
  return titles[currentForm.value] || '未知';
});

const submitButtonText = computed(() => {
  const texts = {
    login: '登录',
    register: '注册',
    forgotPassword: '提交'
  };
  return texts[currentForm.value] || '提交';
});

const switchForm = (formType) => {
  currentForm.value = formType;
};

const submitForm = () => {
  const authForm = ref(null);
  authForm.value.validate((valid) => {
    if (valid) {
      alert('表单验证成功');
      // 这里可以添加提交表单的逻辑
    } else {
      alert('表单验证失败');
      return false;
    }
  });
};
</script>

<style>
.auth-container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
}
.auth-form {
  width: 300px;
}
.form-switch {
  margin-top: 20px;
  text-align: center;
}
</style> 

