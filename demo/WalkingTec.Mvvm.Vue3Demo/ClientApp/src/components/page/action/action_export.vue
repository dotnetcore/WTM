<template>
  <a-dropdown v-if="isExport">
    <template #overlay>
      <a-menu @click="onMenuClick">
        <a-menu-item :key="EnumActionType.Export" v-if="onAuthority(EnumActionType.Export)">
          <i18n-t :keypath="$locales.action_exportAll" />
        </a-menu-item>
        <a-menu-item :key="EnumActionType.ExportIds" v-if="onAuthority(EnumActionType.ExportIds)">
          <i18n-t :keypath="$locales.action_exportSelect" />
        </a-menu-item>
      </a-menu>
    </template>
    <a-button v-bind="ButtonProps">
      <template #icon v-if="isPageAction">
        <CloudDownloadOutlined />
      </template>
      <i18n-t :keypath="$locales.action_export" />
      <DownOutlined />
    </a-button>
  </a-dropdown>
</template>
<script lang="ts">
import { Vue, Options, mixins, Prop } from "vue-property-decorator";
import { ControllerBasics } from "@/client";
import { ActionBasics } from "./script";
@Options({ components: {} })
export default class extends mixins(ActionBasics) {
  @Prop() readonly PageController: ControllerBasics;
  onExport() {
    this.PageController.onExport()
  }
  onMenuClick(event) {
    console.log("LENG ~ extends ~ onMenuClick ~ event", event)

  }
  created() { }
  mounted() { }
}
</script>
<style lang="less">
</style>
