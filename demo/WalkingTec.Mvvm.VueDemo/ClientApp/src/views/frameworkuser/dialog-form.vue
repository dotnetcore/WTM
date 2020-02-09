<template>
    <wtm-dialog-box componentClass="frameworkuser-form" :is-show.sync="isShow" :status="status" @close="onClose" @open="onBindFormData">
        <el-form :ref="refName" :model="formData" :rules="rules" label-width="100px" class="demo-ruleForm">
            <el-row>
                <wtm-form-item ref="Entity.ITCode" label="账号" prop="Entity.ITCode" :span="12">
                    <el-input v-model="formData.Entity.ITCode" v-edit:[status] />
                </wtm-form-item>
                <wtm-form-item v-if="status===$actionType.add" ref="Entity.Password" label="密码" prop="Entity.Password" :span="12">
                    <el-input v-model="formData.Entity.Password" v-edit:[status] />
                </wtm-form-item>
                <wtm-form-item ref="Entity.Email" label="邮箱" :span="12">
                    <el-input v-model="formData.Entity.Email" v-edit:[status] />
                </wtm-form-item>
                <wtm-form-item ref="Entity.Name" label="姓名" prop="Entity.Name" :span="12">
                    <el-input v-model="formData.Entity.Name" v-edit:[status] />
                </wtm-form-item>
                <wtm-form-item ref="Entity.Sex" label="性别" :status="status" :span="12">
                    <el-select v-model="formData.Entity.Sex">
                        <el-option v-for="(item, index) of sexList" :key="index" :label="item.label" :value="item.value" />
                    </el-select>
                    <template #editValue>
                        {{ sexList[formData.Entity.Sex].label }}
                    </template>
                </wtm-form-item>
                <wtm-form-item ref="Entity.CellPhone" label="手机号" :span="12">
                    <el-input v-model="formData.Entity.CellPhone" v-edit:[status] />
                </wtm-form-item>
                <wtm-form-item ref="Entity.HomePhone" label="座机" :span="12">
                    <el-input v-model="formData.Entity.HomePhone" v-edit:[status] />
                </wtm-form-item>
                <wtm-form-item ref="Entity.Address" label="住址" :span="12">
                    <el-input v-model="formData.Entity.Address" v-edit:[status] />
                </wtm-form-item>
                <wtm-form-item ref="Entity.ZipCode" label="邮编" :span="12">
                    <el-input v-model="formData.Entity.ZipCode" v-edit:[status] />
                </wtm-form-item>
                <wtm-form-item ref="Entity.PhotoId" label="头像" :status="status" :span="12">
                    <upload-img :photo-id.sync="formData.Entity.PhotoId" />
                    <template #editValue>
                        <img v-if="formData.Entity.PhotoId" :src="'/api/_file/downloadFile/'+formData.Entity.PhotoId" class="avatar">
                    </template>
                </wtm-form-item>
                <wtm-form-item ref="Entity.IsValid" label="是否有效" prop="IsValid" :status="status" :span="12">
                    <el-switch v-model="formData.Entity.IsValid" />
                    <template #editValue>
                        {{ formData.Entity.IsValid===true ? "是" : "否" }}
                    </template>
                </wtm-form-item>
                <wtm-form-item ref="Entity.UserRoles" label="角色" :status="status" :span="24">
                    <el-transfer v-model="UserRoles" filterable :filter-method="filterMethod" filter-placeholder="请输入角色" :data="userRolesData" />
                    <template #editValue>
                        <el-tag v-for="item of detailRoles" :key="item.key">{{item.label}}</el-tag>
                    </template>
                </wtm-form-item>
                <wtm-form-item ref="Entity.UserGroups" label="用户组" :status="status" :span="24">
                    <el-transfer v-model="UserGroups" filterable :filter-method="filterMethod" filter-placeholder="请输入用户组" :data="userGroupsData" />
                    <template #editValue>
                        <el-tag v-for="item of detailGroups" :key="item.key">{{item.label}}</el-tag>
                    </template>
                </wtm-form-item>
            </el-row>
        </el-form>
        <dialog-footer :status="status" @onClear="onClose" @onSubmit="onSubmitForm" />
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

    sexList: Array<any> = sexList;
    UserRoles: Array<any> = [];
    UserGroups: Array<any> = [];

    // ★★
    filterMethod = (query, item) => {
        return item.label.indexOf(query) > -1;
    };

    // ★★★★★
    created() {
        this.getFrameworkRoles();
        this.getFrameworkGroups();
    }

    // 验证 ★★★★★
    get rules() {
        if (this["status"] !== this["$actionType"].detail) {
            // 动态验证会走遍验证，需要清除验证
            this.cleanValidate();
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
    // 角色列表数据
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
    // 用户组列表数据
    get userGroupsData() {
        return this.getFrameworkGroupsData.map(item => {
            return {
                key: item.Value,
                label: item.Text,
                disabled: this["status"] === this["$actionType"].detail
            };
        });
    }
    // 详情展示
    get detailRoles() {
        return this.userRolesData.filter(item =>
            this.UserRoles.includes(item.key)
        );
    }
    // 详情展示
    get detailGroups() {
        return this.userGroupsData.filter(item =>
            this.UserGroups.includes(item.key)
        );
    }

    /**
     * 绑定数据之后
     */
    afterBindFormData() {
        this.updDataToTransfer("UserRoles");
        this.updDataToTransfer("UserGroups");
    }
    // 提交 ★★★★★
    onSubmitForm() {
        this.$refs[this.refName].validate(valid => {
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
    /**
     * 上传图片 ★★★
     */
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
    /**
     * Roles&Groups数据格式与穿梭框格式不符，数据格式 >>> 穿梭框格式
     */
    updDataToTransfer(field) {
        let data = _.get(this, `formData.Entity.${field}`);
        data = data.map(item => {
            if (field === "UserGroups") {
                return item.GroupId;
            } else {
                return item.RoleId;
            }
        });
        _.set(this, field, data);
    }
    /**
     * Roles&Groups数据格式与穿梭框格式不符，穿梭框格式 >>> 数据格式
     */
    updTransferToData(field) {
        let data = _.get(this, field);
        data = data.map(item => {
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
    .el-tag {
        margin-right: 10px;
    }
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
        width: 100px;
        height: 100px;
        line-height: 100px;
        text-align: center;
    }
    .avatar {
        // width: 178px;
        min-width: 100px;
        height: 178px;
        display: block;
    }
    .el-transfer {
        display: flex;
        .el-transfer__buttons {
            display: flex;
            justify-content: center;
            align-items: center;
            flex-direction: column;
            .el-button + .el-button {
                margin-left: 0;
            }
        }
    }
}
</style>
