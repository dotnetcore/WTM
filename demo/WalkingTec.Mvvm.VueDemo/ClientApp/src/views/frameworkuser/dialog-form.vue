<template>
    <wtm-dialog-box componentClass="frameworkuser-form" :is-show.sync="isShow" :status="status" :events="formEvent">
        <wtm-create-form :ref="refName" :status="status" :options="formOptions" :events="formEvent">
            <template #UserRoles="data">
                <span v-if="data.status === $actionType.detail">
                    <el-tag v-for="item of data.data" :key="item.key">{{detailRole(item.RoleId).label}}</el-tag>
                </span>
                <el-transfer v-else v-model="UserRoles" filterable :filter-method="filterMethod" filter-placeholder="请输入角色" :data="userRolesData" />
            </template>
            <template #UserGroups="data">
                <span v-if="data.status === $actionType.detail">
                    <el-tag v-for="item of data.data" :key="item.key">{{detailGroup(item.GroupId).label}}</el-tag>
                </span>
                <el-transfer v-else v-model="UserGroups" filterable :filter-method="filterMethod" filter-placeholder="请输入用户组" :data="userGroupsData" />
            </template>
        </wtm-create-form>
    </wtm-dialog-box>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import formMixin from "@/vue-custom/mixin/form-mixin";
import { sexList } from "@/config/entity";
import UploadImg from "@/components/page/UploadImg.vue";

@Component({
    mixins: [formMixin()]
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

    UserRoles: Array<any> = [];
    UserGroups: Array<any> = [];
    mergeFormData = {
        Entity: {
            UserRoles: "",
            UserGroups: ""
        }
    };
    // 表单结构
    get formOptions() {
        return {
            formProps: {
                "label-width": "100px"
            },
            formItem: {
                "Entity.ID": {
                    isHidden: true
                },
                "Entity.ITCode": {
                    type: "input",
                    label: "账号",
                    rules: {
                        required: true,
                        message: "请输入账号",
                        trigger: "blur"
                    }
                },
                "Entity.Password": {
                    type: "input",
                    label: "密码",
                    rules: {
                        required: true,
                        message: "请输入账号",
                        trigger: "blur"
                    },
                    isHidden: (res, status) => status === "edit"
                },
                "Entity.Email": {
                    type: "input",
                    label: "邮箱"
                },
                "Entity.Name": {
                    type: "input",
                    label: "姓名",
                    rules: {
                        required: true,
                        message: "请输入姓名",
                        trigger: "blur"
                    }
                },
                "Entity.Sex": {
                    type: "select",
                    label: "性别",
                    children: sexList
                },
                "Entity.CellPhone": {
                    type: "input",
                    label: "手机号"
                },
                "Entity.HomePhone": {
                    type: "input",
                    label: "座机"
                },
                "Entity.Address": {
                    type: "input",
                    label: "住址"
                },
                "Entity.ZipCode": {
                    type: "input",
                    label: "邮编"
                },
                "Entity.PhotoId": {
                    type: "wtmUploadImg",
                    label: "头像"
                },
                "Entity.IsValid": {
                    type: "switch",
                    label: "是否有效",
                    defaultValue: true
                },
                "Entity.UserRoles": {
                    type: "wtmSlot",
                    label: "角色",
                    span: 24,
                    slotKey: "UserRoles"
                },
                "Entity.UserGroups": {
                    type: "wtmSlot",
                    label: "用户组",
                    span: 24,
                    slotKey: "UserGroups"
                }
            }
        };
    }

    // 是否 自定义搜索方法
    filterMethod = (query, item) => {
        return item.label.indexOf(query) > -1;
    };

    created() {
        this.getFrameworkRoles();
        this.getFrameworkGroups();
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
    // 角色-详情展示
    detailRole(key) {
        return _.find(this.userRolesData, { key: key }) || {};
    }
    // 用户组-详情展示
    detailGroup(key) {
        return _.find(this.userGroupsData, { key: key }) || {};
    }

    /**
     * 绑定数据之后
     */
    afterOpen() {
        this.updDataToTransfer("UserRoles");
        this.updDataToTransfer("UserGroups");
    }
    // 提交
    onSubmit() {
        this.FormComp().validate(valid => {
            if (valid) {
                _.set(
                    this.mergeFormData,
                    "Entity.UserRoles",
                    this.updTransferToData("UserRoles")
                );
                _.set(
                    this.mergeFormData,
                    "Entity.UserGroups",
                    this.updTransferToData("UserGroups")
                );
                if (this["status"] === this["$actionType"].add) {
                    this.onAdd();
                } else if (this["status"] === this["$actionType"].edit) {
                    this.onEdit();
                }
            }
        });
    }
    /**
     * 上传图片
     */
    handleAvatarSuccess(res, file) {
        this["formData"].PhotoId = res.Id; // URL.createObjectURL(file.raw);
    }
    //
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
        let data = _.get(this.FormComp().getFormData(), `Entity.${field}`);
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
        return data;
    }
}
</script>
<style lang="less">
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
