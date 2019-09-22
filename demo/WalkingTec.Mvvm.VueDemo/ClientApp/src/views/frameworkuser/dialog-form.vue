<template>
  <div class="frameworkuser-form">
    <el-form :ref="refName" :model="formData" :rules="rules" label-width="100px" class="demo-ruleForm">
      <el-row>
        <el-col :span="12">
          <el-form-item label="账号" prop="ITCode">
            <el-input v-model="formData.ITCode" v-edit:[status] />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="密码" prop="Password">
            <el-input v-model="formData.Password" v-edit:[status] />
          </el-form-item>
        </el-col>
      </el-row>

      <el-row>
        <el-col :span="12">
          <el-form-item label="邮箱">
            <el-input v-model="formData.Email" v-edit:[status] />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="姓名" prop="Name">
            <el-input v-model="formData.Name" v-edit:[status] />
          </el-form-item>
        </el-col>
      </el-row>

      <el-row>
        <el-col :span="12">
          <el-form-item label="性别">
            <el-select v-model="formData.Sex" v-edit:[status]="{list: sexList, key:'value', label: 'label'}">
              <el-option v-for="(item, index) of sexList" :key="index" :label="item.label" :value="item.value" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="手机号">
            <el-input v-model="formData.CellPhone" v-edit:[status] />
          </el-form-item>
        </el-col>
      </el-row>

      <el-row>
        <el-col :span="12">
          <el-form-item label="座机">
            <el-input v-model="formData.HomePhone" v-edit:[status] />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="住址">
            <el-input v-model="formData.Address" v-edit:[status] />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :span="12">
          <el-form-item label="邮编">
            <el-input v-model="formData.ZipCode" v-edit:[status] />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="头像">
            <edit-box :is-edit="status !== dialogType.detail">
              <upload-img :photo-id.sync="formData.PhotoId" />
              <!-- <el-upload class="avatar-uploader" action="/api/_file/upload" :show-file-list="false" :on-success="handleAvatarSuccess" :before-upload="beforeAvatarUpload">
                <img v-if="formData.PhotoId" :src="'/api/_file/downloadFile/'+formData.PhotoId" class="avatar">
                <i v-else class="el-icon-plus avatar-uploader-icon" />
              </el-upload> -->
              <template #editValue>
                <img v-if="formData.PhotoId" :src="'/api/_file/downloadFile/'+formData.PhotoId" class="avatar">
              </template>
            </edit-box>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :span="24">
          <el-form-item label="是否有效" prop="IsValid">
            <edit-box :is-edit="status !== dialogType.detail">
              <el-switch v-model="formData.IsValid" />
              <template #editValue>
                {{ formData.IsValid===true ? "是" : "否" }}
              </template>
            </edit-box>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :span="24">
          <el-form-item label="角色">
            <el-transfer v-model="formData.UserRoles" filterable :filter-method="filterMethod" filter-placeholder="请输入角色" :data="userRolesData" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :span="24">
          <el-form-item label="用户组">
            <el-transfer v-model="formData.UserGroups" filterable :filter-method="filterMethod" filter-placeholder="请输入用户组" :data="userGroupsData" />
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>
    <dialog-footer :status="status" @onClear="onClear" @onSubmit="onSubmitForm" />
  </div>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import mixinDialogForm from "@/util/mixin/form-mixin";
import { sexList } from "@/config/entity";
import uploadImg from "@/components/form/upload-img.vue";
// 表单结构
const defaultFormData = {
    // 表单名称
    refName: "refName",
    // 表单数据
    formData: {
        ID: "",
        ITCode: "",
        Password: "",
        Email: "",
        Name: "",
        Sex: 0,
        CellPhone: "",
        HomePhone: "",
        Address: "",
        ZipCode: "",
        PhotoId: "",
        IsValid: "true",
        UserRoles: [],
        UserGroups: []
    }
};

