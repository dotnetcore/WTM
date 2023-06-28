
<template>
<div class="card-fill layout-padding">
<el-card shadow="hover" class="layout-padding-auto" >
  <el-form ref="formRefSchoolVue3" :model="stateSchoolVue3.vmModel" label-width="100px">
    <el-row>
      
    <el-col :xs="24" :lg="12" class="mb20">
        <el-form-item ref="Entity_SchoolCode_FormItem" prop="Entity.SchoolCode" label="学校编码" :rules="[{ required: true, message:'学校编码为必填项',trigger:'blur'}]">
            <el-input v-model="stateSchoolVue3.vmModel.Entity.SchoolCode" clearable></el-input>
        </el-form-item>
    </el-col>
    <el-col :xs="24" :lg="12" class="mb20">
        <el-form-item ref="Entity_SchoolName_FormItem" prop="Entity.SchoolName" label="学校名称" :rules="[{ required: true, message:'学校名称为必填项',trigger:'blur'}]">
            <el-input v-model="stateSchoolVue3.vmModel.Entity.SchoolName" clearable></el-input>
        </el-form-item>
    </el-col>
    <el-col :xs="24" :lg="12" class="mb20">
        <el-form-item ref="Entity_SchoolType_FormItem" prop="Entity.SchoolType" label="学校类型">
            <el-select v-model="stateSchoolVue3.vmModel.Entity.SchoolType" clearable>
                <el-option key="PUB" value="PUB" label="公立学校"></el-option>
                <el-option key="PRI" value="PRI" label="私立学校"></el-option></el-select>
        </el-form-item>
    </el-col>
    <el-col :xs="24" :lg="12" class="mb20">
        <el-form-item ref="Entity_Remark_FormItem" prop="Entity.Remark" label="备注">
            <el-input v-model="stateSchoolVue3.vmModel.Entity.Remark" clearable></el-input>
        </el-form-item>
    </el-col>
    <el-col :xs="24" :lg="12" class="mb20">
        <el-form-item ref="Entity_Level_FormItem" prop="Entity.Level" label="级别" :rules="[{ required: true, message:'级别为必填项',trigger:'blur'}]">
            <el-input-number v-model="stateSchoolVue3.vmModel.Entity.Level" clearable></el-input-number>
        </el-form-item>
    </el-col>
    <el-col :xs="24" :lg="12" class="mb20">
        <el-form-item ref="Entity_PlaceId_FormItem" prop="Entity.PlaceId" label="地点">
            <el-select v-model="stateSchoolVue3.vmModel.Entity.PlaceId" clearable>
                       <el-option v-for="item in stateSchoolVue3.AllCitys" :key="item.Value" :value="item.Value" :label="item.Text"></el-option></el-select>
        </el-form-item>
    </el-col>
    <el-col :xs="24" :lg="12" class="mb20">
        <el-form-item ref="Entity_IsSchool_FormItem" prop="Entity.IsSchool" label="是学校" :rules="[{ required: true, message:'是学校为必填项',trigger:'blur'}]">
            <el-select v-model="stateSchoolVue3.vmModel.Entity.IsSchool" clearable>
                <el-option :key="1" :value=true :label="$t('message._system.common.vm.tips_bool_true')"></el-option>
                <el-option :key="0" :value=false :label="$t('message._system.common.vm.tips_bool_false')"></el-option>
                </el-select>
        </el-form-item>
    </el-col>
    <el-col :xs="24" :lg="12" class="mb20">
        <el-form-item ref="Entity_PhotoId_FormItem" prop="Entity.PhotoId" label="照片">
            <wtm-upload-image v-model="stateSchoolVue3.vmModel.Entity.PhotoId" clearable></wtm-upload-image>
        </el-form-item>
    </el-col>
    <el-col :xs="24" :lg="12" class="mb20">
        <el-form-item ref="Entity_FileId_FormItem" prop="Entity.FileId" label="附件">
            <wtm-upload-file v-model="stateSchoolVue3.vmModel.Entity.FileId" clearable></wtm-upload-file>
        </el-form-item>
    </el-col>
    </el-row>

    <div style="text-align:right;">
      <WtmButton @click="onSubmitSchoolVue3"  type="primary" :button-text="$t('message._system.common.vm.submit')" style="margin-top:15px;"/>
      <WtmButton @click="onCloseSchoolVue3"  type="primary" :button-text="$t('message._system.common.vm.cancel')" style="margin-top:15px;"/>
    </div>
  </el-form>

</el-card>
</div>
</template>


<script setup lang="ts" name="message._system.common.vm.edit;false">
import {  ElMessageBox, ElMessage } from 'element-plus';
import { defineAsyncComponent,reactive, ref, getCurrentInstance, onMounted, nextTick } from 'vue';
import { SchoolVue3Api } from '/@/api/testvue/SchoolVue3';
import other from '/@/utils/other';
import fileApi from '/@/api/file';
import { useRouter } from "vue-router";
const ci = getCurrentInstance() as any;

// 定义变量内容
const formRefSchoolVue3 = ref();
const stateSchoolVue3 = reactive({
     vmModel: {
	  Entity:{
      			ID: null,
			SchoolCode: null,
			SchoolName: null,
			SchoolType: null,
			Remark: null,
			Level: null,
			PlaceId: null,
			IsSchool: null,
			PhotoId: null,
			FileId: null,

      },
      
	},
    
     
    AllCitys: [] as any[],
});

// 取消
const onCloseSchoolVue3 = () => {
    closeDialog();
};

// 提交
const onSubmitSchoolVue3 = () => {
    formRefSchoolVue3.value?.validate((valid: boolean, fields: any) => {
		if (valid) {
            SchoolVue3Api().edit(stateSchoolVue3.vmModel).then(() => {
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
    
    other.getSelectList('/api/SchoolVue3/GetCitys',[],false).then(x=>{stateSchoolVue3.AllCitys = x});

     if (ci.attrs["wtmdata"]) {
		stateSchoolVue3.vmModel.Entity.ID = ci.attrs["wtmdata"].ID;
	}
	else if (useRouter().currentRoute.value.query.id) {
		stateSchoolVue3.vmModel.Entity.ID = useRouter().currentRoute.value.query.id as any;
	}
	SchoolVue3Api().get(stateSchoolVue3.vmModel.Entity.ID ?? "").then((data: any) => other.setValue(stateSchoolVue3.vmModel, data));
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

