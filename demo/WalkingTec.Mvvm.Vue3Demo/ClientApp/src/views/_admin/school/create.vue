<template>
	<div class="card-fill layout-padding">
		<el-card shadow="hover" class="layout-padding-auto" >
		
			<el-form ref="formRef" :model="state.vmModel" label-width="100px">
				<el-row>
					<el-col :xs="24"  :lg="12"  class="mb20">
						<el-form-item ref="Entity_SchoolCode_FormItem" prop="Entity.SchoolCode"
							label="编号"
							:rules="[{ required: true, message: $t('message._admin.group.req.GroupCode'), trigger: 'blur' }]">
							<el-input v-model="state.vmModel.Entity.SchoolCode" clearable></el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24"  :lg="12"  class="mb20">
						<el-form-item ref="Entity_SchoolName_FormItem" prop="Entity.SchoolName"
							label="名称"
							:rules="[{ required: true, message: $t('message._admin.group.req.GroupName'), trigger: 'blur' }]">
							<el-input v-model="state.vmModel.Entity.SchoolName" clearable></el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24"  :lg="24"  class="mb20">
						<el-form-item ref="Entity_Remark_FormItem" prop="Entity.Remark"
							label="Remark">
							<WtmEditor v-model="state.vmModel.Entity.Remark" ></WtmEditor>
						</el-form-item>
					</el-col>
				</el-row>				
				<WtmTable
				ref="tableRef"
				v-bind="tableData"
				v-model="state.vmModel.Entity.Majors"
			>
		</WtmTable>			
			</el-form>
			<footer class="el-dialog__footer" style="padding: unset;padding-top: 10px;">
			<span class="dialog-footer">
				<el-button type="primary" @click="onSubmit">{{ $t('message._system.common.vm.submit') }}</el-button>
					<el-button @click="onCancel">{{ $t('message._system.common.vm.cancel') }}</el-button>
				</span>
			</footer>
		</el-card>
	</div>
</template>

<script setup lang="ts" name="message._system.common.vm.add;false">
import { ElMessage } from 'element-plus';
import { reactive, ref, getCurrentInstance, onMounted, nextTick } from 'vue';
import schoolApi from '/@/api/school';
import other from '/@/utils/other';

const ci = getCurrentInstance() as any;
// 定义子组件向父组件传值/事件
const emit = defineEmits(['refresh','closeDialog']);
// 定义变量内容
const formRef = ref();
const tableRef = ref();
const state = reactive({
	vmModel: {
		Entity: {
			SchoolCode: null,
			SchoolName: null,
			Remark:null,
			Majors:[],
		},
	}
});
const tableData = ref({
	// 列表数据（必传）
	data: [],
	// 表头内容（必传，注意格式）
	header: [
		{ key: 'MajorName', colWidth: '', title: '专业名称', type: 'textbox', isCheck: true },
		{ key: 'MajorCode', colWidth: '', title: '专业编号', type: 'textbox', isCheck: true },
		{ key: 'MajorType', colWidth:'', title:'专业类别', type:'combobox',isCheck:true,comboData:{Required:'必修',Optional:'选修'}},
		{ key: 'CityId', colWidth:'', title:'url测试', type:'combobox',isCheck:true,comboData:'/api/city/GetCitys'},
		{ key: 'test', colWidth: '250', title: 'test', type: 'date', isCheck: true },
	],
	// 配置项（必传）
	config: {
		total: 0, // 列表总数
		loading: false, // loading 加载
		isSerialNo: true, // 是否显示表格序号
		isSelection: false, // 是否显示表格多选
		isOperate: true, // 是否显示表格操作栏
		isSub:true, //是否子表控件
	}
});
// 打开弹窗
onMounted(() => {	
	
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
	formRef.value?.validate((valid: boolean, fields: any) => {
		if (valid) {
			schoolApi().add(state.vmModel).then(() => {
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
</script>
