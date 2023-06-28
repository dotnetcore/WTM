<template>
	<div class="card-fill layout-padding">
		<el-card shadow="hover" class="layout-padding-auto">
			<el-form ref="formRef" :model="state.vmModel" label-width="90px">
				<WtmImportor url="/api/_frameworkgroup/GetExcelTemplate" v-model="state.vmModel.UploadFileId"
					:files="state.vmModel.files" />
				<div v-if="state.errorFileId" style="margin-top: 10px;vertical-align: bottom;">
					<el-button @click="onErrorFile" size="small" type="danger">{{ $t('message._system.common.vm.importtip')
					}}</el-button>
				</div>
			</el-form>
			<footer class="el-dialog__footer" style="padding: unset;padding-top: 10px;">
				<span class="dialog-footer">
					<el-button @click="onCancel">{{ $t('message._system.common.vm.cancel') }}</el-button>
					<el-button :disabled="state.vmModel.UploadFileId === null" type="primary" @click="onSubmit">{{
						$t('message._system.common.vm.submit') }}</el-button>
				</span>
			</footer>
		</el-card>
	</div>
</template>

<script setup lang="ts" name="message._system.common.vm.import;false">
import { ElMessage } from 'element-plus';
import { reactive, ref, getCurrentInstance, onMounted, nextTick } from 'vue';
import frameworkgroupApi from '/@/api/frameworkgroup';
import other from '/@/utils/other';
import fileApi from '/@/api/file';


// 定义子组件向父组件传值/事件
const emit = defineEmits(['refresh', 'closeDialog']);

// 定义变量内容
const formRef = ref();
const state = reactive({
	vmModel: {
		UploadFileId: null,
		files: []
	},
	errorFileId: null
});
const ci = getCurrentInstance() as any;
// 打开弹窗
onMounted(() => {

});
// 关闭弹窗
const closeDialog = () => {
	emit('closeDialog');
};
// 取消
const onCancel = () => {
	closeDialog();
};

const onSubmit = () => {
	frameworkgroupApi().import(state.vmModel)
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
// 暴露变量
defineExpose({

});
</script>
