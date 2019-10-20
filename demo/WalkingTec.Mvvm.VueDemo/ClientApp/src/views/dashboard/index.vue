<template>
    <div class="dashboard-container">
        <div class="dashboard-text">
            首页
        </div>
        <div class="dashboard-text">
            name:{{ info.Name }}
        </div>
        <!-- <component :is="currentRole" /> -->
    </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { UserModule } from "@/store/modules/user";

@Component({
    name: "Dashboard"
})
export default class extends Vue {
    get info() {
        return UserModule.info;
    }

    get roles() {
        return UserModule.roles;
    }
    // currentRole = "admin-dashboard";
    // get roles() {
    //     return "roles";
    // }
    // created() {
    //     if (!this.roles.includes("admin")) {
    //         this.currentRole = "editor-dashboard";
    //     }
    // }
}
</script>
