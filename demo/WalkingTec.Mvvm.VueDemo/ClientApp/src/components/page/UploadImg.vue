<template>
    <div class="uploadimg">
        <el-upload class="avatar-uploader" :class="[isHead ? 'head-wrap' : '']" :action="uploadApi" :show-file-list="false" :on-success="handleAvatarSuccess" :before-upload="beforeAvatarUpload">
            <img v-if="imageUrl" :src="imageUrl" class="avatar" :style="imageStyle" />
            <i v-else class="el-icon-plus avatar-uploader-icon" :style="imageStyle" />
            <span v-if="imageUrl" class="upload-actions">
                <span class="upload-actions-item" @click.stop="handlePictureCardPreview">
                    <i class="el-icon-zoom-in" />
                </span>
                <span class="upload-actions-item">
                    <i class="el-icon-refresh" />
                </span>
                <span class="upload-actions-item" @click.stop="handleRemove">
                    <i class="el-icon-delete" />
                </span>
            </span>
        </el-upload>
        <el-dialog :visible.sync="dialogVisible" :modal-append-to-body="true" :append-to-body="true">
            <img v-if="imageUrl" width="100%" :src="imageUrl" alt="" />
        </el-dialog>
    </div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";
import { actionApi, fileApi } from "@/service/modules/upload";

@Component
export default class UploadImg extends Vue {
    @Prop({ type: String, default: "" })
    imgId;
    @Prop({ type: String, default: actionApi })
    uploadApi;
    @Prop({ type: Boolean, default: false })
    isHead;
    @Prop({ type: Object, default: () => {} })
    imageStyle;

    dialogVisible: Boolean = false;

    get imageUrl() {
        if (this.imgId) {
            return fileApi + this.imgId;
        } else {
            return false;
        }
    }

    // 上传图片
    handleAvatarSuccess(res, file) {
        this.$emit("onBackImgId", res.Id);
    }
    // 验证
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

    handleRemove() {
        this.$emit("onBackImgId", "");
    }
    handlePictureCardPreview() {
        this.dialogVisible = true;
    }
}
</script>
<style lang="less">
@import "~@/assets/css/variable.less";
.uploadimg {
    .avatar-uploader {
        .upload-actions {
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
            &:after {
                display: inline-block;
                content: "";
                height: 100%;
                vertical-align: middle;
            }
            span {
                display: inline-block;
            }
            span + span {
                margin-left: 15px;
            }
        }
    }

    .head-wrap {
        .el-upload {
            border: 1px dashed #d9d9d9;
            border-radius: 6px;
            cursor: pointer;
            position: relative;
            overflow: hidden;
        }
        .el-upload:hover {
            border-color: #409eff;
        }
        .avatar-uploader-icon {
            font-size: 28px;
            color: #8c939d;
            width: 100px;
            height: 100px;
            line-height: 100px;
            text-align: center;
        }
        .avatar {
            min-width: 100px;
            display: block;
        }
    }
}
</style>
