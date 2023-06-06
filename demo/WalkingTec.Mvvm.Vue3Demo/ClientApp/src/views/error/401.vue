<template>
	<div class="error layout-padding">
		<div class="layout-padding-auto layout-padding-view">
			<div class="error-flex">
				<div class="left">
					<div class="left-item">
						<div class="left-item-animation left-item-num">401</div>
						<div class="left-item-animation left-item-title">{{ $t('message._system.noAccess.accessTitle') }}</div>
						<div class="left-item-animation left-item-msg">{{ $t('message._system.noAccess.accessMsg') }}</div>
						<div class="left-item-animation left-item-btn">
							<el-button type="primary" round @click="onSetAuth">{{ $t('message._system.noAccess.accessBtn') }}</el-button>
						</div>
					</div>
				</div>
				<div class="right">
					
				</div>
			</div>
		</div>
	</div>
</template>

<script setup lang="ts" name="noPower">
import { Session } from '/@/utils/storage';

const onSetAuth = () => {
	// https://gitee.com/lyt-top/vue-next-admin/issues/I5C3JS
	// 清除缓存/token等
	Session.clear();
	// 使用 reload 时，不需要调用 resetRoute() 重置路由
	window.location.reload();
};
</script>

<style scoped lang="scss">
.error {
	height: 100%;
	.error-flex {
		margin: auto;
		display: flex;
		height: 350px;
		width: 900px;
		.left {
			flex: 1;
			height: 100%;
			align-items: center;
			display: flex;
			.left-item {
				.left-item-animation {
					opacity: 0;
					animation-name: error-num;
					animation-duration: 0.5s;
					animation-fill-mode: forwards;
				}
				.left-item-num {
					color: var(--el-color-info);
					font-size: 55px;
				}
				.left-item-title {
					font-size: 20px;
					color: var(--el-text-color-primary);
					margin: 15px 0 5px 0;
					animation-delay: 0.1s;
				}
				.left-item-msg {
					color: var(--el-text-color-secondary);
					font-size: 12px;
					margin-bottom: 30px;
					animation-delay: 0.2s;
				}
				.left-item-btn {
					animation-delay: 0.2s;
				}
			}
		}
		.right {
			flex: 1;
			opacity: 0;
			animation-name: error-img;
			animation-duration: 2s;
			animation-fill-mode: forwards;
			img {
				width: 100%;
				height: 100%;
			}
		}
	}
}
</style>
