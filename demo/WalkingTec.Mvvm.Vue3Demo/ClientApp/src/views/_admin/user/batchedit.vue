
<template>
<div class="card-fill layout-padding">
<el-card shadow="hover" class="layout-padding-auto" >
  <el-form ref="formRefFrameworkUser" :model="stateFrameworkUser.vmModel" label-width="100px">
    <div style="margin-bottom:10px;">{{$t('message.autotrans.SysBatchEditConfirm')}}</div>
    <el-row>
      <el-col :xs="24" :lg="12" class="mb20">
      <el-form-item ref="LinkedVM_Email_FormItem" prop="LinkedVM_Email" :label="$t('message.autotrans._Model_FrameworkUser_Email')">
          <el-input v-model="stateFrameworkUser.vmModel.LinkedVM.Email" clearable></el-input>
      </el-form-item>

      </el-col>
      <el-col :xs="24" :lg="12" class="mb20">
      <el-form-item ref="LinkedVM_Gender_FormItem" prop="LinkedVM_Gender" :label="$t('message.autotrans._Model_FrameworkUser_Gender')">
          <el-select v-model="stateFrameworkUser.vmModel.LinkedVM.Gender" clearable>
             <el-option key="Male" value="Male" :label="$t('message.autotrans._Enum_GenderEnum_Male')"></el-option>
             <el-option key="Female" value="Female" :label="$t('message.autotrans._Enum_GenderEnum_Female')"></el-option>
          </el-select>
      </el-form-item>

      </el-col>
      
    </el-row>

    <div style="text-align:right;">
      <el-button @click="onSubmitFrameworkUser" >提交</el-button>

      <el-button @click="onCloseFrameworkUser" >关闭</el-button>

    </div>

  </el-form>

</el-card>
</div>
</template>


<script setup lang="ts" name="message.autotrans._Page_AdminFrameworkUserBatchEdit,false">
import {  ElMessageBox, ElMessage } from 'element-plus';
import { defineAsyncComponent,reactive, ref, getCurrentInstance, onMounted, nextTick } from 'vue';
import frameworkuserApi from '/@/api//frameworkuser';
import other from '/@/utils/other';
import fileApi from '/@/api/file';
import { useRouter } from "vue-router";

const ci = getCurrentInstance() as any;


// 定义子组件向父组件传值/事件
const emit = defineEmits(['refresh','closeDialog']);
// 定义变量内容
const formRefFrameworkUser = ref();
const stateFrameworkUser = reactive({
    vmModel: {
      LinkedVM:{
          Email: '',
          Gender: undefined,
          CellPhone: '',
          HomePhone: '',
          Address: '',
          ZipCode: '',
          SelectedFrameworkUserRolesIDs: [],
          SelectedFrameworkUserGroupsIDs: [],     
      },
      Ids:[],      
	},
    
    AllFrameworkUserRoles: [] as any[],
    AllFrameworkUserGroups: [] as any[],
});

// 取消
const onCloseFrameworkUser = () => {
    closeDialog();
};

// 提交
const onSubmitFrameworkUser = () => {
    formRefFrameworkUser.value?.validate((valid: boolean, fields: any) => {
		if (valid) {
            frameworkuserApi().batchedit(stateFrameworkUser.vmModel).then(() => {
                ElMessage.success(ci.proxy.$t('message._system.common.vm.submittip'));
                emit('refresh');
                closeDialog();
            }).catch((error) => {
                other.setFormError(ci, error);
            })
		}
	})
};

// 暴露变量
defineExpose({

});
// 页面加载时
onMounted(() => {
    stateFrameworkUser.vmModel.Ids = ci.attrs["wtmdata"];
});

// 关闭弹窗
const closeDialog = () => {
    emit('closeDialog');
};
</script>

<style scoped lang="scss">

</style>

