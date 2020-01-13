<template>
  <div class="uploadimg">
    <el-upload class="avatar-uploader" :action="uploadApi" :show-file-list="false" :on-success="handleAvatarSuccess" :before-upload="beforeAvatarUpload">
      <img v-if="imageUrl" :src="imageUrl" class="avatar">
      <i v-else class="el-icon-plus avatar-uploader-icon" />
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
      <img width="100%" :src="imageUrl" alt="">
    </el-dialog>
  </div>
</template>

<script lang='ts'>
import { Component, Vue, Prop } from "vue-property-decorator";

@Component
export default class UploadImg extends Vue {
    @Prop({ type: String, default: "" })
    photoId;
    @Prop({ type: String, default: "/api/_file/upload" })
    uploadApi;

    dialogVisible: Boolean = false;

    get imageUrl() {
        if (this.photoId) {
            return "/api/_file/downloadFile/" + this.photoId;
        } else {
            return false;
        }
    }

    // 上传图片
    handleAvatarSuccess(res, file) {
        this.$emit("update:photoId", res.Id);
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
        this.$emit("update:photoId", "");
    }
    handlePictureCardPreview() {
        this.dialogVisible = true;
    }
}
</script>
<style lang='less'>
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
}
</style>
