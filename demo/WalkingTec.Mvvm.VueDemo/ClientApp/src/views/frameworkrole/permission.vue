<template>
    <dialog-box :is-show.sync="isShow" :status="status" @close="onClose" @open="onGetFormData">
        <div class="frameworkrole-permission-form">
            <el-form :ref="refName" :model="formData" label-width="100px">
                <el-row>
                    <el-col :span="12">
                        <wtm-form-item label="角色编号:">{{formData.Entity.RoleCode}}</wtm-form-item>
                    </el-col>
                    <el-col :span="12">
                        <wtm-form-item label="角色名称:">{{formData.Entity.RoleName}}</wtm-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="24">
                        <wtm-form-item label="备注:">
                            <el-table :data="formData.Pages" stripe border element-loading-text="拼命加载中">
                                <el-table-column label="页面" prop="Name" width="150"></el-table-column>
                                <el-table-column label="动作" width="400">
                                    <template slot-scope="scope">
                                        <el-checkbox-group v-model="formData.Pages[scope.$index].Actions">
                                            <el-checkbox v-for="item in formData.Pages[scope.$index].AllActions" :key="item.Value" :label="item.Value">{{ item.Text }}</el-checkbox>
                                        </el-checkbox-group>
                                    </template>
                                </el-table-column>
                            </el-table>
                        </wtm-form-item>
                    </el-col>
                </el-row>
            </el-form>
            <dialog-footer :status="status" @onClear="onClose" @onSubmit="onSubmitForm" />
        </div>
    </dialog-box>
</template>

<script lang='ts'>
import { Component, Vue, Prop } from "vue-property-decorator";
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
            RoleCode: "",
            RoleName: "",
            RoleRemark: ""
        },
        Pages: []
    }
};

@Component({
    mixins: [formMixin(defaultFormData)]
})
export default class extends Vue {
    @Action("editPrivilege") edit;
    @Action getPageActions;

    onGetFormData() {
        this.getPageActions({ ID: this["dialogData"].ID }).then(res => {
            this['formData'].Entity = { ...res.Entity };
            this['formData'].Pages = _.cloneDeep(res.Pages.concat());
        })
    }

}
</script>
