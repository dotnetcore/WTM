<template>
    <div class="uploadimg">
        <el-upload class="avatar-uploader" :action="uploadApi" :headers="headers" :show-file-list="false" :on-progress="onProgress" :on-success="handleAvatarSuccess" :before-upload="beforeAvatarUpload">
            <div v-if="imageUrl">
                <img :src="imageUrl">
                <span class="el-upload__item-actions">
                    <span class="item-actions-but img-download">
                        <img src="~static/img/upload/upload.png">
                        <span class="flt">替换</span>
                    </span>
                    <span class="item-actions-but img-del" @click.stop="onDeleteImg()">
                        <img src="~static/img/upload/del.png">
                        <span class="flt">删除</span>
                    </span>
                </span>
            </div>
            <div v-else-if="imgType===1">
                <i class="el-icon-picture avatar-uploader-icon"></i>
                <div class="el-upload__text">加载失败，点此<em>重新加载</em></div>
            </div>
            <div v-else-if="imgType===2">
                <i class="el-icon-loading avatar-uploader-icon"></i>
                <div class="el-upload__text">上传中...</div>
            </div>
            <div v-else>
                <i class="el-icon-plus avatar-uploader-icon"></i>
                <div class="el-upload__text">将文件拖到此处，或<em>点击上传</em></div>
            </div>
            <div class="el-upload__tip" slot="tip">请上传不超过3mb的图片</div>
        </el-upload>
    </div>
</template>

<script lang='ts'>
import { Component, Vue, Prop, Watch } from "vue-property-decorator";
import store from "@/store/setting/shop-manage";
import { Action, State } from "vuex-class";
import config from "@/config/index";
import cookie from "@/util/cookie.js";

@Component({
    store
    })
export default class UploadImg extends Vue {
    @Prop()
    url;
    headers: object = {};
    imageUrl: string = "";
    imgType: number = 0; // 失败1 上传中2
    uploadApi: string = "/uds/api/nsp/v1/upload_file";
    created() {
        this.imageUrl = this.url;
        this.headers = {
            Authorization: cookie.getCookie(config.tokenKey)
        };
    }
    handleAvatarSuccess(res, file) {
        if (res.data && res.data.url) {
            this.imageUrl = res.data.url;
            this.$emit("getUrl", this.imageUrl);
        } else {
            this.imgType = 1;
        }
        // this.imageUrl = URL.createObjectURL(file.raw);
    }
    beforeAvatarUpload(file) {
        const isJPG = file.type.search("image") !== -1;
        const isLt2M = file.size / 1024 / 1024 < 3;
        if (!isJPG) {
            this["$message"].error("上传只能图片格式!");
        }
        if (!isLt2M) {
            this["$message"].error("上传图片大小不能超过 3MB!");
        }
        return isJPG && isLt2M;
    }
    onProgress() {
        this.imgType = 2;
    }
    onDeleteImg() {
        this.imageUrl = "";
        this.imgType = 0;
        this.$emit("getUrl", this.imageUrl);
    }
}
</script>
<style lang='less'>
@import "~@/assets/css/variable.less";
.uploadimg {
    display: inline-block;
    img {
        width: 100%;
    }
    .avatar-uploader {
        display: inline-block;
        width: 360vw * @vwUnit;
        height: 180vw * @vwUnit;
        .el-upload {
            border: 1px dashed #d9d9d9;
            border-radius: 6px;
            cursor: pointer;
            position: relative;
            overflow: hidden;
            height: 100%;
            width: 100%;
        }
        .el-upload:hover {
            border-color: #409eff;
        }
        .el-upload__item-actions {
            position: absolute;
            width: 100%;
            height: 100%;
            left: 0;
            top: 0;
            cursor: default;
            text-align: center;
            color: #fff;
            opacity: 0;
            font-size: 20px;
            background-color: rgba(0, 0, 0, 0.5);
            transition: opacity 0.3s;
            &:hover {
                opacity: 1;
            }
            .item-actions-but {
                display: inline-block;
                position: absolute;
                bottom: 8vw * @vwUnit;
                width: 34vw * @vwUnit;
                height: 27vw * @vwUnit;
                line-height: 27vw * @vwUnit;
                border-radius: 2px;
                background-color: #0d849a;
                cursor: pointer;
                .flt {
                    width: 44vw * @vwUnit;
                    height: 33vw * @vwUnit;
                    background-color: #606266;
                    color: #c0c4cc;
                    line-height: 33vw * @vwUnit;
                    position: absolute;
                    top: -37vw * @vwUnit;
                    right: 0;
                    font-size: 12px;
                    display: none;
                }
                img {
                    width: 14vw * @vwUnit;
                    height: 14vw * @vwUnit;
                }
                &.img-download {
                    right: 50vw * @vwUnit;
                }
                &.img-del {
                    right: 8vw * @vwUnit;
                }
                &:hover {
                    background-color: #16b4d1;
                    .flt {
                        display: inline-block;
                    }
                }
            }
        }
    }
    .avatar-uploader-icon {
        font-size: 28px;
        color: #8c939d;
        text-align: center;
        margin: 64vw * @vwUnit 0 19vw * @vwUnit;
    }
    // .avatar {
    //     width: 178px;
    //     height: 178px;
    //     display: block;
    // }
}
</style>
