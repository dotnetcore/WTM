<template>
	<div class="card-fill layout-padding">
		<el-card shadow="hover" class="layout-padding-auto">
			<el-form ref="formRef" :model="state.vmModel" label-width="100px">
				<el-row>
					<el-col :xs="24" :lg="24" class="mb20">
						<el-form-item ref="OldPassword_FormItem" prop="OldPassword"
							:label="$t('message._admin.account.oldpassword')"
							:rules="[{ required: true, message: $t('message._system.common.vm.input',{input:$t('message._admin.account.oldpassword')}), trigger: 'blur' }]">
							<el-input v-model="state.vmModel.OldPassword" autocomplete="off" show-password clearable ></el-input>
						</el-form-item>
						<el-form-item ref="NewPassword_FormItem" prop="NewPassword"
							:label="$t('message._admin.account.newpassword')"
							:rules="[{ required: true, message: $t('message._system.common.vm.input',{input:$t('message._admin.account.newpassword')}), trigger: 'blur' }]">
							<el-input v-model="state.vmModel.NewPassword" autocomplete="off" show-password clearable ></el-input>
						</el-form-item>
						<el-form-item ref="NewPasswordComfirm_FormItem" prop="NewPasswordComfirm"
							:label="$t('message._admin.account.newpassword')"
							:rules="[{ required: true, message: $t('message._system.common.vm.input',{input:$t('message._admin.account.newpassword')}), trigger: 'blur' }]">
							<el-input :placeholder="$t('message._admin.account.newpassword2')" v-model="state.vmModel.NewPasswordComfirm" autocomplete="off" show-password clearable ></el-input>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<footer class="el-dialog__footer" style="padding: unset;padding-top: 10px;">
				<span class="dialog-footer">
					<el-button type="primary" @click="onSubmit">{{ $t('message._system.common.vm.submit') }}</el-button>
					<el-button @click="onCancel">{{ $t('message._system.common.vm.cancel') }}</el-button>
				</span>
			</footer>
		</el-card>
	</div>
</template>

<script setup lang="ts" name="message._system.common.vm.edit,false">
import { ElMessage } from 'element-plus';
import { reactive, ref, getCurrentInstance, onMounted, nextTick } from 'vue';
import loginApi from '/@/api/login';
import other from '/@/utils/other';

const emit = defineEmits(['refresh', 'closeDialog']);

// 定义变量内容
const formRef = ref();
const state = reactive({
	vmModel: {
			OldPassword: null,
			NewPassword: null,
			NewPasswordComfirm: null
	}
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
// 提交
const onSubmit = () => {

	formRef.value?.validate((valid: boolean, fields: any) => {
		if (valid) {
			loginApi().changePassword(state.vmModel).then(() => {
				ElMessage.success(ci.proxy.$t('message._system.common.vm.submittip'));
				closeDialog();
			}).catch((error) => {
				other.setFormError(ci, error);
			})
		}
	})
};


// 暴露变量
defineExpose({
});
</script>
