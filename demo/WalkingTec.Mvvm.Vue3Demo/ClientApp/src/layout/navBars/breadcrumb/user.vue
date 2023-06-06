<template>
	<div class="layout-navbars-breadcrumb-user pr15" :style="{ flex: layoutUserFlexNum }">
		<el-dropdown :show-timeout="70" :hide-timeout="50" trigger="click" @command="onComponentSizeChange">
			<div class="layout-navbars-breadcrumb-user-icon">
				<i class="fa fa-font" :title="$t('message._system.user.title0')"></i>
			</div>
			<template #dropdown>
				<el-dropdown-menu>
					<el-dropdown-item command="large" :disabled="state.disabledSize === 'large'">{{
						$t('message._system.user.dropdownLarge') }}</el-dropdown-item>
					<el-dropdown-item command="default" :disabled="state.disabledSize === 'default'">{{
						$t('message._system.user.dropdownDefault') }}</el-dropdown-item>
					<el-dropdown-item command="small" :disabled="state.disabledSize === 'small'">{{
						$t('message._system.user.dropdownSmall') }}</el-dropdown-item>
				</el-dropdown-menu>
			</template>
		</el-dropdown>
		<el-dropdown :show-timeout="70" :hide-timeout="50" trigger="click" @command="onLanguageChange">
			<div class="layout-navbars-breadcrumb-user-icon">
				<i class="fa fa-globe" :title="$t('message._system.user.title1')"></i>
			</div>
			<template #dropdown>
				<el-dropdown-menu>
					<el-dropdown-item command="zh-cn" :disabled="state.disabledI18n === 'zh-cn'">简体中文</el-dropdown-item>
					<el-dropdown-item command="en" :disabled="state.disabledI18n === 'en'">English</el-dropdown-item>
				</el-dropdown-menu>
			</template>
		</el-dropdown>
		<div class="layout-navbars-breadcrumb-user-icon" @click="onLayoutSetingClick">
			<i class="fa fa-shirt" :title="$t('message._system.user.title3')"></i>
		</div>
		<!-- <div class="layout-navbars-breadcrumb-user-icon">
			<el-popover placement="bottom" trigger="click" transition="el-zoom-in-top" :width="300" :persistent="false">
				<template #reference>
					<el-badge :is-dot="true">
						<i class="fa fa-bell" :title="$t('message._system.user.title4')">
							
						</i>
					</el-badge>
				</template>
				<template #default>
					<UserNews />
				</template>
			</el-popover>
		</div> -->
		<div class="layout-navbars-breadcrumb-user-icon" @click="onScreenfullClick">
			<i class="iconfont"
				:title="state.isScreenfull ? $t('message._system.user.title6') : $t('message._system.user.title5')"
				:class="!state.isScreenfull ? 'fa fa-expand' : 'fa fa-compress'"></i>
		</div>
		<el-dropdown style="padding-right: 10px;" v-if="state.allTenant.length>0" :show-timeout="70" :hide-timeout="50" @command="onHandleCommandClick">
			<span>{{state.allTenant.filter(x=>x.disabled==true)[0].Text}}</span>
			<template #dropdown>
				<el-dropdown-menu >
					<el-dropdown-item v-for="item in state.allTenant" :disabled="item.disabled" :command="'settenant|'+item.Value">{{ item.Text }}</el-dropdown-item>
				</el-dropdown-menu>
			</template>
		</el-dropdown>
		<el-dropdown :show-timeout="70" :hide-timeout="50" @command="onHandleCommandClick">
			<span class="layout-navbars-breadcrumb-user-link">
				<img v-if="userInfos.photo!=''" :src="userInfos.photo" class="layout-navbars-breadcrumb-user-link-photo mr5" />
				<i v-else class="fa fa-user mr5" ></i>
				{{ userInfos.userName === '' ? 'User' : userInfos.userName }}
				<el-icon class="el-icon--right">
					<ele-ArrowDown />
				</el-icon>
			</span>
			<template #dropdown>
				<el-dropdown-menu>
					<el-dropdown-item command="/home">{{ $t('message._system.user.dropdown1') }}</el-dropdown-item>
					<el-dropdown-item command="changepw">{{ $t('message._system.user.dropdown2')}}</el-dropdown-item>
					<el-dropdown-item v-if="userInfos.isDebug" command="codegen">{{ $t('message._system.user.dropdown3') }}</el-dropdown-item>
					<el-dropdown-item v-if="userInfos.isDebug" command="swagger">{{ $t('message._system.user.dropdown4') }}</el-dropdown-item>
					<el-dropdown-item divided command="logOut">{{ $t('message._system.user.dropdown5') }}</el-dropdown-item>
				</el-dropdown-menu>
			</template>
		</el-dropdown>
		<Search ref="searchRef" />
	</div>
</template>

