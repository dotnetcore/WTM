<template>
	<div class="upgrade-dialog">
		<el-dialog
			v-model="state.isUpgrade"
			width="300px"
			destroy-on-close
			:show-close="false"
			:close-on-click-modal="false"
			:close-on-press-escape="false"
		>
			<div class="upgrade-title">
				<div class="upgrade-title-warp">
					<span class="upgrade-title-warp-txt">{{ $t('message._system.upgrade.title') }}</span>
					<span class="upgrade-title-warp-version">v{{ state.version }}</span>
				</div>
			</div>
			<div class="upgrade-content">
				{{ getThemeConfig.globalTitle }} {{ $t('message._system.upgrade.msg') }}
				<div class="mt5">
					<el-link type="primary" class="font12" href="https://gitee.com/lyt-top/vue-next-admin/blob/master/CHANGELOG.md" target="_black">
						CHANGELOG.md
					</el-link>
				</div>
				<div class="upgrade-content-desc mt5">{{ $t('message._system.upgrade.desc') }}</div>
			</div>
			<div class="upgrade-btn">
				<el-button round type="info" text @click="onCancel">{{ $t('message._system.upgrade.btnOne') }}</el-button>
				<el-button type="primary" round  @click="onUpgrade" :loading="state.isLoading">{{ state.btnTxt }}</el-button>
			</div>
		</el-dialog>
	</div>
</template>

<script setup lang="ts" name="layoutUpgrade">
import { reactive, computed, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { storeToRefs } from 'pinia';
import { useThemeConfig } from '/@/stores/themeConfig';
import { Local } from '/@/utils/storage';

// 定义变量内容
const { t } = useI18n();
const storesThemeConfig = useThemeConfig();
const { themeConfig } = storeToRefs(storesThemeConfig);
const state = reactive({
	isUpgrade: false,
	// @ts-ignore
	version: __NEXT_VERSION__,
	isLoading: false,
	btnTxt: '',
});

// 获取布局配置信息
const getThemeConfig = computed(() => {
	return themeConfig.value;
});
// 残忍拒绝
const onCancel = () => {
	state.isUpgrade = false;
};
// 马上更新
const onUpgrade = () => {
	state.isLoading = true;
	state.btnTxt = t('message._system.upgrade.btnTwoLoading');
	setTimeout(() => {
		Local.clear();
		window.location.reload();
		Local.set('version', state.version);
	}, 2000);
};
// 延迟显示，防止刷新时界面显示太快
const delayShow = () => {
	setTimeout(() => {
		state.isUpgrade = true;
	}, 2000);
};
// 页面加载时
onMounted(() => {
	delayShow();
	setTimeout(() => {
		state.btnTxt = t('message._system.upgrade.btnTwo');
	}, 200);
});
</script>

<style scoped lang="scss">
.upgrade-dialog {
	:deep(.el-dialog) {
		.el-dialog__body {
			padding: 0 !important;
		}
		.el-dialog__header {
			display: none !important;
		}
		.upgrade-title {
			text-align: center;
			height: 130px;
			display: flex;
			align-items: center;
			justify-content: center;
			position: relative;
			&::after {
				content: '';
				position: absolute;
				background-color: var(--el-color-primary-light-1);
				width: 130%;
				height: 130px;
				border-bottom-left-radius: 100%;
				border-bottom-right-radius: 100%;
			}
			.upgrade-title-warp {
				z-index: 1;
				position: relative;
				.upgrade-title-warp-txt {
					color: var(--next-color-white);
					font-size: 22px;
					letter-spacing: 3px;
				}
				.upgrade-title-warp-version {
					color: var(--next-color-white);
					background-color: var(--el-color-primary-light-4);
					font-size: 12px;
					position: absolute;
					display: flex;
					top: -2px;
					right: -50px;
					padding: 2px 4px;
					border-radius: 2px;
				}
			}
		}
		.upgrade-content {
			padding: 20px;
			line-height: 22px;
			.upgrade-content-desc {
				color: var(--el-color-info-light-5);
				font-size: 12px;
			}
		}
		.upgrade-btn {
			border-top: 1px solid var(--el-border-color-lighter, #ebeef5);
			display: flex;
			justify-content: space-around;
			padding: 15px 20px;
			.el-button {
				width: 100%;
			}
		}
	}
}
</style>
