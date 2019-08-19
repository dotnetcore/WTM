<template>
  <div class="dataprivilege-add">
    <el-form :ref="refName" :model="formData" :rules="rules" label-width="100px" class="demo-ruleForm">
      <el-row>
        <el-col :span="24">
          <el-form-item label="权限类型" prop="DpType">
            <el-radio-group v-model="formData.DpType">
              <el-radio label="0">
                用户组权限
              </el-radio>
              <el-radio label="1">
                用户权限
              </el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :span="24">
          <el-form-item label="用户组" prop="GroupId">
            <el-select v-model="formData.Entity.GroupId" v-display.status="formData.Entity.GroupId" placeholder="请选择用户组">
              <el-option v-for="(item,index) of groups" :key="index" :label="item.GroupName" :value="item.ID" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :span="12">
          <el-form-item label="权限名称" prop="ID">
            <el-select v-model="formData.Entity.ID" placeholder="请选择权限名称">
              <el-option label="School" value="7adfad26-b175-4e42-9061-ca7df8eaabdc" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="全部权限" prop="IsAll">
            <el-select v-model="formData.IsAll" placeholder="请选择全部权限">
              <el-option label="是" :value="true" />
              <el-option label="否" :value="false" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :span="24">
          <el-form-item label="允许访问" prop="SelectedItemsID">
            <el-select v-model="formData.SelectedItemsID" multiple filterable placeholder="请选择允许访问" />
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>
    <dialog-footer @onClear="onClear" @onSubmit="onSubmitForm" />
  </div>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action } from "vuex-class";
import mixinDialogForm from "@/mixin/dialog-form";
import cache from "@/util/cache";
import config from "@/config/index";
// 表单结构
const defaultFormData = {
    // 表单名称
    refName: "refName",
    // 表单数据
    formData: {
        DpType: "0",
        Entity: {
            GroupId: "",
            ID: "",
            TableName: "School"
        },
        IsAll: true,
        SelectedItemsID: []
    }
};

@Component({ mixins: [mixinDialogForm(defaultFormData)] })
export default class Index extends Vue {
    @Action
    postDataprivilegeAdd;
    rules = {
        DpType: [
            { required: true, message: "请选择权限类型", trigger: "change" }
        ],
        IsAll: [
            { required: true, message: "请选择全部权限", trigger: "change" }
        ],
        ID: [{ required: true, message: "请选择权限名称", trigger: "change" }]
    };
    // 用户组
    groups = [];

    created() {
        this.groups = cache.getStorage(config.tokenKey, true).Groups || [];
    }
    // 提交
    onSubmitForm() {
        this.$refs[this["refName"]]["validate"](valid => {
            if (valid) {
                this.postDataprivilegeAdd(this["formData"]);
            } else {
                return false;
            }
        });
    }
}
</script>
<style lang='less'>
.dataprivilege-add {
}
</style>
