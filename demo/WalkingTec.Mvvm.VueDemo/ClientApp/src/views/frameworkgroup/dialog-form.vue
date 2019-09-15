<template>
  <div class="frameworkgroup-form">
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
import mixinForm from "@/util/mixin/form-mixin";
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

@Component({ mixins: [mixinForm(defaultFormData)] })
export default class Index extends Vue {
    @Action add;
    @Action edit;
    @Action detail;
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
}
</script>
<style lang='less'>
</style>
