<template>
  <wtm-dialog-box :ref="refName" :is-show.sync="isShow" :events="formEvent" :attrs="formAttrs">
    <wtm-form-item ref="Entity.ModuleName" label="模块">
      <el-input v-model="formData.Entity.ModuleName" v-edit:[status] />
    </wtm-form-item>
    <wtm-form-item ref="Entity.ActionName" label="动作">
      <el-input v-model="formData.Entity.ActionName" v-edit:[status] />
    </wtm-form-item>
    <wtm-form-item ref="Entity.ITCode" label="ITCode">
      <el-input v-model="formData.Entity.ITCode" v-edit:[status] />
    </wtm-form-item>
    <wtm-form-item ref="Entity.ActionUrl" label="Url">
      <el-input v-model="formData.Entity.ActionUrl" v-edit:[status] />
    </wtm-form-item>
    <wtm-form-item ref="Entity.ActionTime" label="操作时间">
      <el-input v-model="formData.Entity.ActionTime" v-edit:[status] />
    </wtm-form-item>
    <wtm-form-item ref="Entity.Duration" label="时长">
      <el-input v-model="formData.Entity.Duration" v-edit:[status] />
    </wtm-form-item>
    <wtm-form-item ref="Entity.IP" label="IP">
      <el-input v-model="formData.Entity.IP" v-edit:[status] />
    </wtm-form-item>
    <wtm-form-item ref="Entity.Remark" label="备注">
      <el-input v-model="formData.Entity.Remark" v-edit:[status] />
    </wtm-form-item>
  </wtm-dialog-box>
</template>

<script lang="ts">
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
            ModuleName: "",
            ActionName: "",
            ActionTime: "",
            ActionUrl: "",
            IP: "",
            ITCode: 0,
            LogType: "",
            Duration: "",
            Remark: ""
        }
    }
};

@Component({ mixins: [formMixin(defaultFormData)] })
export default class Index extends Vue {}
</script>
