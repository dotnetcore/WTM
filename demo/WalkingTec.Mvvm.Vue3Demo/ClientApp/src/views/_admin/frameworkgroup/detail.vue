<template>
	<div class="card-fill layout-padding">
		<el-card shadow="hover" class="layout-padding-auto">

			<el-form ref="formRef" :model="state.vmModel" label-width="100px">
				<el-row>
					<el-col :xs="24" :lg="12" class="mb20">
						<el-form-item ref="Entity_GroupCode_FormItem" prop="Entity.GroupCode"
							:label="$t('message._admin.group.vm.GroupCode')">
							<el-input v-model="state.vmModel.Entity.GroupCode" clearable disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :lg="12" class="mb20">
						<el-form-item ref="Entity_GroupName_FormItem" prop="Entity.GroupName"
							:label="$t('message._admin.group.vm.GroupName')">
							<el-input v-model="state.vmModel.Entity.GroupName" clearable disabled></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :xs="24" :lg="12" class="mb20">
						<el-form-item ref="Entity_ParentId_FormItem" prop="Entity.ParentId"
							:label="$t('message._admin.group.vm.ParentId')">
							<el-cascader :options="state.allGroups"
								:props="{ checkStrictly: true, emitPath: false, multiple: true, label: 'Text', value: 'Value', children: 'Children' }"
								clearable disabled filterable class="w100" v-model="state.vmModel.Entity.ParentId" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :lg="12" class="mb20">
						<el-form-item ref="Entity_Manager_FormItem" prop="Entity.Manager"
							:label="$t('message._admin.group.vm.Manager')">
							<el-input v-model="state.vmModel.Entity.Manager" clearable disabled></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :xs="24" :lg="24" class="mb20">
						<el-form-item ref="Entity_GroupRemark_FormItem" prop="Entity.GroupRemark"
							:label="$t('message._admin.group.vm.GroupRemark')">
							<el-input v-model="state.vmModel.Entity.GroupRemark" clearable disabled></el-input>
						</el-form-item>
					</el-col>

				</el-row>

			</el-form>
			<footer class="el-dialog__footer" style="padding: unset;padding-top: 10px;">
				<span class="dialog-footer">
					<el-button @click="onCancel">{{ $t('message._system.common.vm.cancel') }}</el-button>
				</span>
			</footer>
		</el-card>
	</div>
</template>

<script setup lang="ts" name="message._system.common.vm.detail;false">
import { ElMessage } from 'element-plus';
import { reactive, ref, getCurrentInstance, onMounted, nextTick } from 'vue';
import frameworkgroupApi from '/@/api/frameworkgroup';
import other from '/@/utils/other';
import { useRouter } from "vue-router";


// 定义子组件向父组件传值/事件
const emit = defineEmits(['refresh', 'closeDialog']);

// 定义变量内容
const formRef = ref();
const state = reactive({
	vmModel: {
		Entity: {
			ID: null,
			GroupCode: null,
			GroupName: null,
			ParentId: null,
			Manager: null,
			GroupRemark: null
		},
	},
	allGroups: [] as any[],
});
const ci = getCurrentInstance() as any;
// 打开弹窗
onMounted(() => {
	if (ci.attrs["wtmdata"]) {
		state.vmModel.Entity.ID = ci.attrs["wtmdata"].ID;
	}
	else if (useRouter().currentRoute.value.query.id) {
		state.vmModel.Entity.ID = useRouter().currentRoute.value.query.id as any;
	}
	frameworkgroupApi().get(state.vmModel.Entity.ID ?? "").then((data: any) => other.setValue(state.vmModel, data));
	other.getSelectList('/api/_FrameworkGroup/GetParentsTree', [], false).then(x => { state.allGroups = x });
});
// 关闭弹窗
const closeDialog = () => {
	emit('closeDialog');
};
// 取消
const onCancel = () => {
	closeDialog();
};

// 暴露变量
defineExpose({

});
</script>
