<template>
	<div class="card-fill layout-padding">
		<el-card shadow="hover" class="layout-padding-auto">
			<el-form ref="formRef" :model="state.vmModel" label-width="100px">
				<el-row>
					<el-col :xs="24" :lg="24" class="mb20">
						<el-form-item ref="Entity_IsInside_FormItem" prop="Entity.TCode"
							:label="$t('message._admin.menu.vm.IsInside')">
							<el-radio-group v-model="state.vmModel.Entity.IsInside">
								<el-radio :label=true>{{ $t('message._admin.menu.vm.IsInside_0') }}</el-radio>
								<el-radio :label=false>{{ $t('message._admin.menu.vm.IsInside_1') }}</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>

				</el-row>
				<el-row v-if="state.vmModel.Entity.IsInside == false">
					<el-col :xs="24" :lg="12" class="mb20">
						<el-form-item ref="Entity_Url_FormItem" prop="Entity.Url" :label="$t('message._admin.menu.vm.Url')">
							<el-input v-model="state.vmModel.Entity.Url" clearable></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row v-if="state.vmModel.Entity.IsInside">
					<el-col :xs="24" :lg="24" class="mb20">
						<el-form-item ref="SelectedModule_FormItem" prop="SelectedModule"
							:label="$t('message._admin.menu.vm.SelectedModule')">
							<el-cascader :options="state.menuData"
								:props="{ value: 'classpath', label: 'title', emitPath: false, }"
								clearable class="w100" v-model="state.vmModel.SelectedModule" @change="modelChange">
							</el-cascader>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row v-if="state.vmModel.Entity.IsInside">
					<el-col :xs="24" :lg="24" class="mb20">
						<el-form-item ref="SelectedActionIDs_FormItem" prop="SelectedActionIDs"
							:label="$t('message._admin.menu.vm.SelectedActionIDs')">
							<el-row style="width:100%">
								<el-checkbox v-if="state.allActions.length > 0" v-model="checkAll"
									:indeterminate="isIndeterminate"
									@change="handleCheckAllChange">{{ $t('message._system.common.vm.all') }}</el-checkbox>
							</el-row>
							<el-checkbox-group v-model="state.vmModel.SelectedActionIDs" @change="handleCheckedChange">
								<el-checkbox v-for="item in state.allActions" :label="item.Value">{{
									item.Text
								}}</el-checkbox>
							</el-checkbox-group>
						</el-form-item> </el-col>
				</el-row>

				<el-row>
					<el-col :xs="24" :lg="12" class="mb20">
						<el-form-item ref="Entity_PageName_FormItem" prop="Entity.PageName"
							:label="$t('message._admin.menu.vm.PageName')"
							:rules="[{ required: true, message: $t('message._system.common.vm.input', { input: $t('message._admin.menu.vm.PageName') }), trigger: 'blur' }]">
							<el-input v-model="state.vmModel.Entity.PageName" clearable></el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :lg="12" class="mb20">
						<el-form-item ref="Entity_ParentId_FormItem" prop="Entity.ParentId"
							:label="$t('message._admin.menu.vm.ParentId')">
							<el-select v-model="state.vmModel.Entity.ParentId">
								<el-option v-for="item in state.allParents" :key="item.Value" :value="item.Value"
									:label="item.Text">{{
										item.Text
									}}</el-option>
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :xs="24" :lg="12" class="mb20">
						<el-form-item ref="Entity_DisplayOrder_FormItem" prop="Entity.DisplayOrder"
							:label="$t('message._admin.menu.vm.DisplayOrder')"
							:rules="[{ required: true, message: $t('message._system.common.vm.input', { input: $t('message._admin.menu.vm.DisplayOrder') }), trigger: 'blur' }]">
							<el-input v-model="state.vmModel.Entity.DisplayOrder" clearable></el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :lg="12" class="mb20">
						<el-form-item ref="Entity_TenantAllowed_FormItem" prop="Entity.TenantAllowed"
							:label="$t('message._admin.menu.vm.TenantAllowed')">
							<el-switch v-model="state.vmModel.Entity.TenantAllowed" clearable></el-switch>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :xs="24" :lg="24" class="mb20">
						<el-form-item ref="Entity_DisplayOrder_FormItem" prop="Entity.Icon"
							:label="$t('message._admin.menu.vm.Icon')">
							<IconSelector v-model="state.vmModel.Entity.Icon" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :xs="24" :lg="8" class="mb20">
						<el-form-item ref="Entity_ShowOnMenu_FormItem" prop="Entity.ShowOnMenu"
							:label="$t('message._admin.menu.vm.ShowOnMenu')">
							<el-switch v-model="state.vmModel.Entity.ShowOnMenu" clearable></el-switch>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :lg="8" class="mb20">
						<el-form-item ref="Entity_FolderOnly_FormItem" prop="Entity.FolderOnly"
							:label="$t('message._admin.menu.vm.FolderOnly')">
							<el-switch v-model="state.vmModel.Entity.FolderOnly" clearable></el-switch>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :lg="8" class="mb20">
						<el-form-item ref="Entity_IsPublic_FormItem" prop="Entity.IsPublic"
							:label="$t('message._admin.menu.vm.IsPublic')">
							<el-switch v-model="state.vmModel.Entity.IsPublic" clearable></el-switch>
						</el-form-item>
					</el-col>

				</el-row>
				<el-row>
					<el-col :xs="24" :lg="24" class="mb20">
						<el-form-item ref="SelectedRolesIds_FormItem" prop="SelectedRolesIds"
							:label="$t('message._admin.menu.vm.SelectedRolesIds')">
							<el-select v-model="state.vmModel.SelectedRolesCodes" multiple>
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
					<el-button type="primary" @click="onSubmit">{{ $t('message._system.common.vm.submit') }}</el-button>
					<el-button @click="onCancel">{{ $t('message._system.common.vm.cancel') }}</el-button>
				</span>
			</footer>
		</el-card>
	</div>
