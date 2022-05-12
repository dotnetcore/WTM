

<template>
  <WtmFilter :PageController="PageController" @finish="onFinish" @reset="onReset">
    <WtmField entityKey="Password_Filter" />
    <WtmField entityKey="Email_Filter" />
    <WtmField entityKey="Name_Filter" />
    <template #more>
    <WtmField entityKey="Sex_Filter" />
    <WtmField entityKey="CellPhone_Filter" />
    <WtmField entityKey="Address_Filter" />
    <WtmField entityKey="ZipCode_Filter" />
    <WtmField entityKey="EnRollDate_Filter" />
    <WtmField entityKey="SelectedMajorStudent_MT_WtmsIDs_Filter" />
    <WtmField entityKey="CreateTime_Filter" />
    <WtmField entityKey="UpdateTime_Filter" />
    <WtmField entityKey="CreateBy_Filter" />
    <WtmField entityKey="UpdateBy_Filter" />
    </template>
  </WtmFilter>
</template>



<script lang="ts">
import { Vue, Options, Provide, Inject } from "vue-property-decorator";
import { StudentPageController,ExStudentPageController,ExStudentEntity } from "../../controller";
import language from '@/client/locales/languagesys';
@Options({ components: {} })
export default class extends Vue {
  @Provide({ reactive: true }) readonly PageController = ExStudentPageController;
  @Provide({ reactive: true }) readonly PageEntity = ExStudentEntity;
  @Provide({ reactive: true }) readonly formState = { 
        Password: '',
        Email: '',
        Name: '',
        Sex: undefined,
        CellPhone: '',
        Address: '',
        ZipCode: '',
        EnRollDate: undefined,
        SelectedMajorStudent_MT_WtmsIDs: [],
        CreateTime: undefined,
        UpdateTime: undefined,
        CreateBy: '',
        UpdateBy: '', 
  };
  //readonly EnumLocaleLabel = EnumLocaleLabel;

    get queryKey() {
        return 'modelsstudentindex';
    }
  get Pagination() {
    return this.PageController.Pagination;
  }
  async onFinish(values) {
    await this.Pagination.onLoading(values);
  }
  async onReset(values) {
    await this.Pagination.onLoading(values);
  }
  mounted() { }
  created(){
    
}
}
</script>

