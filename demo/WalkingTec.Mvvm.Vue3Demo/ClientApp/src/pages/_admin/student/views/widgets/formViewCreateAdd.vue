
<template>
  <WtmDetails :queryKey="queryKey" :loading="Entities.loading" :onFinish="onFinish">
    <a-row :gutter="6">
       <a-col :span="12">
      <WtmField entityKey="IDAdd"/>
      </a-col>     <a-col :span="12">
      <WtmField entityKey="PasswordAdd"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="EmailAdd"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="NameAdd"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="SexAdd"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="CellPhoneAdd"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="AddressAdd"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="ZipCodeAdd"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="PhotoIdAdd"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="FileIdAdd"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="IsValidAdd"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="EnRollDateAdd"/>
      </a-col>
    </a-row>

      <a-row :gutter="6">
        <a-col :span="24">
        <WtmField entityKey="SelectedMajorStudent_MT_WtmsIDsAdd"/>
        </a-col>
      </a-row>

        <div style="text-align:right;">


        </div>

  <template #button>
  
   <a-divider type="vertical" />
   <a-button type="primary" html-type="submit">
       <template v-slot:icon>
           <SaveOutlined style='margin-right:5px'/>
       </template>
       <i18n-t keypath="提交" />
   </a-button>
   <a-divider type="vertical" />
   <a-button type="primary" @click.stop.prevent="__wtmBackDetails()">
       <template v-slot:icon>
           <RedoOutlined style='margin-right:5px'/>
       </template>
       <i18n-t keypath="关闭" />
   </a-button>
  </template>
  </WtmDetails>
</template>



<script lang="ts">
import { PageDetailsBasics } from "@/components";
import { Inject, mixins, Options, Provide } from "vue-property-decorator";
import { StudentPageController,ExStudentPageController,ExStudentEntity } from "../../controller";
import {$locales, $i18n} from "@/client";

@Options({ components: {} })
export default class extends mixins(PageDetailsBasics) {
  @Provide({ reactive: true }) readonly PageController = ExStudentPageController;
  @Provide({ reactive: true }) readonly PageEntity = ExStudentEntity;
  
  @Provide({ reactive: true }) formState = {
      Entity: {
        ID:'',
        Password: '',
        Email: '',
        Name: '',
        Sex: undefined,
        CellPhone: '',
        Address: '',
        ZipCode: '',
        PhotoId: undefined,
        FileId: undefined,
        IsValid: false,
        EnRollDate: undefined,
    },
    SelectedMajorStudent_MT_WtmsIDs: [],
  };
  
  async onFinish(values) {
      await this.PageController.onInsert(this.formState)
      await this.onRefreshGrid();
  }
  
  
  
  created() {}
  mounted() {
    this.onLoading();
  }
}
</script>

