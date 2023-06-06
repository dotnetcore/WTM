<template>
	<div class="system-user-dialog-container">
		<el-dialog title="修改用户" v-model="state.dialog.isShowDialog" draggable>
			<el-form ref="formRef" :model="state.vmModel"  label-width="90px" >
				<el-tabs type="border-card">
					<el-tab-pane label="基础信息">
						<el-row >
							<el-col :xs="24" :lg="24"  class="mb20">
								<el-form-item ref="Entity_PhotoId_FormItem" prop="Entity.PhotoId" label="头像" >
									<Uploader v-model="state.vmModel.Entity.PhotoId"  />
								</el-form-item>
							</el-col>
						</el-row>
						<el-row >
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item ref="Entity_ITCode_FormItem" prop="Entity.ITCode" label="账号" :rules="[{ required: true, message: '请输入账号', trigger: 'blur' }]">
									<el-input v-model="state.vmModel.Entity.ITCode" placeholder="请输入账户名称" clearable></el-input>
								</el-form-item>
							</el-col>
						</el-row>
						<el-row >
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item ref="Entity_Name_FormItem" prop="Entity.Name" label="姓名" :rules="[{ required: true, message: '请输入姓名', trigger: 'blur' }]">
									<el-input v-model="state.vmModel.Entity.Name" placeholder="请输入姓名" clearable></el-input>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item ref="Entity_Gender_FormItem" prop="Entity.Gender" label="性别" >
									<el-select v-model="state.vmModel.Entity.Gender" show-password placeholder="请选择" clearable>
										<el-option label="男" value="Male"/>
										<el-option label="女" value="Female"/>
									</el-select>
								</el-form-item>
							</el-col>
						</el-row>
						<el-row >
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item ref="Entity_SelectedRolesCodes_FormItem" prop="Entity.SelectedRolesCodes" label="角色" >
									<el-checkbox-group v-model="state.vmModel.SelectedRolesCodes">
										<el-checkbox v-for="item in state.allRoles" :key="item.Value" :label="item.Value">{{
										item.Text
										}}</el-checkbox>
									</el-checkbox-group>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item ref="Entity_SelectedGroupCodes_FormItem" prop="Entity.SelectedGroupCodes" label="部门">
									<el-cascader
										:options="state.allGroups"
										:props="{ checkStrictly: true,emitPath:false,multiple:true,label:'Text',value:'Value',children:'Children' }"
										placeholder="请选择部门"
										clearable
										filterable
										class="w100"
										v-model="state.vmModel.SelectedGroupCodes"
									/>
								</el-form-item>
							</el-col>				
						</el-row>	
					</el-tab-pane>
    				<el-tab-pane label="附加信息">
						<el-row >
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="手机" ref="Entity_CellPhone_FormItem" prop="Entity.CellPhone" >
									<el-input v-model="state.vmModel.Entity.CellPhone" placeholder="请输入手机" clearable></el-input>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="座机" ref="Entity_HomePhone_FormItem" prop="Entity.HomePhone" >
									<el-input v-model="state.vmModel.Entity.HomePhone"  placeholder="请输入座机" clearable></el-input>
								</el-form-item>
							</el-col>
						</el-row>
						<el-row >
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="邮箱" ref="Entity_Email_FormItem" prop="Entity.Email" >
									<el-input v-model="state.vmModel.Entity.Email" placeholder="请输入邮箱" clearable></el-input>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="邮编" ref="Entity_ZipCode_FormItem" prop="Entity.ZipCode" >
									<el-input v-model="state.vmModel.Entity.ZipCode"  placeholder="请输入邮编" clearable></el-input>
								</el-form-item>
							</el-col>
						</el-row>	
						<el-row >
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="地址" ref="Entity_Address_FormItem" prop="Entity.Address" >
									<el-input v-model="state.vmModel.Entity.Address" placeholder="请输入地址" clearable></el-input>
								</el-form-item>
							</el-col>						
						</el-row>					
					</el-tab-pane>
  				</el-tabs>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="onCancel" >取消</el-button>
					<el-button type="primary" @click="onSubmit" >提交</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts" name="message._system.common.vm.edit,false">
import { ElMessage } from 'element-plus';
import { array } from 'snabbdom';
import { reactive, ref, getCurrentInstance,nextTick } from 'vue';
import frameworkuserApi from '/@/api/frameworkuser';
import other from '/@/utils/other';
import Uploader from '/@/components/uploadImage/index.vue';

// 定义子组件向父组件传值/事件
const emit = defineEmits(['refresh']);

// 定义变量内容
const formRef = ref();
const state = reactive({
	vmModel: {
		Entity:{
			ID:null,
			ITCode:null,
			Name:null,
			Gender:null,
			PhotoId:null,
			CellPhone:null,
			HomePhone:null,
			Email:null,
			ZipCode:null,
			Address:null
		},
		SelectedRolesCodes: [], // 关联角色
		SelectedGroupCodes: [], // 部门
	},
	allRoles : [] as any[],
	allGroups: [] as any[], 
	dialog: {
		isShowDialog: false,
	},
});
const ci = getCurrentInstance() as any;
// 打开弹窗
const openDialog = (row: any) => {
	other.clearObj(state.vmModel);
	other.clearFormError(ci);
	state.dialog.isShowDialog = true;
	frameworkuserApi().get(row.ID).then((data:any)=>other.setValue(state.vmModel,data));
	other.getSelectList('/api/_FrameworkUser/GetFrameworkRoles',[],false).then(x=>{state.allRoles = x});
	other.getSelectList('/api/_FrameworkUser/GetFrameworkGroupsTree',[],true).then(x=>{state.allGroups = x});
};
// 关闭弹窗
const closeDialog = () => {
	state.dialog.isShowDialog = false;
};
// 取消
const onCancel = () => {
	closeDialog();
};
// 提交
const onSubmit = () => {

  formRef.value?.validate((valid:boolean, fields:any) => {
    if (valid) {
		frameworkuserApi().edit(state.vmModel).then(()=>{
			ElMessage.success("提交成功");
			closeDialog();
			emit('refresh');
		}).catch((error)=>{
			other.setFormError(ci,error);
		})
    } 
  })
};


// 暴露变量
defineExpose({
	openDialog,
});
</script>
