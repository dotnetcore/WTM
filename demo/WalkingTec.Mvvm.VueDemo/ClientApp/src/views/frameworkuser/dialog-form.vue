<template>
  <wtm-dialog-box :is-show.sync="isShow" :status="status" @close="onClose" @open="onGetFormData">
    <div class="frameworkuser-form">
      <el-form :ref="refName" :model="formData" :rules="rules" label-width="100px" class="demo-ruleForm">
        <el-row>
          <el-col :span="12">
            <wtm-form-item ref="Entity.RoleCode" label="账号" prop="ITCode">
              <el-input v-model="formData.Entity.ITCode" v-edit:[status] />
            </wtm-form-item>
          </el-col>
          <el-col :span="12">
            <wtm-form-item ref="Entity.RoleCode" label="密码" prop="Entity.Password">
              <el-input v-model="formData.Entity.Password" v-edit:[status] />
            </wtm-form-item>
          </el-col>
        </el-row>

        <el-row>
          <el-col :span="12">
            <wtm-form-item ref="Entity.RoleCode" label="邮箱">
              <el-input v-model="formData.Entity.Email" v-edit:[status] />
            </wtm-form-item>
          </el-col>
          <el-col :span="12">
            <wtm-form-item ref="Entity.RoleCode" label="姓名" prop="Entity.Name">
              <el-input v-model="formData.Entity.Name" v-edit:[status] />
            </wtm-form-item>
          </el-col>
        </el-row>

        <el-row>
          <el-col :span="12">
            <wtm-form-item label="性别">
              <el-select v-model="formData.Entity.Sex" v-edit:[status]="{list: sexList, key:'value', label: 'label'}">
                <el-option v-for="(item, index) of sexList" :key="index" :label="item.label" :value="item.value" />
              </el-select>
            </wtm-form-item>
          </el-col>
          <el-col :span="12">
            <wtm-form-item label="手机号">
              <el-input v-model="formData.Entity.CellPhone" v-edit:[status] />
            </wtm-form-item>
          </el-col>
        </el-row>

        <el-row>
          <el-col :span="12">
            <wtm-form-item label="座机">
              <el-input v-model="formData.Entity.HomePhone" v-edit:[status] />
            </wtm-form-item>
          </el-col>
          <el-col :span="12">
            <wtm-form-item label="住址">
              <el-input v-model="formData.Entity.Address" v-edit:[status] />
            </wtm-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="12">
            <wtm-form-item label="邮编">
              <el-input v-model="formData.Entity.ZipCode" v-edit:[status] />
            </wtm-form-item>
          </el-col>
          <el-col :span="12">
            <wtm-form-item label="头像">
              <edit-box :is-edit="status !== $actionType.detail">
                <upload-img :photo-id.sync="formData.Entity.PhotoId" />
                <template #editValue>
                  <img v-if="formData.Entity.PhotoId" :src="'/api/_file/downloadFile/'+formData.Entity.PhotoId" class="avatar">
                </template>
              </edit-box>
            </wtm-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="24">
            <wtm-form-item label="是否有效" prop="IsValid">
              <edit-box :is-edit="status !== $actionType.detail">
                <el-switch v-model="formData.Entity.IsValid" />
                <template #editValue>
                  {{ formData.Entity.IsValid===true ? "是" : "否" }}
                </template>
              </edit-box>
            </wtm-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="24">
            <wtm-form-item label="角色">
              <el-transfer v-model="formData.Entity.UserRoles" filterable :filter-method="filterMethod" filter-placeholder="请输入角色" :data="userRolesData" />
            </wtm-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="24">
            <wtm-form-item label="用户组">
              <el-transfer v-model="formData.Entity.UserGroups" filterable :filter-method="filterMethod" filter-placeholder="请输入用户组" :data="userGroupsData" />
            </wtm-form-item>
          </el-col>
        </el-row>
      </el-form>
      <dialog-footer :status="status" @onClear="onClose" @onSubmit="onSubmitForm" />
    </div>
  </wtm-dialog-box>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import formMixin from "@/vue-custom/mixin/form-mixin";
import { sexList } from "@/config/entity";
import uploadImg from "@/components/page/upload-img.vue";
// 表单结构
const defaultFormData = {
    // 表单名称
    refName: "refName",
    // 表单数据
    formData: {
        Entity: {
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
    }
};

@Component({
    mixins: [formMixin(defaultFormData)],
    components: { "upload-img": uploadImg }
})
export default class Index extends Vue {
    @Action
    getFrameworkRoles;
    @Action
    getFrameworkGroups;
    @State
    getFrameworkRolesData;
    @State
    getFrameworkGroupsData;

    sexList = sexList;
    // ★★
    filterMethod = (query, item) => {
        return item.label.indexOf(query) > -1;
    };
    // 验证 ★★★★★
    get rules() {
        if (this["status"] !== this["$actionType"].detail) {
            // 动态验证会走遍验证，暂时需要清除验证
            this.$refs[defaultFormData.refName] &&
                this.$nextTick(() => {
                    this.$refs[defaultFormData.refName].resetFields();
                });
            return {
                "Entity.ITCode": [
                    {
                        required: true,
                        message: "请输入账号",
                        trigger: "blur"
                    }
                ],
                "Entity.Password": [
                    {
                        required: true,
                        message: "请输入密码",
                        trigger: "blur"
                    }
                ],
                "Entity.Name": [
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
        return this.getFrameworkRolesData.map(item => {
            return {
                key: item.Value,
                label: item.Text,
                // 判断是否修改
                disabled: this["status"] === this["$actionType"].detail
            };
        });
    }
    // ★★
    get userGroupsData() {
        return this.getFrameworkGroupsData.map(item => {
            return {
                key: item.Value,
                label: item.Text,
                disabled: this["status"] === this["$actionType"].detail
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
        if (this["status"] !== this["$actionType"].add) {
            const parameters = { ID: this["dialogData"].ID };
            this.detail(parameters).then(res => {
                this["setFormData"](res);
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
                if (this["status"] === this["$actionType"].add) {
                    this.onAdd();
                } else if (this["status"] === this["$actionType"].edit) {
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
        const data = _.get(this, `formData.Entity.${field}`).map(item => {
            if (field === "UserGroups") {
                return item.GroupId;
            } else {
                return item.RoleId;
            }
        });
        _.set(this, `formData.Entity.${field}`, data);
    }
    // Roles&Groups数据格式与穿梭框格式不符，穿梭框格式 >>> 数据格式 ★★
    updTransferToData(field) {
        const data = _.get(this, `formData.Entity.${field}`).map(item => {
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
        _.set(this, `formData.Entity.${field}`, data);
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
