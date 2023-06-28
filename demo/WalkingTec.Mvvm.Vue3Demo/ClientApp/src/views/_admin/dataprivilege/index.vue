<template>
	<div class="card-fill layout-padding">
		<el-card shadow="hover" class="layout-padding-auto" >
			<WtmSearcher v-model="searchData.vmModel" @search="getTableData()">
				<el-row>
					<el-col :xs="24" :lg="12" class="mb20">
						<el-form-item prop="TableName" :label="$t('message._admin.dp.vm.TableName')">
							<el-select v-model="searchData.vmModel.TableName" :placeholder="$t('message._system.common.vm.all')">
								<el-option v-for="item in searchData.AllPrivileges" :key="item.Value" :value="item.Value"
									:label="item.Text">{{
										item.Text
									}}</el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :lg="12" class="mb20">
						<el-form-item prop="DpType"  :label="$t('message._admin.dp.vm.DpType')">
							<el-select v-model="searchData.vmModel.DpType" >
								<el-option value="UserGroup" :label="$t('message._admin.dp.vm.DpGroup')"/>
								<el-option value="User" :label="$t('message._admin.dp.vm.DpUser')"/>
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
			</WtmSearcher>
			<div style="text-align: right;">
				<el-button type="primary" class="ml10" @click="onCreate()" v-auth="'/api/_dataprivilege/Add'">
					<i class="fa fa-plus"></i>
					{{ $t('message._system.common.vm.add') }}
				</el-button>
			</div>
			<WtmTable
				ref="tableRef"
				v-bind="tableData"
			>
			<template #operation>
				<el-table-column :label="$t('message._system.common.vm.operate')" width="150">
				<template v-slot="scope">
					<el-button text type="primary" @click="onEdit(scope.row)" v-auth="'/api/_dataprivilege/Edit'">{{ $t('message._system.common.vm.edit') }}</el-button>
					<el-button text type="primary" @click="onDetail(scope.row)" v-auth="'/api/_dataprivilege/{id}'">{{ $t('message._system.common.vm.detail') }}</el-button>
					<el-popconfirm :title="$t('message._system.common.vm.deletetip')"  @confirm="onDelete(scope.row)">
						<template #reference>
							<el-button text type="danger" v-auth="'/api/_dataprivilege/delete'">{{ $t('message._system.common.vm.delete') }}</el-button>
						</template>
					</el-popconfirm>
				</template>
			</el-table-column>
			</template>
		</WtmTable>			
		</el-card>

	</div>
</template>

<script setup lang="ts" name="message._system.menukey.DataPrivilege;true;WalkingTec.Mvvm.Admin.Api;DataPrivilege">
import { defineAsyncComponent, reactive, onMounted, ref, getCurrentInstance } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import dataPrivilegeApi from '/@/api/dataprivilege';
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
		{ key: 'Name', colWidth: '', title: ci.proxy.$t('message._admin.dp.vm.Name'), type: 'text', isCheck: true },
		{ key: 'PName', colWidth: '', title: ci.proxy.$t('message._admin.dp.vm.TableName'), type: 'text', isCheck: true },
		{ key: 'RelateIDs', colWidth: '', title: ci.proxy.$t('message._admin.dp.vm.PCount'), type: 'text', isCheck: true },
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
const searchData = reactive({
	vmModel:{
		TableName: null,
		DpType: 'UserGroup'
	},
	AllPrivileges:[] as any[]
});


// 初始化表格数据
const getTableData = () => {
	tableRef.value.doSearch(dataPrivilegeApi().search, searchData.vmModel)
		.catch((error: any) => {
			other.setFormError(ci, error);
		});
};
// 打开新增弹窗
const onCreate = () => {
	other.openDialog(ci.proxy.$t('message._system.common.vm.add'),createDialog,{DpType:searchData.vmModel.DpType},getTableData)
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
	dataPrivilegeApi().delete(row).then(() => { getTableData() })
};



// 页面加载时
onMounted(() => {
	other.getSelectList('/api/_DataPrivilege/GetPrivileges',[],true).then(x=>{searchData.AllPrivileges = x});
	getTableData();
});
</script>

<style scoped lang="scss">
</style>
