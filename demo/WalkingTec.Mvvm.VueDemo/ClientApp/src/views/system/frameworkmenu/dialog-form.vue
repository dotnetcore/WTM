<template>
  <div class="frameworkmenu-form">
    <el-form :ref="refName" :model="formData" :rules="rules" label-width="100px" class="demo-ruleForm">
      <el-row>
        <el-col :span="24">
          <el-form-item label="地址类型" prop="IsInside">
            <edit-box :is-edit="status !== dialogType.detail">
              <el-radio-group v-model="formData.Entity.IsInside">
                <el-radio label="内部地址" :value="true" />
                <el-radio label="外部地址" :value="false" />
              </el-radio-group>
              <template #editValue>
                {{ formData.Entity.IsValid==='true' ? "内部地址" : "外部地址" }}
              </template>
            </edit-box>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :span="12">
          <el-form-item label="模块名称">
            <el-select v-model="formData.SelectedModule" v-edit:[status]="{list: pageNameList, key:'id', label: 'name'}" filterable placeholder="请选择">
              <el-option v-for="item in pageNameList" :key="item.id" :label="item.name" :value="item.id" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="动作名称">
            <edit-box :is-edit="status !== dialogType.detail">
              <el-select v-model="formData.SelectedActionIDs" v-edit:[status] multiple placeholder="请选择">
                <el-option v-for="item in getActionsByModelData" :key="item.Value" :label="item.Text" :value="item.Value" />
              </el-select>
              <template #editValue>
                动作名称 value
              </template>
            </edit-box>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :span="12">
          <el-form-item label="页面名称">
            <el-input v-model="formData.Entity.PageName" v-edit:[status] placeholder="请输入内容" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="父目录">
            <el-select v-model="formData.Entity.ParentId" v-edit:[status]="{list: getFoldersData, key:'Value', label: 'Text'}" filterable placeholder="请选择">
              <el-option v-for="item in getFoldersData" :key="item.id" :label="item.Text" :value="item.Value" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>

      <el-row>
        <el-col :span="12">
          <el-form-item label="目录">
            <el-switch v-model="formData.Entity.FolderOnly" v-edit:[status] active-value="true" inactive-value="false" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="菜单显示">
            <el-switch v-model="formData.Entity.ShowOnMenu" v-edit:[status] active-value="true" inactive-value="false" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :span="12">
          <el-form-item label="公开">
            <el-switch v-model="formData.Entity.IsPublic" v-edit:[status] active-value="true" inactive-value="false" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="顺序">
            <el-input v-model="formData.Entity.DisplayOrder" v-edit:[status] placeholder="请输入顺序" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :span="24">
          <el-form-item label="图标">
            <el-select v-model="formData.Entity.ICon" v-edit:[status]="{list: options, key:'value', label: 'label'}" filterable placeholder="请选择">
              <el-option v-for="item in options" :key="item.value" :label="item.label" :value="item.value" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>
    <dialog-footer :status="status" @onClear="onClear" @onSubmit="onSubmitForm" />
  </div>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import mixinDialogForm from "@/mixin/form-mixin";
import user from "@/store/common/user";
// 表单结构
const defaultFormData = {
    // 表单名称
    refName: "refName",
    // 表单数据
    formData: {
        SelectedModule: "",
        SelectedActionIDs: [],
        Entity: {
            ParentId: "",
            DomainId: "",
            PageName: "",
            ICon: "",
            DisplayOrder: 0,
            IsInside: true,
            ShowOnMenu: false,
            IsPublic: false
        }
    }
};

@Component({ mixins: [mixinDialogForm(defaultFormData)] })
export default class Index extends Vue {
    @Action add;
    @Action edit;
    @Action detail;
    @Action getActionsByModel;
    @Action getFolders;
    @State getActionsByModelData;
    @State getFoldersData;
    options: string[] = [];
    // 模版集合
    get pageNameList() {
        return user.parallelMenus
            .filter(item => item.meta.ParentId)
            .map(item => {
                return {
                    name: item.name,
                    id: item.meta.Id
                };
            });
    }

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
    created() {
        this.getActionsByModel({ ModelName: "FrameworkUser" });
        this.getFolders();
    }
}
</script>
<style lang='less'>
</style>
