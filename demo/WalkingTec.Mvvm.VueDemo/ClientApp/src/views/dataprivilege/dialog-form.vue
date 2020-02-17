<template>
  <wtm-dialog-box componentClass="dataprivilege-add" :is-show.sync="isShow" :status="status" @close="onClose" @open="onBindFormData">
    <el-form :ref="refName" :model="formData" :rules="rules" label-width="100px" class="demo-ruleForm">
      <el-row>
        <wtm-form-item label="权限类型" prop="DpType" :status="status" :span="24">
          <el-radio-group v-model="formData.DpType" v-edit:[status]>
            <el-radio :label="0">
              用户组权限
            </el-radio>
            <el-radio :label="1">
              用户权限
            </el-radio>
          </el-radio-group>
          <template #editValue>
            {{ formData.DpType === 0 ? "用户组权限" : "用户权限" }}
          </template>
        </wtm-form-item>
        <wtm-form-item v-if="formData.DpType === 0" label="用户组" prop="Entity.GroupId" :span="12">
          <el-select v-model="formData.Entity.GroupId" v-edit:[status]="getUserGroupsData" placeholder="请选择用户组">
            <el-option v-for="(item,index) of getUserGroupsData" :key="index" :label="item.Text" :value="item.Value" />
          </el-select>
        </wtm-form-item>
        <wtm-form-item v-if="formData.DpType === 1" label="用户Id" prop="UserItCode" ref="UserItCode" :span="12">
          <el-input v-model="formData.UserItCode" v-edit:[status] />
        </wtm-form-item>
        <wtm-form-item label="权限名称" prop="Entity.TableName" ref="Entity.TableName" :span="12">
          <el-select v-model="formData.Entity.TableName" v-edit:[status]="getPrivilegesData" placeholder="请选择权限名称" @change="onPrivileges">
            <el-option v-for="(item,index) of getPrivilegesData" :key="index" :label="item.Text" :value="item.Value" />
          </el-select>
        </wtm-form-item>
        <wtm-form-item label="全部权限" prop="IsAll" :span="12">
          <el-select v-model="formData.IsAll" v-edit:[status]="whether" placeholder="请选择全部权限">
            <el-option v-for="(item, index) of whether" :key="index" :label="item.Text" :value="item.Value" />
          </el-select>
        </wtm-form-item>
        <wtm-form-item label="允许访问" prop="SelectedItemsID" :span="12">
          <el-select v-model="formData.SelectedItemsID" v-edit:[status] :disabled="formData.IsAll" multiple filterable placeholder="请选择允许访问">
            <el-option v-for="(item, index) of getPrivilegeByTableNameData" :key="index" :label="item.Text" :value="item.Value" />
          </el-select>
        </wtm-form-item>
      </el-row>
    </el-form>
    <dialog-footer :status="status" @onClear="onClose" @onSubmit="onSubmitForm" />
  </wtm-dialog-box>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import mixinForm from "@/vue-custom/mixin/form-mixin";
import { whether } from "@/config/entity";
// 表单结构
const defaultFormData = {
    // 表单名称
    refName: "refName",
    // 表单数据
    formData: {
        DpType: 0,
        UserItCode: "",
        Entity: {
            GroupId: "",
            ID: "",
            TableName: ""
        },
        IsAll: true,
        SelectedItemsID: []
    }
};

@Component({ mixins: [mixinForm(defaultFormData)] })
export default class Index extends Vue {
    @Action("get")
    detail;
    @Action
    getUserGroups;
    @Action
    getPrivileges;
    @Action
    getPrivilegeByTableName;

    @State
    getUserGroupsData;
    @State
    getPrivilegesData;
    @State
    getPrivilegeByTableNameData;
    // 是否列表
    whether = whether;
    rules = {
        DpType: [
            {
                type: "number",
                required: true,
                message: "请选择权限类型",
                trigger: "change"
            }
        ],
        IsAll: [
            {
                type: "boolean",
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
        ],
        UserItCode: [
            {
                required: true,
                message: "用户Id",
                trigger: "blur"
            }
        ]
    };

    created() {
        this.getUserGroups();
        this.getPrivileges();
    }

    /**
     * 查询详情-after-调用
     */
    afterBindFormData(data) {
        _.set(this, `formData.DpType`, data.DpType);
        _.set(this, `formData.IsAll`, data.IsAll);
        _.set(this, `formData.SelectedItemsID`, data.SelectedItemsID);
        this.onPrivileges(false);
    }
    /**
     * 权限名称
     */
    onPrivileges(isClear: boolean) {
        isClear && _.set(this, `formData.SelectedItemsID`, []);
        this.getPrivilegeByTableName({
            table: _.get(this, `formData.Entity.TableName`)
        });
    }
}
</script>
<style lang='less'>
.dataprivilege-add {
}
</style>
