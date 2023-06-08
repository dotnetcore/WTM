<template>
	<el-form size="large" class="login-content-form">
		<el-form-item class="login-animation1">
			<el-input text :placeholder="'testtest'" v-model="state.ruleForm.Account"
				clearable autocomplete="off">
				<template #prefix>
					<i class="fa fa-user"></i>
				</template>
			</el-input>
		</el-form-item>
		<el-form-item class="login-animation2">
			<el-input :type="state.isShowPassword ? 'text' : 'password'"
				:placeholder="$t('message._admin.account.accountPlaceholder2')" v-model="state.ruleForm.Password"
				autocomplete="off" show-password clearable>
				<template #prefix>
					<i class="fa fa-lock"></i>
				</template>
				<!-- <template #suffix>
					<i class="iconfont el-input__icon login-content-password"
						:class="state.isShowPassword ? 'icon-yincangmima' : 'icon-xianshimima'"
						@click="state.isShowPassword = !state.isShowPassword">
					</i>
				</template> -->
			</el-input>
		</el-form-item>
		<el-form-item class="login-animation3">
			<el-input text maxlength="4" :placeholder="$t('message._admin.account.accountPlaceholder3')"
				v-model="state.ruleForm.Tenant" clearable autocomplete="off">
				<template #prefix>
					<i class="fa fa-store"></i>
				</template>
			</el-input>
		</el-form-item>
		<el-form-item class="login-animation4">
			<el-button type="primary" class="login-content-submit" round v-waves @click="onSignIn"
				:loading="state.loading.signIn">
				<span>{{ $t('message._admin.account.accountBtnText') }}</span>
			</el-button>
		</el-form-item>
	</el-form>
</template>

<script setup lang="ts" name="loginAccount">
import { reactive, computed,getCurrentInstance,onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { ElMessage } from 'element-plus';
import loginapi from '/@/api/login';
import Cookies from 'js-cookie';
import { storeToRefs } from 'pinia';
import { useThemeConfig } from '/@/stores/themeConfig';
import { initFrontEndControlRoutes } from '/@/router/frontEnd';
import { initBackEndControlRoutes } from '/@/router/backEnd';
import { Session,Local } from '/@/utils/storage';
import { formatAxis } from '/@/utils/formatTime';
import { NextLoading } from '/@/utils/loading';

// 定义变量内容
const storesThemeConfig = useThemeConfig();
const { themeConfig } = storeToRefs(storesThemeConfig);
const route = useRoute();
const router = useRouter();
const state = reactive({
	isShowPassword: false,
	ruleForm: {
		Account: 'admin',
		Password: '000000',
		Tenant: '',
	},
	loading: {
		signIn: false,
	},
});
const ci = getCurrentInstance() as any;
// 时间获取
const currentTime = computed(() => {
	return formatAxis(new Date());
});
// 登录
const onSignIn = async () => {
	state.loading.signIn = true;

	loginapi().signIn(state.ruleForm).then(async res=>{
		Local.set('token',res.access_token);
		const isNoPower = await initBackEndControlRoutes();
		signInSuccess(isNoPower);
	}).catch(()=>{
		state.loading.signIn = false;
	})
	
};
// 登录成功后的跳转
const signInSuccess = (isNoPower: boolean | undefined) => {
	if (isNoPower) {		
		Local.clear();
	} else {
		// 初始化登录成功时间问候语
		let currentTimeInfo = currentTime.value;
		// 登录成功，跳到转首页
		// 如果是复制粘贴的路径，非首页/登录页，那么登录成功后重定向到对应的路径中
		if (route.query?.redirect) {
			router.push({
				path: <string>route.query?.redirect,
				query: Object.keys(<string>route.query?.params).length > 0 ? JSON.parse(<string>route.query?.params) : '',
			});
		} else {
			router.push('/');
		}
		// 登录成功提示
		const signInText = ci.proxy.$t('message._admin.signInText');
		ElMessage.success(`${currentTimeInfo}，${signInText}`);
		// 添加 loading，防止第一次进入界面时出现短暂空白
		NextLoading.start();
	}
	state.loading.signIn = false;
};
onMounted(()=>{
	if (useRouter().currentRoute.value.query._remotetoken){
		loginapi().signInSSO(useRouter().currentRoute.value.query._remotetoken).then(async res=>{
		Local.set('token',res.access_token);
		const isNoPower = await initBackEndControlRoutes();
		signInSuccess(isNoPower);
	}).catch(()=>{
		state.loading.signIn = false;
	})
	}
})
</script>

<style scoped lang="scss">
.login-content-form {
	margin-top: 20px;

	@for $i from 1 through 4 {
		.login-animation#{$i} {
			opacity: 0;
			animation-name: error-num;
			animation-duration: 0.5s;
			animation-fill-mode: forwards;
			animation-delay: calc($i/10) + s;
		}
	}

	.login-content-password {
		display: inline-block;
		width: 20px;
		cursor: pointer;

		&:hover {
			color: #909399;
		}
	}

	.login-content-code {
		width: 100%;
		padding: 0;
		font-weight: bold;
		letter-spacing: 5px;
	}

	.login-content-submit {
		width: 100%;
		letter-spacing: 2px;
		font-weight: 300;
		margin-top: 15px;
	}
}
</style>
