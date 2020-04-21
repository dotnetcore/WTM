<template>
  <w-actions
    :PageStore="PageStore"
    :Entities="Entities"
    :params="params"
    @fieldsChange="onFieldsChange"
  >
    <template #pageActionLeft>
      <a-button @click="onRefreshMenu">
        <a-icon type="reload" />
        <span v-text="RefreshMenuText" />
      </a-button>
      <a-divider type="vertical" />
    </template>
    <template #Insert>
      <Entity-FolderOnly />
      <Entity-ShowOnMenu />
      <Entity-IsInside :disabled="disabled.FolderOnly" />
      <SelectedModule :disabled="disabled.FolderOnly" />
      <SelectedActionIDs :disabled="disabled.FolderOnly" />
      <Entity-Url :disabled="disabled.FolderOnly" />
      <Entity-ICon />
    </template>
    <template #Update>
      <Entity-IsInside />
      <SelectedModule />
      <SelectedActionIDs />
      <Entity-Url />
      <Entity-ICon />
    </template>
    <!--<template #Details></template>-->
  </w-actions>
</template>
<script lang="ts">
import { WrappedFormUtils } from "ant-design-vue/types/form/form";
import { Component, Prop, Vue, Inject } from "vue-property-decorator";
import { ICellRendererParams } from "ag-grid-community";
import { Modal } from "ant-design-vue";
import lodash from "lodash";
import { Subject } from "rxjs";
import wtm from "../../../components";
import PageStore from "../store";
import { onGetController } from "../store";
import Entities from "./entities";
const entities = Entities.editEntities();
@Component({
  components: wtm.createFormItem({ entities })
})
export default class ViewAction extends Vue {
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
  disabled = {
    FolderOnly: false
  };
  display = {};
  /** @fieldsChange="onFieldsChange" form onFieldsChange 事件 */
  onFieldsChange(props, fields, form) {
    console.log("ViewAction -> onFieldsChange -> fields", fields);
    if (lodash.hasIn(fields, "SelectedModule")) {
      const SelectedModule = lodash.get(fields, "SelectedModule.value");
      onGetController().subscribe(data => {
        form.setFieldsValue({
          "Entity.Url": lodash.get(
            lodash.find(data, ["Value", SelectedModule]),
            "Url"
          )
        });
      });
    }
    if (lodash.hasIn(fields, "Entity.FolderOnly")) {
      const FolderOnly = lodash.get(fields, "Entity.FolderOnly.value");
      this.disabled.FolderOnly = FolderOnly;
    }
  }
  /** @submit="onSubmit" 替换 默认 提交函数 */
  onSubmit(value, type, v) {}
  mounted() {}
  /**
   * 刷新菜单事件
   */
  async onRefreshMenu() {
    try {
      await this.PageStore.onRefreshMenu();
      this.$message.success(`Success`);
      this.PageStore.onSearch({
        body: {
          Page: 1
        }
      });
    } catch (error) {
      this.$message.error(`Error`);
    }
  }
  get RefreshMenuText() {
    return lodash.get(
      { "zh-CN": "刷新菜单", "en-US": "RefreshMenu" },
      this.$i18n.locale
    );
  }
}
</script>
<style scoped lang="less"></style>
