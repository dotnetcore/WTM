<template>
    <wtm-dialog-box :is-show.sync="isShow" :status="status" :events="formEvent">
        <wtm-create-form :ref="refName" :status="status" :options="formOptions" >
            <template #temptext>
                {{$t("frameworktenant.temptext")}}
        </template>
        </wtm-create-form>
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
                 label: this.$t("frameworktenant.TCode"),
                 rules: [{ required: true, message: this.$t("frameworktenant.TCode")+this.$t("form.notnull"),trigger: "blur" }],
                    type: "input"
            },
             "Entity.TName":{
                 label: this.$t("frameworktenant.TName"),
                 rules: [{ required: true, message: this.$t("frameworktenant.TName")+this.$t("form.notnull"),trigger: "blur" }],
                    type: "input"
            },
             "Entity.TDb":{
                 label: this.$t("frameworktenant.TDb"),
                span:24,
                 rules: [],
                    type: "input"
            },
             "Entity.TDbType":{
                 label: this.$t("frameworktenant.TDbType"),
                 rules: [],
                    type: "select",
                    children: TDbTypeTypes,
                    props: {
                        clearable: true
                    }
            },
             "Entity.DbContext":{
                 label: this.$t("frameworktenant.DbContext"),
                 rules: [],
                    type: "input"
            },
             "Entity.TDomain":{
                 label: this.$t("frameworktenant.TDomain"),
                   span:24,
              rules: [],
                    type: "input"
            },
             "Entity.EnableSub":{
                 label: this.$t("frameworktenant.EnableSub"),
                 rules: [{ required: true, message: this.$t("frameworktenant.EnableSub")+this.$t("form.notnull"),trigger: "blur" }],
                    type: "switch"
            },
             "Entity.Enabled":{
                 label: this.$t("frameworktenant.Enabled"),
                 rules: [{ required: true, message:this.$t("frameworktenant.Enabled")+this.$t("form.notnull"),trigger: "blur" }],
                    type: "switch"
            },
           "AdminRoleCode": {
                   label: this.$t("frameworktenant.AdminRoleCode"),
                    type: "select",
                    children: this.getFrameworkRolesData,
                    props: {
                        clearable: true
                    }
                },
                   "temptext": {
                    type: "wtmSlot",
                  span:24,
                  slotKey: "temptext"
            },

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
