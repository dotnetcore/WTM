<template>
	<div class="card-fill layout-padding">
		<el-card shadow="hover" class="layout-padding-auto">

			<el-form ref="formRef" :model="state.vmModel" label-width="100px">
				<el-row v-if="state.vmModel.DpType == 'User'">
					<el-col :xs="24" :lg="12" class="mb20">
						<el-form-item ref="Entity_UserCode_FormItem" prop="Entity.UserCode"
							:label="$t('message._admin.dp.vm.UserCode')"
							:rules="[{ required: true, message: $t('message._system.common.vm.input', { input: $t('message._admin.dp.vm.UserCode') }), trigger: 'blur' }]">
							<el-input v-model="state.vmModel.Entity.UserCode" clearable></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row v-else>
					<el-col :xs="24" :lg="12" class="mb20">
						<el-form-item ref="Entity_GroupCode_FormItem" prop="Entity.GroupCode"
							:label="$t('message._admin.dp.vm.GroupCode')"
							:rules="[{ required: true, message: $t('message._system.common.vm.input', { input: $t('message._admin.dp.vm.GroupCode') }), trigger: 'blur' }]">
							<el-cascader :options="state.AllGroups"
								:props="{ checkStrictly: true, emitPath: false, label: 'Text', value: 'Value', children: 'Children' }"
								clearable filterable class="w100" v-model="state.vmModel.Entity.GroupCode" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :xs="24" :lg="12" class="mb20">
						<el-form-item ref="Entity_TableName_FormItem" prop="Entity.TableName"
							:label="$t('message._admin.dp.vm.TableName')">
							<el-select v-model="state.vmModel.Entity.TableName" @change="modelChange">
								<el-option v-for="item in state.AllPrivileges" :key="item.Value" :value="item.Value"
									:label="item.Text">{{
										item.Text
									}}</el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :lg="12" class="mb20">
						<el-form-item ref="Entity_IsAll_FormItem" prop="IsAll"
							:label="$t('message._admin.dp.vm.IsAll')">
							<el-select v-model="state.vmModel.IsAll">
								<el-option :value="true" :label="$t('message._system.common.vm.tips_bool_true')" />
								<el-option :value="false" :label="$t('message._system.common.vm.tips_bool_false')" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row v-if="state.vmModel.IsAll == false">
					<el-col :xs="24" :lg="24" class="mb20">
						<el-form-item ref="Entity_SelectedItemsID_FormItem" prop="SelectedItemsID"
							:label="$t('message._admin.dp.vm.SelectedItemsID')">
							<el-select v-model="state.vmModel.SelectedItemsID" multiple>
								<el-option v-for="item in state.AllPrivilegeTableNames" :key="item.Value"
									:value="item.Value" :label="item.Text">{{
										item.Text
									}}</el-option>
							</el-select>
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

<script setup lang="ts" name="message._system.common.vm.add;false">
import { ElMessage } from 'element-plus';
import { reactive, ref, getCurrentInstance, onMounted, nextTick } from 'vue';
import dataPrivilegeApi from '/@/api/dataprivilege';
import other from '/@/utils/other';
import { useRouter } from "vue-router";

const ci = getCurrentInstance() as any;
// 定义子组件向父组件传值/事件
const emit = defineEmits(['refresh', 'closeDialog']);
// 定义变量内容
const formRef = ref();
const state = reactive({
	vmModel: {
		Entity: {
			GroupCode: null,
			UserCode: null,
			TableName: null,
			RelatedId: null,
		},
		IsAll: false,
		SelectedItemsID: [] as any[],
		DpType: 'User',
	},
	AllPrivileges: [] as any[],
	AllGroups: [] as any[],
	AllItems: [] as any[],
	AllPrivilegeTableNames: [] as any[],
});

// 打开弹窗
onMounted(() => {
    other.getSelectList('/api/_account/GetFrameworkGroupsTree', [], true).then(x => { state.AllGroups = x });
	other.getSelectList('/api/_DataPrivilege/GetPrivileges', [], true).then(x => { state.AllPrivileges = x });
	if (ci.attrs["wtmdata"]) {
		state.vmModel.DpType = ci.attrs["wtmdata"].DpType;
	}
	else if (useRouter().currentRoute.value.query.dptype) {
		state.vmModel.DpType = useRouter().currentRoute.value.query.dptype as any;
	}
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
			dataPrivilegeApi().add(state.vmModel).then(() => {
				ElMessage.success(ci.proxy.$t('message._system.common.vm.submittip'));
				emit('refresh');
				closeDialog();
			}).catch((error) => {
				other.setFormError(ci, error);
			})
		}
	})
};

const modelChange = (value: any) => {
	if (value && value !== '') {
		other.getSelectList('/api/_DataPrivilege/GetPrivilegeByTableName?table=' + value, [], false, "get").then(x => {
			state.AllPrivilegeTableNames = x;
		});
	}
	else {
		state.AllPrivilegeTableNames = [];
	}
}
// 暴露变量
defineExpose({
});
</script>
