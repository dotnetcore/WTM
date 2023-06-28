<template>
	<div>
		<!-- <el-dialog title="新建用户" v-model="state.dialog.isShowDialog" draggable> -->
			<el-form ref="formRef" :model="state.vmModel"  label-width="90px" >
				<el-tabs type="border-card">
					<el-tab-pane label="基础信息">
						<el-row >
							<el-col :xs="24" :lg="24"  class="mb20">
								<el-form-item ref="Entity_PhotoId_FormItem" prop="Entity.PhotoId" label="头像" >
									<WtmUploadImage  v-model="state.vmModel.Entity.PhotoId"  :multi="true"/>
								</el-form-item>
							</el-col>
						</el-row>
						<el-row >
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item ref="Entity_ITCode_FormItem" prop="Entity.ITCode" :label="$t('message._system.user.vm.ITCode')" :rules="[{ required: true, message: $t('message._system.user.placeholder.ITCode'), trigger: 'blur' }]">
									<el-input v-model="state.vmModel.Entity.ITCode" :placeholder="$t('message._system.user.placeholder.ITCode')" clearable></el-input>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item ref="Entity_Password_FormItem" prop="Entity.Password" label="密码" :rules="[{ required: true, message: '请输入密码', trigger: 'blur' }]">
									<el-input v-model="state.vmModel.Entity.Password" type="password" show-password placeholder="请输入密码" clearable></el-input>
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
									<el-select v-model="state.vmModel.Entity.Gender" clearable>
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
			<footer class="el-dialog__footer" style="padding: unset;padding-top: 10px;">
			<span class="dialog-footer">
					<el-button @click="onCancel" >取消</el-button>
					<el-button type="primary" @click="onSubmit" >提交</el-button>
				</span>
			</footer>
	</div>
</template>

<script setup lang="ts" name="message._system.common.vm.add;false">
import { ElMessage } from 'element-plus';
import { reactive, ref, getCurrentInstance, onMounted} from 'vue';
import { useI18n } from 'vue-i18n';
import frameworkuserApi from '/@/api/frameworkuser';
import other from '/@/utils/other';
// 定义子组件向父组件传值/事件
const emit = defineEmits(['refresh','closeDialog']);
const { t } = useI18n();
// 定义变量内容
const formRef = ref();
const state = reactive({
	vmModel: {
		Entity:{
			ITCode:"123",
			Password:null,
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

onMounted(()=>{
	other.getSelectList('/api/_FrameworkUser/GetFrameworkRoles',[],false).then(x=>{state.allRoles = x});
	other.getSelectList('/api/_FrameworkUser/GetFrameworkGroupsTree',[],true).then(x=>{state.allGroups = x});
});

// 关闭弹窗
const closeDialog = () => {
	emit('closeDialog');
};
// 取消
const onCancel = () => {
	closeDialog();
};
// 提交
const onSubmit = () => {	
  formRef.value?.validate((valid:boolean, fields:any) => {
	 if (valid) {
		frameworkuserApi().add(state.vmModel).then(()=>{
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
});
</script>
