<template>
  <wtm-dialog-box componentClass="frameworkmenu-form" :is-show.sync="isShow" :status="status" @close="onClose" @open="onBindFormData">
    <el-form :ref="refName" :model="formData" :rules="rules" label-width="100px" class="demo-ruleForm">
      <el-row>
        <wtm-form-item ref="Entity.IsInside" label="地址类型" prop="Entity.IsInside" :status="status" :span="24">
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
        </wtm-form-item>
      </el-row>
      <el-row v-show="formData.Entity.IsInside">
        <wtm-form-item label="模块名称" :span="12">
          <el-select v-model="formData.SelectedModule" filterable placeholder="请选择" @change="onSelectedAction">
            <el-option v-for="item in moduleList" :key="item.Value" :label="item.Text" :value="item.Value" />
          </el-select>
          <template #editValue>
            {{ moduleList[formData.SelectedModule].Text }}
          </template>
        </wtm-form-item>
        <wtm-form-item label="动作名称" :status="status" :span="12">
          <el-select v-model="formData.SelectedActionIDs" v-edit:[status] multiple placeholder="请选择">
            <el-option v-for="item in getActionsByModelData" :key="item.Value" :label="item.Text" :value="item.Value" />
          </el-select>
          <template #editValue>
            {{formData.SelectedActionIDs}}
          </template>
        </wtm-form-item>
      </el-row>
      <el-row v-show="!formData.Entity.IsInside">
        <wtm-form-item ref="Entity.Url" label="Url" :span="24">
          <el-input v-model="formData.Entity.Url" v-edit:[status] />
        </wtm-form-item>
      </el-row>
      <el-row>
        <wtm-form-item ref="Entity.PageName" label="页面名称" :span="12">
          <el-input v-model="formData.Entity.PageName" v-edit:[status] placeholder="请输入内容" />
        </wtm-form-item>
        <wtm-form-item ref="Entity.ParentId" label="父目录" :span="12">
          <el-select v-model="formData.Entity.ParentId" filterable placeholder="请选择">
            <el-option v-for="item in getFoldersData" :key="item.Value" :label="item.Text" :value="item.Value" />
          </el-select>
          <template #editValue>
            {{ getFoldersData[formData.Entity.ParentId].Text }}
          </template>
        </wtm-form-item>
      </el-row>

      <el-row>
        <wtm-form-item ref="Entity.FolderOnly" label="目录" :span="12">
          <el-switch v-model="formData.Entity.FolderOnly" v-edit:[status] />
        </wtm-form-item>
        <wtm-form-item ref="Entity.ShowOnMenu" label="菜单显示" :span="12">
          <el-switch v-model="formData.Entity.ShowOnMenu" v-edit:[status] />
        </wtm-form-item>
        <wtm-form-item ref="Entity.IsPublic" label="公开" :span="12">
          <el-switch v-model="formData.Entity.IsPublic" v-edit:[status] />
        </wtm-form-item>
        <wtm-form-item ref="Entity.DisplayOrder" label="顺序" :span="12">
          <el-input v-model="formData.Entity.DisplayOrder" v-edit:[status] placeholder="请输入顺序" />
        </wtm-form-item>
        <wtm-form-item ref="Entity.ICon" label="图标" :span="24">
          <el-select v-model="formData.Entity.ICon" v-edit:[status] filterable placeholder="请选择">
            <el-option v-for="item in []" :key="item.value" :label="item.label" :value="item.value" />
          </el-select>
          <template #editValue>
            {{ formData.Entity.ICon }}
          </template>
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
import { RoutesModule } from "@/store/modules/routes";

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

@Component({ mixins: [mixinForm(defaultFormData)] })
export default class Index extends Vue {
    @Action
    getActionsByModel;
    @Action
    getFolders;
    @State
    getActionsByModelData;
    @State
    getFoldersData;

    // 验证 ★★★★★
    get rules() {
        if (this["status"] !== this["$actionType"].detail) {
            // 动态验证会走遍验证，需要清除验证
            this.cleanValidate();
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
    get moduleList() {
        return RoutesModule.pageList;
    }
    // /**
    //  * 列表数据 区分大小写 无法对应，增加get，set
    //  * formData.SelectedModule
    //  */
    // get SelectedModule() {
    //     const val = _.get(this, `formData.Entity.SelectedModule`);
    //     return val ? val.toLowerCase() : "";
    // }
    // set SelectedModule(newValue) {
    //     _.set(this, `formData.Entity.SelectedModule`, newValue);
    // }
    created() {
        this.getFolders();
    }
    /**
     * 查询详情-after-调用
     */
    afterBindFormData(data) {
        _.set(this, `formData.SelectedModule`, data.SelectedModule);
        _.set(this, `formData.SelectedActionIDs`, data.SelectedActionIDs);
        this.onSelectedAction(false);
    }
    /**
     * 动作名称
     */
    onSelectedAction(isClear) {
        isClear && _.set(this, `formData.SelectedActionIDs`, []);
        this.getActionsByModel({
            ModelName: _.get(this, `formData.SelectedModule`)
        });
    }
}
</script>
<style lang='less'>
</style>
