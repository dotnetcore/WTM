
<template>
  <WtmGrid :PageController="PageController" :columnDefs="columnDefs" :gridOptions="gridOptions" />
</template>



<script lang="ts">
import { ColDef, ColGroupDef, GridOptions } from "ag-grid-community";
import { Inject, Options,Provide,Vue } from "vue-property-decorator";
import { StudentPageController,ExStudentPageController } from "../../controller";
import language from '@/client/locales/languagesys';
import RowAction from "./actionViewIndex.vue";
@Options({ components: {} })
export default class extends Vue {
  @Provide({ reactive: true }) readonly PageController = ExStudentPageController;
  get columnDefs(): (ColDef | ColGroupDef)[] {
    return [
      {
        headerName:'Page密码',
        field: "Student_Password",
      },
      {
        headerName:'Page邮箱',
        field: "Student_Email",
      },
      {
        headerName:'Page姓名',
        field: "Student_Name",
      },
      {
        headerName:'Page性别',
        field: "Student_Sex",
      },
      {
        headerName:'Page手机',
        field: "Student_CellPhone",
      },
      {
        headerName:'Page住址',
        field: "Student_Address",
      },
      {
        headerName:'Page邮编',
        field: "Student_ZipCode",
      },
      {
        headerName:'Page是否有效',
        field: "Student_IsValid",
        cellRenderer: this.$FrameworkComponents.switch,
      },
      {
        headerName:'Page日期',
        field: "Student_EnRollDate",
      },
      {
        headerName:'Page专业',
        field: "Student_StudentMajor",
      },
      {
        headerName:'Page创建时间',
        field: "Student_CreateTime",
      },
      {
        headerName:'Page修改时间',
        field: "Student_UpdateTime",
      },
      {
        headerName:'Page创建人',
        field: "Student_CreateBy",
      },
      {
        headerName:'Page修改人',
        field: "Student_UpdateBy",
      },

    ]
  };
    get queryKey() {
        return 'modelsstudentindex';
    }
  get gridOptions(): GridOptions {
    return {
      frameworkComponents: {
        RowAction: this.__wtmToRowAction(RowAction, this.PageController),
      },
    };
  }
  created() { 
      
  }
}
</script>

