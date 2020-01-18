<template>
  <wtm-dialog-box :is-show.sync="isShow" :status="status" @close="onClose" @open="onGetFormData">
    <div class="frameworkmenu-form">
      <el-form :ref="refName" :model="formData" label-width="100px" class="demo-ruleForm">
          <el-row>
              <el-col :span="12">
                  <wtm-form-item ref="Entity.ModuleName" label="模块">
                      <el-input v-model="formData.Entity.ModuleName" v-edit:[status] />
                  </wtm-form-item>
              </el-col>
              <el-col :span="12">
                  <wtm-form-item ref="Entity.ActionName" label="动作">
                      <el-input v-model="formData.Entity.ActionName" v-edit:[status] />
                  </wtm-form-item>
              </el-col>
              <el-col :span="12">
                  <wtm-form-item ref="Entity.ITCode" label="ITCode">
                      <el-switch v-model="formData.Entity.ITCode" v-edit:[status] />
                  </wtm-form-item>
              </el-col>
              <el-col :span="12">
                  <wtm-form-item ref="Entity.ActionUrl" label="Url">
                      <el-switch v-model="formData.Entity.ActionUrl" v-edit:[status] />
                  </wtm-form-item>
              </el-col>
              <el-col :span="12">
                  <wtm-form-item ref="Entity.ActionTime" label="操作时间">
                      <el-switch v-model="formData.Entity.ActionTime" v-edit:[status] />
                  </wtm-form-item>
              </el-col>
              <el-col :span="12">
                  <wtm-form-item ref="Entity.Duration" label="时长">
                      <el-switch v-model="formData.Entity.Duration" v-edit:[status] />
                  </wtm-form-item>
              </el-col>
              <el-col :span="12">
                  <wtm-form-item ref="Entity.IP" label="IP">
                      <el-switch v-model="formData.Entity.IP" v-edit:[status] />
                  </wtm-form-item>
              </el-col>
              <el-col :span="12">
                  <wtm-form-item ref="Entity.Remark" label="备注">
                      <el-switch v-model="formData.Entity.Remark" v-edit:[status] />
                  </wtm-form-item>
              </el-col>
          </el-row>
      </el-form>
      <dialog-footer :status="status" @onClear="onClose" />
    </div>
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
            ModuleName: "",
            ActionName: "",
            ActionTime: "",
            ActionUrl: "",
            IP: "",
            ITCode: 0,
            LogType: true,
            Duration: false,
            Remark: false
        }
    }
};

@Component({ mixins: [formMixin(defaultFormData)] })
export default class Index extends Vue {
    @Action detail;
    @State getFoldersData;
}
</script>
<style lang='less'>
</style>
