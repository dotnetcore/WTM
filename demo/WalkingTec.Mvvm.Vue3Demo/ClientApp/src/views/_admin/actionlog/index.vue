<template>
	<div class="card-fill layout-padding">
		<el-card shadow="hover" class="layout-padding-auto" >
			<WtmSearcher v-model="searchData" @search="getTableData()">
				<el-row>

					<el-col :xs="24" :lg="8" class="mb20">
						<el-form-item ref="LogType_FormItem" prop="LogType" :label="$t('message._admin.actionlog.vm.LogType')">
							<el-select v-model="searchData.LogType" multiple>
								<el-option key="0" value="0" :label="$t('message._admin.actionlog.vm.LogType_0')"></el-option>
								<el-option key="1" value="1" :label="$t('message._admin.actionlog.vm.LogType_1')"></el-option>
								<el-option key="2" value="2" :label="$t('message._admin.actionlog.vm.LogType_2')"></el-option>
								<el-option key="3" value="3" :label="$t('message._admin.actionlog.vm.LogType_3')"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :lg="8" class="mb20">
						<el-form-item ref="ITCode_FormItem" prop="ITCode" :label="$t('message._admin.actionlog.vm.ITCode')">
							<el-input v-model="searchData.ITCode" clearable></el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :lg="8" class="mb20">
						<el-form-item ref="ActionUrl_FormItem" prop="ActionUrl"
							:label="$t('message._admin.actionlog.vm.ActionUrl')">
							<el-input v-model="searchData.ActionUrl" clearable></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :xs="24" :lg="8" class="mb20">
						<el-form-item ref="ActionTime_FormItem" prop="ActionTime"
							:label="$t('message._admin.actionlog.vm.ActionTime')">
							<el-date-picker v-model="searchData.ActionTime" type="daterange"></el-date-picker>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :lg="8" class="mb20">
						<el-form-item ref="IP_FormItem" prop="IP" :label="$t('message._admin.actionlog.vm.IP')">
							<el-input v-model="searchData.IP" clearable> </el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :lg="8" class="mb20">
						<el-form-item ref="Duration_FormItem" prop="Duration" :label="$t('message._admin.actionlog.vm.Duration')">
							<el-input v-model="searchData.Duration" clearable> </el-input>
						</el-form-item>
					</el-col>
				</el-row>

			</WtmSearcher>
			<WtmTable ref="tableRef" v-bind="tableData" :cell-style="tablecellstyle">
				<template #operation>
					<el-table-column :label="$t('message._system.common.vm.operate')" width="100">
						<template v-slot="scope">
							<el-button text type="primary" v-auth="'/api/_ActionLog/{id}'" @click="onDetail(scope.row)">{{
								$t('message._system.common.vm.detail') }}</el-button>
						</template>
					</el-table-column>
				</template>
			</WtmTable>
		</el-card>
	</div>
</template>

<script setup lang="ts" name="message._system.menukey.ActionLog;true;WalkingTec.Mvvm.Admin.Api;ActionLog">
import { defineAsyncComponent, reactive, onMounted, ref, getCurrentInstance } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import actionlogApi from '/@/api/actionlog';
import { useThemeConfig } from '/@/stores/themeConfig';
import Searcher from '/@/components/searchPanel/index.vue'
import other from '/@/utils/other';

const ci = getCurrentInstance() as any
// 引入组件
const detailDialog = defineAsyncComponent(() => import('./detail.vue'));
const storesThemeConfig = useThemeConfig();
// 定义变量内容
const tableRef = ref();
const tableData = ref({
	// 列表数据（必传）
	data: [],
	// 表头内容（必传，注意格式）
	header: [
		{ key: 'LogType', colWidth: '', title: ci.proxy.$t('message._admin.actionlog.vm.LogType'), type: 'text', isCheck: true },
		{ key: 'ModuleName', colWidth: '', title: ci.proxy.$t('message._admin.actionlog.vm.ModuleName'), type: 'text', isCheck: true },
		{ key: 'ActionName', colWidth: '', title: ci.proxy.$t('message._admin.actionlog.vm.ActionName'), type: 'text', isCheck: true },
		{ key: 'ITCode', colWidth: '', title: ci.proxy.$t('message._admin.actionlog.vm.ITCode'), type: 'text', isCheck: true },
		{ key: 'ActionUrl', colWidth: '', title: ci.proxy.$t('message._admin.actionlog.vm.ActionUrl'), type: 'text', isCheck: true },
		{ key: 'ActionTime', colWidth: '', title: ci.proxy.$t('message._admin.actionlog.vm.ActionTime'), type: 'text', isCheck: true },
		{ key: 'Duration', colWidth: '', title: ci.proxy.$t('message._admin.actionlog.vm.Duration'), type: 'text', sortable: 'custom', isCheck: true },
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
	ITCode: null,
	ActionUrl: null,
	ActionTime: null,
	IP: null,
	LogType: null,
	ActionName: null,
	Duration: null,
	ModuleName: null,
	Remark: null
});

// 初始化表格数据
const getTableData = () => {
	tableRef.value.doSearch(actionlogApi().search, searchData.value)
		.catch((error: any) => {
			other.setFormError(ci, error);
		});
};
// 打开详情
const onDetail = (row: any) => {
	other.openDialog(ci.proxy.$t('message._system.common.vm.detail'),detailDialog,row)
};

const tablecellstyle=(data:any)=>{
	if(data.column.property == 'Duration'){
		if(data.row.Duration<1){
			return {color:'green'};
		}
		else if(data.row.Duration<3){
			return {color:'orange'};
		}
		else{
			return {color:'red'};
		}
	}
}
// 页面加载时
onMounted(() => {
	getTableData();
});
</script>

<style scoped lang="scss">

</style>
