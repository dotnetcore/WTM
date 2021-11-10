<template>
  <WtmView
    queryKey="changePassword"
    :fixedPage="false"
    :title="$t($locales.action_user_changePassword)"
  >
    <WtmDetails queryKey="changePassword" :onFinish="onFinish">
      <WtmField
        name="OldPassword"
        :valueType="EnumValueType.password"
        :label="$locales.update_pwd_old"
        :rules="[{ required: true }]"
      />
      <WtmField
        name="NewPassword"
        :valueType="EnumValueType.password"
        :label="$locales.update_pwd_new"
        :rules="[{ required: true }]"
      />
      <WtmField
        name="NewPasswordComfirm"
        :valueType="EnumValueType.password"
        :label="$locales.update_pwd_confirm"
        :rules="[{ required: true }, { validator }]"
      />
    </WtmDetails>
  </WtmView>
</template>
<script lang="ts">
import { SystemController } from "@/client";
import { Vue, Options, Inject, Provide } from "vue-property-decorator";
// Component definition
@Options({ components: {} })
export default class extends Vue {
  /**
   * 从 Aapp 中 注入 系统管理
   */
  @Inject({ from: SystemController.Symbol }) System: SystemController;
  @Provide({ reactive: true }) formState = {
    OldPassword: undefined,
    NewPassword: undefined,
    NewPasswordComfirm: undefined
  };
  async validator(rule, value) {
    if (value && value !== this.formState.NewPassword) {
      throw this.$t(this.$locales.update_pwd_inconsistent);
    } else {
      return true;
    }
  }
  onFinish(formState) {
    return this.System.UserController.onChangePassword(formState);
  }
  created() {}
  mounted() {}
}
</script>
<style lang="less"></style>
