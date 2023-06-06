<template>
  <el-upload ref="uploadRef" action="/api/_file/Upload" :on-success="onSuccess" :headers="header"
    v-model:file-list="files" :show-file-list="multi" list-type="picture-card" :disabled="props.disabled" :multiple="props.multi">
    <div v-if="imageUrl && multi !== true">
      <el-image :src="imageUrl" fit="fill"
        :preview-src-list="props.disabled === true ? ['/api/_file/getfile/' + props.modelValue] : undefined" />
    </div>
    <i v-else-if="disabled!==true" class="fa fa-plus"></i>
    <span v-else>{{$t("message._system.uploadFile.total",{count:files?.length})}}</span>
    <el-image-viewer teleported v-if="previewOpen" :url-list="imageUrls" :initial-index="previewIndex"
     infinite hide-on-click-modal @close="previewOpen=false"></el-image-viewer>
    <template #file="{ file }">
      <div class="el-upload-list--picture-card">
        <img class="el-upload-list__item-thumbnail" :src="file.url" />
        <span class="el-upload-list__item-actions">
          <span class="el-upload-list__item-delete" @click.stop="onPreview(file)">
            <i class="fa fa-eye"></i>
          </span>
          <span  class="el-upload-list__item-delete" @click.stop="onDownload(file)">
            <i class="fa fa-download"></i>
          </span>
          <span v-if="!props.disabled" class="el-upload-list__item-delete" @click.stop="onRemove(file)">
            <i class="fa fa-trash"></i>
          </span>
        </span>
      </div>
    </template>
  </el-upload>
</template>

<script setup lang="ts">

import { ref, computed, watch } from 'vue';
import { Local } from '/@/utils/storage';
import fileApi from '/@/api/file';

const emit = defineEmits(['refresh', 'update:modelValue', 'update:deletedFiles']);
const uploadRef = ref();
// 定义父组件传过来的值
const props = defineProps({
  multi: Boolean,
  disabled: Boolean,
  modelValue: null,
  deletedFiles: null
});

const filevalue = computed({
  get() {
    return props.modelValue;
  },
  set(value) {
    emit('update:modelValue', value)
  }
})

const deletedFiles = computed<any[]>({
  get() {
    return props.deletedFiles ?? [];
  },
  set(value) {
    emit('update:deletedFiles', value)
  }
});

const previewOpen = ref(false);
const previewIndex = ref(0);
const files = ref();
const imageUrl = ref('');
const imageUrls = computed(()=>{
  return files.value.map((item:any)=>{return '/api/_file/getfile/'+item.FileId});
})
const header = computed(() => {
  return { Authorization: `Bearer ${Local.get('token')}` }
})
watch(filevalue, async () => {

if(!filevalue.value){
  files.value = [];
  return;
}

if(!files.value){
  files.value = [];
}

if (props.multi == false) {

  let fv = null;
  if (files.value && files.value.length > 0) {
    fv = files.value[0].FileId;
  }
  if (props.modelValue !== fv) {
    imageUrl.value = "";
    files.value = [];
    if (props.modelValue) {
      imageUrl.value = "empty";
      let filename = await fileApi().getName(props.modelValue);
      files.value = [{ name: filename, url: '/api/_file/getfile/' + props.modelValue+"?width=150&height=150", FileId: props.modelValue }];
      imageUrl.value = '/api/_file/getfile/' + props.modelValue+"?width=150&height=150";
    }
  }
}
else{
  for(let i = 0;i<props.modelValue.length;i++){
    if(files.value.length<= i || files.value[i].FileId != props.modelValue[i].FileId){
      let filename = await fileApi().getName(props.modelValue[i].FileId);
      let nv = 
        { 
          name: filename, 
          url: '/api/_file/getfile/' + props.modelValue[i].FileId +"?width=150&height=150", 
          FileId: props.modelValue[i].FileId,
          keyID: props.modelValue[i].ID,
          index:i
        };
      if(files.value.length <=i ){
        files.value.push(nv);
      }
      else{
        files.value[i] = nv;
      }
    }
  }
  if(files.value.length > props.modelValue.length){
    files.value.splice(props.modelValue.length,props.modelValue.length-files.value.length);
  }
}
})

const onDownload = (file: any) => {
  fileApi().downloadFile(file.FileId);
}
const onPreview = (file: any) => {
  previewIndex.value = file.index;
  previewOpen.value = true;  
}
const onSuccess = (res: any, uploadFile: any, uploadFiles: any) => {
  uploadFile.FileId = res.Id;
  if (props.multi == false) {    
    filevalue.value = res.Id;
    imageUrl.value = URL.createObjectURL(uploadFile.raw!);
    if (files.value.length > 1) {
      let nv = [...deletedFiles.value];
      deletedFiles.value.push(files.value[0].FileId);
      files.value.splice(0, 1);
    }
  }
  else{
    let tempfv = [];
    tempfv = uploadFiles.map((item:any,i:number)=>{
      item.index = i;
      return {FileId : item.FileId, index:i};
    });
    filevalue.value = tempfv;
  }
}

const onRemove = (file: any) => {
  fileApi().deleteFile(file.FileId);
  const f = files.value;
  if (f) {
    for (let index = 0; index < f.length; index++) {
      const item = files.value[index];
      if (item.FileId === file.FileId) {
        let nv = [...deletedFiles.value];
        deletedFiles.value.push(file.FileId);
        f.splice(index, 1);
        index--;
      }
    }
  }
  filevalue.value = f.map((item:any,i:number)=>{item.index=i; return {FileId: item.FileId, ID: item.keyID, index:i}});
}

// 暴露变量
defineExpose({

});
</script>

<style scoped lang="scss"></style>
