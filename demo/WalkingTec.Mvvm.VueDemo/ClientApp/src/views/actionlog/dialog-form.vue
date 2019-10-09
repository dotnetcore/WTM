<template>
  <div class="frameworkmenu-form">
    <el-form :ref="refName" :model="formData" :rules="rules" label-width="100px" class="demo-ruleForm">
      <el-row>
        <el-col :span="12">
          <wtm-form-item ref="Entity.ModuleName" label="模块">
            <el-input v-model="formData.Entity.ModuleName" v-edit:[status] placeholder="请输入内容" />
          </wtm-form-item>
        </el-col>
        <el-col :span="12">
          <wtm-form-item ref="Entity.ActionName" label="动作">
            <el-select v-model="formData.Entity.ActionName" v-edit:[status]="{list: getFoldersData, key:'Value', label: 'Text'}" filterable placeholder="请选择">
              <el-option v-for="item in getFoldersData" :key="item.id" :label="item.Text" :value="item.Value" />
            </el-select>
          </wtm-form-item>
        </el-col>
      </el-row>
      <el-row>
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
      </el-row>
      <el-row>
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
      </el-row>
      <el-row>
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
    <dialog-footer :status="status" @onClear="onClear" @onSubmit="onSubmitForm" />
  </div>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action } from "vuex-class";
import mixinDialogForm from "@/util/mixin/form-mixin";
// 表单结构
const defaultFormData = {
    // 表单名称
    refName: "refName",
    // 表单数据
    formData: {
        Entity: {
            ID: "",
            ActionName: "",
            ActionTime: "",
            ActionUrl: "",
            IP: "",
            ITCode: 0,
            LogType: true,
            ModuleName: false,
            Duration: false,
            Remark: false
        }
    }
};

@Component({ mixins: [mixinDialogForm(defaultFormData)] })
export default class Index extends Vue {
    @Action detail;
}
</script>
<style lang='less'>
</style>
