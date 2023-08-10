<template>
	<component :is="isEmpty? emptyLayout : layouts[themeConfig.layout]" />
	
</template>

<script setup lang="ts" name="layout">
import { onBeforeMount, onUnmounted, defineAsyncComponent,ref } from 'vue';
import { storeToRefs } from 'pinia';
import { useThemeConfig } from '/@/stores/themeConfig';
import { Local } from '/@/utils/storage';
import mittBus from '/@/utils/mitt';
    import { useRouter } from "vue-router";

    const isEmpty = ref(false)
// 引入组件
const layouts: any = {
	defaults: defineAsyncComponent(() => import('/@/layout/main/defaults.vue')),
	classic: defineAsyncComponent(() => import('/@/layout/main/classic.vue')),
	transverse: defineAsyncComponent(() => import('/@/layout/main/transverse.vue')),
	columns: defineAsyncComponent(() => import('/@/layout/main/columns.vue')),
};

    const emptyLayout = defineAsyncComponent(() => import('/@/layout/main/empty.vue'))
// 定义变量内容
const storesThemeConfig = useThemeConfig();
const { themeConfig } = storeToRefs(storesThemeConfig);

// 窗口大小改变时(适配移动端)
const onLayoutResize = () => {
	if (!Local.get('oldLayout')) Local.set('oldLayout', themeConfig.value.layout);
	const clientWidth = document.body.clientWidth;
	if (clientWidth < 1000) {
		themeConfig.value.isCollapse = false;
		mittBus.emit('layoutMobileResize', {
			layout: 'defaults',
			clientWidth,
		});
	} else {
		mittBus.emit('layoutMobileResize', {
			layout: Local.get('oldLayout') ? Local.get('oldLayout') : themeConfig.value.layout,
			clientWidth,
		});
	}
};
// 页面加载前
	onBeforeMount(() => {
		if (useRouter().currentRoute.value.query._nomenu) {
			isEmpty.value = true;
		}
	onLayoutResize();
	window.addEventListener('resize', onLayoutResize);
});
// 页面卸载时
onUnmounted(() => {
	window.removeEventListener('resize', onLayoutResize);
});
</script>
