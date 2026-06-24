<template>
	<div>
		<el-dialog title="导入" v-model="state.dialog.isShowDialog" draggable>
			<el-form ref="formRef" :model="state.vmModel"  label-width="90px" >
				<WtmImportor url="/api/_FrameworkUser/GetExcelTemplate" v-model="state.vmModel.UploadFileId" :files="state.vmModel.files"/>
				<div v-if="state.errorFileId" style="margin-top: 10px;vertical-align: bottom;">
					<el-button @click="onErrorFile" size="small" type="danger">导入数据错误，点击下载错误文件</el-button>
				</div>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="onCancel" >关闭</el-button>
					<el-button :disabled="state.vmModel.UploadFileId===null" type="primary" @click="onSubmit" >提交</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts" name="message._system.common.vm.import;false">
import { ElMessage } from 'element-plus';
import { reactive, ref, getCurrentInstance } from 'vue';
import frameworkuserApi from '/@/api/frameworkuser';
import other from '/@/utils/other';
import fileApi from '/@/api/file';

// 定义子组件向父组件传值/事件
const emit = defineEmits(['refresh']);

// 定义变量内容
const formRef = ref();
const state = reactive({
	vmModel: {
		UploadFileId:null,
		files:[]
	},
	dialog: {
		isShowDialog: false,
	},	
	errorFileId:null
});
const ci = getCurrentInstance() as any;
// 打开弹窗
const openDialog = () => {
	other.clearObj(state.vmModel);
	other.clearFormError(ci);
	state.errorFileId = null;
	state.dialog.isShowDialog = true;
};
// 关闭弹窗
const closeDialog = () => {
	state.dialog.isShowDialog = false;
};
// 取消
const onCancel = () => {
	closeDialog();
};

const onSubmit = ()=>{
	frameworkuserApi().import(state.vmModel)
	.then((res)=>{
		ElMessage.success("导入成功"+res+"条数据");
			closeDialog();
			emit('refresh');
	})
	.catch((error)=>{
		state.errorFileId = error.FormError['Entity.ErrorFileId']
		ElMessage.error(error.FormError['Entity.Import']);
	});
}

const onErrorFile = ()=>{
	fileApi().downloadFile(state.errorFileId);
}
// 暴露变量
defineExpose({
	openDialog,
});
</script>
