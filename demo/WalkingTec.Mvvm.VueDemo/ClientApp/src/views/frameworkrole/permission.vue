<template>
    <dialog-box :is-show.sync="isShow">
        <div class="frameworkrole-permission-form">
            <el-form :model="formData" label-width="100px">
                <el-row>
                    <el-col :span="12">
                        <wtm-form-item label="角色编号">
                            <el-input v-model="formData.Entity.RoleCode" />
                        </wtm-form-item>
                    </el-col>
                    <el-col :span="12">
                        <wtm-form-item label="角色名称">
                            <el-input v-model="formData.Entity.RoleName" />
                        </wtm-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="24">
                        <wtm-form-item label="备注">
                            <el-table stripe border element-loading-text="拼命加载中">
                                <el-table-column label="页面"></el-table-column>
                                <el-table-column label="动作">
                                    <template slot-scope="scope">

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
import DialogBox from "@/components/page/dialog/dialog-box.vue";

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
    components: { DialogBox }
})
export default class extends Vue {
    @Action editPrivilege;
    @Action getPageActions;
    @State getPageActionsData;

    @Prop({ type: Boolean, default: false })
    isShow;

    formData = {
        ...defaultFormData.formData
    }; // 表单传入数据

    onGetFormData(item) {
        console.log("itemitemitem", item);
        this.getPageActions({ ID: item.ID });
    }
}
</script>
