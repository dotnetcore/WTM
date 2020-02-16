<template>
  <wtm-dialog-box componentClass="frameworkgroup-form" :is-show.sync="isShow" :status="status" @close="onClose" @open="onBindFormData">
    <el-form :ref="refName" :model="formData" :rules="rules" label-width="100px" class="demo-ruleForm">
      <el-row>
        <wtm-form-item ref="Entity.GroupCode" label="用户组编码" prop="Entity.GroupCode" :span="12">
          <el-input v-model="formData.Entity.GroupCode" v-edit:[status] />
        </wtm-form-item>
        <wtm-form-item ref="Entity.GroupName" label="用户组名称" prop="Entity.GroupName" :span="12">
          <el-input v-model="formData.Entity.GroupName" v-edit:[status] />
        </wtm-form-item>
        <wtm-form-item ref="Entity.GroupRemark" label="备注" :span="24">
          <el-input v-model="formData.Entity.GroupRemark" v-edit:[status] />
        </wtm-form-item>
      </el-row>
    </el-form>
    <dialog-footer :status="status" @onClear="onClose" @onSubmit="onSubmitForm" />
  </wtm-dialog-box>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action } from "vuex-class";
import mixinForm from "@/vue-custom/mixin/form-mixin";
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
    // 验证 ★★★★★
    rules = {
        "Entity.GroupCode": [
            {
                required: true,
                message: "请输入用户组编号",
                trigger: "blur"
            }
        ],
        "Entity.GroupName": [
            {
                required: true,
                message: "请输入用户组名称",
                trigger: "blur"
            }
        ]
    };
}
</script>
<style lang='less'>
</style>
