<template>
	<slot v-if="getUserAuthBtnList" />
</template>

<script setup lang="ts" name="auth">
import { computed } from 'vue';
import { auth } from '/@/utils/authFunction';

// 定义父组件传过来的值
const props = defineProps({
	value: {
		type: String,
		default: () => '',
	},
});

// 获取 pinia 中的用户权限
const getUserAuthBtnList = computed(() => {
	return auth(props.value);
});
</script>
