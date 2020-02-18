
import Vue from 'vue';
import directive from './directive';
import grid from './grid/index.vue';
import layout from './layout/index.vue';
import filterForm from './pages/filterForm.vue';
import actions from './pages/actions.vue';
import avatar from './upload/avatar.vue';
import { createFormItem, renderFormItem, createFilterFormItem } from './utils/entitiesHelp';
function install(vue: typeof Vue) {
    directive.install(vue);
    vue.component('w-grid', grid);
    vue.component('w-layout', layout);
    vue.component('w-filter', filterForm);
    vue.component('w-actions', actions);
    vue.component('w-avatar', avatar);
}
export default {
    install,
    createFilterFormItem,
    createFormItem,
    renderFormItem
}