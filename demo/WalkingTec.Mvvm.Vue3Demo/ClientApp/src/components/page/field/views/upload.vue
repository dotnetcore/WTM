<template>
    <div class="w-uploader" :class="{ 'w-uploader-readonly': _readonly }">
        <a-upload :fileList="fileList"
                  :disabled="disabled || _readonly"
                  :multiple="true"
                  :action="action"
                  :headers="headers"
                  :before-upload="beforeUpload"
                  :remove="onRemove"
                  @change="onChange"
                  v-bind="_fieldProps">
            <div v-if="isPlusBottun">
                <a-button>
                    <upload-outlined />Upload
                </a-button>
            </div>
        </a-upload>
    </div>
</template>
<script lang="ts">
    import { Vue, Options, Watch, mixins, Inject } from "vue-property-decorator";
    import { $System } from "@/client";
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
        @Inject({ default: '' }) readonly formType;
        get action() {
            return $System.FilesController.getUploadUrl()
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
        
        fileListCheck = null
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
            this.fileListCheck = 1
            this.fileList = event.fileList
            if (event.file.status === "removed") {
                if(this.max !== 1){
                     if(!this.formState.DeletedFileIds){
                        this.formState.DeletedFileIds = [event.file.Id]
                     }else{
                        this.formState.DeletedFileIds.push(event.file.Id);
                     }
                     let value = this.lodash.map(event.fileList)
                     this.value = this.lodash.map(value, (item, index) => {
                        return {
                            order:index,
                            name:this.lodash.get(item, "name") || this.lodash.get(item, "response.Name"),
                            FileId:this.lodash.get(item, "Id") || this.lodash.get(item, 'FileId') || this.lodash.get(item, "response.Id")
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
                        console.log(event.fileList)
                        this.value = this.lodash.map(value, (item, index) => {
                            return {
                                order:index,
                                name:this.lodash.get(item, "name") || this.lodash.get(item, "response.Name"),
                                FileId:this.lodash.get(item, "Id") || this.lodash.get(item, 'FileId') || this.lodash.get(item, "response.Id")
                            }
                        })
                    }
                }
            }
        }
        @Watch("value")
        onValueChange(val, old) {
            let that = this
            if (val) {
                if (this.max !== 1) {
                    this.fileList = []
                    this.lodash.map(
                        val,
                        item => {
                            if(!item.name){
                                $System.FilesController.getFileName(item['FileId']).then((res)=>{
                                    console.log(res)
                                     item.name = res
                                })
                            }
                            
                            setTimeout(()=>{
                                console.log(item)
                                that.fileList.push({
                                    Id: item['Id'] || item['FileId'],
                                    fileurl: $System.FilesController.getDownloadUrl(item['FileId']),
                                    name:item.name,
                                    uid: item.FileId,
                                    status: "done",
                                })
                            },that.fileListCheck == 1 ? 0 : 4000)
                        }
                    );
                } else {
                    this.filedata = [{
                        FileId: val,
                        fileurl: $System.FilesController.getDownloadUrl(val),
                        filename: $System.FilesController.getFileName(val)
                    }]
                }
               /* Promise.all(this.lodash.map(this.filedata, "filename")).then((ps) => {
                        this.fileList = this.lodash.map(this.filedata, (item, index) => {
                            return {
                                uid: item.FileId,
                                Id:item.FileId,
                                status: "done",
                                name:item.name,
                                url: item.fileurl
                            };
                        });
                    
                })*/
            }
        }

        onRemove(file) {
            const response = this.lodash.get(file, 'response')
            return response ? $System.FilesController.deleteFiles(response) : $System.FilesController.deleteFiles({ Id: file.uid })
        }
    }

</script>
<style lang="less">
    .w-uploader {
        &-readonly

    {
        .ant-upload

    {
        display: none;
    }

    }

    .ant-upload-list-item-info {
        padding-right: 60px;
    }
    }
</style>
