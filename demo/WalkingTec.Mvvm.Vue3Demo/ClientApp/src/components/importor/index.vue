<template>
	<div style="text-align: right;margin-bottom: 10px;">
					<el-button ref="uploadRef" type="success" class="ml10" @click="onDownload()">
						<i class="fa fa-download"></i>
						{{$t('message._system.importor.download')}}
					</el-button>
				</div>
				<el-upload ref="uploadRef"
					drag
					action="/api/_file/Upload"
					:on-success="onSuccess"
					:on-remove="onRemove"
					:headers="header"
					:limit="1"
					:file-list="props.files"
				>
					<i class="fa fa-cloud" style="font-size: 48px;"></i>
					<div class="el-upload__text">
					{{$t('message._system.importor.drag')}} <em>{{$t('message._system.importor.click')}}</em>
					</div>
					<template #tip>
					<div class="el-upload__tip">
						{{$t('message._system.importor.tip')}}
					</div>
					</template>
				</el-upload>
</template>

<script setup lang="ts">
import { ref,computed } from 'vue';
import request from '/@/utils/request';
import other from '/@/utils/other';
import { Local } from '/@/utils/storage';
import fileApi from '/@/api/file';

const emit = defineEmits(['refresh','update:modelValue']);
const uploadRef = ref();
// 定义父组件传过来的值
const props = defineProps({
	url: String,
	files:Array,
	modelValue:null
});

const header = computed(()=>{
	return { Authorization: `Bearer ${Local.get('token')}` }
})

const onDownload = ()=>{
	return request<any,Blob>({
		responseType: "blob",
		url: props.url,
		method: 'get'
	}).then((data)=>{other.downloadFile(data)});
}

const onSuccess = (res: any, uploadFile: any, uploadFiles: any)=>{
	uploadFile.fileID = res.Id;
	emit('update:modelValue',res.Id);

}

const onRemove = (file:any)=>{
	fileApi().deleteFile(file.fileID);
	emit('update:modelValue',null);
}
// 暴露变量
defineExpose({
	
});
</script>

<style scoped lang="scss">

</style>