</template>

<script setup lang="ts" name="message._system.common.vm.edit;false">
import { ElMessage } from 'element-plus';
import { reactive, ref, getCurrentInstance, onMounted, defineAsyncComponent } from 'vue';
import frameworkmenuApi from '/@/api/frameworkmenu';
import other from '/@/utils/other';
import { storeToRefs } from 'pinia';
import { useRoutesList } from '/@/stores/routesList';
import { useRouter } from "vue-router";
import { GetLocalFile } from '/@/router/backEnd';

const IconSelector = defineAsyncComponent(() => import('/@/components/iconSelector/index.vue'));
const ci = getCurrentInstance() as any;
const stores = useRoutesList();
const { routesList } = storeToRefs(stores);
const checkAll = ref(false)
const isIndeterminate = ref(false)

// 定义子组件向父组件传值/事件
const emit = defineEmits(['refresh', 'closeDialog']);
// 定义变量内容
const formRef = ref();
const state = reactive({
	vmModel: {
		Entity: {
			ID: null,
			Url: '',
			PageName: null,
			ParentId: null,
			TenantAllowed: null,
			DisplayOrder: null,
			Icon: null,
			ShowOnMenu: true,
			FolderOnly: null,
			IsPublic: null,
			IsInside: true,
		},
		IsInside: true,
		SelectedModule: null,
		SelectedActionIDs: [] as any[],
		SelectedRolesCodes: []
	},
	allRoles: [] as any[],
	allParents: [] as any[],
	allActions: [] as any[],
	menuData: [] as RouteItems,
});
// 打开弹窗
onMounted(() => {
	if (ci.attrs["wtmdata"]) {
		state.vmModel.Entity.ID = ci.attrs["wtmdata"].ID;
	}
	else if (useRouter().currentRoute.value.query.id) {
		state.vmModel.Entity.ID = useRouter().currentRoute.value.query.id as any;
	}
    GetLocalFile().then(x => {
        state.menuData = getMenuData(x)
    });
	frameworkmenuApi().get(state.vmModel.Entity.ID ?? "").then((data: any) => {
		other.setValue(state.vmModel, data);
		modelChange(state.vmModel.SelectedModule, false)
	});
	other.getSelectList('/api/_account/GetFrameworkRoles', [], false).then(x => { state.allRoles = x });
	other.getSelectList('/api/_FrameworkMenu/GetFolders', [], false).then(x => { state.allParents = x });
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
			frameworkmenuApi().edit(state.vmModel).then(() => {
				ElMessage.success(ci.proxy.$t('message._system.common.vm.submittip'));
				emit('refresh');
				closeDialog();
			}).catch((error) => {
				other.setFormError(ci, error);
			})
		}
	})
};

const getMenuData = (routes: any) => {
	const arr: RouteItems = [];
	routes.map((val: RouteItem) => {
		if (val.meta?.isHide === false && val.path != '/home') {
			val['title'] = ci.proxy.$t(val.meta?.title as string);
			(<any>val)['classpath'] = val.meta?.className && val.meta?.className != '' ? val.meta?.className : val.path;
			const newvalue = other.deepClone(val);
			newvalue.children = null;
			if (val.children) newvalue.children = getMenuData(val.children);
			arr.push(newvalue);
		}
	});

	return arr;
};
const modelChange = (value: any, reset: boolean = true) => {
	if (reset === true) {
		state.vmModel.SelectedActionIDs = [];
	}
	if (value && value !== '') {
		other.getSelectList('/api/_FrameworkMenu/GetActionsByModel?ModelName=' + value, [], false, "get").then(x => {
			state.allActions = x;
			handleCheckedChange(state.vmModel.SelectedActionIDs);
		});
		state.vmModel.Entity.Url = other.flatTree(state.menuData).find(x => x.classpath == value)?.path;
	}
	else {
		state.allActions = [];
		if(state.vmModel.Entity.IsInside == true){
			state.vmModel.Entity.Url = '';
		}
	}
}
const handleCheckAllChange = (val: boolean) => {
	state.vmModel.SelectedActionIDs = val ? state.allActions.map(x => x.Value) : []
	isIndeterminate.value = false
}
const handleCheckedChange = (value: string[]) => {
	const checkedCount = value.length
	checkAll.value = checkedCount === state.allActions.length
	isIndeterminate.value = checkedCount > 0 && checkedCount < state.allActions.length
}
// 暴露变量
defineExpose({
});
</script>
