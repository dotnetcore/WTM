<template>
    <el-dialog title="出库单明细" :visible.sync="isShow" :before-close="CloseCallback">
        <wtm-table-box height="300px" :attrs="{...searchAttrs, actionList}" :events="{...searchEvent, ...actionEvent}"> </wtm-table-box>
    </el-dialog>
</template>

<script lang='ts'>
import { Component, Vue, Prop } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import formMixin from "@/vue-custom/mixin/form-mixin";

import searchMixin from "@/vue-custom/mixin/search";
import actionMixin from "@/vue-custom/mixin/action-mixin";
import mixinForm from "@/vue-custom/mixin/form-mixin";

// 查询参数, table列 ★★★★★
import { TABLE_HEADER, ASSEMBLIES } from "./config";

@Component({
    name: "outorder",
    mixins: [mixinForm(), searchMixin(TABLE_HEADER), actionMixin(ASSEMBLIES)],
    components: {},
})
export default class extends Vue {
    @Action
    search1;

    @Action
    batchDelete1;

    get SEARCH_DATA() {
        return {
            formProps: {
                "label-width": "75px",
                inline: true,
            },
            formItem: {
                OutOrderId: {
                    label: "出库单号",
                    rules: [],
                    type: "select",
                    children: this.getOutOrderData,
                    props: {
                        clearable: true,
                        placeholder: "全部",
                    },
                },
                BookinfoId: {
                    label: "图书名称",
                    rules: [],
                    type: "select",
                    children: this.getBookinfoData,
                    props: {
                        clearable: true,
                        placeholder: "全部",
                    },
                },
                State: {
                    label: "状态",
                    rules: [],
                    type: "select",
                    children: StateTypes,
                    props: {
                        clearable: true,
                        placeholder: "全部",
                    },
                    isHidden: !this.isActive,
                },
            },
        };
    }

    onDelete(row) {
        this.onConfirm().then(() => {
            this.batchDelete1(new Array(row.ID))
                .then((res) => {
                    this["$notify"]({
                        title: "删除成功",
                        type: "success",
                    });
                    this["onHoldSearch"]();
                })
                .catch((err) => {
                    this["$notify"]({
                        title: err.response.data.Form,
                        type: "error",
                    });
                });
        });

        //this.batchDelete1(new Array(row.ID));

        //this["onHoldSearch"]();
    }

    onOpen() {
        console.log("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb");
    }

    mounted() {}

    CloseCallback() {
        this.$emit("update:is-show", false);
    }

    // /**
    //  * 查询接口 ★★★★★
    //  * @param params
    //  */
    // privateRequest(params) {
    //     console.log("11111111111111111111111111111");
    //     return this.search1(params);
    // }
}
</script>
