<template>
  <a-button v-if="isInsert" v-bind="ButtonProps" @click="__wtmToDetails()">
    <template #icon v-if="isPageAction">
      <slot name="icon">
        <FormOutlined />
      </slot>
    </template>
    <slot>
      <i18n-t :keypath="$locales.action_insert" />
    </slot>
  </a-button>
</template>
<script lang="ts">
import { Vue, Options, mixins, Prop } from "vue-property-decorator";
import { $locales, ControllerBasics } from "@/client";
import { ActionBasics } from "./script";
@Options({ components: {} })
export default class extends mixins(ActionBasics) {
  @Prop() readonly PageController: ControllerBasics;
  created() {}
  mounted() {}
}
</script>
<style lang="less"></style>
