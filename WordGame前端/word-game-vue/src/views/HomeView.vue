<template>
   <div class="container" id="app">
      <div class="forms-container">
        <div class="signin-signup">
          <el-form
            ref="loginRef"
            class="sign-in-form"
          >
            <h2 class="title">登录</h2>
            <div class="input-field">
              <i class="fa-solid fa-user"></i>
              <el-form-item prop="name">
                <el-input
                  placeholder="账号/手机号"
                />
              </el-form-item>
            </div>
            <div class="input-field">
              <i class="fa-solid fa-lock"></i>
              <el-form-item prop="password">
                <el-input
                  type="password"
                  placeholder="密码"
                  autocomplete="off"
                  show-password
                />
              </el-form-item>
            </div>
            <el-button
              type="primary"
              class="btn form"
              round
            >
            </el-button>
          </el-form>
          <el-form
            ref="signUpRef"
            class="sign-up-form"
          >
            <h2 class="title">注册</h2>
            <div class="input-field">
              <i class="fa-solid fa-user"></i>
              <el-form-item prop="name">
              </el-form-item>
            </div>
            <div class="input-field">
              <i class="fa-solid fa-phone"></i>
              <el-form-item prop="phone">
              </el-form-item>
            </div>
            <div class="input-field">
              <i class="fa-solid fa-lock"></i>
              <el-form-item prop="password">
                <el-input
                  type="password"
                  placeholder="密码"
                  autocomplete="off"
                  show-password
                />
              </el-form-item>
            </div>
            <div class="input-field">
              <i class="fa-solid fa-lock"></i>
              <el-form-item prop="confirmPassword">
                <el-input
                  type="password"
                  placeholder="确认密码"
                  autocomplete="off"
                  show-password
                />
              </el-form-item>
            </div>
            <el-button
              type="primary"
              class="btn form"
              round
            >
            </el-button>
          </el-form>
        </div>
      </div>

      <div class="panels-container">
        <div class="panel left-panel">
          <div class="content">
            <h3>新用户 ?</h3>
            <p>输入您的信息成为我们的客户</p>
            <button class="btn transparent" id="sign-up-btn">注 册</button>
          </div>
          <img src="img/log.svg" class="image" alt="" />
        </div>
        <div class="panel right-panel">
          <div class="content">
            <h3>已有账号 ?</h3>
            <p>请登录以享受我们更多的服务</p>
            <button class="btn transparent" id="sign-in-btn">登 录</button>
          </div>
          <img src="img/register.svg" class="image" alt="" />
        </div>
      </div>
    </div>

    
</template>



<style lang="scss">
.el-row {
  margin-bottom: 20px;
}
.el-row:last-child {
  margin-bottom: 0;
}
.el-col {
  border-radius: 4px;
}

.grid-content {
  border-radius: 4px;
  min-height: 36px;
}
</style>