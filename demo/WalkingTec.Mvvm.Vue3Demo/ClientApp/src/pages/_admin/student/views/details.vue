
<template>
  <div>
    <FormViewDetailsDetail @refreshGrid="refreshGridStudent" />

  </div>

</template>


<script lang="ts">
import {Inject,mixins,Options,Provide,Vue } from "vue-property-decorator";
import {$locales} from "@/client";
import { ExStudentPageController,} from "../controller";
import FormViewDetailsDetail from "./widgets/formViewDetailsDetail.vue";
@Options({
    name: "_PageModelsStudentDetails",  
    components: {
    FormViewDetailsDetail,
  },
})
export default class extends Vue {

    readonly ExStudentPageController = ExStudentPageController;

    
    get queryKey() {
      return "modelsstudentdetails";
    }

    
    refreshGridStudent() {
        this.$emit('refreshGrid');
    }
    created() {}
    mounted() {}
}
</script>
<style lang="less">
</style>

