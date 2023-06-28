<template>
	<div class="card-fill layout-padding">
		<el-card shadow="hover" class="layout-padding-auto" >
			<WtmSearcher v-model="searchData" @search="getTableData()">
				<el-row>
					<el-col :xs="24" :lg="12" class="mb20">
						<el-form-item ref="GroupCode_FormItem" prop="GroupCode" :label="$t('message._admin.group.vm.GroupCode')">
							<el-input v-model="searchData.GroupCode" clearable></el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :lg="12" class="mb20">
						<el-form-item ref="GroupName_FormItem" prop="GroupName" :label="$t('message._admin.group.vm.GroupName')">
							<el-input v-model="searchData.GroupName" clearable></el-input>
						</el-form-item>
					</el-col>
				</el-row>
			</WtmSearcher>
			<div style="text-align: right;">
				<el-button type="primary" class="ml10" @click="onCreate()" v-auth="'/api/_FrameworkGroup/Add'">
					<i class="fa fa-plus"></i>
					{{ $t('message._system.common.vm.add') }}
				</el-button>
				<el-button type="danger" class="ml10" @click="onBatchDelete()" v-auth="'/api/_FrameworkGroup/BatchDelete'">
					<i class="fa fa-trash"></i>
					{{ $t('message._system.common.vm.delete') }}
				</el-button>
				<el-button  type="primary" class="ml10" @click="onImport()" v-auth="'/api/_FrameworkGroup/Import'">
					<i class="fa fa-upload"></i>
					{{ $t('message._system.common.vm.import') }}
				</el-button>
				<el-button  type="primary" class="ml10" @click="onExport()" v-auth="'/api/_FrameworkGroup/ExportExcel'">
					<i class="fa fa-download"></i>
					{{ $t('message._system.common.vm.export') }}
				</el-button>
			</div>
			<WtmTable
				ref="tableRef"
				v-bind="tableData"
			>
			<template #operation>
				<el-table-column :label="$t('message._system.common.vm.operate')" width="150">
				<template v-slot="scope">
					<el-button text type="primary" @click="onEdit(scope.row)" v-auth="'/api/_FrameworkGroup/Edit'">{{ $t('message._system.common.vm.edit') }}</el-button>
					<el-button text type="primary" @click="onDetail(scope.row)" v-auth="'/api/_FrameworkGroup/{id}'">{{ $t('message._system.common.vm.detail') }}</el-button>
					<el-popconfirm :title="$t('message._system.common.vm.deletetip')"  @confirm="onDelete(scope.row)">
						<template #reference>
							<el-button text type="danger" v-auth="'/api/_FrameworkGroup/BatchDelete'">{{ $t('message._system.common.vm.delete') }}</el-button>
						</template>
					</el-popconfirm>
				</template>
			</el-table-column>
			</template>
		</WtmTable>			
		</el-card>

	</div>
</template>

<script setup lang="ts" name="message._system.menukey.GroupManagement;true;WalkingTec.Mvvm.Admin.Api;FrameworkGroup">
import { defineAsyncComponent, reactive, onMounted, ref, getCurrentInstance } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import frameworkgroupApi from '/@/api/frameworkgroup';
import { useThemeConfig } from '/@/stores/themeConfig';
import other from '/@/utils/other';

const ci = getCurrentInstance() as any;
// 引入组件
const createDialog = defineAsyncComponent(() => import('./create.vue'));
const editDialog = defineAsyncComponent(() => import('./edit.vue'));
const importDialog = defineAsyncComponent(() => import('./import.vue'));
const detailDialog = defineAsyncComponent(() => import('./detail.vue'));
const storesThemeConfig = useThemeConfig();
// 定义变量内容
const tableRef = ref();
const tableData = ref({
	// 列表数据（必传）
	data: [],
	// 表头内容（必传，注意格式）
	header: [
		{ key: 'GroupCode', colWidth: '', title: ci.proxy.$t('message._admin.group.vm.GroupCode'), type: 'text', isCheck: true },
		{ key: 'GroupName', colWidth: '', title: ci.proxy.$t('message._admin.group.vm.GroupName'), type: 'text', isCheck: true },
		{ key: 'ManagerName', colWidth: '', title: ci.proxy.$t('message._admin.group.vm.Manager'), type: 'text', isCheck: true },
		{ key: 'GroupRemark', colWidth: '', title: ci.proxy.$t('message._admin.group.vm.GroupRemark'), type: 'text', isCheck: true }
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
	GroupCode: null,
	GroupName: null
});


// 初始化表格数据
const getTableData = () => {
	tableRef.value.doSearch(frameworkgroupApi().search, searchData.value,true)
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
// 删除
const onDelete = (row: any) => {
	frameworkgroupApi().delete([row.ID]).then(() => { getTableData() })
};
const onBatchDelete = () => {
	const selected = tableRef.value.getSelectedRows();
	frameworkgroupApi().delete(selected.map((x: any) => x.ID)).then(() => { getTableData() })
};
const onExport = () => {
	const selected = tableRef.value.getSelectedRows();
	if (selected.length > 0) {
		frameworkgroupApi().exportById(selected.map((x: any) => x.ID));
	}
	else {
		frameworkgroupApi().export({});
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
