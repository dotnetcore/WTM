<template>
    <wtm-dialog-box :is-show.sync="isShow" :status="status" :events="formEvent">
        <wtm-create-form :ref="refName" :status="status" :options="formOptions" ></wtm-create-form>
    </wtm-dialog-box>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import formMixin from "@/vue-custom/mixin/form-mixin";
import UploadImg from "@/components/page/UploadImg.vue";
import { TDbTypeTypes } from "../config";

@Component({
    mixins: [formMixin()]
})
export default class Index extends Vue {
    @Action
    getFrameworkRoles;

    @State
    getFrameworkRolesData;
    // 表单结构
    get formOptions() {
        const filterMethod = (query, item) => {
            return item.label.indexOf(query) > -1;
        };
        return {
            formProps: {
                "label-width": "100px"
            },
            formItem: {
                "Entity.ID": {
                    isHidden: true
                },
             "Entity.TCode":{
                 label: "租户编号",
                 rules: [{ required: true, message: "租户编号"+this.$t("form.notnull"),trigger: "blur" }],
                    type: "input"
            },
             "Entity.TName":{
                 label: "租户名称",
                 rules: [{ required: true, message: "租户名称"+this.$t("form.notnull"),trigger: "blur" }],
                    type: "input"
            },
             "Entity.TDb":{
                 label: "租户数据库",
                span:24,
                 rules: [],
                    type: "input"
            },
             "Entity.TDbType":{
                 label: "数据库类型",
                 rules: [],
                    type: "select",
                    children: TDbTypeTypes,
                    props: {
                        clearable: true
                    }
            },
             "Entity.DbContext":{
                 label: "数据库架构",
                 rules: [],
                    type: "input"
            },
             "Entity.TDomain":{
                 label: "租户域名",
                 rules: [],
                    type: "input"
            },
             "Entity.TenantCode":{
                 label: "租户",
                 rules: [],
                    type: "input"
            },
             "Entity.EnableSub":{
                 label: "允许子租户",
                 rules: [{ required: true, message: "允许子租户"+this.$t("form.notnull"),trigger: "blur" }],
                    type: "switch"
            },
             "Entity.Enabled":{
                 label: "启用",
                 rules: [{ required: true, message: "启用"+this.$t("form.notnull"),trigger: "blur" }],
                    type: "switch"
            },
           "AdminRoleCode": {
                   label: "启用2",
                    type: "select",
                    children: this.getFrameworkRolesData,
                    props: {
                        clearable: true
                    }
                }
            }
        };
    }
        created() {
        this.getFrameworkRoles();
    }

    beforeOpen() {

    }
}
</script>
