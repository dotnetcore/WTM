
<template>
<div class="card-fill layout-padding">
<el-card shadow="hover" class="layout-padding-auto" >
  <el-form ref="formRefCity" :model="stateCity.vmModel" label-width="100px">
    <el-row>
      
    <el-col :xs="24" :lg="12" class="mb20">
        <el-form-item ref="Entity_Name_FormItem" prop="Entity.Name" label="名称">
            <el-input v-model="stateCity.vmModel.Entity.Name" clearable></el-input>
        </el-form-item>
    </el-col>
    <el-col :xs="24" :lg="12" class="mb20">
        <el-form-item ref="Entity_Level_FormItem" prop="Entity.Level" label="Level" :rules="[{ required: true, message:'Level为必填项',trigger:'blur'}]">
            <el-input-number v-model="stateCity.vmModel.Entity.Level" clearable></el-input-number>
        </el-form-item>
    </el-col>
    <el-col :xs="24" :lg="12" class="mb20">
        <el-form-item ref="Entity_ParentId_FormItem" prop="Entity.ParentId" label="父级">
            <el-select v-model="stateCity.vmModel.Entity.ParentId" clearable>
                       <el-option v-for="item in stateCity.AllCitys" :key="item.Value" :value="item.Value" :label="item.Text"></el-option></el-select>
        </el-form-item>
    </el-col>
    </el-row>

    <div style="text-align:right;">
      <WtmButton @click="onSubmitCity"  type="primary" :button-text="$t('message._system.common.vm.submit')" style="margin-top:15px;"/>
      <WtmButton @click="onCloseCity"  type="primary" :button-text="$t('message._system.common.vm.cancel')" style="margin-top:15px;"/>
    </div>
  </el-form>

</el-card>
</div>
</template>


<script setup lang="ts" name="message._system.common.vm.edit;false">
import {  ElMessageBox, ElMessage } from 'element-plus';
import { defineAsyncComponent,reactive, ref, getCurrentInstance, onMounted, nextTick } from 'vue';
import { CityApi } from '/@/api/schooldata/City';
import other from '/@/utils/other';
import fileApi from '/@/api/file';
import { useRouter } from "vue-router";
const ci = getCurrentInstance() as any;

// 定义变量内容
const formRefCity = ref();
const stateCity = reactive({
     vmModel: {
	  Entity:{
      			ID: null,
			Name: null,
			Level: null,
			ParentId: null,

      },
      
	},
    
     
    AllCitys: [] as any[],
});

// 取消
const onCloseCity = () => {
    closeDialog();
};

// 提交
const onSubmitCity = () => {
    formRefCity.value?.validate((valid: boolean, fields: any) => {
		if (valid) {
            CityApi().edit(stateCity.vmModel).then(() => {
                ElMessage.success(ci.proxy.$t('message._system.common.vm.submittip'));
                emit('refresh');
                closeDialog();
            }).catch((error) => {
                other.setFormError(ci, error);
            })
		}
	})
};

// 页面加载时
onMounted(() => {
    
    other.getSelectList('/api/City/GetCitys',[],false).then(x=>{stateCity.AllCitys = x});

     if (ci.attrs["wtmdata"]) {
		stateCity.vmModel.Entity.ID = ci.attrs["wtmdata"].ID;
	}
	else if (useRouter().currentRoute.value.query.id) {
		stateCity.vmModel.Entity.ID = useRouter().currentRoute.value.query.id as any;
	}
	CityApi().get(stateCity.vmModel.Entity.ID ?? "").then((data: any) => other.setValue(stateCity.vmModel, data));
});

// 定义子组件向父组件传值/事件
const emit = defineEmits(['refresh','closeDialog']);
// 关闭弹窗
const closeDialog = () => {
    emit('closeDialog');
};
// 暴露变量
defineExpose({

});
</script>

<style scoped lang="scss">

</style>

