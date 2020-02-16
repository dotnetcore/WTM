<template>
    <wtm-dialog-box componentClass="frameworkrole-form" :is-show.sync="isShow" :status="status" @close="onClose" @open="onBindFormData">
        <el-form :ref="refName" :model="formData" :rules="rules" label-width="100px" class="demo-ruleForm">
            <el-row>
                <wtm-form-item ref="Entity.RoleCode" label="角色编号" prop="Entity.RoleCode" :span="12">
                    <el-input v-model="formData.Entity.RoleCode" v-edit:[status] />
                </wtm-form-item>
                <wtm-form-item ref="Entity.RoleName" label="角色名称" prop="Entity.RoleName" :span="12">
                    <el-input v-model="formData.Entity.RoleName" v-edit:[status] />
                </wtm-form-item>
                <wtm-form-item ref="Entity.RoleRemark" label="备注" :span="12">
                    <el-input v-model="formData.Entity.RoleRemark" v-edit:[status] />
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
// 表单结构
const defaultFormData = {
    // 表单名称
    refName: "refName",
    // 表单数据
    formData: {
        Entity: {
            ID: "",
            RoleCode: "",
            RoleName: "",
            RoleRemark: ""
        }
    }
};

@Component({ mixins: [formMixin(defaultFormData)] })
export default class extends Vue {
    @Action
    getFrameworkRoles;
    @Action
    getFrameworkGroups;
    @State
    getFrameworkRolesData;
    @State
    getFrameworkGroupsData;
    // 验证 ★★★★★
    rules = {
        "Entity.RoleCode": [
            {
                required: true,
                message: "请输入角色编号",
                trigger: "blur"
            }
        ],
        "Entity.RoleName": [
            {
                required: true,
                message: "请输入角色名称",
                trigger: "blur"
            }
        ]
    };
}
</script>
<style lang='less'>
</style>
