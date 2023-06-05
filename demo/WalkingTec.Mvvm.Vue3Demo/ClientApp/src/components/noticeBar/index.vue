<template>
	<div class="notice-bar" :style="{ background, height: `${height}px` }" v-show="!state.isMode">
		<div class="notice-bar-warp" :style="{ color, fontSize: `${size}px` }">
			<i v-if="leftIcon" class="notice-bar-warp-left-icon" :class="leftIcon"></i>
			<div class="notice-bar-warp-text-box" ref="noticeBarWarpRef">
				<div class="notice-bar-warp-text" ref="noticeBarTextRef" v-if="!scrollable">{{ text }}</div>
				<div class="notice-bar-warp-slot" v-else><slot /></div>
			</div>
			<SvgIcon :name="rightIcon" v-if="rightIcon" class="notice-bar-warp-right-icon" @click="onRightIconClick" />
		</div>
	</div>
</template>

<script setup lang="ts" name="noticeBar">
import { reactive, ref, onMounted, nextTick } from 'vue';

// 定义父组件传过来的值
const props = defineProps({
	// 通知栏模式，可选值为 closeable link
	mode: {
		type: String,
		default: () => '',
	},
	// 通知文本内容
	text: {
		type: String,
		default: () => '',
	},
	// 通知文本颜色
	color: {
		type: String,
		default: () => 'var(--el-color-warning)',
	},
	// 通知背景色
	background: {
		type: String,
		default: () => 'var(--el-color-warning-light-9)',
	},
	// 字体大小，单位px
	size: {
		type: [Number, String],
		default: () => 14,
	},
	// 通知栏高度，单位px
	height: {
		type: Number,
		default: () => 40,
	},
	// 动画延迟时间 (s)
	delay: {
		type: Number,
		default: () => 1,
	},
	// 滚动速率 (px/s)
	speed: {
		type: Number,
		default: () => 100,
	},
	// 是否开启垂直滚动
	scrollable: {
		type: Boolean,
		default: () => false,
	},
	// 自定义左侧图标
	leftIcon: {
		type: String,
		default: () => '',
	},
	// 自定义右侧图标
	rightIcon: {
		type: String,
		default: () => '',
	},
});

// 定义子组件向父组件传值/事件
const emit = defineEmits(['close', 'link']);

// 定义变量内容
const noticeBarWarpRef = ref();
const noticeBarTextRef = ref();
const state = reactive({
	order: 1,
	oneTime: 0,
	twoTime: 0,
	warpOWidth: 0,
	textOWidth: 0,
	isMode: false,
});

// 初始化 animation 各项参数
const initAnimation = () => {
	nextTick(() => {
		state.warpOWidth = noticeBarWarpRef.value.offsetWidth;
		state.textOWidth = noticeBarTextRef.value.offsetWidth;
		document.styleSheets[0].insertRule(`@keyframes oneAnimation {0% {left: 0px;} 100% {left: -${state.textOWidth}px;}}`);
		document.styleSheets[0].insertRule(`@keyframes twoAnimation {0% {left: ${state.warpOWidth}px;} 100% {left: -${state.textOWidth}px;}}`);
		computeAnimationTime();
		setTimeout(() => {
			changeAnimation();
		}, props.delay * 1000);
	});
};
// 计算 animation 滚动时长
const computeAnimationTime = () => {
	state.oneTime = state.textOWidth / props.speed;
	state.twoTime = (state.textOWidth + state.warpOWidth) / props.speed;
};
// 改变 animation 动画调用
const changeAnimation = () => {
	if (state.order === 1) {
		noticeBarTextRef.value.style.cssText = `animation: oneAnimation ${state.oneTime}s linear; opactity: 1;}`;
		state.order = 2;
	} else {
		noticeBarTextRef.value.style.cssText = `animation: twoAnimation ${state.twoTime}s linear infinite; opacity: 1;`;
	}
};
// 监听 animation 动画的结束
const listenerAnimationend = () => {
	noticeBarTextRef.value.addEventListener(
		'animationend',
		() => {
			changeAnimation();
		},
		false
	);
};
// 右侧 icon 图标点击
const onRightIconClick = () => {
	if (!props.mode) return false;
	if (props.mode === 'closeable') {
		state.isMode = true;
		emit('close');
	} else if (props.mode === 'link') {
		emit('link');
	}
};
// 页面加载时
onMounted(() => {
	if (props.scrollable) return false;
	initAnimation();
	listenerAnimationend();
});
</script>

<style scoped lang="scss">
.notice-bar {
	padding: 0 15px;
	width: 100%;
	border-radius: 4px;
	.notice-bar-warp {
		display: flex;
		align-items: center;
		width: 100%;
		height: inherit;
		.notice-bar-warp-text-box {
			flex: 1;
			height: inherit;
			display: flex;
			align-items: center;
			overflow: hidden;
			position: relative;
			.notice-bar-warp-text {
				white-space: nowrap;
				position: absolute;
				left: 0;
			}
			.notice-bar-warp-slot {
				width: 100%;
				white-space: nowrap;
				:deep(.el-carousel__item) {
					display: flex;
					align-items: center;
				}
			}
		}
		.notice-bar-warp-left-icon {
			width: 24px;
			font-size: inherit !important;
		}
		.notice-bar-warp-right-icon {
			width: 24px;
			text-align: right;
			font-size: inherit !important;
			&:hover {
				cursor: pointer;
			}
		}
	}
}
</style>
