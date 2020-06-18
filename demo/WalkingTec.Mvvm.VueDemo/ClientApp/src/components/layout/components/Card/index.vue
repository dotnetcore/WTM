/** * 卡片布局 */
<template>
  <article class="common-card">
    <div v-if="$route.name" class="title">
      {{ $route.name }}
    </div>
    <div class="content">
      <slot />
    </div>
    <div v-if="$slots.footer" class="card-footer">
      <slot name="footer" />
    </div>
  </article>
</template>

<style lang="less" scoped>
.common-card {
  border-top: 3px solid #409eff;
  background-color: #ffffff;
  box-shadow: 0 1px 1px rgba(0, 0, 0, 0.1);
  padding: 10px;
  margin-bottom: 10px;
  border-radius: 3px;
  min-width: 300px;
  .title {
    font-size: 16px;
    font-weight: 500;
    margin-bottom: 10px;
  }
  .card-footer {
    border-top: 1px solid #f4f4f4;
    text-align: center;
    padding-top: 10px;
  }
}
</style>
