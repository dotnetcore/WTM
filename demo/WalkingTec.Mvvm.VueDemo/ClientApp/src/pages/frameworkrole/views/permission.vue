<template>
    <wtm-dialog-box :is-show.sync="isShow" :status="status" :events="formEvent">
        <wtm-create-form :ref="refName" :status="status" :options="formOptions">
            <template #Pages>
                <el-table :data="mergeFormData.Pages" stripe border :element-loading-text="$t('frameworkrole.loading')">
                    <el-table-column :label="$t('frameworkrole.page')" prop="Name" width="150"></el-table-column>
                    <el-table-column :label="$t('frameworkrole.action')">
                        <template slot-scope="scope">
                            <el-checkbox-group v-model="mergeFormData.Pages[scope.$index].Actions">
                                <el-checkbox v-for="item in mergeFormData.Pages[scope.$index].AllActions" :key="item.Value" :label="item.Value">{{ item.Text }}</el-checkbox>
                            </el-checkbox-group>
                        </template>
                    </el-table-column>
                </el-table>
            </template>
        </wtm-create-form>
    </wtm-dialog-box>
</template>

<script lang='ts'>
import { Component, Vue, Prop } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import formMixin from "@/vue-custom/mixin/form-mixin";
import { sexList } from "@/config/entity";

@Component({
    mixins: [formMixin()]
})
export default class extends Vue {
    @Action("editPrivilege")
    edit;
    @Action("getPageActions")
    detail;

    mergeFormData = {
        Pages: []
    };
    // 表单结构
    get formOptions() {
        return {
            formProps: {
                "label-width": "100px"
            },
            formItem: {
                "Entity.ID": {
                    isHidden: true
                },
                "Entity.RoleCode": {
                    type: "label",
                    label: this.$t("frameworkrole.RoleCode")
                },
                "Entity.RoleName": {
                    type: "label",
                    label: this.$t("frameworkrole.RoleName")
                },
                Pages: {
                    type: "wtmSlot",
                    label: this.$t("frameworkrole.RoleRemark"),
                    span: 24,
                    slotKey: "Pages"
                }
            }
        };
    }
}
</script>
