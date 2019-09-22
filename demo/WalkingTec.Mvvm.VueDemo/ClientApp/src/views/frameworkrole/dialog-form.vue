<template>
  <div class="frameworkrole-form">
    <el-form :ref="refName" :model="formData" :rules="rules" label-width="100px" class="demo-ruleForm">
      <el-row>
        <el-col :span="12">
          <el-form-item label="角色编号" prop="RoleCode">
            <el-input v-model="formData.RoleCode" v-edit:[status] />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="角色名称" prop="RoleName">
            <el-input v-model="formData.RoleName" v-edit:[status] />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :span="24">
          <el-form-item label="备注">
            <el-input v-model="formData.RoleRemark" v-edit:[status] />
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
// 表单结构
const defaultFormData = {
    // 表单名称
    refName: "refName",
    // 表单数据
    formData: {
        ID: "",
        RoleCode: "",
        RoleName: "",
        RoleRemark: ""
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
                RoleCode: [
                    {
                        required: true,
                        message: "请输入角色编号",
                        trigger: "blur"
                    }
                ],
                RoleName: [
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
    // ★★★★★
    created() {}
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
                this["setFormData"](res.Entity);
            });
        } else {
            this["onReset"]();
        }
    }
    /**
     * 提交 ★★★★★
     */
    onSubmitForm() {
        console.log("e.test", this.$refs["e.test"]);
        this.$refs["e.test"].error = "123";
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
        const parameters = { ...this["formData"] };
        delete parameters[delID];
        this["add"]({ Entity: parameters }).then(res => {
            this["$notify"]({
                title: "添加成功",
                type: "success"
            });
            this["onClear"]();
            this.$emit("onSearch");
        });
    }
    /**
     * 编辑 ★★★★★
     */
    onEdit() {
        const parameters = { ...this["formData"] };
        this["edit"]({ Entity: parameters }).then(res => {
            this["$notify"]({
                title: "修改成功",
                type: "success"
            });
            this["onClear"]();
            this.$emit("onSearch");
        });
    }
}
</script>
<style lang='less'>
</style>
