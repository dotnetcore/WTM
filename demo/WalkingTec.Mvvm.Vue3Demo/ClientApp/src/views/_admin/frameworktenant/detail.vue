<template>
	<div class="card-fill layout-padding">
		<el-card shadow="hover" class="layout-padding-auto" >
			<el-form ref="formRef" :model="state.vmModel" label-width="100px">
				<el-row>
					<el-col :xs="24"  :lg="12" class="mb20">
						<el-form-item ref="Entity_TCode_FormItem" prop="Entity.TCode" :label="$t('message._admin.tenant.vm.TCode')"
							:rules="[{ required: true, message: $t('message._admin.tenant.req.TCode'), trigger: 'blur' }]">
							<el-input v-model="state.vmModel.Entity.TCode" clearable disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24"  :lg="12" class="mb20">
						<el-form-item ref="Entity_TName_FormItem" prop="Entity.TName" :label="$t('message._admin.tenant.vm.TName')"
							:rules="[{ required: true, message: $t('message._admin.tenant.req.TName'), trigger: 'blur' }]">
							<el-input v-model="state.vmModel.Entity.TName" clearable disabled></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :xs="24"  :lg="12" class="mb20">
						<el-form-item ref="Entity_TDb_FormItem" prop="Entity.TDb" :label="$t('message._admin.tenant.vm.TDb')">
							<el-input v-model="state.vmModel.Entity.TDb" clearable disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24"  :lg="12" class="mb20">
						<el-form-item ref="Entity_TDb_FormItem" prop="Entity.TDbType"
							:label="$t('message._admin.tenant.vm.TDbType')">
							<el-select v-model="state.vmModel.Entity.TDbType" clearable disabled>
								<el-option key="SqlServer" value="SqlServer" label="SqlServer"></el-option>
								<el-option key="MySql" value="MySql" label="MySql"></el-option>
								<el-option key="PgSql" value="PgSql" label="PgSql"></el-option>
								<el-option key="Memory" value="Memory" label="Memory"></el-option>
								<el-option key="SQLite" value="SQLite" label="SQLite"></el-option>
								<el-option key="Oracle" value="Oracle" label="Oracle"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :xs="24"  :lg="12" class="mb20">
						<el-form-item ref="Entity_DbContext_FormItem" prop="Entity.DbContext"
							:label="$t('message._admin.tenant.vm.DbContext')">
							<el-input v-model="state.vmModel.Entity.DbContext" clearable disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24"  :lg="12" class="mb20">
						<el-form-item ref="Entity_TDomain_FormItem" prop="Entity.TDomain"
							:label="$t('message._admin.tenant.vm.TDomain')">
							<el-input v-model="state.vmModel.Entity.TDomain" clearable disabled></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :xs="24"  :lg="12" class="mb20">
						<el-form-item ref="Entity_EnableSub_FormItem" prop="Entity.EnableSub"
							:label="$t('message._admin.tenant.vm.EnableSub')">
							<el-switch v-model="state.vmModel.Entity.EnableSub" clearable disabled></el-switch>
						</el-form-item>
					</el-col>
					<el-col :xs="24"  :lg="12" class="mb20">
						<el-form-item ref="Entity_Enabled_FormItem" prop="Entity.Enabled"
							:label="$t('message._admin.tenant.vm.Enabled')">
							<el-switch v-model="state.vmModel.Entity.Enabled" clearable disabled></el-switch>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :xs="24"  :lg="12" class="mb20">
						<el-form-item ref="AdminRoleCode_FormItem" prop="AdminRoleCode"
							:label="$t('message._admin.tenant.vm.AdminRoleCode')">
							<el-select v-model="state.vmModel.AdminRoleCode" clearable disabled>
								<el-option v-for="item in state.allRoles" :key="item.Value" :value="item.Value"
									:label="item.Text">{{
										item.Text
									}}</el-option>
							</el-select>
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
import { reactive, ref, getCurrentInstance,onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import frameworktenantApi from '/@/api/frameworktenant';
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
			TCode: null,
			TName: null,
			TDb: null,
			TDbType: null,
			DbContext: null,
			TDomain: null,
			EnableSub: null,
			Enabled: null
		},
		AdminRoleCode: null
	},
	allRoles: [] as any[]
});
const ci = getCurrentInstance() as any;

onMounted(() => {
	if (ci.attrs["wtmdata"]) {
		state.vmModel.Entity.ID = ci.attrs["wtmdata"].ID;
	}
	else if (useRouter().currentRoute.value.query.id) {
		state.vmModel.Entity.ID = useRouter().currentRoute.value.query.id as any;
	}
	frameworktenantApi().get(state.vmModel.Entity.ID ?? "").then((data:any)=>other.setValue(state.vmModel,data));
	other.getSelectList('/api/_account/GetFrameworkRoles', [], false).then(x => { state.allRoles = x });
});

// 关闭弹窗
const closeDialog = () => {
	emit('closeDialog');
};
// 取消
const onCancel = () => {
	emit('closeDialog');
};

// 暴露变量
defineExpose({
});
</script>
