<template>
  <div class="frameworkmenu-form">
    <el-form :ref="refName" :model="formData" :rules="rules" label-width="100px" class="demo-ruleForm">
      <el-row>
        <el-col :span="12">
          <el-form-item label="用户组编码" prop="GroupCode">
            <el-input v-model="formData.GroupCode" v-edit:[status] />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="用户组名称" prop="GroupName">
            <el-input v-model="formData.GroupName" v-edit:[status] />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :span="24">
          <el-form-item label="备注">
            <el-input v-model="formData.GroupRemark" v-edit:[status] />
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>
    <dialog-footer :status="status" @onClear="onClear" @onSubmit="onSubmitForm" />
  </div>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action } from "vuex-class";
import mixinDialogForm from "@/mixin/form-mixin";
// 表单结构
const defaultFormData = {
    // 表单名称
    refName: "refName",
    // 表单数据
    formData: {
        ID: "",
        GroupCode: "",
        GroupName: "",
        GroupRemark: ""
    }
};

@Component({ mixins: [mixinDialogForm(defaultFormData)] })
export default class Index extends Vue {
    @Action
    postFrameworkmenuAdd;
    @Action
    putFrameworkmenuEdit;
    @Action
    getFrameworkmenu;
    // 验证 ★★★★★
    get rules() {
        if (this["status"] !== this["dialogType"].detail) {
            // 动态验证会走遍验证，需要清除验证
            this.$nextTick(() => {
                this.$refs[defaultFormData.refName].resetFields();
            });
            return {
                GroupCode: [
                    {
                        required: true,
                        message: "请输入用户组编号",
                        trigger: "blur"
                    }
                ],
                GroupName: [
                    {
                        required: true,
                        message: "请输入用户组名称",
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
    // 打开详情 ★★★★★
    onGetFormData() {
        if (!this["dialogData"]) {
            console.log(this["dialogData"]);
            console.error("dialogData 没有id数据");
        }
        if (this["status"] !== this["dialogType"].add) {
            const parameters = { ID: this["dialogData"].ID };
            this.getFrameworkmenu(parameters).then(res => {
                this["setFormData"](res.Entity);
            });
        } else {
            this["onReset"]();
        }
    }
    // 提交 ★★★★★
    onSubmitForm() {
        this.$refs[defaultFormData.refName].validate(valid => {
            if (valid) {
                if (this["status"] === this["dialogType"].add) {
                    this.onAdd();
                } else if (this["status"] === this["dialogType"].edit) {
                    this.onEdit();
                }
            }
        });
    }
    // ★★★★★
    onAdd() {
        const parameters = { ...this["formData"] };
        delete parameters.ID;
        this.postFrameworkmenuAdd({ Entity: parameters }).then(res => {
            this["$notify"]({
                title: "添加成功",
                type: "success"
            });
            this["onClear"]();
            this.$emit("onSearch");
        });
    }
    // ★★★★★
    onEdit() {
        const parameters = { ...this["formData"] };
        this.putFrameworkmenuEdit({ Entity: parameters }).then(res => {
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
