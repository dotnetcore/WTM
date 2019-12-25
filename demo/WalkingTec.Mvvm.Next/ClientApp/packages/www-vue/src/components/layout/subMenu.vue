<template functional>
  <a-sub-menu :key="props.menuInfo.key" :itemKey="props.menuInfo.key">
    <span slot="title">
      <a-icon type="mail" />
      <span>{{ props.menuInfo.name }}</span>
    </span>
    <template v-for="item in props.menuInfo.children">
      <a-menu-item v-if="!item.children.length" :key="item.key" :itemKey="item.key">
        <router-link :to="item.path">
          <a-icon :type="item.icon || 'pie-chart'" />
          <span>{{ item.name }}</span>
        </router-link>
      </a-menu-item>
      <sub-menu v-else :key="item.key" :menu-info="item" />
    </template>
  </a-sub-menu>
</template>
<script>
export default {
  props: ["menuInfo"]
};
</script>