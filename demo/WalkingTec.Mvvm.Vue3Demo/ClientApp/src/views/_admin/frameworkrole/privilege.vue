<template>
	<div class="card-fill layout-padding">
		<el-card shadow="hover" class="layout-padding-auto">
			<el-form :model="state.vmModel" label-width="100px">
				<el-row>
					<el-col :xs="24" :lg="12" class="mb20">
						<el-form-item ref="Entity_RoleCode_FormItem" prop="Entity.RoleCode"
							:label="$t('message._admin.role.vm.RoleCode')">
							<el-input v-model="state.vmModel.Entity.RoleCode" clearable disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :lg="12" class="mb20">
						<el-form-item ref="Entity_RoleName_FormItem" prop="Entity.RoleName"
							:label="$t('message._admin.role.vm.RoleName')">
							<el-input v-model="state.vmModel.Entity.RoleName" clearable disabled></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<WtmTable ref="tableRef" v-bind="tableData" v-model="state.vmModel.Pages">
					<template #customColumn>
						<el-table-column :label="$t('message._system.common.vm.operate')" width="550">
							<template v-slot="scope">

								<el-row style="width:100%">
									<el-checkbox v-if="scope.row.AllActions && scope.row.AllActions.length > 0"
										v-model="scope.row.checkAll" :indeterminate="scope.row.isIndeterminate"
										@change="handleCheckAllChange($event, scope.row)">{{
											$t('message._system.common.vm.all') }}</el-checkbox>
								</el-row>
								<el-checkbox-group v-model="scope.row.Actions"
									@change="handleCheckedChange($event, scope.row)">
									<el-checkbox v-for="item in scope.row.AllActions" :label="item.Value">{{
										item.Text
									}}</el-checkbox>
								</el-checkbox-group>
							</template>
						</el-table-column>
					</template>
				</WtmTable>
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


<script setup lang="ts" name="message._system.common.vm.action_privilege;false">
import { ElMessage } from 'element-plus';
import { reactive, ref, getCurrentInstance, onMounted } from 'vue';
import frameworkroleApi from '/@/api/frameworkrole';
import other from '/@/utils/other';
import { useRouter } from "vue-router";

const ci = getCurrentInstance() as any;
// 定义子组件向父组件传值/事件
const emit = defineEmits(['refresh', 'closeDialog']);
// 定义变量内容
const formRef = ref();
const tableRef = ref();
const tableData = ref({
	// 列表数据（必传）
	data: [],
	// 表头内容（必传，注意格式）
	header: [
		{ key: 'Name', colWidth: '', title: ci.proxy.$t('message._admin.menu.vm.PageName'), type: 'text', isCheck: true },
	],
	// 配置项（必传）
	config: {
		total: 0, // 列表总数
		loading: true, // loading 加载
		isSerialNo: false, // 是否显示表格序号
		isSelection: false, // 是否显示表格多选
		isOperate: true, // 是否显示表格操作栏
		isDisabled: true,
		isSub: true
	}
});
const state = reactive({
	vmModel: {
		Entity: {
			ID: null,
			RoleCode: null,
			RoleName: null,
			RoleRemark: null
		},
		Pages: []
	}
});

onMounted(() => {
	if (ci.attrs["wtmdata"]) {
		state.vmModel.Entity.ID = ci.attrs["wtmdata"].ID;
		state.vmModel.Entity.RoleCode = ci.attrs["wtmdata"].RoleCode;
		state.vmModel.Entity.RoleName = ci.attrs["wtmdata"].RoleName;
	}
	else if (useRouter().currentRoute.value.query.id) {
		state.vmModel.Entity.ID = useRouter().currentRoute.value.query.id as any;
	}
	frameworkroleApi().getPageActions(state.vmModel.Entity.ID ?? '').then(res => {
		res.Pages.forEach((element: any) => {
			element.checkAll = element.Actions.length == element.AllActions.length;
			element.isIndeterminate = element.Actions.length > 0 && element.Actions.length < element.AllActions.length;
			element['children'] = res.Pages.filter((x: any) => x['ParentID'] == element.ID);
		});
		state.vmModel.Pages = res.Pages.filter((x: any) => !x['ParentID'] || x['ParentID'] == '');;
	})
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
	frameworkroleApi().editPrivilege({Entity:state.vmModel.Entity,Pages:other.flatTree(state.vmModel.Pages)}).then(() => {
		ElMessage.success(ci.proxy.$t('message._system.common.vm.submittip'));
		closeDialog();
	}).catch((error) => {
		other.setFormError(ci, error);
	})
};
const handleCheckAllChange = (val: boolean, row: any) => {
	row.Actions = val ? row.AllActions.map((x: any) => x.Value) : []
	row.isIndeterminate = false
}
const handleCheckedChange = (value: string[], row: any) => {
	const checkedCount = value.length
	row.checkAll = checkedCount === row.AllActions.length
	row.isIndeterminate = checkedCount > 0 && checkedCount < row.AllActions.length
}


// 暴露变量
defineExpose({
});
</script>
