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
      <!-- <a-form-item label="Note" :label-col="{ span: 5 }" :wrapper-col="{ span: 12 }">
        <a-input
          v-decorator="['note', { rules: [{ required: true, message: 'Please input your note!' }] }]"
        />
    </a-form-item> -->
    </template>
    <template #Update>
      <Entity-ID v-show="false" />
      <Entity-ITCode disabled />
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
      <Entity-ITCode display />
      <!-- <Entity-Password display /> -->
      <Entity-Email display />
      <Entity-Name display />
      <Entity-Sex display />
      <Entity-CellPhone display />
      <Entity-HomePhone display />
      <Entity-Address display />
      <Entity-ZipCode display />
      <Entity-PhotoId display />
      <Entity-IsValid display />
      <Entity-UserRoles display />
      <Entity-UserGroups display />
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