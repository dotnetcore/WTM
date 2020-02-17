<template>
  <w-actions :PageStore="PageStore" :Entities="Entities" :params="params">
    <template #Insert>
      <Entity-ITCode />
      <Entity-Password />
      <Entity-Email />
      <Entity-Name />
      <Entity-Sex />
      <Entity-CellPhone />
      <Entity-HomePhone />
      <Entity-Address />
      <Entity-ZipCode />
      <Entity-PhotoId />
      <Entity-IsValid v-show="false" />
      <Entity-UserRoles />
      <Entity-UserRoles2 />
      <Entity-UserRoles3 />
      <Entity-UserGroups />
    </template>
    <template #Update>
      <Entity-ID v-show="false" />
      <Entity-ITCode :disabled="true" />
      <!-- <Entity-Password v-show="false" /> -->
      <Entity-Email />
      <Entity-Name />
      <Entity-Sex />
      <Entity-CellPhone />
      <Entity-HomePhone />
      <Entity-Address />
      <Entity-ZipCode />
      <Entity-PhotoId />
      <Entity-IsValid />
      <Entity-UserRoles />
      <Entity-UserGroups />
    </template>
    <template #Details>
      <Entity-ITCode :display="true" />
      <Entity-Password :display="true" />
      <Entity-Email :display="true" />
      <Entity-Name :display="true" />
      <Entity-Sex :display="true" />
      <Entity-CellPhone :display="true" />
      <Entity-HomePhone :display="true" />
      <Entity-Address :display="true" />
      <Entity-ZipCode :display="true" />
      <Entity-PhotoId :display="true" />
      <Entity-IsValid :display="true" />
      <Entity-UserRoles :display="true" />
      <Entity-UserGroups :display="true" />
    </template>
  </w-actions>
</template> 
<script lang="ts">
import { WrappedFormUtils } from "ant-design-vue/types/form/form";
import { Component, Prop, Vue } from "vue-property-decorator";
import { ICellRendererParams } from "ag-grid-community";
import { Modal } from "ant-design-vue";
import lodash from "lodash";
import { Subject } from "rxjs";
import wtm from "../../../components";
import PageStore from "../store";
import Entities from "./entities";
const entities = Entities.editEntities();
@Component({
  components: wtm.createFormItem({ entities })
})
export default class ViewAction extends Vue {
  @Prop() private PageStore: PageStore;
  Entities = entities;
  // FieldsChange = new Subject<{
  //   props: any;
  //   fields: any;
  //   form: WrappedFormUtils;
  // }>();
  // aggird 组件 自带属性 不可删除
  params = {};
  mounted() {}
}
</script>
<style scoped lang="less">
</style>