<template>
  <span>
    <a-select v-model="selectIcon" @change="onChangeType">
      <a-select-option v-for="item in Fonts" :key="item.class">
        <span v-text="item.name" />
      </a-select-option>
    </a-select>
    <a-select v-model="iconValue" @change="onChange">
      <a-select-option v-for="item in icons" :key="item">
        <a-icon :type="onGetIcon(item)" style="font-size: 20px;" /> <span v-text="item" />
      </a-select-option>
    </a-select>
  </span>
</template>
<script lang="ts">
import { Component, Prop, Vue, Inject, Watch } from "vue-property-decorator";
import AntIcons from "@ant-design/icons/lib/manifest";
import Fonts from "@/assets/font/font.ts";
import lodash from "lodash";
Fonts.unshift({
  name: "Antd",
  class: "Antd",
  icons: AntIcons.fill
});
@Component
export default class extends Vue {
  @Prop() value;
  Fonts = Fonts;
  selectIcon = "Antd";
  icons = this.onGetItems("Antd");
  iconValue = lodash.head(this.icons);
  onChange(event) {
    // this.props.onChange(event);
    this.$emit("change", event);
  }
  onChangeType(event) {
    this.icons = this.onGetItems(event);
    this.onChange(lodash.head(this.icons));
  }
  onGetItems(iconType): string[] {
    return lodash.get(lodash.find(Fonts, ["class", iconType]), "icons", []);
  }
  onGetIcon(icon) {
    // 自定义 图标 需要 使用 name 和 class名称拼接，并且前面需要一个空格
    return this.selectIcon === "Antd" ? icon : ` ${this.selectIcon} ${icon}`;
  }
  @Watch("value")
  onChildChanged(val: string, oldVal: string) {
    if (!lodash.eq(this.iconValue, val)) {
      // 某人 懒得加字段。就自己用一个字段截取吧
      const fontClass = lodash.trim(val).split(" ");
      // 自定义
      if (fontClass.length > 1) {
        const iconType = fontClass[0];
        this.selectIcon = iconType;
        this.icons = this.onGetItems(iconType);
        this.iconValue = fontClass[1];
      } else {
        this.iconValue = val;
      }
    }
  }
  mounted() {}
};
@Component({
  template: `<a-icon v-if="type" :type="type" style="font-size: 28px;" />`
})
export class GridIcon extends Vue {
  get type() {
    return lodash.get(this, "params.value");
  }
}
</script>
<style scoped lang="less"></style>
