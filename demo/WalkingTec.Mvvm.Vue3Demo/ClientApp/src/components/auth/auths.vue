<template>
	<slot v-if="getUserAuthBtnList" />
</template>

<script setup lang="ts" name="auths">
import { computed } from 'vue';
import { auths } from '/@/utils/authFunction';

// 定义父组件传过来的值
const props = defineProps({
	value: {
		type: Array,
		default: () => [],
	},
});

// 获取 pinia 中的用户权限
const getUserAuthBtnList = computed(() => {
	return auths(props.value);
});
</script>
