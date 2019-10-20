<template>
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
        <dialog-footer :status="status" @onClear="onClear" @onSubmit="onSubmitForm" />
    </div>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import mixinDialogForm from "@/util/mixin/form-mixin";
import { sexList } from "@/config/entity";
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

@Component({ mixins: [mixinDialogForm(defaultFormData)] })
export default class Index extends Vue {
    @Action add;
    @Action edit;
    @Action detail;
    @Action getFrameworkRoles;
    @Action getFrameworkGroups;
    @State
    getFrameworkRolesData;
    @State
    getFrameworkGroupsData;
    // 用户组
    groups = [];
    sexList = sexList;

    testerror = "error";
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
    /**
     * 打开详情 ★★★★★
     */
    onGetFormData() {
        if (!this["dialogData"]) {
            console.log(this["dialogData"]);
            console.error("dialogData 没有id数据");
        }
        if (this["status"] !== this["dialogType"].add) {
            const parameters = { ID: this["dialogData"].ID };
            this["detail"](parameters).then(res => {
                this["setFormData"](res);
            });
        } else {
            this["onReset"]();
        }
    }
    /**
     * 提交 ★★★★★
     */
    onSubmitForm() {
        this.$refs[this["refName"]].validate(valid => {
            if (valid) {
                if (this["status"] === this["dialogType"].add) {
                    this.onAdd();
                } else if (this["status"] === this["dialogType"].edit) {
                    this.onEdit();
                }
            }
        });
    }
    /**
     * 添加 ★★★★★
     */
    onAdd(delID: string = "ID") {
        const parameters = _.cloneDeep(this["formData"]);
        if (parameters.Entity) {
            delete parameters.Entity[delID];
        }
        this["add"](parameters)
            .then(res => {
                this["$notify"]({
                    title: "添加成功",
                    type: "success"
                });
                this["onClear"]();
                this.$emit("onSearch");
            })
            .catch(error => {
                this["showResponseValidate"](error.response.data.Form);
            });
    }
    /**
     * 编辑 ★★★★★
     */
    onEdit() {
        const parameters = _.cloneDeep(this["formData"]);
        this["edit"](parameters)
            .then(res => {
                this["$notify"]({
                    title: "修改成功",
                    type: "success"
                });
                this["onClear"]();
                this.$emit("onSearch");
            })
            .catch(error => {
                this["showResponseValidate"](error.response.data.Form);
            });
    }
}
</script>
<style lang='less'>
</style>
