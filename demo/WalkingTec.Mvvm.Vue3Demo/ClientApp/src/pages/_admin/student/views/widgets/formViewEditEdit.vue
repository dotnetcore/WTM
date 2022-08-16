<template>
  <WtmDetails :queryKey="queryKey" :loading="Entities.loading" :onFinish="onFinish">
    <a-row :gutter="6">
      <a-col :span="12">
      <WtmField entityKey="PasswordEdit"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="EmailEdit"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="NameEdit"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="SexEdit"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="CellPhoneEdit"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="AddressEdit"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="ZipCodeEdit"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="PhotoIdEdit"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="FileIdEdit"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="IsValidEdit"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="EnRollDateEdit"/>
      </a-col>
    </a-row>

      <a-row :gutter="6">
        <a-col :span="24">
        <WtmField entityKey="SelectedMajorStudent_MT_WtmsIDsEdit"/>
        </a-col>
      </a-row>
      <div style="text-align:right;"></div>
      <template #button>
         <a-divider type="vertical" />
         <a-button type="primary" html-type="submit">
             <template v-slot:icon>
                 <SaveOutlined style='margin-right:5px;' />
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
  
  get queryKey() {
    return "modelsstudentedit";
  }
  @Provide({ reactive: true }) formState = {
      Entity: {
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
      await this.PageController.onUpdate(this.formState);
      await this.onRefreshGrid();
  }
  
  async onLoading() {
      await this.Entities.onLoading({ ID: this.lodash.get(this.$route.query, this.queryKey) });
      this.formState = this.lodash.assign({}, this.formState, this.Entities.dataSource)
  }
  
  
  created() {}
  mounted() {
    this.onLoading();
  }
}
</script>

