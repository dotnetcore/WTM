<template>
	<div class="card-fill layout-padding">
		<el-card shadow="hover" class="layout-padding-auto">
			<el-form ref="formRef" :model="state.vmModel" label-width="90px">
				<el-row>
					<el-col :xs="24" :lg="12"  class="mb20">
						<el-form-item ref="Entity_ITCode_FormItem" prop="Entity.ITCode" :label="$t('message._admin.actionlog.vm.ITCode')">
							<el-input v-model="state.vmModel.Entity.ITCode" clearable disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :lg="12"  class="mb20">
						<el-form-item ref="Entity_LogType_FormItem" prop="Entity.LogType" :label="$t('message._admin.actionlog.vm.LogType')">
							<el-select v-model="state.vmModel.Entity.LogType" disabled>
								<el-option key="0" value="Normal" :label="$t('message._admin.actionlog.vm.LogType_0')"></el-option>
								<el-option key="1" value="Exception" :label="$t('message._admin.actionlog.vm.LogType_1')"></el-option>
								<el-option key="2" value="Debug" :label="$t('message._admin.actionlog.vm.LogType_2')"></el-option>
								<el-option key="3" value="Job" :label="$t('message._admin.actionlog.vm.LogType_3')"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :xs="24" :lg="12"  class="mb20">
						<el-form-item ref="Entity_ActionUrl_FormItem" prop="Entity.ActionUrl" :label="$t('message._admin.actionlog.vm.ActionUrl')">
							<el-input v-model="state.vmModel.Entity.ActionUrl" clearable disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :lg="12"  class="mb20">
						<el-form-item ref="Entity_ActionTime_FormItem" prop="Entity.ActionTime" :label="$t('message._admin.actionlog.vm.ActionTime')">
							<el-input v-model="state.vmModel.Entity.ActionTime" clearable disabled> </el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :xs="24" :lg="12"  class="mb20">
						<el-form-item ref="Entity_ActionName_FormItem" prop="Entity.ActionName" :label="$t('message._admin.actionlog.vm.ActionName')">
							<el-input v-model="state.vmModel.Entity.ActionName" clearable disabled> </el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :lg="12"  class="mb20">
						<el-form-item ref="Entity_Duration_FormItem" prop="Entity.Duration" :label="$t('message._admin.actionlog.vm.Duration')">
							<el-input v-model="state.vmModel.Entity.Duration" clearable disabled> </el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :xs="24" :lg="12"  class="mb20">
						<el-form-item ref="Entity_ModuleName_FormItem" prop="Entity.ModuleName" :label="$t('message._admin.actionlog.vm.ModuleName')">
							<el-input v-model="state.vmModel.Entity.ModuleName" clearable disabled> </el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :lg="12"  class="mb20">
						<el-form-item ref="Entity_IP_FormItem" prop="Entity.IP" :label="$t('message._admin.actionlog.vm.IP')">
							<el-input v-model="state.vmModel.Entity.IP" clearable disabled> </el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :xs="24" :lg="24" class="mb20">
						<el-form-item ref="Entity_Remark_FormItem" prop="Entity.Remark" :label="$t('message._admin.actionlog.vm.Remark')">
							<el-input v-model="state.vmModel.Entity.Remark" type="textarea" clearable disabled></el-input>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<footer class="el-dialog__footer" style="padding: unset;padding-top: 10px;">
				<span class="dialog-footer">
					<el-button @click="onCancel">{{ $t('message._system.common.vm.cancel') }}</el-button>
				</span>
			</footer>
		</el-card>
	</div>
</template>

<script setup lang="ts" name="message._system.common.vm.detail;false">
import { ElMessage } from 'element-plus';
import { array } from 'snabbdom';
import { reactive, ref, getCurrentInstance, onMounted } from 'vue';
import actionlogApi from '/@/api/actionlog';
import other from '/@/utils/other';
import { useRouter } from "vue-router";

// 定义子组件向父组件传值/事件
const emit = defineEmits(['refresh', 'closeDialog']);

// 定义变量内容
const formRef = ref();
const state = reactive({
	vmModel: {
		Entity: {
			ID: null,
			ITCode: null,
			ActionUrl: null,
			ActionTime: null,
			IP: null,
			LogType: null,
			ActionName: null,
			Duration: null,
			ModuleName: null,
			Remark:null
		}

	}
});
const ci = getCurrentInstance() as any;
onMounted(() => {
	if (ci.attrs["wtmdata"]) {
		state.vmModel.Entity.ID = ci.attrs["wtmdata"].ID;
	}
	else if (useRouter().currentRoute.value.query.id) {
		state.vmModel.Entity.ID = useRouter().currentRoute.value.query.id as any;
	}
	actionlogApi().get(state.vmModel.Entity.ID ?? "").then((data: any) => other.setValue(state.vmModel, data));
});
// 关闭弹窗
const closeDialog = () => {
	emit('closeDialog');
};
// 取消
const onCancel = () => {
	closeDialog();
};

// 暴露变量
defineExpose({
	
});
</script>