@Component({
    mixins: [mixinDialogForm(defaultFormData)],
    components: { "upload-img": uploadImg }
})
export default class Index extends Vue {
    @Action add;
    @Action edit;
    @Action detail;
    @Action getFrameworkRoles;
    @Action getFrameworkGroups;
    @State getFrameworkRolesData;
    @State getFrameworkGroupsData;
    // 用户组
    groups = [];
    sexList = sexList;
    // ★★
    filterMethod = (query, item) => {
        return item.label.indexOf(query) > -1;
    };
    // 验证 ★★★★★
    get rules() {
        if (this["status"] !== this["dialogType"].detail) {
            // 动态验证会走遍验证，需要清除验证
            this.$nextTick(() => {
                this.$refs[defaultFormData.refName].resetFields();
            });
            return {
                ITCode: [
                    {
                        required: true,
                        message: "请输入账号",
                        trigger: "blur"
                    }
                ],
                Password: [
                    {
                        required: true,
                        message: "请输入密码",
                        trigger: "blur"
                    }
                ],
                Name: [
                    {
                        required: true,
                        message: "请输入姓名",
                        trigger: "blur"
                    }
                ]
            };
        } else {
            return {};
        }
    }
    // ★★
    get userRolesData() {
        console.log(
            "this.getFrameworkRolesData",
            this.getFrameworkRolesData,
            this
        );
        return this.getFrameworkRolesData.map(item => {
            return {
                key: item.Value,
                label: item.Text,
                // 判断是否修改
                disabled: this["status"] === this["dialogType"].detail
            };
        });
    }
    // ★★
    get userGroupsData() {
        return this.getFrameworkGroupsData.map(item => {
            return {
                key: item.Value,
                label: item.Text,
                disabled: this["status"] === this["dialogType"].detail
            };
        });
    }
    // ★★★★★
    created() {
        this.getFrameworkRoles();
        this.getFrameworkGroups();
    }
    // 打开详情 ★★★★★
    onGetFormData() {
        if (!this["dialogData"]) {
            console.log(this["dialogData"]);
            console.error("dialogData 没有id数据");
        }
        if (this["status"] !== this["dialogType"].add) {
            const parameters = { ID: this["dialogData"].ID };
            this.detail(parameters).then(res => {
                this["setFormData"](res.Entity);
                this.updDataToTransfer("UserRoles");
                this.updDataToTransfer("UserGroups");
            });
        } else {
            this["onReset"]();
        }
    }
    // 提交 ★★★★★
    onSubmitForm() {
        this.$refs[defaultFormData.refName].validate(valid => {
            if (valid) {
                this.updTransferToData("UserRoles");
                this.updTransferToData("UserGroups");
                if (this["status"] === this["dialogType"].add) {
                    this.onAdd();
                } else if (this["status"] === this["dialogType"].edit) {
                    this.onEdit();
                }
            }
        });
    }
    // 上传图片 ★★★
    handleAvatarSuccess(res, file) {
        this["formData"].PhotoId = res.Id; // URL.createObjectURL(file.raw);
    }
    // ★★★
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
    // Roles&Groups数据格式与穿梭框格式不符，数据格式 >>> 穿梭框格式 ★★
    updDataToTransfer(field) {
        this["formData"][field] = this["formData"][field].map(item => {
            if (field === "UserGroups") {
                return item.GroupId;
            } else {
                return item.RoleId;
            }
        });
    }
    // Roles&Groups数据格式与穿梭框格式不符，穿梭框格式 >>> 数据格式 ★★
    updTransferToData(field) {
        this["formData"][field] = this["formData"][field].map(item => {
            if (field === "UserGroups") {
                return {
                    GroupId: item
                };
            } else {
                return {
                    RoleId: item
                };
            }
        });
    }
}
</script>
<style lang='less'>
.frameworkuser-form {
    .avatar-uploader .el-upload {
        border: 1px dashed #d9d9d9;
        border-radius: 6px;
        cursor: pointer;
        position: relative;
        overflow: hidden;
    }
    .avatar-uploader .el-upload:hover {
        border-color: #409eff;
    }
    .avatar-uploader-icon {
        font-size: 28px;
        color: #8c939d;
        width: 178px;
        height: 178px;
        line-height: 178px;
        text-align: center;
    }
    .avatar {
        width: 178px;
        height: 178px;
        display: block;
    }
}
</style>
