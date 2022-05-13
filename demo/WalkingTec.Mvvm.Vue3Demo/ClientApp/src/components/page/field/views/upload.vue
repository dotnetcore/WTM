<template>
    <div class="w-uploader" :class="{ 'w-uploader-readonly': _readonly }">
        <a-upload :fileList="fileList"
                  :disabled="disabled || _readonly"
                  :multiple="max == 1 ? false : true"
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
    import { $System,globalProperties } from "@/client";
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
            console.log(event.fileList)
            if (event.file.status === "removed") {
                if(this.max !== 1){
                     let ID = this.lodash.get(event.file, "Id") || this.lodash.get(event.file, "response.Id")
                     if(!this.formState.DeletedFileIds){
                        this.formState.DeletedFileIds = [ID]
                     }else{
                        this.formState.DeletedFileIds.push(ID);
                     }
                     let value = this.lodash.map(event.fileList)
                     this.value = this.lodash.map(value, (item, index) => {
                        return {
                            order:index,
                            FileId:this.lodash.get(item, "Id") || this.lodash.get(item, "response.Id"),
                            name:this.lodash.get(item, "name") || this.lodash.get(item, "response")
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
                        console.log(event.fileList,'done')
                        this.value = this.lodash.map(value, (item, index) => {
                            return {
                                order:index,
                                FileId:this.lodash.get(item, "Id") || this.lodash.get(item, "response.Id"),
                                name:this.lodash.get(item, "name") || this.lodash.get(item, "response")
                            }
                        })
                    }
                }
            }
        }
        @Watch("value")
        onValueChange(val, old) {
            console.log(this.fileList.length)
            /*if(!this.lodash.map(val)[0] || this.lodash.map(val).length == 0){
                console.log(val)
                this.fileList = []
                this.value = [] 
            }else{*/
                if (val) {
                  /*  if(!this.lodash.map(val)[0] || this.lodash.map(val).length == 0){
                        console.log(val)
                        this.fileList = []
                        this.value = [] 
                        val = []
                    }*/
                    if (this.max !== 1) {
                        this.filedata = this.lodash.map(
                            val,
                            item => {
                                return {
                                    FileId: item.FileId,
                                    fileurl: globalProperties.$WtmConfig.WtmGlobalUrl+$System.FilesController.getDownloadUrl(item.FileId),
                                    filename: $System.FilesController.getFileName(item['FileId'])
                                }
                            }
                        );
                    } else {
                        this.filedata = [{
                            FileId: val,
                            fileurl: globalProperties.$WtmConfig.WtmGlobalUrl+$System.FilesController.getDownloadUrl(val),
                            filename: $System.FilesController.getFileName(val)
                        }]
                    }
                    Promise.all(this.lodash.map(this.filedata, "filename")).then((ps) => {
                        var ips = ps;
                        if (this.fileList.length === 0) {
                            this.fileList = this.lodash.map(this.filedata, (item, index) => {
                                console.log(item)
                                return {
                                    uid: item.FileId,
                                    name: ps[index],
                                    Id:item.FileId,
                                    status: "done",
                                    url: item.fileurl
                                };
                            });
                            console.log(this.fileList)
                        }

                    })
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
