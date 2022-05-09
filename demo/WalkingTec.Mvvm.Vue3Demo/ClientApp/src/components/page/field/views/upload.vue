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
                     this.formState.DeletedFileIds.push(event.file.Id);
                     let value = this.lodash.map(event.fileList)
                     this.value = this.lodash.map(value, (item, index) => {
                        return {
                            order:index,
                            FileId:this.lodash.get(item, "Id") || this.lodash.get(item, "response.Id"),
                            name:this.lodash.get(item, "name") || this.lodash.get(item, "response.Name")
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
                                name:this.lodash.get(item, "name") || this.lodash.get(item, "response.Name"),
                                FileId:this.lodash.get(item, "Id") || this.lodash.get(item, "response.Id")
                            }
                        })
                        console.log(this.value)
                    }
                }
            }
        }
        @Watch("value")
        onValueChange(val, old) {
            if (val) {
                if (this.max !== 1) {
                    this.filedata = this.lodash.map(
                        val,
                        item => {
                             console.log(item)
                            return {
                                fileid: item['FileId'],
                                fileurl: $System.FilesController.getDownloadUrl(item['FileId']),
                                name:item.name || item['FileId']
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
                                name:item.name,
                                url: item.fileurl
                            };
                        });
                    
                })
            }
        }

        onRemove(file) {
            
            /*const response = this.lodash.get(file, 'response')
            return response ? $System.FilesController.deleteFiles(response) : $System.FilesController.deleteFiles({ Id: file.uid })*/
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
