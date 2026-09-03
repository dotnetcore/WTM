<template>
	<slot v-if="getUserAuthBtnList" />
</template>

<script setup lang="ts" name="authAll">
import { computed } from 'vue';
import { authAll } from '/@/utils/authFunction';

// 定义父组件传过来的值
const props = defineProps({
	value: {
		type: Array,
		default: () => [],
	},
});

// 获取 pinia 中的用户权限
const getUserAuthBtnList = computed(() => {
	return authAll(props.value);
});
</script>
