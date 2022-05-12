
<template>
  <FilterViewIndexStudentSearcher />

  <ActionViewIndex />
  <a-divider />
  <GridViewIndex />


  <WtmView queryKey="modelsstudentcreate" title="新建">
    <Create @refreshGrid="refreshGridStudent"/>
  </WtmView>

  <WtmView queryKey="modelsstudentedit" title="修改">
    <Edit @refreshGrid="refreshGridStudent"/>
  </WtmView>

  <WtmView queryKey="modelsstudentdetails" title="详情">
    <Details @refreshGrid="refreshGridStudent"/>
  </WtmView>

  <WtmView queryKey="modelsstudentbatchedit" title="批量修改">
    <BatchEdit @refreshGrid="refreshGridStudent"/>
  </WtmView>

  <WtmView queryKey="modelsstudentimport" title="导入">
    <Import @refreshGrid="refreshGridStudent"/>
  </WtmView>
</template>


<script lang="ts">
import {Inject,mixins,Options,Provide,Vue } from "vue-property-decorator";
import {$locales} from "@/client";
import {ExStudentPageController,} from "./controller";
import FilterViewIndexStudentSearcher from "./views/widgets/filterViewIndexStudentSearcher.vue";
import ActionViewIndex from "./views/widgets/actionViewIndex.vue";
import GridViewIndex from "./views/widgets/gridViewIndex.vue";
import Create from "@/pages/_admin/student/views/create.vue";
import Edit from "@/pages/_admin/student/views/edit.vue";
import Details from "@/pages/_admin/student/views/details.vue";
import BatchEdit from "@/pages/_admin/student/views/batchEdit.vue";
import Import from "@/pages/_admin/student/views/import.vue";
@Options({
  name: "WalkingTec.Mvvm.Admin.Api,Student",
    components: {
    FilterViewIndexStudentSearcher,
    ActionViewIndex,
    GridViewIndex,
    Create,
    Edit,
    Details,
    BatchEdit,
    Import,
  },
})
export default class extends Vue {

    readonly ExStudentPageController = ExStudentPageController;

    
    get queryKey() {
      return "modelsstudentindex";
    }

    
    refreshGridStudent() {
        this.ExStudentPageController.Pagination.onCurrentChange({ current: 1 })
    }
    created() {}
    mounted() {}
}
</script>
<style lang="less">
</style>

