
import Vue from 'vue';
import directive from './directive';
import grid from './grid/index.vue';
import layout from './layout/index.vue';
import filterForm from './pages/filterForm.vue';
import actions from './pages/actions.vue';
import { createFormItem, renderFormItem } from './utils/entitiesHelp';
function install(vue: typeof Vue) {
    directive.install(vue);
    vue.component('w-grid', grid);
    vue.component('w-layout', layout);
    vue.component('w-filter', filterForm);
    vue.component('w-actions', actions);
}
export default {
    install,
    createFormItem,
    renderFormItem
}