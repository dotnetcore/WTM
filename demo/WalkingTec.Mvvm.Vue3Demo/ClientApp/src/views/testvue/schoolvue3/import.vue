
<template>
<div class="card-fill layout-padding">
<el-card shadow="hover" class="layout-padding-auto" >

        <el-form ref="formRef" :model="state.vmModel"  label-width="90px" >
			<WtmImportor  url="/api/SchoolVue3/GetExcelTemplate" v-model="state.vmModel.UploadFileId" :files="state.vmModel.files"/>
			<div v-if="state.errorFileId" style="margin-top: 10px;vertical-align: bottom;">
				<el-button @click="onErrorFile" size="small" type="danger">{{ $t('message._system.common.vm.importtip') }}</el-button>
			</div>
		</el-form>
		<footer class="el-dialog__footer" style="padding: unset;padding-top: 10px;">
			<span class="dialog-footer">
				<el-button @click="onCancel" >{{$t('message._system.common.vm.cancel')}}</el-button>
				<el-button :disabled="state.vmModel.UploadFileId===null" type="primary" @click="onSubmit" >{{$t('message._system.common.vm.submit')}}</el-button>
			</span>
		</footer>
</el-card>
</div>
</template>


<script setup lang="ts" name="message._system.common.vm.import;false">
import {  ElMessageBox, ElMessage } from 'element-plus';
import { defineAsyncComponent,reactive, ref, getCurrentInstance, onMounted, nextTick } from 'vue';
import { SchoolVue3Api } from '/@/api/testvue/SchoolVue3';
import other from '/@/utils/other';
import fileApi from '/@/api/file';
import { useRouter } from "vue-router";
const ci = getCurrentInstance() as any;


// 定义变量内容
const formRef = ref();
const state = reactive({
	vmModel: {
		UploadFileId: null,
		files: []
	},
	errorFileId: null
});
// 取消
const onCancel = () => {
	closeDialog();
};

const onSubmit = () => {
	SchoolVue3Api().import(state.vmModel)
		.then((res) => {
            ElMessage.success(ci.proxy.$t('message._system.common.vm.importsuc',{count:res}));
			emit('refresh');
			closeDialog();
		})
		.catch((error) => {
			state.errorFileId = error.FormError['Entity.ErrorFileId']
			ElMessage.error(error.FormError['Entity.Import']);
		});
}

const onErrorFile = () => {
	fileApi().downloadFile(state.errorFileId);
}

// 页面加载时
onMounted(() => {
    
});

// 定义子组件向父组件传值/事件
const emit = defineEmits(['refresh','closeDialog']);
// 关闭弹窗
const closeDialog = () => {
    emit('closeDialog');
};
// 暴露变量
defineExpose({

});
</script>

<style scoped lang="scss">

</style>

