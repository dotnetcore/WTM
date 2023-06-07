
<template>
<div class="card-fill layout-padding">
<el-card shadow="hover" class="layout-padding-auto" >
  <WtmSearcher v-model="searchDataCity" @search="getTableDataCity">
      <el-row>
        
    <el-col :xs="24" :lg="12" class="mb20">
        <el-form-item ref="Name_FormItem" prop="Name" label="名称">
            <el-input v-model="searchDataCity.Name" clearable></el-input>
        </el-form-item>
    </el-col>
    <el-col :xs="24" :lg="12" class="mb20">
        <el-form-item ref="Code_FormItem" prop="Code" label="Code">
            <el-input v-model="searchDataCity.Code" clearable></el-input>
        </el-form-item>
    </el-col>
    <el-col :xs="24" :lg="12" class="mb20">
        <el-form-item ref="ParentId_FormItem" prop="ParentId" label="父级">
            <el-select v-model="searchDataCity.ParentId" :data="stateCity.AllCitys" clearable></el-select>
        </el-form-item>
    </el-col>
      </el-row>

  </WtmSearcher>

  <div style="text-align: right;">
      <WtmButton v-auth="'/api/City/Add'" icon='fa fa-plus' type='primary' :button-text="$t('message._system.common.vm.add')" @click="OnCreateClick()"/>
    <WtmButton v-auth="'/api/City/BatchDelete'"  icon='fa fa-trash' type='danger' :button-text="$t('message._system.common.vm.batchdelete')" :confirm="$t('message._system.common.vm.deletetip')" @click="onBatchDelete()"/>
    <WtmButton v-auth="'/api/City/Import'" icon='fa fa-tasks' type='primary' :button-text="$t('message._system.common.vm.import')" @click="OnImportClick()"/>
    <WtmButton v-auth="'/api/City/CityExportExcel'"  icon='fa fa-arrow-circle-down' type='primary' :button-text="$t('message._system.common.vm.export')" @click="onExport()"/>

  </div>
  <WtmTable ref="tableRefCity" v-bind="tableDataCity">
    <template #operation>
      <el-table-column :label="$t('message._system.common.vm.operate')" width="180">
        <template v-slot="scope">
          <el-button text type="primary" @click="OnEditrowClick(scope.row)" v-auth="'/api/City/Edit'">{{ $t('message._system.common.vm.edit') }}</el-button>
					<el-button text type="primary" @click="OnDetailsrowClick(scope.row)" v-auth="'/api/City/{id}'">{{ $t('message._system.common.vm.detail') }}</el-button>
					<el-popconfirm :title="$t('message._system.common.vm.deletetip')"  @confirm="onDelete(scope.row)">
						<template #reference>
							<el-button text type="danger" v-auth="'/api/City/BatchDelete'">{{ $t('message._system.common.vm.delete') }}</el-button>
						</template>
					</el-popconfirm>
        </template>
      </el-table-column>
    </template>
  </WtmTable>

</el-card>
</div>
</template>


<script setup lang="ts" name="城市,true,WalkingTec.Mvvm.Vue3Demo.Controllers,City">
import {  ElMessageBox, ElMessage } from 'element-plus';
import { defineAsyncComponent,reactive, ref, getCurrentInstance, onMounted, nextTick } from 'vue';
import { CityApi } from '/@/api/schooldata/City';
import other from '/@/utils/other';
import fileApi from '/@/api/file';
import { useRouter } from "vue-router";
const ci = getCurrentInstance() as any;

const CreateDialog = defineAsyncComponent(() => import('./create.vue'));
const EditDialog = defineAsyncComponent(() => import('./edit.vue'));
const DetailsDialog = defineAsyncComponent(() => import('./details.vue'));
const ImportDialog = defineAsyncComponent(() => import('./import.vue'));

const stateCity = reactive({
    
    AllCitys: [] as any[],
});

const searchDataCity = ref({
    			Name: null,
			Code: null,
			ParentId: null,

});

// 定义变量内容
const tableRefCity = ref();
const tableDataCity = ref({
    // 列表数据（必传）
    data: [],
	// 表头内容（必传，注意格式）
	header: [
  
        {title:'名称',key: 'Name',type: 'text',isCheck: true},
        {title:'Level',key: 'Level',type: 'text',isCheck: true},
        {title:'父级',key: 'Name_view',type: 'text',isCheck: true},
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
const getTableDataCity = () => {
    tableRefCity.value.doSearch(CityApi().search, searchDataCity.value)
        .catch((error: any) => {
			other.setFormError(ci, error);
		});
};
const OnCreateClick = () => {
    other.openDialog(ci.proxy.$t('message._system.common.vm.add'), CreateDialog, null, getTableDataCity)
};

const OnEditrowClick = (row: any) => {
    other.openDialog(ci.proxy.$t('message._system.common.vm.edit'), EditDialog, row, getTableDataCity)
};

const OnDetailsrowClick = (row: any) => {
    other.openDialog(ci.proxy.$t('message._system.common.vm.detail'), DetailsDialog, row, getTableDataCity)
};

// 删除
const onDelete = (row: any) => {
    CityApi().delete([row.ID]).then(() => { getTableDataCity()})
};

const onBatchDelete = () => {
    const selectedrows = tableRefCity.value.getSelectedRows();
    const selectedids = selectedrows.map((x: any) => x.ID);
    if (selectedids.length > 0)
      CityApi().delete(selectedids).then(() => { getTableDataCity()})
    else
      ElMessage.error(ci.proxy.$t('message._system.common.vm.check'));
};

const OnImportClick = () => {
    other.openDialog(ci.proxy.$t('message._system.common.vm.import'), ImportDialog, null, getTableDataCity)
};

const onExport = () => {
	const selected = tableRefCity.value.getSelectedRows();
	if (selected.length > 0) {
		CityApi().exportById(selected.map((x: any) => x.ID));
	}
	else {
		CityApi().export({});
	}
};

// 页面加载时
onMounted(() => {
     
    other.getSelectList('/api/City/GetCitys',[],false).then(x=>{stateCity.AllCitys = x});

        getTableDataCity();

});

// 暴露变量
defineExpose({

});
</script>

<style scoped lang="scss">

</style>

