<template>
	<div class="editor-container">
		<Toolbar :editor="editorRef" :mode="mode" />
		<Editor
			:mode="mode"
			:defaultConfig="state.editorConfig"
			:style="{ height }"
			v-model="state.editorvalue"
			@onCreated="handleCreated"
			@onChange="handleChange"
		/>
	</div>
</template>

<script setup lang="ts" name="wngEditor">
// https://www.wangeditor.com/v5/for-frame.html#vue3
import '@wangeditor/editor/dist/css/style.css';
import { reactive, shallowRef, watch, onBeforeUnmount } from 'vue';
import { IDomEditor } from '@wangeditor/editor';
import { Toolbar, Editor } from '@wangeditor/editor-for-vue';
import { Local } from '/@/utils/storage';
// 定义父组件传过来的值
const props = defineProps({
	// 是否禁用
	disabled: {
		type: Boolean,
		default: () => false,
	},
	// 内容框默认 placeholder
	placeholder: {
		type: String,
		default: () => '',
	},
	// https://www.wangeditor.com/v5/getting-started.html#mode-%E6%A8%A1%E5%BC%8F
	// 模式，可选 <default|simple>，默认 default
	mode: {
		type: String,
		default: () => 'default',
	},
	// 高度
	height: {
		type: String,
		default: () => '310px',
	},
	// 双向绑定，用于获取 editor.getHtml()
	getHtml: String,
	// 双向绑定，用于获取 editor.getText()
	getText: String,
	modelValue: null,
});

// 定义子组件向父组件传值/事件
const emit = defineEmits(['update:getHtml', 'update:getText','update:modelValue']);

// 定义变量内容
const editorRef = shallowRef();
const state = reactive({
	editorConfig: {
		placeholder: props.placeholder,
		readOnly:props.disabled,
		MENU_CONF: {
			uploadImage:{
				server: '/api/_file/uploadImage',
				fieldName: 'file',
				maxFileSize: 10 * 1024 * 1024, 
				headers: {
					Authorization: `Bearer ${Local.get('token')}`
    			},
			}
		}
	},
	editorvalue:''
});

// 编辑器回调函数
const handleCreated = (editor: IDomEditor) => {
	editorRef.value = editor;
};
// 编辑器内容改变时
const handleChange = (editor: IDomEditor) => {
	emit('update:getHtml', editor.getHtml());
	emit('update:getText', editor.getText());
	emit('update:modelValue', editor.getHtml());
};
// 页面销毁时
onBeforeUnmount(() => {
	const editor = editorRef.value;
	if (editor == null) return;
	editor.destroy();
});
// 监听是否禁用改变
// https://gitee.com/lyt-top/vue-next-admin/issues/I4LM7I
// watch(
// 	() => props.disabled,
// 	(bool) => {
// 		const editor = editorRef.value;
// 		if (editor == null) return;
// 		bool ? editor.disable() : editor.enable();
// 	},
// 	{
// 		deep: true,
// 	}
// );
// 监听双向绑定值改变，用于回显
watch(
	() => props.modelValue,
	(val) => {
		state.editorvalue = val;
	},
	{
		deep: true,
	}
);
</script>
