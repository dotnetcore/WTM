<template>
	<div class="card-fill layout-padding">
		<el-card shadow="hover" class="layout-padding-auto">
			<el-form ref="formRef" :model="state.vmModel" label-width="100px">
				<el-row>
					<el-col :xs="24"  :lg="12"  class="mb20">
						<el-form-item ref="Entity_RoleCode_FormItem" prop="Entity.RoleCode"
							:label="$t('message._admin.role.vm.RoleCode')"
							:rules="[{ required: true, message: $t('message._system.common.vm.input',{input:$t('message._admin.role.vm.RoleCode')}), trigger: 'blur' }]">
							<el-input v-model="state.vmModel.Entity.RoleCode" clearable disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24"  :lg="12"  class="mb20">
						<el-form-item ref="Entity_RoleName_FormItem" prop="Entity.RoleName"
							:label="$t('message._admin.role.vm.RoleName')"
							:rules="[{ required: true, message: $t('message._system.common.vm.input',{input:$t('message._admin.role.vm.RoleName')}), trigger: 'blur' }]">
							<el-input v-model="state.vmModel.Entity.RoleName" clearable></el-input>
						</el-form-item>
					</el-col>
				</el-row>
			
				<el-row>
					<el-col :xs="24"  :lg="24"  class="mb20">
						<el-form-item ref="Entity_RoleRemark_FormItem" prop="Entity.RoleRemark"
							:label="$t('message._admin.role.vm.RoleRemark')">
							<el-input v-model="state.vmModel.Entity.RoleRemark" clearable></el-input>
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

<script setup lang="ts" name="message._system.common.vm.edit;false">
import { ElMessage } from 'element-plus';
import { array } from 'snabbdom';
import { reactive, ref, getCurrentInstance,onMounted } from 'vue';
import frameworkroleApi from '/@/api/frameworkrole';
import other from '/@/utils/other';
import { useRouter } from "vue-router";


// 定义子组件向父组件传值/事件
const emit = defineEmits(['refresh', 'closeDialog']);


// 定义变量内容
const formRef = ref();
const state = reactive({
	vmModel: {
		Entity:{
			ID:null,
			RoleCode: null,
			RoleName: null,
			RoleRemark: null
		},
	}
});
const ci = getCurrentInstance() as any;

onMounted(() => {
	if (ci.attrs["wtmdata"]) {
		state.vmModel.Entity.ID = ci.attrs["wtmdata"].ID;
	}
	else if (useRouter().currentRoute.value.query.id) {
		state.vmModel.Entity.ID = useRouter().currentRoute.value.query.id as any;
	}
	frameworkroleApi().get(state.vmModel.Entity.ID ?? "").then((data: any) => other.setValue(state.vmModel, data));
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

  formRef.value?.validate((valid:boolean, fields:any) => {
    if (valid) {
		frameworkroleApi().edit(state.vmModel).then(()=>{
			ElMessage.success(ci.proxy.$t('message._system.common.vm.submittip'));
			emit('refresh');
			closeDialog();
		}).catch((error)=>{
			other.setFormError(ci,error);
		})
    } 
  })
};


// 暴露变量
defineExpose({
});
</script>
