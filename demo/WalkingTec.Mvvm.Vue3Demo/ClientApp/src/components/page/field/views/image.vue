<template>
    <template v-if="_readonly">
        <a-upload :disabled="true"
              v-if="max !== 1" 
              name="file"
              list-type="picture-card"
              accept="image/*"
              :action="action"
              :file-list="fileList"
              @preview="handlePreview"
              :headers="headers"
              v-bind="_fieldProps">
        </a-upload>  

         <!-- </a-spin> -->
            <a-modal :visible="previewVisible" :footer="null" @cancel="handlePreview(false)">
                <img alt="example" style="width: 100%" :src="previewUrl" />
            </a-modal>

        <a-upload :disabled="true"
              v-if="max == 1" 
              name="file"
              list-type="picture-card"
              accept="image/*"
              :action="action"
              :file-list="fileList"
              @preview="handlePreview"
              :headers="headers"
              v-bind="_fieldProps">
        </a-upload>
    </template>
    <template v-else>
        <div class="w-avatar-uploader">
            <!-- <a-spin :spinning="spinning"> -->
            <!-- v-model:fileList="fileList" -->
            <a-upload 
                      name="file"
                      list-type="picture-card"
                      :multiple="max == 1 ? false : true"
                      accept="image/*"
                      :action="action"
                      :file-list="fileList"
                      :headers="headers"
                      :before-upload="beforeUpload"
                      :onRemove="onRemove"
                      @change="onChange"
                      @preview="handlePreview"
                      v-bind="_fieldProps">
                <!-- <img v-if="imageUrl" :src="imageUrl" class="w-upload-img" /> -->
                <!-- <a-image
                    @click.self.stop
                    v-if="imageUrl"
                    :width="104"
                    class="w-upload-img"
                    :src="imageUrl"
                    :fallback="imagefallback"
                />-->
                <div v-if="isPlusBottun">
                    <!-- <loading-outlined v-if="loading"></loading-outlined> -->
                    <plus-outlined></plus-outlined>
                    <div class="ant-upload-text">Upload</div>
                </div>
            </a-upload>
            <a-modal :visible="previewVisible" :footer="null" @cancel="handlePreview(false)">
                <img alt="example" style="width: 100%" :src="previewUrl" />
            </a-modal>
        </div>
    </template>
</template>
<script lang="ts">
    import { Vue, Options, Watch, mixins, Inject } from "vue-property-decorator";
    import { $System, globalProperties } from "@/client";
    import { FieldBasics } from "../script";
    @Options({ components: {} })
    export default class extends mixins(FieldBasics) {
        // 表单状态值
        @Inject() readonly formState;
        // 自定义校验状态
        @Inject() readonly formValidate;
        // 实体
        @Inject() readonly PageEntity;
        // 表单类型
        @Inject({ default: "" }) readonly formType;
        previewUrl = "";
        previewVisible = false;
        get action() {
            return $System.FilesController.getUploadUrl();
        }
        get headers() {
            return { Authorization: $System.UserController.Authorization }
        }
        get max() {
            return this.lodash.get(this._fieldProps, "max", 1);
        }
        get value() {
            return this.lodash.get(this.formState, this._name);
        }
        set value(value) {
            this.lodash.set(this.formState, this._name, value);
        }
        get isPlusBottun() {
            return (
                !(this.disabled || this._readonly) && this.fileList.length < this.max
            );
        }

        fileList = [];
        filedata = [];
        async mounted() {
            // this.onRequest();
            /*if (this.value) {
               this.imageUrl = $System.FilesController.getDownloadUrl(this.value)
            }*/
            
            if (this.debug) {
                console.log("");
                console.group(`Field ~ ${this.entityKey} ${this._name} `);
                console.log(this);
                console.groupEnd();
            }
            this.onValueChange(this.value, undefined);
        }
        beforeUpload() { }
        onChange(event) {
            this.fileList = event.fileList
            if (event.file.status === "removed") {
                if(this.max !== 1){
                    console.log(event)
                     if(!this.formState.DeletedFileIds){
                        this.formState.DeletedFileIds = [event.file.Id]
                     }else{
                        this.formState.DeletedFileIds.push(event.file.Id);
                     }
                     let value = this.lodash.map(event.fileList)
                     this.value = this.lodash.map(value, (item, index) => {
                        return {
                            order:index,
                            FileId:this.lodash.get(item, "Id") || this.lodash.get(item, "response.Id")
                        }
                    })
                }else{
                    this.formState.DeletedFileIds = [this.value];
                    this.value = null;
                }
            }
            if (event.file.status === "uploading") {
                this.spinning = true;
            } else {
                this.spinning = false;
                if (event.file.status === "done") {
                    if (this.max === 1) {
                        const Id = this.lodash.get(event, "file.response.Id");
                        this.value = Id;
                    } else {
                        let value = this.lodash.map(event.fileList)
                        this.value = this.lodash.map(value, (item, index) => {
                            return {
                                order:index,
                                FileId:this.lodash.get(item, "Id") || this.lodash.get(item, "response.Id")
                            }
                        })
                    }
                }
            }
        }
        @Watch("value")
        onValueChange(val, old) {
                if (val) {
                  /*  console.log(this.lodash.map(val)[0] || this.lodash.map(val).length)
                    if(!this.lodash.map(val)[0] || this.lodash.map(val).length == 0){
                        console.log(val)
                        this.fileList = []
                        this.value = [] 
                        return false
                    }*/
                    if (this.max !== 1) {
                        this.filedata = this.lodash.map(
                            val,
                            item => {
                              return {
                                fileid: item['FileId'],
                                fileurl: $System.FilesController.getDownloadUrl(item['FileId']),
                                filename: $System.FilesController.getFileName(val)
                              }
                            }
                        );
                    } else {
                        this.filedata = [{
                            fileid: val,
                            fileurl: $System.FilesController.getDownloadUrl(val),
                            filename: $System.FilesController.getFileName(val)
                        }]
                    }
                    Promise.all(this.lodash.map(this.filedata, "filename")).then((ps) => {
                            this.fileList = this.lodash.map(this.filedata, (item, index) => {
                                return {
                                    uid: item.fileid,
                                    Id:item.fileid,
                                    status: "done",
                                    url: item.fileurl
                                };
                            });
                    })
                }
        }
        handlePreview(file) {
            if (file) {
                this.previewVisible = true;
                this.previewUrl = file.url;
            } else {
                this.previewVisible = false;
                this.previewUrl = "";
            }
        }
        onRemove(file) {
            const response = this.lodash.get(file, 'response')
            return response ? $System.FilesController.deleteFiles(response) : $System.FilesController.deleteFiles({ Id: file.uid })
        }
    }
    function getBase64(img: Blob, callback: (base64Url: string) => void) {
        const reader = new FileReader();
        reader.addEventListener("load", () => callback(reader.result as string));
        reader.readAsDataURL(img);
    }
</script>
<style lang="less">
    .w-avatar-uploader { min-height: 120px;
    .ant-upload.ant-upload-select-picture-card
    { margin: 0; }
    .anticon-eye { display: none; }
    }
    .w-upload-img { width: 104px; height: 104px; }
</style>
