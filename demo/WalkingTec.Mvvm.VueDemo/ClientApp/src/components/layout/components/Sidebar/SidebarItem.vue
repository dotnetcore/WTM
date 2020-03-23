<template>
  <div
    v-if="!item.meta || !item.meta.hidden"
    :class="[
      'menu-wrapper',
      isCollapse ? 'simple-mode' : 'full-mode',
      { 'first-level': isFirstLevel }
    ]"
  >
    <template
      v-if="!alwaysShowRootMenu && theOnlyOneChild && !theOnlyOneChild.children"
    >
      <sidebar-item-link
        v-if="theOnlyOneChild.meta"
        :to="resolvePath(theOnlyOneChild.path)"
      >
        <el-menu-item
          :index="resolvePath(theOnlyOneChild.path)"
          :class="{ 'submenu-title-noDropdown': isFirstLevel }"
        >
          <i
            :class="[theOnlyOneChild.meta.icon || 'el-icon-files']"
            class="i-icon"
          ></i>
          <span v-if="theOnlyOneChild.meta.title" slot="title">{{
            $t("route." + theOnlyOneChild.meta.title)
          }}</span>
        </el-menu-item>
      </sidebar-item-link>
    </template>
    <el-submenu v-else :index="resolvePath(item.path)" popper-append-to-body>
      <template slot="title">
        <i :class="[item.meta.icon || 'el-icon-files']" class="i-icon"></i>
        <span v-if="item.meta && item.meta.title" slot="title">
          {{ $t("route." + item.meta.title) }}
        </span>
      </template>
      <template v-if="item.children">
        <sidebar-item
          v-for="child in item.children"
          :key="child.path"
          :item="child"
          :is-collapse="isCollapse"
          :is-first-level="false"
          :base-path="resolvePath(child.path)"
          class="nest-menu"
        />
      </template>
    </el-submenu>
  </div>
</template>

<script lang="ts">
import path from "path";
import { Component, Prop, Vue } from "vue-property-decorator";
import { Route } from "vue-router";
import { isExternal } from "@/util/validate";
import SidebarItemLink from "./SidebarItemLink.vue";

@Component({
  name: "SidebarItem",
  components: {
    SidebarItemLink
  }
})
export default class extends Vue {
  @Prop({ required: true })
  private item;
  @Prop({ default: false })
  private isCollapse!: boolean;
  @Prop({ default: true })
  private isFirstLevel!: boolean;
  @Prop({ default: "" })
  private basePath!: string;

  get alwaysShowRootMenu() {
    if (this.item.meta && this.item.meta.alwaysShow) {
      return true;
    }
    return false;
  }

  get showingChildNumber() {
    if (this.item.children) {
      const showingChildren = this.item.children.filter(item => {
        if (item.meta && item.meta.hidden) {
          return false;
        } else {
          return true;
        }
      });
      return showingChildren.length;
    }
    return 0;
  }

  get theOnlyOneChild() {
    if (this.showingChildNumber > 1) {
      return null;
    }
    if (this.item.children) {
      for (let child of this.item.children) {
        if (!child.meta || !child.meta.hidden) {
          return child;
        }
      }
    }
    const { path, name, meta, component } = this.item;
    return {
      path,
      name,
      meta,
      component
    };
  }

  private resolvePath(routePath: string) {
    if (isExternal(routePath)) {
      return routePath;
    }
    if (isExternal(this.basePath)) {
      return this.basePath;
    }
    return path.resolve(this.basePath, routePath);
  }
}
</script>

<style lang="less">
@import "~@/assets/css/variable.less";
.el-submenu__title:hover,
.el-menu-item:hover {
  background-color: @subMenuHover !important;
}

.el-submenu.is-active > .el-submenu__title {
  color: @subMenuActiveText !important;
}

.full-mode {
  .nest-menu .el-submenu > .el-submenu__title,
  .el-submenu .el-menu-item {
    min-width: @sideBarWidth !important;
    // background-color: @subMenuBg !important;
  }
}

.simple-mode {
  &.first-level {
    .submenu-title-noDropdown {
      padding: 0 !important;
      position: relative;

      .el-tooltip {
        padding: 0 !important;
      }
    }

    .el-submenu {
      overflow: hidden;

      & > .el-submenu__title {
        padding: 0px !important;

        .el-submenu__icon-arrow {
          display: none;
        }

        & > span {
          visibility: hidden;
        }
      }
    }
  }
}
.el-menu--popup-right-start {
  background-color: #545c64 !important;
}
</style>

<style lang="less" scoped>
.icon() {
  width: 1em;
  height: 1em;
  margin-right: 16px;
  color: #909399;
}

.el-menu-item.is-active {
  .i-icon {
    .icon();
  }
}
.i-icon {
  .icon();
}
.svg-icon {
  margin-right: 16px;
}

.simple-mode {
  .svg-icon {
    margin-left: 20px;
  }
  .i-icon {
    margin-left: 20px;
  }
}
</style>
