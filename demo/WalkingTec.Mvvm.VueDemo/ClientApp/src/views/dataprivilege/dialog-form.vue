<template>
  <wtm-dialog-box componentClass="dataprivilege-add" :is-show.sync="isShow" :status="status" @close="onClose" @open="onBindFormData">
    <el-form :ref="refName" :model="formData" :rules="rules" label-width="100px" class="demo-ruleForm">
      <el-row>
        <wtm-form-item label="权限类型" prop="DpType" :span="24">
          <el-radio-group v-model="formData.DpType" v-edit:[status]>
            <el-radio label="0">
              用户组权限
            </el-radio>
            <el-radio label="1">
              用户权限
            </el-radio>
          </el-radio-group>
        </wtm-form-item>
        <wtm-form-item label="用户组" prop="Entity.GroupId" :span="24">
          <el-select v-model="formData.Entity.GroupId" v-edit:[status]="{list: getUserGroupsData, key:'Value', label: 'Text'}" placeholder="请选择用户组">
            <el-option v-for="(item,index) of getUserGroupsData" :key="index" :label="item.Text" :value="item.Value" />
          </el-select>
        </wtm-form-item>
        <wtm-form-item label="权限名称" prop="Entity.ID" :span="12">
          <el-select v-model="formData.Entity.ID" v-edit:[status] placeholder="请选择权限名称">
            <el-option label="School" :value="'7adfad26-b175-4e42-9061-ca7df8eaabdc'" />
            <el-option v-for="(item,index) of getPrivilegesData" :key="index" :label="item.Text" :value="item.Value" />
          </el-select>
        </wtm-form-item>
        <wtm-form-item label="全部权限" prop="IsAll" :span="12">
          <el-select v-model="formData.IsAll" v-edit:[status]="{list: whether, key:'value', label: 'label'}" placeholder="请选择全部权限">
            <el-option v-for="(item, index) of whether" :key="index" :label="item.label" :value="item.value" />
          </el-select>
        </wtm-form-item>
        <!-- <wtm-form-item label="允许访问" prop="SelectedItemsID" :span="24">
          <el-select v-model="formData.SelectedItemsID" v-edit:[status] multiple filterable placeholder="请选择允许访问" />
        </wtm-form-item> -->
      </el-row>
    </el-form>
    <dialog-footer :status="status" @onClear="onClose" @onSubmit="onSubmitForm" />
  </wtm-dialog-box>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import mixinForm from "@/vue-custom/mixin/form-mixin";
import cache from "@/util/cache";
import { whether } from "@/config/entity";
// 表单结构
const defaultFormData = {
    // 表单名称
    refName: "refName",
    // 表单数据
    formData: {
        ID: "",
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

@Component({ mixins: [mixinForm(defaultFormData)] })
export default class Index extends Vue {
    @Action
    getUserGroups;
    @Action
    getPrivileges;
    @State
    getUserGroupsData;
    @State
    getPrivilegesData;
    // 是否列表
    whether = whether;
    // 验证
    get rules() {
        if (this["status"] !== this["$actionType"].detail) {
            // 动态验证会走遍验证，需要清除验证
            this.cleanValidate();
            return {
                DpType: [
                    {
                        required: true,
                        message: "请选择权限类型",
                        trigger: "change"
                    }
                ],
                IsAll: [
                    {
                        required: true,
                        message: "请选择全部权限",
                        trigger: "change"
                    }
                ],
                "Entity.ID": [
                    {
                        required: true,
                        message: "请选择权限名称",
                        trigger: "change"
                    }
                ],
                "Entity.GroupId": [
                    {
                        required: true,
                        message: "请选择用户组",
                        trigger: "change"
                    }
                ]
            };
        } else {
            return {};
        }
    }

    created() {
        this.getUserGroups();
        this.getPrivileges();
    }
}
</script>
<style lang='less'>
.dataprivilege-add {
}
</style>
