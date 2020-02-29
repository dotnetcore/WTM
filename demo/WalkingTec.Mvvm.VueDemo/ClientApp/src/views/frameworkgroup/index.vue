<template>
  <card class="dataprivilege">
    <wtm-search-box :events="searchEvent">
      <wtm-form-item label="用户组编码">
        <el-input v-model="searchForm.GroupCode" />
      </wtm-form-item>
      <wtm-form-item label="用户组名称">
        <el-input v-model="searchForm.GroupName" />
      </wtm-form-item>
    </wtm-search-box>
    <!-- 操作按钮 -->
    <wtm-but-box :assembly="assembly" :action-list="actionList" :selected-data="selectData" :events="actionEvent" />
    <!-- 列表 -->
    <wtm-table-box :attrs="{...searchAttrs, actionList}" :events="{...searchEvent, ...actionEvent}" />
    <!-- 弹出框 -->
    <dialog-form :is-show.sync="dialogIsShow" :dialog-data="dialogData" :status="dialogStatus" @onSearch="onHoldSearch" />
    <!-- 导入 -->
    <upload-box :is-show.sync="uploadIsShow" @onImport="onImport" @onDownload="onDownload" />
  </card>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import searchMixin from "@/vue-custom/mixin/search";
import actionMixin from "@/vue-custom/mixin/action-mixin";
import DialogForm from "./dialog-form.vue";
import store from "@/store/system/frameworkgroup";
// 查询参数/列表 ★★★★★
import { ASSEMBLIES, SEARCH_DATA, TABLE_HEADER } from "./config";

@Component({
    mixins: [searchMixin(SEARCH_DATA, TABLE_HEADER), actionMixin],
    store,
    components: {
        DialogForm
    }
})
export default class Index extends Vue {
    // 动作
    assembly = ASSEMBLIES;
}
</script>
