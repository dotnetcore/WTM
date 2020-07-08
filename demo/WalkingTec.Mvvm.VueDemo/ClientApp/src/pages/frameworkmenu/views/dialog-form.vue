<template>
    <wtm-dialog-box :is-show.sync="isShow" :status="status" :events="formEvent">
        <wtm-create-form :ref="refName" :status="status" :options="formOptions" :sourceFormData="formData">
            <template #ICon="data">
                <wtm-icon v-if="data.status === $actionType.detail" :icon="data.data" />
                <template v-else>
                    <el-select v-model="formData.Entity.IConType" clearable class="icon-select-type" @change="formData.Entity.ICon = ''">
                        <el-option v-for="(item, index) in iconList" :key="index" :label="item.name" :value="item.name"></el-option>
                    </el-select>
                    <el-select v-model="formData.Entity.ICon" filterable clearable>
                        <template v-for="fontItem in iconList">
                            <el-option v-if="formData.Entity.IConType === fontItem.name || !formData.Entity.IConType" v-for="(item, index) in fontItem.icons" :key="fontItem.name + index" :label="item" :value="item">
                                <span style="float: left">{{ item }}</span>
                                <span style="float: right; font-size: 14px"><i :class="[fontItem.class, item]"></i></span>
                            </el-option>
                        </template>
                    </el-select>
                </template>
            </template>
        </wtm-create-form>
    </wtm-dialog-box>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import mixinForm from "@/vue-custom/mixin/form-mixin";
import { RoutesModule } from "@/store/modules/routes";
import fonts from "@/assets/font/font.ts";
import { isExternal } from "@/util/validate";
import config from "@/config/index";

@Component({ mixins: [mixinForm()] })
export default class Index extends Vue {
    @Action
    getActionsByModel;
    @Action
    getFolders;
    @State
    getActionsByModelData;
    @State
    getFoldersData;

    formData = {
        SelectedModule: "",
        SelectedActionIDs: [],
        Entity: {
            ID: "",
            IsInside: true,
            Url: "",
            PageName: "",
            ParentId: "",
            FolderOnly: false,
            ShowOnMenu: true,
            IsPublic: false,
            DisplayOrder: 0,
            IConType: "",
            ICon: ""
        }
    };

    iconList: Array<any> = fonts;

    get formOptions() {
        const moduleChildren = RoutesModule.pageList.map(item => {
            return {
                ...item,
                Text: this.$t(`route.${item.Text}`)
            };
        });
        return {
            formProps: {
                "label-width": this.$t("frameworkmenu.LabelWidth")
            },
            formItem: {
                "Entity.ID": {
                    isHidden: true
                },
                "Entity.IsInside": {
                    type: "radioGroup",
                    label: this.$t("frameworkmenu.RadioGroup"),
                    span: 24,
                    children: [
                        {
                            Value: true,
                            Text: this.$t("frameworkmenu.InternalAddress")
                        },
                        {
                            Value: false,
                            Text: this.$t("frameworkmenu.ExternalAddress")
                        }
                    ],
                    defaultValue: true
                },
                SelectedModule: {
                    type: "select",
                    label: this.$t("frameworkmenu.SelectedModule"),
                    props: { clearable: true },
                    children: moduleChildren,
                    events: {
                        change: this.onSelectedAction
                    },
                    isHidden: data => !_.get(data, "Entity.IsInside")
                },
                SelectedActionIDs: {
                    type: "select",
                    label: this.$t("frameworkmenu.Action"),
                    props: { multiple: true, clearable: true },
                    children: this.getActionsByModelData,
                    isHidden: data => !_.get(data, "Entity.IsInside"),
                    defaultValue: []
                },
                "Entity.Url": {
                    type: "input",
                    label: "Url",
                    span: 24,
                    isHidden: data => _.get(data, "Entity.IsInside")
                },
                "Entity.PageName": {
                    type: "input",
                    label: this.$t("frameworkmenu.PageName")
                },
                "Entity.ParentId": {
                    type: "select",
                    label: this.$t("frameworkmenu.ParentDirectory"),
                    children: this.getFoldersData
                },
                "Entity.FolderOnly": {
                    type: "switch",
                    label: this.$t("frameworkmenu.Directory"),
                    defaultValue: false
                },
                "Entity.ShowOnMenu": {
                    type: "switch",
                    label: this.$t("frameworkmenu.ShowOnMenu"),
                    defaultValue: true
                },
                "Entity.IsPublic": {
                    type: "switch",
                    label: this.$t("frameworkmenu.Public"),
                    defaultValue: false
                },
                "Entity.DisplayOrder": {
                    type: "input",
                    label: this.$t("frameworkmenu.DisplayOrder"),
                    defaultValue: 0,
                    rules: {
                        required: true,
                        message: this.$t(
                            "frameworkmenu.pleaseEnterDisplayOrder"
                        ),
                        trigger: "blur"
                    }
                },
                "Entity.ICon": {
                    type: "wtmSlot",
                    label: this.$t("frameworkmenu.ICon"),
                    span: 12,
                    slotKey: "ICon"
                }
            }
        };
    }

    /**
     * 查询详情-after-调用
     */
    afterOpen(data) {
        this.getFolders();
        this.onSelectedAction(data && data.SelectedModule);
        if (data) {
            const icon = _.findLast(fonts, item =>
                item.icons.includes(data.Entity.ICon)
            );
            this.formData.Entity.IConType = icon ? icon.name : "";
        }
    }
    /**
     * 动作名称
     */
    onSelectedAction(SelectedModule?: string) {
        this.getActionsByModel({
            ModelName: SelectedModule || ""
        });
    }

    beforeRequest(formData) {
        if (formData.Entity.IsInside) {
            const moduleObj = _.find(RoutesModule.pageList, {
                Value: formData.SelectedModule
            });
            formData.Entity.Url = moduleObj ? moduleObj.Url : "";
        } else {
            if (
                !isExternal(formData.Entity.Url) &&
                formData.Entity.Url &&
                formData.Entity.Url.indexOf(config.staticPage) !== 0
            ) {
                formData.Entity.Url = config.staticPage + formData.Entity.Url;
            }
        }
        return formData;
    }
}
</script>

<style lang="less" scoped>
.icon-select-type {
    margin-bottom: 10px;
}
</style>