<script setup lang="ts" name="layoutBreadcrumbUser">
import { defineAsyncComponent, ref, computed, reactive, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { ElMessageBox, ElMessage } from 'element-plus';
import screenfull from 'screenfull';
import { useI18n } from 'vue-i18n';
import { storeToRefs } from 'pinia';
import { useUserInfo } from '/@/stores/userInfo';
import { useThemeConfig } from '/@/stores/themeConfig';
import other from '/@/utils/other';
import mittBus from '/@/utils/mitt';
import { Session, Local } from '/@/utils/storage';
import frameworktenantApi from '/@/api/frameworktenant';

// 引入组件
const UserNews = defineAsyncComponent(() => import('/@/layout/navBars/breadcrumb/userNews.vue'));
const Search = defineAsyncComponent(() => import('/@/layout/navBars/breadcrumb/search.vue'));
const cpwDialog = defineAsyncComponent(() => import('/@/views/login/changePassword.vue'));

// 定义变量内容
const { locale, t } = useI18n();
const router = useRouter();
const stores = useUserInfo();
const storesThemeConfig = useThemeConfig();
const { userInfos } = storeToRefs(stores);
const { themeConfig } = storeToRefs(storesThemeConfig);
const searchRef = ref();
const state = reactive({
	isScreenfull: false,
	disabledI18n: 'zh-cn',
	disabledSize: 'large',
	allTenant:[] as any[]
});

// 设置分割样式
const layoutUserFlexNum = computed(() => {
	let num: string | number = '';
	const { layout, isClassicSplitMenu } = themeConfig.value;
	const layoutArr: string[] = ['defaults', 'columns'];
	if (layoutArr.includes(layout) || (layout === 'classic' && !isClassicSplitMenu)) num = '1';
	else num = '';
	return num;
});
// 全屏点击时
const onScreenfullClick = () => {
	if (!screenfull.isEnabled) {
		ElMessage.warning('暂不不支持全屏');
		return false;
	}
	screenfull.toggle();
	screenfull.on('change', () => {
		if (screenfull.isFullscreen) state.isScreenfull = true;
		else state.isScreenfull = false;
	});
};
// 布局配置 icon 点击时
const onLayoutSetingClick = () => {
	mittBus.emit('openSetingsDrawer');
};
// 下拉菜单点击时
const onHandleCommandClick = (path: string) => {
	if (path === 'logOut') {
		ElMessageBox({
			closeOnClickModal: false,
			closeOnPressEscape: false,
			title: t('message._system.user.logOutTitle'),
			message: t('message._system.user.logOutMessage'),
			showCancelButton: true,
			confirmButtonText: t('message._system.user.logOutConfirm'),
			cancelButtonText: t('message._system.user.logOutCancel'),
			buttonSize: 'default',
			beforeClose: (action, instance, done) => {
				if (action === 'confirm') {
					instance.confirmButtonLoading = true;
					instance.confirmButtonText = t('message._system.user.logOutExit');
					setTimeout(() => {
						done();
						setTimeout(() => {
							instance.confirmButtonLoading = false;
						}, 300);
					}, 700);
				} else {
					done();
				}
			},
		})
			.then(async () => {
				// 清除缓存/token等
				Session.clear();
				Local.remove('userinfo');
				Local.remove('token');
				// 使用 reload 时，不需要调用 resetRoute() 重置路由
				window.location.reload();
			})
			.catch(() => { });
	} else if (path === 'codegen') {
        window.open(import.meta.env.VITE_API_URL + '/_codegen?ui=vue3');
	}
	else if (path === 'swagger') {
		window.open(import.meta.env.VITE_API_URL + '/swagger');
	} 
	else if(path.startsWith("settenant|")){
		const v = path.split('|')[1];
		frameworktenantApi().setTenant(v).then(res=>{
		window.location.reload();
		});
	}
	else if(path === 'changepw'){
		other.openDialog(t('message._system.user.dropdown2'),cpwDialog,{},null,400)
	}
	else {
		router.push(path);
	}
};

// 组件大小改变
const onComponentSizeChange = (size: string) => {
	Local.remove('themeConfig');
	themeConfig.value.globalComponentSize = size;
	Local.set('themeConfig', themeConfig.value);
	initI18nOrSize('globalComponentSize', 'disabledSize');
	window.location.reload();
};
// 语言切换
const onLanguageChange = (lang: string) => {
	Local.remove('themeConfig');
	themeConfig.value.globalI18n = lang;
	Local.set('themeConfig', themeConfig.value);
	// locale.value = lang;
	// other.useTitle();
	// initI18nOrSize('globalI18n', 'disabledI18n');
	window.location.reload();
};
// 初始化组件大小/i18n
const initI18nOrSize = (value: string, attr: string) => {
	(<any>state)[attr] = Local.get('themeConfig')[value];
};
// 页面加载时
onMounted(() => {
	if (Local.get('themeConfig')) {
		initI18nOrSize('globalComponentSize', 'disabledSize');
		initI18nOrSize('globalI18n', 'disabledI18n');
	}
	frameworktenantApi().getFrameworkTenants(userInfos.value.tenantCode).then((res:any)=>{
		state.allTenant = [];
		state.allTenant.push({Text:t('message._system.common.vm.mainsite'), Value:userInfos.value.tenantCode});
		state.allTenant.push(...res);
		state.allTenant.forEach(element => {
			if(element.Value == userInfos.value.currentTenant){
				element.disabled = true
			}
			else{
				element.disabled = false;
			}
		});
	})
});
</script>

<style scoped lang="scss">
.layout-navbars-breadcrumb-user {
	display: flex;
	align-items: center;
	justify-content: flex-end;

	&-link {
		height: 100%;
		display: flex;
		align-items: center;
		white-space: nowrap;

		&-photo {
			width: 25px;
			height: 25px;
			border-radius: 100%;
		}
	}

	&-icon {
		padding: 0 10px;
		cursor: pointer;
		color: var(--next-bg-topBarColor);
		height: 50px;
		line-height: 50px;
		display: flex;
		align-items: center;

		&:hover {
			background: var(--next-color-user-hover);

			i {
				display: inline-block;
				animation: logoAnimation 0.3s ease-in-out;
			}
		}
	}

	:deep(.el-dropdown) {
		color: var(--next-bg-topBarColor);
	}

	:deep(.el-badge) {
		height: 40px;
		line-height: 40px;
		display: flex;
		align-items: center;
	}

	:deep(.el-badge__content.is-fixed) {
		top: 12px;
	}
}</style>
