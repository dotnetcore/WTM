<template class="table-search-container">
		<el-card accordion :body-style="{padding:'10px'}" v-bind="$attrs">
			<div class="card-header" @click="state.isExpand=!state.isExpand">
				<el-text style="text-align: left;flex:1">{{$t('message._system.searchPanel.condition')}}</el-text>
				<div>
					<el-button type="primary" @click.stop="onSearch(tableSearchRef)">{{$t('message._system.searchPanel.search')}} </el-button>
					<el-button type="info" class="ml10" @click.stop="onReset(tableSearchRef)">{{$t('message._system.searchPanel.reset')}}</el-button>
				</div>
			</div>
			<el-form v-show="state.isExpand" ref="tableSearchRef" :model="props.modelValue" label-width="100px" class="search-form">
				<slot></slot>
			</el-form>
		</el-card>
		<div style="height:10px"></div>
</template>

<script setup lang="ts">
import { ref, reactive, computed } from 'vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
const tableSearchRef = ref();
const emit = defineEmits(['search','update:modelValue']);
// 定义父组件传过来的值
const props = defineProps({
	modelValue: {}
});
const state = reactive({
	form: {},
	isExpand: false,
});

const onSearch = (formEl: any | undefined) => {
	if (!formEl) return;
	formEl.validate((valid: boolean) => {
		if (valid) {
			emit('search', state.form);
		} else {
			return false;
		}
	});
};

const onReset = (formEl: any | undefined) => {
	if (!formEl) return;
	formEl.resetFields();
};
// 暴露变量
defineExpose({
	inheritAttrs: false
});
</script>

<style scoped lang="scss">
.card-header {
	width:100%;
	display: flex;
	justify-content: flex-end;
}
.search-form{
	padding-left: 20px;
	padding-right: 20px;
	padding-top: 10px;
}
</style>
