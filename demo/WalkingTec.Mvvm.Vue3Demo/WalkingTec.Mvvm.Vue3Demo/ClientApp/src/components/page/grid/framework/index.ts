import headerIcon from './headerIcon.vue';
import noRowsOverlay from './noRowsOverlay.vue';
import loadingOverlay from './loadingOverlay.vue';

export default {
    headerIcon,
    noRowsOverlay,
    loadingOverlay
}
/**
 * 枚举
 */
export enum frameworkComponents {
    /** 标题图标 */
    headerIcon = 'headerIcon',
    /** 空数据 */
    noRowsOverlay = 'noRowsOverlay',
    loadingOverlay = 'loadingOverlay',
}