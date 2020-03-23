<template>
    <wtm-dialog-box :is-show.sync="isShow" :status="status" :events="formEvent">
        <wtm-create-form :ref="refName" :status="status" :options="formOptions">
            <template #ICon="data">
                <i v-if="data.status === $actionType.detail" :class="[data.data]"></i>
                <el-select v-else v-model="mergeFormData.Entity.ICon" filterable placeholder="请选择">
                    <el-option v-for="(item, index) in iconList" :key="index" :label="item" :value="item">
                        <span style="float: left">{{ item }}</span>
                        <span style="float: right; font-size: 14px"><i :class="[item]"></i></span>
                    </el-option>
                </el-select>
            </template>
        </wtm-create-form>
    </wtm-dialog-box>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import mixinForm from "@/vue-custom/mixin/form-mixin";
import { RoutesModule } from "@/store/modules/routes";
import { iconList } from "../config";

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

    mergeFormData = {
        Entity: {
            ICon: ""
        }
    };
    get formOptions() {
        return {
            formProps: {
                "label-width": "100px"
            },
            formItem: {
                "Entity.ID": {
                    isHidden: true
                },
                "Entity.IsInside": {
                    type: "radioGroup",
                    label: "地址类型",
                    span: 24,
                    children: [
                        { Value: true, Text: "内部地址" },
                        { Value: false, Text: "外部地址" }
                    ],
                    defaultValue: true
                },
                SelectedModule: {
                    type: "select",
                    label: "模块名称",
                    children: RoutesModule.pageList,
                    events: {
                        change: this.onSelectedAction
                    },
                    isHidden: data => !_.get(data, "Entity.IsInside")
                },
                SelectedActionIDs: {
                    type: "select",
                    label: "动作名称",
                    props: { multiple: true },
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
                    label: "页面名称"
                },
                "Entity.ParentId": {
                    type: "select",
                    label: "父目录",
                    children: this.getFoldersData
                },
                "Entity.FolderOnly": {
                    type: "switch",
                    label: "目录",
                    defaultValue: false
                },
                "Entity.ShowOnMenu": {
                    type: "switch",
                    label: "菜单显示"
                },
                "Entity.IsPublic": {
                    type: "switch",
                    label: "公开",
                    defaultValue: false
                },
                "Entity.DisplayOrder": {
                    type: "input",
                    label: "顺序",
                    defaultValue: 0,
                    rules: {
                        required: true,
                        message: "请输入顺序",
                        trigger: "blur"
                    }
                },
                "Entity.ICon": {
                    type: "wtmSlot",
                    label: "图标",
                    span: 24,
                    slotKey: "ICon"
                }
            }
        };
    }

    iconList: Array<any> = iconList;

    created() {
        this.getFolders();
    }
    /**
     * 查询详情-after-调用
     */
    afterOpen(data) {
        this.onSelectedAction(data && data.SelectedModule);
    }
    /**
     * 动作名称
     */
    onSelectedAction(SelectedModule?: string) {
        this.getActionsByModel({
            ModelName: SelectedModule || ""
        });
    }
}
</script>
