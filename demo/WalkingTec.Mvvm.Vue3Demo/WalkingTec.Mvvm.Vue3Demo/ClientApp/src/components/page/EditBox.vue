<template>
  <div class="edit">
    <slot v-if="isEdit" />
    <template v-else>
      <img v-if="isImg" :src="value">
      <slot v-else name="editValue">
        <!--后备内容 -->
        {{ value }}
      </slot>
    </template>
  </div>
</template>

<script lang='ts'>
import { Component, Vue, Prop } from "vue-property-decorator";
/**
 * 自定义展示内容/图片展示
 *      调用：
 *      <template v-slot:editValue></template>
 */
@Component
export default class EditBox extends Vue {
    @Prop({ type: Boolean })
    isEdit;
    @Prop({ type: String, default: "" })
    value;
    @Prop({ type: Boolean })
    isImg;
}
</script>
<style lang='less' scoped>
.edit {
    display: inline-block;
}
</style>
