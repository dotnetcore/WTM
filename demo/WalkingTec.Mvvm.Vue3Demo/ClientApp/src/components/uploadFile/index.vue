<template>
  <el-upload style="width:100%" ref="uploadRef" action="/api/_file/Upload" :on-success="onSuccess" :on-remove="onRemove" :on-preview="onDownload" :headers="header"
    :limit="props.multi==true?undefined:1" v-model:file-list="files" :disabled="props.disabled" :multiple="props.multi">    
      <el-button v-if="disabled!==true" type="primary">{{$t('message._system.uploadFile.select')}}</el-button>
      <span v-else>{{$t("message._system.uploadFile.total",{count:files?.length})}}</span>
  </el-upload>
</template>

<script setup lang="ts">

import { ref, computed, watch } from 'vue';
import { Local } from '/@/utils/storage';
import fileApi from '/@/api/file';
import { useI18n } from 'vue-i18n';


const emit = defineEmits(['refresh', 'update:modelValue', 'update:deletedFiles']);
const uploadRef = ref();
// 定义父组件传过来的值
const props = defineProps({
  multi: Boolean,
  disabled: Boolean,
  modelValue: null,
  deletedFiles: null
});
const { t } = useI18n();
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

const files = ref();
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
      files.value = [];
      if (props.modelValue) {
        let filename = await fileApi().getName(props.modelValue);
        files.value = [{ name: filename, url: '/api/_file/getfile/' + props.modelValue, FileId: props.modelValue }];
      }
    }
  }
  else{
    for(let i = 0;i<props.modelValue.length;i++){
      if(files.value.length<= i || files.value[i].FileId != props.modelValue[i].FileId){
        let filename = await fileApi().getName(props.modelValue[i].FileId);
        let nv = { name: filename, url: '/api/_file/getfile/' + props.modelValue[i].FileId, FileId: props.modelValue[i].FileId,keyID: props.modelValue[i].ID};
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

const onSuccess = (res: any, uploadFile: any, uploadFiles: any) => {
  uploadFile.FileId = res.Id;
  if (props.multi == false) {    
    filevalue.value = res.Id;
    if (files.value.length > 1) {
      let nv = [...deletedFiles.value];
      deletedFiles.value.push(files.value[0].FileId);
      files.value.splice(0, 1);
    }
  }
  else{
    let tempfv = [];
    tempfv = uploadFiles.map((item:any)=>{
      return {FileId : item.FileId};
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
        if (props.multi == false) {
            filevalue.value = null;
        }
        else {
            filevalue.value = f.map((item: any) => { return { FileId: item.FileId, ID: item.keyID } });
        }
}
// 暴露变量
defineExpose({

});
</script>

<style scoped lang="scss"></style>
