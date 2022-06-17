<template>
  <el-dropdown
    id="tenant-select"
    trigger="click"
    @command="handleSetTenant"
    v-show="tenantOptions.length > 0"
  >
    <el-tooltip :content="$t('navbar.tenant')" effect="dark" placement="bottom">
      <i class="el-icon-s-custom"></i>
    </el-tooltip>
    <el-dropdown-menu slot="dropdown">
      <el-dropdown-item
        v-for="item of tenantOptions"
        :key="item.Value"
        :command="item.Value"
        :disabled="code === item.Value"
      >
        {{ item.Text }}
      </el-dropdown-item>
    </el-dropdown-menu>
  </el-dropdown>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import setAction from "@/service/modules/setting";
import { UserModule } from "@/store/modules/user";

@Component({
  name: "TenantSelect"
})
export default class extends Vue {
  private tenantOptions = [];

  get code() {
    return UserModule.currentTenant;
  }
  mounted() {
    setAction
      .getFrameworkTenants({ parent: UserModule.tenantCode })
      .then(res => {
        console.log("getFrameworkTenants", res, UserModule.currentTenant);
        this.tenantOptions =
          res.length > 0
            ? [
                {
                  Value: UserModule.tenantCode,
                  Text: this.$t("navbar.mainHost")
                },
                ...res
              ]
            : res;
      });
  }

  private handleSetTenant(tenant: string) {
    setAction.setTenants({ tenant }).then(() => {
      location.reload();
    });
  }
}
</script>
<style lang="less" scoped>
i {
  font-size: 16px;
}
</style>
