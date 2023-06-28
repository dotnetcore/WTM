<template>
	<div class="card-fill layout-padding">
		<el-card shadow="hover" class="layout-padding-auto" >
			<WtmSearcher v-model="searchData" @search="getTableData()">
				<el-row>
					<el-col :xs="24" :lg="12" class="mb20">
						<el-form-item ref="RoleCode_FormItem" prop="RoleCode" :label="$t('message._admin.role.vm.RoleCode')">
							<el-input v-model="searchData.RoleCode" clearable></el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :lg="12" class="mb20">
						<el-form-item ref="RoleName_FormItem" prop="RoleName" :label="$t('message._admin.role.vm.RoleName')">
							<el-input v-model="searchData.RoleName" clearable></el-input>
						</el-form-item>
					</el-col>
				</el-row>
			</WtmSearcher>
			<div style="text-align: right;">
				<el-button type="primary" class="ml10" @click="onCreate()" v-auth="'/api/_FrameworkRole/Add'">
					<i class="fa fa-plus"></i>
					{{ $t('message._system.common.vm.add') }}
				</el-button>
				<el-button type="danger" class="ml10" @click="onBatchDelete()" v-auth="'/api/_FrameworkRole/BatchDelete'">
					<i class="fa fa-trash"></i>
					{{ $t('message._system.common.vm.delete') }}
				</el-button>
				<el-button  type="primary" class="ml10" @click="onImport()" v-auth="'/api/_FrameworkRole/Import'">
					<i class="fa fa-upload"></i>
					{{ $t('message._system.common.vm.import') }}
				</el-button>
				<el-button  type="primary" class="ml10" @click="onExport()" v-auth="'/api/_FrameworkRole/ExportExcel'">
					<i class="fa fa-download"></i>
					{{ $t('message._system.common.vm.export') }}
					</el-button>					
			</div>
			<WtmTable
				ref="tableRef"
				v-bind="tableData"
			>
			<template #operation>
				<el-table-column :label="$t('message._system.common.vm.operate')" width="250">
				<template v-slot="scope">
					<el-button text type="primary" @click="onEdit(scope.row)" v-auth="'/api/_FrameworkRole/Edit'">{{ $t('message._system.common.vm.edit') }}</el-button>
					<el-button text type="primary" @click="onDetail(scope.row)" v-auth="'/api/_FrameworkRole/{id}'">{{ $t('message._system.common.vm.detail') }}</el-button>
					<el-popconfirm :title="$t('message._system.common.vm.deletetip')"  @confirm="onDelete(scope.row)">
						<template #reference>
							<el-button text type="danger" v-auth="'/api/_FrameworkRole/BatchDelete'">{{ $t('message._system.common.vm.delete') }}</el-button>
						</template>
					</el-popconfirm>
					<el-button text type="primary" @click="onPrivilege(scope.row)">{{ $t('message._system.common.vm.action_privilege') }}</el-button>
				</template>
			</el-table-column>
			</template>
		</WtmTable>			
		</el-card>
	</div>
</template>

<script setup lang="ts" name="message._system.menukey.RoleManagement;true;WalkingTec.Mvvm.Admin.Api;FrameworkRole">
import { defineAsyncComponent, reactive, onMounted, ref, getCurrentInstance } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import frameworkRoleApi from '/@/api/frameworkrole';
import { useThemeConfig } from '/@/stores/themeConfig';
import other from '/@/utils/other';

const ci = getCurrentInstance() as any;

// 引入组件
const createDialog = defineAsyncComponent(() => import('./create.vue'));
const editDialog = defineAsyncComponent(() => import('./edit.vue'));
const importDialog = defineAsyncComponent(() => import('./import.vue'));
const detailDialog = defineAsyncComponent(() => import('./detail.vue'));
const privilegeDialog = defineAsyncComponent(() => import('./privilege.vue'));
// 定义变量内容
const createDialogRef = ref();
const editDialogRef = ref();
const importDialogRef = ref();
const detailDialogRef = ref();
const privilegeDialogRef = ref();
const tableRef = ref();
const tableData = ref({
	// 列表数据（必传）
	data: [],
	// 表头内容（必传，注意格式）
	header: [
		{ key: 'RoleCode', colWidth: '', title: ci.proxy.$t('message._admin.role.vm.RoleCode'),width:150, type: 'text', isCheck: true },
		{ key: 'RoleName', colWidth: '', title: ci.proxy.$t('message._admin.role.vm.RoleName'), type: 'text', isCheck: true },
		{ key: 'RoleRemark', colWidth: '', title: ci.proxy.$t('message._admin.role.vm.RoleRemark'), type: 'text', isCheck: true }
	],
	// 配置项（必传）
	config: {
		total: 0, // 列表总数
		loading: true, // loading 加载
		isSerialNo: true, // 是否显示表格序号
		isSelection: true, // 是否显示表格多选
		isOperate: true, // 是否显示表格操作栏
	}
});
const searchData = ref({
	RoleCode: null,
	RoleName: null
});

// 初始化表格数据
const getTableData = () => {
	tableRef.value.doSearch(frameworkRoleApi().search, searchData.value)
		.catch((error: any) => {
			other.setFormError(ci, error);
		});
};
// 打开新增弹窗
const onCreate = () => {
	other.openDialog(ci.proxy.$t('message._system.common.vm.add'),createDialog,null,getTableData)
};
// 打开修改弹窗
const onEdit = (row: any) => {
	other.openDialog(ci.proxy.$t('message._system.common.vm.edit'),editDialog,row,getTableData)
};
const onDetail = (row: any) => {
	other.openDialog(ci.proxy.$t('message._system.common.vm.detail'),detailDialog,row,getTableData)
};


const onPrivilege = (row: any) => {
	other.openDialog(ci.proxy.$t('message._system.common.vm.action_privilege'),privilegeDialog,row,getTableData)
};

// 删除
const onDelete = (row: any) => {
	frameworkRoleApi().delete([row.ID]).then(() => { getTableData() })
};
const onBatchDelete = () => {
	const selected = tableRef.value.getSelectedRows();
	frameworkRoleApi().delete(selected.map((x: any) => x.ID)).then(() => { getTableData() })
};
const onExport = () => {
	const selected = tableRef.value.getSelectedRows();
	if (selected.length > 0) {
		frameworkRoleApi().exportById(selected.map((x: any) => x.ID));
	}
	else {
		frameworkRoleApi().export({});
	}
};
const onImport = () => {
	other.openDialog(ci.proxy.$t('message._system.common.vm.import'),importDialog,null,getTableData)
};
// 页面加载时
onMounted(() => {
	getTableData();
});
</script>

<style scoped lang="scss">
</style>
