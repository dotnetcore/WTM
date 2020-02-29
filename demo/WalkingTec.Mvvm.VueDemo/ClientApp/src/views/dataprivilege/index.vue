<template>
  <card class="dataprivilege">
    <wtm-search-box :events="searchEvent">
      <wtm-form-item label="权限名称" prop="car_model">
        <el-select v-model="searchForm.TableName" clearable multiple placeholder="请选择权限名称">
          <el-option v-for="(item,index) of getPrivilegesData" :key="index" :label="item.Text" :value="item.Value" />
        </el-select>
      </wtm-form-item>
      <wtm-form-item label="权限类型" prop="car_model" :span="8">
        <el-radio v-model="searchForm.DpType" label="0">
          用户组权限
        </el-radio>
        <el-radio v-model="searchForm.DpType" label="1">
          用户权限
        </el-radio>
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
import { Action, State } from "vuex-class";
import searchMixin from "@/vue-custom/mixin/search";
import actionMixin from "@/vue-custom/mixin/action-mixin";
import store from "@/store/system/dataprivilege";
import DialogForm from "./dialog-form.vue";
// 查询参数/列表 ★★★★★
import { ASSEMBLIES, SEARCH_DATA, TABLE_HEADER } from "./config";

@Component({
    mixins: [searchMixin(SEARCH_DATA, TABLE_HEADER), actionMixin],
    store,
    components: { DialogForm }
})
export default class Index extends Vue {
    @Action
    getPrivileges;
    @State
    getPrivilegesData;

    exportParams = {};
    // 动作
    assembly = ASSEMBLIES;

    created() {
        this.getPrivileges();
    }
}
</script>
