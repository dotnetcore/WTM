<template>
	<div class="card-fill layout-padding">
		<el-card shadow="hover" class="layout-padding-auto">
			<div style="text-align: right;">
				<el-button type="primary" @click="onCreate()" v-auth="'/api/_FrameworkMenu/Add'">
					<i class="fa fa-plus"></i>
					{{ $t('message._system.common.vm.add') }}
				</el-button>
				<el-button type="danger" @click="onBatchDelete()" v-auth="'/api/_FrameworkMenu/BatchDelete'">
					<i class="fa fa-trash"></i>
					{{ $t('message._system.common.vm.delete') }}
				</el-button>
				<el-button type="primary" @click="onExport()" v-auth="'/api/_FrameworkMenu/ExportExcel'">
					<i class="fa fa-download"></i>
					{{ $t('message._system.common.vm.export') }}
				</el-button>
				<el-button type="primary" @click="onRefresh()" v-auth="'/api/_FrameworkMenu/RefreshMenu'">
					<i class="fa fa-refresh"></i>
					{{ $t('message._admin.menu.vm.Refresh') }}
				</el-button>
			</div>
			<WtmTable ref="tableRef" v-bind="tableData">
				<template #operation>
					<el-table-column :label="$t('message._system.common.vm.operate')" width="150">
						<template v-slot="scope">
							<el-button text type="primary" @click="onEdit(scope.row)" v-auth="'/api/_FrameworkMenu/Edit'">{{
								$t('message._system.common.vm.edit') }}</el-button>
							<el-button text type="primary" @click="onDetail(scope.row)"
								v-auth="'/api/_FrameworkMenu/{id}'">{{ $t('message._system.common.vm.detail') }}</el-button>
							<el-popconfirm :title="$t('message._system.common.vm.deletetip')"
								@confirm="onDelete(scope.row)">
								<template #reference>
									<el-button text type="danger" v-auth="'/api/_FrameworkMenu/BatchDelete'">{{
										$t('message._system.common.vm.delete') }}</el-button>
								</template>
							</el-popconfirm>
						</template>
					</el-table-column>
				</template>
			</WtmTable>
		</el-card>
	</div>
</template>

<script setup lang="ts" name="message._system.menukey.MenuMangement;true;WalkingTec.Mvvm.Admin.Api;FrameworkMenu">
import { defineAsyncComponent, reactive, onMounted, ref, getCurrentInstance } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import frameworkmenuApi from '/@/api/frameworkmenu';
import { useThemeConfig } from '/@/stores/themeConfig';
import other from '/@/utils/other';

const ci = getCurrentInstance() as any;
// 引入组件
const createDialog = defineAsyncComponent(() => import('./create.vue'));
const editDialog = defineAsyncComponent(() => import('./edit.vue'));
const detailDialog = defineAsyncComponent(() => import('./detail.vue'));
const storesThemeConfig = useThemeConfig();
// 定义变量内容
const tableRef = ref();
const tableData = ref({
	// 列表数据（必传）
	data: [],
	// 表头内容（必传，注意格式）
	header: [
		{ key: 'PageName', colWidth: '', title: ci.proxy.$t('message._admin.menu.vm.PageName'), type: 'text', isCheck: true },
		{ key: 'ModuleName', colWidth: '', title: ci.proxy.$t('message._admin.menu.vm.ModuleName'), type: 'text', isCheck: true },
		{ key: 'ShowOnMenu', colWidth: '', title: ci.proxy.$t('message._admin.menu.vm.ShowOnMenu'), type: 'switch', isCheck: true },
		{ key: 'FolderOnly', colWidth: '', title: ci.proxy.$t('message._admin.menu.vm.FolderOnly'), type: 'switch', isCheck: true },
		{ key: 'IsPublic', colWidth: '', title: ci.proxy.$t('message._admin.menu.vm.IsPublic'), type: 'switch', isCheck: true },
		{ key: 'TenantAllowed', colWidth: '', title: ci.proxy.$t('message._admin.menu.vm.TenantAllowed'), type: 'switch', isCheck: true },
		{ key: 'DisplayOrder', colWidth: '', title: ci.proxy.$t('message._admin.menu.vm.DisplayOrder'), type: 'text', isCheck: true },
		{ key: 'Icon', colWidth: '', title: ci.proxy.$t('message._admin.menu.vm.Icon'), type: 'icon', isCheck: true },

	],
	// 配置项（必传）
	config: {
		total: 0, // 列表总数
		loading: true, // loading 加载
		isSerialNo: false, // 是否显示表格序号
		isSelection: true, // 是否显示表格多选
		isOperate: true, // 是否显示表格操作栏
	}
});


// 初始化表格数据
const getTableData = () => {
	tableRef.value.doSearch(frameworkmenuApi().search, {}, true)
		.catch((error: any) => {
			other.setFormError(ci, error);
		});
};
// 打开新增弹窗
const onCreate = () => {
	other.openDialog(ci.proxy.$t('message._system.common.vm.add'), createDialog, null, getTableData)
};
// 打开修改弹窗
const onEdit = (row: any) => {
	other.openDialog(ci.proxy.$t('message._system.common.vm.edit'), editDialog, row, getTableData)
};
const onDetail = (row: any) => {
	other.openDialog(ci.proxy.$t('message._system.common.vm.detail'), detailDialog, row, getTableData)
};
// 删除
const onDelete = (row: any) => {
	frameworkmenuApi().delete([row.ID]).then(() => { getTableData() })
};
const onRefresh = () => {
	frameworkmenuApi().refresh().then(() => { ElMessage.success(ci.proxy.$t('message._system.common.vm.submittip')); })
};
const onBatchDelete = () => {
	const selected = tableRef.value.getSelectedRows();
	frameworkmenuApi().delete(selected.map((x: any) => x.ID)).then(() => { getTableData() })
};
const onExport = () => {
	const selected = tableRef.value.getSelectedRows();
	if (selected.length > 0) {
		frameworkmenuApi().exportById(selected.map((x: any) => x.ID));
	}
	else {
		frameworkmenuApi().export({});
	}
};

// 页面加载时
onMounted(() => {
	getTableData();
});
</script>

<style scoped lang="scss"></style>
