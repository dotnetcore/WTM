<template>
	<div class="system-user-container layout-padding">
		<el-card shadow="hover" class="layout-padding-auto">
			<div class="system-user-search mb15">
				<el-input placeholder="请输入用户名称" style="max-width: 180px"> </el-input>
				<el-form-item ref="Entity_Gender_FormItem" prop="Entity.Gender" label="性别" >
									<el-select clearable>
									</el-select>
								</el-form-item>
				<el-button type="primary" class="ml10">
					<i class="fa fa-search"></i>
					查询
				</el-button>
			</div>
			<div style="text-align: right;">
				<el-button type="success" class="ml10" @click="onCreate()" >
					<i class="fa fa-plus"></i>
					新增
				</el-button>
				<el-button type="success" class="ml10" @click="onBatchDelete()" v-auth="'/api/_Account/LoginRemote'">
					<i class="fa fa-trash"></i>
					批量删除
				</el-button>
				<el-button  type='primary' @click="OnBatchEditClick()"><i class="fa fa-pencil-square"></i>批量修改</el-button>
				<el-button  type="success" class="ml10" @click="onImport()">
					<i class="fa fa-upload"></i>
					导入
				</el-button>
				<el-button  type="success" class="ml10" @click="onExport()">
					<i class="fa fa-download"></i>
					导出
				</el-button>
			</div>
			<WtmTable
				ref="tableRef"
				v-bind="tableData"
			>
			<template #operation>
				<el-table-column label="操作" width="100">
				<template v-slot="scope">
					<el-button text type="primary" @click="onEdit(scope.row)">修改</el-button>
					<el-popconfirm title="确定删除吗？" @confirm="onDelete(scope.row)">
						<template #reference>
							<el-button text type="primary">删除</el-button>
						</template>
					</el-popconfirm>
				</template>
			</el-table-column>
			</template>
		</WtmTable>			
		</el-card>
		
		<!-- <createDialog ref="createDialogRef" @refresh="getTableData()" /> -->
		<editDialog ref="editDialogRef" @refresh="getTableData()" />
		<importDialog ref="importDialogRef" @refresh="getTableData()" />
	</div>
</template>

<script setup lang="ts" name="message._system.menukey.UserManagement,true,WalkingTec.Mvvm.Admin.Api,FrameworkUser">
import { defineAsyncComponent, reactive, onMounted, onUpdated, ref,nextTick } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import frameworkuserApi from '/@/api/frameworkuser';
import { useThemeConfig } from '/@/stores/themeConfig';
import other from '/@/utils/other';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
// 引入组件
const createDialog = defineAsyncComponent(() => import('./create.vue'));
const editDialog = defineAsyncComponent(() => import('./edit.vue'));
const importDialog = defineAsyncComponent(() => import('./import.vue'));
const BatchEditDialog = defineAsyncComponent(() => import('./batchedit.vue'));
const storesThemeConfig = useThemeConfig();
// 定义变量内容
const createDialogRef = ref();
const editDialogRef = ref();
const importDialogRef = ref();
const tableRef = ref();
const tableData = ref({	
	// 列表数据（必传）
	data: [],
	// 表头内容（必传，注意格式）
	header: [
		{ key: 'ITCode', colWidth: '', title: '账户', type: 'text',sortable:'custom', isCheck: true },
		{ key: 'Name', colWidth: '', title: '姓名', type: 'text', isCheck: true },
		{ key: 'Gender', colWidth: '', title: '性别', type: 'switch', isCheck: true,  },
		{ key: 'CellPhone', colWidth: '', title: '手机', type: 'text', isCheck: true },
		{ key: 'RoleName_view', colWidth: '', title: '角色', type: 'text', isCheck: true },
		{ key: 'GroupName_view', colWidth: '', title: '用户组', type: 'text', isCheck: true },
		{ key: 'PhotoId', colWidth: '', imageWidth: '70', imageHeight: '40', title: '头像', type: 'image', isCheck: true },
		{ key: 'IsValid', colWidth: '', title: '启用', type: 'switch', isCheck: true,  },
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

// 初始化表格数据
const getTableData = () => {
	tableRef.value.doSearch(frameworkuserApi().search);
};
// 打开新增用户弹窗
const onCreate = () => {
	other.openDialog(t('message._system.common.vm.add'),createDialog);
	//createDialogRef.value.openDialog();
};
// 打开修改用户弹窗
const onEdit = (row: any) => {
	editDialogRef.value.openDialog(row);
};
// 删除用户
const onDelete = (row: any) => {
	frameworkuserApi().delete([row.ID]).then(()=>{getTableData()})
};
const onBatchDelete = () => {
	const selected = tableRef.value.getSelectedRows();
	frameworkuserApi().delete(selected.map((x:any)=>x.ID)).then(()=>{getTableData()})
};
const OnBatchEditClick = () => {
    const selectedrows = tableRef.value.getSelectedRows();
    const selectedids = selectedrows.map((x: any) => x.ID);
    other.openDialog("批量修改", BatchEditDialog, selectedids, getTableData)
};
const onExport = () => {
	const selected = tableRef.value.getSelectedRows();
	if(selected.length > 0){
		frameworkuserApi().exportById(selected.map((x:any)=>x.ID));
	}
	else{
		frameworkuserApi().export({});
	}
};
const onImport = () => {
	importDialogRef.value.openDialog();
};
// 页面加载时
onMounted(() => {
	getTableData();
});
</script>

<style scoped lang="scss">

.system-user-container {
	:deep(.el-card__body) {
		display: flex;
		flex-direction: column;
		flex: 1;
		overflow: auto;
	}
}
</style>
