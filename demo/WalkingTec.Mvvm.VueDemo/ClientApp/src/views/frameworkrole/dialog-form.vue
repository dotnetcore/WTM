<template>
    <dialog-box :is-show.sync="isShow" :status="status" @close="onClose" @open="onGetFormData">
        <div class="frameworkrole-form">
            <el-form :ref="refName" :model="formData" :rules="rules" label-width="100px" class="demo-ruleForm">
                <el-row>
                    <el-col :span="12">
                        <wtm-form-item ref="Entity.RoleCode" label="角色编号" prop="Entity.RoleCode">
                            <el-input v-model="formData.Entity.RoleCode" v-edit:[status] />
                        </wtm-form-item>
                    </el-col>
                    <el-col :span="12">
                        <wtm-form-item ref="Entity.RoleName" label="角色名称" prop="Entity.RoleName">
                            <el-input v-model="formData.Entity.RoleName" v-edit:[status] />
                        </wtm-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="24">
                        <wtm-form-item ref="Entity.RoleRemark" label="备注">
                            <el-input v-model="formData.Entity.RoleRemark" v-edit:[status] />
                        </wtm-form-item>
                    </el-col>
                </el-row>
            </el-form>
            <dialog-footer :status="status" @onClear="onClose" @onSubmit="onSubmitForm" />
        </div>
    </dialog-box>
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
    @Action getFrameworkRoles;
    @Action getFrameworkGroups;
    @State getFrameworkRolesData;
    @State getFrameworkGroupsData;
    // 验证 ★★★★★
    get rules() {
        if (this["status"] !== this["dialogType"].detail) {
            // 动态验证会走遍验证，需要清除验证
            this.$refs[defaultFormData.refName] && this.$nextTick(() => {
                this.$refs[defaultFormData.refName].resetFields();
            });
            return {
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
        } else {
            return {};
        }
    }
}
</script>
<style lang='less'>
</style>
