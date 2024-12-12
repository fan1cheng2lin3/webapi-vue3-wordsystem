<template>
  <el-dialog v-model="dialogVisible" :title="dialogTitle" width="35%" :before-close="handleClose">
    <el-form :model="ruleFroms" label-width="auto" style="max-width: 600px">
      <el-form-item label="分类名称" prop="name">
        <el-input v-model="ruleFroms.name" />
      </el-form-item>
      <el-form-item label="分类描述" prop="description">
        <el-input v-model="ruleFroms.description" />
      </el-form-item>
    </el-form>

    <template #footer>
      <div class="dialog-footer">
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="addCategory">提交</el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup>
import axios from '@/api/api_config';
import { reactive, toRefs, watch, inject } from 'vue';
import { ElMessage } from 'element-plus';

// 定义 props
const props = defineProps({
  dialogTitle: { type: String },
  tableRow: { type: Object }
});

const realod = inject("getList")

const state = reactive({
  dialogVisible: false,
  ruleFroms: {
    id: "",
    name: "",
    description: "", // 新增描述字段
  },
});

//监听器
watch(
    () => props.tableRow,
    () => {
        state.ruleFroms = { ...props.tableRow }; // 自动填充 name 和 description
    },
    { deep: true, immediate: true }
);

const dialogCategory = () => {
  state.dialogVisible = true;


};


// 提交修改
const addCategory = () => {
    const param = {
        name: ruleFroms.value.name,
        description: ruleFroms.value.description, // 新增字段
    };

    if (props.dialogTitle === "添加分类") {
        axios.post("/Category", param).then(() => {
            ElMessage.success("添加成功");
            state.dialogVisible = false; // 关闭窗口
            realod(); // 刷新列表
        });
    } else {
        axios.put(`/Category/${props.tableRow.id}`, param).then(() => {
            ElMessage.success("修改成功");
            state.dialogVisible = false; // 关闭窗口
            realod(); // 刷新列表
        });
    }
};


//结构
const { dialogVisible, ruleFroms } = toRefs(state);

// 暴露给父组件的方法
defineExpose({ dialogCategory });
</script>