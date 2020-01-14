<template>
    <dialog-box :is-show.sync="isShow" :status="status" @close="onClose" @open="onGetFormData">
        <div class="frameworkrole-permission-form">
            <el-form :model="formData" label-width="100px">
                <el-row>
                    <el-col :span="12">
                        <wtm-form-item label="角色编号:">{{pageActionsData.Entity.RoleCode}}</wtm-form-item>
                    </el-col>
                    <el-col :span="12">
                        <wtm-form-item label="角色名称:">{{pageActionsData.Entity.RoleName}}</wtm-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="24">
                        <wtm-form-item label="备注:">
                            <el-table :data="pageActionsData.Pages" stripe border element-loading-text="拼命加载中">
                                <el-table-column label="页面" prop="Name" width="150"></el-table-column>
                                <el-table-column label="动作" width="400">
                                    <template slot-scope="scope">
                                        <span v-for="item in scope.row.AllActions" :key="item.ID">{{ item.Text }}</span>
                                    </template>
                                </el-table-column>
                            </el-table>
                        </wtm-form-item>
                    </el-col>
                </el-row>
            </el-form>
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
        Pages: {}
    }
};

@Component({
    mixins: [formMixin(defaultFormData)]
})
export default class extends Vue {
    @Action editPrivilege;
    @Action getPageActions;
    @State getPageActionsData;

    // 表单传入数据
    formData = {
        ...defaultFormData.formData
    };

    onGetFormData() {
        console.log('item:', this["dialogData"].ID);
        this.getPageActions({ ID: this["dialogData"].ID });
    }

    get pageActionsData() {
        if (this.getPageActionsData.Entity) {
            return this.getPageActionsData;
        }
        return {
            Entity: {},
            Pages: []
        };
    }
}
</script>
