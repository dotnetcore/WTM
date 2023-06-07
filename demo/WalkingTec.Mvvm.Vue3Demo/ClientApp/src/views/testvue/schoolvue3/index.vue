
<template>
<div class="card-fill layout-padding">
<el-card shadow="hover" class="layout-padding-auto" >
  <WtmSearcher v-model="searchDataSchoolVue3" @search="getTableDataSchoolVue3">
      <el-row>
        
    <el-col :xs="24" :lg="12" class="mb20">
        <el-form-item ref="SchoolCode_FormItem" prop="SchoolCode" label="学校编码">
            <el-input v-model="searchDataSchoolVue3.SchoolCode" clearable></el-input>
        </el-form-item>
    </el-col>
    <el-col :xs="24" :lg="12" class="mb20">
        <el-form-item ref="SchoolName_FormItem" prop="SchoolName" label="学校名称">
            <el-input v-model="searchDataSchoolVue3.SchoolName" clearable></el-input>
        </el-form-item>
    </el-col>
    <el-col :xs="24" :lg="12" class="mb20">
        <el-form-item ref="Level_FormItem" prop="Level" label="级别">
            <el-input-number v-model="searchDataSchoolVue3.Level" clearable></el-input-number>
        </el-form-item>
    </el-col>
      </el-row>

  </WtmSearcher>

  <div style="text-align: right;">
      <WtmButton v-auth="'/api/SchoolVue3/Add'" icon='fa fa-plus' type='primary' :button-text="$t('message._system.common.vm.add')" @click="OnCreateClick()"/>
    <WtmButton v-auth="'/api/SchoolVue3/BatchDelete'"  icon='fa fa-trash' type='danger' :button-text="$t('message._system.common.vm.batchdelete')" :confirm="$t('message._system.common.vm.deletetip')" @click="onBatchDelete()"/>
    <WtmButton v-auth="'/api/SchoolVue3/Import'" icon='fa fa-tasks' type='primary' :button-text="$t('message._system.common.vm.import')" @click="OnImportClick()"/>
    <WtmButton v-auth="'/api/SchoolVue3/SchoolVue3ExportExcel'"  icon='fa fa-arrow-circle-down' type='primary' :button-text="$t('message._system.common.vm.export')" @click="onExport()"/>

  </div>
  <WtmTable ref="tableRefSchoolVue3" v-bind="tableDataSchoolVue3">
    <template #operation>
      <el-table-column :label="$t('message._system.common.vm.operate')" width="180">
        <template v-slot="scope">
          <el-button text type="primary" @click="OnEditrowClick(scope.row)" v-auth="'/api/SchoolVue3/Edit'">{{ $t('message._system.common.vm.edit') }}</el-button>
					<el-button text type="primary" @click="OnDetailsrowClick(scope.row)" v-auth="'/api/SchoolVue3/{id}'">{{ $t('message._system.common.vm.detail') }}</el-button>
					<el-popconfirm :title="$t('message._system.common.vm.deletetip')"  @confirm="onDelete(scope.row)">
						<template #reference>
							<el-button text type="danger" v-auth="'/api/SchoolVue3/BatchDelete'">{{ $t('message._system.common.vm.delete') }}</el-button>
						</template>
					</el-popconfirm>
        </template>
      </el-table-column>
    </template>
  </WtmTable>

</el-card>
</div>
</template>


<script setup lang="ts" name="测试,true,WalkingTec.Mvvm.Vue3Demo.Controllers,SchoolVue3">
import {  ElMessageBox, ElMessage } from 'element-plus';
import { defineAsyncComponent,reactive, ref, getCurrentInstance, onMounted, nextTick } from 'vue';
import { SchoolVue3Api } from '/@/api/testvue/SchoolVue3';
import other from '/@/utils/other';
import fileApi from '/@/api/file';
import { useRouter } from "vue-router";
const ci = getCurrentInstance() as any;

const CreateDialog = defineAsyncComponent(() => import('./create.vue'));
const EditDialog = defineAsyncComponent(() => import('./edit.vue'));
const DetailsDialog = defineAsyncComponent(() => import('./details.vue'));
const ImportDialog = defineAsyncComponent(() => import('./import.vue'));

const stateSchoolVue3 = reactive({
    
});

const searchDataSchoolVue3 = ref({
    			SchoolCode: null,
			SchoolName: null,
			Level: null,

});

// 定义变量内容
const tableRefSchoolVue3 = ref();
const tableDataSchoolVue3 = ref({
    // 列表数据（必传）
    data: [],
	// 表头内容（必传，注意格式）
	header: [
  
        {title:'学校编码',key: 'SchoolCode',type: 'text',isCheck: true},
        {title:'学校名称',key: 'SchoolName',type: 'text',isCheck: true},
        {title:'学校类型',key: 'SchoolType',type: 'text',isCheck: true},
        {title:'备注',key: 'Remark',type: 'text',isCheck: true},
        {title:'级别',key: 'Level',type: 'text',isCheck: true},
        {title:'地点',key: 'Name_view',type: 'text',isCheck: true},
        {title:'是学校',key: 'IsSchool',type: 'switch',isCheck: true},
        {title:'照片',key: 'PhotoId',type: 'image',isCheck: true},
        {title:'附件',key: 'FileId',type: 'text',isCheck: true},
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
const getTableDataSchoolVue3 = () => {
    tableRefSchoolVue3.value.doSearch(SchoolVue3Api().search, searchDataSchoolVue3.value)
        .catch((error: any) => {
			other.setFormError(ci, error);
		});
};
const OnCreateClick = () => {
    other.openDialog(ci.proxy.$t('message._system.common.vm.add'), CreateDialog, null, getTableDataSchoolVue3)
};

const OnEditrowClick = (row: any) => {
    other.openDialog(ci.proxy.$t('message._system.common.vm.edit'), EditDialog, row, getTableDataSchoolVue3)
};

const OnDetailsrowClick = (row: any) => {
    other.openDialog(ci.proxy.$t('message._system.common.vm.detail'), DetailsDialog, row, getTableDataSchoolVue3)
};

// 删除
const onDelete = (row: any) => {
    SchoolVue3Api().delete([row.ID]).then(() => { getTableDataSchoolVue3()})
};

const onBatchDelete = () => {
    const selectedrows = tableRefSchoolVue3.value.getSelectedRows();
    const selectedids = selectedrows.map((x: any) => x.ID);
    if (selectedids.length > 0)
      SchoolVue3Api().delete(selectedids).then(() => { getTableDataSchoolVue3()})
    else
      ElMessage.error(ci.proxy.$t('message._system.common.vm.check'));
};

const OnImportClick = () => {
    other.openDialog(ci.proxy.$t('message._system.common.vm.import'), ImportDialog, null, getTableDataSchoolVue3)
};

const onExport = () => {
	const selected = tableRefSchoolVue3.value.getSelectedRows();
	if (selected.length > 0) {
		SchoolVue3Api().exportById(selected.map((x: any) => x.ID));
	}
	else {
		SchoolVue3Api().export({});
	}
};

// 页面加载时
onMounted(() => {
     
        getTableDataSchoolVue3();

});

// 暴露变量
defineExpose({

});
</script>

<style scoped lang="scss">

</style>

