<template>
  <dialog-box :is-show.sync="isShow" :status="status" @close="onClose" @open="onGetFormData">
    <div class="frameworkgroup-form">
      <el-form :ref="refName" :model="formData" :rules="rules" label-width="100px" class="demo-ruleForm">
        <el-row>
          <el-col :span="12">
            <wtm-form-item ref="Entity.GroupCode" label="用户组编码" prop="Entity.GroupCode">
              <el-input v-model="formData.Entity.GroupCode" v-edit:[status] />
            </wtm-form-item>
          </el-col>
          <el-col :span="12">
            <wtm-form-item ref="Entity.GroupName" label="用户组名称" prop="Entity.GroupName">
              <el-input v-model="formData.Entity.GroupName" v-edit:[status] />
            </wtm-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="24">
            <wtm-form-item ref="Entity.GroupRemark" label="备注">
              <el-input v-model="formData.Entity.GroupRemark" v-edit:[status] />
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
import { Action } from "vuex-class";
import mixinForm from "@/util/mixin/form-mixin";
// 表单结构
const defaultFormData = {
    // 表单名称
    refName: "refName",
    // 表单数据
    formData: {
        Entity: {
            ID: "",
            GroupCode: "",
            GroupName: "",
            GroupRemark: ""
        }
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
            this.$refs[defaultFormData.refName] && this.$nextTick(() => {
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
