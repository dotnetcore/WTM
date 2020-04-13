<template>
  <w-actions
    ref="action"
    :PageStore="PageStore"
    :Entities="Entities"
    :params="params"
    @submitPrivilege="onSubmitPrivilege"
    width="700px"
  >
    <template #Insert>
      <Entity-RoleCode />
      <Entity-RoleName />
      <Entity-RoleRemark />
    </template>
    <template #Update>
      <Entity-ID v-show="false" />
      <Entity-RoleCode />
      <Entity-RoleName />
      <Entity-RoleRemark />
    </template>
    <template #Details>
      <Entity-RoleCode display/>
      <Entity-RoleName display/>
      <Entity-RoleRemark display/>
    </template>
    <template #Privilege>
      <Entity-ID v-show="false" />
      <Entity-RoleCode display/>
      <Entity-RoleName display/>
      <Entity-RoleRemark display/>
    </template>
    <template #rowActionRight>
      <a-button type="link" size="small" @click="onPrivilege()">
        <a-icon type="unlock" />
      </a-button>
    </template>
  </w-actions>
</template>
<script lang="ts">
import { WrappedFormUtils } from "ant-design-vue/types/form/form";
import { Component, Prop, Vue, Inject } from "vue-property-decorator";
import { ICellRendererParams } from "ag-grid-community";
import { Modal } from "ant-design-vue";
import lodash from "lodash";
import { Subject } from "rxjs";
import wtm, { ViewAction } from "../../../components";
import PageStore from "../store";
import Entities from "./entities";
const entities = Entities.editEntities();
@Component({
  components: wtm.createFormItem({ entities })
})
export default class PageViewAction extends Vue {
  @Inject("PageStore")
  PageStore: PageStore;
  Entities = entities;
  // FieldsChange = new Subject<{
  //   props: any;
  //   fields: any;
  //   form: WrappedFormUtils;
  // }>();
  // aggird 组件 自带属性 不可删除
  params = {};
  /**
   * 获取 w-actions 组件对象
   */
  get action(): ViewAction {
    return this.$refs.action as ViewAction;
  }
  /** @fieldsChange="onFieldsChange" form onFieldsChange 事件 */
  onFieldsChange(props, fields) {}
  /** @submit="onSubmit" 替换 默认 提交函数 */
  onSubmit(value, type, v) {}
  /**
   * 配置权限 事件 参数 操作  w-actions 组件对象
   */
  onPrivilege() {
    this.action.title = "action.privilege";
    this.action.slotName = "Privilege";
    this.action.onVisible(true);
    this.action.onGetDetails()
  }
  /**
   *
   */
  onSubmitPrivilege(value, Callback) {
    console.log(
      "PageViewAction -> onSubmitPrivilege -> value",
      value,
      Callback
    );
    // Callback();
  }
  mounted() {
    // console.log(this);
  }
}
</script>
<style scoped lang="less"></style>
