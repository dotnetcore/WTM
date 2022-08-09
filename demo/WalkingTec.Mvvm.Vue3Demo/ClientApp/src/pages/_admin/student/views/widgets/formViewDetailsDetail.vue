<template>
  <WtmDetails :queryKey="queryKey" :loading="Entities.loading" :onFinish="onFinish">
    <a-row :gutter="6">
      <a-col :span="12">
      <WtmField entityKey="PasswordDetail"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="EmailDetail"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="NameDetail"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="SexDetail"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="CellPhoneDetail"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="AddressDetail"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="ZipCodeDetail"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="PhotoIdDetail"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="FileIdDetail"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="IsValidDetail"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="EnRollDateDetail"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="SelectedMajorStudent_MT_WtmsIDsDetail"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="CreateTimeDetail"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="UpdateTimeDetail"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="CreateByDetail"/>
      </a-col>
      <a-col :span="12">
      <WtmField entityKey="UpdateByDetail"/>
      </a-col>
    </a-row>

      <div style="text-align:right;">
      </div>

  <template #button v-if="JumpType =='' || JumpType=='Dialog'">
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
    return "modelsstudentdetails";
  }

  get JumpType() {
    return this.lodash.get(this.$route.query, 'type') || null
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
        CreateTime: undefined,
        UpdateTime: undefined,
        CreateBy: '',
        UpdateBy: '',
    },
    SelectedMajorStudent_MT_WtmsIDs: [],
  };
  
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

