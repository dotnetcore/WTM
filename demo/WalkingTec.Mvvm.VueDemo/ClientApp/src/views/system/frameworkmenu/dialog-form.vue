<template>
  <div class="frameworkmenu-form">
    <el-form :ref="refName" :model="formData" :rules="rules" label-width="100px" class="demo-ruleForm">
      <el-row>
        <el-col :span="24">
          <el-form-item label="地址类型" prop="IsInside">
            <edit-box :is-edit="status !== dialogType.detail">
              <el-radio-group v-model="formData.Entity.IsInside">
                <el-radio :label="true">
                  内部地址
                </el-radio>
                <el-radio :label="false">
                  外部地址
                </el-radio>
              </el-radio-group>
              <template #editValue>
                {{ formData.Entity.IsInside==='true' ? "内部地址" : "外部地址" }}
              </template>
            </edit-box>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row v-show="formData.Entity.IsInside">
        <el-col :span="12">
          <el-form-item label="模块名称">
            <el-select v-model="SelectedModule" v-edit:[status]="{list: pageNameList, key:'modelName', label: 'name'}" filterable placeholder="请选择" @change="onSelectedAction">
              <el-option v-for="item in pageNameList" :key="item.modelName" :label="item.name" :value="item.modelName" />
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
      <el-row v-show="!formData.Entity.IsInside">
        <el-col :span="24">
          <el-form-item label="Url">
            <el-input v-model="formData.Entity.Url" v-edit:[status] />
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
            <el-switch v-model="formData.Entity.FolderOnly" v-edit:[status] />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="菜单显示">
            <el-switch v-model="formData.Entity.ShowOnMenu" v-edit:[status] />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :span="12">
          <el-form-item label="公开">
            <el-switch v-model="formData.Entity.IsPublic" v-edit:[status] />
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
            <el-select v-model="formData.Entity.ICon" v-edit:[status]="{list: [], key:'value', label: 'label'}" filterable placeholder="请选择">
              <el-option v-for="item in []" :key="item.value" :label="item.label" :value="item.value" />
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
            ID: "",
            ParentId: "",
            DomainId: "",
            PageName: "",
            ICon: "",
            DisplayOrder: 0,
            IsInside: true,
            ShowOnMenu: false,
            IsPublic: false,
            Url: "",
            FolderOnly: false
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
    /**
     * 模版集合
     */
    get pageNameList() {
        return user.parallelMenus
            .filter(item => item.meta.ParentId)
            .map(item => {
                return {
                    name: item.name,
                    id: item.meta.Id,
                    modelName: item.path.substr(1).toLowerCase()
                };
            });
    }
    /**
     * 列表数据 区分大小写 无法对应，增加get，set
     * formData.SelectedModule
     */
    get SelectedModule() {
        const val = this["formData"].SelectedModule;
        return val ? val.toLowerCase() : "";
    }
    set SelectedModule(newValue) {
        this["formData"].SelectedModule = newValue;
    }
    created() {
        this.getFolders();
    }
    /**
     * 查询详情-end-调用
     */
    endFormData() {
        this["formData"].SelectedModule &&
            this.getActionsByModel({
                ModelName: this["formData"].SelectedModule
            });
    }
    /**
     * 动作名称
     */
    onSelectedAction() {
        this["formData"].SelectedActionIDs = [];
        this.getActionsByModel({ ModelName: this["formData"].SelectedModule });
    }
}
</script>
<style lang='less'>
</style>
