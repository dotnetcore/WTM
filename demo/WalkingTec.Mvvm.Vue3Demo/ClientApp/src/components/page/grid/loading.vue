<template>
  <a-row :gutter="16" v-for="item in 5" :key="item">
    <a-col :span="6">
      <a-skeleton :paragraph="false" />
    </a-col>
    <a-col :span="6">
      <a-skeleton :paragraph="false" />
    </a-col>
    <a-col :span="6">
      <a-skeleton :paragraph="false" />
    </a-col>
    <a-col :span="6">
      <a-skeleton :paragraph="false" />
    </a-col>
  </a-row>
</template>

<script lang="ts">
import { Options,Prop,Vue } from "vue-property-decorator";
@Options({
  components: {},
})
export default class extends Vue {
  @Prop({ default: false }) loading;
  @Prop({ default: "material" }) theme;
  @Prop({ default: () => [] }) columnDefs;
  @Prop({}) rowData;
  @Prop({ default: () => ({}) }) gridOptions;
  created() {}
}
</script>
<style  lang="less">
</style>
